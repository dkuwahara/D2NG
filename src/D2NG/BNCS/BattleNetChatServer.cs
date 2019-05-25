using D2NG.BNCS.Hashing;
using D2NG.BNCS.Packet;
using Serilog;
using Stateless;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace D2NG.BNCS
{
    internal class BattleNetChatServer
    {
        /***
         * Constants
         */
        private readonly string DefaultChannel = "Diablo II";

        private BncsConnection Connection { get; } = new BncsConnection();

        protected ConcurrentDictionary<Sid, Action<BncsPacket>> PacketReceivedEventHandlers { get; } = new ConcurrentDictionary<Sid, Action<BncsPacket>>();

        protected ConcurrentDictionary<Sid, Action<BncsPacket>> PacketSentEventHandlers { get; } = new ConcurrentDictionary<Sid, Action<BncsPacket>>();

        private readonly StateMachine<State, Trigger> _machine = new StateMachine<State, Trigger>(State.NotConnected);

        private readonly StateMachine<State, Trigger>.TriggerWithParameters<string> _connectTrigger;
        private readonly StateMachine<State, Trigger>.TriggerWithParameters<string, string> _loginTrigger;
        private readonly StateMachine<State, Trigger>.TriggerWithParameters<uint, string> _joinChannelTrigger;
        private readonly StateMachine<State, Trigger>.TriggerWithParameters<string> _chatCommandTrigger;

        enum State
        {
            NotConnected,
            Connected,
            Verified,
            KeysAuthorized,
            UserAuthenticated,
            Chatting,
            InChat,
        }
        enum Trigger
        {
            Connect,
            Disconnect,
            VerifyClient,
            AuthorizeKeys,
            Login,
            EnterChat,
            JoinChannel,
            ChatCommand
        }

        public BncsContext Context { get; private set; }

        private readonly BncsEvent AuthCheckEvent = new BncsEvent();
        private readonly BncsEvent AuthInfoEvent = new BncsEvent();
        private readonly BncsEvent EnterChatEvent = new BncsEvent();
        private readonly BncsEvent LogonEvent = new BncsEvent();
        private readonly BncsEvent ListRealmsEvent = new BncsEvent();
        private readonly BncsEvent RealmLogonEvent = new BncsEvent();

        internal BattleNetChatServer()
        {
            _connectTrigger = _machine.SetTriggerParameters<string>(Trigger.Connect);

            _loginTrigger = _machine.SetTriggerParameters<string, string>(Trigger.Login);
            _joinChannelTrigger = _machine.SetTriggerParameters<uint, string>(Trigger.JoinChannel);
            _chatCommandTrigger = _machine.SetTriggerParameters<string>(Trigger.ChatCommand);

            _machine.Configure(State.NotConnected)
                .Permit(Trigger.Connect, State.Connected);

            _machine.Configure(State.Connected)
                .OnEntryFrom<String>(_connectTrigger, OnConnect)
                .Permit(Trigger.VerifyClient, State.Verified)
                .Permit(Trigger.AuthorizeKeys, State.KeysAuthorized)
                .Permit(Trigger.Disconnect, State.NotConnected);

            _machine.Configure(State.KeysAuthorized)
                .SubstateOf(State.Connected)
                .OnEntryFrom(Trigger.AuthorizeKeys, OnAuthorizeKeys)
                .Permit(Trigger.Login, State.UserAuthenticated)
                .Permit(Trigger.Disconnect, State.NotConnected);

            _machine.Configure(State.UserAuthenticated)
                .SubstateOf(State.Connected)
                .SubstateOf(State.KeysAuthorized)
                .OnEntryFrom(_loginTrigger, (username, password) => OnLogin(username, password))
                .Permit(Trigger.EnterChat, State.InChat)
                .Permit(Trigger.Disconnect, State.NotConnected);

            _machine.Configure(State.InChat)
                .SubstateOf(State.Connected)
                .SubstateOf(State.KeysAuthorized)
                .SubstateOf(State.UserAuthenticated)
                .OnEntryFrom(Trigger.EnterChat, OnEnterChat)
                .InternalTransition(_joinChannelTrigger, (flags, channel, t) => OnJoinChannel(flags, channel))
                .InternalTransition(_chatCommandTrigger, (message, t) => OnChatCommand(message))
                .Permit(Trigger.Disconnect, State.NotConnected);

            Connection.PacketReceived += (obj, packet) => PacketReceivedEventHandlers.GetValueOrDefault(packet.Type, null)?.Invoke(packet);
            Connection.PacketSent += (obj, packet) => PacketSentEventHandlers.GetValueOrDefault(packet.Type, null)?.Invoke(packet);

            Connection.PacketReceived += (obj, packet) => Log.Verbose($"Received packet of type: {packet.Type}");

            OnReceivedPacketEvent(Sid.PING, packet => Connection.WritePacket(packet.Raw));
            OnReceivedPacketEvent(Sid.QUERYREALMS2, ListRealmsEvent.Set);
            OnReceivedPacketEvent(Sid.LOGONREALMEX, RealmLogonEvent.Set);
            OnReceivedPacketEvent(Sid.AUTH_CHECK, AuthCheckEvent.Set);
            OnReceivedPacketEvent(Sid.AUTH_INFO, AuthInfoEvent.Set);
            OnReceivedPacketEvent(Sid.ENTERCHAT, EnterChatEvent.Set);
            OnReceivedPacketEvent(Sid.LOGONRESPONSE2, LogonEvent.Set);
        }

        public void JoinChannel(string channel)
        {
            _machine.Fire(_joinChannelTrigger, 0x02U, channel);
        }

        public void ChatCommand(string message)
        {
            _machine.Fire(_chatCommandTrigger, message);
        }

        private void OnChatCommand(string message)
        {
            Connection.WritePacket(new ChatCommandPacket(message));
        }

        public void ConnectTo(string realm, string classicKey, string expansionKey)
        {
            Log.Information($"Connecting to {realm}");

            Context = new BncsContext
            {
                ClientToken = (uint)Environment.TickCount,
                ClassicKey = new CdKeySha1(classicKey),
                ExpansionKey = new CdKeySha1(expansionKey)
            };

            _machine.Fire(_connectTrigger, realm);
            _machine.Fire(Trigger.AuthorizeKeys);
            Log.Information($"Connected to {realm}");
        }

        public void EnterChat()
        {
            _machine.Fire(Trigger.EnterChat);
        }

        private void OnJoinChannel(uint flags, string channel)
        {
            Connection.WritePacket(new JoinChannelRequestPacket(flags, channel));
        }

        private void Listen()
        {
            while (_machine.IsInState(State.Connected))
            {
                _ = Connection.ReadPacket();
            }
        }

        public void Login(string username, string password) => _machine.Fire(_loginTrigger, username, password);

        private void OnConnect(string realm)
        {
            Connection.Connect(realm);

            var listener = new Thread(Listen);
            listener.Start();
        }

        private void OnEnterChat()
        {
            EnterChatEvent.Reset();
            Connection.WritePacket(new EnterChatRequestPacket(Context.Username));
            OnJoinChannel(0x05, DefaultChannel);
            _ = EnterChatEvent.WaitForPacket();
        }

        private void OnLogin(string username, string password)
        {
            Context.Username = username;

            LogonEvent.Reset();
            Connection.WritePacket(new LogonRequestPacket(Context.ClientToken, Context.ServerToken, Context.Username, password));
            var response = LogonEvent.WaitForPacket();
            _ = new LogonResponsePacket(response);
        }

        private void OnAuthorizeKeys()
        {
            AuthInfoEvent.Reset();
            Connection.WritePacket(new AuthInfoRequestPacket());
            var packet = new AuthInfoResponsePacket(AuthInfoEvent.WaitForPacket());
            Context.ServerToken = packet.ServerToken;

            var result = CheckRevisionV4.CheckRevision(packet.FormulaString);
            
            AuthCheckEvent.Reset();
            Connection.WritePacket(new AuthCheckRequestPacket(
                Context.ClientToken,
                Context.ServerToken,
                result.Version,
                result.Checksum,
                result.Info,
                Context.ClassicKey,
                Context.ExpansionKey));

            _ = new AuthCheckResponsePacket(AuthCheckEvent.WaitForPacket());
        }

        public void OnReceivedPacketEvent(Sid type, Action<BncsPacket> handler)
            => PacketReceivedEventHandlers.AddOrUpdate(type, handler, (t, h) => h += handler);

        public void OnSentPacketEvent(Sid type, Action<BncsPacket> handler)
            => PacketSentEventHandlers.AddOrUpdate(type, handler, (t, h) => h += handler);

        public List<string> ListMcpRealms()
        {
            ListRealmsEvent.Reset();
            Connection.WritePacket(new QueryRealmsRequestPacket());
            var packet = ListRealmsEvent.WaitForPacket();
            return new QueryRealmsResponsePacket(packet.Raw).Realms;
        }

        public RealmLogonResponsePacket RealmLogon(string realmName)
        {
            RealmLogonEvent.Reset();
            Connection.WritePacket(new RealmLogonRequestPacket(Context.ClientToken, Context.ServerToken, realmName, "password"));
            var packet = RealmLogonEvent.WaitForPacket();
            return new RealmLogonResponsePacket(packet.Raw);
        }
    }
}
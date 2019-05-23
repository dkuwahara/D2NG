using D2NG.BNCS.Packet;
using Serilog;
using Stateless;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using D2NG.BNCS.Login;
using System.Threading;
using Polly;

namespace D2NG.BNCS
{
    public class BattleNetChatServer
    {
        /***
         * Constants
         */
        private readonly String DefaultChannel = "Diablo II";

        private BncsConnection Connection { get; } = new BncsConnection();

        protected ConcurrentDictionary<Sid, Action<BncsPacket>> PacketReceivedEventHandlers { get; } = new ConcurrentDictionary<Sid, Action<BncsPacket>>();

        protected ConcurrentDictionary<Sid, Action<BncsPacket>> PacketSentEventHandlers { get; } = new ConcurrentDictionary<Sid, Action<BncsPacket>>();

        protected ConcurrentDictionary<Sid, ConcurrentQueue<BncsPacket>> ReceivedQueue { get; set; }

        private readonly StateMachine<State, Trigger> _machine = new StateMachine<State, Trigger>(State.NotConnected);

        private readonly StateMachine<State, Trigger>.TriggerWithParameters<string> _connectTrigger;

        private readonly StateMachine<State, Trigger>.TriggerWithParameters<string, string> _loginTrigger;

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
            EnterChat
        }

        public BncsContext Context { get; set; }

        private readonly BncsEvent ListRealmsEvent = new BncsEvent();
        private readonly BncsEvent RealmLogonEvent = new BncsEvent();

        internal BattleNetChatServer()
        {
            _connectTrigger = _machine.SetTriggerParameters<String>(Trigger.Connect);

            _loginTrigger = _machine.SetTriggerParameters<String, String>(Trigger.Login);

            _machine.Configure(State.NotConnected)
                .Permit(Trigger.Connect, State.Connected);

            _machine.Configure(State.Connected)
                .OnEntryFrom<String>(_connectTrigger, OnConnect)
                .Permit(Trigger.VerifyClient, State.Verified)
                .Permit(Trigger.Disconnect, State.NotConnected);

            _machine.Configure(State.Verified)
                .SubstateOf(State.Connected)
                .OnEntryFrom(Trigger.VerifyClient, OnVerifyClient)
                .Permit(Trigger.AuthorizeKeys, State.KeysAuthorized)
                .Permit(Trigger.Disconnect, State.NotConnected);

            _machine.Configure(State.KeysAuthorized)
                .SubstateOf(State.Connected)
                .SubstateOf(State.Verified)
                .OnEntryFrom(Trigger.AuthorizeKeys, OnAuthorizeKeys)
                .Permit(Trigger.Login, State.UserAuthenticated)
                .Permit(Trigger.Disconnect, State.NotConnected);

            _machine.Configure(State.UserAuthenticated)
                .SubstateOf(State.Connected)
                .SubstateOf(State.Verified)
                .SubstateOf(State.KeysAuthorized)
                .OnEntryFrom(_loginTrigger, (username, password) => OnLogin(username, password))
                .Permit(Trigger.EnterChat, State.InChat)
                .Permit(Trigger.Disconnect, State.NotConnected);

            _machine.Configure(State.InChat)
                .SubstateOf(State.Connected)
                .SubstateOf(State.Verified)
                .SubstateOf(State.KeysAuthorized)
                .SubstateOf(State.UserAuthenticated)
                .OnEntryFrom(Trigger.EnterChat, OnEnterChat)
                .Permit(Trigger.Disconnect, State.NotConnected);

            Connection.PacketReceived += (obj, packet) => {
                var sid = packet.Type;
                var handler = PacketReceivedEventHandlers.GetValueOrDefault(sid, null);

                if (handler is null)
                {
                    ReceivedQueue.GetOrAdd(sid, new ConcurrentQueue<BncsPacket>())
                        .Enqueue(packet);
                }
                else
                {
                    handler.Invoke(packet);
                }
            };

            Connection.PacketSent += (obj, packet) => PacketSentEventHandlers.GetValueOrDefault((Sid)packet.Type, null)?.Invoke(packet);

            OnReceivedPacketEvent(Sid.PING, packet => Connection.WritePacket(packet.Raw));
            OnReceivedPacketEvent(Sid.QUERYREALMS2, packet => ListRealmsEvent.Set(packet));
            OnReceivedPacketEvent(Sid.LOGONREALMEX, packet => RealmLogonEvent.Set(packet));
        }

        internal void ConnectTo(string realm, string classicKey, string expansionKey)
        {
            Log.Information($"Connecting to {realm}");
            _machine.Fire(_connectTrigger, realm);
            _machine.Fire(Trigger.VerifyClient);
            Context.ClassicKey = new CdKeySha1(classicKey);
            Context.ExpansionKey = new CdKeySha1(expansionKey);

            _machine.Fire(Trigger.AuthorizeKeys);
            Log.Information($"Connected to {realm}");
        }

        internal void EnterChat()
        {
            _machine.Fire(Trigger.EnterChat);
        }

        private void Listen()
        {
            while (_machine.IsInState(State.Connected))
            {
                _ = Connection.ReadPacket();
            }
        }

        internal void Login(string username, string password) => _machine.Fire(_loginTrigger, username, password);

        private byte[] WaitForPacket(Sid sid)
        {
            Policy.Handle<PacketNotFoundException>()
                .WaitAndRetry(new[] {
                    TimeSpan.FromMilliseconds(100),
                    TimeSpan.FromMilliseconds(1000),
                    TimeSpan.FromMilliseconds(2000)
                })
                .Execute(() => CheckForPacket(sid));

            if (!ReceivedQueue.GetOrAdd(sid, new ConcurrentQueue<BncsPacket>())
                    .TryDequeue(out BncsPacket packet))
            {
                throw new PacketNotFoundException();
            }

            return packet.Raw;
        }

        private void CheckForPacket(Sid sid)
        {
            if (!ReceivedQueue.ContainsKey(sid) || ReceivedQueue[sid].IsEmpty)
            {
                throw new PacketNotFoundException("No packet in queue");
            }
        }

        private void OnConnect(String realm)
        {
            ReceivedQueue = new ConcurrentDictionary<Sid, ConcurrentQueue<BncsPacket>>();
            Connection.Connect(realm);
            this.Context = new BncsContext();
            this.Context.ClientToken = (uint)Environment.TickCount;

            var listener = new Thread(Listen);
            listener.Start();
        }

        private void OnEnterChat()
        {
            Connection.WritePacket(new EnterChatRequestPacket(Context.Username));
            Connection.WritePacket(new JoinChannelRequestPacket(DefaultChannel));
            _ = WaitForPacket(Sid.ENTERCHAT);
        }

        private void OnLogin(string username, string password)
        {
            Context.Username = username;
            var packet = new LogonRequestPacket(Context.ClientToken, Context.ServerToken, Context.Username, password);
            Connection.WritePacket(packet);

            var response = WaitForPacket(Sid.LOGONRESPONSE2);
            _ = new LogonResponsePacket(response);
        }

        private void OnVerifyClient()
        {
            Connection.WritePacket(new AuthInfoRequestPacket());
        }

        private void OnAuthorizeKeys()
        {
            var packet = new AuthInfoResponsePacket(WaitForPacket(Sid.AUTH_INFO));
            Context.ServerToken = packet.ServerToken;
            
            Log.Debug("[{0}] Server token: {1} Logon Type: {2}", GetType(), Context.ServerToken, packet.LogonType);

            var result = CheckRevisionV4.CheckRevision(packet.FormulaString);
            Log.Debug("[{0}] CheckRevision: {1}", GetType(), result);
            Connection.WritePacket(new AuthCheckRequestPacket(
                Context.ClientToken,
                Context.ServerToken,
                result.Version,
                result.Checksum,
                result.Info,
                Context.ClassicKey,
                Context.ExpansionKey));

            _ = new AuthCheckResponsePacket(WaitForPacket(Sid.AUTH_CHECK));
        }

        public void OnReceivedPacketEvent(Sid type, Action<BncsPacket> handler)
        {
            if (PacketReceivedEventHandlers.ContainsKey(type))
            {
                PacketReceivedEventHandlers[type] += handler;
            }
            else
            {
                PacketReceivedEventHandlers.GetOrAdd(type, handler);
            }
        }

        public void OnSentPacketEvent(Sid type, Action<BncsPacket> handler)
        {
            if (PacketSentEventHandlers.ContainsKey(type))
            {
                PacketSentEventHandlers[type] += handler;
            }
            else
            {
                PacketSentEventHandlers.GetOrAdd(type, handler);
            }
        }

        internal List<string> ListMcpRealms()
        {
            ListRealmsEvent.Reset();
            Connection.WritePacket(new QueryRealmsRequestPacket());
            var packet = ListRealmsEvent.WaitForPacket();
            return new QueryRealmsResponsePacket(packet.Raw).Realms;
        }

        internal RealmLogonResponsePacket RealmLogon(string realmName)
        {
            RealmLogonEvent.Reset();
            Connection.WritePacket(new RealmLogonRequestPacket(Context.ClientToken, Context.ServerToken, realmName, "password"));
            var packet = RealmLogonEvent.WaitForPacket();
            return new RealmLogonResponsePacket(packet.Raw);
        }
    }
}


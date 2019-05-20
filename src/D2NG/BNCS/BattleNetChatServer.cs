using D2NG.BNCS.Packet;
using Serilog;
using Stateless;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using D2NG.BNCS.Event;
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

        protected ConcurrentDictionary<Sid, Action<BncsPacketReceivedEvent>> PacketReceivedEventHandlers { get; } = new ConcurrentDictionary<Sid, Action<BncsPacketReceivedEvent>>();

        protected ConcurrentDictionary<Sid, Action<BncsPacketSentEvent>> PacketSentEventHandlers { get; } = new ConcurrentDictionary<Sid, Action<BncsPacketSentEvent>>();

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

        private (AutoResetEvent Event, List<(string Name, string Description)>  Realms) ListRealmsEvent = (new AutoResetEvent(false), null);
        private (AutoResetEvent Event, RealmLogonResponsePacket Packet) RealmLogonEvent = (new AutoResetEvent(false), null);

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

            Connection.PacketReceived += (obj, eventArgs) => {
                var sid = (Sid)eventArgs.Packet.Type;
                var handler = PacketReceivedEventHandlers.GetValueOrDefault(sid, null);

                if (handler is null)
                {
                    ReceivedQueue.GetOrAdd(sid, new ConcurrentQueue<BncsPacket>())
                        .Enqueue(eventArgs.Packet);
                }
                else
                {
                    handler.Invoke(eventArgs);
                }
            };

            Connection.PacketSent += (obj, eventArgs) => {
                var sid = (Sid)eventArgs.Type;
                PacketSentEventHandlers.GetValueOrDefault(sid, null)?.Invoke(eventArgs);
            };

            OnReceivedPacketEvent(Sid.PING, obj => Connection.WritePacket(obj.Packet.Raw));
            OnReceivedPacketEvent(Sid.QUERYREALMS2, obj => OnReceivedQueryRealmsResponse(new QueryRealmsResponsePacket(obj.Packet.Raw)));
            OnReceivedPacketEvent(Sid.LOGONREALMEX, obj => OnReceivedRealmLogonResponse(new RealmLogonResponsePacket(obj.Packet.Raw)));
        }

        public void ConnectTo(string realm, string classicKey, string expansionKey)
        {
            Log.Information($"Connecting to {realm}");
            _machine.Fire(_connectTrigger, realm);
            _machine.Fire(Trigger.VerifyClient);
            if (classicKey.Length == 16)
            {
                Context.ClassicKey = new CdKeyBsha1(classicKey);
                Context.ExpansionKey = new CdKeyBsha1(expansionKey);
            }
            else
            {
                Context.ClassicKey = new CdKeySha1(classicKey);
                Context.ExpansionKey = new CdKeySha1(expansionKey);
            }

            _machine.Fire(Trigger.AuthorizeKeys);
            Log.Information($"Connected to {realm}");
        }

        public void EnterChat()
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

        public void Login(string username, string password)
        {
            Log.Information($"Logging in as {username}");
            _machine.Fire(_loginTrigger, username, password);
            Log.Information($"Logged in as {username}");
        }

        private byte[] WaitForPacket(Sid sid)
        {
            Policy.Handle<PacketNotFoundException>()
                .WaitAndRetry(new[] {
                    TimeSpan.FromMilliseconds(100),
                    TimeSpan.FromMilliseconds(1000),
                    TimeSpan.FromMilliseconds(2000)
                })
                .Execute(() => CheckForPacket(sid));

            BncsPacket packet;
            if (!ReceivedQueue.GetOrAdd(sid, new ConcurrentQueue<BncsPacket>())
                    .TryDequeue(out packet))
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
                result,
                Context.ClassicKey,
                Context.ExpansionKey));

            _ = new AuthCheckResponsePacket(WaitForPacket(Sid.AUTH_CHECK));
        }

        public void OnReceivedPacketEvent(Sid type, Action<BncsPacketReceivedEvent> handler)
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

        public void OnSentPacketEvent(Sid type, Action<BncsPacketSentEvent> handler)
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

        internal List<(string Name, string Description)> ListMcpRealms()
        {
            Connection.WritePacket(new QueryRealmsRequestPacket());
            ListRealmsEvent.Event.WaitOne();
            return ListRealmsEvent.Realms;
        }

        private void OnReceivedQueryRealmsResponse(QueryRealmsResponsePacket packet)
        {
            ListRealmsEvent.Realms = packet.Realms;
            ListRealmsEvent.Event.Set();
        }

        internal RealmLogonResponsePacket RealmLogon(string realmName)
        {
            Connection.WritePacket(new RealmLogonRequestPacket(Context.ClientToken, Context.ServerToken, realmName, "password"));
            RealmLogonEvent.Event.WaitOne();
            return RealmLogonEvent.Packet;
        }

        private void OnReceivedRealmLogonResponse(RealmLogonResponsePacket packet)
        {
            RealmLogonEvent.Packet = packet;
            RealmLogonEvent.Event.Set();
        }
    }
}


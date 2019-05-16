using D2NG.BNCS.Packet;
using Serilog;
using Stateless;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using D2NG.BNCS.Event;
using D2NG.BNCS.Login;

namespace D2NG
{
    public class BattleNetChatServer
    {
        private BncsConnection Connection { get; } = new BncsConnection();

        protected ConcurrentDictionary<byte, Action<BncsPacketReceivedEvent>> PacketReceivedEventHandlers { get; } = new ConcurrentDictionary<byte, Action<BncsPacketReceivedEvent>>();

        protected ConcurrentDictionary<byte, Action<BncsPacketSentEvent>> PacketSentEventHandlers { get; } = new ConcurrentDictionary<byte, Action<BncsPacketSentEvent>>();
        

        private readonly StateMachine<State, Trigger> _machine = new StateMachine<State, Trigger>(State.NotConnected);

        private readonly StateMachine<State, Trigger>.TriggerWithParameters<string> _connectTrigger;

        private readonly StateMachine<State, Trigger>.TriggerWithParameters<string, string> _loginTrigger;

        private CdKey _classicKey;

        private CdKey _expansionKey;

        private readonly uint _clientToken;

        private readonly String DefaultChannel = "Diablo II";
        private uint _serverToken;
        enum State
        {
            NotConnected,
            Connected,
            Verified,
            KeysAuthorized,
            UserAuthenticated,
            Chatting,
        }
        enum Trigger
        {
            Connect,
            Disconnect,
            VerifyClient,
            AuthorizeKeys,
            Login
        }


        public BattleNetChatServer()
        {
            _clientToken = (uint)Environment.TickCount;
            
            _connectTrigger = _machine.SetTriggerParameters<String>(Trigger.Connect);

            _loginTrigger = _machine.SetTriggerParameters<String, String>(Trigger.Login);

            _machine.Configure(State.NotConnected)
                .Permit(Trigger.Connect, State.Connected);

            _machine.Configure(State.Connected)
                .OnEntryFrom<String>(_connectTrigger, realm => Connection.Connect(realm), "Realm to connect to")
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
                .Permit(Trigger.Login, State.UserAuthenticated);

            _machine.Configure(State.UserAuthenticated)
                .SubstateOf(State.Connected)
                .SubstateOf(State.Verified)
                .SubstateOf(State.KeysAuthorized)
                .OnEntryFrom(_loginTrigger, (username, password) => OnLogin(username, password));

            Connection.PacketReceived += (obj, eventArgs) => {
                Log.Debug("[{0}] Received Packet {1}", GetType(), BitConverter.ToString(eventArgs.Packet.Raw));
                PacketReceivedEventHandlers.GetValueOrDefault(eventArgs.Packet.Type, null)?.Invoke(eventArgs);
            };

            Connection.PacketSent += (obj, eventArgs) => {
                Log.Debug("[{0}] Sent Packet {1}", GetType(), BitConverter.ToString(eventArgs.Packet));
                PacketSentEventHandlers.GetValueOrDefault(eventArgs.Type, null)?.Invoke(eventArgs);
            };

            OnReceivedPacketEvent(0x25, obj => Connection.WritePacket(obj.Packet.Raw));
        }


        public void ConnectTo(string realm, string classicKey, string expansionKey)
        {
            _machine.Fire(_connectTrigger, realm);
            _machine.Fire(Trigger.VerifyClient);
            if (classicKey.Length == 16)
            {
                _classicKey = new CdKeyBsha1(classicKey);
                _expansionKey = new CdKeyBsha1(expansionKey);
            }
            else
            {
                _classicKey = new CdKeySha1(classicKey);
                _expansionKey = new CdKeySha1(expansionKey);
            }

            _machine.Fire(Trigger.AuthorizeKeys);
        }

        public void Login(string username, string password)
        {
            _machine.Fire(_loginTrigger, username, password);
        }

        private void OnLogin(string username, string password)
        {
            var packet = new BncsLogonRequestPacket(_clientToken, _serverToken, username, password);
            Connection.WritePacket(packet);
            byte[] response;
            do
            {
                response = Connection.ReadPacket();
                if (response[1] == 0xFF)
                {
                    throw new LogonFailedException();
                }
            } while (response[1] != BncsLogonResponsePacket.SidByte);
            _ = new BncsLogonResponsePacket(response);

            Connection.WritePacket(new EnterChatRequestPacket(username));
            Connection.WritePacket(new JoinChannelRequestPacket(DefaultChannel));
        }

        public void OnVerifyClient()
        {
            Connection.WritePacket(new BncsAuthInfoRequestPacket());
            _ = Connection.ReadPacket();
        }

        public void OnAuthorizeKeys()
        {
            var packet = new BncsAuthInfoResponsePacket(Connection.ReadPacket());
            _serverToken = packet.ServerToken;
            
            Log.Debug("[{0}] Server token: {1} Logon Type: {2}", GetType(), _serverToken, packet.LogonType);

            var result = CheckRevisionV4.CheckRevision(packet.FormulaString);
            Log.Debug("[{0}] CheckRevision: {1}", GetType(), result);
            Connection.WritePacket(new BncsAuthCheckRequestPacket(
                _clientToken,
                packet.ServerToken,
                result,
                _classicKey,
                _expansionKey));

            var authCheckResponse = new BncsAuthCheckResponsePacket(Connection.ReadPacket());

            Log.Debug("{0:X}", authCheckResponse);
        }

        public void OnReceivedPacketEvent(byte type, Action<BncsPacketReceivedEvent> handler)
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

        public void OnSentPacketEvent(byte type, Action<BncsPacketSentEvent> handler)
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

    }
}


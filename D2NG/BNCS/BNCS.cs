using D2NG.BNCS.Packet;
using Serilog;
using Stateless;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using D2NG.BNCS.Login;

namespace D2NG
{
    public class BattleNetChatServer
    {
        private BncsConnection Connection { get; } = new BncsConnection();

        protected ConcurrentDictionary<byte, Action<BNCSPacketReceivedEvent>> PacketReceivedEventHandlers { get; } = new ConcurrentDictionary<byte, Action<BNCSPacketReceivedEvent>>();

        protected ConcurrentDictionary<byte, Action<BNCSPacketSentEvent>> PacketSentEventHandlers { get; } = new ConcurrentDictionary<byte, Action<BNCSPacketSentEvent>>();

        private readonly StateMachine<State, Trigger> _machine = new StateMachine<State, Trigger>(State.NotConnected);

        private readonly StateMachine<State, Trigger>.TriggerWithParameters<string> _connectTrigger;

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
            _connectTrigger = _machine.SetTriggerParameters<String>(Trigger.Connect);

            _machine.Configure(State.NotConnected)
                .Permit(Trigger.Connect, State.Connected);

            _machine.Configure(State.Connected)
                .OnEntryFrom<String>(_connectTrigger, realm => Connection.Connect(realm), "Realm to connect to")
                .Permit(Trigger.VerifyClient, State.Verified)
                .Permit(Trigger.Disconnect, State.NotConnected);

            _machine.Configure(State.Verified)
                .SubstateOf(State.Connected)
                .OnEntryFrom(Trigger.VerifyClient, () => OnVerifyClient())
                .Permit(Trigger.AuthorizeKeys, State.KeysAuthorized)
                .Permit(Trigger.Disconnect, State.NotConnected);

            _machine.Configure(State.KeysAuthorized)
                .SubstateOf(State.Connected)
                .SubstateOf(State.Verified)
                .OnEntryFrom(Trigger.AuthorizeKeys, () => OnAuthorizeKeys());


            Connection.PacketReceived += (obj, eventArgs) => {
                Log.Debug("[{0}] Received Packet 0x{1:X}", GetType(), eventArgs.Packet.Type);
                PacketReceivedEventHandlers.GetValueOrDefault(eventArgs.Packet.Type, null)?.Invoke(eventArgs);
            };

            Connection.PacketSent += (obj, eventArgs) => {
                Log.Debug("[{0}] Sent Packet 0x{1:X}", GetType(), eventArgs.Type);
                PacketSentEventHandlers.GetValueOrDefault(eventArgs.Type, null)?.Invoke(eventArgs);
            };

            OnReceivedPacketEvent(0x25, obj => Connection.WritePacket(obj.Packet.Raw));
        }

        public void ConnectTo(String realm)
        {
            _machine.Fire(_connectTrigger, realm);
            _machine.Fire(Trigger.VerifyClient);
            _machine.Fire(Trigger.AuthorizeKeys);
            
        }

        public void OnVerifyClient()
        {
            Connection.WritePacket(new BncsAuthInfoRequestPacket());
            var packet = Connection.ReadPacket();
            Log.Debug("{0:X}", packet[1]);
        }

        public void OnAuthorizeKeys()
        {
            var packet = new BncsAuthInfoResponsePacket(Connection.ReadPacket());
            Log.Debug("{0}", packet);
            var result = CheckRevisionV4.CheckRevision(packet.FormulaString);

        }

        public void SendPacket(byte command, params IEnumerable<byte>[] args)
        {
            byte[] packet = BuildPacket(command, args);
            Connection.WritePacket(packet);
        }

        protected static byte[] BuildPacket(byte command, params IEnumerable<byte>[] args)
        {
            var packet = new List<byte> { 0xFF, command };
            var packetArray = new List<byte>();

            foreach (var a in args)
            {
                packetArray.AddRange(a);
            }

            UInt16 packetSize = (UInt16)(packetArray.Count + 4);
            packet.AddRange(BitConverter.GetBytes(packetSize));
            packet.AddRange(packetArray);

            return packet.ToArray();
        }

        public void OnReceivedPacketEvent(byte type, Action<BNCSPacketReceivedEvent> handler)
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

        public void OnSentPacketEvent(byte type, Action<BNCSPacketSentEvent> handler)
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


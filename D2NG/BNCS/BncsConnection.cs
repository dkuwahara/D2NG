using D2NG.BNCS.Packet;
using Serilog;
using Stateless;
using Stateless.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace D2NG
{
    class BncsConnection
    {
        /**
         * Default port used to connected to BNCS
         */
        public static readonly int DefaultPort = 6112;

        private enum State
        {
            NotConnected,
            Connected
        }

        private enum Trigger
        {
            ConnectSocket,
            KillSocket,
            Write,
            Read
        }

        /**
         * State Machine for the connection
         */
        private readonly StateMachine<State, Trigger> _machine;

        private readonly StateMachine<State, Trigger>.TriggerWithParameters<string> _connectTrigger;

        private readonly StateMachine<State, Trigger>.TriggerWithParameters<BncsPacket> _readTrigger;

        private readonly StateMachine<State, Trigger>.TriggerWithParameters<byte[]> _writeTrigger;

        /**
         * Events on send and receive
         */
        public event EventHandler<BncsPacketReceivedEvent> PacketReceived;

        public event EventHandler<BncsPacketSentEvent> PacketSent;

        /**
         * The actual TCP Connection
         */
        private NetworkStream _stream;

        private TcpClient _tcpClient;


        public BncsConnection()
        {
            _machine = new StateMachine<State, Trigger>(State.NotConnected);
            _connectTrigger = _machine.SetTriggerParameters<String>(Trigger.ConnectSocket);
            _writeTrigger = _machine.SetTriggerParameters<byte[]>(Trigger.Write);
            _readTrigger = _machine.SetTriggerParameters<BncsPacket>(Trigger.Read);

            _machine.Configure(State.NotConnected)
                .OnEntryFrom(Trigger.KillSocket, t => OnTerminate())
                .Permit(Trigger.ConnectSocket, State.Connected)
                .OnEntry(() => Log.Debug("[{0}] Entered State: {1}", GetType(), State.NotConnected))
                .OnExit(() => Log.Debug("[{0}] Exited State: {1}", GetType(), State.NotConnected));

            _machine.Configure(State.Connected)
                .OnEntryFrom<String>(_connectTrigger, realm => OnConnect(realm), "Realm to connect to")
                .InternalTransition(_writeTrigger, (data, t) => OnWritePacket(data))
                .InternalTransition(_readTrigger, (packet, t) => OnGetPacket(packet))
                .Permit(Trigger.KillSocket, State.NotConnected)
                .OnEntry(() => Log.Debug("[{0}] Entered State: {1}", GetType(), State.Connected))
                .OnExit(() => Log.Debug("[{0}] Exited State: {1}", GetType(), State.Connected));

        }

        public void Connect(String realm) => _machine.Fire(_connectTrigger, realm);

        public bool Connected => _machine.IsInState(State.Connected);

        public byte[] ReadPacket()
        {
            List<byte> buffer = new List<byte>();

            // Get the first 4 bytes, packet type and length
            ReadUpTo(ref buffer, 4);
            short packetLength = BitConverter.ToInt16(buffer.ToArray(), 2);

            // Read the rest of the packet and return it
            ReadUpTo(ref buffer, packetLength);

            var packet = new BncsPacket(buffer.ToArray());
            _machine.Fire(_readTrigger, packet);
            return buffer.ToArray();
        }

        public void Terminate() => _machine.Fire(Trigger.KillSocket);

        public void WritePacket(byte[] packet) => _machine.Fire(_writeTrigger, packet);

        public void WritePacket(BncsPacket packet) => this.WritePacket(packet.Raw);

        private void OnConnect(String realm)
        {
            Log.Debug("[{0}] Resolving {1}", GetType(), realm);
            var server = Dns.GetHostAddresses(realm).First();

            Log.Debug("[{0}] Found server {1}", GetType(), server);
            this.Connect(server, DefaultPort);
        }

        private void Connect(IPAddress ip, int port)
        {
            Log.Debug("[{0}] Connecting to {1}:{2}", GetType(), ip, port);
            _tcpClient = new TcpClient();
            _tcpClient.Connect(ip, port);
            _stream = _tcpClient.GetStream();
            if(!_stream.CanWrite)
            {
                Log.Debug("[{0}] Unable to write to {1}:{2}, closing connection", GetType(), ip, port);
                _tcpClient.Close();
                _stream.Close();
                _tcpClient = null;
                _stream = null;
                throw new BncsConnectException();
            }
            _stream.WriteByte(0x01);
            Log.Debug("[{0}] Successfully connected to {1}:{2}", GetType(), ip, port);
        }

        private void OnGetPacket(BncsPacket packet)
        {
            PacketReceived?.Invoke(this, new BncsPacketReceivedEvent(packet));
        }

        private void OnTerminate()
        {
            _tcpClient.Close();
            _stream.Close();
        }


        private void OnWritePacket(byte[] packet)
        {
            _stream.Write(packet, 0, packet.Length);
            PacketSent?.Invoke(this, new BncsPacketSentEvent(packet));
        }

        private void ReadUpTo(ref List<byte> bncsBuffer, int count)
        {
            while (bncsBuffer.Count < count)
            {
                byte temp = (byte)_stream.ReadByte();
                bncsBuffer.Add(temp);
            }
        }
    }
}

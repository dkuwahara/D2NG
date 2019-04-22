using Serilog;
using Stateless;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace D2NG
{
    class BNCSConnection
    {

        /**
         * Default port used to connected to BNCS
         */

        public static readonly int DEFAULT_PORT = 6112;

        private TcpClient _tcpClient;

        private NetworkStream _stream;

        private readonly StateMachine<State, Trigger>.TriggerWithParameters<string> _setConnectTrigger;

        public event EventHandler<BNCSPacketReceivedEvent> PacketReceived;

        public event EventHandler<BNCSPacketSentEvent> PacketSent;

        private readonly StateMachine<State, Trigger> _stateMachine = new StateMachine<State, Trigger>(State.NotConnected);

        enum State
        {
            NotConnected,
            Connected,
            Listening
        }
        enum Trigger {
            ConnectSocket,
            ListenToSocket,
            KillSocket
        }

        public BNCSConnection()
        {
            _setConnectTrigger = _stateMachine.SetTriggerParameters<String>(Trigger.ConnectSocket);

            _stateMachine.Configure(State.NotConnected)
                .OnEntryFrom(Trigger.KillSocket, t => OnTerminate())
                .Permit(Trigger.ConnectSocket, State.Connected)
                .OnEntry(() => Log.Debug("[{0}] Entered State: {1}", GetType(), State.NotConnected))
                .OnExit(() => Log.Debug("[{0}] Exited State: {1}", GetType(), State.NotConnected));

            _stateMachine.Configure(State.Connected)
                .OnEntryFrom<String>(_setConnectTrigger, realm => OnConnect(realm), "Realm to connect to")
                .Permit(Trigger.ListenToSocket, State.Listening)
                .Permit(Trigger.KillSocket, State.NotConnected)
                .OnEntry(() => Log.Debug("[{0}] Entered State: {1}", GetType(), State.Connected))
                .OnExit(() => Log.Debug("[{0}] Exited State: {1}", GetType(), State.Connected));

            _stateMachine.Configure(State.Listening)
                .OnEntryFrom(Trigger.ListenToSocket, t => OnListen())
                .SubstateOf(State.Connected)
                .Permit(Trigger.KillSocket, State.NotConnected)
                .OnEntry(() => Log.Debug("[{0}] Entered State: {1}", GetType(), State.Listening))
                .OnExit(() => Log.Debug("[{0}] Exited State: {1}", GetType(), State.Listening));
        }

        public bool IsListening() => _stateMachine.IsInState(State.Listening);

        public bool IsConnected() => _stateMachine.IsInState(State.Connected);

        public void Connect(String realm) => _stateMachine.Fire(_setConnectTrigger, realm);
        private void OnConnect(String realm)
        {
            Log.Debug("[{0}] Resolving {1}", GetType(), realm);
            var server = Dns.GetHostAddresses(realm).First();

            Log.Debug("[{0}] Found server {1}", GetType(), server);
            this.Connect(server);
        }
        private void Connect(IPAddress ip) => this.Connect(ip, DEFAULT_PORT);

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
                throw new BNCSConnectException();
            }
            Log.Debug("[{0}] Successfully connected to {1}:{2}", GetType(), ip, port);
        }

        public void Send(byte packet)
        {
            _stream.WriteByte(packet);
        }

        public void Send(List<byte> packet)
        {
            Send(packet.ToArray());
        }
        public void Send(byte[] packet)
        {
            _stream.Write(packet, 0, packet.Length);
            PacketSent?.Invoke(this, new BNCSPacketSentEvent(packet));
        }

        public void Terminate() => _stateMachine.Fire(Trigger.KillSocket);

        private void OnTerminate()
        {
            _tcpClient.Close();
            _stream.Close();
        }

        public void Listen() => _stateMachine.Fire(Trigger.ListenToSocket);

        private void OnListen()
        {
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                while (_stateMachine.IsInState(State.Connected) && _tcpClient != null && _tcpClient.Connected)
                {
                    try
                    {
                        var packet = GetPacket();
                        PacketReceived?.Invoke(this, new BNCSPacketReceivedEvent(packet));
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "[{0}] Failed to get Packet", GetType());
                        break;
                    }
                }
                Terminate();
            });
        }

        private byte[] GetPacket()
        {
            List<byte> buffer = new List<byte>();

            // Get the first 4 bytes, packet type and length
            ReadUpTo(ref buffer, 4);
            short packetLength = BitConverter.ToInt16(buffer.ToArray(), 2);

            // Read the rest of the packet and return it
            ReadUpTo(ref buffer, packetLength);
            return buffer.ToArray();
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

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
            SocketConnected,
            ListenToSocket,
            SocketKilled
        }

        public BNCSConnection()
        {
            _stateMachine.Configure(State.NotConnected)
                .Permit(Trigger.SocketConnected, State.Connected)
                .OnEntry(() => Console.WriteLine("[{0}] Entered State: {1}", GetType(), State.NotConnected))
                .OnExit(() => Console.WriteLine("[{0}] Exited State: {1}", GetType(), State.NotConnected));

            _stateMachine.Configure(State.Connected)
                .Permit(Trigger.ListenToSocket, State.Listening)
                .Permit(Trigger.SocketKilled, State.NotConnected)
                .OnEntry(() => Console.WriteLine("[{0}] Entered State: {1}", GetType(), State.Connected))
                .OnExit(() => Console.WriteLine("[{0}] Exited State: {1}", GetType(), State.Connected));

            _stateMachine.Configure(State.Listening)
                .SubstateOf(State.Connected)
                .Permit(Trigger.SocketKilled, State.NotConnected)
                .OnEntry(() => Console.WriteLine("[{0}] Entered State: {1}", GetType(), State.Listening))
                .OnExit(() => Console.WriteLine("[{0}] Exited State: {1}", GetType(), State.Listening));
        }

        public bool IsListening() => _stateMachine.IsInState(State.Listening);

        public bool IsConnected() => _stateMachine.IsInState(State.Connected);

        public void Connect(String realm)
        {
            if(_tcpClient != null && _tcpClient.Connected)
            {
                throw new AlreadyConnectedException("BNCS Already Connected");
            }

            Console.WriteLine("[{0}] Resolving {1}", GetType(), realm);
            var server = Dns.GetHostAddresses(realm).First();

            Console.WriteLine("[{0}] Found server {1}", GetType(), server);
            this.Connect(server);
            _stateMachine.Fire(Trigger.SocketConnected);
        }

        private void Connect(IPAddress ip)
        {
            this.Connect(ip, DEFAULT_PORT);
        }

        private void Connect(IPAddress ip, int port)
        {
            Console.WriteLine("[{0}] Connecting to {1}:{2}", GetType(), ip, port);
            _tcpClient = new TcpClient();
            _tcpClient.Connect(ip, port);
            _stream = _tcpClient.GetStream();
            if(!_stream.CanWrite)
            {
                Console.WriteLine("[{0}] Unable to write to {1}:{2}, closing connection", GetType(), ip, port);
                _tcpClient.Close();
                _stream.Close();
                throw new BNCSConnectException();
            }
            Console.WriteLine("[{0}] Successfully connected to {1}:{2}", GetType(), ip, port);
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

        public void Terminate()
        {
            _tcpClient.Close();
            _stream.Close();
            _stateMachine.Fire(Trigger.SocketKilled);
        }
        public void Listen()
        {
            ThreadPool.QueueUserWorkItem( (obj) =>
            {
                _stateMachine.Fire(Trigger.ListenToSocket);
                while (_stateMachine.IsInState(State.Connected) && _tcpClient != null && _tcpClient.Connected)
                {
                    try
                    {
                        var packet = GetPacket();
                        var packetType = packet[1];

                        PacketReceived?.Invoke(this, new BNCSPacketReceivedEvent(packet));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("[{0}] Failed to get Packet {1}", GetType(), e.StackTrace);
                        break;
                    }
                }
                Terminate();
            } );
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

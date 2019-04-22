using System;
using System.Collections.Concurrent;
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
         * Current version byte, update this on new patches
         */
        public static readonly byte VERSION = 0x0d;

        /**
         * Packet sent to authenticate version.
         */
        public static readonly byte[] AuthInfoPacket =
        {
            0xff, 0x50, 0x3a, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x36, 0x38, 0x58, 0x49, 0x50, 0x58, 0x32, 0x44,
            VERSION, 0x00, 0x00, 0x00, 0x53, 0x55, 0x6e, 0x65,
            0x55, 0xb4, 0x47, 0x40, 0x88, 0xff, 0xff, 0xff,
            0x09, 0x04, 0x00, 0x00, 0x09, 0x04, 0x00, 0x00,
            0x55, 0x53, 0x41, 0x00, 0x55, 0x6e, 0x69, 0x74,
            0x65, 0x64, 0x20, 0x53, 0x74, 0x61, 0x74, 0x65,
            0x73, 0x00
        };

        /**
         * Default port used to connected to BNCS
         */

        public static readonly int DEFAULT_PORT = 6112;

        private TcpClient _tcpClient;

        private NetworkStream _stream;

        private event EventHandler<BNCSPacketReceivedEvent> PacketReceived;

        private event EventHandler<BNCSPacketSentEvent> PacketSent;

        private readonly ConcurrentDictionary<byte, Action<BNCSPacketReceivedEvent>> _packetReceivedEventHandlers = new ConcurrentDictionary<byte, Action<BNCSPacketReceivedEvent>>();

        private readonly ConcurrentDictionary<byte, Action<BNCSPacketSentEvent>> _packetSentEventHandlers = new ConcurrentDictionary<byte, Action<BNCSPacketSentEvent>>();

        public void  SubscribeToReceivedPacketEvent(byte type, Action<BNCSPacketReceivedEvent> handler)
        {
            if(_packetReceivedEventHandlers.ContainsKey(type))
            {
                _packetReceivedEventHandlers[type] += handler;
            }
            else
            {
                _packetReceivedEventHandlers.GetOrAdd(type, handler);
            }
        }

        public void SubscribeToSentPacketEvent(byte type, Action<BNCSPacketSentEvent> handler)
        {
            if (_packetSentEventHandlers.ContainsKey(type))
            {
                _packetSentEventHandlers[type] += handler;
            }
            else
            {
                _packetSentEventHandlers.GetOrAdd(type, handler);
            }
        }

        public BNCSConnection()
        {
            EventHandler<BNCSPacketReceivedEvent> onReceived = (sender, eventArgs) =>
            {
                Console.WriteLine("[{0}] Received Packet 0x{1:X}", GetType(), eventArgs.Type);
                _packetReceivedEventHandlers.GetValueOrDefault(eventArgs.Type, null)?.Invoke(eventArgs);
            };
            PacketReceived += onReceived;

            EventHandler<BNCSPacketSentEvent> onSent = (sender, eventArgs) =>
            {
                Console.WriteLine("[{0}] Sent Packet 0x{1:X}", GetType(), eventArgs.Type);
                _packetSentEventHandlers.GetValueOrDefault(eventArgs.Type, null)?.Invoke(eventArgs);
            };
            PacketSent += onSent;
        }

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
        }
        public void Listen()
        {    
            ThreadPool.QueueUserWorkItem( (obj) =>
            {
                while (_tcpClient != null && _tcpClient.Connected)
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace D2NG
{
    class BNCS
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

        private TcpClient _client;

        private NetworkStream _stream;

        public void Connect(String realm)
        {
            if(_client != null && _client.Connected)
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
            _client = new TcpClient();
            _client.Connect(ip, port);
            _stream = _client.GetStream();
            if(!_stream.CanWrite)
            {
                Console.WriteLine("[{0}] Unable to write to {1}:{2}, closing connection", GetType(), ip, port);
                _client.Close();
                _stream.Close();
                throw new BNCSConnectException();
            }
            Console.WriteLine("[{0}] Successfully connected to {1}:{2}", GetType(), ip, port);
        }

        public void Send(byte packet)
        {
            _stream.WriteByte(packet);
        }

        public void Send(byte[] packet)
        {
            _stream.Write(packet, 0, packet.Length);
        }

        public void Listen()
        {           
            while (_client != null && _client.Connected)
            {
                try
                {
                    var packet = GetPacket();
                    var packetType = packet[1];
                    Console.WriteLine("[{0}] Received packet 0x{1:X} from server", GetType(), packetType);
                }
                catch(Exception e)
                {
                    Console.WriteLine("[{0}] Failed to get Packet {1}", GetType(), e.StackTrace);
                    break;
                }
               
            }
            _client.Close();
            _stream.Close();
        }

        private List<byte> GetPacket()
        {
            List<byte> buffer = new List<byte>();

            // Get the first 4 bytes, packet type and length
            ReadUpTo(ref buffer, 4);
            short packetLength = BitConverter.ToInt16(buffer.ToArray(), 2);

            // Read the rest of the packet and return it
            ReadUpTo(ref buffer, packetLength);
            return buffer;
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

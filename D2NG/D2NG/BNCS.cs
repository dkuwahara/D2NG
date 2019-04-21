using System;
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

        public void Connect(IPAddress ip)
        {
            this.Connect(ip, DEFAULT_PORT);
        }

        private void Connect(IPAddress ip, int port)
        {
            _client = new TcpClient();
            _client.Connect(ip, port);
            _stream = _client.GetStream();
            if(!_stream.CanWrite)
            {
                Console.Write("[{0}] Unable to write to {1}:{2}, closing connection", GetType(), ip, port);
                _stream.Close();
                _client.Close();
                throw new BNCSConnectException();
            }
        }
    }
}
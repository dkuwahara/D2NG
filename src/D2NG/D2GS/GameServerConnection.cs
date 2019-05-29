using D2NG.D2GS.Packet;
using Serilog;
using System;
using System.Collections.Generic;

namespace D2NG.D2GS
{
    internal class GameServerConnection : Connection
    {
        private static readonly short[] PacketSizes =
        {
            1, 8, 1, 12, 1, 1, 1, 6, 6, 11, 6, 6, 9, 13, 12, 16,
            16, 8, 26, 14, 18, 11, 0, 0, 15, 2, 2, 3, 5, 3, 4, 6,
            10, 12, 12, 13, 90, 90, 0, 40, 103,97, 15, 0, 8, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 34, 8,
            13, 0, 6, 0, 0, 13, 0, 11, 11, 0, 0, 0, 16, 17, 7, 1,
            15, 14, 42, 10, 3, 0, 0, 14, 7, 26, 40, 0, 5, 6, 38, 5,
            7, 2, 7, 21, 0, 7, 7, 16, 21, 12, 12, 16, 16, 10, 1, 1,
            1, 1, 1, 32, 10, 13, 6, 2, 21, 6, 13, 8, 6, 18, 5, 10,
            4, 20, 29, 0, 0, 0, 0, 0, 0, 2, 6, 6, 11, 7, 10, 33,
            13, 26, 6, 8, 0, 13, 9, 1, 7, 16, 17, 7, 0, 0, 7, 8,
            10, 7, 8, 24, 3, 8, 0, 7, 0, 7, 0, 7, 0, 0, 0, 0,
            1
        };

        internal event EventHandler<D2gsPacket> PacketReceived;

        internal event EventHandler<D2gsPacket> PacketSent;

        internal override void Initialize()
        {
            if (D2gs.NEGOTIATECOMPRESSION != (D2gs)_stream.ReadByte())
            {
                throw new UnableToConnectException("Unexpected packet");
            }
            switch (_stream.ReadByte())
            {
                case 0x00:
                    Log.Debug("No compression for D2GS");
                    break;
                case 0x01:
                    Log.Debug("Default compression mode");
                    throw new NotImplementedException("D2GS Compression not implemented");
                default:
                    throw new UnableToConnectException("Unknown compression mode, cannot continue");
            }
        }

        internal override void WritePacket(byte[] packet)
        {
            _stream.Write(packet, 0, packet.Length);
            PacketSent?.Invoke(this, new D2gsPacket(packet));
        }

        internal override byte[] ReadPacket()
        {
            var buffer = new List<byte>();
            buffer.Add((byte)_stream.ReadByte());

            var identifier = buffer[0];
            switch(identifier)
            {
                case 0x26:
                    buffer.AddRange(ReadBytes(11));
                    buffer.AddRange(GetChatPacket(buffer));
                    break;
                case 0x5b:
                    buffer.AddRange(ReadBytes(2));
                    buffer.AddRange(ReadBytes(BitConverter.ToInt16(buffer.ToArray(), 1) - 3));
                    break;
                case 0x94:
                    buffer.AddRange(ReadBytes(1));
                    buffer.AddRange(ReadBytes((buffer[1] * 3) + 4));
                    break;
                case 0xa8:
                case 0xaa:
                    buffer.AddRange(ReadBytes(6));
                    buffer.AddRange(ReadBytes(buffer[6] - 7));
                    break;
                case 0xac:
                    buffer.AddRange(ReadBytes(12));
                    buffer.AddRange(ReadBytes(buffer[12] - 13));
                    break;
                case 0xae:
                    buffer.AddRange(ReadBytes(2));
                    buffer.AddRange(ReadBytes(BitConverter.ToInt16(buffer.ToArray(), 1)));
                    break;
                case 0x9c:
                case 0x9d:
                    buffer.AddRange(ReadBytes(2));
                    buffer.AddRange(ReadBytes(buffer[2] - 3));
                    break;
                default:
                    if (identifier >= PacketSizes.Length)
                    {
                        throw new D2GSPacketException("Unable to determine packet size");
                    }
                    buffer.AddRange(ReadBytes(PacketSizes[identifier] - 1));
                    break;
            }
            PacketReceived?.Invoke(this, new D2gsPacket(buffer.ToArray()));
            return buffer.ToArray();
        }

        private List<byte> ReadBytes(int count)
        {
            var buffer = new List<byte>();
            while (buffer.Count < count)
            {
                byte temp = (byte)_stream.ReadByte();
                buffer.Add(temp);
            }
            return buffer;
        }

        private List<byte> GetChatPacket(List<byte> buffer)
        {
            const int initialOffset = 10;

            int nameOffset = buffer.IndexOf(0, initialOffset);

            if (nameOffset == -1)
            {
                throw new D2GSPacketException("Unable to determine packet size");
            }

            nameOffset -= initialOffset;

            int messageOffset = buffer.IndexOf(0, initialOffset + nameOffset + 1);

            if (messageOffset == -1)
            {
                throw new D2GSPacketException("Unable to determine packet size");
            }

            messageOffset = messageOffset - initialOffset - nameOffset - 1;

            var length = initialOffset + nameOffset + 1 + messageOffset + 1;

            return ReadBytes(length - buffer.Count);
        }
    }
}
using Serilog;
using System.IO;
using System.Text;

namespace D2NG.BNCS.Packet
{
    public class LogonResponsePacket : BncsPacket
    {
        public static readonly byte SidByte = 0x3A;
        private uint _status;

        public LogonResponsePacket(byte[] packet) : base(packet)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(packet), Encoding.ASCII);
            if (PrefixByte != reader.ReadByte())
            {
                throw new BncsPacketException("Not a valid BNCS Packet");
            }
            if (SidByte != reader.ReadByte())
            {
                throw new BncsPacketException("Expected type was not found");
            }
            if (packet.Length != reader.ReadUInt16())
            {
                throw new BncsPacketException("Packet length does not match");
            }

            _status = reader.ReadUInt32();

            switch (_status)
            {
                case 0x00:
                    Log.Debug("Logon success");
                    break;
                case 0x01:
                    throw new LogonFailedException("Account does not exist");
                case 0x02:
                    throw new LogonFailedException("Invalid Password");
                case 0x06:
                    string message = reader.ReadString();
                    throw new LogonFailedException($"Account closed {message}");
                default:
                    throw new LogonFailedException($"Unknown login error {_status:X}");
            }
        }
    }
}
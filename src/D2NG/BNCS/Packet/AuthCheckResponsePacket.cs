using System.IO;
using System.Text;
using Serilog;

namespace D2NG.BNCS.Packet
{
    internal class AuthCheckResponsePacket : BncsPacket
    {
        private readonly uint _result;

        private readonly string _info;
        public AuthCheckResponsePacket(BncsPacket bncsPacket) : this(bncsPacket.Raw)
        {
        }

        public AuthCheckResponsePacket(byte[] packet) : base(packet)
        {
            var reader = new BinaryReader(new MemoryStream(packet), Encoding.ASCII);
            if (PrefixByte != reader.ReadByte())
            {
                throw new BncsPacketException("Not a valid BNCS Packet");
            }
            if ((byte)Sid.AUTH_CHECK != reader.ReadByte())
            {
                throw new BncsPacketException("Expected type was not found");
            }
            var packetSize = reader.ReadUInt16();
            if (packet.Length != packetSize)
            {
                throw new BncsPacketException("Packet length does not match");
            }
            
            _result = reader.ReadUInt32();
            _info = Encoding.ASCII.GetString(reader.ReadBytes(packetSize - 8));

            ValidateResult();
        }

        private void ValidateResult()
        { 
            switch (_result)
            {
                case 0x000:
                    Log.Verbose("Auth Check OK");
                    break;
                case 0x100:
                    throw new AuthCheckResponseException($" Old game version: {_info}");
                case 0x101:
                    throw new AuthCheckResponseException("Invalid game version");
                case 0x102:
                    throw new AuthCheckResponseException("Game version must be downgraded");
                case 0x200:
                    throw new AuthCheckResponseException("STATUS_INVALID_CD_KEY");
                case 0x210:
                    throw new AuthCheckResponseException("STATUS_INVALID_EXP_CD_KEY");
                case 0x201:
                    throw new AuthCheckResponseException("STATUS_KEY_IN_USE");
                case 0x211:
                    throw new AuthCheckResponseException("STATUS_EXP_KEY_IN_USE");
                case 0x202:
                    throw new AuthCheckResponseException("STATUS_BANNED_CD_KEY");
                case 0x212:
                    throw new AuthCheckResponseException("STATUS_BANNED_EXP_CD_KEY");
                case 0x203:
                    throw new AuthCheckResponseException("WRONG_PRODUCT");
                default:
                    throw new UnknownAuthCheckResultException("Unable to determine result of AuthCheck");
            }
        }
    }
}
using System;
using System.IO;
using System.Text;
using Serilog;

namespace D2NG.BNCS.Packet
{
    internal class BncsAuthCheckResponsePacket : BncsPacket
    {
        private const byte AuthCheckType = 0x51;

        private readonly uint _result;

        private readonly string _info;

        public BncsAuthCheckResponsePacket(byte[] packet) : base(packet)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(packet), Encoding.ASCII);
            if (PrefixByte != reader.ReadByte())
            {
                throw new BncsPacketException("Not a valid BNCS Packet");
            }

            if (AuthCheckType != reader.ReadByte())
            {
                throw new BncsPacketException("Expected type was not found");
            }
            
            var packetSize = reader.ReadUInt16();
            _result = reader.ReadUInt32();
            _info = Encoding.ASCII.GetString(reader.ReadBytes(packetSize - 8));

            InspectResult();
        }


        private void InspectResult()
        {
            switch (_result)
            {
                case 0x000:
                    Log.Debug("Auth Check OK");
                    break;
                case 0x100:
                    throw new AuthCheckResponseException(String.Format(" Old game version: {0}", _info));
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
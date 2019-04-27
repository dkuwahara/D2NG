using System;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using D2NG.BNCS.Packet;
using Serilog;

namespace D2NG
{
    internal class BncsAuthCheckResponsePacket : BncsPacket
    {
        private const byte AuthCheckType = 0x51;

        private ushort _packetSize;

        private uint _result;

        private string _info;

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

            _packetSize = reader.ReadUInt16();
            _result = reader.ReadUInt32();
            _info = Encoding.ASCII.GetString(reader.ReadBytes(_packetSize - 8));

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
                    Log.Debug("[BNCS] Old game version");
                    Log.Debug("Info: {0}", _info);
                    break;
                case 0x101:
                    Log.Debug("[BNCS] Invalid game version");
                    break;
                case 0x102:
                    break;
                case 0x200:
                    Log.Debug("[BNCS] STATUS_INVALID_CD_KEY");
                    break;
                case 0x210:
                    Log.Debug("[BNCS] STATUS_INVALID_EXP_CD_KEY");
                    break;
                case 0x201:
                    Log.Debug("[BNCS] STATUS_KEY_IN_USE");
                    break;
                case 0x211:
                    Log.Debug("[BNCS] STATUS_EXP_KEY_IN_USE");
                    break;
                case 0x202:
                    Log.Debug("[BNCS] STATUS_BANNED_CD_KEY");
                    break;
                case 0x212:
                    Log.Debug("[BNCS] STATUS_BANNED_EXP_CD_KEY");
                    break;
                case 0x203:
                    break;
            }
        }
    }
}
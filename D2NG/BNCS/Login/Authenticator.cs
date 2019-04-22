using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace D2NG.BNCS.Login
{
    class Authenticator
    {
        private const String EXE_INFO = "Game.exe 03/09/10 04:10:51 61440";

        static public readonly byte[] NULL_INT_AS_BYTE_ARRAY = { 0x00, 0x00, 0x00, 0x00 };
        static public readonly byte[] TEN = { 0x10, 0x00, 0x00, 0x00 };
        static public readonly byte[] SIX = { 0x06, 0x00, 0x00, 0x00 };
        static public readonly byte[] ZERO_BYTE = { 0x00 };
        static public readonly byte[] ONE = { 0x01, 0x00, 0x00, 0x00 };

        public CdKey ClassicKey { get; set; }
        public CdKey ExpansionKey { get; set; }

        private BattleNetChatServer _bncs;

        public Authenticator(BattleNetChatServer bncs)
        {
            this._bncs = bncs;
        }

        private static string ReadNullTerminatedString(string packet, ref int offset)
        {
            int zero = packet.IndexOf('\0', offset);
            string output;
            if (zero == -1)
            {
                zero = packet.Length;
                output = packet.Substring(offset, zero - offset);
                offset = 0;
            }
            else
            {
                output = packet.Substring(offset, zero - offset);
                offset = zero + 1;
            }
            return output;
        }

        public void AuthInfoRequest(BNCSPacketReceivedEvent obj)
        {
            var packet = obj.Packet;
            var data = new List<byte>();
            data.AddRange(packet);

            var serverToken = BitConverter.ToUInt32(data.ToArray(), 8);
            var temp = data.GetRange(16, 8);
            var mpq_file_time = Encoding.ASCII.GetString(temp.ToArray());

            var offset = data[24] == 0x76 ? 24 : 24;

            var mpqFileName = ReadNullTerminatedString(Encoding.ASCII.GetString(data.ToArray()), ref offset);
            var formulaString = ReadNullTerminatedString(Encoding.ASCII.GetString(data.ToArray()), ref offset);

            /*
             * Download MPQ would go here.
             */

            var exeChecksum = AdvancedCheckRevision.FastComputeHash(formulaString, mpqFileName,
                Path.Combine("data", "Game.exe"),
                Path.Combine("data", "Bnclient.dll"),
                Path.Combine("data", "D2Client.dll"));

            var clientToken = (uint)Environment.TickCount;

            List<byte> classicHash = new List<byte>(),
                lodHash = new List<byte>(),
                classicPublic = new List<byte>(),
                lodPublic = new List<byte>();
            _classicKey.GetD2KeyHash(ref clientToken, serverToken, ref classicHash, ref classicPublic);
            _expansionKey.GetD2KeyHash(ref clientToken, serverToken, ref lodHash, ref lodPublic);

            _bncs.SendPacket(0x51, BitConverter.GetBytes(clientToken), BitConverter.GetBytes(0x01000001),
                BitConverter.GetBytes(exeChecksum), BitConverter.GetBytes(0x00000002), NULL_INT_AS_BYTE_ARRAY,
                TEN, SIX, classicPublic, NULL_INT_AS_BYTE_ARRAY, classicHash, TEN,
                BitConverter.GetBytes((UInt32)10), lodPublic, NULL_INT_AS_BYTE_ARRAY, lodHash,
                Encoding.UTF8.GetBytes(EXE_INFO),
                ZERO_BYTE, Encoding.ASCII.GetBytes("D2NG"), ZERO_BYTE);
        }
    }
}

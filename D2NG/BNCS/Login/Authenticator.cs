using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace D2NG.BNCS.Login
{
    class Authenticator
    {
        private const String EXE_INFO = "Game.exe 05/31/16 19:02:24 3618792";
        private const string DATA_DIRECTORY = "data";
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

        private const String PLATFORM = "68XI", CLASSIC_ID = "VD2D", LOD_ID = "PX2D";

        public void AuthInfoRequest(BNCSPacketReceivedEvent obj)
        {
            var data = new List<byte>();
            data.AddRange(obj.Packet);

            var serverToken = BitConverter.ToUInt32(data.ToArray(), 8);
            var temp = data.GetRange(16, 8);
            var mpq_file_time = Encoding.ASCII.GetString(temp.ToArray());

            var offset = data[24] == 0x76 ? 24 : 24;

            var mpqFileName = ReadNullTerminatedString(Encoding.ASCII.GetString(data.ToArray()), ref offset);
            var formulaString = ReadNullTerminatedString(Encoding.ASCII.GetString(data.ToArray()), ref offset);

            Log.Debug("Server Token: {0} Temp: {1}, MPQ File Time: {2}, Offset: {3}, MPQ File Name: {4}, Formula String: {5}", serverToken, temp, mpq_file_time, offset, mpqFileName, formulaString);

            /*
             * Download MPQ would go here.
             */

            var exeChecksum = AdvancedCheckRevision.FastComputeHash(
                formulaString,
                mpqFileName,
                Path.Combine(DATA_DIRECTORY, "Game.exe"),
                Path.Combine(DATA_DIRECTORY, "Bnclient.dll"),
                Path.Combine(DATA_DIRECTORY, "D2Client.dll"));

            var clientToken = (uint)Environment.TickCount;

            List<byte> classicHash = new List<byte>(),
                lodHash = new List<byte>(),
                classicPublic = new List<byte>(),
                lodPublic = new List<byte>();
            ClassicKey.GetD2KeyHash(ref clientToken, serverToken, ref classicHash, ref classicPublic);
            ExpansionKey.GetD2KeyHash(ref clientToken, serverToken, ref lodHash, ref lodPublic);

            _bncs.SendPacket(0x51, BitConverter.GetBytes(clientToken), BitConverter.GetBytes(0x01000001),
                BitConverter.GetBytes(exeChecksum), BitConverter.GetBytes(0x00000002), NULL_INT_AS_BYTE_ARRAY,
                TEN, SIX, classicPublic, NULL_INT_AS_BYTE_ARRAY, classicHash, TEN,
                BitConverter.GetBytes((UInt32)10), lodPublic, NULL_INT_AS_BYTE_ARRAY, lodHash,
                Encoding.UTF8.GetBytes(EXE_INFO),
                ZERO_BYTE, Encoding.ASCII.GetBytes("D2NG"), ZERO_BYTE);
        }
    }
}

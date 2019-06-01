using D2NG.D2GS.Packet;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace D2NG.D2GS.Quest.Packet
{
    class QuestInfoPacket : D2gsPacket
    {
        public QuestInfoPacket(D2gsPacket packet) : base(packet.Raw)
        {
            var reader = new BinaryReader(new MemoryStream(Raw), Encoding.ASCII);
            var id = reader.ReadByte();
            if (id != 0x28)
            {
                throw new D2GSPacketException("Invalid packet id found");
            }
            UpdateType = reader.ReadByte();
            UnitGid = reader.ReadUInt32();
            Timer = reader.ReadByte();
            for (int i = 0; i < 96; i++)
            {
                Quests[i] = reader.ReadByte();
            }

            Log.Verbose($"(0x{id, 2:X2}) Player Quest Info:\n" +
                $"\tUpdate Type: {UpdateType}\n" +
                $"\tUnit Gid: {UnitGid}\n" +
                $"\tTimer: {Timer}\n" +
                $"\tQuests: {Quests.Aggregate("", (s, i) => s + "," + $"{i,2:X2}")}");
        }

        public byte UpdateType { get; }
        public uint UnitGid { get; }
        public byte Timer { get; }

        public byte[] Quests { get; } = new byte[96];
    }
}

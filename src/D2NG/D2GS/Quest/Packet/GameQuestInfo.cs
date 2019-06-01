using D2NG.D2GS.Packet;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace D2NG.D2GS.Quest.Packet
{
    class GameQuestInfoPacket : D2gsPacket
    {
        public GameQuestInfoPacket(D2gsPacket packet) : base(packet.Raw)
        {
            var reader = new BinaryReader(new MemoryStream(Raw), Encoding.ASCII);
            var id = reader.ReadByte();
            if (id != 0x29)
            {
                throw new D2GSPacketException("Invalid packet id found");
            }
            for (int i = 0; i < 96; i++)
            {
                Quests[i] = reader.ReadByte();
            }

            Log.Verbose($"(0x{id,2:X2}) Game Quest Info:\n" +
                $"\tQuests: {Quests.Aggregate("", (s, i) => s + i)}");
        }

        public byte[] Quests { get; } = new byte[96];
    }
}

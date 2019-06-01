using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace D2NG.D2GS.Packet.Server
{
    public class BaseSkillLevelsPacket
    {
        internal BaseSkillLevelsPacket(D2gsPacket packet)
        {
            var reader = new BinaryReader(new MemoryStream(packet.Raw), Encoding.ASCII);
            if (reader.ReadByte() != 0x94)
            {
                throw new D2GSPacketException("Invalid packet id");
            }
            var count = reader.ReadByte();
            PlayerId = reader.ReadUInt32();
            
            for (int i = 0; i < count; i ++)
            {
                Skills[(Skill)reader.ReadUInt16()] = reader.ReadByte();
            }
            reader.Close();

            Log.Verbose($"(0x{packet.Raw[0],2:X2}) Base Skill Levels:\n" +
                string.Join("\n", Skills
                            .OrderBy(s => s.Key.ToString())
                            .Select(s => $"\t{s.Key} : {s.Value}")));
        }

        public ConcurrentDictionary<Skill, int> Skills { get; } = new ConcurrentDictionary<Skill, int>();
        public uint PlayerId { get; }
    }
}
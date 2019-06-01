using Serilog;
using System.IO;
using System.Text;

namespace D2NG.D2GS.Packet.Server
{
    internal class SetActiveSkillPacket
    {
        public byte UnitType { get; }
        public uint UnitGid { get; }
        public Hand Hand { get; }
        public Skill Skill { get; }
        public uint ItemGid { get; }

        public SetActiveSkillPacket(D2gsPacket packet)
        {
            var reader = new BinaryReader(new MemoryStream(packet.Raw), Encoding.ASCII);
            if (reader.ReadByte() != 0x23)
            {
                throw new D2GSPacketException("Unexpected packet id");
            }
            UnitType = reader.ReadByte();
            UnitGid = reader.ReadUInt32();
            Hand = (Hand) reader.ReadByte();
            Skill = (Skill)reader.ReadUInt16();
            ItemGid = reader.ReadUInt32();

            Log.Verbose($"(0x{packet.Raw[0], 2:X2}) Set Skill:\n" +
                $"\tUnit Type: {UnitType}\n" +
                $"\tUnit GID: {UnitGid}\n" +
                $"\tHand: {Hand}\n" +
                $"\tSkill: {Skill}\n" +
                $"\tItemGid: {ItemGid}");
        }
    }
}
using D2NG.D2GS;
using D2NG.D2GS.Packet;
using Serilog;
using System.IO;
using System.Text;

namespace D2NG
{
    internal class SetItemSkillPacket : D2gsPacket
    {
        public SetItemSkillPacket(D2gsPacket packet) : base(packet.Raw)
        {
            var reader = new BinaryReader(new MemoryStream(packet.Raw), Encoding.ASCII);
            var id = reader.ReadByte();
            _ = reader.ReadUInt16();
            UnitId = reader.ReadUInt32();
            Skill = (Skill)reader.ReadUInt16();
            switch(id)
            {
                case 0x21:
                    BaseLevel = reader.ReadByte();
                    goto case 0x22;
                case 0x22:
                    Amount = reader.ReadByte();
                    break;
                default:
                    throw new D2GSPacketException("Invalid packet id");
            }
            reader.Close();

            Log.Verbose($"(0x{packet.Raw[0],2:X2}) Set Skill:\n" +
                        $"\tUnit ID: {UnitId}\n" +
                        $"\tSkill: {Skill}\n" +
                        $"\tAmount: {Amount}");
        }

        public uint UnitId { get; }
        public Skill Skill { get; }
        public int BaseLevel { get; }
        public int Amount { get; }
    }
}
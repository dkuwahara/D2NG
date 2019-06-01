using Serilog;

namespace D2NG.D2GS.Packet.Server
{
    internal class UpdateSelfPacket : D2gsPacket
    {
        public UpdateSelfPacket(D2gsPacket packet) : base(packet.Raw)
        {
            var reader = new BitReader(Raw);
            var id = reader.ReadByte();
            Life = reader.Read(15);
            Mana = reader.Read(15);
            Stamina = reader.Read(15);
            LifeRegen = reader.Read(7);
            ManaRegen = reader.Read(7);
            var x = reader.ReadUInt16();
            var y = reader.ReadUInt16();
            Location = new Point(x, y);
            Log.Verbose($"(0x{id, 2:X2})Update Self Packet:\n" +
                        $"\tLife: {Life}\n" +
                        $"\tMana: {Mana}\n" +
                        $"\tLife Regen: {LifeRegen}\n" +
                        $"\tMana Regen: {ManaRegen}\n" +
                        $"\tLocation: {Location}");
        }

        public int Life { get; }
        public int Mana { get; }
        public int Stamina { get; }
        public int LifeRegen { get; }
        public int ManaRegen { get; }
        public Point Location { get; }
    }
}
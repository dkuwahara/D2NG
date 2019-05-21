namespace D2NG.MCP
{
    public class Character
    {
        public Character(uint expiry, string name, string statstring)
        {
            Expiry = expiry;
            Name = name;
            Statstring = statstring;
        }

        public byte CharacterClass { get => (byte) ((Statstring[13] - 0x01) & 0xFF); }

        public byte Level { get => (byte)((Statstring[25]) & 0xFF); }

        public uint Expiry { get; }
        public string Name { get; }
        public string Statstring { get; }
    }
}
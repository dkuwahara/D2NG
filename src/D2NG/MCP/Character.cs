using System.Text;

namespace D2NG.MCP
{
    public class Character
    {
        public Character(uint expiry, string name, string statstring)
        {
            Expiry = expiry;
            Name = name;
            Statstring = Encoding.ASCII.GetBytes(statstring);
        }

        public byte CharacterClass { get => (byte)((Statstring[13] - 0x01) & 0xFF); }

        public byte Level { get => Statstring[25]; }

        public bool Hardcore { get => (Statstring[26] & 0x04) != 0; }

        public bool Expansion { get => (Statstring[26] & 0x20) != 0; }

        public uint Expiry { get; }
        public string Name { get; }
        public byte[] Statstring { get; }
    }
}
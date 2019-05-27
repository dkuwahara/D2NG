using System.Text;

namespace D2NG.MCP
{
    public class Character
    {
        private readonly byte[] _stats;

        public string Name { get; }

        public Character(string name, string statstring)
        {
            Name = name;
            _stats = Encoding.ASCII.GetBytes(statstring);
        }

        public CharacterClass CharacterClass { get => (CharacterClass)((_stats[13] - 0x01) & 0xFF); }

        public uint Level { get => _stats[25]; }
    }
}
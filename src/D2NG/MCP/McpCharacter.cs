namespace D2NG
{
    public class McpCharacter
    {
        public McpCharacter(uint expiry, string name, string statstring)
        {
            Expiry = expiry;
            Name = name;
            Statstring = statstring;
        }

        public uint Expiry { get; }
        public string Name { get; }
        public string Statstring { get; }
    }
}
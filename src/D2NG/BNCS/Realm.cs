namespace D2NG.BNCS.Packet
{
    public class Realm
    {
        public string Name { get; }
        public string Description { get; }

        public Realm(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }
    }
}
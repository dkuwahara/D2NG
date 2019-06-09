namespace D2NG.D2GS
{
    public abstract class Entity
    {
        public byte Type { get; protected set; }
        public uint Id { get; protected set; }
        public Point Location { get; protected set; }
    }
}
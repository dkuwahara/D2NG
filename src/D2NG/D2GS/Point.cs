namespace D2NG.D2GS
{
    public class Point
    {
        public ushort X { get; }
        public ushort Y { get; }

        public Point(ushort x, ushort y) => (X, Y) = (x, y);

        public void Deconstruct(out ushort x, out ushort y) => (x, y) = (X, Y);

        public override string ToString() => $"({X}, {Y})";
    }
}

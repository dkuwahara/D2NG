using System;

namespace D2NG.D2GS
{
    public class Point
    {
        public ushort X { get; }
        public ushort Y { get; }

        public Point(ushort x, ushort y) => (X, Y) = (x, y);

        public void Deconstruct(out ushort x, out ushort y) => (x, y) = (X, Y);

        public double Distance(Point other) 
            => Math.Sqrt(Math.Pow(X - other.X, 2.0) + Math.Pow(Y - other.Y, 2.0));
        public override string ToString() => $"({X}, {Y})";
    }
}

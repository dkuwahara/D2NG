using System;
using System.Collections.Generic;
using System.Text;

namespace D2NG.D2GS.Act
{
    public class Level
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }

        public Level(int w, int h, int _x, int _y, String s, int t)
        {
            Width = w;
            Height = h;
            X = _x;
            Y = _y;
            Name = s;
            Type = t;
        }
    }
}

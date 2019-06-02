using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using D2NG.D2GS.Act.Packet;
using Serilog;

namespace D2NG.D2GS.Act
{
    class ActData
    {
        private byte Act { get; set; }
        private uint MapId { get; set; }
        public Area Area { get; set; }

        private readonly ConcurrentDictionary<byte, List<Tile>> _tiles = new ConcurrentDictionary<byte, List<Tile>>();

        public List<Tile> Tiles { get => _tiles.GetOrAdd(Act, new List<Tile>()); }

        internal void LoadActData(ActDataPacket packet)
        {
            Act = packet.Act;
            Area = packet.Area;
            MapId = packet.MapId;
        }

        internal void AddTile(MapRevealPacket p)
        {
            var tile = new Tile(p.X, p.Y, p.Area);
            if(!Tiles.Any(item => item.Equals(tile)))
            {
                lock(Tiles)
                {
                    Tiles.Add(tile);
                    foreach(var t in Tiles)
                    {
                        t.North = Tiles.DefaultIfEmpty(null).FirstOrDefault(w => w.NorthOf(t));
                        t.East = Tiles.DefaultIfEmpty(null).FirstOrDefault(w => w.EastOf(t));
                        t.South = Tiles.DefaultIfEmpty(null).FirstOrDefault(w => w.SouthOf(t));
                        t.West = Tiles.DefaultIfEmpty(null).FirstOrDefault(w => w.WestOf(t));
                    }
                }
            }
        }
    }
}

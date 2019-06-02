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
        private Area Area { get; set; }

        private readonly ConcurrentDictionary<byte, List<Tile>> Tiles = new ConcurrentDictionary<byte, List<Tile>>();

        internal void LoadActData(ActDataPacket packet)
        {
            Act = packet.Act;
            Area = packet.Area;
            MapId = packet.MapId;
        }

        internal void AddTile(MapRevealPacket p)
        {
            var tile = new Tile(p.X, p.Y, p.Area);
            var tiles = Tiles.GetOrAdd(Act, new List<Tile>());
            if(!tiles.Any(item => item.Equals(tile)))
            {
                lock(tiles)
                {
                    tiles.Add(tile);
                    
                    foreach(var t in tiles)
                    {
                        t.North = tiles.DefaultIfEmpty(null).FirstOrDefault(w => w.NorthOf(t));
                        t.East = tiles.DefaultIfEmpty(null).FirstOrDefault(w => w.EastOf(t));
                        t.South = tiles.DefaultIfEmpty(null).FirstOrDefault(w => w.SouthOf(t));
                        t.West = tiles.DefaultIfEmpty(null).FirstOrDefault(w => w.WestOf(t));
                    }
                }
            }
        }
    }
}

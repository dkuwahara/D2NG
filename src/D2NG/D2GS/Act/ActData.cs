using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using D2NG.D2GS.Act.Packet;
using D2NG.D2GS.Objects.Packet;
using Serilog;

namespace D2NG.D2GS.Act
{
    class ActData
    {
        private byte Act { get; set; }
        private uint MapId { get; set; }
        public Area Area { get; set; }

        private readonly ConcurrentDictionary<byte, List<Tile>> _tiles = new ConcurrentDictionary<byte, List<Tile>>();
        private readonly ConcurrentDictionary<byte, ConcurrentDictionary<uint, Entity>> _entities 
            = new ConcurrentDictionary<byte, ConcurrentDictionary<uint, Entity>>();
        private readonly ConcurrentDictionary<byte, ConcurrentDictionary<uint, WorldObject>> _worldObjects
            = new ConcurrentDictionary<byte, ConcurrentDictionary<uint, WorldObject>>();

        public List<Tile> Tiles { get => _tiles.GetOrAdd(Act, new List<Tile>()); }
        public ConcurrentDictionary<uint, Entity> Entities { get => _entities.GetOrAdd(Act, new ConcurrentDictionary<uint, Entity>()); }
        public ConcurrentDictionary<uint, WorldObject> WorldObjects { get => _worldObjects.GetOrAdd(Act, new ConcurrentDictionary<uint, WorldObject>()); }

        internal void LoadActData(ActDataPacket packet)
        {
            Act = packet.Act;
            Area = packet.Area;
            MapId = packet.MapId;
        }

        internal void AddWorldObject(WorldObject obj)
        {
            Entities[obj.Id] = obj;
            WorldObjects[obj.Id] = obj;
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
                        if(t.IsNorthOf(tile))
                        {
                            tile.North = t;
                            t.South = tile;
                        }
                        if(t.IsEastOf(tile))
                        {
                            tile.East = t;
                            t.West = tile;
                        }
                        if (t.IsSouthOf(tile))
                        {
                            tile.South = t;
                            t.North = tile;
                        }
                        if (t.IsWestOf(tile))
                        {
                            tile.West = t;
                            t.East = tile;
                        }
                    }
                }
            }
        }
    }
}

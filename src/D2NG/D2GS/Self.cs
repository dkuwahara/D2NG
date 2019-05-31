using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace D2NG.D2GS
{
    public class Self : Player
    {
        public ConcurrentDictionary<Hand, Skill> Skills { get ; } = new ConcurrentDictionary<Hand, Skill>();

        public ConcurrentDictionary<Attribute, int> Attributes { get; } = new ConcurrentDictionary<Attribute, int>();
        public uint Experience { get; internal set; } = 0;

        internal Self(AssignPlayerPacket assignPlayer) : base(assignPlayer)
        {
        }
    }
}

﻿using D2NG.D2GS.Packet.Server;
using System.Collections.Concurrent;

namespace D2NG.D2GS
{
    public class Self : Player
    {
        public ConcurrentDictionary<Hand, Skill> ActiveSkills { get; } = new ConcurrentDictionary<Hand, Skill>();
        public ConcurrentDictionary<Skill, int> Skills { get; } = new ConcurrentDictionary<Skill, int>();
        public ConcurrentDictionary<Skill, int> ItemSkills { get; internal set; } = new ConcurrentDictionary<Skill, int>();
        public ConcurrentDictionary<Attribute, int> Attributes { get; } = new ConcurrentDictionary<Attribute, int>();
        public uint Experience { get; internal set; }
        public int Life { get; internal set; }
        public int Mana { get; internal set; }
        public int Stamina { get; internal set; }

        internal Self(AssignPlayerPacket assignPlayer) : base(assignPlayer)
        {
        }
    }
}

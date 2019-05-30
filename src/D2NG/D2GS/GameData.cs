using D2NG.D2GS;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace D2NG
{
    class GameData
    {
        private GameFlags _gameFlags;
        internal GameData(GameFlags gameFlags)
        {
            _gameFlags = gameFlags;
        }

        public Difficulty Difficulty { get => _gameFlags.Difficulty; }
        public bool Hardcore { get => _gameFlags.Hardcore;  }
        public bool Ladder { get => _gameFlags.Ladder; }
        public bool Expansion { get => _gameFlags.Expansion; }
        public List<Player> Players { get; internal set; } = new List<Player>();

        internal void AssignPlayer(AssignPlayer assignPlayer)
        {
            Players.Add(new Player(assignPlayer));
        }

        internal void SetSkill(SetSkill setSkill)
        {
        }
    }
}

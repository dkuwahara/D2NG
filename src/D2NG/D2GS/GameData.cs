using D2NG.D2GS;
using D2NG.MCP;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace D2NG
{
    class GameData
    {
        private Character _character;
        private GameFlags _gameFlags;

        internal GameData(MCP.Character character, GameFlags gameFlags)
        {
            _character = character;
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

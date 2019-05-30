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

        internal GameData(MCP.Character character, GameFlags gameFlags)
        {
            _character = character;
            Flags = gameFlags;
        }

        public GameFlags Flags { get; }
        public Player Me { get; private set; }
        public List<Player> Players { get; internal set; } = new List<Player>();

        internal void AssignPlayer(AssignPlayer assignPlayer)
        {
            if (assignPlayer.Name == _character.Name)
            {
                Log.Verbose("Assigning self");
                Me = new Player(assignPlayer);
            }
            else
            {
                Log.Verbose("Assigning other player");
                Players.Add(new Player(assignPlayer));
            }
        }

        internal void SetSkill(SetSkill setSkill)
        {
        }
    }
}

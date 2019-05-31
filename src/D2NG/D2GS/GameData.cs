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

        internal GameData(GameFlags gameFlags)
        {
            Flags = gameFlags;
        }

        public GameFlags Flags { get; }
        public Self Me { get; private set; }
        public List<Player> Players { get; internal set; } = new List<Player>();

        internal void AssignPlayer(AssignPlayerPacket packet)
        {
            if (packet.Location.X == 0x00 && packet.Location.Y == 0x00)
            {
                Log.Verbose("Assigning self");
                Me = new Self(packet);
            }
            else
            {
                Log.Verbose("Assigning other player");
                Players.Add(new Player(packet));
            }
        }

        internal void SetSkill(SetSkillPacket packet)
        {
            if(packet.UnitGid == Me.Id)
            {
                Me.SetSkill(packet.Hand, packet.Skill);
            }
        }
    }
}

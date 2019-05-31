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

        internal GameData(GameFlagsPacket gameFlags)
        {
            Flags = gameFlags;
        }

        public GameFlagsPacket Flags { get; }
        public Self Me { get; private set; }
        public List<Player> Players { get; internal set; } = new List<Player>();

        internal void AssignPlayer(AssignPlayerPacket packet)
        {
            if (packet.Location.X == 0x00 && packet.Location.Y == 0x00)
            {
                Me = new Self(packet);
            }
            else
            {
                Players.Add(new Player(packet));
            }
        }

        internal void SetAttribute(BaseAttributePacket baseAttribute)
            => Me.Attributes[baseAttribute.Attribute] = baseAttribute.Value;

        internal void SetSkill(SetSkillPacket packet)
        {
            if(packet.UnitGid == Me.Id)
            {
                Me.Skills[packet.Hand] = packet.Skill;
            }
        }
    }
}

﻿using D2NG.D2GS;
using D2NG.D2GS.Packet.Server;
using System.Collections.Generic;
using System.Linq;

namespace D2NG
{
    internal class GameData
    {
        internal GameData(GameFlags gameFlags)
        {
            Flags = gameFlags;
        }

        public GameFlags Flags { get; }
        public Self Me { get; private set; }
        public List<Player> Players { get; internal set; } = new List<Player>();

        internal void AddExperience(AddExpPacket addExpPacket)
            => Me.Experience += addExpPacket.Experience;

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

        internal void SetItemSkill(SetItemSkillPacket packet)
        {
            if (packet.UnitId == Me.Id)
            {
                Me.ItemSkills[packet.Skill] = packet.Amount;
            }
        }

        internal void SetSkills(BaseSkillLevelsPacket packet)
        {
            if(packet.PlayerId == Me.Id)
            {
                packet.Skills.Select(s => Me.Skills[s.Key] = s.Value);
            }
        }

        internal void SetActiveSkill(SetActiveSkillPacket packet)
        {
            if(packet.UnitGid == Me.Id)
            {
                Me.ActiveSkills[packet.Hand] = packet.Skill;
            }
        }

        internal void UpdateSelf(UpdateSelfPacket packet)
        {
            Me.Location = packet.Location;
            Me.Life = packet.Life;
            Me.Mana = packet.Mana;
            Me.Stamina = packet.Stamina;
        }
    }
}
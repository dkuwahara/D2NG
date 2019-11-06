using D2NG.D2GS;
using D2NG.D2GS.Act;
using D2NG.D2GS.Items;
using D2NG.D2GS.Items.Containers;
using D2NG.D2GS.Packet.Server;
using D2NG.D2GS.Players.Packet;
using D2NG.Items;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Action = D2NG.D2GS.Items.Action;

namespace D2NG
{
    internal class GameData
    {
        internal GameData(GameFlags gameFlags)
        {
            Flags = gameFlags;
            Act = new ActData();
        }

        public GameFlags Flags { get; }
        public ActData Act { get; }
        public Self Me { get; private set; }

        public Container Stash { get; } = new Stash();

        public List<Player> Players { get; internal set; } = new List<Player>();

        public ConcurrentDictionary<uint, Item> Items { get; private set; } = new ConcurrentDictionary<uint, Item>();

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

        internal void ReassignPlayer(ReassignPlayerPacket packet)
        {
            if (packet.UnitId == Me.Id)
            {
                Me.Location = packet.Location;
            }
            var tile = Act.Tiles.First(t => t.Contains(Me.Location));
            Log.Verbose($"Player Tile:\n" +
                $"\t{String.Join(", ", tile.Adjacent.Keys)}");
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

        internal void ItemUpdate(ParseItemPacket packet)
        {
            var item = packet.Item;
            Items[item.id] = packet.Item;
            switch (item.action)
            {
                case Action.put_in_container:
                    PutInContainer(item);
                    break;
                case Action.remove_from_container:
                    RemoveFromContainer(item);
                    break;
                case Action.add_quantity:
                    break;
                default:
                    // Do nothing because we don't know
                    break;
            }
        }

        private void RemoveFromContainer(Item item)
        {
            switch (item.container)
            {
                case ContainerType.stash:
                    Log.Verbose("Removing item from stash");
                    Stash.Remove(item);
                    break;
                case ContainerType.belt:
                    Log.Verbose("Removing item from belt");
                    break;
                case ContainerType.cube:
                    Log.Verbose("Removing item from cube");
                    break;
                default:
                    // Do nothing we don't know how to handle this
                    break;
            }
        }

        private void PutInContainer(Item item)
        {
            switch (item.container)
            {
                case ContainerType.stash:
                    Log.Verbose("Adding item to stash");
                    Stash.Add(item);
                    break;
                case ContainerType.belt:
                    Log.Verbose("Adding item from belt");
                    break;
                case ContainerType.cube:
                    Log.Verbose("Adding item from cube");
                    break;
                default:
                    // Do nothing we don't know how to handle this
                    break;
            }
        }
    }
}

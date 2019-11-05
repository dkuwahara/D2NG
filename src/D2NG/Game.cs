using D2NG.D2GS;
using D2NG.D2GS.Act.Packet;
using D2NG.D2GS.Items;
using D2NG.D2GS.Items.Containers;
using D2NG.D2GS.Objects.Packet;
using D2NG.D2GS.Packet;
using D2NG.D2GS.Packet.Server;
using D2NG.D2GS.Players.Packet;
using D2NG.D2GS.Quest.Packet;
using D2NG.Items;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace D2NG
{
    public class Game
    {
        private readonly GameServer _gameServer;

        private GameData Data { get; set; }

        internal Game(GameServer gameServer)
        {
            _gameServer = gameServer;

            _gameServer.OnReceivedPacketEvent(0x00, _ => Log.Information("Game loading..."));
            _gameServer.OnReceivedPacketEvent(0x01, p => Initialize(new GameFlags(p)));
            _gameServer.OnReceivedPacketEvent(0x01, p => _gameServer.Ping());
            _gameServer.OnReceivedPacketEvent(0x03, p => Data.Act.LoadActData(new ActDataPacket(p)));
            _gameServer.OnReceivedPacketEvent(0x07, p => Data.Act.AddTile(new MapRevealPacket(p)));
            _gameServer.OnReceivedPacketEvent(0x0B, p => new GameHandshakePacket(p));
            _gameServer.OnReceivedPacketEvent(0x15, p => Data.ReassignPlayer(new ReassignPlayerPacket(p)));
            _gameServer.OnReceivedPacketEvent(0x1A, p => Data.AddExperience(new AddExpPacket(p)));
            _gameServer.OnReceivedPacketEvent(0x1B, p => Data.AddExperience(new AddExpPacket(p)));
            _gameServer.OnReceivedPacketEvent(0x1C, p => Data.AddExperience(new AddExpPacket(p)));
            _gameServer.OnReceivedPacketEvent(0x1D, p => Data.SetAttribute(new BaseAttributePacket(p)));
            _gameServer.OnReceivedPacketEvent(0x1E, p => Data.SetAttribute(new BaseAttributePacket(p)));
            _gameServer.OnReceivedPacketEvent(0x1F, p => Data.SetAttribute(new BaseAttributePacket(p)));
            _gameServer.OnReceivedPacketEvent(0x21, p => Data.SetItemSkill(new SetItemSkillPacket(p)));
            _gameServer.OnReceivedPacketEvent(0x22, p => Data.SetItemSkill(new SetItemSkillPacket(p)));
            _gameServer.OnReceivedPacketEvent(0x23, p => Data.SetActiveSkill(new SetActiveSkillPacket(p)));
            _gameServer.OnReceivedPacketEvent(0x28, p => new QuestInfoPacket(p));
            _gameServer.OnReceivedPacketEvent(0x29, p => new GameQuestInfoPacket(p));
            _gameServer.OnReceivedPacketEvent(0x51, p => Data.Act.AddWorldObject(new AssignObjectPacket(p).AsWorldObject()));
            _gameServer.OnReceivedPacketEvent(0x59, p => Data.AssignPlayer(new AssignPlayerPacket(p)));
            _gameServer.OnReceivedPacketEvent(0x94, p => Data.SetSkills(new BaseSkillLevelsPacket(p)));
            _gameServer.OnReceivedPacketEvent(0x18, p => Data.UpdateSelf(new UpdateSelfPacket(p)));
            _gameServer.OnReceivedPacketEvent(0x8F, _ => { });
            _gameServer.OnReceivedPacketEvent(0x95, p => Data.UpdateSelf(new UpdateSelfPacket(p)));
            _gameServer.OnReceivedPacketEvent(0x9c, p => Data.ItemUpdate(new ParseItemPacket(p)));
            _gameServer.OnReceivedPacketEvent(0x9d, p => Data.ItemUpdate(new ParseItemPacket(p)));
        }

        private void Initialize(GameFlags packet)
        {
            Data = new GameData(packet);
        }

        public void LeaveGame()
        {
            Log.Information("Leaving game");
            _gameServer.LeaveGame();
        }

        public Self Me { get => Data.Me; }

        public List<Item> Items { get => Data.Items.Values.ToList(); }

        public IReadOnlyDictionary<uint, Entity> Entities { get => Data.Act.Entities; }

        public IReadOnlyDictionary<uint,WorldObject> WorldObjects { get => Data.Act.WorldObjects; }

        public Container Stash { get => Data.Stash; }
        public int LastTeleport { get; private set; }

        public void RemoveItemFromBuffer(Item item)
        {
            var packet = D2gsPacket.BuildPacket(0x19,
                    BitConverter.GetBytes(item.id)
                );
            _gameServer.SendPacket(packet);
        }

        public void SwitchSkill(Hand hand, Skill skill)
        {
            var packet = D2gsPacket.BuildPacket(0x3c,
                BitConverter.GetBytes((ushort)skill),
                hand == Hand.Left ? new byte[] { 0x00, 0x80 } :  new byte[] { 0x00, 0x00 },
                BitConverter.GetBytes(-1)
                );
            _gameServer.SendPacket(packet);
        }

        public void UseSkillOnUnit(Hand hand, Entity entity)
        {
            var packet = D2gsPacket.BuildPacket(
                (byte) (hand == Hand.Left ? 0X06 : 0x0D),
                BitConverter.GetBytes((uint)entity.Type),
                BitConverter.GetBytes(entity.Id));
            _gameServer.SendPacket(packet);
        }

        public void InsertItemToBuffer(Item item, Point location, Item.ItemContainer container)
        {
            var packet = D2gsPacket.BuildPacket(0x18,
                        BitConverter.GetBytes(item.id),
                        BitConverter.GetBytes((uint)location.X),
                        BitConverter.GetBytes((uint)location.Y),
                        BitConverter.GetBytes((uint)container)
                    );
            _gameServer.SendPacket(packet);
        }

        public void MoveTo(ushort x, ushort y) => MoveTo(new Point(x, y));
        public void MoveTo(Point location)
        {
            int time = Environment.TickCount / 1000;
            if (time - LastTeleport > 5)
            {
                var packet = D2gsPacket.BuildPacket(0x5f,
                    BitConverter.GetBytes(location.X),
                    BitConverter.GetBytes(location.Y));
                _gameServer.SendPacket(packet);
                Thread.Sleep(120);
            }
            else
            {
                var packet = D2gsPacket.BuildPacket(0x03,
                BitConverter.GetBytes(location.X),
                BitConverter.GetBytes(location.Y));
                _gameServer.SendPacket(packet);
                Thread.Sleep((int)Me.Location.Distance(location) * 80);
            }

            Me.Location = location;
        }

        public void MoveTo(Entity entity)
        {
            int time = Environment.TickCount / 1000;
            if (time - LastTeleport > 5)
            {
                var packet = D2gsPacket.BuildPacket(0x5f,
                    BitConverter.GetBytes(entity.Location.X),
                    BitConverter.GetBytes(entity.Location.Y));
                _gameServer.SendPacket(packet);
                Thread.Sleep(120);
            }
            else
            {
                var packet = D2gsPacket.BuildPacket(0x04,
                    BitConverter.GetBytes((uint)entity.Type),
                    BitConverter.GetBytes(entity.Id));
                _gameServer.SendPacket(packet);
                Thread.Sleep((int)Me.Location.Distance(entity.Location) * 80);
            }
            Me.Location = entity.Location;
        }

        public void Interact(Entity entity)
        {
            var packet = D2gsPacket.BuildPacket(0x13,
               BitConverter.GetBytes((uint)entity.Type),
               BitConverter.GetBytes(entity.Id));
            _gameServer.SendPacket(packet);
        }

        public void RequestUpdate(uint id)
        {
            var packet = D2gsPacket.BuildPacket(0x4b,
                BitConverter.GetBytes(0),
                BitConverter.GetBytes(id)
                );
            _gameServer.SendPacket(packet);
            Thread.Sleep(400);
        }
    }
}

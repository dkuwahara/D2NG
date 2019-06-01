﻿using D2NG.D2GS;
using D2NG.D2GS.Packet.Server;
using D2NG.D2GS.Quest.Packet;
using Serilog;

namespace D2NG
{
    public class Game
    {
        private readonly GameServer _gameServer;

        private GameData Data { get; set; }

        internal Game(GameServer gameServer)
        {
            _gameServer = gameServer;

            _gameServer.OnReceivedPacketEvent(0x01, p => Initialize(new GameFlags(p)));
            _gameServer.OnReceivedPacketEvent(0x01, p => _gameServer.Ping());
            _gameServer.OnReceivedPacketEvent(0x0B, p => new GameHandshakePacket(p));
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
            _gameServer.OnReceivedPacketEvent(0x59, p => Data.AssignPlayer(new AssignPlayerPacket(p)));
            _gameServer.OnReceivedPacketEvent(0x94, p => Data.SetSkills(new BaseSkillLevelsPacket(p)));
            _gameServer.OnReceivedPacketEvent(0x18, p => Data.UpdateSelf(new UpdateSelfPacket(p)));
            _gameServer.OnReceivedPacketEvent(0x95, p => Data.UpdateSelf(new UpdateSelfPacket(p)));
        }

        private void Initialize(GameFlags packet) 
            => Data = new GameData(packet);

        public void LeaveGame()
        {
            Log.Information("Leaving game");
            _gameServer.LeaveGame();
        }
    }
}
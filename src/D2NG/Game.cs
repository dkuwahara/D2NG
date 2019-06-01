using D2NG.D2GS;
using D2NG.D2GS.Packet;
using D2NG.D2GS.Packet.Server;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace D2NG
{
    public class Game
    {
        private readonly GameServer _gameServer;

        private GameData Data { get; set; }

        internal Game(GameServer gameServer)
        {
            _gameServer = gameServer;

            _gameServer.OnReceivedPacketEvent((byte)D2gs.GAMEFLAGS, p => Data = new GameData(new GameFlagsPacket(p)));
            _gameServer.OnReceivedPacketEvent(0x59, p => Data.AssignPlayer(new AssignPlayerPacket(p)));
            _gameServer.OnReceivedPacketEvent(0x23, p => Data.SetItemSkill(new SetItemSkillPacket(p)));
            _gameServer.OnReceivedPacketEvent(0x23, p => Data.SetSkill(new SetSkillPacket(p)));
            _gameServer.OnReceivedPacketEvent(0x0B, p => new GameHandshakePacket(p));
            _gameServer.OnReceivedPacketEvent(0x1A, p => Data.AddExperience(new AddExpPacket(p)));
            _gameServer.OnReceivedPacketEvent(0x1B, p => Data.AddExperience(new AddExpPacket(p)));
            _gameServer.OnReceivedPacketEvent(0x1C, p => Data.AddExperience(new AddExpPacket(p)));
            _gameServer.OnReceivedPacketEvent(0x1D, p => Data.SetAttribute(new BaseAttributePacket(p)));
            _gameServer.OnReceivedPacketEvent(0x1E, p => Data.SetAttribute(new BaseAttributePacket(p)));
            _gameServer.OnReceivedPacketEvent(0x1F, p => Data.SetAttribute(new BaseAttributePacket(p)));
            _gameServer.OnReceivedPacketEvent(0x94, p => new BaseSkillLevelsPacket(p));
        }

        public void LeaveGame()
        {
            Log.Information("Leaving game");
            _gameServer.LeaveGame();
        }
    }
}

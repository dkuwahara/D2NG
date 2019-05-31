using D2NG.D2GS;
using D2NG.D2GS.Packet;
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
            _gameServer.OnReceivedPacketEvent(0x23, p => Data.SetSkill(new SetSkillPacket(p)));
            _gameServer.OnReceivedPacketEvent(0x0B, p => new GameHandshakePacket(p));
            _gameServer.OnReceivedPacketEvent(0x1D, p => Data.SetAttribute(new BaseAttributePacket(p)));
            _gameServer.OnReceivedPacketEvent(0x1E, p => Data.SetAttribute(new BaseAttributePacket(p)));
            _gameServer.OnReceivedPacketEvent(0x1F, p => Data.SetAttribute(new BaseAttributePacket(p)));
        }

        public void LeaveGame()
        {
            Log.Information("Leaving game");
            _gameServer.LeaveGame();
        }
    }
}

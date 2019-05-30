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
        private GameServer _gameServer;

        public Difficulty Difficulty { get => GameContext.Difficulty; }
        
        private GameData GameContext { get; set; }

        internal Game(GameServer gameServer)
        {
            _gameServer = gameServer;

            _gameServer.OnReceivedPacketEvent((byte)D2gs.GAMEFLAGS, p => GameContext = new GameData(_gameServer.Character, new GameFlags(p)));
            _gameServer.OnReceivedPacketEvent(0x59, p => GameContext.AssignPlayer(new AssignPlayer(p)));
            _gameServer.OnReceivedPacketEvent(0x23, p => GameContext.SetSkill(new SetSkill(p)));
            _gameServer.OnReceivedPacketEvent(0x0B, p => new GameHandshake(p));
            _gameServer.OnReceivedPacketEvent(0x1D, p => new BaseAttribute(p));
            _gameServer.OnReceivedPacketEvent(0x1E, p => new BaseAttribute(p));
            _gameServer.OnReceivedPacketEvent(0x1F, p => new BaseAttribute(p));
        }

        public void LeaveGame()
        {
            Log.Information("Leaving game");
            _gameServer.LeaveGame();
        }
    }
}

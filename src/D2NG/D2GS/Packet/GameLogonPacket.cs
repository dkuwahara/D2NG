using D2NG.D2GS.Packet;
using D2NG.MCP;
using System;
using System.Text;

namespace D2NG.D2GS
{
    internal class GameLogonPacket : D2gsPacket
    {
        private const int Version = 0x0e;

        private static readonly byte[] Locale = { 0x00 };

        private static readonly byte[] Constant = { 0x50, 0xcc, 0x5d, 0xed, 0xb6, 0x19, 0xa5, 0x91 };

        public GameLogonPacket(uint gameHash, ushort gameToken, Character character) :
            base(
                BuildPacket(
                    D2gs.GAMELOGON,
                    BitConverter.GetBytes(gameHash),
                    BitConverter.GetBytes(gameToken),
                    new byte[] { (byte)character.Class },
                    BitConverter.GetBytes(Version),
                    Constant,
                    Locale,
                    Encoding.ASCII.GetBytes(character.Name.PadRight(16, '\0'))
                )
            )
        {
        }
    }
}
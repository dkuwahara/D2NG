using D2NG.D2GS.Packet;

namespace D2NG
{
    internal class GameHandshake
    {
        private D2gsPacket p;

        public GameHandshake(D2gsPacket p)
        {
            this.p = p;
        }
    }
}
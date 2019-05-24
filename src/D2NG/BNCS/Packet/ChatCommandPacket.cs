using System.Text;

namespace D2NG.BNCS.Packet
{
    internal class ChatCommandPacket : BncsPacket
    {
        public ChatCommandPacket(string message) : 
            base(
                BuildPacket(
                    Sid.CHATCOMMAND,
                    Encoding.ASCII.GetBytes($"{message}\0")
                )
            )
        {
        }
    }
}
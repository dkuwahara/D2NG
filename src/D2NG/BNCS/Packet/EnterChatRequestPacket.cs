using Serilog;
using System.Text;

namespace D2NG.BNCS.Packet
{
    public class EnterChatRequestPacket : BncsPacket
    {
        public EnterChatRequestPacket(string username) : base(
            BuildPacket(
                Sid.ENTERCHAT,
                Encoding.ASCII.GetBytes(username),
                Encoding.ASCII.GetBytes("\0\0")
                ))
        {
            Log.Verbose($"EnterChatRequestPacket:\n" +
                $"\tUsername: {username}");
        }
    }
}

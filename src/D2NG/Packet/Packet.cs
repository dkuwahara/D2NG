using System.IO;
using System.Text;

namespace D2NG
{
    public class Packet
    {
        public byte[] Raw { get; }

        public Packet(byte[] packet)
        {
            Raw = packet;
        }

        public static string ReadString(BinaryReader reader)
        {
            var text = new StringBuilder();
            while (reader.PeekChar() != 0)
            {
                text.Append(reader.ReadChar());
            }
            reader.ReadChar();
            return text.ToString();
        }
    }
}
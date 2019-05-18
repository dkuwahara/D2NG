using System.IO;
using System.Text;

namespace D2NG.BNCS.Packet
{
    public class ChatEventPacket : BncsPacket
    { 
        public Eid Eid { get; }
        public uint UserFlags { get; }
        public uint Ping { get; }
        public string Username { get; }
        public string Text { get; }
        public ChatEventPacket(byte[] packet) : base(packet)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(packet), Encoding.ASCII);
            if (PrefixByte != reader.ReadByte())
            {
                throw new BncsPacketException("Not a valid BNCS Packet");
            }
            if ((byte)Sid.CHATEVENT != reader.ReadByte())
            {
                throw new BncsPacketException("Expected type was not found");
            }
            if (packet.Length != reader.ReadUInt16())
            {
                throw new BncsPacketException("Packet length does not match");
            }

            Eid = (Eid) reader.ReadUInt32();
            UserFlags = reader.ReadUInt32();
            Ping = reader.ReadUInt32();
            _ = reader.ReadUInt32();
            _ = reader.ReadUInt32();
            _ = reader.ReadUInt32();

            var username = new StringBuilder();
            while (reader.PeekChar() != 0)
            {
                username.Append(reader.ReadChar());
            }
            Username = username.ToString();

            reader.ReadChar();

            var text = new StringBuilder();
            while (reader.PeekChar() != 0)
            {
                text.Append(reader.ReadChar());
            }
            Text = text.ToString();

            reader.ReadChar();
            reader.Close();
        }

        public string RenderText()
        {
            switch (this.Eid)
            {
                case Eid.JOIN:
                    return $"{this.Username} has joined the channel";
                case Eid.LEAVE:
                    return $"{this.Username} has left the channel";
                case Eid.WHISPER:
                    return $"From <{this.Username}>: {this.Text}";
                case Eid.TALK:
                    return $"<{this.Username}>: {this.Text}";
                case Eid.CHANNEL:
                    return $"Joined channel: {this.Text}";
                case Eid.INFO:
                    return $"INFO: {this.Text}";
                case Eid.ERROR:
                    return $"ERROR: {this.Text}";
                case Eid.EMOTE:
                    return $"<{this.Username} {this.Text}>";
                default:
                    return $"{this.Text}";
            }
        }
    }

    public enum Eid
    {
        SHOWUSER = 0x01,
        JOIN = 0x02,
        LEAVE = 0x03,
        WHISPER = 0x04,
        TALK = 0x05,
        BROADCAST = 0x06,
        CHANNEL = 0x07,
        USERFLAGS = 0x09,
        WHISPERSENT = 0x0A,
        CHANNELFULL = 0x0D,
        CHANNELDOESNOTEXIST = 0x0E,
        CHANNELRESTRICTED = 0x0F,
        INFO = 0x12,
        ERROR = 0x13,
        IGNORE = 0x15,
        ACCEPT = 0x16,
        EMOTE = 0x17
    }
}
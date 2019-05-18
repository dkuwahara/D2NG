namespace D2NG.BNCS.Packet
{
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
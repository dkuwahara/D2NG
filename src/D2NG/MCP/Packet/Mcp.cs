namespace D2NG.MCP.Packet
{
    public enum Mcp : byte
    {
        STARTUP = 0x01,
        CHARCREATE = 0x02,
        CREATEGAME = 0x03,
        JOINGAME = 0x04,
        GAMELIST = 0x05,
        GAMEINFO = 0x06,
        CHARLOGON = 0x07,
        CHARDELETE = 0x0A,
        REQUESTLADDERDATA = 0x11,
        MOTD = 0x12,
        CANCELGAMECREATE = 0x13,
        CREATEQUEUE = 0x14,
        CHARRANK = 0x16,
        CHARLIST = 0x17,
        CHARUPGRADE = 0x18,
        CHARLIST2 = 0x19
    }
}

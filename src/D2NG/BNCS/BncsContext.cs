using D2NG.BNCS.Login;

namespace D2NG
{
    public class BncsContext
    {
        public uint ClientToken { get; set; }
        public uint ServerToken { get; set; }
        public string Username { get; internal set; }
        public CdKey ClassicKey { get; internal set; }
        public CdKey ExpansionKey { get; internal set; }
    }
}
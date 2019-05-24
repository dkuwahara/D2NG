using D2NG.BNCS;

namespace D2NG
{
    public class Chat
    {
        private BattleNetChatServer _bncs;

        internal Chat(BattleNetChatServer bncs)
        {
            this._bncs = bncs;
        }

        public void JoinChannel(string channel)
        {
            _bncs.JoinChannel(channel);
        }

        public void Send(string message)
        {
            _bncs.ChatCommand(message);
        }
    }
}
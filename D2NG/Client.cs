using System;

namespace D2NG
{
    public class Client
    {
        public BattleNetChatServer Bncs { get; }

        public Client()
        {
            Bncs = new BattleNetChatServer();
        }
    }
}

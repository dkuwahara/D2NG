using D2NG.BNCS;
using D2NG.BNCS.Login;
using System;
using System.Collections.Generic;
using System.Text;

namespace D2NG
{
    public class Client
    {
        public BattleNetChatServer BNCS { get; } 

        public Client()
        {
            BNCS = new BattleNetChatServer("", "");
        }

    }
}

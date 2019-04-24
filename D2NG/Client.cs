using D2NG.BNCS;
using D2NG.BNCS.Login;
using System;
using System.Collections.Generic;
using System.Text;

namespace D2NG
{
    public class Client
    {
        public BattleNetChatServer Bncs { get; } = new BattleNetChatServer();
        private Authenticator Authenticator { get; }

        public Client()
        {
            Authenticator = new Authenticator(Bncs);
        }

        public void UsingKeys(String classicKey, String expansionKey)
        {
            Authenticator.ClassicKey = new CdKey(classicKey);
            Authenticator.ExpansionKey = new CdKey(expansionKey);
        }
    }
}

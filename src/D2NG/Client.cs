using System;
using System.Collections.Generic;

namespace D2NG
{
    public class Client
    {
        public BattleNetChatServer Bncs { get; }

        public Client()
        {
            Bncs = new BattleNetChatServer();
        }

        public List<(string Name, string Description)> ListRealms() => this.Bncs.ListRealms();


        public void RealmLogon()
        {
            //Bncs.RealmLogon();
        }
    }
}

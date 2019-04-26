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

        private readonly Config _config;

        public Client(String config)
        {
            _config = Config.FromFile(config);
            BNCS = new BattleNetChatServer(_config.ClassicKey, _config.ExpansionKey);
        }
    }
}

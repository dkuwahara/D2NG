using System;

namespace D2NG
{
    public class Client
    {
        public BattleNetChatServer Bncs { get; }

        private readonly Config _config;

        public Client(String config)
        {
            _config = Config.FromFile(config);
            Bncs = new BattleNetChatServer(_config.ClassicKey, _config.ExpansionKey);
        }
    }
}

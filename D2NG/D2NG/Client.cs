using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Linq;

namespace D2NG
{
    public class Client
    {
        private readonly BNCS _BNCS = new BNCS();

        public void ConnectToBattleNet(String realm)
        {
            _BNCS.Connect(realm);
            _BNCS.Send(0x01);
            _BNCS.Send(BNCS.AuthInfoPacket);
            _BNCS.Listen();
        }
    }
}


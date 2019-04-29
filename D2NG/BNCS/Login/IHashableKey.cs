using System;
using System.Collections.Generic;
using System.Text;

namespace D2NG.BNCS.Login
{
    interface IHashableKey
    {
        List<byte> Hash(uint clientToken, uint serverToken);
    }
}

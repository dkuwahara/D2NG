using System;
using System.Collections.Generic;
using System.Text;

namespace D2NG.BNCS.Login
{
    interface IHashableKey
    {
        byte[] ComputeHash(uint clientToken, uint serverToken);
    }
}

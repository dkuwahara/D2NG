using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Core.Tokens;

namespace D2NG.BNCS.Login
{
    public abstract class CdKey
    {
        public string Key { get; }
        public int Product { get; set; }
        public byte[] Public { get; set; }
        public byte[] Private { get; set; }
        public int KeyLength { get; set; }

        public CdKey(String key)
        {
            Key = key;
        }

        public abstract byte[] ComputeHash(uint clientToken, uint serverToken);
    }
}

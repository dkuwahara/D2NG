using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Core.Tokens;

namespace D2NG.BNCS.Login
{
    public abstract class CdKey : IHashableKey
    {
        public string Key { get; }
        public int Product { get; set; }
        public byte[] Public { get; set; }
        public byte[] Private { get; set; }

        public CdKey(String key)
        {
            Key = key;
        }

        public abstract List<byte> Hash(uint clientToken, uint serverToken);
    }
}

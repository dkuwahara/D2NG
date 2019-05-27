using Serilog;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace D2NG.BNCS.Hashing
{
    public static class CheckRevisionV4
    {
        private const string Version = "1.14.3.71";

        public static (int Version, byte[] Checksum, byte[] Info) CheckRevision(string value)
        {
            Log.Verbose("Calculating Checksum");
            var bytes = new List<byte>(Convert.FromBase64String(value))
                .GetRange(0, 4);
            bytes.AddRange(Encoding.ASCII.GetBytes(":" + Version + ":"));
            bytes.Add(1);

            SHA1 sha = new SHA1CryptoServiceProvider();
            var hash = sha.ComputeHash(bytes.ToArray());
            var b64Hash = Convert.ToBase64String(hash);
            var checksum = Encoding.ASCII.GetBytes(b64Hash.Substring(0, 4));
            var info = Encoding.ASCII.GetBytes(b64Hash.Substring(4) + "\0");
            return (0, checksum, info);
        }
    }
}

﻿using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;
using Serilog;

namespace D2NG.BNCS.Login
{
    public class CheckRevisionV4
    {
        private const String VERSION = "1.14.3.71";
        public static CheckRevisionResult CheckRevision(string value)
        {
            var bytes = new List<byte>(Convert.FromBase64String(value))
                .GetRange(0, 4);
            bytes.AddRange(Encoding.ASCII.GetBytes(":" + VERSION + ":"));
            bytes.Add(1);

            SHA1 sha = new SHA1CryptoServiceProvider();
            var hash = sha.ComputeHash(bytes.ToArray());
            var b64Hash = Convert.ToBase64String(hash);

            var checksum = Encoding.ASCII.GetBytes(b64Hash.Substring(0, 4))
                .Reverse()
                .ToArray();
            var info = Encoding.ASCII.GetBytes(b64Hash.Substring(4) + "\0");
            return new CheckRevisionResult(0, checksum, info);
        }
    }

    public class CheckRevisionResult
    {
        public int Version { get; }
        public byte[] Checksum { get; }
        public byte[] Info { get; }

        public CheckRevisionResult(int version, byte[] checksum, byte[] info)
        {
            Version = version;
            Checksum = checksum;
            Info = info;
        }
    }
}

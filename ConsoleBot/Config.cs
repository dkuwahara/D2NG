﻿using System;
using System.IO;
using YamlDotNet.Serialization;

namespace ConsoleBot
{
    public class Config
    {
        public static Config FromFile(String file)
        {
            return FromString(File.ReadAllText(file));
        }

        public static Config FromString(String file)
        {
            return new Deserializer().Deserialize<Config>(file);
        }

        [YamlMember(Alias = "classicKey")]
        public String ClassicKey { get; set; }
        [YamlMember(Alias = "expansionKey")]
        public String ExpansionKey { get; set; }
    }
}

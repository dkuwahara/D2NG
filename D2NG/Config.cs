using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using YamlDotNet.Serialization;

namespace D2NG
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

using System.IO;
using YamlDotNet.Serialization;

namespace ConsoleBot
{
    public class Config
    {
        public static Config FromFile(string file)
        {
            return FromString(File.ReadAllText(file));
        }

        public static Config FromString(string file)
        {
            return new Deserializer().Deserialize<Config>(file);
        }

        [YamlMember(Alias = "classicKey")]
        public string ClassicKey { get; set; }

        [YamlMember(Alias = "expansionKey")]
        public string ExpansionKey { get; set; }

        [YamlMember(Alias = "realm")]
        public string Realm { get; set; }

        [YamlMember(Alias = "username")]
        public string Username { get; set; }

        [YamlMember(Alias = "password")]
        public string Password { get; set; }
    }
}

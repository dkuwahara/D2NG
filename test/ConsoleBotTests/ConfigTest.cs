using ConsoleBot;
using Xunit;

namespace ConfigTests
{
    public class ConfigTest
    {
        private readonly string yaml =
            @"classicKey: 1234567890ABCDEF
expansionKey: FEDCBA0987654321";

       [Fact]
        public void TestParsing()
        {
            Config config = Config.FromString(yaml);
            Assert.Equal("1234567890ABCDEF", config.ClassicKey);
            Assert.Equal("FEDCBA0987654321", config.ExpansionKey);
        }
    }
}

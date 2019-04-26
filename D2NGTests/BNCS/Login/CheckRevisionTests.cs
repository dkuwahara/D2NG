using System;
using System.Text;
using Xunit;

namespace D2NG.BNCS.Login
{
    public class CheckRevisionTests
    {
        [Fact]
        public void Test()
        {
            var result = CheckRevisionV4.CheckRevision("0RFf+AAA");
            var checksum = "4f78392b";
            Assert.Equal(0, result.Version);
            Assert.Equal(1333279019, BitConverter.ToInt32(result.Checksum));
            Assert.Equal("qiNZCXbTGzoPvl76Y66lNfg=\0", Encoding.ASCII.GetString(result.Info));
            Assert.Equal( checksum, BitConverter.ToInt32(result.Checksum).ToString("x"));
        }

        [Fact]
        public void Test2()
        {
            var result = CheckRevisionV4.CheckRevision("3ou3jQAA");
            Assert.Equal(0, result.Version);
            Assert.Equal(1886214221, BitConverter.ToInt32(result.Checksum));
            Assert.Equal("g8AEAY1dgVaotHYLUzJgKJo=\0", Encoding.ASCII.GetString(result.Info));
        }

        [Fact]
        public void Test3()
        {
            var result = CheckRevisionV4.CheckRevision("ZYDAHQAA");
            Assert.Equal(0, result.Version);
            Assert.Equal(1734822998, BitConverter.ToInt32(result.Checksum));
            Assert.Equal("+9kKei3b960lDtOxuQeURoI=\0", Encoding.ASCII.GetString(result.Info));
        }

        [Fact]
        public void Test4()
        {
            var result = CheckRevisionV4.CheckRevision("Ry4VIgAA");
            Assert.Equal(0, result.Version);
            Assert.Equal(1699508065, BitConverter.ToInt32(result.Checksum));
            Assert.Equal("Kyl+7a6yKVGWEj7rzhB+Q40=\0", Encoding.ASCII.GetString(result.Info));
        }

    }
}

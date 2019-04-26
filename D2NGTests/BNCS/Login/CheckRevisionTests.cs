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
            byte[] checksum = { 0x4f, 0x78, 0x39, 0x2b };

            var result = CheckRevisionV4.CheckRevision("0RFf+AAA");
            Assert.Equal(0, result.Version);
            Assert.Equal(BitConverter.ToInt32(checksum), BitConverter.ToInt32(result.Checksum));
            Assert.Equal("qiNZCXbTGzoPvl76Y66lNfg=\0", Encoding.ASCII.GetString(result.Info));
        }

        [Fact]
        public void Test2()
        {
            byte[] checksum = { 0x70, 0x6d, 0x58, 0x4d };

            var result = CheckRevisionV4.CheckRevision("3ou3jQAA");
            Assert.Equal(0, result.Version);
            Assert.Equal(BitConverter.ToInt32(checksum), BitConverter.ToInt32(result.Checksum));
            Assert.Equal("g8AEAY1dgVaotHYLUzJgKJo=\0", Encoding.ASCII.GetString(result.Info));
        }

        [Fact]
        public void Test3()
        {
            byte[] checksum = { 0x67, 0x67, 0x4c, 0x56 };

            var result = CheckRevisionV4.CheckRevision("ZYDAHQAA");
            Assert.Equal(0, result.Version);
            Assert.Equal(BitConverter.ToInt32(checksum), BitConverter.ToInt32(result.Checksum));
            Assert.Equal("+9kKei3b960lDtOxuQeURoI=\0", Encoding.ASCII.GetString(result.Info));
        }

        [Fact]
        public void Test4()
        {
            byte[] checksum = { 0x65, 0x4c, 0x6f, 0x61 };

            var result = CheckRevisionV4.CheckRevision("Ry4VIgAA");
            Assert.Equal(0, result.Version);
            Assert.Equal(BitConverter.ToInt32(checksum), BitConverter.ToInt32(result.Checksum));
            Assert.Equal("Kyl+7a6yKVGWEj7rzhB+Q40=\0", Encoding.ASCII.GetString(result.Info));
        }

    }
}

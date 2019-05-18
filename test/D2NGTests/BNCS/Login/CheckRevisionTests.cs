using System;
using System.Runtime.InteropServices;
using System.Text;
using Xunit;

namespace D2NG.BNCS.Login
{
    public static class CheckRevisionTests
    {
        [Fact]
        public static void TestCheckRevision1()
        {
            byte[] checksum = { 0x2b, 0x39, 0x78, 0x4f };

            var result = CheckRevisionV4.CheckRevision("0RFf+AAA");
            Assert.Equal(0, result.Version);
            Assert.Equal(BitConverter.ToInt32(checksum), BitConverter.ToInt32(result.Checksum));
            Assert.Equal("qiNZCXbTGzoPvl76Y66lNfg=\0", Encoding.ASCII.GetString(result.Info));
        }

        [Fact]
        public static void TestCheckRevision2()
        {
            byte[] checksum = { 0x4d, 0x58, 0x6d, 0x70 };

            var result = CheckRevisionV4.CheckRevision("3ou3jQAA");
            Assert.Equal(0, result.Version);
            Assert.Equal(BitConverter.ToInt32(checksum), BitConverter.ToInt32(result.Checksum));
            Assert.Equal("g8AEAY1dgVaotHYLUzJgKJo=\0", Encoding.ASCII.GetString(result.Info));
        }

        [Fact]
        public static void TestCheckRevision3()
        {
            byte[] checksum = { 0x56, 0x4c, 0x67, 0x67};

            var result = CheckRevisionV4.CheckRevision("ZYDAHQAA");
            Assert.Equal(0, result.Version);
            Assert.Equal(BitConverter.ToInt32(checksum), BitConverter.ToInt32(result.Checksum));
            Assert.Equal("+9kKei3b960lDtOxuQeURoI=\0", Encoding.ASCII.GetString(result.Info));
        }

        [Fact]
        public static void TestCheckRevision4()
        {
            byte[] checksum = { 0x61, 0x6f, 0x4c, 0x65 };

            var result = CheckRevisionV4.CheckRevision("Ry4VIgAA");
            Assert.Equal(0, result.Version);
            Assert.Equal(BitConverter.ToInt32(checksum), BitConverter.ToInt32(result.Checksum));
            Assert.Equal("Kyl+7a6yKVGWEj7rzhB+Q40=\0", Encoding.ASCII.GetString(result.Info));
        }

    }
}

using System;
using System.Linq;
using D2NG.BNCS.Hashing;
using Xunit;

namespace D2NGTests.BNCS.Login
{
    public class CdKeySha1Tests : CdKeySha1
    { 
        public CdKeySha1Tests() : base("01234567890123456789123456")
        {
        }

        [Fact]
        public void TestMaskingBytes()
        {
            int[] priv =
            {
                53, -4, 41, 65, -24, 76, -124, 36, 12, 42
            };
            Assert.Equal(9993, this.Product);
            Assert.Equal(BitConverter.GetBytes(18067384), this.Public);
            Assert.Equal(priv.Select(v => (byte)v), this.Private);
        }

        [Fact]
        public void TestBuildTableFromKey()
        {
            var expectedTable = new []
            {
                0x33, 0x00, 0x33, 0x01, 0x33, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x33, 0x00, 0x33, 0x00,
                0x33, 0x00, 0x00, 0x00, 0x01, 0x00, 0x33, 0x04,
                0x33, 0x00, 0x00, 0x00, 0x01, 0x03, 0x33, 0x04,
                0x00, 0x02, 0x00, 0x02, 0x00, 0x03, 0x00, 0x00,
                0x00, 0x00, 0x33, 0x02, 0x33, 0x01, 0x00, 0x01,
                0x00, 0x00, 0x00, 0x0
            };
            var table = BuildTableFromKey(this.Key);
            Assert.Equal(expectedTable, table);
        }
    }
}

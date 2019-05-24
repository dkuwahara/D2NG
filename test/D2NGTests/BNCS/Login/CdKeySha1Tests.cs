using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using D2NG.BNCS.Login;
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

        [Theory]
        [ClassData(typeof(EbpTestData))]
        public static void TestEbp(long ebp, long value, int ecx)
        {
            Assert.Equal(ebp, CdKeySha1.Ebp(value, ecx));
        }

        public class EbpTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { 11, 11889537, 20 };
                yield return new object[] { 5, 11889537, 16 };
                yield return new object[] { 6, 11889537, 12 };
                yield return new object[] { 11, 11889537, 8 };
                yield return new object[] { 8, 11889537, 4 };
                yield return new object[] { 1, 11889537, 0 };
                yield return new object[] { 12, 3446176343, 28 };
                yield return new object[] { 11, 11889537, 20 };
                yield return new object[] { 2, 3261626967, 24 };
                yield return new object[] { 11, 11889537, 20 };
                yield return new object[] { 5, 11889537, 16 };
                yield return new object[] { 6, 11889537, 12 };
                yield return new object[] { 2, 3262640643, 8 };
                yield return new object[] { 0, 3262640643, 4 };
                yield return new object[] { 3, 3262640643, 0 };
                yield return new object[] { 8, -2109570360, 28 };
                yield return new object[] { 2, -2109570360, 24 };
                yield return new object[] { 4, -2109570360, 20 };
                yield return new object[] { 2, -2109570360, 16 };
                yield return new object[] { 8, -2109570360, 12 };
                yield return new object[] { 2, -2109570360, 8 };
                yield return new object[] { 12, -2109570360, 4 };
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}

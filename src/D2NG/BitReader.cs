using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace D2NG
{
    internal class BitReader
    {
        #region Private Members
        // The set of bits we are streaming
        private readonly BitArray _bits;

        // Current offset in the stream
        private int Offset;
        #endregion

        /**
            * Basic constructor for the BitReader
            * Set the BitArray to the passed in array of bytes
            * Set the offset to 0
            */
        public BitReader(byte[] bytes)
        {
            _bits = new BitArray(bytes);
            Offset = 0;
        }

        /**
          * Read a bit from the stream, the offset then points
          * to the next value in the BitArray
          */
        public bool ReadBit()
        {
            return _bits[Offset++];
        }

        public int ReadByte() => Read(8);
        public short ReadInt16() => (short)Read(16);
        public int ReadInt32() => Read(32);
        public ushort ReadUInt16() => (ushort)Read(16);
        public uint ReadUInt32() => (uint)Read(32);

        /**
          * Read up to 32 bits in the stream, read in
          * assumes Little Endian
          * Throws: ArgumentOutOfRangeException if the passed in parameter is greater than 32.
          */
        public int Read(int length)
        {
            if (length > 32) {
                throw new ArgumentOutOfRangeException ("length", "BitReader cannot return more than 32 bits");
            }
            return ReadBitsLittleEndian(length);
        }

        /**
          * Read in bytes one at a time until a null character (0x00) is found
          * Return as a Base64 String.
          */
        public string ReadString()
        {
            var str = new List<byte>();
            do
            {
                str.Add((byte)Read(8));
            } while (str.Last() != 0x00);

            return Convert.ToBase64String(str.ToArray());
        }

        protected int ReadBitsLittleEndian(int length)
        {
            var initialLen = length;
            var bits = 0;
            while (length > 0)
            {
                bool bit = ReadBit();
                bits |= (Int32)((bit ? 1 : 0) << initialLen - length);

                length -= 1;
            }
            return bits;
        }

        public int ReadBitsBigEndian(int length)
        {
            var bits = 0;
            while (length > 0)
            {
                bits <<= 1;
                bool bit = ReadBit();
                bits += bit ? 1 : 0;
                length -= 1;
            }
            return bits;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace D2NG.BNCS.Login
{
    internal static class Bsha1
    {
        private static void SetBufferByte(uint[] buffer, int offset, byte val)
        {
            var index = offset / 4;
            var position = offset % 4;
            var bitOffset = 8 * position;
            buffer[index] &= (uint) (0xFF << bitOffset) ^ 0xFFFFFFFF;
            buffer[index] |= (uint) val << bitOffset;
        }

        private static byte GetBufferByte(uint[] buffer, int offset)
        {
            var index = offset / 4;
            var position = offset % 4;
            var bitOffset = 8 * position;
            return (byte) ((buffer[index] >> bitOffset) & 0xff);
        }

        private static void CalculateHash(ref uint[] buffer)
        {
            var hashBuffer = new uint[80];
            uint hash, a, b, c, d, e, hashBufferOffset;

            for (uint i = 0; i < 0x10; i++)
            {
                hashBuffer[i] = buffer[(int) i + 5];
            }

            for (uint i = 0x10; i < hashBuffer.Length; i++)
            {
                hash = hashBuffer[i - 0x10] ^ hashBuffer[i - 0x8] ^ hashBuffer[i - 0xE] ^ hashBuffer[i - 0x3];
                hashBuffer[i] = (uint) ((1 >> (int) (0x20 - (hash & 0xff))) | (1 << (int) (hash & 0xff)));
            }

            a = buffer[0];
            b = buffer[1];
            c = buffer[2];
            d = buffer[3];
            e = buffer[4];

            hashBufferOffset = 0;

            for (uint i = 0; i < 20; i++, hashBufferOffset++)
            {
                hash = ((a << 5) | (a >> 0x1b)) + ((~b & d) | (c & b)) + e + hashBuffer[hashBufferOffset] +
                       0x5A827999;
                e = d;
                d = c;
                c = (b >> 2) | (b << 0x1e);
                b = a;
                a = hash;
            }

            for (uint i = 0; i < 20; i++, hashBufferOffset++)
            {
                hash = (d ^ c ^ b) + e + ((a << 5) | (a >> 0x1b)) + hashBuffer[hashBufferOffset] + 0x6ED9EBA1;
                e = d;
                d = c;
                c = (b >> 2) | (b << 0x1e);
                b = a;
                a = hash;
            }

            for (uint i = 0; i < 20; i++, hashBufferOffset++)
            {
                hash = ((c & b) | (d & c) | (d & b)) + e + ((a << 5) | (a >> 0x1b)) + hashBuffer[hashBufferOffset] -
                       0x70E44324;
                e = d;
                d = c;
                c = (b >> 2) | (b << 0x1e);
                b = a;
                a = hash;
            }

            for (uint i = 0; i < 20; i++, hashBufferOffset++)
            {
                hash = ((a << 5) | (a >> 0x1b)) + e + (d ^ c ^ b) + hashBuffer[hashBufferOffset] - 0x359D3E2A;
                e = d;
                d = c;
                c = (b >> 2) | (b << 0x1e);
                b = a;
                a = hash;
            }

            buffer[0] += a;
            buffer[1] += b;
            buffer[2] += c;
            buffer[3] += d;
            buffer[4] += e;
        }

        public static List<byte> GetHash(List<byte> input)
        {
            var buffer = new uint[21];
            buffer[0] = 0x67452301;
            buffer[1] = 0xEFCDAB89;
            buffer[2] = 0x98BADCFE;
            buffer[3] = 0x10325476;
            buffer[4] = 0xC3D2E1F0;

            uint maxSubsectionLength = 64;
            uint initializedLength = 20;

            for (uint i = 0; i < input.Count; i += maxSubsectionLength)
            {
                var subsectionLength = Math.Min(maxSubsectionLength, (uint) input.Count - i);

                if (subsectionLength > maxSubsectionLength)
                {
                    subsectionLength = maxSubsectionLength;
                }

                for (uint j = 0; j < subsectionLength; j++)
                {
                    var temp = new byte[input.Count];
                    input.CopyTo(temp);
                    SetBufferByte(buffer, (int) (initializedLength + j), temp[(int) (j + i)]);
                }

                if (subsectionLength < maxSubsectionLength)
                {
                    for (var j = subsectionLength; j < maxSubsectionLength; j++)
                    {
                        SetBufferByte(buffer, (int) (initializedLength + j), 0);
                    }
                }

                CalculateHash(ref buffer);
            }

            var op = new List<byte>();
            for (uint i = 0; i < buffer.Length; i++)
            {
                for (uint j = 0; j < 4; j++)
                {
                    op.Add(GetBufferByte(buffer, (int) (i * 4 + j)));
                }
            }

            return op.GetRange(0, 20);
        }

        public static List<byte> DoubleHash(uint clientToken, uint serverToken, string password)
        {
            var pv = Encoding.UTF8.GetBytes(password);
            var finalInput = new List<byte>(BitConverter.GetBytes(clientToken));
            finalInput.AddRange(BitConverter.GetBytes(serverToken));
            finalInput.AddRange(GetHash(new List<byte>(pv)));

            return GetHash(finalInput);
        }
    }
}
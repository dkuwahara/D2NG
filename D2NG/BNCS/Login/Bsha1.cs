using System;
using System.Collections.Generic;

namespace D2NG.BNCS.Login
{
    class Bsha1
    {
        protected static void setBufferByte(uint[] buffer, int offset, byte val)
        {
            int index = offset / 4;
            int position = offset % 4;
            int bit_offset = 8 * position;
            buffer[index] &= (uint)(0xFF << bit_offset) ^ 0xFFFFFFFF;
            buffer[index] |= (uint)val << bit_offset;
        }

        protected static byte getBufferByte(uint[] buffer, int offset)
        {
            int index = offset / 4;
            int position = offset % 4;
            int bit_offset = 8 * position;
            return (byte)((buffer[index] >> bit_offset) & 0xff);
        }

        protected static void CalculateHash(ref uint[] buffer)
        {
            uint[] hash_buffer = new uint[80];
            uint hash, a, b, c, d, e, hash_buffer_offset;

            for (uint i = 0; i < 0x10; i++)
            {
                hash_buffer[i] = buffer[(int)i + 5];
            }

            for (uint i = 0x10; i < hash_buffer.Length; i++)
            {
                hash = hash_buffer[i - 0x10] ^ hash_buffer[i - 0x8] ^ hash_buffer[i - 0xE] ^ hash_buffer[i - 0x3];
                hash_buffer[i] = (uint)((1 >> (int)(0x20 - (hash & 0xff))) | (1 << (int)(hash & 0xff)));
            }

            a = buffer[0];
            b = buffer[1];
            c = buffer[2];
            d = buffer[3];
            e = buffer[4];

            hash_buffer_offset = 0;

            for (uint i = 0; i < 20; i++, hash_buffer_offset++)
            {
                hash = ((a << 5) | (a >> 0x1b)) + ((~b & d) | (c & b)) + e + hash_buffer[hash_buffer_offset] + 0x5A827999;
                e = d;
                d = c;
                c = (b >> 2) | (b << 0x1e);
                b = a;
                a = hash;
            }

            for (uint i = 0; i < 20; i++, hash_buffer_offset++)
            {
                hash = (d ^ c ^ b) + e + ((a << 5) | (a >> 0x1b)) + hash_buffer[hash_buffer_offset] + 0x6ED9EBA1;
                e = d;
                d = c;
                c = (b >> 2) | (b << 0x1e);
                b = a;
                a = hash;
            }

            for (uint i = 0; i < 20; i++, hash_buffer_offset++)
            {
                hash = ((c & b) | (d & c) | (d & b)) + e + ((a << 5) | (a >> 0x1b)) + hash_buffer[hash_buffer_offset] - 0x70E44324;
                e = d;
                d = c;
                c = (b >> 2) | (b << 0x1e);
                b = a;
                a = hash;
            }

            for (uint i = 0; i < 20; i++, hash_buffer_offset++)
            {
                hash = ((a << 5) | (a >> 0x1b)) + e + (d ^ c ^ b) + hash_buffer[hash_buffer_offset] - 0x359D3E2A;
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
            uint[] buffer = new uint[21];
            buffer[0] = 0x67452301;
            buffer[1] = 0xEFCDAB89;
            buffer[2] = 0x98BADCFE;
            buffer[3] = 0x10325476;
            buffer[4] = 0xC3D2E1F0;

            uint max_subsection_length = 64;
            uint initialized_length = 20;

            for (uint i = 0; i < input.Count; i += max_subsection_length)
            {
                uint subsection_length = Math.Min(max_subsection_length, (uint)input.Count - i);

                if (subsection_length > max_subsection_length)
                    subsection_length = max_subsection_length;

                for (uint j = 0; j < subsection_length; j++)
                {
                    byte[] temp = new byte[input.Count];
                    input.CopyTo(temp);
                    setBufferByte(buffer, (int)(initialized_length + j), temp[(int)(j + i)]);
                }

                if (subsection_length < max_subsection_length)
                {
                    for (uint j = subsection_length; j < max_subsection_length; j++)
                        setBufferByte(buffer, (int)(initialized_length + j), 0);
                }

                CalculateHash(ref buffer);

            }

            List<byte> op = new List<byte>();
            for (uint i = 0; i < buffer.Length; i++)
                for (uint j = 0; j < 4; j++)
                    op.Add(getBufferByte(buffer, (int)(i * 4 + j)));
            return new List<byte>(op.GetRange(0, 20));
        }

        public static List<byte> DoubleHash(UInt32 client_token, UInt32 server_token, string password)
        {
            byte[] pv = System.Text.Encoding.UTF8.GetBytes(password);
            List<byte> password_hash = GetHash(new List<byte>(pv));

            List<byte> final_input = new List<byte>(BitConverter.GetBytes((UInt32)client_token));
            final_input.AddRange(BitConverter.GetBytes((UInt32)server_token));
            final_input.AddRange(password_hash);

            List<byte> output = GetHash(final_input);

            return output;
        }
    }
}
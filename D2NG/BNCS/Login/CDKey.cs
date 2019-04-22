using System;
using System.Collections.Generic;

namespace D2NG.BNCS.Login
{
    public class CdKey
    {
        string _cdKey;

        public CdKey(string cdKey)
        {
            _cdKey = cdKey;
        }

        static private readonly byte[] alphaMap =
        {
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0x00, 0xFF, 0x01, 0xFF, 0x02, 0x03,
            0x04, 0x05, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B,
            0x0C, 0xFF, 0x0D, 0x0E, 0xFF, 0x0F, 0x10, 0xFF,
            0x11, 0xFF, 0x12, 0xFF, 0x13, 0xFF, 0x14, 0x15,
            0x16, 0xFF, 0x17, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B,
            0x0C, 0xFF, 0x0D, 0x0E, 0xFF, 0x0F, 0x10, 0xFF,
            0x11, 0xFF, 0x12, 0xFF, 0x13, 0xFF, 0x14, 0x15,
            0x16, 0xFF, 0x17, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF
        };

        protected char ConvertToHexDigit(ulong byt)
        {
            byt &= 0xF;
            if (byt < 10)
                return (char)(byt + 0x30);
            else
                return (char)(byt + 0x37);
        }

        protected ulong ConvertFromHexDigit(char input)
        {
            if (input >= '0' && input <= '9')
                return (ulong)(input - 0x30);
            else
                return (ulong)(input - 0x37);
        }

        public bool GetD2KeyHash(ref uint client_token, uint server_token, ref List<byte> output, ref List<byte> public_value)
        {
            ulong checksum = 0;
            ulong n, n2, v, v2;
            char c1, c2, c;

            string manipulatedKey = _cdKey;

            for (int i = 0; i < _cdKey.Length; i += 2)
            {
                char[] tmpBuffer = manipulatedKey.ToCharArray();
                c1 = (char)alphaMap[_cdKey[i]];
                n = (ulong)c1 * 3;
                c2 = (char)alphaMap[_cdKey[i + 1]];
                n = (ulong)c2 + 8 * n;

                if (n >= 0x100)
                {
                    n -= 0x100;
                    ulong temp = (ulong)1 << (i >> 1);
                    checksum |= temp;
                }
                n2 = n;
                n2 >>= 4;
                tmpBuffer[i] = ConvertToHexDigit(n2);
                tmpBuffer[i + 1] = ConvertToHexDigit(n);

                manipulatedKey = new string(tmpBuffer);
            }

            v = 3;

            for (int i = 0; i < 16; i++)
            {
                n = ConvertFromHexDigit(manipulatedKey[i]);
                n2 = v * 2;
                n ^= n2;
                v += n;
            }

            v &= 0xFF;
            if (v != checksum)
                return false;

            for (int i = 15; i >= 0; i--)
            {
                c = manipulatedKey[i];
                if (i > 8)
                    n = (ulong)i - 9;
                else
                    n = 0xF - (ulong)(8 - i);
                n &= 0xF;

                c2 = manipulatedKey[(int)n];
                char[] tmpBuffer = manipulatedKey.ToCharArray();
                tmpBuffer[i] = c2;
                tmpBuffer[n] = c;
                manipulatedKey = new string(tmpBuffer);
            }

            v2 = 0x13AC9741;

            for (int i = 15; i >= 0; i--)
            {
                c = char.ToUpper(manipulatedKey[i]);
                char[] tmpBuffer = manipulatedKey.ToCharArray();
                tmpBuffer[i] = c;


                if (c <= '7')
                {
                    v = v2;
                    c2 = (char)(v & 0xFF);
                    c2 &= (char)7;
                    c2 ^= c;
                    v >>= 3;
                    tmpBuffer[i] = c2;
                    v2 = v;
                }
                else if (c < 'A')
                {
                    c2 = (char)i;
                    c2 &= (char)1;
                    c2 ^= c;
                    tmpBuffer[i] = c2;
                }
                manipulatedKey = new string(tmpBuffer);
            }

            string hexString = manipulatedKey.Substring(2, 6);
            UInt32 num = UInt32.Parse(hexString, System.Globalization.NumberStyles.HexNumber);

            public_value = new List<byte>(BitConverter.GetBytes(num));

            List<byte> hashData = new List<byte>(BitConverter.GetBytes(client_token));
            hashData.AddRange(BitConverter.GetBytes(server_token));


            hashData.AddRange(BitConverter.GetBytes(UInt32.Parse(manipulatedKey.Substring(0, 2), System.Globalization.NumberStyles.HexNumber)));

            hashData.AddRange(BitConverter.GetBytes(num));
            hashData.AddRange(BitConverter.GetBytes((int)0));
            hashData.AddRange(BitConverter.GetBytes(UInt32.Parse(manipulatedKey.Substring(8, 8), System.Globalization.NumberStyles.HexNumber)));

            output = Bsha1.GetHash(hashData);

            return true;
        }
    }
}
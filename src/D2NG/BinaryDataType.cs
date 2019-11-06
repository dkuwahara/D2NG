using System;
using System.Collections.Generic;
using System.IO;

namespace D2NG
{
    class BinaryDataType
    {
        private readonly List<byte> m_data;
        public BinaryDataType(String file)
        {
            m_data = new List<byte>(File.ReadAllBytes(file));
        }

        public Boolean Get(int offset, int length, out byte[] output)
        {
            if (offset < 0 || offset + length > m_data.Count)
            {
                output = null;
                return false;
            }
            output = m_data.GetRange(offset, length).ToArray();
            return true;
        }
    }

}
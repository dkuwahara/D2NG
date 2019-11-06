using System;
using System.Collections.Generic;
using System.IO;

namespace D2NG
{
    class PlainTextDataType
    {
        private readonly List<String[]> m_lines;

        public PlainTextDataType(String file)
        {
            m_lines = new List<string[]>();
            var lines = new List<string>();

            using (StreamReader r = new StreamReader(file))
            {
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }

            foreach (String line in lines)
            {
                String[] tokens = line.Split('|');
                m_lines.Add(tokens);
            }
        }

        public Boolean Get(int offset, out String output)
        {
            if (offset < 0 || offset >= m_lines.Count)
            {
                output = "";
                return false;
            }
            String[] line = m_lines[offset];
            output = line.Length == 0 ? "" : line[0];
            return true;
        }

        public Boolean Get(int offset, out String[] output)
        {
            if (offset < 0 || offset >= m_lines.Count)
            {
                output = null;
                return false;
            }
            output = m_lines[offset];
            return true;
        }
    }

}
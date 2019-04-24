using System;
using System.Collections.Generic;
using System.Text;

namespace D2NG.BNLS
{
    class BnlsClient
    {
        public void Connect(String server)
        {

        }

        public void Disconnect()
        {

        }

        public void Authorize(String user, String password)
        {

        }

        public byte RequestVersionByte(String product)
        {
            return 0x0e;
        }

        public VersionInfo CheckVersion(String Product, long ftime, String filename, String value)
        {
            return null
        }
    }
}

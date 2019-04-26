using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D2NG.BNCS.Packet
{

    public class BncsAuthInfoRequestPacket : BncsPacket
    {
        private static readonly byte[] PROTOCOL_ID = BitConverter.GetBytes(0x00);


        private const String PLATFORM_CODE = "IX86";

        private const String PRODUCT_CODE = "D2XP";

        private const String LANGUAGE_CODE = "enUS";

        private static readonly byte[] LOCAL_IP = BitConverter.GetBytes(0x00);

        private static readonly byte[] TIME_ZONE_BIAS = BitConverter.GetBytes((int)(DateTime.UtcNow.Subtract(DateTime.Now).TotalSeconds / 60));

        private static readonly byte[] MPQ_LOCALE_ID = BitConverter.GetBytes(0x00);

        private static readonly byte[] USER_LANG_ID = BitConverter.GetBytes(0x00);

        private const String COUNTRY_ABBR = "USA\0";

        private const String COUNTRY = "United States\0";

        public BncsAuthInfoRequestPacket()
            : this(0x0e)
        {
        }

        public BncsAuthInfoRequestPacket(int version)
            : base(
                BuildPacket(
                    0x50,
                    PROTOCOL_ID,
                    Encoding.ASCII.GetBytes(PLATFORM_CODE).Reverse().ToArray(),
                    Encoding.ASCII.GetBytes(PRODUCT_CODE).Reverse().ToArray(),
                    BitConverter.GetBytes(version),
                    Encoding.ASCII.GetBytes(LANGUAGE_CODE).Reverse().ToArray(),
                    LOCAL_IP,
                    TIME_ZONE_BIAS,
                    MPQ_LOCALE_ID,
                    USER_LANG_ID,
                    Encoding.ASCII.GetBytes(COUNTRY_ABBR),
                    Encoding.ASCII.GetBytes(COUNTRY)
                )
            )
        {
        }
    }
}

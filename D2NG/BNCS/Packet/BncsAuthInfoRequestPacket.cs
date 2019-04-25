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

        public BncsAuthInfoRequestPacket(String platformCode = PLATFORM_CODE,
                                  String productCode = PRODUCT_CODE,
                                  int productVersion = 0x0e,
                                  String languageCode = LANGUAGE_CODE,
                                  String countryAbbr = COUNTRY_ABBR,
                                  String country = COUNTRY) 
            : base(
                  BuildPacket(
                    0x50,
                    PROTOCOL_ID,
                    Encoding.ASCII.GetBytes(platformCode).Reverse().ToArray(),
                    Encoding.ASCII.GetBytes(productCode).Reverse().ToArray(),
                    BitConverter.GetBytes(productVersion),
                    Encoding.ASCII.GetBytes(languageCode).Reverse().ToArray(),
                    LOCAL_IP,
                    TIME_ZONE_BIAS,
                    MPQ_LOCALE_ID,
                    USER_LANG_ID,
                    Encoding.ASCII.GetBytes(countryAbbr),
                    Encoding.ASCII.GetBytes(country)
                )
            )
        {
        }
    }
}

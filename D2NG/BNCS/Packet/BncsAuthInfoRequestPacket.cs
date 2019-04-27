using System;
using System.Linq;
using System.Text;

namespace D2NG.BNCS.Packet
{

    public class BncsAuthInfoRequestPacket : BncsPacket
    {
        private static readonly byte[] ProtocolId = BitConverter.GetBytes(0x00);


        private const String PlatformCode = "IX86";

        private const String ProductCode = "D2XP";

        private const String LanguageCode = "enUS";

        private static readonly byte[] LocalIp = BitConverter.GetBytes(0x00);

        private static readonly byte[] TimeZoneBias = BitConverter.GetBytes((int)(DateTime.UtcNow.Subtract(DateTime.Now).TotalSeconds / 60));

        private static readonly byte[] MpqLocaleId = BitConverter.GetBytes(0x00);

        private static readonly byte[] UserLangId = BitConverter.GetBytes(0x00);

        private const String CountryAbbr = "USA\0";

        private const String Country = "United States\0";

        public BncsAuthInfoRequestPacket()
            : this(0x0e)
        {
        }

        public BncsAuthInfoRequestPacket(int version)
            : base(
                BuildPacket(
                    0x50,
                    ProtocolId,
                    Encoding.ASCII.GetBytes(PlatformCode).Reverse().ToArray(),
                    Encoding.ASCII.GetBytes(ProductCode).Reverse().ToArray(),
                    BitConverter.GetBytes(version),
                    Encoding.ASCII.GetBytes(LanguageCode).Reverse().ToArray(),
                    LocalIp,
                    TimeZoneBias,
                    MpqLocaleId,
                    UserLangId,
                    Encoding.ASCII.GetBytes(CountryAbbr),
                    Encoding.ASCII.GetBytes(Country)
                )
            )
        {
        }
    }
}

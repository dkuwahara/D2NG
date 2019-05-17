using System;
using System.Linq;
using System.Text;
using Serilog;

namespace D2NG.BNCS.Packet
{

    public class AuthInfoRequestPacket : BncsPacket
    {
        private static readonly byte[] ProtocolId = BitConverter.GetBytes(0x00);

        private const String PlatformCode = "IX86";

        private const String ProductCode = "D2XP";

        private const String LanguageCode = "enUS";

        private static readonly byte[] LocalIp = BitConverter.GetBytes(0x00);

        private static readonly byte[] TimeZoneBias = BitConverter.GetBytes((uint)(DateTime.UtcNow.Subtract(DateTime.Now).TotalSeconds / 60));

        private static readonly byte[] MpqLocaleId = BitConverter.GetBytes(1033);

        private static readonly byte[] UserLangId = BitConverter.GetBytes(1033);

        private const String CountryAbbr = "USA\0";

        private const String Country = "United States\0";

        public AuthInfoRequestPacket()
            : this(0x0E)
        {
            Log.Debug("VERSION BYTE: {0}",  BitConverter.ToString(BitConverter.GetBytes(0x0E)));
        }

        public AuthInfoRequestPacket(int version)
            : base(
                BuildPacket(
                    (byte)Sid.AUTH_INFO,
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

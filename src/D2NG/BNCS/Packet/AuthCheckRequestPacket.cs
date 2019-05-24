using System;
using System.Text;
using D2NG.BNCS.Hashing;

namespace D2NG.BNCS.Packet
{
    public class AuthCheckRequestPacket : BncsPacket
    {
        private static readonly byte[] KeyCount = BitConverter.GetBytes(0x02U);

        private static readonly byte[] IsSpawn = BitConverter.GetBytes(0x00);

        public AuthCheckRequestPacket(
            uint clientToken,
            uint serverToken,
            int version,
            byte[] checksum,
            byte[] info,
            CdKey classic,
            CdKey expansion
            ) : base(BuildPacket(
                    (byte)Sid.AUTH_CHECK,
                    BitConverter.GetBytes(clientToken),
                    BitConverter.GetBytes(version),
                    checksum,
                    KeyCount,
                    IsSpawn,
                    BitConverter.GetBytes(classic.KeyLength),
                    BitConverter.GetBytes(classic.Product),
                    classic.Public,
                    BitConverter.GetBytes(0x00),
                    classic.ComputeHash(clientToken, serverToken),
                    BitConverter.GetBytes(expansion.KeyLength),
                    BitConverter.GetBytes(expansion.Product),
                    expansion.Public,
                    BitConverter.GetBytes(0x00),
                    expansion.ComputeHash(clientToken, serverToken),
                    info,
                    Encoding.ASCII.GetBytes("D2NG\0")
                )
            )
        {
        }
    }
}

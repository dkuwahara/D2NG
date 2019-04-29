using System;
using System.Text;
using D2NG.BNCS.Login;

namespace D2NG.BNCS.Packet
{
    public class BncsAuthCheckRequestPacket : BncsPacket
    {
        private static readonly byte[] KeyCount = BitConverter.GetBytes(0x02);

        private static readonly byte[] ClassicProduct = BitConverter.GetBytes(0x06);

        private static readonly byte[] ExpansionProduct = BitConverter.GetBytes(0x0A);

        private static readonly byte[] IsSpawn = BitConverter.GetBytes(0x00);

        public BncsAuthCheckRequestPacket(
            uint clientToken,
            uint serverToken,
            CheckRevisionResult crResult,
            CdKey classic,
            CdKey expansion
            ) : base(
            BuildPacket(
                    0x51,
                    BitConverter.GetBytes(clientToken),
                    BitConverter.GetBytes(crResult.Version),
                    crResult.Checksum,
                    KeyCount,
                    IsSpawn,
                    BitConverter.GetBytes(classic.KeyLength),
                    BitConverter.GetBytes(classic.Product),
                    classic.Public,
                    BitConverter.GetBytes(0x0),
                    classic.ComputeHash(clientToken, serverToken),
                    BitConverter.GetBytes(expansion.KeyLength),
                    BitConverter.GetBytes(expansion.Product),
                    expansion.Public,
                    BitConverter.GetBytes(0x0),
                    expansion.ComputeHash(clientToken, serverToken),
                    crResult.Info,
                    Encoding.ASCII.GetBytes("D2NG\0")
                )
            )
        {
        }
    }
}

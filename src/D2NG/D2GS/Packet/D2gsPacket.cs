using System.Collections.Generic;
using System.Linq;

namespace D2NG.D2GS.Packet
{
    public class D2gsPacket : D2NG.Packet
    {
        public D2gsPacket(List<byte> packet) : this(packet.ToArray())
        {
        }

        public D2gsPacket(byte[] packet) : base(packet)
        {
        }

        public byte Type { get => Raw[0]; }

        protected static byte[] BuildPacket(D2gs command, params IEnumerable<byte>[] args)
        {
            var packet = new List<byte>
            {
                (byte)command,
            };
            packet.AddRange(args.SelectMany(a => a));
            return packet.ToArray();
        }
    }
}

using D2NG.D2GS.Packet;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace D2NG.D2GS.Act.Packet
{
    class MapRevealPacket : D2gsPacket
    {
        public MapRevealPacket(D2gsPacket packet) : base(packet.Raw)
        {
            var reader = new BinaryReader(new MemoryStream(packet.Raw), Encoding.ASCII);
            var id = reader.ReadByte();
            if (id != 0x07)
            {
                throw new D2GSPacketException("Invalid Packet Id");
            }
            X = reader.ReadUInt16();
            Y = reader.ReadUInt16();
            Area = (Area)reader.ReadByte();
            reader.Close();

            Log.Verbose($"(0x{ id,2:X2}) Map Reveal:\n" +
                $"\tLocation: ({X}, {Y})\n" +
                $"\tArea: {Area}");
        }

        public ushort X { get; }
        public ushort Y { get; }
        public Area Area { get; }
    }
}

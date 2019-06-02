using D2NG.D2GS.Packet;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace D2NG.D2GS.Players.Packet
{
    class ReassignPlayerPacket : D2gsPacket
    {
        public ReassignPlayerPacket(D2gsPacket packet) : base(packet.Raw)
        {
            var reader = new BinaryReader(new MemoryStream(packet.Raw), Encoding.ASCII);
            var id = reader.ReadByte();
            if (id != 0x15)
            {
                throw new D2GSPacketException("Invalid Packet ID");
            }
            UnitType = reader.ReadByte();
            UnitId = reader.ReadUInt32();
            Location = new Point(reader.ReadUInt16(), reader.ReadUInt16());
            _ = reader.ReadByte();
            reader.Close();

            Log.Verbose($"(0x{id,2:X2}) Reassign Player:\n" +
                $"\tUnitType: {UnitType}\n" +
                $"\tUnitId: {UnitId}\n" +
                $"\tLocation: {Location}");
        }

        public byte UnitType { get; }
        public uint UnitId { get; }
        public Point Location { get; }
    }
}

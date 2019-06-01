using System.Collections.Generic;
using System.IO;
using System.Text;
using D2NG.D2GS.Packet;
using Serilog;

namespace D2NG.D2GS.Act.Packet
{
    internal class ActDataPacket : D2gsPacket
    {
        public ActDataPacket(D2gsPacket packet) : base(packet.Raw)
        {
            var reader = new BinaryReader(new MemoryStream(packet.Raw), Encoding.ASCII);
            var id = reader.ReadByte();
            if(id != 0x03)
            {
                throw new D2GSPacketException("Invalid Packet Id");
            }
            Act = reader.ReadByte();
            MapId = reader.ReadUInt32();
            Area = (Area)reader.ReadUInt16();
            _ = reader.ReadUInt32();
            reader.Close();

            Log.Verbose($"(0x{ id,2:X2}) Act Data:\n" +
                $"\tAct: {Act + 1} : {Area}\n" +
                $"\tMapId: {MapId}");
        }

        public byte Act { get; }
        public uint MapId { get; }
        public Area Area { get; }
    }
}
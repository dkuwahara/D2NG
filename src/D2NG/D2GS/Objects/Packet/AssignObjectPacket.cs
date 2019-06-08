using D2NG.D2GS.Packet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace D2NG.D2GS.Objects.Packet
{
    public class AssignObjectPacket : D2gsPacket
    {
        public AssignObjectPacket(D2gsPacket packet) : base(packet.Raw)
        {
            var reader = new BinaryReader(new MemoryStream(packet.Raw), Encoding.ASCII);
            var id = reader.ReadByte();
            if (id != 0x51)
            {
                throw new D2GSPacketException("Invalid Packet Id");
            }
            ObjectType = reader.ReadByte();
            ObjectId = reader.ReadUInt32();
            ObjectCode = reader.ReadUInt16();
            Location = new Point(reader.ReadUInt16(), reader.ReadUInt16());
            State = reader.ReadByte();
            InteractionType = reader.ReadByte();
            reader.Close();
        }

        public WorldObject AsWorldObject() 
            => new WorldObject(ObjectType, ObjectId, ObjectCode, Location, State, InteractionType);

        public byte ObjectType { get; }
        public uint ObjectId { get; }
        public ushort ObjectCode { get; }
        public Point Location { get; }
        public byte State { get; }
        public byte InteractionType { get; }
    }
}

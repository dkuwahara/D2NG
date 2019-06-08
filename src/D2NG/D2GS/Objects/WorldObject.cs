namespace D2NG.D2GS.Objects.Packet
{
    public class WorldObject : Entity
    {
        public ushort Code { get; }
        public byte State { get; }
        public byte InteractionType { get; }

        public WorldObject(byte objectType, uint objectId, ushort objectCode, Point location, byte state, byte interactionType)
        {
            this.Type = objectType;
            this.Id = objectId;
            this.Code = objectCode;
            this.Location = location;
            this.State = state;
            this.InteractionType = interactionType;
        }
    }
}
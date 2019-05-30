using D2NG.D2GS;

namespace D2NG
{
    internal class Player
    {
        public Point Location { get; }
        public string Name { get; }
        public uint Id { get; }
        public CharacterClass Class { get; }

        public Player(AssignPlayer assignPlayer)
        {
            Location = assignPlayer.Location;
            Name = assignPlayer.Name;
            Id = assignPlayer.Id;
            Class= assignPlayer.Class;
        }
    }
}
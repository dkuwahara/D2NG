using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace D2NG.D2GS.Items
{
    public class Item
    {
        public Item()
        {
            Prefixes = new List<uint>();
            Suffixes = new List<uint>();
            Sockets = uint.MaxValue;
        }

        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        [YamlMember(Alias = "ethereal")]
        public bool Ethereal { get; set; }

        public bool has_sockets;

        [YamlMember(Alias = "sockets")]
        public uint Sockets { get; set; }

        [YamlMember(Alias = "quality")]
        public QualityType Quality { get; set; }

        [YamlMember(Alias = "type")]
        public string Type { get; set; }

        public Action Action { get; set; }
        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public uint Category { get; set; }
        public uint Id { get; set; }
        public bool IsEquipped { get; set; }
        public bool IsInSocket { get; set; }
        public bool IsIdentified { get; set; }
        public bool IsSwitchedIn { get; set; }
        public bool IsSwitchedOut { get; set; }
        public bool IsBroken { get; set; }
        public bool IsPotion { get; set; }
        public bool IsInStore { get; set; }
        public bool IsNotInASocket { get; set; }
        public bool IsEar { get; set; }
        public bool IsStartItem { get; set; }
        public bool IsSimpleItem { get; set; }
        public bool IsPersonalised { get; set; }
        public bool Gambling { get; set; }
        public bool IsRuneword { get; set; }
        public bool Ground { get; set; }
        public VersionType Version { get; set; }
        public bool UnspecifiedDirectory { get; set; }
        public uint X { get; set; }
        public uint Y { get; set; }
        public uint Directory { get; set; }
        public ContainerType Container { get; set; }
        public uint EarLevel { get; set; }
        public string EarName { get; set; }
        public bool IsGold { get; set; }
        public uint Amount { get; set; }
        public uint UsedSockets { get; set; }
        public uint Level { get; set; }
        public bool HasGraphic { get; set; }
        public uint Graphic { get; set; }
        public bool HasColour { get; set; }
        public uint Colour { get; set; }
        public uint Prefix { get; set; }
        public uint Suffix { get; set; }
        public uint SetCode { get; set; }
        public uint UniqueCode { get; set; }
        public List<uint> Prefixes { get; set; }
        public List<uint> Suffixes { get; set; }
        public uint RunewordId { get; set; }
        public uint RunewordParameter { get; set; }
        public SuperiorItemClassType Superiority { get; set; }
        public string PersonalisedName { get; set; }
        public bool IsArmor { get; set; }
        public bool IsWeapon { get; set; }
        public uint Defense { get; set; }
        public bool IsIndestructible { get; set; }
        public uint Durability { get; set; }
        public uint MaximumDurability { get; set; }
    }
}
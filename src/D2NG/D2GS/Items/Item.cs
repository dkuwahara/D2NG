using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace D2NG.D2GS.Items
{
    public partial class Item
    {
        public Item()
        {
            prefixes = new List<uint>();
            suffixes = new List<uint>();
            Sockets = uint.MaxValue;
        }

        public Action action;

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

        public ushort width;
        public ushort height;

        public uint category;
        public uint id;
        public bool equipped;
        public bool in_socket;
        public bool identified;
        public bool switched_in;
        public bool switched_out;
        public bool broken;
        public bool potion;

        public bool in_store;
        public bool not_in_a_socket;
        public bool ear;
        public bool start_item;
        public bool simple_item;

        public bool personalised;
        public bool gambling;
        public bool rune_word;

        public bool ground;

        public VersionType version;

        public bool unspecified_directory;
        public uint x;
        public uint y;
        public uint directory;
        public ContainerType container;

        public uint ear_level;
        public string ear_name;

        public bool is_gold;
        public uint amount;

        public uint used_sockets;
        public uint level;

        public bool has_graphic;
        public uint graphic;

        public bool has_colour;
        public uint colour;

        public uint prefix;
        public uint suffix;

        public List<uint> prefixes;
        public List<uint> suffixes;

        public SuperiorItemClassType superiority;

        public uint set_code;
        public uint unique_code;

        public uint runeword_id;
        public uint runeword_parameter;

        public string personalised_name;

        public bool is_armor;
        public bool is_weapon;
        public uint defense;

        public uint indestructible;
        public uint durability;
        public uint maximum_durability;

        public static bool operator <(Item first, Item other)
        {
            return false;
        }

        public static bool operator >(Item first, Item other)
        {
            return false;
        }
    }
}
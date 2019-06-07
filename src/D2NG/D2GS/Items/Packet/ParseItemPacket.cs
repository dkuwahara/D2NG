using D2NG;
using D2NG.D2GS.Items;
using D2NG.D2GS.Packet;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D2NG.Items
{
    class ParseItemPacket : D2gsPacket
    {
        public Item Item { get; }

        public ParseItemPacket(D2gsPacket packet) : base(packet.Raw)
        {
            Item = Parse(Raw.ToList());
            Log.Verbose($"{BitConverter.ToString(Raw)}");
            Log.Verbose($"(0x{packet.Type,2:X2}) Parse Item Packet:\n" +
                $"\t[{Item.Type}] " + $"{ (Item.identified ? "" : "Unidentified")} " + $"{Item.Name}\n" +
                $"\tItem ID: {Item.id}\n" +
                $"\tAction: {Item.action}\n" +
                $"\tContainer: {Item.container}\n" +
                $"\tLevel {Item.level}, {Item.Quality}" + $" { (Item.Ethereal ? "Ethereal" : "")}" +
                $"\t{ (Item.has_sockets ? $"Sockets: {Item.Sockets}" : "" )}");
        }

        private static void GenericInfo(BitReader reader, ref Item item) // get basic info such as item
        {
            byte packet = reader.ReadByte();
            item.action = (Item.Action)reader.ReadByte();
            item.category = reader.ReadByte();
            _ = reader.ReadByte();
            item.id = reader.ReadUInt32();
            if (packet == 0x9d)
            {
                _ = reader.ReadUInt32();
                _ = reader.ReadByte();
            }
        }

        private static void StatusInfo(BitReader reader, ref Item item) // get info for basic status info
        {
            item.equipped = reader.ReadBit();
            reader.ReadBit();
            reader.ReadBit();
            item.in_socket = reader.ReadBit();
            item.identified = reader.ReadBit();
            reader.ReadBit();
            item.switched_in = reader.ReadBit();
            item.switched_out = reader.ReadBit();
            item.broken = reader.ReadBit();
            reader.ReadBit();
            item.potion = reader.ReadBit();
            item.has_sockets = reader.ReadBit();
            reader.ReadBit();
            item.in_store = reader.ReadBit();
            item.not_in_a_socket = reader.ReadBit();
            reader.ReadBit();
            item.ear = reader.ReadBit();
            item.start_item = reader.ReadBit();
            reader.ReadBit();
            reader.ReadBit();
            reader.ReadBit();
            item.simple_item = reader.ReadBit();
            item.Ethereal = reader.ReadBit();
            reader.ReadBit();
            item.personalised = reader.ReadBit();
            item.gambling = reader.ReadBit();
            item.rune_word = reader.ReadBit();
            reader.Read(5);
            item.version = (Item.VersionType)reader.ReadByte();
        }

        private static void ReadLocation(BitReader reader, ref Item item)
        {
            _ = reader.Read(2);
            item.ground = reader.Read(3) == 0x03;

            if (item.ground)
            {
                item.x = reader.ReadUInt16();
                item.y = reader.ReadUInt16();
            }
            else
            {
                item.directory = (byte)reader.Read(4);
                item.x = (byte)reader.Read(4);
                item.y = (byte)reader.Read(3);
                item.container = (Item.ContainerType)(reader.Read(4));
            }
            item.unspecified_directory = false;

            if (item.action == Item.Action.add_to_shop || item.action == Item.Action.remove_from_shop)
            {
                long container = (long)(item.container);
                container |= 0x80;
                if ((container & 1) != 0)
                {
                    container--; //remove first bit
                    item.y += 8;
                }
                item.container = (Item.ContainerType)container;
            }
            else if (item.container == Item.ContainerType.unspecified)
            {
                if (item.directory == (uint)Item.DirectoryType.not_applicable)
                {
                    if (item.in_socket)
                    {
                        //y is ignored for this container type, x tells you the index
                        item.container = Item.ContainerType.item;
                    }
                    else if (item.action == Item.Action.put_in_belt || item.action == Item.Action.remove_from_belt)
                    {
                        item.container = Item.ContainerType.belt;
                        item.y = item.x / 4;
                        item.x %= 4;
                    }
                }
                else
                {
                    item.unspecified_directory = true;
                }
            }
        }

        public static bool EarInfo(BitReader reader, ref Item item)
        {
            if (item.ear)
            {
                reader.Read(3);
                item.ear_level = (byte)reader.Read(7);
                //item.ear_name = "Fix Me"; //fix me later
                List<Byte> ear_name = new List<byte>();
                reader.Read(8);
                while (ear_name.Last() != 0x00)
                {
                    reader.Read(8); // 16 characters of 7 bits each for the name of the ear to process later
                }

                item.ear_name = Convert.ToBase64String(ear_name.ToArray());
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool ReadItemType(BitReader reader, ref Item item) // gets the 3 letter item code
        {
            byte[] code_bytes = new byte[4];
            for (int i = 0; i < code_bytes.Length; i++)
            {
                code_bytes[i] = reader.ReadByte();
            }
            code_bytes[3] = 0;

            item.Type = Encoding.ASCII.GetString(code_bytes).Substring(0, 3);

            ItemEntry entry;
            if (!DataManager.Instance.m_itemData.Get(item.Type, out entry))
            {
                Console.WriteLine("Failed to look up item in item data table");
                return true;
            }

            item.Name = entry.Name;
            item.width = entry.Width;
            item.height = entry.Height;

            item.is_armor = entry.IsArmor();
            item.is_weapon = entry.IsWeapon();

            if (item.Type == "gld")
            {
                item.is_gold = true;
                bool big_pile = reader.ReadBit();
                if (big_pile)
                {
                    item.amount = (uint)reader.Read(32);
                }
                else
                {
                    item.amount = (uint)reader.Read(12);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void ReadSocketInfo(BitReader reader, ref Item item)
        {
            item.used_sockets = (byte)reader.Read(3);
        }

        public static bool ReadLevelQuality(BitReader reader, ref Item item)
        {
            item.Quality = Item.QualityType.Normal;
            if (item.simple_item || item.gambling)
            {
                return false;
            }
            item.level = (byte)reader.Read(7);
            item.Quality = (Item.QualityType)(reader.Read(4));
            return true;
        }

        public static void ReadGraphicInfo(BitReader reader, ref Item item)
        {
            item.has_graphic = reader.ReadBit(); ;
            if (item.has_graphic)
            {
                item.graphic = (byte)reader.Read(3);
            }

            item.has_colour = reader.ReadBit();
            if (item.has_colour)
            {
                item.colour = (UInt16)reader.Read(11);
            }
        }

        public static void ReadIdentifiedInfo(BitReader reader, ref Item item)
        {
            if (item.identified)
            {
                switch (item.Quality)
                {
                    case Item.QualityType.Inferior:
                        item.prefix = (byte)reader.Read(3);
                        break;
                    case Item.QualityType.Superior:
                        item.superiority = (Item.SuperiorItemClassType)(reader.Read(3));
                        break;
                    case Item.QualityType.Magical:
                        item.prefix = (uint)reader.Read(11);
                        item.suffix = (uint)reader.Read(11);
                        break;

                    case Item.QualityType.Crafted:
                    case Item.QualityType.Rare:
                        item.prefix = (uint)reader.Read(8) - 156;
                        item.suffix = (uint)reader.Read(8) - 1;
                        break;

                    case Item.QualityType.Set:
                        item.set_code = (uint)reader.Read(12);
                        break;
                    case Item.QualityType.Unique:
                        if (item.Type != "std") //standard of heroes exception?
                        {
                            item.unique_code = (uint)reader.Read(12);
                        }
                        break;
                    default:
                        break;
                }
            }

            if (item.Quality == Item.QualityType.Rare || item.Quality == Item.QualityType.Crafted)
            {
                for (ulong i = 0; i < 3; i++)
                {
                    if (reader.ReadBit())
                    {
                        item.prefixes.Add((uint)reader.Read(11));
                    }
                    if (reader.ReadBit())
                    {
                        item.suffixes.Add((uint)reader.Read(11));
                    }
                }
            }

            if (item.rune_word)
            {
                item.runeword_id = (uint)reader.Read(12);
                item.runeword_parameter = (byte)reader.Read(4);
                //std::cout << "runeword_id: " << item.runeword_id << ", parameter: " << item.runeword_parameter << std::endl;
            }

            if (item.personalised)
            {
                var personalised_name = new List<byte>();
                reader.Read(8);
                while (personalised_name.Last() != 0x00)
                {
                    reader.Read(8); // 16 characters of 7 bits each for the name of the ear to process later
                }
                item.personalised_name = Convert.ToBase64String(personalised_name.ToArray()); //this is also a problem part i'm not sure about
            }

            if (item.is_armor)
            {
                item.defense = (uint)reader.Read(11) - 10;
            }

            if (item.Type == "7cr")
            {
                reader.Read(8);
            }
            else if (item.is_armor || item.is_weapon)
            {
                item.maximum_durability = (byte)reader.Read(8);
                item.indestructible = (uint)((item.maximum_durability == 0) ? 1 : 0);

                item.durability = (byte)reader.Read(8);
                reader.ReadBit();
            }
            if (item.has_sockets)
            {
                item.Sockets = (byte)reader.Read(4);
            }
        }

        public static Item Parse(List<byte> packet)
        {
            var item = new Item();
            var reader = new BitReader(packet.ToArray());
            try
            {
                GenericInfo(reader, ref item);
                StatusInfo(reader, ref item);
                ReadLocation(reader, ref item);
                if (EarInfo(reader, ref item))
                {
                    return item;
                }
                if (ReadItemType(reader, ref item))
                {
                    return item;
                }
                ReadSocketInfo(reader, ref item);
                if (!ReadLevelQuality(reader, ref item))
                {
                    return item;
                }
                ReadGraphicInfo(reader, ref item);
                ReadIdentifiedInfo(reader, ref item); 
            }
            catch (Exception e)
            {
                Log.Error(e, "Failed to parse item");
            }
            return item;
        }
    }
}
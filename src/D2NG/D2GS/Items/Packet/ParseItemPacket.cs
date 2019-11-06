using D2NG;
using D2NG.D2GS.Items;
using D2NG.D2GS.Packet;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Action = D2NG.D2GS.Items.Action;

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
                $"\tLevel {Item.Level}, {Item.Quality}" + $" { (Item.Ethereal ? "Ethereal" : "")}" +
                $"\t{ (Item.HasSockets ? $"Sockets: {Item.Sockets}" : "")}\n" + 
                $"\t[{Item.Type}] " + $"{ (Item.IsIdentified ? "" : "Unidentified")} " + $"{Item.Name}\n" +
                $"\tItem ID: {Item.Id}\n" +
                $"\tAction: {Item.Action}\n" +
                $"\tContainer: {Item.Container}");
        }

        private static void GenericInfo(BitReader reader, ref Item item) // get basic info such as item
        {
            byte packet = reader.ReadByte();
            item.Action = (Action)reader.ReadByte();
            item.Category = reader.ReadByte();
            _ = reader.ReadByte();
            item.Id = reader.ReadUInt32();
            if (packet == 0x9d)
            {
                _ = reader.ReadUInt32();
                _ = reader.ReadByte();
            }
        }

        private static void StatusInfo(BitReader reader, ref Item item) // get info for basic status info
        {
            item.IsEquipped = reader.ReadBit();
            reader.ReadBit();
            reader.ReadBit();
            item.IsInSocket = reader.ReadBit();
            item.IsIdentified = reader.ReadBit();
            reader.ReadBit();
            item.IsSwitchedIn = reader.ReadBit();
            item.IsSwitchedOut = reader.ReadBit();
            item.IsBroken = reader.ReadBit();
            reader.ReadBit();
            item.IsPotion = reader.ReadBit();
            item.HasSockets = reader.ReadBit();
            reader.ReadBit();
            item.IsInStore = reader.ReadBit();
            item.IsNotInASocket = reader.ReadBit();
            reader.ReadBit();
            item.IsEar = reader.ReadBit();
            item.IsStartItem = reader.ReadBit();
            reader.ReadBit();
            reader.ReadBit();
            reader.ReadBit();
            item.IsSimpleItem = reader.ReadBit();
            item.Ethereal = reader.ReadBit();
            reader.ReadBit();
            item.IsPersonalised = reader.ReadBit();
            item.Gambling = reader.ReadBit();
            item.IsRuneword = reader.ReadBit();
            reader.Read(5);
            item.Version = (VersionType)reader.ReadByte();
        }

        private static void ReadLocation(BitReader reader, ref Item item)
        {
            _ = reader.Read(2);
            item.Ground = reader.Read(3) == 0x03;

            if (item.Ground)
            {
                item.X = reader.ReadUInt16();
                item.Y = reader.ReadUInt16();
            }
            else
            {
                item.Directory = (byte)reader.Read(4);
                item.X = (byte)reader.Read(4);
                item.Y = (byte)reader.Read(3);
                item.Container = (ContainerType)(reader.Read(4));
            }
            item.UnspecifiedDirectory = false;

            if (item.Action == Action.add_to_shop || item.Action == Action.remove_from_shop)
            {
                long container = (long)(item.Container);
                container |= 0x80;
                if ((container & 1) != 0)
                {
                    container--; //remove first bit
                    item.Y += 8;
                }
                item.Container = (ContainerType)container;
            }
            else if (item.Container == ContainerType.unspecified)
            {
                if (item.Directory == (uint)DirectoryType.not_applicable)
                {
                    if (item.IsInSocket)
                    {
                        //y is ignored for this container type, x tells you the index
                        item.Container = ContainerType.item;
                    }
                    else if (item.Action == Action.put_in_belt || item.Action == Action.remove_from_belt)
                    {
                        item.Container = ContainerType.belt;
                        item.Y = item.X / 4;
                        item.X %= 4;
                    }
                }
                else
                {
                    item.UnspecifiedDirectory = true;
                }
            }
        }

        public static bool EarInfo(BitReader reader, ref Item item)
        {
            if (item.IsEar)
            {
                reader.Read(3);
                item.EarLevel = (byte)reader.Read(7);
                //item.ear_name = "Fix Me"; //fix me later
                List<Byte> ear_name = new List<byte>();
                reader.Read(8);
                while (ear_name.Last() != 0x00)
                {
                    reader.Read(8); // 16 characters of 7 bits each for the name of the ear to process later
                }

                item.EarName = Convert.ToBase64String(ear_name.ToArray());
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

            if (!DataManager.Instance.ItemData.Get(item.Type, out ItemEntry entry))
            {
                Log.Error("Failed to look up item in item data table");
                return true;
            }

            item.Name = entry.Name;
            item.Width = entry.Width;
            item.Height = entry.Height;

            item.IsArmor = entry.IsArmor();
            item.IsWeapon = entry.IsWeapon();

            if (item.Type == "gld")
            {
                item.IsGold = true;
                bool big_pile = reader.ReadBit();
                if (big_pile)
                {
                    item.Amount = (uint)reader.Read(32);
                }
                else
                {
                    item.Amount = (uint)reader.Read(12);
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
            item.UsedSockets = (byte)reader.Read(3);
        }

        public static bool ReadLevelQuality(BitReader reader, ref Item item)
        {
            item.Quality = QualityType.Normal;
            if (item.IsSimpleItem || item.Gambling)
            {
                return false;
            }
            item.Level = (byte)reader.Read(7);
            item.Quality = (QualityType)(reader.Read(4));
            return true;
        }

        public static void ReadGraphicInfo(BitReader reader, ref Item item)
        {
            item.HasGraphic = reader.ReadBit();
            if (item.HasGraphic)
            {
                item.Graphic = (byte)reader.Read(3);
            }

            item.HasColour = reader.ReadBit();
            if (item.HasColour)
            {
                item.Colour = (ushort)reader.Read(11);
            }
        }

        public static void ReadIdentifiedInfo(BitReader reader, ref Item item)
        {
            if (item.IsIdentified)
            {
                switch (item.Quality)
                {
                    case QualityType.Inferior:
                        item.Prefix = (byte)reader.Read(3);
                        break;
                    case QualityType.Superior:
                        item.Superiority = (SuperiorItemClassType)(reader.Read(3));
                        break;
                    case QualityType.Magical:
                        item.Prefix = (uint)reader.Read(11);
                        item.Suffix = (uint)reader.Read(11);
                        break;
                    case QualityType.Crafted:
                    case QualityType.Rare:
                        item.Prefix = (uint)reader.Read(8) - 156;
                        item.Suffix = (uint)reader.Read(8) - 1;
                        break;
                    case QualityType.Set:
                        item.SetCode = (uint)reader.Read(12);
                        break;
                    case QualityType.Unique:
                        if (item.Type != "std") //standard of heroes exception?
                        {
                            item.UniqueCode = (uint)reader.Read(12);
                        }
                        break;
                    default:
                        // No additional bits to read for item.Quality
                        break;
                }
            }

            if (item.Quality == QualityType.Rare || item.Quality == QualityType.Crafted)
            {
                for (var i = 0; i < 3; i++)
                {
                    var hasPrefix = reader.ReadBit();
                    if (hasPrefix)
                    {
                        item.Prefixes.Add((uint)reader.Read(11));
                    }
                    var hasSuffix = reader.ReadBit();
                    if (hasSuffix)
                    {
                        item.Suffixes.Add((uint)reader.Read(11));
                    }
                }
            }

            if (item.IsRuneword)
            {
                item.RunewordId = (uint)reader.Read(12);
                item.RunewordParameter = (byte)reader.Read(4);
            }

            if (item.IsPersonalised)
            {
                var personalisedName = new List<byte>();
                personalisedName.Add(reader.ReadByte());
                while (personalisedName.Last() != 0x00)
                {
                    personalisedName.Add(reader.ReadByte()); // 16 characters of 7 bits each for the name of the ear to process later
                }
                item.PersonalisedName = Encoding.ASCII.GetString(personalisedName.ToArray()); 
            }

            if (item.IsArmor)
            {
                item.Defense = (uint)reader.Read(11) - 10;
            }

            if (item.Type == "7cr")
            {
                reader.Read(8);
            }
            else if (item.IsArmor || item.IsWeapon)
            {
                item.MaximumDurability = (byte)reader.Read(8);
                item.IsIndestructible = item.MaximumDurability == 0 ? true : false;

                item.Durability = (byte)reader.Read(8);
                reader.ReadBit();
            }
            if (item.HasSockets)
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using D2NG.D2GS.Items;
using Serilog;

namespace D2NG
{
    class DataManager
    {
        private static DataManager sm_instance;

        public static DataManager Instance
        {
            get
            {
                if (sm_instance == null)
                {
                    sm_instance = new DataManager();
                }
                return sm_instance;
            }

        }
        public ItemDataType m_itemData;
        public PlainTextDataType m_experiences,
                            m_magicalPrefixes,
                            m_magicalSuffixes,
                            m_rarePrefixes,
                            m_rareSuffixes,
                            m_uniqueItems,
                            m_monsterNames,
                            m_monsterFields,
                            m_superUniques,
                            m_itemProperties,
                            m_skills;

        public DataManager(String dataDirectory = "data")
        {
            String[] fileNames =
            {
                "experience.txt",
                "magical_prefixes.txt",
                "magical_suffixes.txt",
                "rare_prefixes.txt",
                "rare_suffixes.txt",
                "unique_items.txt",
                "monster_names.txt",
                "monster_fields.txt",
                "super_uniques.txt",
                "item_properties.txt",
                "skills.txt"
            };

            String itemDataFile = Path.Combine(dataDirectory, "item_data.txt");
            m_itemData = new ItemDataType(itemDataFile);
            m_experiences = new PlainTextDataType(Path.Combine(dataDirectory, fileNames[0]));
            m_magicalPrefixes = new PlainTextDataType(Path.Combine(dataDirectory, fileNames[1]));
            m_magicalSuffixes = new PlainTextDataType(Path.Combine(dataDirectory, fileNames[2]));
            m_rarePrefixes = new PlainTextDataType(Path.Combine(dataDirectory, fileNames[3]));
            m_rareSuffixes = new PlainTextDataType(Path.Combine(dataDirectory, fileNames[4]));
            m_uniqueItems = new PlainTextDataType(Path.Combine(dataDirectory, fileNames[5]));
            m_monsterNames = new PlainTextDataType(Path.Combine(dataDirectory, fileNames[6]));
            m_monsterFields = new PlainTextDataType(Path.Combine(dataDirectory, fileNames[7]));
            m_superUniques = new PlainTextDataType(Path.Combine(dataDirectory, fileNames[8]));
            m_itemProperties = new PlainTextDataType(Path.Combine(dataDirectory, fileNames[9]));
            m_skills = new PlainTextDataType(Path.Combine(dataDirectory, fileNames[10]));
        }
    }

    class ItemDataType
    {
        public List<ItemEntry> Items { get; }

        private ItemDataType()
        {
        }

        public ItemDataType(String file)
        {
            Items = new List<ItemEntry>();
            Dictionary<String, ClassificationType> classificationMap = new Dictionary<string, ClassificationType>();
            classificationMap["Amazon Bow"] = ClassificationType.amazon_bow;
            classificationMap["Amazon Javelin"] = ClassificationType.amazon_javelin;
            classificationMap["Amazon Spear"] = ClassificationType.amazon_spear;
            classificationMap["Amulet"] = ClassificationType.amulet;
            classificationMap["Antidote Potion"] = ClassificationType.antidote_potion;
            classificationMap["Armor"] = ClassificationType.armor;
            classificationMap["Arrows"] = ClassificationType.arrows;
            classificationMap["Assassin Katar"] = ClassificationType.assassin_katar;
            classificationMap["Axe"] = ClassificationType.axe;
            classificationMap["Barbarian Helm"] = ClassificationType.barbarian_helm;
            classificationMap["Belt"] = ClassificationType.belt;
            classificationMap["Body Part"] = ClassificationType.body_part;
            classificationMap["Bolts"] = ClassificationType.bolts;
            classificationMap["Boots"] = ClassificationType.boots;
            classificationMap["Bow"] = ClassificationType.bow;
            classificationMap["Circlet"] = ClassificationType.circlet;
            classificationMap["Club"] = ClassificationType.club;
            classificationMap["Crossbow"] = ClassificationType.crossbow;
            classificationMap["Dagger"] = ClassificationType.dagger;
            classificationMap["Druid Pelt"] = ClassificationType.druid_pelt;
            classificationMap["Ear"] = ClassificationType.ear;
            classificationMap["Elixir"] = ClassificationType.elixir;
            classificationMap["Gem"] = ClassificationType.gem;
            classificationMap["Gloves"] = ClassificationType.gloves;
            classificationMap["Gold"] = ClassificationType.gold;
            classificationMap["Grand Charm"] = ClassificationType.grand_charm;
            classificationMap["Hammer"] = ClassificationType.hammer;
            classificationMap["Health Potion"] = ClassificationType.health_potion;
            classificationMap["Helm"] = ClassificationType.helm;
            classificationMap["Herb"] = ClassificationType.herb;
            classificationMap["Javelin"] = ClassificationType.javelin;
            classificationMap["Jewel"] = ClassificationType.jewel;
            classificationMap["Key"] = ClassificationType.key;
            classificationMap["Large Charm"] = ClassificationType.large_charm;
            classificationMap["Mace"] = ClassificationType.mace;
            classificationMap["Mana Potion"] = ClassificationType.mana_potion;
            classificationMap["Necromancer Shrunken Head"] = ClassificationType.necromancer_shrunken_head;
            classificationMap["Paladin Shield"] = ClassificationType.paladin_shield;
            classificationMap["Polearm"] = ClassificationType.polearm;
            classificationMap["Quest Item"] = ClassificationType.quest_item;
            classificationMap["Rejuvenation Potion"] = ClassificationType.rejuvenation_potion;
            classificationMap["Ring"] = ClassificationType.ring;
            classificationMap["Rune"] = ClassificationType.rune;
            classificationMap["Scepter"] = ClassificationType.scepter;
            classificationMap["Scroll"] = ClassificationType.scroll;
            classificationMap["Shield"] = ClassificationType.shield;
            classificationMap["Small Charm"] = ClassificationType.small_charm;
            classificationMap["Sorceress Orb"] = ClassificationType.sorceress_orb;
            classificationMap["Spear"] = ClassificationType.spear;
            classificationMap["Staff"] = ClassificationType.staff;
            classificationMap["Stamina Potion"] = ClassificationType.stamina_potion;
            classificationMap["Sword"] = ClassificationType.sword;
            classificationMap["Thawing Potion"] = ClassificationType.thawing_potion;
            classificationMap["Throwing Axe"] = ClassificationType.throwing_axe;
            classificationMap["Throwing Knife"] = ClassificationType.throwing_knife;
            classificationMap["Throwing Potion"] = ClassificationType.throwing_potion;
            classificationMap["Tome"] = ClassificationType.tome;
            classificationMap["Torch"] = ClassificationType.torch;
            classificationMap["Wand"] = ClassificationType.wand;

            List<string> lines = new List<string>();

            using (StreamReader r = new StreamReader(file))
            {
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }

            foreach (string line in lines)
            {
                try
                {
                    String[] tokens = line.Split('|');
                    if (tokens.Length == 0)
                    {
                        continue;
                    }

                    if (tokens.Length != 8)
                    {
                        Log.Error("Invalid Token Count: {0}", tokens.Length);
                        throw new Exception("Unable to parse item data");
                    }
                    String name = tokens[0];
                    String code = tokens[1];
                    String classification_string = tokens[2];
                    UInt16 width = UInt16.Parse(tokens[3]);
                    UInt16 height = UInt16.Parse(tokens[4]);
                    bool stackable = UInt32.Parse(tokens[5]) != 0;
                    bool usable = UInt32.Parse(tokens[6]) != 0;
                    bool throwable = UInt32.Parse(tokens[7]) != 0;
                    ClassificationType classification;
                    if (!classificationMap.TryGetValue(classification_string, out classification))
                    {
                        throw new Exception("Unable to parse item classification");
                    }

                    ItemEntry i = new ItemEntry(name, code, classification, width, height, stackable, usable, throwable);
                    Items.Add(i);
                }
                catch (Exception e)
                {
                    Log.Error("Error parsing ItemDataType: {0}", e.ToString());
                }
            }
        }

        public Boolean Get(String code, out ItemEntry output)
        {
            var items = from n in Items where n.Type == code select n;

            foreach (ItemEntry i in items)
            {
                output = i;
                return true;
            }
            output = null;
            return false;
        }

    }
    class PlainTextDataType
    {
        private List<String[]> m_lines;

        public PlainTextDataType(String file)
        {
            m_lines = new List<string[]>();
            List<string> lines = new List<string>();

            using (StreamReader r = new StreamReader(file))
            {
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }

            foreach (String line in lines)
            {
                String[] tokens = line.Split('|');
                m_lines.Add(tokens);
            }
        }

        public Boolean Get(int offset, out String output)
        {
            if (offset < 0 || offset >= m_lines.Count)
            {
                output = "";
                return false;
            }
            String[] line = m_lines[offset];
            output = line.Length == 0 ? "" : line[0];
            return true;
        }

        public Boolean Get(int offset, out String[] output)
        {
            if (offset < 0 || offset >= m_lines.Count)
            {
                output = null;
                return false;
            }
            output = m_lines[offset];
            return true;
        }
    }

    class BinaryDataType
    {
        private List<byte> m_data;
        public BinaryDataType(String file)
        {
            m_data = new List<byte>(File.ReadAllBytes(file));
        }

        public Boolean Get(int offset, int length, out byte[] output)
        {
            if (offset < 0 || offset + length > m_data.Count)
            {
                output = null;
                return false;
            }
            output = m_data.GetRange(offset, length).ToArray();
            return true;
        }
    }

}
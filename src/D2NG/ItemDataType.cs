using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using D2NG.D2GS.Items;
using Serilog;

namespace D2NG
{
    class ItemDataType
    {
        public List<ItemEntry> Items { get; }

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
                    }
                    String name = tokens[0];
                    String code = tokens[1];
                    String classification_string = tokens[2];
                    UInt16 width = UInt16.Parse(tokens[3]);
                    UInt16 height = UInt16.Parse(tokens[4]);
                    bool stackable = UInt32.Parse(tokens[5]) != 0;
                    bool usable = UInt32.Parse(tokens[6]) != 0;
                    bool throwable = UInt32.Parse(tokens[7]) != 0;
                    var classification = classificationMap[classification_string];
                    var i = new ItemEntry(name, code, classification, width, height, stackable, usable, throwable);
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
}
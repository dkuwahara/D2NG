using System;

namespace D2NG.D2GS.Items
{
    class ItemEntry
    {
        public String Name;
        public String Type;
        public Item.ClassificationType Classification;
        public UInt16 Width, Height;
        public Boolean Stackable, Usable, Throwable;

        public ItemEntry()
        {

        }
        public ItemEntry(String name, String type, Item.ClassificationType classification, UInt16 width, UInt16 height, Boolean stackable, Boolean usable, Boolean throwable)
        {
            Name = name;
            Type = type;
            Classification = classification;
            Width = width;
            Height = height;
            Stackable = stackable;
            Usable = usable;
            Throwable = throwable;
        }

        public Boolean IsArmor()
        {
            switch (Classification)
            {
                case Item.ClassificationType.helm:
                case Item.ClassificationType.armor:
                case Item.ClassificationType.shield:
                case Item.ClassificationType.gloves:
                case Item.ClassificationType.boots:
                case Item.ClassificationType.belt:
                case Item.ClassificationType.druid_pelt:
                case Item.ClassificationType.barbarian_helm:
                case Item.ClassificationType.paladin_shield:
                case Item.ClassificationType.necromancer_shrunken_head:
                case Item.ClassificationType.circlet:
                    return true;
            }
            return false;
        }
        public Boolean IsWeapon()
        {
            switch (Classification)
            {
                case Item.ClassificationType.amazon_bow:
                case Item.ClassificationType.amazon_javelin:
                case Item.ClassificationType.amazon_spear:
                case Item.ClassificationType.assassin_katar:
                case Item.ClassificationType.axe:
                case Item.ClassificationType.bow:
                case Item.ClassificationType.club:
                case Item.ClassificationType.crossbow:
                case Item.ClassificationType.dagger:
                case Item.ClassificationType.hammer:
                case Item.ClassificationType.javelin:
                case Item.ClassificationType.mace:
                case Item.ClassificationType.polearm:
                case Item.ClassificationType.scepter:
                case Item.ClassificationType.sorceress_orb:
                case Item.ClassificationType.spear:
                case Item.ClassificationType.sword:
                case Item.ClassificationType.staff:
                case Item.ClassificationType.throwing_axe:
                case Item.ClassificationType.throwing_knife:
                case Item.ClassificationType.throwing_potion:
                case Item.ClassificationType.wand:
                    return true;
            }
            return false;
        }
    }
}
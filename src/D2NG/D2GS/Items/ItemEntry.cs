using System;

namespace D2NG.D2GS.Items
{
    class ItemEntry
    {
        public String Name;
        public String Type;
        public ClassificationType Classification;
        public UInt16 Width, Height;
        public Boolean Stackable, Usable, Throwable;

        public ItemEntry(String name, String type, ClassificationType classification, UInt16 width, UInt16 height, Boolean stackable, Boolean usable, Boolean throwable)
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
                case ClassificationType.helm:
                case ClassificationType.armor:
                case ClassificationType.shield:
                case ClassificationType.gloves:
                case ClassificationType.boots:
                case ClassificationType.belt:
                case ClassificationType.druid_pelt:
                case ClassificationType.barbarian_helm:
                case ClassificationType.paladin_shield:
                case ClassificationType.necromancer_shrunken_head:
                case ClassificationType.circlet:
                    return true;
            }
            return false;
        }
        public Boolean IsWeapon()
        {
            switch (Classification)
            {
                case ClassificationType.amazon_bow:
                case ClassificationType.amazon_javelin:
                case ClassificationType.amazon_spear:
                case ClassificationType.assassin_katar:
                case ClassificationType.axe:
                case ClassificationType.bow:
                case ClassificationType.club:
                case ClassificationType.crossbow:
                case ClassificationType.dagger:
                case ClassificationType.hammer:
                case ClassificationType.javelin:
                case ClassificationType.mace:
                case ClassificationType.polearm:
                case ClassificationType.scepter:
                case ClassificationType.sorceress_orb:
                case ClassificationType.spear:
                case ClassificationType.sword:
                case ClassificationType.staff:
                case ClassificationType.throwing_axe:
                case ClassificationType.throwing_knife:
                case ClassificationType.throwing_potion:
                case ClassificationType.wand:
                    return true;
            }
            return false;
        }
    }
}
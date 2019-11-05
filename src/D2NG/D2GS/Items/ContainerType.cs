namespace D2NG.D2GS.Items
{
    public partial class Item
    {
        public enum ContainerType
        {
            unspecified = 0,
            inventory = 2,
            trader_offer = 4,
            for_trade = 6,
            cube = 8,
            stash = 0x0A,
            belt = 0x20,
            item = 0x40,
            armor_tab = 0x82,
            weapon_tab_1 = 0x84,
            weapon_tab_2 = 0x86,
            misc_tab = 0x88,
        }
    }
}
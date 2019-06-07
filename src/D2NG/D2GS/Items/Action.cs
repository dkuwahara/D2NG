namespace D2NG.D2GS.Items
{
    public partial class Item
    {
        public enum Action
        {
            add_to_ground = 0,
            ground_to_cursor = 1,
            drop_to_ground = 2,
            on_ground = 3,
            put_in_container = 4,
            remove_from_container = 5,
            equip = 6,
            indirectly_swap_body_item = 7,
            unequip = 8,
            swap_body_item = 9,
            add_quantity = 0x0A,
            add_to_shop = 0x0B,
            remove_from_shop = 0x0C,
            swap_in_container = 0x0D,
            put_in_belt = 0x0E,
            remove_from_belt = 0x0F,
            swap_in_belt = 0x10,
            auto_unequip = 0x11,
            to_cursor = 0x12,
            item_in_socket = 0x13,
            update_stats = 0x15,
            weapon_switch = 0x17
        };
    }
}
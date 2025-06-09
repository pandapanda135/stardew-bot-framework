using System.Xml;
using StardewValley;

namespace StardewBotFramework;

public class InventoryManagement
{
    const int ToolBarCount = 12;
    public int CurrentItemSlotCount = Game1.player.MaxItems;

    private static readonly int[] EquippableSlots = new int[]{37,38,39,40,41,42}; // temporary so I don't forget need to do more digging in how the game handles it though

    #region Methods

        public static void ThrowItem(Item item, int quantity)
        {
            
        }

        public static void ThrowStack(Item item)
        {
            
        }

        public static void BinItem(Item item, int quantity) // maybe roll both this and bin stack together?
        {
            
        }

        public static void BinStack(Item item)
        {
            
        }

        public static void MoveItem(Item item, int column, int row) // maybe change this to int of slot not 100% sure on that due to being able to change inventory
        {
            
        }

        public static void SelectRow(int row) // row of items to be used in gameplay
        {
            
        }

        public static void SelectSlot(int slot) // slot in inventory to select from 0-9 (if I remember correctly)
        {
            
        }
    #endregion
}
using StardewModdingAPI.Events;
using StardewValley;

namespace StardewBotFramework.Source.Events;

public class BotInventoryChangedEventArgs : EventArgs
{
    /// <summary>The added item stacks.</summary>
    public IEnumerable<Item> Added { get; }

    /// <summary>The removed item stacks.</summary>
    public IEnumerable<Item> Removed { get; }

    /// <summary>The item stacks whose size changed.</summary>
    public IEnumerable<ItemStackSizeChange> QuantityChanged { get; }
    
    internal BotInventoryChangedEventArgs(Item[] added, Item[] removed, ItemStackSizeChange[] quantityChanged)
    {
        this.Added = added;
        this.Removed = removed;
        this.QuantityChanged = quantityChanged;
    }
}
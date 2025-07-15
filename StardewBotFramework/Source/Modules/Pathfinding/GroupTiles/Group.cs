namespace StardewBotFramework.Source.Modules.Pathfinding.GroupTiles;

/// <summary>
/// A group of tiles, they should all share a common aspect
/// </summary>
public class Group
{
    private List<ITile> Tiles = new();

    public void Add(ITile tile)
    {
        Tiles.Add(tile);
    }

    public void Remove(ITile tile)
    {
        Tiles.Remove(tile);
    }

    public void RemoveAt(int index)
    {
        Tiles.RemoveAt(index);
    }

    public bool Contains(ITile tile)
    {
        return Tiles.Contains(tile);
    }

    public ITile GetAt(int index)
    {
        return Tiles[index];
    }

    public int GetIndex(ITile tile)
    {
        return Tiles.IndexOf(tile);
    }

    public List<ITile> GetTiles()
    {
        return Tiles;
    }
}
using StardewValley;

namespace StardewPathfinding.Pathfinding;

public class PathQueue
{
    private int total_size;

    private Queue<PathNode> nodes;

    public bool IsEmpty()
    {
        if (nodes.Count == 0)
        {
            return true;
        }
        return false;
    }
    
    public void Clear()
    {
        nodes.Clear();
    }

    public void Enqueue(PathNode node)
    {
        nodes.Enqueue(node);
    }

    public PathNode Dequeue()
    {
        return nodes.Dequeue();
    }
}


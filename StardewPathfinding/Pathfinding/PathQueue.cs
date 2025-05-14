using StardewValley;

namespace StardewPathfinding.Pathfinding;

public class PathQueue
{
    private int total_size;

    private Queue<PathNode> _nodes = new Queue<PathNode>();

    public bool IsEmpty()
    {
        if (_nodes.Count == 0)
        {
            return true;
        }
        return false;
    }
    
    public void Clear()
    {
        _nodes.Clear();
    }

    public void Enqueue(PathNode node)
    {
        _nodes.Enqueue(node);
    }

    public PathNode Dequeue()
    {
        return _nodes.Dequeue();
    }
}


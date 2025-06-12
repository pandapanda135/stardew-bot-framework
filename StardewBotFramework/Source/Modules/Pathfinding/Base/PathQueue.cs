namespace StardewBotFramework.Source.Modules.Pathfinding.Base;

public class PathQueue
{
    private int TotalSize => _nodes.Count;

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

    /// <summary>
    /// Adds an object to the end of Queue.
    /// </summary>
    /// <param name="node">Object to be put to the end</param>
    public void Enqueue(PathNode node)
    {
        _nodes.Enqueue(node);
    }

    /// <summary>
    /// Removes and returns the object at the beginning of the Queue.
    /// </summary>
    /// <returns>The object to be removed</returns>
    public PathNode Dequeue()
    {
        return _nodes.Dequeue();
    }
}


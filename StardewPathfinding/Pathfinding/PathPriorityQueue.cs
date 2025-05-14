using Microsoft.Xna.Framework;

namespace StardewPathfinding.Pathfinding;

public class PathPriorityQueue
{
    private int totalSize;

    public SortedDictionary<int, Queue<PathNode>> nodes;
    
    public void Enqueue(PathNode nodeItem,int priority)
    {
        Queue<PathNode> node = new Queue<PathNode>();
        if (!nodes.TryGetValue(priority, out node))
        {
            nodes.Add(priority, new Queue<PathNode>());
            Enqueue(nodeItem, priority);
        }
        else
        {
            node.Enqueue(nodeItem);
            totalSize++;
        }
    }
}

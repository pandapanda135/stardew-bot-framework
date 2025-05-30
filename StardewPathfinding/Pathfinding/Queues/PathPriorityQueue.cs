using System.Reflection.Metadata.Ecma335;
using Microsoft.Xna.Framework;
using StardewPathfinding.Debug;

namespace StardewPathfinding.Pathfinding;

public class PathPriorityQueue
{
    public SortedDictionary<PathNode,int> Nodes = new();
    
    public void Enqueue(PathNode nodeItem, int priority)
    {
        Nodes.Add(nodeItem, priority);
    }

    public PathNode Dequeue()
    {
        SortedDictionary<PathNode,int>.KeyCollection keys = Nodes.Keys;

        PathNode first = keys.First();
        Nodes.Remove(keys.First());
        Logger.Info($"the first Key is {first.X},{first.Y}");
        return first;
    }

    public new void Clear()
    {
        Nodes.Clear();
    }

    public new bool IsEmpty()
    {
        if (Nodes.Count < 1)
            return true;
        return false;
    }

    public bool Contains(PathNode node)
    {
        return Nodes.ContainsKey(node);
    }
}

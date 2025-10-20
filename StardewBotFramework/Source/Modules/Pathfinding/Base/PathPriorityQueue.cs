namespace StardewBotFramework.Source.Modules.Pathfinding.Base;

public class PathPriorityQueue : PriorityQueue<PathNode,int>
{
    public bool IsEmpty()
    {
        return Count < 1;
    }

    public bool Contains(PathNode node)
    {
        return UnorderedItems.Any(queueNode => Equals(queueNode.Element, node));
    }
}

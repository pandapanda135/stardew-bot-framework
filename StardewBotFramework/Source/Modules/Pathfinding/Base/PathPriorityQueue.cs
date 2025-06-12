namespace StardewBotFramework.Source.Modules.Pathfinding.Base;

public class PathPriorityQueue : PriorityQueue<PathNode,int>
{
    public bool IsEmpty()
    {
        if (Count < 1)
            return true;
        return false;
    }

    public bool Contains(PathNode node)
    {
        foreach (var queueNode in UnorderedItems)
        {
            if (queueNode.Element == node)
            {
                return true;
            }
        }
        
        return false;
    }
}

using StardewBotFramework.Debug;
using StardewBotFramework.Source.Modules.Pathfinding.Algorithms;
using StardewBotFramework.Source.Modules.Pathfinding.Base;
using StardewValley;

namespace StardewBotFramework.Source.Utilities;

/// <summary>
/// This is for helping with pathfinding in the framework.
/// </summary>
internal static class PathfindingHelper
{
	public static async Task<bool> Goto(Goal goal, bool canDestroy = false, bool buildCollision = true)
	{
		AlgorithmBase.IPathing pathing = new AStar.Pathing();

		if (buildCollision) pathing.BuildCollisionMap(BotBase.CurrentLocation);

		PathNode start = new PathNode(BotBase.Farmer.TilePoint.X, BotBase.Farmer.TilePoint.Y, null);

		Stack<PathNode> path = await pathing.FindPath(start, goal, BotBase.CurrentLocation, 10000, canDestroy);
		if (path.Count == 0) return false;

		Character? npc = null;
		if (goal is Goal.GoalDynamic dynamic) npc = dynamic.Character;

		var controller = new CharacterController(BotBase.CurrentLocation);
		controller.StartMoveCharacter(path, npc);

		Logger.Info($"thread pool: {Thread.CurrentThread.IsThreadPoolThread}");
		// gets stuck here :(
		_ = Task.Run(() =>
		{
			while (CharacterController.IsMoving()) {}
		});

		return true;
	}
	
	/// <summary>
	/// Follow or attack a character
	/// </summary>
	public static async Task FollowCharacter(Goal goal, bool canDestroy = false, bool buildCollision = true)
	{
		AlgorithmBase.IPathing pathing = new AStar.Pathing();
		if (buildCollision) pathing.BuildCollisionMap(BotBase.CurrentLocation);
        
		PathNode start = new PathNode(BotBase.Farmer.TilePoint.X, BotBase.Farmer.TilePoint.Y, null);

		Stack<PathNode> path = await pathing.FindPath(start, goal, BotBase.CurrentLocation, 10000, canDestroy);

		Character? npc = null;
		
		if (goal is Goal.GoalDynamic dynamic) npc = dynamic.Character; 
		
		var controller = new CharacterController(BotBase.CurrentLocation);
		controller.StartMoveCharacter(path,npc,true);

		while (CharacterController.IsMoving())
		{
		}
	}
}
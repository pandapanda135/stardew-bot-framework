using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using StardewBotFramework.Debug;
using StardewBotFramework.Source.Modules.Pathfinding;
using StardewBotFramework.Source.Modules.Pathfinding.Algorithms;
using StardewBotFramework.Source.Modules.Pathfinding.Base;
using StardewValley;

namespace BotTesting;

/// <summary>
/// This is for testing the performance of the AStar pathfinding algorithm, this will create a .csv file in a folder
/// called "performance" in stardew valley's game file folder (where SMAPI is installed) for the files to be
/// written where they will catalog each run's performance in milliseconds.
/// </summary>
public static class PathfindPerformance
{
	public static async Task TestMethods(Goal goal, int amount, bool includeCollision = true, bool pathfinding = true,
		bool controller = false)
	{
		TaskDispatcher.Initialize();
		Dictionary<string, List<string>> results = new()
		{
			{"Collision", new()},
			{"Pathing", new()},
			{"Controller", new()},
			{"Total", new()}
		};

		await TaskDispatcher.SwitchToMainThread();
		Vector2 startingPos = Game1.player.Position;
		for (int i = 0; i < amount; i++)
		{
			List<KeyValuePair<string, Stopwatch>> watches = new();
			Game1.player.Position = startingPos;
			Logger.Info($"amount: {i}");
			Stopwatch totalWatch = Stopwatch.StartNew();
			await TaskDispatcher.SwitchToMainThread();
			AlgorithmBase.IPathing pathing = new AStar.Pathing();

			if (includeCollision)
			{
				var kvp = Collision(pathing);
				watches.Add(kvp);	
			}

			Stack<PathNode> path = new();
			if (pathfinding)
			{
				PathNode start = new PathNode(Game1.player.TilePoint.X, Game1.player.TilePoint.Y, null);
				var (p, watchKvp) = await GetPath(pathing,start,goal);
				path = p;
				watches.Add(watchKvp);	
			}

			if (controller)
			{
				var controllerKvp = await Controller(path);
				watches.Add(controllerKvp);	
			}
			
			totalWatch.Stop();
			await TaskDispatcher.SwitchToMainThread();
			// switching back after should allow more accurate timing
			watches.Add(new("Total",totalWatch));
			foreach (var kvp in watches)
			{
				results[kvp.Key].Add(kvp.Value.ElapsedMilliseconds.ToString());
			}
		}
		
		CsvWriter.Write(results);
	}

	private static KeyValuePair<string, Stopwatch> Collision(AlgorithmBase.IPathing pathing)
	{
		var collision = Stopwatch.StartNew();
		AlgorithmBase.IPathing.CollisionMap = new CollisionMap();
		pathing.BuildCollisionMap(Game1.currentLocation);
		collision.Stop();
		return new("Collision", collision);
	}

	private static async Task<KeyValuePair<Stack<PathNode>,KeyValuePair<string,Stopwatch>>> GetPath(AlgorithmBase.IPathing pathing, PathNode startNode,Goal goal)
	{
		var pathingWatch = Stopwatch.StartNew();
		
		var path = await pathing.FindPath(startNode, goal, Game1.currentLocation, 10000);
		pathingWatch.Stop();

		return new(path, new("Pathing", pathingWatch));
	}

	private static Task<KeyValuePair<string, Stopwatch>> Controller(Stack<PathNode> path)
	{
		var controllerWatch = Stopwatch.StartNew();
		
		var characterController = new CharacterController(Game1.currentLocation);
		characterController.StartMoveCharacter(path);
		while (CharacterController.IsMoving()) {}
		controllerWatch.Stop();
		return Task.FromResult<KeyValuePair<string, Stopwatch>>(new("Controller", controllerWatch));
	}

	public static async void Command(string arg, string[] args)
	{
		try
		{
			await TestMethods(new Goal.GoalPosition(int.Parse(args[0]), int.Parse(args[1])), int.Parse(args[2]));
		}
		catch (Exception e)
		{
			Logger.Error($"{e}");
		}
	}
	
	internal static class TaskDispatcher
	{
		private static readonly ConcurrentQueue<Action> Actions = new();
		private static int _mainThreadId;

		public static void Initialize()
		{
			_mainThreadId = Environment.CurrentManagedThreadId;
		}

		public static void RunPending()
		{
			while (Actions.TryDequeue(out var action)) action();
		}

		private static bool IsMainThread => Environment.CurrentManagedThreadId == _mainThreadId;

		private static void Post(Action action)
		{
			Actions.Enqueue(action);
		}

		/// <summary>
		/// Anything after this will be run on the main thread, async methods will only switch a thread pool thread after
		/// something has been awaited. If it is already running on the main thread nothing will change. 
		/// </summary>
		public static Task SwitchToMainThread()
		{
			if (IsMainThread) return Task.CompletedTask;

			var tcs = new TaskCompletionSource();
			Post(() => tcs.SetResult());
			return tcs.Task;
		}
	}
}
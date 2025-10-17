using System.Collections.Concurrent;
using System.Diagnostics;
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
public static class PathfindPerformace
{
	public static async Task Test(Goal goal, int amount)
	{
		TaskDispatcher.Initialize();
		await TaskDispatcher.SwitchToMainThread();
		Dictionary<string, List<string>> results = new()
		{
			{"Collision", new()},
			{"Pathing", new()},
			{"Total", new()}
		};

		for (int i = 0; i < amount; i++)
		{
			Logger.Info($"amount: {i}");
			Stopwatch totalWatch = Stopwatch.StartNew();
			await TaskDispatcher.SwitchToMainThread();
			AlgorithmBase.IPathing pathing = new AStar.Pathing();
			AlgorithmBase.IPathing.collisionMap = new CollisionMap();

			var collision = Stopwatch.StartNew();
			pathing.BuildCollisionMap(Game1.currentLocation);
			collision.Stop();
			results["Collision"].Add(collision.ElapsedMilliseconds.ToString());

			var pathingWatch = Stopwatch.StartNew();
			PathNode start = new PathNode(Game1.player.TilePoint.X, Game1.player.TilePoint.Y, null);
			var path = await pathing.FindPath(start, goal, Game1.currentLocation, 10000);
			pathingWatch.Stop();
			totalWatch.Stop();
			// running on different threads might cause issues with data?
			await TaskDispatcher.SwitchToMainThread();
			results["Pathing"].Add(pathingWatch.ElapsedMilliseconds.ToString());
			results["Total"].Add(totalWatch.ElapsedMilliseconds.ToString());
		}
		
		CsvWriter.Write(results);
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
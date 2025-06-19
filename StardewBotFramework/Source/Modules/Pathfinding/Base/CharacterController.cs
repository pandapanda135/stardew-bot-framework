using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using Logger = StardewBotFramework.Debug.Logger;

namespace StardewBotFramework.Source.Modules.Pathfinding.Base;

public class CharacterController
{
	public delegate void OnPathfindingFinished();
	public static event OnPathfindingFinished OnGoalReached;
	
	private static bool _movingCharacter = false;

	private static Stack<PathNode> _endPath = new();
	private static Character _character = new();
	private static GameLocation _currentLocation = new();
	private static GameTime _time = new();
	private static PathNode? _next;
	private static PathNode _currentNode;
	
	public static void Update(object? sender, UpdateTickingEventArgs e)
	{
		// if it has been a minute
		// if (_runningTimer + 10000 > Game1.currentGameTime.TotalGameTime.Ticks)
		// {
		// 	_movingCharacter = false;
		// }

		if (_endPath.Count < 1) _movingCharacter = false;
		
		if (!_movingCharacter) return;

		// foreach (var node in _endPath)
		// {
		// 	if (Game1.player.TilePoint == node.VectorLocation)
		// 	{
		// 		Logger.Info($"player position and node are same");
		// 		if (node.Destroy)
		// 		{
		// 			Logger.Info($"trying to use tool");
		// 			Game1.player.BeginUsingTool();
		// 		}		
		// 	}
		// }
		
		// if (_endPath.Count > 2)
		// {
		// 	_next = _endPath.ToArray()[_endPath.ToArray().Length - 1];
		// 	Logger.Info($"setting _next   pos: {_next.VectorLocation.ToString()}   destroy: {_next.Destroy}");
		// }
					
		
		MoveCharacter(_time);
	}

	public static void StartMoveCharacter(Stack<PathNode> endPointPath, Character character, GameLocation location,
		GameTime time)
	{
		_movingCharacter = true;
		_endPath = endPointPath;
		_character = character;
		_currentLocation = location;
		_time = time;
		
		if (IsMoving()) return;

		MoveCharacter(time);
	}
	
	private static void MoveCharacter(GameTime time)
	{
		PathNode node = _endPath.Peek();
		PathNode? next = null;
		if (_endPath.Count > 2)
		{
			next = _endPath.ToArray()[_endPath.ToArray().Length - 1];
		}

		Rectangle targetTile = new Rectangle(node.X * 64, node.Y * 64, 64, 64);
		Rectangle bbox = _character.GetBoundingBox();

		if ((targetTile.Contains(bbox) || (bbox.Width > targetTile.Width && targetTile.Contains(bbox.Center))) &&
		    targetTile.Bottom - bbox.Bottom >= 2)
		{
			_endPath.Pop();
			_character.stopWithoutChangingFrame();
			
			if (next is not null && next.Destroy)
			{
				Game1.player.BeginUsingTool();
			}
			
			if (_endPath.Count == 0)
			{
				// OnGoalReached.Invoke();
				_character.Halt();
			}

			return;
		}

		Farmer farmer = _character as Farmer;
		if (farmer != null)
		{
			farmer.movementDirections.Clear();
		}

		if (bbox.Left < targetTile.Left && bbox.Right < targetTile.Right)
		{
			_character.SetMovingRight(true);
			_character.FacingDirection = 1;
		}
		else if (bbox.Right > targetTile.Right && bbox.Left > targetTile.Left)
		{
			_character.SetMovingLeft(true);
			_character.FacingDirection = 3;
		}
		else if (bbox.Top <= targetTile.Top)
		{
			_character.SetMovingDown(true);
			_character.FacingDirection = 2;
		}
		else if (bbox.Bottom >= targetTile.Bottom - 2)
		{
			_character.SetMovingUp(true);
			_character.FacingDirection = 0;
		}
		
		foreach (var nextNode in _endPath)
		{
			int index = _endPath.ToList().IndexOf(nextNode);
			PathNode nextPathNode;
			try
			{
				nextPathNode = _endPath.ToArray()[index - 1];
			}
			catch (Exception e)
			{
				break;
			}
			// if (_endPath.ToArray()[index - 1]) break;
			
			if (nextPathNode.VectorLocation == _currentNode.VectorLocation) break;
			
			_currentNode = nextNode;
			Logger.Info($"player position and node are same");
			if (nextPathNode.Destroy)
			{
				Logger.Info($"trying to use tool");
				Game1.player.BeginUsingTool();
			}		
		}
		// if (next is not null && Game1.player.TilePoint == next.VectorLocation)
		// {
		// 	Logger.Info($"player position and node are same");
		// 	if (next.Destroy)
		// 	{
		// 		Logger.Info($"trying to use tool");
		// 		Game1.player.BeginUsingTool();
		// 	}		
		// }
		
		_character.MovePosition(time, Game1.viewport, _currentLocation);
	}

	public static bool IsMoving() => _movingCharacter;
}
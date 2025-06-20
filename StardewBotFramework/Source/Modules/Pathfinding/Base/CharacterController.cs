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
	private static bool _isDestroying = false;

	private static Stack<PathNode> _endPath = new();
	private static Character _character = new();
	private static GameLocation _currentLocation = new();
	private static GameTime _time = new();
	private static int _nextIndex;
	private static int _neighbourIndex;
	private static PathNode _currentNode;
	private static PathNode _nextNode;
	
	private static readonly sbyte[,] Directions = new sbyte[4,2]
	{
		{ -1, 0 }, // west
		{ 1, 0 }, // east
		{ 0, 1 }, // south
		{ 0, -1 }, // north
	};
	
	public static void Update(object? sender, UpdateTickingEventArgs e)
	{
		// if it has been a minute
		// if (_runningTimer + 10000 > Game1.currentGameTime.TotalGameTime.Ticks)
		// {
		// 	_movingCharacter = false;
		// }

		if (_endPath.Count < 1) _movingCharacter = false;
		
		if (!_movingCharacter) return;
		
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
		if (Game1.player.UsingTool) return; // check if animation running
		
		if (_isDestroying) // check if destroy
		{
			_isDestroying = false;
			_endPath.ToArray()[_nextIndex].Destroy = false;
			_endPath.ToArray()[_neighbourIndex].Destroy = false;
		}
		
		Logger.Info($"running MoveCharacter");
		PathNode node = _endPath.Peek();
		Rectangle targetTile = new Rectangle(node.X * 64, node.Y * 64, 64, 64);
		Rectangle bbox = _character.GetBoundingBox();

		if ((targetTile.Contains(bbox) || (bbox.Width > targetTile.Width && targetTile.Contains(bbox.Center))) &&
		    targetTile.Bottom - bbox.Bottom >= 2)
		{
			_endPath.Pop();
			_character.stopWithoutChangingFrame();
			
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
		
		foreach (var pathNode in _endPath)
		{
			_nextIndex = _endPath.ToList().IndexOf(pathNode);
			if (_nextIndex < 0)
			{
				_nextNode = _endPath.ToList()[_nextIndex - 1];
				_neighbourIndex = _endPath.ToList().IndexOf(_nextNode); // need this to set next PathNode.Destroy to false
			}

			Logger.Info($"running foreach: {pathNode.VectorLocation}  destroy: {pathNode.Destroy}");
			if (pathNode.Destroy && !_isDestroying)
			{
				for (int i = 0; i <= 3; i++)
				{
					// get cardinal directions
					int neighborX = pathNode.X + Directions[i, 0];
					int neighborY = pathNode.Y + Directions[i, 1];

					Logger.Info($"getting cardinal directions: {neighborX}, {neighborY}   player pos: {Game1.player.TilePoint.X}, {Game1.player.TilePoint.Y}");		
					if (neighborX == Game1.player.TilePoint.X && neighborY == Game1.player.TilePoint.Y) // need to get neighbour 
					{
						// this is to fix issues with sudden direction changes in path (should maybe try to make it so the bot goes in the middle of a tile as this is kind of a patch fix)
						switch (i)
						{
							case 0:
								_character.FacingDirection = 1; // west
								break;
							case 1:
								_character.FacingDirection = 3; // east
								break;
							case 2:
								_character.FacingDirection = 0; // south
								break;
							case 3:
								_character.FacingDirection = 2; // north
								break;
						}
						
						Logger.Info($"trying to use tool");
						_isDestroying = true;
						Game1.player.BeginUsingTool();
						_character.MovePosition(time, Game1.viewport, _currentLocation);
						return;
					}
				}
			}
		}
		
		_character.MovePosition(time, Game1.viewport, _currentLocation);
	}

	public static bool IsMoving() => _movingCharacter;
}
using Microsoft.Xna.Framework;
using StardewBotFramework.Source.Modules.Pathfinding.Algorithms;
using StardewBotFramework.Source.ObjectToolSwaps;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using Logger = StardewBotFramework.Debug.Logger;
using Object = StardewValley.Object;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace StardewBotFramework.Source.Modules.Pathfinding.Base;

public class CharacterController
{
	// public delegate void OnPathfindingFinished();
	// public static event OnPathfindingFinished OnGoalReached;
	/// <summary>
	/// This is for when pathfinding gets cancelled from staying in the same place for too long.
	/// </summary>
	public static event EventHandler? FailedPathFinding;
	
	private static bool _movingCharacter;
	private static bool _isDestroying;

	private static Stack<PathNode> _endPath = new();
	private static Character _character => BotBase.Farmer;
	private static GameLocation _currentLocation = null!;
	private static GameTime Time => Game1.currentGameTime;
	private static int _nextIndex;
	private static int _neighbourIndex;
	private static Character? _dynamicCharacter;
	private static Vector2 _startingCharacterPos;
	private static PathNode _currentNode = null!;
	private static PathNode _nextNode = null!;
	
	private static readonly sbyte[,] Directions = new sbyte[4,2]
	{
		{ -1, 0 }, // west
		{ 1, 0 }, // east
		{ 0, 1 }, // south
		{ 0, -1 }, // north
	};

	private static int _pausedTimer;
	/// <summary>
	/// The maximum amount of time the bot can stay in the same position for. This is in milliseconds e.g. 5 seconds would be 5000
	/// </summary>
	public static readonly int MaxPauseTime = 5000;
	public static void Update(object? sender, UpdateTickingEventArgs e)
	{
		if (_endPath.Count < 1) _movingCharacter = false;
		
		if (!_movingCharacter) return;

		if (!BotBase.CurrentLocation.Equals(_currentLocation))
		{
			ForceStopMoving();
			return;
		} // stop issue with moving to the left when go through warp

		Vector2 position = BotBase.Farmer.Position;
		MoveCharacter(Time);
		if (position == BotBase.Farmer.Position)
		{
			_pausedTimer += Time.ElapsedGameTime.Milliseconds;
		}
		else
		{
			_pausedTimer = 0;
		}

		if (_pausedTimer < MaxPauseTime) return;
		
		Logger.Error($"Paused for too long");
		ForceStopMoving();
		FailedPathFinding?.Invoke(new CharacterController(),EventArgs.Empty);
	}

	public static void StartMoveCharacter(Stack<PathNode> endPointPath, Character? character = null)
	{
		Logger.Info($"start move character");
		if (IsMoving()) return;
	
		_movingCharacter = true;
		_endPath = endPointPath;
		_currentLocation = BotBase.CurrentLocation;
		_dynamicCharacter = character;

		Logger.Info($"end path: {_endPath.Count}    end point: {endPointPath.Count}");
		if (character is not null) _startingCharacterPos = character.Position;
		Logger.Info($"end path: {_endPath.Count}    end point: {endPointPath.Count}");
		
		MoveCharacter(Time);
	}
	
	private static void MoveCharacter(GameTime time) //TODO: figure out why movePosition does not change character's animation.
	{
		if (BotBase.Farmer.UsingTool) return; // check if animation running
		
		if (_isDestroying) // check if destroy
		{
			_isDestroying = false;
			_endPath.ToArray()[_nextIndex].Destroy = false;
			_endPath.ToArray()[_neighbourIndex].Destroy = false;
		}

		if (_dynamicCharacter is not null && !_currentLocation.characters.Contains((NPC)_dynamicCharacter))
		{
			FailedPathFinding?.Invoke(new CharacterController(),EventArgs.Empty);
			ForceStopMoving();
			return;
		}
		// recalculate path is character moves away from current path
		if (_dynamicCharacter is not null && _dynamicCharacter.Position != _startingCharacterPos)
		{
			AlgorithmBase.IPathing pathing = new AStar.Pathing();
			PathNode start = new PathNode(_character.TilePoint.X, _character.TilePoint.Y, null);
			Task.Run(async () =>
			{
				var path = await pathing.FindPath(start, new Goal.GoalDynamic(_dynamicCharacter, 1),
					_currentLocation, 10000);
				if (path.Count > 0) _endPath = path;
				_startingCharacterPos = _dynamicCharacter.Position;
			});
		}
		
		PathNode node = _endPath.Peek();
		int tilesize = Game1.tileSize;
		Rectangle targetTile = new Rectangle(node.X * tilesize, node.Y * tilesize, tilesize, tilesize); // could probably change this if we want more precise pathfinding onto specific parts of tile
		targetTile.Inflate(-2, 0);
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

		if (_character is Farmer farmer)
		{
			if (farmer != BotBase.Farmer)
			{
				Logger.Error($"Farmer is not the bot");
				return;
			}
			farmer.movementDirections.Clear();
		}

		if (_currentLocation is not MovieTheater)
		{
			foreach (var npc in _currentLocation.characters)
			{
				if (!npc.Equals(_character) && npc.GetBoundingBox().Intersects(_character.GetBoundingBox()) && npc.isMoving())
				{
					Logger.Error($"Ran into a character"); //TODO: re-calulate path (Could get into a loop of being in character until the npc moves. Maybe check next node for character)
				}
			}
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
			if (_nextIndex == 0)
			{
				int indexPlus = _nextIndex;
				if (_endPath.Count > 1) indexPlus += 1;
				_nextNode = _endPath.ToList()[indexPlus];
				_neighbourIndex = _endPath.ToList().IndexOf(_nextNode); // need this to set next PathNode.Destroy to false
				Object objectInNextTile = _currentLocation.getObjectAtTile(_nextNode.X,_nextNode.Y);
			
				if (objectInNextTile is Fence)
				{
					if (objectInNextTile is Fence fence && fence.isGate.Value && !fence.isPassable())
					{
						fence.toggleGate(true);
					}
				}

				if (objectInNextTile is not null && !objectInNextTile.isPassable())
				{
					Logger.Error($"The object in the next tile was not passable, the object was a {objectInNextTile.Name}");
					_character.Halt();
					FailedPathFinding?.Invoke(new CharacterController(),EventArgs.Empty);
					return;
				}
			}
			
			if (!pathNode.Destroy || _isDestroying)
			{
				_character.MovePosition(time, Game1.viewport, _currentLocation);
				HandleWarp(Game1.player);
				return;
			}
			for (int i = 0; i <= 3; i++)
			{
				// get cardinal directions
				int neighborX = pathNode.X + Directions[i, 0];
				int neighborY = pathNode.Y + Directions[i, 1];

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
					SwapItem(node.VectorLocation);
					Game1.player.BeginUsingTool();
					_character.MovePosition(time, Game1.viewport, _currentLocation);
					HandleWarp(Game1.player);
					return;
				}
			}
		}
	}

	private static void HandleWarp(Character character)
	{
		Warp warp = Game1.currentLocation.isCollidingWithWarp(Game1.player.GetBoundingBox(), _character);
		if (warp is null) return;
		
		if (Game1.eventUp)
		{
			Event currentEvent = Game1.CurrentEvent;
			if (!(!(currentEvent != null ? new bool?(currentEvent.isFestival) : null) ?? true))
			{
				Game1.CurrentEvent.TryStartEndFestivalDialogue(character as Farmer);
			}
		}
		Game1.player.warpFarmer(warp,Game1.player.FacingDirection);
	}
	
	public static bool isPlayerPresent()
	{
		
		return _currentLocation.farmers.Any();
	}
	public static bool IsMoving() => _movingCharacter;

	public static void ForceStopMoving()
	{
		_endPath = new Stack<PathNode>();
		_currentNode = new PathNode(-1, -1, null);
		_nextNode = _currentNode;
		_movingCharacter = false;
	}
	
	private static bool SwapItem(Point tile)
	{
		if (TerrainFeatureToolSwap.Swap(tile)) // we also handle bushes here
		{
			TerrainFeatureToolSwap.Swap(tile);
			return true;
		}

		if (ResourceClumpToolSwap.Swap(tile))
		{
			ResourceClumpToolSwap.Swap(tile);
			return true;
		}

		if (LitterObjectToolSwap.Swap(tile))
		{
			LitterObjectToolSwap.Swap(tile);
			return true;
		}

		return false;
	}
}
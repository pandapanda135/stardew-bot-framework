using Microsoft.Xna.Framework;
using StardewBotFramework.Source.Modules.Pathfinding.Algorithms;
using StardewBotFramework.Source.ObjectToolSwaps;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.Tools;
using Logger = StardewBotFramework.Debug.Logger;
using Object = StardewValley.Object;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace StardewBotFramework.Source.Modules.Pathfinding.Base;

public class CharacterController
{
	/// <summary>
	/// This is for when pathfinding gets cancelled from staying in the same place for too long.
	/// </summary>
	public static event EventHandler? FailedPathFinding;

	enum Direction
	{
		South = 0,
		North = 2,
		East = 1,
		West = 3,
	}
	
	private static bool _movingCharacter;
	private static bool _isDestroying;

	private static Stack<PathNode> _endPath = new();
	private static Character _character => BotBase.Farmer;
	private static GameLocation _currentLocation = null!;
	private static GameTime Time => Game1.currentGameTime;
	private static int _nextIndex;
	private static int _neighbourIndex;
	private static Character? _dynamicCharacter;
	private static Point _startingCharacterTile;
	private static PathNode _currentNode = null!;
	private static PathNode _nextNode = null!;

	private static bool _attacking;
	
	private static readonly sbyte[,] Directions =
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
	private const int MaxPauseTime = 5000;
	public static void Update(object? sender, UpdateTickingEventArgs e)
	{
		if (_endPath.Count < 1) _movingCharacter = false;

		var monster = _dynamicCharacter as Monster;
		if ((!_movingCharacter && !_attacking) || (_attacking && monster is not null && monster.Health < 1))
		{
			return;
		}

		if (!BotBase.CurrentLocation.Equals(_currentLocation))
		{
			ForceStopMoving();
			return;
		} // stop issue with moving to the left when go through warp
		
		Vector2 position = _character.Position;
		if (_character is Farmer farmer1)
		{
			farmer1.setRunning(true, true);
			farmer1.updateMovementAnimation(Time);
		}
		MoveCharacter(Time);
		if (_character is Farmer farmer)
		{
			farmer.setRunning(true, true);
			farmer.updateMovementAnimation(Time);
		}
		if (position == _character.Position)
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

	public static void StartMoveCharacter(Stack<PathNode> endPointPath, Character? character = null,bool attacking = false)
	{
		Logger.Info($"start move character");
		if (IsMoving()) return;
	
		// stop pathfinder getting stuck as _movingCharacter wouldn't get set to false again
		if (!endPointPath.Any()) return;
		
		_movingCharacter = true;
		_endPath = endPointPath;
		_currentLocation = BotBase.CurrentLocation;
		_dynamicCharacter = character;
		_attacking = attacking;

		if (character is not null) _startingCharacterTile = character.TilePoint;

		MoveCharacter(Time);
	}

	// this is so bot faces correct direction with diagonal tiles
	private static readonly int[] CorrectFacingDirections = { 0, 1, 2, 3, 0, 0, 2, 2 };

	private static void MoveCharacter(GameTime time) //TODO: figure out why movePosition does not change character's animation.
	{
		if (BotBase.Farmer.UsingTool) return; // check if animation running
		
		if (_isDestroying) // check if destroy
		{
			_isDestroying = false;
			_endPath.ToArray()[_nextIndex].Destroy = false;
			_endPath.ToArray()[_neighbourIndex].Destroy = false;
		}

		if (_dynamicCharacter is not null && !_currentLocation.characters.Contains(_dynamicCharacter))
		{
			if (!_attacking) // if bot is attacking will also be removed if dies
			{
				FailedPathFinding?.Invoke(new CharacterController(),EventArgs.Empty);
			}
			ForceStopMoving();
			return;
		}
		
		if (_dynamicCharacter is not null && _attacking &&
		    (Graph.IsInNeighbours(_character.TilePoint, _dynamicCharacter.TilePoint, out var direction)
		     || _dynamicCharacter.TilePoint == _character.TilePoint))
		{
			Logger.Info($"attacking in character controller");
			BotBase.Farmer.FacingDirection = CorrectFacingDirections[direction];
			SwapItemHandler.SwapItem(typeof(MeleeWeapon),"Weapon");
			BotBase.Farmer.BeginUsingTool();
			return;
		}
		
		// recalculate path is character moves away from current path
		if (_dynamicCharacter is not null && _dynamicCharacter.TilePoint != _startingCharacterTile)
		{
			// stop issues with pathing, if in wall or other occupied tile mainly for monsters.
			if (_currentLocation.isCollidingPosition(_dynamicCharacter.GetBoundingBox(),Game1.viewport,_dynamicCharacter)) return;
			_startingCharacterTile = _dynamicCharacter.TilePoint;
			AlgorithmBase.IPathing pathing = new AStar.Pathing();
			// pathing.BuildCollisionMap(_currentLocation, _character.TilePoint.X + 10, _character.TilePoint.Y + 10
			// 	,_character.TilePoint.X - 10, _character.TilePoint.Y - 10);
			PathNode start = new PathNode(_character.TilePoint.X, _character.TilePoint.Y, null);
			Task.Run(async () =>
			{
				var path = await pathing.FindPath(start, new Goal.GoalDynamic(_dynamicCharacter, 1),
					_currentLocation, 10000);
				if (path.Count > 0) _endPath = path;
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
			if (_currentLocation.characters.Any(npc => !npc.Equals(_character) && npc.GetBoundingBox()
				    .Intersects(_character.GetBoundingBox()) && npc.isMoving()))
			{
				Logger.Error($"Ran into a character"); //TODO: re-calulate path (Could get into a loop of being in character until the npc moves. Maybe check next node for character)
				FailedPathFinding?.Invoke(new CharacterController(),EventArgs.Empty);
				return;
			}
		}
		
		if (bbox.Left < targetTile.Left && bbox.Right < targetTile.Right)
		{
			_character.SetMovingRight(true);
			_character.FacingDirection = (int)Direction.East;
		}
		else if (bbox.Right > targetTile.Right && bbox.Left > targetTile.Left)
		{
			_character.SetMovingLeft(true);
			_character.FacingDirection = (int)Direction.West;
		}
		else if (bbox.Top <= targetTile.Top)
		{
			_character.SetMovingDown(true);
			_character.FacingDirection = (int)Direction.North;
		}
		else if (bbox.Bottom >= targetTile.Bottom - 2)
		{
			_character.SetMovingUp(true);
			_character.FacingDirection = (int)Direction.South;
		}

		foreach (var pathNode in _endPath)
		{
			_nextIndex = _endPath.ToList().IndexOf(pathNode);
			if (_nextIndex != 0) continue;
			
			int indexPlus = _nextIndex;
			if (_endPath.Count > 1) indexPlus += 1;
			_nextNode = _endPath.ToList()[indexPlus];
			_neighbourIndex =
				_endPath.ToList().IndexOf(_nextNode); // need this to set next PathNode.Destroy to false
			Object objectInNextTile = _currentLocation.getObjectAtTile(_nextNode.X, _nextNode.Y);

			if (objectInNextTile is Fence fence && fence.isGate.Value && !fence.isPassable())
			{
				fence.toggleGate(true);
			}

			if (objectInNextTile is null || objectInNextTile.isPassable()) continue;
				
			Logger.Error($"The object in the next tile was not passable, the object was a {objectInNextTile.Name}");
			_character.Halt();
			FailedPathFinding?.Invoke(new CharacterController(), EventArgs.Empty);
			return;
		}

		PathNode peekNode = _endPath.Peek();
		if (!peekNode.Destroy || _isDestroying)
		{
			try
			{
				_character.MovePosition(time, Game1.viewport, _currentLocation);
				HandleWarp(_character.nextPosition(_character.getDirection()));
			}
			catch (Exception e) // sometimes error here idk why
			{
				Logger.Error($"error in character controller: {e}");
				throw;
			}
			return;
		}
		
		for (int i = 0; i <= 3; i++)
		{
			// get cardinal directions
			int neighborX = peekNode.X + Directions[i, 0];
			int neighborY = peekNode.Y + Directions[i, 1];

			if (neighborX != _character.TilePoint.X || neighborY != _character.TilePoint.Y) continue; // need to get neighbour 
			
			// this is to fix issues with sudden direction changes in path (should maybe try to make it so the bot goes in the middle of a tile as this is kind of a patch fix)
			switch (i)
			{
				case 0:
					_character.FacingDirection = (int)Direction.West; // west
					break;
				case 1: // east
					_character.FacingDirection = (int)Direction.East;
					break;
				case 2: // south
					_character.FacingDirection = (int)Direction.South;
					break;
				case 3: // north
					_character.FacingDirection = (int)Direction.North;
					break;
			}
			
			Logger.Info($"trying to use tool");
			_isDestroying = true;
			SwapItem(node.VectorLocation);
			BotBase.Farmer.BeginUsingTool();
			try // incase upper error also happens here
			{
				_character.MovePosition(time, Game1.viewport, _currentLocation);
				HandleWarp(_character.nextPosition(_character.getDirection()));
			}
			catch (Exception e)
			{
				Logger.Error($"error in character controller: {e}");
				throw;
			}
			return;
		}
	}

	private static void HandleWarp(Rectangle character)
	{
		Warp warp = _currentLocation.isCollidingWithWarp(character, _character);
		if (warp is null) return;
		
		if (Game1.eventUp)
		{
			Event currentEvent = Game1.CurrentEvent;
			if (!(!(currentEvent != null ? new bool?(currentEvent.isFestival) : null) ?? true))
			{
				Game1.CurrentEvent.TryStartEndFestivalDialogue(_character as Farmer);
			}
		}

		NPC npc = _character as NPC;
		if (npc is null) return;
		Game1.warpCharacter(_character as NPC, warp.TargetName, new Point(warp.TargetX, warp.TargetY));
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
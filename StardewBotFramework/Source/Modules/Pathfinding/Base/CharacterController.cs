using Microsoft.Xna.Framework;
using StardewBotFramework.Source.Modules.Pathfinding.Algorithms;
using StardewBotFramework.Source.ObjectDestruction;
using StardewBotFramework.Source.ObjectToolSwaps;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.Pathfinding;
using StardewValley.Tools;
using Logger = StardewBotFramework.Debug.Logger;
using Object = StardewValley.Object;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace StardewBotFramework.Source.Modules.Pathfinding.Base;

public class CharacterController : PathFindController
{
	/// <param name="pathToEndPoint">This is not used</param>
	/// <param name="c">This is not used</param>
	/// <param name="l">This is used to set location</param>
	public CharacterController(Stack<Point> pathToEndPoint, Character c, GameLocation l) : base(pathToEndPoint, c, l)
	{
		_currentLocation = l;
	}
	
	public enum FailureReason
	{
		GoalBlocked,
		NoCharacter,
		Paused
	}
	
	/// <summary>
	/// This is for when pathfinding gets cancelled from staying in the same place for too long.
	/// </summary>
	public static event EventHandler<FailureReason>? FailedPathFinding;

	/// <summary>
	/// These are values <see cref="Farmer.FacingDirection"/> uses in the format of an enum
	/// </summary>
	enum Direction
	{
		South = 0,
		North = 2,
		East = 1,
		West = 3,
	}
	
	private static bool MovingCharacter => _endPath.Count >= 1;
	private static bool _isDestroying;

	private static Stack<PathNode> _endPath = new();
	private static Farmer Character => BotBase.Farmer;
	private static GameLocation _currentLocation = null!;
	private static int _nextIndex;
	private static int _neighbourIndex;
	private static Character? _dynamicCharacter;
	private static Point _lastDynamicCharacterTile;
	private static PathNode _currentNode = null!;
	private static PathNode _nextNode = null!;

	private static bool _attacking;
	private static int _pausedTimer;
	/// <summary>
	/// The maximum amount of time the bot can stay in the same position for, this is in milliseconds.
	/// </summary>
	private static int MaxPauseTime => _attacking ? 30000 : 5000;

	private int _updateCallAmount;
	private int _moveCallAmount;
	// this gets called in farmer's update, return true if end else false
	public override bool update(GameTime time)
	{
		_updateCallAmount += 1;
		if (_recalculatingPath)
		{
			Logger.Info($"recalculating path: {_endPath.Count}");
			return false;
		}

		// Logger.Info($"update called  {_updateCallAmount}     {_moveCallAmount}");

		var monster = _dynamicCharacter as Monster;
		if ((!MovingCharacter && !_attacking) || (_attacking && monster is not null && monster.Health < 1))
		{
			Logger.Info($"return true as ended");
			ForceStopMoving();
			return true;
		}
	
		// stop issue with continuing pathfinding when going through warp
		if (!BotBase.CurrentLocation.Equals(_currentLocation))
		{
			Logger.Info($"location change return true");
			ForceStopMoving();
			return true;
		}
		
		// This is here as otherwise, it will get into a loop of fading to black and never warping. Might have issues with multiplayer?
		if (Game1.isWarping)
		{
			ForceStopMoving();
			return true;
		}
		
		Vector2 position = Character.Position;
		moveCharacter(time);
		if (position == Character.Position)
		{
			_pausedTimer += time.ElapsedGameTime.Milliseconds;
		}
		else
		{
			_pausedTimer = 0;
		}

		if (_pausedTimer < MaxPauseTime)
		{
			Logger.Info($"standard return false");
			return false;
		}
		
		Logger.Error($"Paused for too long");
		ForceStopMoving();
		FailedPathFinding?.Invoke(this, FailureReason.Paused);
		return true;
	}
	/// <summary>
	/// This is used to start the process of moving.
	/// </summary>
	public void StartMoveCharacter(Stack<PathNode> endPointPath, Character? dynamicCharacter = null,bool attacking = false)
	{
		Logger.Info($"start move character");
		if (IsMoving())
		{
			Logger.Error($"Not setting character as is already moving {MovingCharacter}  {_attacking}");
			return;
		}
	
		// stop pathfinder getting stuck as _movingCharacter wouldn't get set to false again
		if (!endPointPath.Any())
		{
			Logger.Error($"Not setting character controller as path is empty");
			return;
		}
		ForceStopMoving();
		Logger.Info($"setting");
		
		_endPath = endPointPath;
		_currentLocation = BotBase.CurrentLocation;
		_dynamicCharacter = dynamicCharacter;
		_attacking = attacking;
		Logger.Info($"post set");

		if (dynamicCharacter is not null) _lastDynamicCharacterTile = dynamicCharacter.TilePoint;

		Logger.Info($"set character");
		Character.controller = this;
		
		Logger.Info($"setting move");
		moveCharacter(Game1.currentGameTime);
	}
	
	private static bool _recalculatingPath;
	// different naming convention so we can override PathFindController's moveCharacter
	protected override void moveCharacter(GameTime time) // TODO: figure out why _character.movePosition does not change character's animation.
	{
		Logger.Info($"move character");
		if (_recalculatingPath) return;
		
		if (BotBase.Farmer.UsingTool) return; // check if animation running
		
		if (_isDestroying) // check if destroy
		{
			_isDestroying = false;
			_endPath.Peek().Destroy = false;
			
			// tool can destroy both neighbour and next, so we check here 
			Object objectInNextTile = _currentLocation.getObjectAtTile(_nextNode.X, _nextNode.Y);
			if (objectInNextTile is null || objectInNextTile.isPassable())
			{
				_endPath.ToArray()[_neighbourIndex].Destroy = false;
			}
		}

		if (_dynamicCharacter is not null && !_currentLocation.characters.Contains(_dynamicCharacter))
		{
			if (!_attacking) // if bot is attacking will also be removed if dies
			{
				FailedPathFinding?.Invoke(this,FailureReason.NoCharacter);
			}
			ForceStopMoving();
			return;
		}
		
		if (_dynamicCharacter is not null && _attacking &&
		    (Graph.IsInNeighbours(Character.TilePoint, _dynamicCharacter.TilePoint, out _)
		     || _dynamicCharacter.TilePoint == Character.TilePoint))
		{
			Logger.Info($"attacking in character controller");
			// BotBase.Farmer.FacingDirection = CorrectFacingDirections[direction];
			// this might stop character disappearing
			if (BotBase.Farmer.CurrentTool is not MeleeWeapon || BotBase.Farmer.CurrentTool.isScythe()) SwapItemHandler.SwapItem(typeof(MeleeWeapon), "Weapon");
			BotBase.Farmer.BeginUsingTool();
			return;
		}
		
		// recalculate path if character moves away from current path
		if (_dynamicCharacter is not null && _dynamicCharacter.TilePoint != _lastDynamicCharacterTile)
		{
			// stop issues with pathing, if in wall or other occupied tile, mainly for monsters. Can't rely on collision map as only the walls are blocked.
			if (_currentLocation.isCollidingPosition(_dynamicCharacter.GetBoundingBox(),Game1.viewport,_dynamicCharacter)) return;
			_lastDynamicCharacterTile = _dynamicCharacter.TilePoint;
			_endPath = RecalculatePath(new Goal.GoalDynamic(_dynamicCharacter, 1));
		}

		if (_endPath.Count < 1) return;
		
		PathNode node = _endPath.Peek();
		int tilesize = Game1.tileSize;
		Rectangle targetTile = new Rectangle(node.X * tilesize, node.Y * tilesize, tilesize, tilesize); // could probably change this if we want more precise pathfinding onto specific parts of tile
		targetTile.Inflate(-2, 0);
		Rectangle bbox = Character.GetBoundingBox();

		if ((targetTile.Contains(bbox) || (bbox.Width > targetTile.Width && targetTile.Contains(bbox.Center))) &&
		    targetTile.Bottom - bbox.Bottom >= 2)
		{
			Logger.Error($"remove from end path {node.VectorLocation}");
			_endPath.Pop();
			Character.stopWithoutChangingFrame();
			
			if (_endPath.Count == 0)
			{
				// OnGoalReached.Invoke();
				Logger.Error("Halt character");
				Character.Halt();
			}

			return;
		}

		Character.movementDirections.Clear();

		if (_currentLocation is not MovieTheater)
		{
			var characters = _currentLocation.characters.Where(npc => npc.GetBoundingBox()
				.Intersects(Character.GetBoundingBox())).ToList(); // probably don't need  && npc.isMoving()
			if (characters.Count > 0)
			{
				Logger.Warning($"Ran into character");
				foreach (var npc in characters)
				{
					AlgorithmBase.IPathing.collisionMap.AddBlockedTile(npc.TilePoint.X, npc.TilePoint.Y);
				}

				PathNode endNode = _endPath.ToList()[_endPath.Count - 1];
				_endPath = RecalculatePath(new Goal.GoalPosition(endNode.X, endNode.Y));
			}
		}
		
		if (bbox.Left < targetTile.Left && bbox.Right < targetTile.Right)
		{
			Character.SetMovingRight(true);
		}
		else if (bbox.Right > targetTile.Right && bbox.Left > targetTile.Left)
		{
			Character.SetMovingLeft(true);
		}
		else if (bbox.Top <= targetTile.Top)
		{
			Character.SetMovingDown(true);
		}
		else if (bbox.Bottom >= targetTile.Bottom - 2)
		{
			Character.SetMovingUp(true);
		}

		// destroy object
		foreach (var pathNode in _endPath)
		{
			_nextIndex = _endPath.ToList().IndexOf(pathNode);
			if (_nextIndex != 0) continue;
			
			int indexPlus = _nextIndex;
			if (_endPath.Count > 1) indexPlus += 1;
			_nextNode = _endPath.ToList()[indexPlus];
			_neighbourIndex = _endPath.ToList().IndexOf(_nextNode); // need this to set next PathNode.Destroy to false
		}
		
		// This is for interacting with specific objects and re calculating paths if an object is in the way
		Object? objectAtNextTile = _currentLocation.getObjectAtTile(_nextNode.X, _nextNode.Y);
		Object? objectInCurrentTile = _currentLocation.getObjectAtTile(node.X, node.Y);

		if (objectAtNextTile is Fence fence && fence.isGate.Value && !fence.isPassable())
		{
			fence.toggleGate(true);
		}

		if ((objectAtNextTile is not null && !objectAtNextTile.isPassable() 
		                                  && !DestroyLitterObject.IsDestructible(objectAtNextTile)) ||
		    (objectInCurrentTile is not null && !objectInCurrentTile.isPassable()))
		{
			if (objectAtNextTile != null && !objectAtNextTile.isPassable())
			{
				AlgorithmBase.IPathing.collisionMap.AddBlockedTile(_nextNode.X,_nextNode.Y);
			}

			if (objectInCurrentTile != null && !objectInCurrentTile.isPassable())
			{
				AlgorithmBase.IPathing.collisionMap.AddBlockedTile(node.X,node.Y);
			}
			
			PathNode endNode = _endPath.ToList()[_endPath.Count - 1];
			Logger.Info($"object in next tile was blocked   goal node: {endNode.VectorLocation}   next node: {_nextNode.VectorLocation}");
			var path = RecalculatePath(new Goal.GoalPosition(endNode.X,endNode.Y));
			
			if (path.Count < 1)
			{
				FailedPathFinding?.Invoke(this,FailureReason.GoalBlocked);
				ForceStopMoving();
				return;
			}
			
			_endPath = path;
			return;
		}
		
		PathNode peekNode = _endPath.Peek();
		if (!peekNode.Destroy || _isDestroying)
		{
			try
			{
				_moveCallAmount += 1;
				Character.MovePosition(time, Game1.viewport, _currentLocation);
				Character.updateMovementAnimation(time);
				HandleWarp(Character.nextPosition(Character.getFacingDirection()));
			}
			catch (Exception e) // sometimes error here IDK why might be because character controller is made on non-main thread
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

			if (neighborX != Character.TilePoint.X || neighborY != Character.TilePoint.Y) continue; // need to get neighbour 

			// this is to fix issues with sudden direction changes in path (should maybe try to make it so the bot goes in the middle of a tile as this is kind of a patch fix)
			switch (i)
			{
				case 0:
					Character.FacingDirection = (int)Direction.East;
					break;
				case 1:
					Character.FacingDirection = (int)Direction.West;
					break;
				case 2:
					Character.FacingDirection = (int)Direction.South;
					break;
				case 3:
					Character.FacingDirection = (int)Direction.North;
					break;
			}
			
			Logger.Info($"trying to use tool");
			_isDestroying = true;
			SwapItem(node.VectorLocation);
			BotBase.Farmer.BeginUsingTool();
			try // incase upper error also happens here
			{
				Character.MovePosition(time, Game1.viewport, _currentLocation);
				HandleWarp(Character.nextPosition(Character.getFacingDirection()));
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
		Warp warp = _currentLocation.isCollidingWithWarpOrDoor(character, Character);
		if (warp is null) return;
		
		if (Game1.eventUp)
		{
			Event currentEvent = Game1.CurrentEvent;
			if (currentEvent.isFestival)
			{
				Game1.CurrentEvent.TryStartEndFestivalDialogue(Character);
			}
		}

		Logger.Error($"Warping farmer");
		Character.warpFarmer(warp);
		ForceStopMoving();
	}

	private static Stack<PathNode> RecalculatePath(Goal goal)
	{
		if (_recalculatingPath) return new();
		_recalculatingPath = true;
		AlgorithmBase.IPathing pathing = new AStar.Pathing();
		pathing.BuildCollisionMap(_currentLocation, Character.TilePoint.X + 3, Character.TilePoint.Y + 3
			,Character.TilePoint.X - 3, Character.TilePoint.Y - 3);
		
		PathNode start = new PathNode(Character.TilePoint.X, Character.TilePoint.Y, null);
		var path = Task.Run(async () =>
		{
			var path = await pathing.FindPath(start, goal, _currentLocation, 10000);

			if (path.Count > 0) return path;
			
			Logger.Warning($"recalculated path was less than 0");
			return new();
		});
		
		path.Wait();
		_recalculatingPath = false;
		return path.Result;
	}
	
	public static bool IsMoving() => MovingCharacter || _attacking;

	public static void ForceStopMoving()
	{
		Character.controller = null;
		_endPath = new Stack<PathNode>();
		_currentNode = new PathNode(-1, -1, null);
		_nextNode = _currentNode;
		_lastDynamicCharacterTile = new Point();
		_dynamicCharacter = null;
		_nextIndex = -1;
		_neighbourIndex = -1;
		_attacking = false;
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
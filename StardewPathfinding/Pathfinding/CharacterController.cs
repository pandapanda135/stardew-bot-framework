using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;

namespace StardewPathfinding.Pathfinding;

public class CharacterController
{
	private static bool _movingCharacter = false;

	private static Stack<PathNode> _endPath = new();
	private static Character _character = new();
	private static GameLocation _currentLocation = new();
	private static GameTime _time = new();
	
	public static void Update(object? sender, UpdateTickingEventArgs args)
	{
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
		
		MoveCharacter(time);
	}
	
	private static void MoveCharacter(GameTime time)
	{
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
		}
		else if (bbox.Right > targetTile.Right && bbox.Left > targetTile.Left)
		{
			_character.SetMovingLeft(true);
		}
		else if (bbox.Top <= targetTile.Top)
		{
			_character.SetMovingDown(true);
		}
		else if (bbox.Bottom >= targetTile.Bottom - 2)
		{
			_character.SetMovingUp(true);
		}

		_character.MovePosition(time, Game1.viewport, _currentLocation);
	}
}
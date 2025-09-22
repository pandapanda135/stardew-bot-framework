using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using xTile.Dimensions;

namespace StardewBotFramework.Source.Modules;

public class ActionTiles
{
	/// <summary>
	/// Get all the actionable tiles in this location.
	/// </summary>
	/// <returns>Return <see cref="Point"/> of tiles that are actionable.</returns>
	public List<Point> GetActionableTiles()
	{
		int maxX = BotBase.CurrentLocation.Map.DisplayWidth / Game1.tileSize; 
		int maxY = BotBase.CurrentLocation.Map.DisplayHeight / Game1.tileSize;
		
		List<Point> tiles = new();
		for (int x = 0; x < maxX; x++)
		{
			for (int y = 0; y < maxY; y++)
			{
				if (BotBase.CurrentLocation.isActionableTile(x,y,BotBase.Farmer)) tiles.Add(new Point(x, y));
			}
		}

		return tiles;
	}
	
	/// <summary>
	/// Interact with an action tile.
	/// </summary>
	/// <param name="point">Tile to interact with as a <see cref="Point"/>.</param>
	/// <returns>Will return false if there is either no action tile at the provided tile else true.</returns>
	public bool DoActionTile(Point point)
	{
		if (!BotBase.CurrentLocation.isActionableTile(point.X, point.Y, BotBase.Farmer)) return false;
		return BotBase.CurrentLocation.checkAction(new Location(point.X, point.Y), Game1.viewport, BotBase.Farmer);
	}
}
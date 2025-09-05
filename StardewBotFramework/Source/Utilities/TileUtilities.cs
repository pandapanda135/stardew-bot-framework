using Microsoft.Xna.Framework;
using StardewValley;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace StardewBotFramework.Source.Utilities;

internal static class TileUtilities
{
	internal static Point TileToScreen(Vector2 tile)
	{
		return new Point((int)(tile.X * Game1.tileSize) - Game1.viewport.X, (int)(tile.Y * Game1.tileSize) - Game1.viewport.Y);
	} 
}
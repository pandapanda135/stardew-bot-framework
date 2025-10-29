using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewBotFramework.Source;
using StardewBotFramework.Source.Modules.Pathfinding.Base;
using StardewBotFramework.Source.Modules.Pathfinding.GroupTiles;
using StardewModdingAPI.Events;
using StardewValley;

namespace StardewBotFramework.Debug;

public class DebugDraw
{
    private static bool _textureInitialized;
    private static Texture2D? _outLineTexture;    
    
    private static void InitializeOutlineTextures()
    {
        if (_textureInitialized) return;
        
        // Tile outline texture setup            
        _outLineTexture = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
        _outLineTexture.SetData(new Color[] { Color.White });
        _textureInitialized = true;
    }

    /// <summary>
    /// Handles Rendering Tiles that are found by breadth first search
    /// </summary>
    public static void OnRenderTiles(object? sender, RenderedEventArgs e)
    {
        InitializeOutlineTextures();

        Vector2 tileLocation = new Vector2(Game1.tileSize, Game1.tileSize);
        foreach (ITile tile in StardewClient.DebugTiles.Keys)
        {
            Color tileColor = Color.Green;
        
            // convert from tiles to screen (see stardew wiki GameFundamentals Tiles)
            tileLocation.X = tile.X * Game1.tileSize;
            tileLocation.Y = tile.Y * Game1.tileSize;
        
            tileLocation.X -= Game1.viewport.X;
            tileLocation.Y -= Game1.viewport.Y;
            
            tileColor *= 0.25f;
            Game1.spriteBatch.Draw(_outLineTexture,
                new Rectangle((int)tileLocation.X, (int)tileLocation.Y, Game1.tileSize,
                    Game1.tileSize), tileColor);
        }
    }
    
    public static void OnRenderPathNode(object? sender, RenderedEventArgs e)
    {
        InitializeOutlineTextures();

        Vector2 tileLocation = new Vector2(Game1.tileSize, Game1.tileSize);
        foreach (PathNode tile in StardewClient.DebugNode.Keys)
        {
            Color tileColor = Color.Green;
        
            // convert from tiles to screen (see stardew wiki GameFundamentals Tiles)
            tileLocation.X = tile.X * Game1.tileSize;
            tileLocation.Y = tile.Y * Game1.tileSize;
        
            tileLocation.X -= Game1.viewport.X;
            tileLocation.Y -= Game1.viewport.Y;
            
            tileColor *= 0.25f;
            Game1.spriteBatch.Draw(_outLineTexture,
                new Rectangle((int)tileLocation.X, (int)tileLocation.Y, Game1.tileSize,
                    Game1.tileSize), tileColor);
        }
    }
    
    public static void RenderMap(object? sender, RenderedEventArgs e)
    {
        InitializeOutlineTextures();

        Vector2 tileLocation = new Vector2(Game1.tileSize, Game1.tileSize);
        foreach (var kvp in AlgorithmBase.IPathing.CollisionMap.BlockedTiles)
        {
            Color tileColor = Color.Red;
		
            // convert from tiles to screen (see stardew wiki GameFundamentals Tiles)
            tileLocation.X = kvp.Key.x * Game1.tileSize;
            tileLocation.Y = kvp.Key.y * Game1.tileSize;
    
            tileLocation.X -= Game1.viewport.X;
            tileLocation.Y -= Game1.viewport.Y;
        
            tileColor *= 0.25f;
            Game1.spriteBatch.Draw(_outLineTexture,
                new Rectangle((int)tileLocation.X, (int)tileLocation.Y, Game1.tileSize,
                    Game1.tileSize), tileColor);
        }
    }
}

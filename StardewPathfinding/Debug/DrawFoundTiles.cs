using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewPathfinding.Pathfinding;
using StardewValley;

namespace StardewPathfinding.Debug;

public class DrawFoundTiles
{
    private static Pathfinding.AlgorithmBase _algorithmBase = new(); 
    
    public static Queue<PathNode> debugDirectionTiles = new Queue<PathNode>();
    
    private static bool TextureInitialized;
    private static Texture2D OutLineTexture;    
    
    static public void InitializeOutlineTextures()
    {
        if (!TextureInitialized)
        {
            // Tile outline texture setup            
            OutLineTexture = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            OutLineTexture.SetData(new Color[] { Color.White });
            TextureInitialized = true;
        }
    }

    /// <summary>
    /// Handles Rendering Tiles that are found by breadth first search
    /// </summary>
    public static void OnRenderTiles(object? sender, RenderedEventArgs e)
    {
        InitializeOutlineTextures();

        Vector2 tileLocation = new Vector2(Game1.tileSize, Game1.tileSize);
        // this will be used later to show path to end point
        foreach (PathNode tile in Main.CorrectPath)
        {
            Color tileColor = Color.Green;
        
            // convert from tiles to screen (see stardew wiki GameFundamentals Tiles)
            tileLocation.X = (int)tile.X * Game1.tileSize;
            tileLocation.Y = (int)tile.Y * Game1.tileSize;
        
            tileLocation.X -= Game1.viewport.X;
            tileLocation.Y -= Game1.viewport.Y;
            
            tileColor *= 1f;
            Game1.spriteBatch.Draw(OutLineTexture,
                new Rectangle((int)tileLocation.X, (int)tileLocation.Y, Game1.tileSize,
                    Game1.tileSize), tileColor);
        }

        foreach (var tile in debugDirectionTiles)
        {
            Color tileColors = Color.Red;
            
            // convert from tiles to screen (see stardew wiki GameFundamentals Tiles)
            tileLocation.X = (int)tile.X * Game1.tileSize;
            tileLocation.Y = (int)tile.Y * Game1.tileSize;

            tileLocation.X -= Game1.viewport.X;
            tileLocation.Y -= Game1.viewport.Y;
            
            tileColors *= 0.1f;
            Game1.spriteBatch.Draw(OutLineTexture,
                new Rectangle((int)tileLocation.X, (int)tileLocation.Y, Game1.tileSize,
                    Game1.tileSize), tileColors);
        }
    }
}

using StardewValley.TerrainFeatures;

namespace StardewBotFramework.Source.Modules.Pathfinding.GroupTiles;

public class PlantTile : ITile
{
    /// <summary>
    /// The <see cref="HoeDirt"/> this plant is associated with.
    /// </summary>
    public TerrainFeature TerrainFeature;

    /// <summary>
    /// Where this plant has been watered already.
    /// </summary>
    public bool Watered;
    /// <summary>
    /// Whether the plant needs water.
    /// </summary>
    public bool NeedsWater;
    /// <summary>
    /// This is used for if it is next to a water tile as it will be watered automatically.
    /// </summary>
    public bool WaterTile;

    public PlantTile(TerrainFeature terrainFeature, bool watered, bool needsWater, bool waterTile)
    {
        TerrainFeature = terrainFeature;
        Watered = watered;
        NeedsWater = needsWater;
        WaterTile = waterTile;
    }
}
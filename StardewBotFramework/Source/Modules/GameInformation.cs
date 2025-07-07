using System.Collections;
using Netcode;
using StardewValley;
using StardewValley.Network;

namespace StardewBotFramework.Source.Modules;

/// <summary>
/// This is information about the current game that is mainly meant for development reasons
/// </summary>
public class GameInformation
{
    public int PlayersAmount => Game1.numberOfPlayers();
    public int MaxPlayers => Game1.Multiplayer.MaxPlayers;

    /// <summary>
    /// All other currently online <see cref="Farmer"/>s in the game, including the host.
    /// </summary>
    public IEnumerable<Farmer> Players => Game1.getOnlineFarmers();

    /// <summary>
    /// All other <see cref="Farmer"/>s including the host, online farmhands, and offline farmhands.
    /// </summary>
    public IEnumerable<Farmer> AllPlayers => Game1.getAllFarmers();
}
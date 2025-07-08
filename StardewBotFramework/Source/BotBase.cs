using StardewBotFramework.Debug;
using StardewBotFramework.Source.Events.GamePlayEvents;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Network;

namespace StardewBotFramework.Source;

public abstract class BotBase
{
    private static BotBase? Instance { get; set; }
    private readonly IModHelper _helper;
    private readonly IManifest _manifest;
    private readonly IMonitor _monitor;
    private readonly IMultiplayerHelper _multiplayer;
    internal static Farmer Farmer => Game1.player;
    internal static GameLocation CurrentLocation => Game1.currentLocation;
    internal IModHelper Helper => _helper;
    internal IMonitor Monitor => _monitor;
    internal IMultiplayerHelper Multiplayer => _multiplayer;
    
    private static IReflectedField<int>? _selectedResponse;
    private static IReflectedField<TextBox>? _reflectedTextBox;
    private static IReflectedField<NetMutexQueue<Guid>> _reflectedObjectDestroy;
    public GameEvents GameEvents;
    
    /// <summary>
    /// Change the selectedResponse value in <see cref="DialogueBox"/>, this will change the "Response" to a question dialogue.
    /// Must use Game1.ReceiveLeftClick after calling this for it to register.
    /// </summary>
    /// <param name="value">This should range from 0-3 as there is a max of four responses. Not 100% sure on that though so there is no check. If this is -1 no option will be picked</param>
    internal static void ChangeSelectedResponse(int value)
    {
        if (Instance is null)
        {
            Logger.Error($"Instance is not set");
            return;
        }
        
        _selectedResponse = Instance.Helper.Reflection.GetField<int>(Game1.activeClickableMenu, "selectedResponse", true);
        
        _selectedResponse.SetValue(value);   
    }

    internal static void CharacterCreatorTextBox(CharacterCustomization menu,string name, List<string> properties)
    {
        if (Instance is null)
        {
            Logger.Error($"Instance is not set");
            return;
        }
        
        _reflectedTextBox = Instance.Helper.Reflection.GetField<TextBox>(menu, properties[0], true);
        
        // IReflectedField<string> reflectedText = Instance.Helper.Reflection.GetField<string>(_reflectedTextBox, properties[1], true);

        TextBox textBox = _reflectedTextBox.GetValue();

        textBox.Text = name;
        
        _reflectedTextBox.SetValue(textBox);
        
        // reflectedText.SetValue(name);   
    }

    internal static void AddRemoveFurniture(GameLocation location,Guid f)
    {
        if (Instance is null)
        {
            Logger.Error($"Instance is not set");
            return;
        }
        
        _reflectedObjectDestroy = Instance.Helper.Reflection.GetField<NetMutexQueue<Guid>>(location,"furnitureToRemove",true);

        NetMutexQueue<Guid> value = _reflectedObjectDestroy.GetValue();
        
        if (!value.Contains(f))
        {
            value.Add(f);
        }
        
        _reflectedObjectDestroy.SetValue(value);
    }
}
using StardewBotFramework.Debug;
using StardewBotFramework.Source.Events.GamePlayEvents;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.GameData.Pets;
using StardewValley.Menus;
using StardewValley.Network;

namespace StardewBotFramework.Source;

public abstract class BotBase
{
    protected static BotBase? Instance { get; set; }
    protected IModHelper _helper;
    protected IManifest _manifest;
    protected IMonitor _monitor;
    protected IMultiplayerHelper _multiplayer;
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
        
        TextBox textBox = _reflectedTextBox.GetValue();

        textBox.Text = name;
        
        _reflectedTextBox.SetValue(textBox);
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

    internal static IDictionary<string,PetData> GetPetData()
    {
        if (Instance is null)
        {
            Logger.Error($"Instance is not set");
            return new Dictionary<string, PetData>();
        }
        return Instance.Helper.GameContent.Load<Dictionary<string,PetData>>("Data/Pets");
    }

    internal static int GetLevelUpMenuSkill()
    {
        if (Instance is null)
        {
            Logger.Error($"Instance is not set");
            return -1;
        }

        LevelUpMenu menu = Game1.activeClickableMenu as LevelUpMenu;
        IReflectedField<int> reflectedField = Instance.Helper.Reflection.GetField<int>(menu, "currentSkill");
        return reflectedField.GetValue();
    }
    
    internal static int GetLevelUpMenuLevel()
    {
        if (Instance is null)
        {
            Logger.Error($"Instance is not set");
            return -1;
        }

        LevelUpMenu menu = Game1.activeClickableMenu as LevelUpMenu;
        IReflectedField<int> reflectedField = Instance.Helper.Reflection.GetField<int>(menu, "currentLevel");
        return reflectedField.GetValue();
    }
    internal static List<int> GetLevelUpMenuProfessions()
    {
        if (Instance is null)
        {
            Logger.Error($"Instance is not set");
            return new();
        }

        LevelUpMenu menu = Game1.activeClickableMenu as LevelUpMenu;
        IReflectedField<List<int>> reflectedField = Instance.Helper.Reflection.GetField<List<int>>(menu, "professionsToChoose");
        return reflectedField.GetValue();
    }
}
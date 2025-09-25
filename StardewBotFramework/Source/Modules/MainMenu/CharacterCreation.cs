using Microsoft.Xna.Framework;
using Sickhead.Engine.Util;
using StardewBotFramework.Debug;
using StardewValley;
using StardewValley.Characters;
using StardewValley.GameData.Pets;
using StardewValley.Menus;

namespace StardewBotFramework.Source.Modules.MainMenu;

public class CharacterCreation
{
    private static CharacterCustomization? _characterCustomization;

    public List<ClickableTextureComponent>? FarmTypes = _characterCustomization?.farmTypeButtons;

    /// <summary>
    /// You can get character creation in the main menu using TitleMenu.subMenu as CharacterCustomization
    /// </summary>
    /// <param name="characterCustomization"></param>
    public void SetCreator(CharacterCustomization? characterCustomization)
    {
        _characterCustomization = characterCustomization;
    }

    public void ExitCharacterCreation()
    {
        if (_characterCustomization is null) return;
        if (_characterCustomization.source == CharacterCustomization.Source.Dresser || // find how to do this in these places
            _characterCustomization.source == CharacterCustomization.Source.Wizard ||
            _characterCustomization.source == CharacterCustomization.Source.ClothesDye)
        {
            Logger.Error($"Tried to run ExitCharacterCreation in a place that doesn't support it");
            return;
        }
        
        Rectangle buttonBounds = _characterCustomization.backButton.bounds; 
        _characterCustomization.receiveLeftClick(buttonBounds.X,buttonBounds.Y);
    }
    
    public void SkipIntro()
    {
        if ((_characterCustomization?.source == CharacterCustomization.Source.NewGame ||
             _characterCustomization?.source == CharacterCustomization.Source.HostNewFarm))
        {
            Rectangle buttonBounds = _characterCustomization.skipIntroButton.bounds;
            _characterCustomization.receiveLeftClick(_characterCustomization.skipIntroButton.bounds.X,_characterCustomization.skipIntroButton.bounds.Y);
            return;
        }
        else
        {
            Logger.Warning($"Tried to call SkipIntro when not starting new game or hosting new farm");
        }
        
    }

    public void StartGame()
    {
        if (_characterCustomization is null) return;
        Rectangle buttonBounds = _characterCustomization.okButton.bounds; 
        _characterCustomization.receiveLeftClick(buttonBounds.X,buttonBounds.Y);
    }

    public void RandomiseCharacter()
    {
        if (_characterCustomization?.source == CharacterCustomization.Source.DyePots ||
            _characterCustomization?.source == CharacterCustomization.Source.ClothesDye)
        {
            Logger.Warning($"Tried to call RandomiseCharacter when randomising character is not available. This happens when you call it in DyePots or ClothesDye");
        }
        
        Rectangle buttonBounds = _characterCustomization.randomButton.bounds; 
        _characterCustomization.receiveLeftClick(buttonBounds.X,buttonBounds.Y);
    }
    
    /// <summary>
    /// rotate the character preview
    /// </summary>
    /// <param name="goLeft">if true will do the equivalent of hitting the left arrow else the right</param>
    public void RotateCharacterPreview(bool goLeft)
    {
        ChangeArrow(goLeft,CharacterCustomization.region_directionLeft,CharacterCustomization.region_directionRight);
    }
    
    /// <summary>
    /// Change farmer's name.
    /// </summary>
    /// <param name="name">The name that the farmer will take. This will go through the normal filters</param>
    public void SetName(string name)
    {
        BotBase.ChangeTextBox(_characterCustomization,name,new (){"nameBox","Text"}); // _text
    }

    
    /// <summary>
    /// Change the farm name.
    /// </summary>
    /// <param name="name">The name that the farm name will take. This will go through the normal filters</param>
    public void SetFarmName(string name)
    {
        BotBase.ChangeTextBox(_characterCustomization,name,new (){"farmnameBox","_text"});
    }
    
    /// <summary>
    /// Change farmer's favourite thing.
    /// </summary>
    /// <param name="name">The favourite thing of the farmer. This will go through the normal filters</param>
    public void SetFavThing(string name)
    {
        BotBase.ChangeTextBox(_characterCustomization,name,new (){"favThingBox","_text"});
    }

    /// <summary>
    /// Change the character's gender, the two options provided here are either male or female
    /// </summary>
    /// <param name="male">If true the player's gender wil be male else female</param>
    public void ChangeGender(bool male)
    {
        if (male)
        {
            BotBase.Farmer.Gender = Gender.Male;
            return;
        }

        BotBase.Farmer.Gender = Gender.Female;
    }
    
    /// <summary>
    /// Change the character's skin colour
    /// </summary>
    /// <param name="change">change current value by this amount</param>
    public void ChangeSkinColour(int change)
    {
        BotBase.Farmer.changeSkinColor(BotBase.Farmer.skin.Value + change);
    }
    
    /// <summary>
    /// Change the character's hairstyle
    /// </summary>
    /// <param name="change">change current value by this amount</param>
    public void ChangeHair(int change)
    {
        Game1.player.changeHairStyle(Game1.player.hair.Value + change);
    }
    
    /// <summary>
    /// Change the character's accessory
    /// </summary>
    /// <param name="change">change current value by this amount</param>
    public void ChangeAccessory(int change)
    {
        Game1.player.changeAccessory(Game1.player.accessory.Value + change);
    }

    /// <summary>
    /// Returns the pet date from <c>'Data/Pets'</c> 
    /// </summary>
    public IDictionary<string, PetData> GetPetData()
    {
        return BotBase.GetPetData();
    } 
    
    /// <summary>
    /// This will change the pet Type.
    /// </summary>
    /// <param name="petType">This will change the selected pet type, these can only be "Cat" or "Dog" in vanilla however there is no check in the function for this</param>
    public bool ChangePetType(string petType)
    {
        IDictionary<string, PetData> petTypes = BotBase.GetPetData();

        if (petTypes[petType].Breeds.Count(breed => breed.CanBeChosenAtStart) < 1) return false;
        
        BotBase.Farmer.whichPetType = petType;
        return true;
    }

    /// <summary>
    /// This will change the pet breed, this is a subset of a pet type. You must set pet type before changing this.
    /// </summary>
    /// <param name="breed">This will change the selected pet breed, these can only be "0" to "4" in vanilla however there is no check in the function for this</param>
    /// <returns></returns>
    public bool ChangePetBreed(string breed)
    {
        IDictionary<string, PetData> petTypes = BotBase.GetPetData();

        if (!petTypes[BotBase.Farmer.whichPetType].Breeds[int.Parse(breed)].CanBeChosenAtStart) return false;
        
        BotBase.Farmer.whichPetBreed = breed;
        return true;
    }

    /// <summary>
    /// Change farm types
    /// </summary>
    /// <param name="option">this will be the index of farm type you want from <see cref="FarmTypes"/></param>
    public void ChangeFarmTypes(int option)
    {
        ClickableTextureComponent button = _characterCustomization.farmTypeButtons[option];
        
        _characterCustomization.receiveLeftClick(button.bounds.X,button.bounds.Y);
    }
    
    private void ChangeArrow(bool goLeft,int regionLeft, int regionRight)
    {
        List<ClickableComponent> selectionButtons;
        int region;
        if (goLeft)
        {
            region = regionLeft;
            selectionButtons = _characterCustomization.leftSelectionButtons;
        }
        else
        {
            region = regionRight;
            selectionButtons = _characterCustomization.rightSelectionButtons;
        }
        
        foreach (var button in selectionButtons)
        {
            if (button.region == region)
            {
                _characterCustomization.receiveLeftClick(button.bounds.X,button.bounds.Y);                
            }
        }
    }

    /// <summary>
    /// Change colour selected by sliders.
    /// </summary>
    /// <param name="option">these correspond to the slider for eye = 0,hair = 1 and pants = 2.</param>
    /// <param name="hue">This is the slider that is of all the colours.</param>
    /// <param name="saturation">This is the slider that goes from white to the full colour.</param>
    /// <param name="brightness">this is the slider that goes from black from the full colour.</param>
    public void ChangeColour(int option,int hue, int saturation, int brightness)
    {
        ChangeBarColour(option,hue,saturation,brightness);
    }

    private void ChangeBarColour(int option,int hue, int saturation, int brightness)
    {
        List<ColorPicker> colorPickers = new()
        {
            _characterCustomization.eyeColorPicker, _characterCustomization.hairColorPicker,
            _characterCustomization.pantsColorPicker
        };

        colorPickers[option].hueBar.value = hue;
        colorPickers[option].saturationBar.value = saturation;
        colorPickers[option].valueBar.value = brightness;
    }
    
    public Dictionary<int, string> GetPossibleShirts()
    {
        Dictionary<int, string> shirts = new();
        if (_characterCustomization is null)
        {
            Logger.Error($"you have not set character creation yet");
            return new();
        }
        foreach (var shirtId in _characterCustomization.GetValidShirtIds())
        {
            Item shirt = ItemRegistry.Create($"(S){shirtId}");
            shirts.Add(Int32.Parse(shirtId),shirt.Name);
        }

        return shirts;
    }
    
    /// <summary>
    /// Change the character's selected shirt
    /// </summary>
    /// <param name="change">change current value by this amount</param>
    public void ChangeShirt(int change)
    {
        // increase is positive will wrap around if negative
        Game1.player.rotateShirt(change, _characterCustomization?.GetValidShirtIds());
        Game1.playSound("coin");
    }
    
    /// <summary>
    /// Change the character's selected pants
    /// </summary>
    /// <param name="change">change current value by this amount</param>
    public void ChangePants(int change)
    {
        Game1.player.rotatePantStyle(change, _characterCustomization?.GetValidPantsIds());
        Game1.playSound("coin");
    }
    
    public Dictionary<int, string> GetPossiblePants()
    {
        Dictionary<int, string> pants = new();
        foreach (var pantsId in _characterCustomization.GetValidPantsIds())
        {
            Logger.Info($"shirtID: {pantsId}");
            Item pant = ItemRegistry.Create($"(S){pantsId}");
            pants.Add(Int32.Parse(pantsId),pant.Name);
        }

        return pants;
    }
}
using Microsoft.Xna.Framework;
using StardewBotFramework.Debug;
using StardewValley;
using StardewValley.Menus;

namespace StardewBotFramework.Source.Modules;

public class CharacterCreation
{
    private static CharacterCustomization _characterCustomization;

    public void SetCreator(CharacterCustomization characterCustomization)
    {
        _characterCustomization = characterCustomization;
    }

    public void ExitCharacterCreation(bool mainMenu = false)
    {
        if (mainMenu)
        {
            Rectangle buttonBounds = _characterCustomization.backButton.bounds; 
            _characterCustomization.receiveLeftClick(buttonBounds.X,buttonBounds.Y);
        }
    }
    
    public void SkipIntro()
    {
        if ((_characterCustomization.source == CharacterCustomization.Source.NewGame ||
             _characterCustomization.source == CharacterCustomization.Source.HostNewFarm))
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
        Rectangle buttonBounds = _characterCustomization.okButton.bounds; 
        _characterCustomization.receiveLeftClick(buttonBounds.X,buttonBounds.Y);
    }

    public void RandomiseCharacter()
    {
        if (_characterCustomization.source == CharacterCustomization.Source.DyePots ||
            _characterCustomization.source == CharacterCustomization.Source.ClothesDye)
        {
            Logger.Warning($"Tried to call RandomiseCharacter when randomising character is not available. This happens when you call it in DyePots or ClothesDye");
        }
        
        Rectangle buttonBounds = _characterCustomization.randomButton.bounds; 
        _characterCustomization.receiveLeftClick(buttonBounds.X,buttonBounds.Y);
    }

    public void RotateCharacterPreview(bool goLeft)
    {
        ChangeArrow(goLeft,CharacterCustomization.region_directionLeft,CharacterCustomization.region_directionRight);
    }
    
    public void SetName(string name)
    {
        StardewClient.CharacterCreatorTextBox(name,new (){"nameBox","_text"});
    }

    public void SetFarmName(string name)
    {
        StardewClient.CharacterCreatorTextBox(name,new (){"farmnameBox","_text"});
    }
    
    public void SetFavThing(string name)
    {
        StardewClient.CharacterCreatorTextBox(name,new (){"favThingBox","_text"});
    }

    public void ChangeGender(bool male)
    {
        List<ClickableComponent> genderButtons = _characterCustomization.genderButtons;
        int region = CharacterCustomization.region_female;

        if (male) region = CharacterCustomization.region_male;
        foreach (var button in genderButtons)
        {
            if (button.region == region)
            {
                _characterCustomization.receiveLeftClick(button.bounds.X,button.bounds.Y);
            }
        }
    }
    
    public string ChangeSkin(bool goLeft)
    {
        ChangeArrow(goLeft,CharacterCustomization.region_skinLeft,CharacterCustomization.region_skinRight);
        
        return StardewClient.Farmer.skin.Name;
    }

    public string ChangeHair(bool goLeft)
    {
        ChangeArrow(goLeft,CharacterCustomization.region_hairLeft,CharacterCustomization.region_hairRight);
        
        return StardewClient.Farmer.hair.Name;
    }

    public string ChangeAccessory(bool goLeft)
    {
        ChangeArrow(goLeft,CharacterCustomization.region_accLeft,CharacterCustomization.region_accRight);
        
        return StardewClient.Farmer.accessory.Name;
    }
    
    
    public string ChangePet(bool goLeft)
    {
        ChangeArrow(goLeft,CharacterCustomization.region_accLeft,CharacterCustomization.region_accRight);
        
        return StardewClient.Farmer.whichPetType;
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
        foreach (var shirtId in _characterCustomization.GetValidShirtIds())
        {
            Logger.Info($"shirtID: {shirtId}");
            Item shirt = ItemRegistry.Create($"(S){shirtId}");
            shirts.Add(Int32.Parse(shirtId),shirt.Name);
        }

        return shirts;
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
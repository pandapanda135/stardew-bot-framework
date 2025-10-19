using Microsoft.Xna.Framework;
using StardewBotFramework.Debug;
using StardewBotFramework.Source.Events.GamePlayEvents;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace StardewBotFramework.Source.Utilities;

public abstract class MenuHandler
{
	protected MenuHandler()
	{
		if (GameEvents._helper is null) return;
		GameEvents._helper.Events.GameLoop.UpdateTicking += Update;
	}
	
	/// <summary>
	/// This is to be used as a backing field for module's own Menu property
	/// </summary>
	protected IClickableMenu? _menu;

	/// <summary>
	/// This is for any child menus opened up
	/// </summary>
	protected IClickableMenu? _childMenu;
	
	/// <summary>
	/// Set the currently stored menu, you should only use this if the module does not have its own set menu method.
	/// </summary>
	/// <param name="menu"></param>
	public void SetStoredMenu(IClickableMenu menu) => _menu = menu;
	/// <summary>
	/// Remove the ui stored in this class
	/// </summary>
	public void RemoveStoredMenu() => _menu = null;
	/// <summary>
	/// Remove the stored and currently displayed ui
	/// </summary>
	public void RemoveMenu()
	{
		_menu?.exitThisMenu();
		// upper should work but just doing a sanity check
		if (Game1.activeClickableMenu != null)
		{
			Game1.activeClickableMenu = null;
		}
		
		_menu = null;
	}

	public void RemoveChildMenu()
	{
		_childMenu?.exitThisMenu();
		_childMenu = null;
	}

	private Point _hoverPoint;
	private float _desiredTime = -1;
	private float _timer = -1;

	private void Update(object? sender, UpdateTickingEventArgs e)
	{
		if (_desiredTime < 0 || _hoverPoint == Point.Zero) return;

		_timer += Game1.currentGameTime.ElapsedGameTime.Milliseconds;

		if (_timer >= _desiredTime)
		{
			_desiredTime = -1;
			_timer = -1;
			_hoverPoint = Point.Zero;
			return;
		}
		
		// TODO: I don't think this works due to both cursor and this being counted, might be wrong though.
		_menu?.performHoverAction(_hoverPoint.X,_hoverPoint.X);
	}

	public void Hover(int x, int y, float seconds)
	{
		_hoverPoint = new Point(x,y);
		_menu?.performHoverAction(x,y);
		_desiredTime = seconds * 1000;
	}

	public void Hover(ClickableComponent cc,float seconds)
	{
		Hover(cc.bounds.X,cc.bounds.Y,seconds);
	}

	public void LeftClick(int x, int y, bool sound = false)
	{
		_menu?.receiveLeftClick(x,y);
	}
	
	public void LeftClick(ClickableComponent cc, bool sound = false)
	{
		LeftClick(cc.bounds.X,cc.bounds.Y);
	}

	public void RightClick(int x, int y, bool sound = false)
	{
		_menu?.receiveRightClick(x,y);
	}

	public void RightClick(ClickableComponent cc, bool sound = false)
	{
		RightClick(cc.bounds.X,cc.bounds.Y,sound);
	}
}
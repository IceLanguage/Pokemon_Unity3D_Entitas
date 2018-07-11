#pragma strict



//#if UNITY_EDITOR
//import UnityEditor;
//#endif


@script AddComponentMenu("ControlFreak-Demos-JS/DemoSwipeMenuJS")

public class DemoSwipeMenuJS extends MonoBehaviour
	{
	public var	ctrl 	: TouchController;
	public var 	menu	: SwipeMenuJS;
	public var	guiSkin	: GUISkin;

	//private var	itemWidth	: float;
	
	public var	imgFpp			: Texture2D;
	public var	imgRpg			: Texture2D;
	public var	imgMap			: Texture2D;		
	public var	imgDualStick	: Texture2D;

	public var 	fadeInDuration	: float	= 1;
	private var fadeInTimer		: AnimTimer;


	private static var	MENU_SCENE_NAME			: String	= "CF-Demo-JS-Menu";
	private static var	FPP_DEMO_SCENE_NAME		: String	= "CF-Demo-JS-FPP";
	private static var	RPG_DEMO_SCENE_NAME 	: String	= "CF-Demo-JS-RPG";
	private static var	MAP_DEMO_SCENE_NAME 	: String 	= "CF-Demo-JS-Map";
	private static var	DSS_DEMO_SCENE_NAME		: String	= "CF-Demo-JS-Dual-Stick-Shooter";
	private static var	EXIT_SCREEN_SCENE_NAME	: String 	= "CF-Demo-Exit-Screen";
	
	public static var ITEM_DSS		: int	= 0;
	public static var ITEM_FPP		: int	= 1;
	public static var ITEM_RPG		: int	= 2;
	public static var ITEM_MAP		: int	= 3;
	public static var ITEM_COUNT	: int	= 4;
	
	
	
	// --------------
	static public function LoadMenuScene() : void
		{
		Application.LoadLevel(MENU_SCENE_NAME);
		}


	// ------------------
	function Start() : void
		{
		// Init swipe menu ...

		//this.itemWidth = Screen.width * 0.9f;
		this.menu.Init(ITEM_COUNT, 0, Screen.width, this.ctrl.GetDPI(), false);

		// Init fade-in timer...

		this.fadeInTimer.Start(this.fadeInDuration);
		}
	


	// ---------------
	private function StartDemo(sceneName : String) : void
		{
#if UNITY_EDITOR
		if (!EditorApplication.isPlaying)
			return;
#endif		

		Application.LoadLevel(sceneName);
		}


	// ---------------
	function Update() : void	
		{
		// If controller's layout changed, reset menu's width...

		if ((this.ctrl != null) && this.ctrl.LayoutChanged())
			{
			this.menu.SetWindowSize(Screen.width, this.ctrl.GetDPI());
			}


		// Fade-in phase...

		if (this.fadeInTimer.Enabled)
			{
			this.fadeInTimer.Update(Time.deltaTime);
			if (this.fadeInTimer.Completed)
				this.fadeInTimer.Disable();
			}

		// Control...
		else
			{
			if (Input.GetKeyUp(KeyCode.Escape))
				{
				Application.LoadLevel(EXIT_SCREEN_SCENE_NAME);
				return;
				//Application.Quit();
				}
	
			// Get first and the only one touch zone (no need for IDs)...
	
			var zone : TouchZone = this.ctrl.GetZone(0);
	
	
			// Handle tap...
	
			if (zone.JustTapped())	
				{
				this.menu.OnTap();
				}
			
			// Handle unified-touch press...
	
			if (zone.JustUniPressed(false, false))
				this.menu.OnPress();
	
			// 	If unified-touch moved, use it's drag delta...
		
			if (zone.UniDragged())
				this.menu.Move(-zone.GetUniDragDelta(TouchCoordSys.SCREEN_PX, false).x);
	
	
			// On unfied-touch release, get released drag velocity to detect those quick swipes...
	
			else if (zone.JustUniReleased(true, true))
				{
				this.menu.OnRelease(-zone.GetReleasedUniDragVel(TouchCoordSys.SCREEN_PX, false).x);		
				}
			}


		// If swipe-menu completed - go to selected mode...

		this.menu.UpdateMenu();
		if (this.menu.JustCompleted())
			{
			switch (this.menu.GetCurItem())
				{
				case ITEM_FPP : 
					this.StartDemo(FPP_DEMO_SCENE_NAME);
					break;
	
				case ITEM_RPG : 
					this.StartDemo(RPG_DEMO_SCENE_NAME);
					break;

				case ITEM_DSS : 
					this.StartDemo(DSS_DEMO_SCENE_NAME);
					break;
		
				case ITEM_MAP : 
					this.StartDemo(MAP_DEMO_SCENE_NAME);
					break;
				}

			}

		}


	// ---------------
	function OnGUI() : void	
		{
		GUI.skin = this.guiSkin;

		GUI.color = (this.fadeInTimer.Enabled ? 
			new Color(1,1,1, this.fadeInTimer.Nt) : Color.white);
	

		var topY : float = (this.fadeInTimer.Enabled ? 
			Mathf.Lerp(Screen.height, 0, this.fadeInTimer.Nt) : 0);

		var startX : float = (Screen.width * 0.5f) - (this.menu.windowSize * 0.5f) + 
			-this.menu.displayPos;
		
		var windowRect : Rect = new Rect(startX, topY, this.menu.windowSize, Screen.height * 0.66f); 
		for (var i : int = 0; i < ITEM_COUNT; ++i)
			{
			var img 		: Texture2D = null;
			var descText 	: String 	= "";

			switch (i)
				{
				case ITEM_FPP : 
					img = this.imgFpp; descText = FPP_DEMO_DESCRIPTION; break;
				case ITEM_RPG : 
					img = this.imgRpg; descText = RPG_DEMO_DESCRIPTION; break;
				case ITEM_MAP : 
					img = this.imgMap; descText = MAP_DEMO_DESCRIPTION; break;
				case ITEM_DSS : 
					img = this.imgDualStick; descText = DSS_DEMO_DESCRIPTION; break;
				}


			var buttonScale : float = 1.0f;
			
			

			if (( this.menu.Selected()) && (i == this.menu.curItem))
				{
				buttonScale = 1.0f - Mathf.Clamp01(this.menu.GetTimeSinceSelection() / this.menu.completionDuration);
				buttonScale *= buttonScale;
				}
			else
				{
				}

			var buttonRect : Rect = windowRect;
			buttonRect.width *= buttonScale;
			buttonRect.height *= buttonScale;
			buttonRect.x = windowRect.center.x - buttonRect.width * 0.5f;
			buttonRect.y = windowRect.center.y - buttonRect.height * 0.5f;

			GUI.DrawTexture(buttonRect, img, ScaleMode.ScaleToFit);

			GUI.Box(new Rect(windowRect.x, topY + (Screen.height * 0.65f), //y + (0.6f * windowRect.height), 
				windowRect.width,  Screen.height * 0.35f), //windowRect.height * 0.35f), 
				descText);

			windowRect.x += this.menu.windowSize;
			} 
		}
	


	// --------------------
	private static var	FPP_DEMO_DESCRIPTION : String	= 
		"FPP DEMO\nDynamic Stick, tapping, dragging, etc.";

	private static var 	RPG_DEMO_DESCRIPTION : String =
		"RPG DEMO\nDynamic Stick, Tap, Pinch, Canceling";

	private static var MAP_DEMO_DESCRIPTION	: String = 
		"MAP DEMO\nTwist, Pinch, Drag, Double Tap, Multi-finger Tap";

	private static var DSS_DEMO_DESCRIPTION	: String = 
		"DUAL STICK SHOOTER DEMO\nSimple Dual Sticks and Pause Button.";

	}

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


[AddComponentMenu("ControlFreak-Demos-CS/DemoSwipeMenuCS")]
public class DemoSwipeMenuCS : MonoBehaviour
	{
	public TouchController	ctrl;
	public SwipeMenuCS 		menu;
	public GUISkin			guiSkin;
	

	public float 			fadeInDuration	= 1;
	private AnimTimer		fadeInTimer;


	//private float			itemWidth;
	

	public Texture2D
		imgFpp,
		imgRpg,
		imgMap,
		imgDualStick;		
	

	private const string 
		MENU_SCENE_NAME				= "CF-Demo-CS-Menu",
		FPP_DEMO_SCENE_NAME			= "CF-Demo-CS-FPP",
		RPG_DEMO_SCENE_NAME 		= "CF-Demo-CS-RPG",
		MAP_DEMO_SCENE_NAME 		= "CF-Demo-CS-Map",
		DUAL_STICK_DEMO_SCENE_NAME 	= "CF-Demo-CS-Dual-Stick-Shooter",
		EXIT_SCREEN_SCENE_NAME 		= "CF-Demo-Exit-Screen";

	public const int ITEM_DSS 		= 0;
	public const int ITEM_FPP 		= 1;
	public const int ITEM_RPG 		= 2;
	public const int ITEM_MAP 		= 3;
	public const int ITEM_COUNT 	= 4;

	
	
	
	// --------------
	static public void LoadMenuScene()
		{
		Application.LoadLevel(MENU_SCENE_NAME);
		}


	// ------------------
	private void Start()
		{
		//this.itemWidth = Screen.width * 0.9f;
		this.menu.Init(ITEM_COUNT, 0, Screen.width /*this.itemWidth*/, 
			this.ctrl.GetDPI(), false);

		this.fadeInTimer.Start(this.fadeInDuration);
		}
	


	// ---------------
	private void StartDemo(string sceneName)
		{
#if UNITY_EDITOR
		if (!EditorApplication.isPlaying)
			return;
#endif		

		Application.LoadLevel(sceneName);
		}


	// ---------------
	private void Update()	
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
	
			TouchZone zone = this.ctrl.GetZone(0);
	
	
			// Handle tap...
	
			if (zone.JustTapped())	
				{
				this.menu.OnTap();
				}
			
			// Handle unified-touch press...
	
			if (zone.JustUniPressed())
				this.menu.OnPress();
	
			// 	If unified-touch moved, use it's drag delta...
		
			if (zone.UniDragged())
				this.menu.Move(-zone.GetUniDragDelta(TouchCoordSys.SCREEN_PX).x);
	
	
			// On unfied-touch release, get released drag velocity to detect those quick swipes...
	
			else if (zone.JustUniReleased())
				{
				this.menu.OnRelease(-zone.GetReleasedUniDragVel().x);		
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
					return;
	
				case ITEM_RPG :
					this.StartDemo(RPG_DEMO_SCENE_NAME);
					return;
		
				case ITEM_MAP :
					this.StartDemo(MAP_DEMO_SCENE_NAME);
					return;

				case ITEM_DSS :
					this.StartDemo(DUAL_STICK_DEMO_SCENE_NAME);
					return;
				}

			}

		}


	// ---------------
	private void OnGUI()	
		{
		GUI.skin = this.guiSkin;
	
		GUI.color = (this.fadeInTimer.Enabled ? 
			new Color(1,1,1, this.fadeInTimer.Nt) : Color.white);
	

		float topY = (this.fadeInTimer.Enabled ? 
			Mathf.Lerp(Screen.height, 0, this.fadeInTimer.Nt) : 0);

		float startX = (Screen.width * 0.5f) - (this.menu.windowSize * 0.5f) + 
			-this.menu.displayPos;
		
		Rect windowRect = new Rect(startX, topY, this.menu.windowSize, Screen.height * 0.66f); 
		for (int i = 0; i < ITEM_COUNT; ++i)
			{
			Texture2D img = null;
			string descText = "";

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


			float buttonScale = 1.0f;
			
			

			if (( this.menu.Selected()) && (i == this.menu.curItem))
				{
				buttonScale = 1.0f - Mathf.Clamp01(this.menu.GetTimeSinceSelection() / this.menu.completionDuration);
				buttonScale *= buttonScale;
				}
			else
				{
				//GUI.color = Color.white;
				}

			Rect buttonRect = windowRect;
			buttonRect.width *= buttonScale;
			buttonRect.height *= buttonScale;
			buttonRect.x = windowRect.center.x - buttonRect.width * 0.5f;
			buttonRect.y = windowRect.center.y - buttonRect.height * 0.5f;

			GUI.DrawTexture(buttonRect, img, ScaleMode.ScaleToFit);

			GUI.Box(new Rect(windowRect.x, topY + (Screen.height * 0.65f), //y + (0.6f * windowRect.height), 
				windowRect.width,  Screen.height * 0.35f), //windowRect.height * 0.35f), 
				descText);

			windowRect.x += this.menu.windowSize; //this.itemWidth;
			} 
		}
	


	// --------------------
	private const string	FPP_DEMO_DESCRIPTION	= 
		"FPP DEMO\nDynamic Stick, tapping, dragging, etc.";

	private const string	RPG_DEMO_DESCRIPTION	= 
		"RPG DEMO\nDynamic Stick, Tap, Pinch, Canceling";

	private const string	MAP_DEMO_DESCRIPTION	= 
		"MAP DEMO\nTwist, Pinch, Drag, Double Tap, Multi-finger Tap";

	private const string	DSS_DEMO_DESCRIPTION	= 
		"DUAL STICK SHOOTER DEMO\nSimple Dual Sticks and Pause Button.";

	}
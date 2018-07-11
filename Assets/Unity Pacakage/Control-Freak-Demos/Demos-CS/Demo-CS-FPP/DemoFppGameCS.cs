// --------------------------
// FPP Demo -----------------
// Dan's Game Tools 2013 ----
// --------------------------


using UnityEngine;

//[ExecuteInEditMode()]
[AddComponentMenu("ControlFreak-Demos-CS/DemoFppGameCS")]
public class DemoFppGameCS : MonoBehaviour 
	{
	public GUISkin			guiSkin;

	
	public TouchController  touchCtrl;
	public DemoFppCharaCS	chara;
	
	public PopupBoxCS		popupBox;


	// ------------------
	// Controller's constants.
	// ------------------

	public const int STICK_WALK		= 0;
	public const int ZONE_FIRE		= 0;
	public const int ZONE_ZOOM		= 1;
	public const int ZONE_RELOAD	= 2;
	public const int ZONE_AIM		= 3;
	public const int ZONE_PAUSE		= 4;




	private bool		pauseMenuOn;
	private	AnimTimer	pauseMenuTimer;
	private bool		pauseMenuFadeIn;
	
	public float		pauseMenuAnimDuration = 0.3f;

	private TouchController.AnimFloat	pauseMenuAnim;
	private Rect		pauseMenuRect;

	



	// ------------------
	private void Start()
		{
		if (this.touchCtrl != null)
			{
			// Manually init the controller.

			this.touchCtrl.InitController();
			
			// Hide the controller (0 = reset mode).

			this.touchCtrl.HideController(0);

			// Start two seconds long fade-out...

			this.touchCtrl.ShowController(2);
			}
		
		if (this.chara != null)
			{
			this.chara.SetTouchController(this.touchCtrl);
			}
		}



	// ------------------
	private void Update()	
		{
		if (this.touchCtrl != null)
			{
			// Manually poll the controller...

			this.touchCtrl.PollController();

			// If controller's layout changed...
			
			if (this.touchCtrl.LayoutChanged())
				{
				// Set custom position to the Pause button...
	
				this.touchCtrl.GetZone(ZONE_PAUSE).SetRect(new Rect(5,5, 32,32));
				
				// Update camera's viewport to support screen-emulation.
				// This code can be excluded from release build...

				if ((this.chara != null) && (this.chara.playerViewCam != null))
					{
					Rect r =  this.touchCtrl.GetScreenEmuRect(true);

					this.chara.playerViewCam.pixelRect=  r;
					}

				// Indicate layout change handling.

				this.touchCtrl.LayoutChangeHandled();
				}
			}
		

		// Manually update the controller...

		if (this.touchCtrl != null)
			this.touchCtrl.UpdateController();

		
		// Pause menu active...

		if (this.IsPaused())
			{
			this.UpdatePauseMenu();
			}

	
		// Gameplay active...

		else
			{
			if (this.popupBox != null)
				{
				if (!this.popupBox.IsVisible())
					{
					if (Input.GetKeyDown(KeyCode.Space))
						{
						this.popupBox.Show(INSTRUCTIONS_TITLE, INSTRUCTIONS_TEXT, 
							INSTRUCTIONS_BUTTON_TEXT);
						}
					}
				else
					{
					if (Input.GetKeyDown(KeyCode.Space))
						this.popupBox.Hide();
					}
				}	


			if (this.chara != null)
				{
				this.chara.UpdateChara();
				}
			
			
			if (Input.GetKeyUp(KeyCode.Escape))
				{
				DemoSwipeMenuCS.LoadMenuScene();
				//DemoMenu.LoadMenuLevel();
				return;
				}
			
			// If the pause button just have been released (including mid-frame press)
			// show the Pause Menu...
			// Here you can see a method call on a returned zone object (in default safe-mode).

			if (this.touchCtrl.GetZone(ZONE_PAUSE).JustUniReleased(true, false)) 
				this.StartPauseMenu();
			}

		}

	// -------------
	private void OnGUI()
		{
		if (this.chara != null)
			this.chara.DrawGUIBG();

		GUI.skin = this.guiSkin;

		// Manually render controller's GUI...

		if (this.touchCtrl != null)
			{
			this.touchCtrl.DrawControllerGUI();
			}

		// Render character's GUI...

		if (this.chara != null)
			this.chara.DrawCustomGUI();

		// Draw pause menu...

		if (this.IsPaused())
			this.DrawPauseMenu();
		

		// Draw pop-up box..
	
		if ((this.popupBox != null) && this.popupBox.IsVisible())
			this.popupBox.DrawGUI();
		else
			{
			GUI.color = Color.white;
			GUI.Label(new Rect(40, 10, Screen.width - 100, 100),
				"FPP Demo - Press [Space] for help, [Esc] to quit.");
			}

		}



	// ------------------
	public void StartPauseMenu()
		{
		this.chara.OnPauseStart();


		this.pauseMenuOn = true;
		this.pauseMenuTimer.Start(this.pauseMenuAnimDuration);
		this.pauseMenuFadeIn = true;


		this.pauseMenuRect = new Rect(
			0.05f * Screen.width, 
			0.05f * Screen.height,
			0.90f * Screen.width,
			0.90f * Screen.height);


		// Slide from the top of the screen....

		this.pauseMenuAnim.Reset(-Screen.height);
		this.pauseMenuAnim.MoveTo(this.pauseMenuRect.y);

		
		// Disable controller when paused...

		this.touchCtrl.DisableController();
		//this.touchCtrl.ReleaseTouches();

		
		}

	// ---------------
	public void ExitPauseMenu()
		{
		this.pauseMenuTimer.Start(this.pauseMenuAnimDuration);
		this.pauseMenuFadeIn = false;
		
		// Slide to the right...
 
		this.pauseMenuAnim.MoveTo(-Screen.height);


		}

	// ------------------
	private void OnPauseEnd()
		{
		this.pauseMenuOn = false;
		
		this.chara.OnPauseEnd();

		
		// Enable controller...

		this.touchCtrl.EnableController();
		}
	

	// -----------------
	private void UpdatePauseMenu()
		{
		if (!this.pauseMenuOn)
			return;

		if (this.pauseMenuTimer.Enabled)
			{
			if (this.pauseMenuTimer.Completed)
				{
				this.pauseMenuTimer.Disable();

				if (!this.pauseMenuFadeIn)
					{
					this.OnPauseEnd();
					}
				}
			else
				{
				this.pauseMenuTimer.Update(Time.deltaTime);
				this.pauseMenuAnim.Update(this.pauseMenuTimer.Nt * this.pauseMenuTimer.Nt);
				}
			}

		// When not animating...

		else
			{
			if (Input.GetKeyDown(KeyCode.Escape))	
				this.ExitPauseMenu();
			}
		}


	// --------------
	private void DrawPauseMenu()
		{
		if (!this.pauseMenuOn)
			return;
		
		//Color bgColor = new Color(0.1f, 0.1f, 0.1f, 1.0f);

		Rect r = this.pauseMenuRect;
		
		r.y = this.pauseMenuAnim.cur;
		
		GUI.color 			= Color.white;
		GUI.contentColor 	= new Color(1,1,1,1);
		GUI.backgroundColor = new Color(0.3f, 0.3f, 0.3f, 1.0f);

//		GUI.color = bgColor;
		GUI.Box(r, "PAUSE");
		
//		GUI.color = Color.white;
				


		r.x += this.pauseMenuRect.width * 0.1f;

		//r.x 		+= 20;

		r.y 		+= 30;
		r.width 	= this. pauseMenuRect.width * 0.8f;
		r.height 	= 30;

		GUI.Label(r, "Aim Sensitivity");
	
		r.y += r.height + 10;

		this.chara.aimSensitivity = GUI.HorizontalSlider(r, this.chara.aimSensitivity, 0, 1);
		

		r.y 		+= r.height  + 10;
		//r.width 	= 50;
	
		bool initialLeftHanded = this.touchCtrl.GetLeftHandedMode();
		if (initialLeftHanded != GUI.Toggle(r, initialLeftHanded, "Left Handed Mode"))
			this.touchCtrl.SetLeftHandedMode(!initialLeftHanded);
	


		r.y 		+= r.height + 10;
		//r.width 	= 40;
		r.height 	= 30;

		r.y = (this.pauseMenuAnim.cur + this.pauseMenuRect.height) - (r.height + 20); 

		//GUI.backgroundColor = bgColor;

		if (GUI.Button(r, "EXIT"))
			this.ExitPauseMenu();


		GUI.color = Color.white;

		}

	// --------------
	public bool IsPaused()
		{
		return this.pauseMenuOn; 	
		}


#if UNITY_3_5
	private const string	CAPTION_COLOR_BEGIN 		= "";	
	private const string	CAPTION_COLOR_END 			= "";
#else
	private const string	CAPTION_COLOR_BEGIN 		= "<color='#FF0000'>";	
	private const string	CAPTION_COLOR_END 			= "</color>";
#endif

	private const string 
		INSTRUCTIONS_TITLE 			= "Inctructions",
		INSTRUCTIONS_BUTTON_TEXT 	= "",
		INSTRUCTIONS_TEXT  			= 
			CAPTION_COLOR_BEGIN +
			"* Walking.\n" +
			CAPTION_COLOR_END +
			"Press on the left half of the screen to activate the dynamic stick.\n" +
			"\n" +
			CAPTION_COLOR_BEGIN +
			"* Aiming.\n" +
			CAPTION_COLOR_END +
			"Drag on the right half of the screen.\n" +
			"\n" +
			CAPTION_COLOR_BEGIN +
			"* Reset Vertical Aim.\n" + 
			CAPTION_COLOR_END +
			"Double tap on the right half of the screen.\n" +	
			"\n" +
			CAPTION_COLOR_BEGIN +	
			"* Toggle Zoom.\n" +
			CAPTION_COLOR_END +
			"Tap on the ZOOM button.\n" +
			"\n" +
			CAPTION_COLOR_BEGIN +
			"* Zoom In/Out.\n" +	
			CAPTION_COLOR_END +
			"When zoom is activated, touch the ZOOM button and drag up or down.";



	}

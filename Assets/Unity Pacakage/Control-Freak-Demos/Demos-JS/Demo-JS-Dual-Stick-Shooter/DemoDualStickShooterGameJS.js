// --------------------------
// Dual Stick Shoter Demo ---
// Dan's Game Tools 2013 ----
// --------------------------

#pragma strict


@script AddComponentMenu("ControlFreak-Demos-JS/DemoDualStickShooterGameJS")

public class DemoDualStickShooterGameJS extends MonoBehaviour 
	{
	public var	ctrl				: TouchController;
	public var	player				: DemoDualStickShooterCharaJS;
	public var	cam					: Camera;
	public var	guiSkin				: GUISkin;
	public var	popupBox			: PopupBoxJS;

	public static var STICK_WALK 	: int= 0;
	public static var STICK_FIRE 	: int= 1;
	public static var ZONE_SCREEN 	: int= 0;
	public static var ZONE_PAUSE 	: int= 1;
	
	private var	camOfs	: Vector3;


	// ------------------	
	function Start()
		{
		// Init player state...

		this.player.Init(this);

		
		// Get initial camera offset...

		if ((this.cam != null) && (this.player != null))
			this.camOfs = this.cam.transform.position - this.player.transform.position;
		else
			this.camOfs = new Vector3(0, 5, -5);
		}


	// ---------------
	function Update()
		{	
		// If the game is paused...

		if ((this.popupBox != null) && this.popupBox.IsVisible())
			{
			if (Input.GetKeyUp(KeyCode.Escape))
				{
				this.popupBox.End();
				}
			
			// Unpause...

			if (this.popupBox.IsComplete())
				{
				this.popupBox.Hide();

				// Enable controller...

				this.ctrl.EnableController();

				// Unpause the player...

				this.player.OnUnpause();
				}
			}
		
		// When not paused...
		else
			{

			// Return to Main menu...
	
			if (Input.GetKeyUp(KeyCode.Escape))
				{
				DemoSwipeMenuJS.LoadMenuScene();
				return;
				}
	
	
			// Handle input...
	
			if (this.ctrl)
				{	
				// Get stick and zone references by IDs...
 
				var walkStick : TouchStick	= this.ctrl.GetStick(STICK_WALK);
				var	fireStick : TouchStick	= this.ctrl.GetStick(STICK_FIRE);
				var pauseZone : TouchZone	= this.ctrl.GetZone(ZONE_PAUSE);
				
				
				// If the PAUSE zone (or SPACEBAR) is released, show info box...
 
				if (pauseZone.JustUniReleased() || Input.GetKeyUp(KeyCode.Space))
					{
					// Show popup box...

					this.popupBox.Show(INFO_TITLE, INFO_BODY, INFO_BUTTON_TEXT);
					
					// Disable controller to stop it from reacting to touch input...

					this.ctrl.DisableController();

					// Pause the game...

					this.player.OnPause();
					}
	
				else
					{
					// Walk when left stick is pressed...

					if (walkStick.Pressed())
						{	
						// Use stick's normalized XZ vector and tilt to move...

						this.player.Move(walkStick.GetVec3d(true, 0), walkStick.GetTilt());
						}
	
					// Stop when stick is released...

					else
						{
						this.player.Move(Vector3.zero, 0);
						}
					
		
					// Shoot when right stick is pressed...
	
					if (fireStick.Pressed())
						{
						this.player.SetTriggerState(true);
			
						// Get target angle and stick's tilt to determinate turning speed.

						this.player.Aim(fireStick.GetAngle(), fireStick.GetTilt());
						}

					// ...or stop shooting and aiming when right stick is released.

					else
						{
						this.player.SetTriggerState(false);
						this.player.Aim(0,0);
						}
					}
				}		
			}
		

		// Update character...

		this.player.UpdateChara();

		
		// Update camera...
	
		if (this.cam != null)
			{
			var camTf : Transform  = this.cam.transform;
			camTf.position = this.player.transform.position + this.camOfs;
			}
		}


	// ---------------
	function OnGUI()
		{
		if (this.ctrl != null)
			this.ctrl.DrawControllerGUI();

		if ((this.popupBox != null) && !this.popupBox.IsVisible())
			{
			GUI.skin = this.guiSkin;
			GUI.color = Color.white;
			GUI.Label(new Rect(40, 10, Screen.width - 100, 100),
				"Dual Stick Demo - Press [Space] for help, [Esc] to quit.");
			}

		if (this.popupBox != null)
			this.popupBox.DrawGUI();
		}

	


	// ------------------------
	// Info strings -----------
	// ------------------------

#if UNITY_3_5
	private static var CAPTION_COLOR_BEGIN	: String	= "";	
	private static var CAPTION_COLOR_END	: String	= "";
#else
	private static var CAPTION_COLOR_BEGIN	: String	= "<color='#FF0000'>";	
	private static var CAPTION_COLOR_END	: String	= "</color>";
#endif

	private static var INFO_TITLE			: String	= "Inctructions";
	private static var INFO_BUTTON_TEXT		: String	= "OK";
	private static var INFO_BODY			: String	= 
			CAPTION_COLOR_BEGIN +
			"* Walking.\n" +
			CAPTION_COLOR_END +
			"Press on the left half of the screen to activate blue dynamic stick.\n" +
			"\n" +

			CAPTION_COLOR_BEGIN +
			"* Shooting.\n" +
			CAPTION_COLOR_END +
			"Press on the right half of the screen to activate red dynamic stick.\n" +
			"Holding the stick will activate gun's trigger.\n" +
			"\n" +

			CAPTION_COLOR_BEGIN +
			"* Aiming.\n" +
			CAPTION_COLOR_END +
			"Moving the stick from it's neutral position will orient the character in it's direction.\n" +
			"Aiming speed is proportional to stick's tilt.\n" +
			"";

	
	}

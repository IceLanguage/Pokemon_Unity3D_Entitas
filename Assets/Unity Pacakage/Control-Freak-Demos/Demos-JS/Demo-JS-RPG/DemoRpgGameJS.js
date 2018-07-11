
@script AddComponentMenu("ControlFreak-Demos-JS/DemoRpgGameJS")

public class DemoRpgGameJS extends MonoBehaviour
{
public var 	player			: DemoRpgCharaJS;
public var 	cam				: Camera;
public var	ctrl			: TouchController;
	
public var	guiSkin			: GUISkin;
public var 	popupBox		: PopupBoxJS;
	
public var	camOrbitalAngle	: float = 0;

public var	zoomFactorPerCm	: float = 0.3f;		// Zoom factor change per centimeter of pinch.	

public var	camSmoothingTime: float = 0.2f;
	

public var	camZoom			: float	= 0.5f;
private var	camZoomForDisplay	: float;
private var	camZoomVel		: float;

private	var	camFov			: float;
private	var	camDist			: float;
private	var	camBank			: float;


public var	camFarBank		: float	= 60;
public var	camCloseBank	: float = 25;
						
public var	camFarFov		: float	= 60;
public var	camCloseFov		: float	= 50;
							
public var	camFarDist		: float	= 10;
public var	camCloseDist	: float	= 4;
	
	
// Helper variables...

private var isMultiTouching	: boolean;
private var	twistStartAngle	: float;
private var	pinchStartZoom	: float;
						

// ----------------------
// Controller constants
// ----------------------

	public static var STICK_WALK	: int = 0;
	public static var ZONE_SCREEN	: int = 0;
	public static var ZONE_FIRE	: int = 1;
	public static var ZONE_ACTION	: int = 2;




// --------------------
function Start()
	{
	if (this.ctrl == null)
		Debug.LogError("TouchController not assigned!");		
	if (this.cam == null)
		Debug.LogError("Camera not assigned!");
	


	// Manually init the controller...

	this.ctrl.InitController();

		
	this.SnapCameraDisplay();
	}
	



// ---------------
@ContextMenu("Snap Cam Display") 
private function SnapCameraDisplay() : void
	{
	this.camZoomForDisplay = this.camZoom;
	}

	






// ----------------
function Update()
	{
	if (Input.GetKeyUp(KeyCode.Escape))
		{
		DemoSwipeMenuJS.LoadMenuScene();
		return;
		}

	// Manually poll and update the controller...

	this.ctrl.PollController();
	this.ctrl.UpdateController();
		

	// Control and update the player controller...

	if (this.player != null)
		{
		this.player.ControlByTouch(this.ctrl, this);
		this.player.UpdateChara();
		}


		
	// Popup box update...

	if (this.popupBox != null)
		{
		if (!this.popupBox.IsVisible())
			{
			if (Input.GetKeyDown(KeyCode.Space))
				this.popupBox.Show(INSTRUCTIONS_TITLE, INSTRUCTIONS_TEXT, 
					INSTRUCTIONS_BUTTON_TEXT);
			}
		else
			{
			if (Input.GetKeyDown(KeyCode.Space))
				this.popupBox.Hide();
			}
		}	




	// Control camera...
		
	var	zoneScreen	: TouchZone		= this.ctrl.GetZone(ZONE_SCREEN);
	var	stickWalk	: TouchStick	= this.ctrl.GetStick(STICK_WALK);


	// If screen is pressed by two fingers (excluding mid-frame press and release).		
	
	if (zoneScreen.MultiPressed(false, true))
		{
			
		if (!this.isMultiTouching)
			{
			// If we just started multi-touch, store initial zoom factor.

			this.pinchStartZoom 	= this.camZoom;
			this.isMultiTouching 	= true;				

			// Cancel stick's touch if it's shared with our catch-all zone...

			zoneScreen.TakeoverTouches(stickWalk);
			}

			
		// If pinching is active...

		if (zoneScreen.Pinched())
			{
			// Get pinch distance delta in centimeters (screen-size independence!),
			// then add it to our non-clamped state variable...
 
			this.pinchStartZoom += this.zoomFactorPerCm * 
				zoneScreen.GetPinchDistDelta(TouchCoordSys.SCREEN_CM, false);

			// ... and pass it to proper function when zoom factor will be clamped.

			this.SetZoom(this.pinchStartZoom);
			}
		}

	// If less than two fingers are touching the zone...
	else
		{
		this.isMultiTouching = false;

		}

		


	// Update camera...
	
	this.camZoom = Mathf.Clamp01(this.camZoom);
	this.camZoomForDisplay = Mathf.SmoothDamp(this.camZoomForDisplay, this.camZoom,
		this.camZoomVel, this.camSmoothingTime);

		
	

	// Place camera...

	this.PlaceCamera();
	}



// ---------------
private function SetZoom(zoomFactor : float) : void
	{
	this.camZoom = Mathf.Clamp01(zoomFactor);	
	}

// ------------------
#if UNITY_EDITOR
@ContextMenu("Place Camera")
#endif
private function PlaceCamera() : void
	{
	
	this.camZoom = Mathf.Clamp01(this.camZoom);

	this.camBank	= Mathf.Lerp(this.camCloseBank,	this.camFarBank,this.camZoomForDisplay);
	this.camDist	= Mathf.Lerp(this.camCloseDist,	this.camFarDist,this.camZoomForDisplay);
	this.camFov		= Mathf.Lerp(this.camCloseFov,	this.camFarFov, this.camZoomForDisplay);
	

	//this.camDist = Mathf.Clamp(this.camDist, this.camMinDist, this.camMaxDist);
	//this.camBankAngle = Mathf.Clamp(this.camBankAngle, this.camMinBankAngle, this.camMaxBankAnge);

	if (this.cam != null)
		{
		var camRot : Quaternion= Quaternion.Euler(this.camBank, this.camOrbitalAngle, 0);
		this.cam.transform.rotation = camRot;
		this.cam.transform.position = this.player.transform.position +
			new Vector3(0, 1.5f, 0) + 
			(camRot * new Vector3(0, 0, -this.camDist));
	
		this.cam.fieldOfView = this.camFov;
		}
	}





// --------------
function OnGUI()
	{
	GUI.skin = this.guiSkin;


	// Manually draw controller's GUI...

	if (this.ctrl != null)
		this.ctrl.DrawControllerGUI();
		

	// Popup box GUI...

	if ((this.popupBox != null) && this.popupBox.IsVisible())
		this.popupBox.DrawGUI();
	else
		{
		GUI.color = Color.white;
		GUI.Label(new Rect(10, 10, Screen.width - 20, 100),
			"RPG Demo - Press [Space] for help, [Esc] to quit.");
		}


	}
	



	// Inscructions strings -------------

#if UNITY_3_5
	private static var	CAPTION_COLOR_BEGIN 		: String	= "";
	private static var	CAPTION_COLOR_END 			: String	= "";
#else
	private static var	CAPTION_COLOR_BEGIN 		: String	= "<color='#FF0000'>";
	private static var	CAPTION_COLOR_END 			: String	= "</color>";
#endif

	private static var	INSTRUCTIONS_TITLE			: String	= "Inctructions";
	private static var	INSTRUCTIONS_BUTTON_TEXT	: String 	= "";
	private static var	INSTRUCTIONS_TEXT			: String	= 
			CAPTION_COLOR_BEGIN +
			"* Walking.\n" +
			CAPTION_COLOR_END +
			"Press anywhere on the screen to activate the dynamic stick.\n" +
			"\n" +
			CAPTION_COLOR_BEGIN +
			"* Action.\n" +
			CAPTION_COLOR_END +
			"Tap on the screen or press the ACTION button to perform ACTION move.\n" +
			"\n" +
			CAPTION_COLOR_BEGIN +
			"* Zoom.\n" + 
			CAPTION_COLOR_END +
			"Place two fingers on the screen and spread them to zoom out or pinch to zoom-in.\n" +
			"\n" +
			CAPTION_COLOR_BEGIN +	
			"* Fire Weapon.\n" +
			CAPTION_COLOR_END +
			"Hold FIRE button to fire yout weapon.\n";


}


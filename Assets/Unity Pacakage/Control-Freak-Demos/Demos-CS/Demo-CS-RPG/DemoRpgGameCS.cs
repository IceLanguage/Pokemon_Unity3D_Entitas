using UnityEngine;
//using TouchControllerLib;


[AddComponentMenu("ControlFreak-Demos-CS/DemoRpgGameCS")]
public class DemoRpgGameCS : MonoBehaviour
{
public DemoRpgCharaCS		player;
public Camera				cam;
public TouchController		ctrl;
	
public GUISkin				guiSkin;
public PopupBoxCS			popupBox;
	

// ----------------------
// Controller constants
// ----------------------

	public const int STICK_WALK		= 0;
	public const int ZONE_SCREEN	= 0;
	public const int ZONE_FIRE		= 1;
	public const int ZONE_ACTION	= 2;

	

public float				camOrbitalAngle = 0;


private	float				camFov;
private	float				camDist;
private	float				camBank;

public float				zoomFactorPerCm = 0.3f;		// Zoom factor change per centimeter of pinch.	

public float				camSmoothingTime = 0.2f;
	

public float				camZoom				= 0.5f;
private float				camZoomForDisplay;
private float				camZoomVel;

public float				camFarBank			= 60;
public float				camCloseBank 		= 25;
						
public float				camFarFov			= 60;
public float				camCloseFov			= 50;
							
public float				camFarDist		= 10;
public float				camCloseDist	= 4;
	
							

// --------------------
private void Start()
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
[ContextMenu("Snap Cam Display")] 
private void SnapCameraDisplay()
	{
	this.camZoomForDisplay = this.camZoom;
	}

	



// Helper variables...

private bool	isMultiTouching;
private float 	twistStartAngle;
private float	pinchStartZoom;



// ----------------
private void Update()
	{
	if (Input.GetKeyUp(KeyCode.Escape))
		{
		DemoSwipeMenuCS.LoadMenuScene();
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
		
	TouchZone 	zoneScreen 	= this.ctrl.GetZone(ZONE_SCREEN);
	TouchStick	stickWalk 	= this.ctrl.GetStick(STICK_WALK);



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
				zoneScreen.GetPinchDistDelta(TouchCoordSys.SCREEN_CM);

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
		ref this.camZoomVel, this.camSmoothingTime);

		
	

	// Place camera...

	this.PlaceCamera();
	}



// ---------------
private void SetZoom(float zoomFactor)
	{
	this.camZoom = Mathf.Clamp01(zoomFactor);	
	}

// ------------------
#if UNITY_EDITOR
[ContextMenu("Place Camera")]
#endif
private void PlaceCamera()
	{
	
	this.camZoom = Mathf.Clamp01(this.camZoom);

	this.camBank	= Mathf.Lerp(this.camCloseBank,	this.camFarBank,this.camZoomForDisplay);
	this.camDist	= Mathf.Lerp(this.camCloseDist,	this.camFarDist,this.camZoomForDisplay);
	this.camFov		= Mathf.Lerp(this.camCloseFov,	this.camFarFov, this.camZoomForDisplay);
	

	if (this.cam != null)
		{
		Quaternion camRot = Quaternion.Euler(this.camBank, this.camOrbitalAngle, 0);
		this.cam.transform.rotation = camRot;
		this.cam.transform.position = this.player.transform.position +
			new Vector3(0, 1.5f, 0) + 
			(camRot * new Vector3(0, 0, -this.camDist));
	
		this.cam.fieldOfView = this.camFov;
		}
	}





// --------------
public void OnGUI()
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

	private const string	INSTRUCTIONS_TITLE 			= "Inctructions";
	private const string	INSTRUCTIONS_BUTTON_TEXT 	= "";

#if UNITY_3_5
	private const string	CAPTION_COLOR_BEGIN 		= "";	
	private const string	CAPTION_COLOR_END 			= "";
#else
	private const string	CAPTION_COLOR_BEGIN 		= "<color='#FF0000'>";	
	private const string	CAPTION_COLOR_END 			= "</color>";
#endif

	private const string	INSTRUCTIONS_TEXT  			= 
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


// -----------------------------------------------
// Control Freak - The Ultimate Virtual Controller
// Copyright (C) 2013 Dan's Game Tools
// -----------------------------------------------



//#define ENABLE_DEBUG_LOGGING		


#if UNITY_EDITOR
#	define	DEBUG_MODE
#	define	USE_EDITOR_KEYBOARD_KEYS	
#	define	ENABLE_SCREEN_EMU
#endif

#if UNITY_EDITOR || (!UNITY_ANDROID && !UNITY_IPHONE && !UNITY_BLACKBERRY && !UNITY_WP8)
#	define EMULATE_TOUCHES_BY_MOUSE
#endif

//#if (UNITY_WEBPLAYER && !UNITY_EDITOR)
//#define FORCE_EMULATED_TOUCH_DRAWING
//#endif


#define DRAW_DEBUG_GUI

#define USE_JOYSTICK_INPUT

#define ENABLED_DEBUGGING_HELPERS



#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections.Generic;

#if ENABLE_DEBUG_LOGGING		
using System.IO;
#endif


// -----------------
/*!
\brief Main controller class.
 
\nosubgrouping


**/
// -----------------

//#if UNITY_EDITOR
[ExecuteInEditMode()]
//#endif

[AddComponentMenu("ControlFreak/Control Freak Controller")]
public class TouchController : MonoBehaviour 
{
	
// \nosubgrouping

// --------------
/// \cond DGT_DOCS_SHOW_PUBLIC_VARS
// --------------

public bool			automaticMode	= true;		///< When true, Monobehaviour will automaticly update joystick's state, otherwise - Poll(), UpdateJoy() functions need to called manually.
public bool			manualGui		= false;	///< When true, automatic GUI rendering will be disabled and DrawControllerGUI() must be called manually.
	
public bool			autoActivate	= true;		///< Automatically set this controller as active (CFInput.ctrl) 
public bool			disableWhenNoTouchScreen = false;	///< Disactivate this controller when the hardware doesn't support multi-touch input.
	
//public bool 		initiallyHidden	= false;			///< Iniitally hidden?


public int 			guiDepth		= 10;		///< Controller's base GUI depth
public int			guiPressedOfs	= 10;		///< Depth offset for pressed control
public float 		fingerBufferCm			= 0.8f;		///< finger area in centimeters
private float		fingerBufferRadPx		= 10;		// recompute this on res-change

public float		stickMagnetAngleMargin	= 10.0f;	///< number of degrees over needed to angle threshold in degrees, angle magnet margin
public float		stickDigitalEnterThresh	= 0.5f;		///< digital thershold for enetring active zone (0..1) 
public float		stickDigitalLeaveThresh	= 0.4f;		///< digital thershold for leaving active zone to resting zone (SHOULD be always lower than EnterThresh!) 
	
public float		touchTapMaxTime			= (8.0f / 60.0f);	///< Maximal touch time for a tap (from touch start to release) and time betwwen taps for a double tap
public float		doubleTapMaxGapTime		= (20.0f / 60.0f);	///< Maxmimal allowed time between taps to form a double tap
public float		strictMultiFingerMaxTime= (0.2f);			///< Maximal allowed time for the second finger to touch the zone after the first one to start a multi-touch
public float		velPreserveTime			= (0.1f);			///< Amount of time that last non-zero drag velocity will be preserved after movement stopped. Use this to velocity-based swapping easier on those high-pixel-density, low-touch-precision devices.  
public float		touchTapMaxDistCm		= (0.5f);			///< Maximal allowed drag distance for a touch to be considered static.
					

public float		twistThresh				= 5.0f;				///< twist angular dead zone in degrees
	
public float		pinchMinDistCm			= 0.5f;				///< Minimal finger distance difference to activate pinch.
public float		twistSafeFingerDistCm	= 1.0f;				///< Minimal safe finger distance for twisting (in centimeters) 
	

	


public float		curTime 				= 0,			///< Internal time
					deltaTime				= (1.0f / 60.0f),///< Internal delta time
					invDeltaTime			= 60;			///< Delta time inverse (for safe division)
private float		lastRealTime;
	
private bool		initialized;			// flag used to check proper initialization order.
	
public 	TouchStick[]	sticks;				///< TouchStick array
public	TouchZone[]		touchZones;			///< TouchZone array
public	LayoutBox[]		layoutBoxes;

[System.NonSerialized]
private TouchStick		blankStick;

[System.NonSerialized]
private TouchZone		blankZone;

[System.NonSerialized]
private	List<TouchableControl>	touchables;

[System.NonSerialized]
private	List<Rect>				maskAreas;	

	
private	bool	layoutDirtyFlag,
				contentDirtyFlag,
				releaseTouchesFlag;


	
public float	pressAnimDuration		= 0.1f; ///< Default press animation duration (in seconds)
public float	releaseAnimDuration		= 0.3f; ///< Default release animation duration (in seconds)
public float	disableAnimDuration		= 0.3f; ///< Default disable animation duration (in seconds)
public float	enableAnimDuration		= 0.3f;	///< Default enable animation duration (in seconds)
public float	cancelAnimDuration		= 0.3f;	///< Default cancel animation duration (used by dynamic-sticks after forced untouch)
public float	showAnimDuration		= 0.3f;	///< Default showing animation duration (in seconds).
public float	hideAnimDuration		= 0.3f;	///< Default hiding animation duration (in seconds).

	
public float	releasedZoneScale		= 1.0f;		///< Global Zone's scale when released.
public float	pressedZoneScale		= 1.1f;		///< Global Zone's scale when pressed.
public float	disabledZoneScale		= 1.0f;		///< Global Zone's scale when disabled.

public float	releasedStickHatScale	= 0.75f;	///< Global Stick Hat's scale when released.
public float	pressedStickHatScale	= 0.9f;		///< Global Stick Hat's scale when pressed.
public float	disabledStickHatScale	= 0.75f;	///< Global Stick Hat's scale when disabled.
public float	releasedStickBaseScale	= 1.0f;		///< Global Stick Base's scale when released.
public float	pressedStickBaseScale	= 0.9f;		///< Global Stick Base's scale when pressed.
public float	disabledStickBaseScale	= 1.0f;		///< Global Stick Base's scale when disabled.


public Color	defaultPressedZoneColor		= new Color(1.0f, 1.0f, 1.0f, 1.0f);
public Color	defaultReleasedZoneColor	= new Color(1.0f, 1.0f, 1.0f, 0.75f);
public Color	defaultDisabledZoneColor	= new Color(0.5f, 0.5f, 0.5f, 0.35f);

public Color	defaultPressedStickHatColor		= new Color(1.0f, 1.0f, 1.0f, 1.0f);
public Color	defaultReleasedStickHatColor	= new Color(1.0f, 1.0f, 1.0f, 0.75f);
public Color	defaultDisabledStickHatColor	= new Color(0.5f, 0.5f, 0.5f, 0.35f);

public Color	defaultPressedStickBaseColor	= new Color(1.0f, 1.0f, 1.0f, 1.0f);
public Color	defaultReleasedStickBaseColor	= new Color(1.0f, 1.0f, 1.0f, 0.75f);
public Color	defaultDisabledStickBaseColor	= new Color(0.5f, 0.5f, 0.5f, 0.35f);


private float	globalAlpha				= 1.0f;
private float	globalAlphaStart,
				globalAlphaEnd;
private	AnimTimer	
				globalAlphaTimer;


private int		screenWidth,		// store this to detect resultion change
				screenHeight;
	

 
private bool	disableAll;
private bool	leftHandedMode = false;		


public const int
	DEFAULT_ZONE_PRIO		= 0,	// gesture detector should always have LOWEST priority
	DEFAULT_STICK_PRIO		= 0;

private const int	
	MAX_EVENT_SHARE_COUNT	= 8;
	
private const float
	NON_MOBILE_DIAGONAL_INCHES	= 7;	// Used for web and standalone demos to calculate virtual DPI


private const float 
	DEFAULT_MONITOR_DPI	= 96;			// Used in case when system's DPI is unknown.



// Debugging stuff ----------------

public 	KeyCode		debugSecondTouchDragModeKey 	= KeyCode.LeftShift;	// Second finger copies the movement of the mouse.
public 	KeyCode		debugSecondTouchPinchModeKey	= KeyCode.LeftControl;	// Second finger mirrors horizontal movement of the mouse, allowing horizontal pinch.  
public 	KeyCode		debugSecondTouchTwistModeKey	= KeyCode.LeftAlt;		// Second finger mirrors horizontal and vertical movement of the mouse allowing twist.
	
public bool			debugDrawTouches 			= true;
public bool			debugDrawLayoutBoxes		= true;
public bool			debugDrawAreas 				= true;

public Texture2D	debugTouchSprite;
public Texture2D	debugSecondTouchSprite;
public Color		debugFirstTouchNormalColor = new Color(1,1, 0.6f, 0.3f);
public Color		debugFirstTouchActiveColor = new Color(1, 1, 0, 0.7f);
public Color		debugSecondTouchNormalColor	= new Color(1,1,1, 0.3f);
public Color		debugSecondTouchActiveColor	= new Color(1,0,0, 0.6f);
	
public Texture2D	defaultZoneImg;
public Texture2D	defaultStickHatImg;
public Texture2D	defaultStickBaseImg;


	
public Texture2D	debugCircleImg;
public Texture2D	debugRectImg;
	
public bool			screenEmuOn 			= false;		// Enable screen emulation
public bool			screenEmuPortrait		= false;		// Display in portrait mode (swap horizontal and vertical resolutions)
public bool			screenEmuShrink			= true;			// Allow scaling down when the screen's too small.
public Vector2		screenEmuPan			= new Vector2(0.5f, 0.5f);		// normalized screen coordinates 
public int			screenEmuHwDpi 			= 250;			// Emulated device's DPI.
public ScreenEmuMode	screenEmuMode		= ScreenEmuMode.EXPAND;
	
public int			screenEmuHwHorzRes		= (1024);
public int			screenEmuHwVertRes		= (int)(1024 * (10.0f / 16.0f));
public float		monitorDiagonal			= 15;	/// monitor's diagonal in inches
public Color		screenEmuBorderColor	= new Color(0,		0,		0,		0.75f);	// Emulated screen's border color when displayed correctly
public Color		screenEmuBorderBadColor	= new Color(0.5f,0,		0,		0.75f);	// Emulated screen's border color when shrunk.

#if ENABLE_SCREEN_EMU	
[System.NonSerialized]
private float		monitorDpi				= 96;

[System.NonSerialized]
private float		screenEmuCurWidth,
					screenEmuCurHeight,		
					screenEmuCurDPI,
					screenEmuCurAspectRatio;
[System.NonSerialized]
private Vector2		screenEmuCurOfs;
[System.NonSerialized]
private bool		screenEmuBorderShrunk	= false;
	
#endif









	
public RealWorldUnit	rwUnit;		///< Real-world unit used to display coordinates in the editor
public PreviewMode		previewMode;	///< In-Editor Preview mode.


	

private bool		//firstPostPollUpdate,
					//layoutEventWaiting,
					customLayoutNeedsRebuild;
					//forceLayoutChangeEventFlag;
	

private Vector2		emuMousePos;				///< Emulated mouse position.

public int			version				= 0;	// script version for future updates...
	

#if EMULATE_TOUCHES_BY_MOUSE
[System.NonSerialized]
private Vector2		debugPrevFrameMousePos,
					debugFirstTouchPos,
					debugSecondTouchPos;
[System.NonSerialized]
private bool		debugPrevFrameMouseButton0,
					debugPrevFrameMouseButton1;
#endif
	
// -----------------
/// Real world unit.
// ------------------
public enum RealWorldUnit
	{
	CM,				///< Centimeters.
	INCH			///< Inches.
	}

	

// ---------------
/// In-Editor Preview Mode
// ----------------
public enum PreviewMode
	{	
	RELEASED,		///< Preview controls in released state.
	PRESSED,		///< Preview controls in pressed state.
	DISABLED		///< Preview controls in disabled state.
	}
		


public const int LayoutBoxCount = 16; 


	
// --------------------
/// Layout box anchor.
// ---------------------

public enum LayoutAnchor
	{
	BOTTOM_LEFT,
	BOTTOM_CENTER,
	BOTTOM_RIGHT,
	MID_LEFT,
	MID_CENTER,
	MID_RIGHT,
	TOP_LEFT,
	TOP_CENTER,
	TOP_RIGHT
	}
		



// \cond 

public float		twistSafeFingerDistPx	= 10;					
public float		pinchMinDistPx			= 10;
public float		touchTapMaxDistPx		= 10;				// calculated on layout
	
// \endcond

// -----------
/// \endcond
// All variables hidden.
// -----------
	

// --------------------
/// Control's Shape
// --------------------

public enum ControlShape
	{	
	CIRCLE,			///< Circle	
	RECT,			///< Rectangular box	
	SCREEN_REGION	///< Normalized screen region. Doesn't affect layout of other shapes!
	}


// ----------------------
/// Screen emulation mode.
// ----------------------

public enum ScreenEmuMode
	{
	PIXEL_PERFECT,	///< Pixel-perfect display using monitor's DPI.
	PHYSICAL,		///< Emulated screen's physical dimensions will match real hardware's - for example, emulated Nexus 7 will take up about a quarter of the screen on a 14-inch monitor.    
	EXPAND			///< Emulated screen will expand to window. Good for taking screenshots. Warning - emulated screen may have higher resolution than the actual device. 
	}





	
#if DEBUG_MODE
[System.NonSerialized]
private bool	autoPollErrReported,
				autoUpdateErrReported,	
				autoGuiErrReported;
#endif
	
	
	
/// \name Genral Methods
///	<ul>
///		<li>Controller can work either in automatic or manual mode.
///		When in manual mode, it's up to you to call InitController(), PollController(), UpdateController() and DrawControllerGUI().
///		</li>
///		<li>It's always a good idea to call ResetController(), when starting a new level or mode.
///		</li>
///	</ul>
/// \{


// --------------------
///	\brief 
/// Manually initialize the controller.
/// \note It's critical to call this function before any other!
// --------------------
public void InitController()
	{
	this.contentDirtyFlag = false;
		
	//this.firstPostPollUpdate = true;
//	this.forceLayoutChangeEvent = true;
		

	
	this.curTime 		= 0;
	this.deltaTime		= (1.0f / 60.0f);
	this.invDeltaTime	= (1.0f / this.deltaTime);
	this.lastRealTime	= Time.realtimeSinceStartup;



	// Reset emulated mouse position...

	this.emuMousePos = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);


	// Create unified touchable objects list and init them...
		
	if (this.sticks == null)
		this.sticks = new TouchStick[0];
	if (this.touchZones == null)
		this.touchZones = new TouchZone[0];


	if (this.touchables == null)
		this.touchables = new List<TouchableControl>(16);
	
	this.touchables.Clear();


	
	// Add sticks...

	if (this.sticks != null)
		{
		foreach (TouchStick c in this.sticks)
			{
			if (c != null)		
				this.touchables.Add(c);
			}
		}

	// Add Touchable3dLayer ...

	//if (this.touchable3dLayer != null)
	//	this.touchables.Add(this.touchable3dLayer);

	// Add gesture detector...

	if (this.touchZones != null)
		{
		foreach (TouchZone c in this.touchZones)
			{
			if (c != null)
				this.touchables.Add(c);
			}
		}

			

	// Init the controls...
	
	foreach (TouchableControl o in this.touchables)
		{
		o.Init(this);
		}
		
		
	// Start alpha animation
	
	//this.StartAlphaAnim(0, 0);
	//this.StartAlphaAnim(1, this.alphaFadeInDuration);

#if UNITY_EDITOR

//Debug.Log("ContrlInit : play:" + EditorApplication.isPlaying + " platyOrChange: " + EditorApplication.isPlayingOrWillChangePlaymode);

	if (!EditorApplication.isPlayingOrWillChangePlaymode)	
		{
		this.StartAlphaAnim(1, 0);
		//this.OnGlobalColorChange();
		}
	else
#endif
		{
		// Show with full alpha by default...

		//this.StartAlphaAnim((this.initiallyHidden ? 0 : 1), 0);

		if (!this.initialized)	
			this.StartAlphaAnim(1, 0);
		}


	// Layout!

	this.Layout();

		
	// Init second touch....

#if EMULATE_TOUCHES_BY_MOUSE
	this.debugSecondTouchPos = new Vector2(Screen.width / 2.0f, Screen.height / 2.0f);
#endif

	this.initialized = true;


	}




// -------------------------
///	\brief 
/// Manually poll controller's state. 
///
/// When called by a controller in automatic mode, error will be logged.
/// This function must be called inside Unity's Update() event. 
// -----------------------
public void PollController()
	{
	if (this.automaticMode)
		{
#		if DEBUG_MODE
		if (!this.autoPollErrReported)
			{
			Debug.LogError("Manual controller polling is not allowed when in automatic mode!!");
			this.autoPollErrReported = true;
			}	
#		endif
		return;
		}

	this.PollControllerInternal();
	}


// -------------------------
///	\brief 
/// Manually update controller's state. 
///
/// When called by a controller in automatic mode, error will be logged.
/// This function should be called only ONCE per game's update cycle (either inside Update() or FixedUpdate()),
/// otherwise functionality such as button press/release detection will not behave as expected!
/// Make sure to call this function after PollController().
// -------------------------
public void UpdateController()	
	{
	if (this.automaticMode)
		{
#		if DEBUG_MODE
		if (!this.autoUpdateErrReported)
			{
			Debug.LogError("Manual controller updated is not allowed when in automatic mode!!");
			this.autoUpdateErrReported = true;
			}	
#		endif
		return;
		}

	this.UpdateControllerInternal();
	}
	


// ----------------
///	\brief 
/// Manually draw controller's GUI. 
///
/// When called by a controller in automatic mode, error will be logged.
/// This function should be called inside Unity's OnGUI() function.
// -------------------
public void DrawControllerGUI()
	{
//#	if UNITY_EDITOR
//	if (EditorApplication.isPlayingOrWillChangePlaymode)
//#	endif

	if (this.automaticMode && !this.manualGui)
		{
#		if DEBUG_MODE
		if (!this.autoGuiErrReported)
			{
			Debug.LogError("Manual controller GUI drawing is not allowed when in automatic mode!!");
			this.autoGuiErrReported = true;
			}	
#		endif
		return;
		}

	this.DrawControllerGUIInternal();
	}
			



// --------------------
///	\brief 
/// Resets controller's state.
///
/// Call this before showing the controller.
// ------------------
public void ResetController()
	{
	if (this.touchables != null)
		{
		for (int i = 0; i < this.touchables.Count; ++i)
			{
			this.touchables[i].OnReset();
			}
		}
	}



// -----------------
///	\brief 
/// Release all active touches of all controls, without resetting their state.
///
/// This will count as normal touch release and will trigger normal state change.
// -----------------
public void ReleaseTouches()
	{
	foreach (TouchableControl o in this.touchables)
		o.ReleaseTouches();
	}	
	



//---------------
/// Show the controller. 
// ---------------
public void ShowController(
	float animDuration	///< Alpha animation duration.
	)
	{
	this.StartAlphaAnim(1, animDuration);
	}


// -------------
///	\brief 
/// Hide the controller controller. 
/// \note Warning - controller will be controllable even when invisible. Call, DisableController().
// ---------------
public void HideController(
	float animDuration		///< Alpha animation duration in seconds.
	)
	{
	this.StartAlphaAnim(0, animDuration);
	}
	

// ------------
/// Get controller's global alpha.
// ------------
public float GetAlpha()
	{
	return this.globalAlpha;
	}


// --------------------
///	\brief 
/// Disable touch input and release all active touches.
/// \note This doesn't affect controls' individual enabled/disabled state. 
// ---------------------
public void DisableController()
	{
	this.disableAll = true;	
	this.ReleaseTouches();
	}

	
// -------------------
/// Enable controller. 
// ---------------------
public void EnableController()
	{
	this.disableAll = false;
	}


// --------------
/// Return true if controller is enabled.
// ---------------
public bool ControllerEnabled()
	{
	return !this.disableAll;
	}
	
	
/// \}
	




	
// ------------------------------
/// \name Layout-related Methods
///	It's possible to override default controls' layout.\n 
///	Any custom positioning will be reset to default on the event of screen (window) resolution or orientation change, so 
///	it should re-applied every time LayoutChanged() returns true.  
// ------------------------
/// \{

// -------------------
///	\brief 
/// Returns true if layout changed and any custom layout need to be done.
///
/// Check this every update cycle.
/// When you're done with your custom layout, call LayoutChangeHandled() function or otherwise LayoutChanged() will keep returning true forever.
// ---------------------
public bool LayoutChanged()
	{
	return this.customLayoutNeedsRebuild;
	}


// --------------------
/// Indicate layout change handling.
// ---------------------  
public void LayoutChangeHandled()
	{
	this.customLayoutNeedsRebuild = false;
	}


// -------------------------
/// Reset every control's position and size to default (calculated by the layout system).
// ----------------------
public void ResetAllRects()
	{
	foreach (TouchableControl c in this.touchables)
		c.ResetRect();
	}


	
// -------------
/// \brief Get DPI (dots per inch).
/// 
/// If screen emulation is active, emulated DPI will be returned.
// -------------
public float GetDPI()
	{
#if UNITY_WEBPLAYER && !UNITY_EDITOR
	return TouchController.webPlayerDPI;	
#else	
		
#if ENABLE_SCREEN_EMU
	if (this.screenEmuOn)
		{
		return this.screenEmuCurDPI;
		}
#endif

	return this.GetActualDPI();
#endif
	}
	

// -------------
/// \brief Get DPCM (Dots per centimeter).
/// 
/// If screen emulation is active, emulated DPCM will be returned.
// -------------
public float GetDPCM()
	{
	return (this.GetDPI() / 2.54f);
	}



// ---------------
/// \brief Get Actual DPI.
/// 
/// Returns display's actual DPI.\n
/// When hardware's DPI isn't available, average monitor's DPI will be returned.   
// ---------------
public float GetActualDPI()
	{
#if UNITY_EDITOR
	return this.monitorDpi;
#else
	if (Screen.dpi != 0)
		return Screen.dpi; 

#	if UNITY_WP8 || UNITY_ANDROID || UNITY_IPHONE || UNITY_BLACKBERRY 
	if ((wp8dpi == 0) || (Screen.width != wp8width) || (Screen.height != wp8height))
		{
		wp8width	= Screen.width;
		wp8height	= Screen.height;
		wp8dpi		= (Mathf.Sqrt((wp8width * wp8width) + (wp8height * wp8height)) / 4.5f);
		}

	return wp8dpi;		

#	else
	return DEFAULT_MONITOR_DPI;
#	endif

#endif
	}

#if ( UNITY_WP8 || UNITY_ANDROID || UNITY_IPHONE || UNITY_BLACKBERRY ) && !UNITY_EDITOR
	static private float	wp8dpi		= 0;
	static private int		wp8width	= 0;
	static private int		wp8height	= 0;
#endif


// --------------------
/// \brief Get Screen Emulation Rectangle.
///
/// When Screen Emulation is turned off, full screen rectangle will be returned.
/// This function can be used to adjust 
// --------------------
public Rect GetScreenEmuRect(
	bool	viewportRect = false		///< When set to true, returned rect will be relative to bottom-left corner of the screen (as used by Camera.pixelRect). When false, rect will be relative to top-left corner (as used by GUI).  
	)
	{
#if ENABLE_SCREEN_EMU

	if (this.screenEmuOn)
		{
		Rect r = new Rect(
			this.screenEmuCurOfs.x, 
			this.screenEmuCurOfs.y, 
			this.screenEmuCurWidth, 	
			this.screenEmuCurHeight);
			
		if (viewportRect)
			{
			r.y = Screen.height - (r.y + r.height);
			}

		return r;
		}
#endif

	return new Rect(0,0, Screen.width, Screen.height);
	}	


// -------------
/// Return true if left-handed mode is enabled.
//---------------
public bool GetLeftHandedMode()
	{
	return this.leftHandedMode;
	}

// -------------
/// Set left-handed mode on or off.
// -------------
public void SetLeftHandedMode(bool enableLeftedHandMode)
	{
	if (this.leftHandedMode != enableLeftedHandMode)
		{
		this.leftHandedMode = enableLeftedHandMode;
		this.SetLayoutDirtyFlag();
		}
	}





	
/// \}



// -------------------------
/// \name Mask Area Methods
///
///	Mixing touch input and Unity's GUI can be problematic, 
///	because Unity sends touch events even if they were already used to control a GUI element 
///	(for example - when pressing a GUI button).\n
///	To overcome this problem, you can employ <b>Mask Areas</b>.
///	Begin by calling ResetMaskAreas() to clear the list. 
///	Then add all screen rectangles occupied by standard GUI by calling AddMaskArea().
///	For best performance try to minimize the number of masked areas, for example try passing a combined rect of group of closely placed GUI elements.\n 
///	Mask Areas should be redefined whenever LayoutChanged() returns <b>true</b> and it's best tdo do it before any input handling code.\n
///	\note ResetController() does not remove any of the mask areas.\n 


// -------------------------
/// \{
	
// ---------------
/// Remove all specified mask areas
// ---------------
public void ResetMaskAreas()
	{
	if (this.maskAreas == null)
		this.maskAreas = new List<Rect>(8);
	else 
		this.maskAreas.Clear();
	}

	
// -----------------
/// Add new mask rectangle 
// -----------------	
public void AddMaskArea(Rect r)
	{
	if (this.maskAreas == null)
		this.maskAreas = new List<Rect>(8);

	this.maskAreas.Add(r);
	}

/// \}
		


	


	
// --------------------------
/// \name Stick and Zone Query
// --------------------------- 
/// \{

// -------------------
// Stick functions ---
// -------------------

// ---------------------
/// Get Number of Sticks.
// ---------------------
public int StickCount
	{
	get { return this.sticks.Length; }
	}


// ---------------------
/// Get Number of Sticks.
// ---------------------
public int GetStickCount()
	{
	return this.sticks.Length;
	}

	
// ---------------------
/// Get Touch Zone ID by name
// ---------------------
public int GetStickId(
	string	name		///< Name of the stick (case-insensitive)
	)
	{
	return this.GetTouchableArrayElemId(this.sticks, name);
	}


// --------------------
/// Get Touch Zone object reference by ID. Returns dummy TouchStick on failure.
// -------------------- 
public TouchStick GetStick(
	int 	id					///< Touch zone ID 
	)
	{
	if ((id < 0) || (this.sticks == null) || (id >= this.sticks.Length))
		return this.GetBlankStick();

	return this.sticks[id];
	}
	
// --------------------
/// Get touch zone object reference by name. Returns dummy TouchStick on failure.
// -------------------- 
public TouchStick GetStick(
	string 	name				///< Name of the touch stick (case-insensitive)
	)
	{
	return this.GetStick(this.GetStickId(name));
	}


// --------------------
/// Get Touch Zone object reference by ID. Returns null on failure.
// -------------------- 
public TouchStick GetStickOrNull(
	int 	id					///< Touch zone ID 
	)
	{
	if ((id < 0) || (this.sticks == null) || (id >= this.sticks.Length))
		return null;

	return this.sticks[id];
	}


	
// --------------------
/// Get touch zone object reference by name. Returns null on failure.
// -------------------- 
public TouchStick GetStickOrNull(
	string 	name				///< Name of the touch stick (case-insensitive)
	)
	{
	return this.GetStickOrNull(this.GetStickId(name));
	}
	





// ---------------------
private TouchStick GetBlankStick()
	{
	if (this.blankStick != null)
		return this.blankStick;
		
	this.blankStick = new TouchStick();
		
	this.blankStick.Init(this);
	this.blankStick.OnReset();

	this.blankStick.name = "BLANK-STICK";		

	return this.blankStick;
	}




// ---------------------------
// Zone Access ---------------
// ---------------------------

// ---------------------
/// Get Number of Touch Zones.
// ---------------------
public int ZoneCount
	{
	get { return this.touchZones.Length; }
	}


// --------------------
/// Get Number of Touch Zones.
// ---------------------
public int GetZoneCount()
	{
	return this.touchZones.Length;
	}

	

// ---------------------
/// Get Touch Zone ID by name
// ---------------------
public int GetZoneId(string name)
	{
	return this.GetTouchableArrayElemId(this.touchZones, name);
	}

// --------------------
/// Get Touch Zone reference by ID. \nReturns dummy TouchZone on failure.
// -------------------- 
public TouchZone GetZone(
	int 	id					///< Touch zone ID 
	)
	{
	if ((id < 0) || (this.touchZones == null) || (id >= this.touchZones.Length))
		return this.GetBlankZone();

	return this.touchZones[id];
	}

	
// --------------------
/// Get touch zone reference by name. \nReturns dummy TouchZone on failure.
// -------------------- 
public TouchZone GetZone(
	string 	name				///< Name of the touch zone (case-insensitive)
	)
	{
	return this.GetZone(this.GetZoneId(name));
	}


// --------------------
/// Get Touch Zone reference by ID. \nReturns null on failure.
// -------------------- 
public TouchZone GetZoneOrNull(
	int 	id					///< Touch zone ID 
	)
	{
	if ((id < 0) || (this.touchZones == null) || (id >= this.touchZones.Length))
		return (null);

	return this.touchZones[id];
	}


// --------------------
/// Get touch zone reference by name. \nReturns null on failure.
// -------------------- 
public TouchZone GetZoneOrNull(
	string 	name				///< Name of the touch zone (case-insensitive)
	)
	{
	return this.GetZoneOrNull(this.GetZoneId(name));
	}



// ---------------------
private TouchZone GetBlankZone()
	{
	if (this.blankZone != null)
		return this.blankZone;
		
	this.blankZone = new TouchZone();
		
	this.blankZone.Init(this);
	this.blankZone.OnReset();

	this.blankZone.name = "NULL";		

	return this.blankZone;
	}

	


// ---------------------------
// Generic Control Access ----
// ---------------------------

// ---------------------
/// Get Number of Controls (Sticks and Zones).
// ---------------------
public int ControlCount
	{
	get { return ((this.touchables == null) ? 0 : this.touchables.Count); }
	}


// --------------------
/// Get Number of Controls (Sticks and Zones).
// ---------------------
public int GetControlCount()
	{
	return ((this.touchables == null) ? 0 : this.touchables.Count);
	}

	


// --------------------
/// Get Control reference. \nReturns NULL on failure.
// -------------------- 
public TouchableControl GetControl(
	int 	id					///< Control's slot
	)
	{
	if ((id < 0) || (this.touchables == null) || (id >= this.touchables.Count))
		return null;

	return this.touchables[id];
	}



/// \}




// -----------------------------
/// \name Unity's Input class replacement functions.
/// Caution: Functions below can be expensive when used on complex controllers.
/// Please consider using TouchStick and TouchZone classes directly. 
// -----------------------------
/// \{
		

/// \cond

// ----------------------	
public float GetAxisEx(
	string		name,		///< Axis name
	out bool	axisSupported	///< Will be set to true, when the axis is supported.
	)
	{
	axisSupported = false;
	float v = 0;

	for (int i = 0; i < this.sticks.Length; ++i)
		{
		bool stickSupported = false;
		float stickv = this.sticks[i].GetAxisEx(name, out stickSupported);
		if (stickSupported)
			{
			axisSupported = true;
			v += stickv;
			}			
		}

	for (int i = 0; i < this.touchZones.Length; ++i)
		{
		bool zoneSupported = false;
		float zonev = this.touchZones[i].GetAxisEx(name, out zoneSupported);
		if (zoneSupported)
			{
			axisSupported = true;
			v += zonev;
			}			
		}
	
	return v;
	}

/// \endcond


// ----------------
/// \brief Simulates Input.GetAxis().
/// 
/// \note If there are more than one controls with the same axis name, 
/// returned value will be the sum of them all.  
// ----------------

public float GetAxis(
	string		name		///< Axis name
	)
	{
	bool supported = false;
	return this.GetAxisEx(name, out supported);
/*
	float v = 0;
	for (int i = 0; i < this.sticks.Length; ++i)
		v += this.sticks[i].GetAxis(name);
		
	for (int i = 0; i < this.touchZones.Length; ++i)
		v += this.touchZones[i].GetAxis(name);
	
	return v;
*/
	}

// ----------------
/// Alias for GetAxis()
// ----------------
public float GetAxisRaw(
	string	name	///< Axis name
	)
	{
	return this.GetAxis(name);
	}
	
	


// --------------
/// Returns true if at least one of controls with matching getButtonName is pressed. 
// --------------
public bool GetButton(
	string buttonName		///< Axis name
	)
	{
	bool buttonSupported = false;
	return this.GetButtonEx(buttonName, out buttonSupported);

//	for (int i = 0; i < this.touchZones.Length; ++i)
//		{
//		if (this.touchZones[i].GetButton(axisName))
//			return true;
//		}
//
//	return false;
	}


// --------------
/// Returns true if at least one of controls with matching getButtonName has just been pressed. 
// --------------
public bool GetButtonDown(string buttonName)
	{
	bool buttonSupported = false;
	return this.GetButtonDownEx(buttonName, out buttonSupported);

//	for (int i = 0; i < this.touchZones.Length; ++i)
//		{
//		if (this.touchZones[i].GetButtonDown(axisName))
//			return true;
//		}
//
//	return false;
	}
	
// --------------
/// Returns true if at least one of controls with matching getButtonName has just been released. 
// --------------
public bool GetButtonUp(string buttonName)
	{
	bool buttonSupported = false;
	return this.GetButtonUpEx(buttonName, out buttonSupported);

//	for (int i = 0; i < this.touchZones.Length; ++i)
//		{
//		if (this.touchZones[i].GetButtonUp(axisName))
//			return true;
//		}
//
//	return false;
	}




/// \cond

// --------------
public bool GetButtonEx(
	string 		buttonName,		///< Axis name
	out bool 	buttonSupported
	)
	{
	buttonSupported = false;

	for (int i = 0; i < this.sticks.Length; ++i)
		{
		bool stickButtonSupported = false;
		bool stickOn = this.sticks[i].GetButtonEx(buttonName, out stickButtonSupported);
		if (stickButtonSupported)
			buttonSupported = true;	
		if (stickOn)
			return true;
		}

	for (int i = 0; i < this.touchZones.Length; ++i)
		{
		bool zoneButtonSupported = false;
		bool zoneOn = this.touchZones[i].GetButtonEx(buttonName, out zoneButtonSupported);
		if (zoneButtonSupported)
			buttonSupported = true;	
		if (zoneOn)
			return true;
		}

	return false;
	}

// --------------
public bool GetButtonDownEx(
	string 		buttonName,		///< Axis name
	out bool 	buttonSupported
	)
	{
	buttonSupported = false;

	for (int i = 0; i < this.sticks.Length; ++i)
		{
		bool stickButtonSupported = false;
		bool stickOn = this.sticks[i].GetButtonDownEx(buttonName, out stickButtonSupported);
		if (stickButtonSupported)
			buttonSupported = true;	
		if (stickOn)
			return true;
		}

	for (int i = 0; i < this.touchZones.Length; ++i)
		{
		bool zoneButtonSupported = false;
		bool zoneOn = this.touchZones[i].GetButtonDownEx(buttonName, out zoneButtonSupported);
		if (zoneButtonSupported)
			buttonSupported = true;	
		if (zoneOn)
			return true;
		}

	return false;
	}


// --------------
public bool GetButtonUpEx(
	string 		buttonName,		///< Axis name
	out bool 	buttonSupported
	)
	{
	buttonSupported = false;

	for (int i = 0; i < this.sticks.Length; ++i)
		{
		bool stickButtonSupported = false;
		bool stickOn = this.sticks[i].GetButtonUpEx(buttonName, out stickButtonSupported);
		if (stickButtonSupported)
			buttonSupported = true;	
		if (stickOn)
			return true;
		}

	for (int i = 0; i < this.touchZones.Length; ++i)
		{
		bool zoneButtonSupported = false;
		bool zoneOn = this.touchZones[i].GetButtonUpEx(buttonName, out zoneButtonSupported);
		if (zoneButtonSupported)
			buttonSupported = true;	
		if (zoneOn)
			return true;
		}

	return false;
	}


/// \endcond
	

	


// --------------
/// Returns true if at least one of controls with matching GetKey key code is pressed. 
// --------------
public bool GetKey(KeyCode keyCode)
	{
	bool keySupported = false;
	return this.GetKeyEx(keyCode, out keySupported);
	}

// --------------
/// Returns true if at least one of controls with matching GetKey key code has just been pressed. 
// --------------
public bool GetKeyDown(KeyCode keyCode)
	{
	bool keySupported = false;
	return this.GetKeyDownEx(keyCode, out keySupported);
	}

// --------------
/// Returns true if at least one of controls with matching GetKey key code has just been released. 
// --------------
public bool GetKeyUp(KeyCode keyCode)
	{
	bool keySupported = false;
	return this.GetKeyUpEx(keyCode, out keySupported);
	}




/// \cond

// --------------
public bool GetKeyEx(KeyCode keyCode, out bool keySupported)
	{
	keySupported = false;

	for (int i = 0; i < this.sticks.Length; ++i)
		{
		bool stickKeySupported = false;
		bool stickOn = this.sticks[i].GetKeyEx(keyCode, out stickKeySupported);
		if (stickKeySupported)
			keySupported = true;	
		if (stickOn)
			return true;
		}

	for (int i = 0; i < this.touchZones.Length; ++i)
		{
		bool zoneKeySupported = false;
		bool zoneOn = this.touchZones[i].GetKeyEx(keyCode, out zoneKeySupported);
		if (zoneKeySupported)
			keySupported = true;	
		if (zoneOn)
			return true;
		}

	return false;
	}

// --------------
public bool GetKeyDownEx(KeyCode keyCode, out bool keySupported)
	{
	keySupported = false;

	for (int i = 0; i < this.sticks.Length; ++i)
		{
		bool stickKeySupported = false;
		bool stickOn = this.sticks[i].GetKeyDownEx(keyCode, out stickKeySupported);
		if (stickKeySupported)
			keySupported = true;	
		if (stickOn)
			return true;
		}

	for (int i = 0; i < this.touchZones.Length; ++i)
		{
		bool zoneKeySupported = false;
		bool zoneOn = this.touchZones[i].GetKeyDownEx(keyCode, out zoneKeySupported);
		if (zoneKeySupported)
			keySupported = true;	
		if (zoneOn)
			return true;
		}

	return false;
	}


// --------------
public bool GetKeyUpEx(KeyCode keyCode, out bool keySupported)
	{
	keySupported = false;

	for (int i = 0; i < this.sticks.Length; ++i)
		{
		bool stickKeySupported = false;
		bool stickOn = this.sticks[i].GetKeyUpEx(keyCode, out stickKeySupported);
		if (stickKeySupported)
			keySupported = true;	
		if (stickOn)
			return true;
		}

	for (int i = 0; i < this.touchZones.Length; ++i)
		{
		bool zoneKeySupported = false;
		bool zoneOn = this.touchZones[i].GetKeyUpEx(keyCode, out zoneKeySupported);
		if (zoneKeySupported)
			keySupported = true;	
		if (zoneOn)
			return true;
		}

	return false;
	}


/// \endcond
	
	
// --------------
/// Input.GetMouseButton() replacement.
// --------------
public bool GetMouseButton(
	int i ///< Mouse Button Id (0, 1 or 2)
	)
	{
	return this.GetKey(
		(i == 0) ? KeyCode.Mouse0 : 
		(i == 1) ? KeyCode.Mouse1 : 
		(i == 2) ? KeyCode.Mouse2 : KeyCode.None);
	}

	
// --------------
/// Input.GetMouseButtonDown() replacement.
// --------------
public bool GetMouseButtonDown(
	int i ///< Mouse Button Id (0, 1 or 2)
	)
	{
	return this.GetKeyDown(
		(i == 0) ? KeyCode.Mouse0 : 
		(i == 1) ? KeyCode.Mouse1 : 
		(i == 2) ? KeyCode.Mouse2 : KeyCode.None);
	}

// --------------
/// Input.GetMouseButtonUp() replacement.
// --------------
public bool GetMouseButtonUp(
	int i ///< Mouse Button Id (0, 1 or 2)
	)
	{
	return this.GetKeyUp(
		(i == 0) ? KeyCode.Mouse0 : 
		(i == 1) ? KeyCode.Mouse1 : 
		(i == 2) ? KeyCode.Mouse2 : KeyCode.None);
	}



// ----------------
/// \brief Input.mousePosition replacement.
///
/// If none of touch zones of this controller is enabled to emulate mouse, 
/// static screen-center position will be returned.
// ----------------
public Vector2 GetMousePos()
	{
	return this.emuMousePos;
	}



/*
// -------------------------
/// \brief Shortcut for <b>GetStick(stickId).GetVec().x</b>.
/// 
/// See also \ref GetStick(), \ref TouchStick.GetVec().
// -------------------------
public float GetHorzAxis(	
	int stickId		///< Touch Stick ID
	)
	{
	return this.GetStick(stickId).GetVec().x;
	}


// -------------------------
/// \brief Shortcut for <b>GetStick(stickId).GetVec().y</b>.
/// 
/// See also \ref GetStick(), \ref TouchStick.GetVec().
// -------------------------
public float GetVertAxis(	
	int stickId		///< Touch Stick ID
	)
	{
	return this.GetStick(stickId).GetVec().y;
	}


// -------------------------
/// \brief Shortcut for <b>GetStick(stickId).GetVec()</b>.
/// 
/// See also \ref GetStick(), \ref TouchStick.GetVec().
// -------------------------
public Vector2 GetStickVec(	
	int stickId		///< Touch Stick ID
	)
	{
	return this.GetStick(stickId).GetVec();
	}

	

// -------------------
/// \brief
/// Simulates Unity's <b>Input.GetKey()</b> using stick's digital state.
/// Accepted KeyCodes: W, S, A, D and arrow keys.
/// 
/// Shortcut for <b>GetStick(stickId).GetKey(key)</b>;\n
/// See also: \ref GetStick(), \ref TouchStick.GetKey().
// -------------------
public bool GetStickKey(
	int 	stickId,	///< Touch Stick ID
	KeyCode key			///< Virtual KeyCode to check.
	)
	{
	return this.GetStick(stickId).GetKey(key);
	}

		

// -------------------
/// \brief
/// Simulates Unity's <b>Input.GetKeyDown()</b> using stick's digital state.
/// Accepted KeyCodes: W, S, A, D and arrow keys.
/// 
/// Shortcut for <b>GetStick(stickId).GetKeyDown(key)</b>;\n
/// See also: \ref GetStick(), \ref TouchStick.GetKeyDown().
// -------------------
public bool GetStickKeyDown(
	int 	stickId,	///< Touch Stick ID
	KeyCode key			///< Virtual KeyCode to check.
	)
	{
	return this.GetStick(stickId).GetKeyDown(key);
	}

// -------------------
/// \brief
/// Simulates Unity's <b>Input.GetKeyUp()</b> using stick's digital state.
/// Accepted KeyCodes: W, S, A, D and arrow keys.
/// 
/// Shortcut for <b>GetStick(stickId).GetKeyUp(key)</b>;\n
/// See also: \ref GetStick(), \ref TouchStick.GetKeyUp().
// -------------------
public bool GetStickKeyUp(
	int 	stickId,	///< Touch Stick ID
	KeyCode key			///< Virtual KeyCode to check.
	)
	{
	return this.GetStick(stickId).GetKeyUp(key);
	}




// ----------------------------
/// \brief Shortcut for <b>GetZone(zoneId).UniPressed()</b>.
/// 
/// See also: \ref GetZone(), \ref TouchZone.UniPressed().
// ----------------------------
public bool GetKey(
	int zoneId		///< Touch Zone ID
	)
	{
	return this.GetZone(zoneId).UniPressed();
	}
		
// ----------------------------
/// \brief Shortcut for <b>GetZone(zoneId).JustUniPressed()</b>.
/// 
/// See also: \ref GetZone(), \ref TouchZone.JustUniPressed().
// ----------------------------
public bool GetKeyDown(
	int zoneId		///< Touch Zone ID
	)
	{
	return this.GetZone(zoneId).JustUniPressed();
	}		

// ----------------------------
/// \brief Shortcut for <b>GetZone(zoneId).JustUniReleased()</b>. 
/// 
/// See also: \ref GetZone(), \ref TouchZone.JustUniReleased().
// ----------------------------
public bool GetKeyUp(
	int zoneId		///< Touch Zone ID
	)
	{
	return this.GetZone(zoneId).JustUniReleased();
	}
	
*/	

/// \}


		
// ---------------------
/// \name Utils 
// ---------------------
/// \{

// ----------------------
/// Pick collider under given screen position.		
// ----------------------
static public Collider PickCollider(
	Vector2 	screenPos,		///< Screen position in pixels, relative to top-left corner. 
	Camera 		cam, 			///< Camera.
	LayerMask 	layerMask		///< Layer Mask to check.	
	)
	{
	Ray ray = cam.ScreenPointToRay(new Vector3(screenPos.x, Screen.height - screenPos.y, 0));
	RaycastHit	rayHit;
	float rad = 0.1f;

	if (!Physics.SphereCast(ray, rad, out rayHit, Mathf.Infinity, layerMask))
		return null;

	return rayHit.collider; 
	}



/// \}



/// \cond 

//#if UNITY_EDITOR
	


// ------------------
private void InitIfNeeded()
	{

	if (!this.initialized ||
		this.contentDirtyFlag)
		{	
//#if UNITY_EDITOR
//		if (EditorApplication.isPlaying)
//			Debug.LogError("Controller ["+this.name+"] wasn't properly initialized!! Enable \'Automatic Mode\' or do it manually."); 
//#endif
		this.InitController();
		}
	
#if UNITY_EDITOR
	if ((this.touchables 	== null) ||
		//(this.buttons 		== null) ||
		(this.sticks 		== null) ||	
		(this.touchZones	== null) ||
		(this.layoutBoxes 	== null) ||
		((
		//this.buttons.Length + 
		this.sticks.Length + this.touchZones.Length) != 
			this.touchables.Count)
		)
		{
		this.InitController();
		}
#endif
	}	

//#endif

	

// ---------------------
// MonoBehaviour Stuff.
// ---------------------

private void OnEnable()
	{
//#if UNITY_EDITOR
	//Debug.Log("Enanle("+this.name+")");
	this.InitIfNeeded();

//#endif
		  
	this.ResetLayoutBoxes();
		 
#if UNITY_EDITOR
	this.LoadScreenEmuConfig();

	//if (!EditorApplication.isPlayingOrWillChangePlaymode)
		this.UpgradeIfNeeded();

#endif
	}
	


// -------------------
static public bool IsSupported()
	{
	return (((SystemInfo.deviceType == DeviceType.Handheld) && Input.multiTouchEnabled) ||
		(Application.platform == RuntimePlatform.IPhonePlayer));
	}

	

// ------------------
private void Awake()
	{
//#if UNITY_EDITOR
	this.InitIfNeeded();

//#endif


	// Disable when there's no touch-screen.

	if (this.disableWhenNoTouchScreen && !TouchController.IsSupported())
		{
		
#if UNITY_EDITOR
		//Debug.Log("Disablig controller because there's no touch-screen.");
#else
		this.enabled = false;
#endif
		}	
	}
	

// -----------------
private void OnDestroy()
	{
	if (CFInput.ctrl == this)
		CFInput.ctrl = null;
	}	


// ----------------
private void Start()
	{
#if UNITY_EDITOR
	this.LoadScreenEmuConfig();
#endif

	if (this.automaticMode && !this.initialized)
		this.InitController();
	

	// Set as active...

	if (this.autoActivate)
		{
#if UNITY_EDITOR
	//Debug.Log("Frame[" + Time.frameCount + "] CFInput.ctrl = [" + this.name + "]!!");
#endif	
		CFInput.ctrl = this;
		}
	}

	
// ---------------
private void Update()
	{
	this.InitIfNeeded();


#if UNITY_EDITOR
	if (!EditorApplication.isPlayingOrWillChangePlaymode)
		{
		// Safety Update...

		this.OnEditorUpdate();			


		this.UpdateControllerInternal();
		return;
		}
#endif

	if (this.automaticMode)
		{
		this.PollControllerInternal();
	
		//if (!this.fixedUpdateMode)
			this.UpdateControllerInternal();
		}
	}

// ------------
private void OnGUI()
	{
#if UNITY_EDITOR
	if (!EditorApplication.isPlayingOrWillChangePlaymode)
		{
		this.InitIfNeeded();
		this.LayoutIfDirty();

		this.DrawControllerGUIInternal();
		return;
		}

#endif
		

	if (this.automaticMode && !this.manualGui)
		{
		this.DrawControllerGUIInternal();
		}
	}

	
	

// -------------------
private void OnApplicationPause(bool pause)
	{
	// Walk-around for Unity Android bug!

	this.releaseTouchesFlag = true;
	//this.ResetJoy();	
	}
	
	
/// \endcond




// -----------------
// Hide documantation
/// \cond 
// -----------------

// -------------
public void SetLayoutDirtyFlag()
	{
	this.layoutDirtyFlag = true;
	}


// --------------
public void SetContentDirtyFlag()
	{
	this.contentDirtyFlag = true;
	}
	


// ------------------
private void LayoutIfDirty()
	{		
	if (this.layoutDirtyFlag ||
		(Screen.width 	!= this.screenWidth) ||
		(Screen.height 	!= this.screenHeight))
		this.Layout();
	}



// -------------------
private int GetTouchableArrayElemId(TouchableControl[] carr, string name)
	{
	if (carr == null) 
		return -1;

	for (int i = 0; i < carr.Length; ++i)
		{
		if (name.Equals(carr[i].name, System.StringComparison.OrdinalIgnoreCase))
			return i;
		}

	return -1;
	}


// --------------
/// \endcond
// --------------


// ------------------
private void Layout()	
	{

#	if ENABLE_SCREEN_EMU

	this.UpdateScreenEmu();

#	endif

	this.customLayoutNeedsRebuild = true;

	//this.layoutEventWaiting	= true;
	this.releaseTouchesFlag	= true;

	this.layoutDirtyFlag 	= false;
	//this.forceLayoutChangeEventFlag = false;


#if UNITY_WEBPLAYER && !UNITY_EDITOR
	this.UpdateWebDPI();
#endif



	this.screenWidth 	= Screen.width;
	this.screenHeight 	= Screen.height;

#if ENABLE_SCREEN_EMU

	// Recalculate monitor's DPI...
		
	if (Screen.dpi != 0)
		{
		this.monitorDpi = Screen.dpi;
		}
	else
		{
		this.monitorDpi = DEFAULT_MONITOR_DPI;
		
		float diagonalInPixels = Mathf.Sqrt(
			(Screen.currentResolution.width * Screen.currentResolution.width) + 
			(Screen.currentResolution.height * Screen.currentResolution.height));
	
		if ((diagonalInPixels > 0) && (this.monitorDiagonal > 0))
			this.monitorDpi = (diagonalInPixels / this.monitorDiagonal);
		}
#endif

		

	//Debug.Log("MONITOR DPI : " + this.monitorDpi);


	this.fingerBufferRadPx = Mathf.Max(1, 0.5f * (this.fingerBufferCm * this.GetDPCM()));

		
	// Calculate max tap distance - at least 1px, no more than 1/3 of the screen

	this.touchTapMaxDistPx = Mathf.Clamp((this.touchTapMaxDistCm * this.GetDPCM()), 
		1, (Mathf.Min(this.GetScreenWidth(), this.GetScreenHeight()) * 0.3f));	
		
	
	this.pinchMinDistPx = Mathf.Clamp((this.pinchMinDistCm * this.GetDPCM()), 
		1, (Mathf.Min(this.GetScreenWidth(), this.GetScreenHeight()) * 0.3f));	
		
	this.twistSafeFingerDistPx = Mathf.Clamp((this.twistSafeFingerDistCm * this.GetDPCM()), 
		1, (Mathf.Min(this.GetScreenWidth(), this.GetScreenHeight()) * 0.3f));	

		



	// Reset layout boxes...

	this.ResetLayoutBoxes();

	// Build layout box content...
		
	foreach (TouchableControl c in this.touchables)	
		{

		c.OnLayoutAddContent();
		}

	// Finalize layout boxes...
		
	foreach (LayoutBox b in this.layoutBoxes)
		b.ContentFinalize();
		

	// Layout controls...

	if (this.touchables != null)
		{
		foreach (TouchableControl c in this.touchables)
			c.OnLayout();
		}
		

#if UNITY_EDITOR
	if (!EditorApplication.isPlayingOrWillChangePlaymode)
		this.OnGlobalColorChange();
#endif

	
	}

	

// -----------------
private void ResetLayoutBoxes()
	{
	if ((this.layoutBoxes == null) || (this.layoutBoxes.Length != LayoutBoxCount) ||
		(this.layoutBoxes[0] == null))
		{
		this.layoutBoxes = new TouchController.LayoutBox[LayoutBoxCount];


		for (int i = 0; i < this.layoutBoxes.Length; ++i)
			{
			switch (i)
				{
				case 0 : this.layoutBoxes[i] = new LayoutBox(
					"Full-Screen",	0.0f, 0.0f, 1.0f, 1.0f, LayoutAnchor.TOP_LEFT); 
					break;

				case 1 : this.layoutBoxes[i] = new LayoutBox(
					"Right-Half",	0.5f, 0.0f, 0.5f, 1.0f, LayoutAnchor.MID_RIGHT); 
					break;

				case 2 : this.layoutBoxes[i] = new LayoutBox(
					"Left-Half",	0.0f, 0.0f, 0.5f, 1.0f, LayoutAnchor.MID_LEFT); 
					break;

				case 3 : this.layoutBoxes[i] = new LayoutBox(
					"Top-Half",		0.0f, 0.0f, 1.0f, 0.5f, LayoutAnchor.TOP_CENTER); 
					break;

				case 4 : this.layoutBoxes[i] = new LayoutBox(
					"Bottom-Half",	0.0f, 0.5f, 1.0f, 0.5f, LayoutAnchor.BOTTOM_CENTER);
					break;

				case 5 : this.layoutBoxes[i] = new LayoutBox(
					"Bottom-Right-Qrtr",	0.5f, 0.5f, 0.5f, 0.5f, LayoutAnchor.BOTTOM_RIGHT);
					break;

				case 6 : this.layoutBoxes[i] = new LayoutBox(
					"Bottom-Left-Qrtr",		0.0f, 0.5f, 0.5f, 0.5f, LayoutAnchor.BOTTOM_LEFT);
					break;

				case 7 : this.layoutBoxes[i] = new LayoutBox(
					"Top-Right-Qrtr",		0.5f, 0.0f, 0.5f, 0.5f, LayoutAnchor.TOP_RIGHT);
					break;

				case 8 : this.layoutBoxes[i] = new LayoutBox(
					"Top-Left-Qrtr",		0.0f, 0.0f, 0.5f, 0.5f, LayoutAnchor.TOP_LEFT);
					break;
	
				default : this.layoutBoxes[i] = new LayoutBox(
					"User" + i.ToString("00"), 0,0, 1,1, LayoutAnchor.TOP_LEFT);
					break;
		
				}	
			}
				
		}

	// Reset content...

	foreach (LayoutBox b in this.layoutBoxes)
		{
		b.SetController(this);
		b.ResetContent();
		}
	}

	
	

// --------------------------
private void PollDefaultTouchEvents()
	{


	int touchCount = Input.touchCount;

#	if EMULATE_TOUCHES_BY_MOUSE
	bool mouseTouchesMode = (touchCount == 0);
	if (mouseTouchesMode)
		touchCount = 2;
#	endif

	// Handle real (and not-real) touch events ...

	for (int ti = 0; ti < touchCount; ++ti)
		{
		TouchPhase	touchPhase 	= TouchPhase.Stationary;
		int			touchId		= -1;
		Vector2		touchPos	= Vector2.zero;


#if EMULATE_TOUCHES_BY_MOUSE
		
		const int MOUSE_FINGER_ID_OFS = 1000;				

		// Mouse emulated touch...

		//if (ti >= touchCount)
		if (mouseTouchesMode)
			{
			int mouseBtnId = (ti); // - touchCount);

			touchId 	= MOUSE_FINGER_ID_OFS + mouseBtnId;
			touchPos	= new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
			
			bool prevButtonState = false;
			bool curButtonState = Input.GetMouseButton(mouseBtnId);

			if (mouseBtnId == 0)
				{		
				prevButtonState = this.debugPrevFrameMouseButton0;
				this.debugPrevFrameMouseButton0 = curButtonState;
				}
			else
				{
				prevButtonState = this.debugPrevFrameMouseButton1;
				this.debugPrevFrameMouseButton1 = curButtonState;
				}

//if (!prevButtonState && curButtonState) Debug.Log("FR["+Time.frameCount+"] MOUSE ["+mouseBtnId+"] DOWN! SYS:"+ Input.GetMouseButtonDown(0) + ", " + Input.GetMouseButtonDown(1));
	
			if (!prevButtonState && curButtonState) 		//Input.GetMouseButtonDown(mouseBtnId))
				touchPhase = TouchPhase.Began;
			else if (prevButtonState && !curButtonState)	//Input.GetMouseButtonUp(mouseBtnId))
				touchPhase = TouchPhase.Ended;
			else if (curButtonState)						//Input.GetMouseButton(mouseBtnId))
				touchPhase = TouchPhase.Moved;
			else 
				continue;
	
			if (mouseBtnId == 0)
				this.debugFirstTouchPos = touchPos;
			else
				touchPos = this.debugSecondTouchPos;
				
			}			
		else
			{
		
#else
		if (true)
			{
#endif
		// Real touch...
			

			Touch touch = Input.GetTouch(ti);
			
			touchPhase = touch.phase;
			touchId		= touch.fingerId;
			touchPos	= new Vector2(touch.position.x, Screen.height - touch.position.y);

		}

//#endif		


		// Handle the evnts...

		switch (touchPhase)
			{
			case TouchPhase.Began :
				this.InternalOnTouchStart(touchId, touchPos);
				break;
					
			case TouchPhase.Ended :
			case TouchPhase.Canceled :
				this.InternalOnTouchEnd(touchId, touchPos);
				break;

			case TouchPhase.Moved :
			case TouchPhase.Stationary :
				this.InternalOnTouchMove(touchId, touchPos);
				break;
			}
		}
	}
		

// ---------------------
private void PollControllerInternal()
	{
/*
	//this.firstPostPollUpdate = true;

	// Check resulution change...
	
	this.LayoutIfDirty();
		



	// Prepare controls to poll...

	foreach (TouchableControl c in this.touchables)
		c.OnPrePoll();
		
		
	// Release touches when needed...

	if (this.releaseTouchesFlag)
		{
		foreach (TouchableControl c in this.touchables)
			c.ReleaseTouches();

		this.releaseTouchesFlag = false;
		}



//#if false
#if EMULATE_TOUCHES_BY_MOUSE
		
	// Emulate second touch...

	Vector2 mousePos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
	Vector2 mouseDelta = mousePos - this.debugPrevFrameMousePos;	
	this.debugPrevFrameMousePos = mousePos;

	if (Input.GetMouseButton(2))
		this.debugSecondTouchPos = mousePos;

	else if (Input.GetMouseButton(1))
		{
		if (Input.GetKey(this.debugSecondTouchTwistModeKey))
			this.debugSecondTouchPos += -mouseDelta;
			
		else if (Input.GetKey(this.debugSecondTouchPinchModeKey))
			this.debugSecondTouchPos += new Vector2(-mouseDelta.x, mouseDelta.y);

		else if (Input.GetKey(this.debugSecondTouchDragModeKey))
			this.debugSecondTouchPos += mouseDelta;
	
		this.debugSecondTouchPos.x = Mathf.Clamp(this.debugSecondTouchPos.x, 0, Screen.width); 
		this.debugSecondTouchPos.y = Mathf.Clamp(this.debugSecondTouchPos.y, 0, Screen.height); 
		}
			

			
// # else
#		endif
			

	if (TouchController.eventProvider == null)
		this.PollDefaultTouchEvents();


	// Post-poll checkup...

	foreach (TouchableControl c in this.touchables)
		c.OnPostPoll();
*/
	}

		

/// \cond
		
// ------------------
public bool InternalOnTouchStart(int touchId, Vector2 touchPos)
	{
	// Check masked areas...

	if (this.maskAreas != null)
		{
		for (int i = 0; i < this.maskAreas.Count; ++i)
			{
			if (this.maskAreas[i].Contains(touchPos))
				{
				return false;
				}
			}
		}

	if (this.disableAll)
		return false;

	// Go through all controls and handle event...

	bool sharedEventHandled = false;

	for (int si = 0; si < MAX_EVENT_SHARE_COUNT; ++si)
		{

		TouchableControl 	closestHit 		= null;
		HitTestResult		closestResult = new HitTestResult(false);

		for (int ci = 0; ci < this.touchables.Count; ++ci)
			{
			if ((si > 0) && !this.touchables[ci].acceptSharedTouches)
				continue;
			if ((closestHit != null) && (closestResult.prio > this.touchables[ci].prio))
				continue;
				
			HitTestResult result = this.touchables[ci].HitTest(touchPos, touchId);
				
			if (!result.hit)
				continue;

			//if (hitDist < HIT_TEST_MIN_DIST)
			//	continue;

			if ((closestHit == null) || result.IsCloserThan(closestResult))
				//(this.touchables[i].prio > closestHitPrio) ||
				//(hitDist < closestHitDist))
				{
				closestHit 		= this.touchables[ci];
				closestResult 	= result;
				//closestHitPrio 	= closestHit.prio;
				//closestHitDist	= hitDist;
				}
			}
			
		if (closestHit != null)
			{
			EventResult result = closestHit.OnTouchStart(touchId, touchPos);
			if (result == EventResult.SHARED)
				{
				sharedEventHandled = true;
				continue;	// Share this touch...
				}
			else
				return (result == EventResult.HANDLED);
			}
		}

	return sharedEventHandled;
	}

		

// ----------------
public bool InternalOnTouchEnd(int touchId, Vector2 touchPos)
	{
	bool eventHandled = false;

	for (int i = 0; i < this.touchables.Count; ++i)
		{
		if (this.touchables[i].OnTouchEnd(touchId) != EventResult.NOT_HANDLED)
			eventHandled = true;
		}
	
	return eventHandled;
	}


// ------------------
public bool InternalOnTouchMove(int touchId, Vector2 touchPos)
	{
	bool eventHandled = false;

	for (int i = 0; i < this.touchables.Count; ++i)
		{
		if (this.touchables[i].OnTouchMove(touchId, touchPos) != EventResult.NOT_HANDLED)
			eventHandled = true;
		}
	
	return eventHandled;
	}

/// \endcond


		

// --------------------
private void UpdateControllerInternal()
	{
	// Move time counter...		
			
	float curRealTime = Time.realtimeSinceStartup;

	this.deltaTime = (curRealTime - this.lastRealTime);
	if (this.deltaTime <= 0.0001f)
		this.deltaTime = (1.0f / 60.0f);
	
	this.invDeltaTime = (1.0f / this.deltaTime);

	this.curTime += this.deltaTime;

	this.lastRealTime = curRealTime;
			


	// Check resulution change...
	
	this.LayoutIfDirty();
		

	
		
	// Release touches when needed...

	if (this.releaseTouchesFlag)
		{
		foreach (TouchableControl c in this.touchables)
			c.ReleaseTouches();

		this.releaseTouchesFlag = false;
		}



#if EMULATE_TOUCHES_BY_MOUSE
		
	// Emulate second touch...

	Vector2 mousePos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
	Vector2 mouseDelta = mousePos - this.debugPrevFrameMousePos;	
	this.debugPrevFrameMousePos = mousePos;

	if (Input.GetMouseButton(2))
		this.debugSecondTouchPos = mousePos;

	else if (Input.GetMouseButton(1))
		{
		if (Input.GetKey(this.debugSecondTouchTwistModeKey))
			this.debugSecondTouchPos += -mouseDelta;
			
		else if (Input.GetKey(this.debugSecondTouchPinchModeKey))
			this.debugSecondTouchPos += new Vector2(-mouseDelta.x, mouseDelta.y);

		else if (Input.GetKey(this.debugSecondTouchDragModeKey))
			this.debugSecondTouchPos += mouseDelta;
	
		this.debugSecondTouchPos.x = Mathf.Clamp(this.debugSecondTouchPos.x, 0, Screen.width); 
		this.debugSecondTouchPos.y = Mathf.Clamp(this.debugSecondTouchPos.y, 0, Screen.height); 
		}
			
#		endif


	
	// Update alpha animation...

	if (this.globalAlphaTimer.Enabled)
		{
		this.globalAlphaTimer.Update(this.deltaTime);
				
		this.globalAlpha = Mathf.Lerp(this.globalAlphaStart, this.globalAlphaEnd, 
			this.globalAlphaTimer.Nt);

		if (this.globalAlphaTimer.Completed)
			this.globalAlphaTimer.Disable();			
		}			
			


	// Process default touch events if there's no external event provider...

	//if (TouchController.eventProvider == null)
		this.PollDefaultTouchEvents();
			
	

			

	if (this.touchables != null)	
		{
		// Launch Post-Poll methods....	
		
		foreach (TouchableControl c in this.touchables)
			c.OnPostPoll();


		// Launch Pre-update methods....	
		
		foreach (TouchableControl c in this.touchables)
			c.OnUpdate(); //this.firstPostPollUpdate);
		
	
		
	
		// Launch Post-update methods....
		
		//foreach (TouchableControl c in this.touchables)
		//	c.OnPostUpdate(); //this.firstPostPollUpdate);


		// Prepare for polling....	
		
		foreach (TouchableControl c in this.touchables)
			c.OnPrePoll();

		}

			

	// Debug stuff..



	// Turn-off first post-poll update flag...

	//this.firstPostPollUpdate = false;
	}

	



// ----------------
private void DrawControllerGUIInternal()
	{
#	if !DEBUG_MODE
	if (Event.current.type != EventType.Repaint)
		return;
#	endif


		
	bool initialEnable 	= GUI.enabled;
	int initialGuiDepth = GUI.depth;
	
	GUI.depth 	= this.guiDepth;
	GUI.enabled = true;				
	
#if DEBUG_MODE

	// Draw layout boxes beneeth the controls...

	if (this.debugDrawLayoutBoxes && (this.layoutBoxes != null))
		{
		foreach (LayoutBox b in this.layoutBoxes)
			b.DrawDebug();
		}

#endif

			
	// Draw the controls...

	if (this.touchables != null)
		{
		for (int i = 0; i < this.touchables.Count; ++i)
			{
			this.touchables[i].DrawGUI();
			}
		}

//#	if DEBUG_MODE
#if ENABLE_SCREEN_EMU
			
	// Draw screen emu border...

	if (this.screenEmuOn && (this.debugRectImg != null))
		{
		GUI.color = (this.screenEmuBorderShrunk ? this.screenEmuBorderBadColor :
			this.screenEmuBorderColor);
		
		float xmin = this.screenEmuCurOfs.x;
		float ymin = this.screenEmuCurOfs.y;
		float xmax = this.screenEmuCurWidth 	+ xmin;
		float ymax = this.screenEmuCurHeight 	+ ymin;


		GUI.DrawTexture(Rect.MinMaxRect(0, 		0,		xmin,			Screen.height), this.debugRectImg);	
		GUI.DrawTexture(Rect.MinMaxRect(xmax,	0,		Screen.width,	Screen.height), this.debugRectImg);	
		GUI.DrawTexture(Rect.MinMaxRect(xmin,	0,		xmax,			ymin), 			this.debugRectImg);	
		GUI.DrawTexture(Rect.MinMaxRect(xmin,	ymax,	xmax,			Screen.height),	this.debugRectImg);	
		}

#endif


#if EMULATE_TOUCHES_BY_MOUSE

	// Draw debug GUI...
			
	if (
#if !FORCE_EMULATED_TOUCH_DRAWING
		this.debugDrawTouches && 
#endif
#if UNITY_EDITOR
		(EditorApplication.isPlayingOrWillChangePlaymode) &&
#endif
		(this.debugTouchSprite != null))
		{
		// First touch...

		GUI.color = (Input.GetMouseButton(0) ? this.debugFirstTouchActiveColor :
			debugFirstTouchNormalColor);

		GUI.DrawTexture(//GetCenImgRectAtPos(this.debugFirstTouchPos, this.debugSecondTouchSprite, this.debugSecondTouchSpriteScale),
			GetCenRect(this.debugFirstTouchPos, this.fingerBufferRadPx * 2),
			this.debugTouchSprite);

		// Second touch...

		GUI.color = (Input.GetMouseButton(1) ? this.debugSecondTouchActiveColor :
			debugSecondTouchNormalColor);

		GUI.DrawTexture(//GetCenImgRectAtPos(this.debugSecondTouchPos, this.debugSecondTouchSprite, this.debugSecondTouchSpriteScale),
			GetCenRect(this.debugSecondTouchPos, this.fingerBufferRadPx * 2),
			(this.debugSecondTouchSprite != null) ? this.debugSecondTouchSprite : this.debugTouchSprite);
		}
#endif

		
			
	// Restore initial GUI depth...

	GUI.depth = initialGuiDepth;
	
	if (GUI.enabled != initialEnable)
		GUI.enabled = initialEnable;
	}
		
		
		


// ----------------
// Set internal emulated mouse position.
// ----------------
public void SetInternalMousePos(Vector2 pos, bool inGuiSpace = true)
	{
	if (inGuiSpace)
		pos.y = Screen.height - pos.y;

	this.emuMousePos = pos;
	}
		

// -------------------
/// Start controller's global alpha animation.
// -------------------
private void StartAlphaAnim(
	float targetAlpha,		///< Target alpha value. 
	float time				///< Blending duration in seconds.
	)
	{
	if (time <= 0)
		{
		this.globalAlphaTimer.Reset(0);
		this.globalAlphaStart 	=
		this.globalAlphaEnd 	=
		this.globalAlpha		= targetAlpha;
		}
	else
		{
		this.globalAlphaStart	= this.globalAlpha;
		this.globalAlphaEnd		= targetAlpha;
		this.globalAlphaTimer.Start(time);
		}
	}
		


		


/// \cond

// ---------------------
// Hit testing ---------
// ---------------------

// ------------------------
// Touch event handler result
// ------------------------

public enum EventResult
	{
	NOT_HANDLED,	///< Event not handled. 
	HANDLED,		///< Exclusively handled.
	SHARED			///< Touch was handled, but touch sharing is allowed.
	}


// --------------------
/// Hit test result data.
// --------------------
public struct HitTestResult
	{
	public bool 	hit;
	public float 	dist;
	public int		prio;
	public bool		hitInside;
	public float	distScale;		///< when comparing two controls of the same priority, distance will be scaled by this value 

			
	// ----------------
	public HitTestResult(bool hit)
		{
		this.hit 		= hit;
		this.dist 		= 1.0f;
		this.distScale 	= 1.0f;
		this.hitInside 	= false;
		this.prio 		= 0;
		}

	// ----------------
	public bool IsCloserThan(HitTestResult r)
		{ 
		if (!this.hit)
			return false;

		return (
			(this.prio > r.prio) ||
			(this.hitInside && !r.hitInside) ||
			((this.prio == r.prio) ?  
				((this.dist * this.distScale) < (r.dist * r.distScale)) : 
				(this.dist < r.dist)) );
		}
	}

 

// ------------------
public HitTestResult HitTestCircle(Vector2 cen, float rad, Vector2 touchPos, 
	bool useFingerBuffer = true)
	{	
	HitTestResult result = new HitTestResult(false);

	result.dist = (touchPos - cen).magnitude;
	if (result.dist > ((rad) + (useFingerBuffer ? this.fingerBufferRadPx : 0)))
		{
		result.hit = false;
		return result;
		}
			
	result.hit 			= true;
	result.hitInside	= (result.dist <= rad);
	result.distScale	= 1.0f;

	return result;
	}

// ----------------
public HitTestResult HitTestBox(Vector2 cen, Vector2 size, Vector2 touchPos, 
	bool useFingerBuffer = true)
	{
	HitTestResult result = new HitTestResult(false);

	float 	margin 	= (useFingerBuffer ? this.fingerBufferRadPx : 0);
	Vector2 v 		= new Vector2(
		Mathf.Abs(touchPos.x - cen.x), 
		Mathf.Abs(touchPos.y - cen.y));
	
	size *= 0.5f;

	if ((v.x > (size.x + margin)) || (v.y > (size.y + margin)))
		{
		result.hit = false;	
		return result;
		}

	result.hit 			= true;
	result.hitInside 	= ((v.x <= size.x) && (v.y <= size.y));
	result.dist			= v.magnitude;
	result.distScale	= 1.0f;

	return result;
	}


// ----------------
public HitTestResult HitTestRect(Rect rect, Vector2 touchPos, 
	bool useFingerBuffer = true)
	{
	HitTestResult result = new HitTestResult(false);

	float 	margin 	= (useFingerBuffer ? this.fingerBufferRadPx : 0);
	Vector2 v 		= touchPos - rect.center;
	v.x = Mathf.Abs(v.x);
	v.y = Mathf.Abs(v.y);

	Vector2 size = new Vector2(rect.width * 0.5f, rect.height * 0.5f);

	if ((v.x > (size.x + margin)) || (v.y > (size.y + margin)))
		{
		result.hit = false;	
		return result;
		}

	result.hit 			= true;
	result.hitInside 	= ((v.x <= size.x) && (v.y <= size.y));
	result.dist			= v.magnitude;
	result.distScale	= 1.0f;
			
	return result;

	}

		
		


// -------------------------
public void EndTouch(int touchId, TouchableControl ctrlToIgnore)
	{
	if (touchId < 0)
		return;

	foreach (TouchableControl tc in this.touchables)
		{
		if (tc == ctrlToIgnore)
			continue;

		tc.OnTouchEnd(touchId);
		}
	} 
	

// ---------------
/// \endcond
// ---------------

	
		

#if ENABLE_SCREEN_EMU
		
// --------------
public void LoadScreenEmuConfig()
	{
	this.rwUnit				= (TouchController.RealWorldUnit)
							  EditorPrefs.GetInt(	"CFEd.rwUnit",		(int)this.rwUnit);
	this.previewMode		= (TouchController.PreviewMode)
							  EditorPrefs.GetInt(	"CFEd.previewMode",		(int)this.previewMode);

	this.monitorDiagonal 	= EditorPrefs.GetFloat(	"CFEd.monitorDiag", 		15);
	this.screenEmuOn		= EditorPrefs.GetBool(	"CFEd.screenEmuOn", 		false);
	this.screenEmuPan.x		= EditorPrefs.GetFloat(	"CFEd.screenEmuPan.x",	 	0);
	this.screenEmuPan.y		= EditorPrefs.GetFloat(	"CFEd.screenEmuPan.y", 		0);
	this.screenEmuPortrait	= EditorPrefs.GetBool(	"CFEd.screenEmuPortrait", 	false);
	this.screenEmuShrink	= EditorPrefs.GetBool(	"CFEd.screenEmuShrink",		false);
	this.screenEmuMode		= (TouchController.ScreenEmuMode)
							  EditorPrefs.GetInt(	"CFEd.screenEmuMode",		(int)this.screenEmuMode);
	this.screenEmuHwHorzRes	= EditorPrefs.GetInt(	"CFEd.screenEmuHwHorzRes",	this.screenEmuHwHorzRes);
	this.screenEmuHwVertRes	= EditorPrefs.GetInt(	"CFEd.screenEmuHwVertRes",	this.screenEmuHwVertRes);
	this.screenEmuHwDpi		= EditorPrefs.GetInt(	"CFEd.screenEmuHwDpi",		this.screenEmuHwDpi);
	}

		

// ---------------
public void SaveScreenEmuConfig()
	{
	EditorPrefs.SetFloat(	"CFEd.monitorDiag",			this.monitorDiagonal);
	EditorPrefs.SetBool(	"CFEd.screenEmuOn",			this.screenEmuOn);
	EditorPrefs.SetFloat(	"CFEd.screenEmuPan.x",		this.screenEmuPan.x);
	EditorPrefs.SetFloat(	"CFEd.screenEmuPan.y", 		this.screenEmuPan.y);
	EditorPrefs.SetBool(	"CFEd.screenEmuPortrait", 	this.screenEmuPortrait);
	EditorPrefs.SetBool(	"CFEd.screenEmuShrink",		this.screenEmuShrink);
	EditorPrefs.SetInt(		"CFEd.screenEmuMode",		(int)this.screenEmuMode);
	EditorPrefs.SetInt(		"CFEd.screenEmuHwHorzRes",	this.screenEmuHwHorzRes);
	EditorPrefs.SetInt(		"CFEd.screenEmuHwVertRes",	this.screenEmuHwVertRes);
	EditorPrefs.SetInt(		"CFEd.screenEmuHwDpi",		this.screenEmuHwDpi);
			
	EditorPrefs.SetInt(		"CFEd.rwUnit",				(int)this.rwUnit);
	EditorPrefs.SetInt(		"CFEd.previewMode",			(int)this.previewMode);

	}
		



// ----------------------
private void UpdateScreenEmu()
	{
	float w =	(!this.screenEmuPortrait ? this.screenEmuHwHorzRes : this.screenEmuHwVertRes);
	float h = 	(!this.screenEmuPortrait ? this.screenEmuHwVertRes : this.screenEmuHwHorzRes);

	switch (this.screenEmuMode)
		{
		case ScreenEmuMode.PIXEL_PERFECT : 
			this.screenEmuCurWidth 	= w;
			this.screenEmuCurHeight	= h;
			this.screenEmuCurDPI 	= this.screenEmuHwDpi;
			break;

		case ScreenEmuMode.PHYSICAL :
			{
			float scale = this.monitorDpi / (float)this.screenEmuHwDpi;
			this.screenEmuCurWidth 	= scale * w;
			this.screenEmuCurHeight	= scale * h;
			this.screenEmuCurDPI 	= scale * this.screenEmuHwDpi;
			}
			break;

		case ScreenEmuMode.EXPAND :
			{
			//float xscale = (float)w / (float)Screen.width;
			//float yscale = (float)h / (float)Screen.height;
			float xscale = (float)Screen.width / (float)w;
			float yscale = (float)Screen.height / (float)h;

			if (xscale <= yscale)
				{
				this.screenEmuCurWidth = Screen.width;		
				this.screenEmuCurHeight = (float)Screen.width * (h / w);
				}				
			else
				{
				this.screenEmuCurHeight = (float)Screen.height;		
				this.screenEmuCurWidth = (float)Screen.height * (w / h);
				}				
				
//Debug.Log("Fr["+Time.frameCount+"] EXPAND Scr["+Screen.width+" x "+Screen.height+"] tgt["+w+" x "+h+"] CALC["+this.screenEmuCurWidth+" x " +this.screenEmuCurHeight+"] SCale["+xscale+" x "+yscale+"]");

			this.screenEmuCurDPI = this.screenEmuHwDpi * ((float)this.screenEmuCurWidth / w); 

			} 
			break;

		}

		
		
	// Shrinking...

	if (this.screenEmuMode == ScreenEmuMode.EXPAND)
		{
		this.screenEmuBorderShrunk = true;
		}
	else
		{
		this.screenEmuBorderShrunk = false;
	
		if (this.screenEmuShrink &&
			((this.screenEmuCurWidth > Screen.width) ||
			 (this.screenEmuCurHeight > Screen.height)))
			{
			this.screenEmuBorderShrunk = true;
	
			float scale = Mathf.Min((Screen.width / this.screenEmuCurWidth),
				(Screen.height / this.screenEmuCurHeight));
	
			this.screenEmuCurWidth 	= (this.screenEmuCurWidth * scale);
			this.screenEmuCurHeight = (this.screenEmuCurHeight * scale);		
			this.screenEmuCurDPI *= scale;
			}
		}

	Vector2 ofs = new Vector2(
		(Screen.width - this.screenEmuCurWidth), 
		(Screen.height - this.screenEmuCurHeight));
		
	this.screenEmuCurOfs = new Vector2(
		ofs.x * this.screenEmuPan.x, 
		ofs.y * this.screenEmuPan.y);
	//this.screenEmuCurOfs = TouchController.AnchorLeftover(ofs, this.screenEmuAnchor);
	}	


#endif
		
	
	
// ---------------------
// Hide from documentation...
/// \cond
// ---------------------

// --------------------
public float GetScreenWidth()
	{
#if ENABLE_SCREEN_EMU
	if (this.screenEmuOn)
		{
		return this.screenEmuCurWidth;
		}		
#endif

	return (float)Screen.width;
	}


// --------------------
public float GetScreenHeight()
	{
#if ENABLE_SCREEN_EMU
	if (this.screenEmuOn)
		{
		return this.screenEmuCurHeight;
		}		
#endif

	return (float)Screen.height;
	}


// --------------------------
public float GetScreenX(float xFactor)
	{
#if ENABLE_SCREEN_EMU
	if (this.screenEmuOn)
		{
		//this.UpdateScreenEmu();
		return (screenEmuCurOfs.x + (xFactor * this.screenEmuCurWidth));
		}		
#endif

	return (xFactor * (float)Screen.width);
	}


		

// --------------------------
public float GetScreenY(float yFactor)
	{
#if ENABLE_SCREEN_EMU
	if (this.screenEmuOn)
		{
		//this.UpdateScreenEmu();
		return (screenEmuCurOfs.y + (yFactor * this.screenEmuCurHeight));
		}		
#endif

	return (yFactor * (float)Screen.height);
	}
		
		

// ---------------
public float CmToPixels(float cmVal)
	{
	return (this.GetDPCM() * cmVal);
	}
		

// ----------------
public float PixelsToWorld(float pxVal)
	{
	float d = ((this.rwUnit == RealWorldUnit.CM) ? this.GetDPCM() : this.GetDPI());
	if (d <= 0.00001f)
		return 0;
	return (pxVal / d);
	}


// -------------------
public Rect NormalizedRectToPx(Rect nrect, bool respectLeftHandMode = true)
	{
	Rect r = Rect.MinMaxRect(
		this.GetScreenX(nrect.xMin),
		this.GetScreenY(nrect.yMin),
		this.GetScreenX(nrect.xMax),
		this.GetScreenY(nrect.yMax));

	if (respectLeftHandMode)
		return RightHandToScreenRect(r);

	return r;
	}
	
// -------------
/// \endcond
// -------------

	
// Hide from documantation...
/// \cond



// --------------
public float GetPreviewScale()
	{
#if ENABLE_SCREEN_EMU
	if (this.screenEmuHwHorzRes < 0.0001f)
		return 0;
	return (this.screenEmuCurWidth / this.screenEmuHwHorzRes); 
#else	
	return 1;
#endif
	}


		


// ----------------
public Vector2 RightHandToScreen(Vector2 pos)
	{
	if (!this.leftHandedMode)
		return pos;

#if ENABLE_SCREEN_EMU
	if (this.screenEmuOn)
		{
		//this.UpdateScreenEmu();
		pos.x = (screenEmuCurOfs.x + (this.screenEmuCurWidth - 
			(pos.x - screenEmuCurOfs.x)));
		return pos;
		}		
#endif
			
	pos.x = Screen.width - pos.x;
	return pos;
	}


// -------------------
public Rect RightHandToScreenRect(Rect rect)
	{
	if (!this.leftHandedMode)
		return rect;

	Vector2 vmin = RightHandToScreen(new Vector2(rect.xMin, rect.yMin));
	Vector2 vmax = RightHandToScreen(new Vector2(rect.xMax, rect.yMax));
			
	return Rect.MinMaxRect(
		Mathf.Min(vmin.x, vmax.x), 
		Mathf.Min(vmin.y, vmax.y), 
		Mathf.Max(vmin.x, vmax.x), 
		Mathf.Max(vmin.y, vmax.y) );
	}

	



// ---------------------------
private static Vector2 AnchorLeftover(
	Vector2 		topLeftOfs, 
	LayoutAnchor 	anchor,
	float			maxMarginX = 0,
	float			maxMarginY = 0,	
	bool			uniformMargins = false)
	{
	float marginX = 0;
	float marginY = 0;
			
	if (topLeftOfs.x > 0) marginX = Mathf.Min(topLeftOfs.x, maxMarginX);
	if (topLeftOfs.y > 0) marginY = Mathf.Min(topLeftOfs.y, maxMarginY);

	if (uniformMargins && (maxMarginX > 0.001f) && (maxMarginY > 0.001f))
		{
		float mscale = Mathf.Min((marginX / maxMarginX), (marginY / maxMarginY));
		marginX = mscale * maxMarginX;
		marginY = mscale * maxMarginY;
		}


	switch (anchor)
		{	
		case LayoutAnchor.BOTTOM_LEFT : 
			topLeftOfs.y = topLeftOfs.y - marginY; 	// BOTTOM
			topLeftOfs.x = 0 			+ marginX;	// LEFT
			break;

		case LayoutAnchor.BOTTOM_CENTER : 
			topLeftOfs.y = topLeftOfs.y - marginY; 	// BOTTOM
			topLeftOfs.x = topLeftOfs.x	* 0.5f;		// CENTER
			break;


		case LayoutAnchor.BOTTOM_RIGHT :
			topLeftOfs.y = topLeftOfs.y - marginY; 	// BOTTOM
			topLeftOfs.x = topLeftOfs.x - marginX; 	// RIGHT
			break;

		case LayoutAnchor.MID_LEFT : 
			topLeftOfs.y = topLeftOfs.y	* 0.5f;		// MID
			topLeftOfs.x = 0 			+ marginX;	// LEFT
			break;

		case LayoutAnchor.MID_CENTER : 
			topLeftOfs.y = topLeftOfs.y	* 0.5f;		// MID
			topLeftOfs.x = topLeftOfs.x	* 0.5f;		// CENTER
			break;

		case LayoutAnchor.MID_RIGHT :
			topLeftOfs.y = topLeftOfs.y	* 0.5f;		// MID
			topLeftOfs.x = topLeftOfs.x - marginX; 	// RIGHT
			break;

		case LayoutAnchor.TOP_LEFT : 
			topLeftOfs.y = 0 			+ marginY;	// TOP
			topLeftOfs.x = 0 			+ marginX;	// LEFT
			break;

		case LayoutAnchor.TOP_CENTER : 
			topLeftOfs.y = 0 			+ marginY;	// TOP
			topLeftOfs.x = topLeftOfs.x	* 0.5f;		// CENTER
			break;

		case LayoutAnchor.TOP_RIGHT :
			topLeftOfs.y = 0 			+ marginY;	// TOP
			topLeftOfs.x = topLeftOfs.x - marginX; 	// RIGHT
			break;
		}

	return topLeftOfs;
	}
	
/// \endcond
	



/// \cond

// -----------------
[System.SerializableAttribute]
public class LayoutBox
	{
	public string		name;
	public LayoutAnchor	anchor;

	public bool 	allowNonuniformScale;
	public bool		ignoreLeftHandedMode;

	public Rect		normalizedRect;
	
	public float 	horzMarginMax,
					vertMarginMax;
	public bool		uniformMargins;			
			
	private TouchController joy;

	private Vector2 contentOfs,		// raw content rect
					contentSize,
					contentPosScale;	// raw content pos (in cm) to screen (px)
	private float	contentSizeScale;	// raw content size (in cm) to uniform screen (px)
	private Vector2	screenDstOfs;
	private Rect	contentScreenBox,
					availableScreenBox;
	//private Rect	dstRect;		// destination rect
			
	public Color	debugColor = new Color(1, 1, 1, 0.2f);
	public bool		debugDraw = false;


			
	// -----------------
	public LayoutBox(string name, float left, float top, float width, float height, LayoutAnchor anchor)
		{
		this.name = name;
		this.normalizedRect = new Rect(left, top, width, height); //Rect.MinMaxRect(left, top, right, bottom);

		this.anchor = anchor; 

		this.uniformMargins = true;
		this.horzMarginMax = 0.5f;
		this.vertMarginMax = 0.5f;
		}

	// -------------
	public void SetController(TouchController joy)
		{
		this.joy = joy;
		}			


	// ---------------
	public void ResetContent()
		{
		this.contentSize = Vector2.zero;
		}
			
			
	// ----------------
	private void AddContentMinMax(Vector2 bbmin, Vector2 bbmax)
		{
		if (this.contentSize.x < 0.001f)
			{
			this.contentOfs 	= bbmin;
			this.contentSize	= (bbmax - bbmin);
			}
		else
			{
			Vector2 curmax = this.contentOfs + this.contentSize;

			this.contentOfs.x = Mathf.Min(bbmin.x, this.contentOfs.x);
			this.contentOfs.y = Mathf.Min(bbmin.y, this.contentOfs.y);

			curmax.x = Mathf.Max(bbmax.x, curmax.x);
			curmax.y = Mathf.Max(bbmax.y, curmax.y);

			this.contentSize = curmax - this.contentOfs;

			}
		
		}

	// -----------------
	public void AddContent(Vector2 cen, float size)
		{
		size *= 0.5f;
		//Vector2 halfSize = new Vector2(rad, rad); 
		Vector2 halfSize = new Vector2(size, size); 
		this.AddContentMinMax(cen - halfSize, cen + halfSize);
		}

	// ---------------
	public void AddContent(Vector2 cen, Vector2 size)
		{
		size *= 0.5f;
		this.AddContentMinMax(cen - size, cen + size);	
		}


	// --------------
	public void ContentFinalize()
		{
		float xmin = this.joy.GetScreenX(this.normalizedRect.xMin); //.left);
		float xmax = this.joy.GetScreenX(this.normalizedRect.xMax); //.right);
		float ymin = this.joy.GetScreenY(this.normalizedRect.yMin); //.top);
		float ymax = this.joy.GetScreenY(this.normalizedRect.yMax); //.bottom);
				
		Vector2 boxSize = new Vector2((xmax - xmin), (ymax - ymin));

		float idealContentSizeX = this.joy.CmToPixels(this.contentSize.x);
		float idealContentSizeY = this.joy.CmToPixels(this.contentSize.y);
			

		if ((idealContentSizeX < 0.01f) || (idealContentSizeY < 0.01f))
			{
			this.contentPosScale 	= Vector2.one;
			this.contentSizeScale 	= 1.0f;
			}
		else
			{	
			float idealToScreenX = Mathf.Clamp01(boxSize.x / idealContentSizeX); 
			float idealToScreenY = Mathf.Clamp01(boxSize.y / idealContentSizeY); 
					
			this.contentSizeScale = Mathf.Clamp01(Mathf.Min(idealToScreenX, idealToScreenY));
		
			if (this.allowNonuniformScale)
				this.contentPosScale = new Vector2(Mathf.Clamp01(idealToScreenX), Mathf.Clamp01(idealToScreenY));
			else
				this.contentPosScale = new Vector2(this.contentSizeScale, this.contentSizeScale);
			}
				

		Vector2 contentScreenSize = new Vector2(
			this.contentPosScale.x * idealContentSizeX,	
			this.contentPosScale.y * idealContentSizeY); 
				
		// Prepare scales to convert from cm to px

		this.contentPosScale *= this.joy.GetDPCM();
		this.contentSizeScale *= this.joy.GetDPCM();
				

		// Calculate margins...
				
		Vector2 leftoverSpace = boxSize - contentScreenSize;

		float maxMarginX = this.horzMarginMax * this.joy.GetDPCM();
		float maxMarginY = this.vertMarginMax * this.joy.GetDPCM();

	
		this.screenDstOfs = new Vector2(xmin, ymin) + 
			TouchController.AnchorLeftover(leftoverSpace, this.anchor,
				maxMarginX, maxMarginY, this.uniformMargins);
		
		// Build debug rects...

		this.contentScreenBox = new Rect(this.screenDstOfs.x, this.screenDstOfs.y,
			contentScreenSize.x, contentScreenSize.y);

		this.availableScreenBox = new Rect(xmin, ymin, boxSize.x, boxSize.y);

			
		if (!this.ignoreLeftHandedMode)
			{
			this.contentScreenBox 	= this.joy.RightHandToScreenRect(this.contentScreenBox);
			this.availableScreenBox = this.joy.RightHandToScreenRect(this.availableScreenBox);
			}
		


		} 

	// -------------
	public Vector2	GetScreenPos(Vector2 pos)
		{
		pos -= this.contentOfs;
		pos.x *= this.contentPosScale.x;		
		pos.y *= this.contentPosScale.y;		
		pos += this.screenDstOfs; //+ pos);  //((pos - this.contentOfs) * this.contentPosScale));
			
		return (this.ignoreLeftHandedMode ? pos : this.joy.RightHandToScreen(pos));
		}

	// ------------	
	public float	GetScreenSize(float size)
		{
		return (size * this.contentSizeScale);
		}
			
	// ------------	
	public Vector2	GetScreenSize(Vector2 size)
		{
		return (size * this.contentSizeScale);
		}


	// ---------------
#if DEBUG_MODE
	public void DrawDebug()
		{
		if (this.debugDraw && (Event.current.type == EventType.Repaint))
			{
			// Available region ...

			GUI.color = this.debugColor * 0.75f;
			GUI.DrawTexture(this.availableScreenBox, this.joy.debugRectImg, ScaleMode.StretchToFill);
		
			// Box used by content
			GUI.color = this.debugColor;
			GUI.DrawTexture(this.contentScreenBox, this.joy.debugRectImg, ScaleMode.StretchToFill);
			}
		}
#endif
	
	}


/// \endcond


// \endcond






/// \cond

		

// ------------------
// Editor-Only Section
// -------------------
		
		
	[System.NonSerialized]
	private double editorLastSafetyUpdateTime;

	[System.NonSerialized]
	private const double EDITOR_SAFETY_UPDATE_INTERVAL = 2.0f;


#if UNITY_EDITOR
		
// ----------------
public void OnEditorUpdate()
	{
	if ((EditorApplication.timeSinceStartup - this.editorLastSafetyUpdateTime) >
		EDITOR_SAFETY_UPDATE_INTERVAL)
		{
//Debug.Log("Editor UPDATE : " + this.editorLastSafetyUpdateTime + " : " + EditorApplication.timeSinceStartup);

		this.editorLastSafetyUpdateTime = EditorApplication.timeSinceStartup;
		this.OnGlobalColorChange();
		}
	}


// ----------------
public void OnGlobalColorChange()
	{	
	if (this.sticks != null)
		{
		foreach (TouchStick ts in this.sticks)
			ts.SetColorsDirtyFlag(); //.OnColorChange();
		}

	if (this.touchZones != null)
		{
		foreach (TouchZone tz in this.touchZones)
			tz.SetColorsDirtyFlag(); //.OnColorChange();
		}
	}

// ------------------
// Upgrade controller.
// ------------------
private void UpgradeIfNeeded() //TouchController joy)
	{
	TouchController joy = this;

//Debug.Log("Checking ["+this.name+"] for upgrades... OK(" + (((joy.sticks != null) && (joy.touchZones != null)) ? "YES" : "NO") + ")");


	if ((joy.sticks == null) || (joy.touchZones == null))
		{
		return;
		}
	
	bool joyModified = false;

	// --------------
	// Version 4
	// ---------------

	if (joy.version < 4)
		{
		Debug.Log("Upgrading [" + joy.name + "] from ver:"+ joy.version + " to ver. 4");

		foreach (TouchStick stick in joy.sticks)
			{
			stick.smoothReturn = true;
			}

		joy.version = 4;

		joyModified = true;
		}
		
	// ----------------
	// Version 5 
	// ----------------

	if (joy.version < 5)
		{
		Debug.Log("Upgrading [" + joy.name + "] from ver:"+ joy.version + " to ver. 5");

		foreach (TouchStick stick in joy.sticks)
			{	
			stick.enableGetKey 		= false;
			stick.enableGetButton 	= false;
			stick.enableGetAxis 	= false;

			stick.axisHorzFlip		= false;
			stick.axisVertFlip		= false;
			stick.axisHorzName		= "Horizontal";			
			stick.axisVertName		= "Vertical";			

			stick.getKeyCodePress	= KeyCode.None;
			stick.getKeyCodePressAlt= KeyCode.None;
			stick.getKeyCodeUp		= KeyCode.W;
			stick.getKeyCodeUpAlt	= KeyCode.UpArrow;
			stick.getKeyCodeDown	= KeyCode.S;
			stick.getKeyCodeDownAlt	= KeyCode.DownArrow;
			stick.getKeyCodeLeft	= KeyCode.A;
			stick.getKeyCodeLeftAlt	= KeyCode.LeftArrow;
			stick.getKeyCodeRight	= KeyCode.D;
			stick.getKeyCodeRightAlt= KeyCode.RightArrow;

			stick.smoothReturn = true;
			}

		foreach (TouchZone zone in joy.touchZones)
			{
			/*
			zone.enableGetKey		= false;
			zone.enableGetButton	= false;
			zone.enableGetAxis		= false;
	
			zone.getKeyCode			= KeyCode.None;
			zone.getKeyCodeAlt		= KeyCode.None;
			zone.getKeyCodeMulti	= KeyCode.None;
			zone.getKeyCodeMultiAlt	= KeyCode.None;
			*/

			zone.axisHorzName		= "Mouse X";
			zone.axisVertName		= "Mouse Y";

			zone.getButtonName		= "Fire1";
			zone.getButtonMultiName	= "Fire1Multi";
		
			zone.axisValScale		= 0.1f;
			}

		joy.version = 5;

		joyModified = true;
		}

		
	// ----------------
	// Version 6 
	// ----------------

	if (joy.version < 6)
		{
		Debug.Log("Upgrading [" + joy.name + "] from ver:"+ joy.version + " to ver. 6");

		//foreach (TouchStick stick in joy.sticks)
		//	{	
		//	}

		foreach (TouchZone zone in joy.touchZones)
			{
			zone.codeUniPressed = true;	
			zone.codeMultiPressed = true;
			zone.codeUniJustReleased = true;
			zone.codeMultiJustReleased = true;
			}

		joy.version = 6;

		joyModified = true;
		}
			
	if (joyModified)
		{
		EditorUtility.SetDirty(joy);
		AssetDatabase.SaveAssets();

		}

	}
		

/*		
// -----------------------------
// Check if given symbol is already defined
// -----------------------------
private static bool IsSymDefined(BuildTargetGroup tgt, string sym)
	{
	string s = PlayerSettings.Get
	}  
*/

#endif




// -------------------
// WebPlayer-Only Section 
// ---------------------


#if UNITY_WEBPLAYER && !UNITY_EDITOR

static private float 
	webPlayerDiagonalInches	= NON_MOBILE_DIAGONAL_INCHES,		// When in web player mode, this will
	webPlayerDPI 			= 72.0f;

		
// -------------------
public void SetWebDiagonal(float diagonalInches)
	{
	TouchController.webPlayerDiagonalInches = Mathf.Clamp(diagonalInches, 1, 60);	
	this.UpdateWebDPI();
	this.SetLayoutDirtyFlag();
	}

// ---------------
private void UpdateWebDPI()
	{
	TouchController.webPlayerDPI = Mathf.Sqrt((float)((Screen.width*Screen.width) + 
		(Screen.height * Screen.height))) / TouchController.webPlayerDiagonalInches;
	}


#endif



// ------------------------
// Aninamted Property Helpers 
// ------------------------
		
// ---------------------
public struct AnimFloat
	{	
	public float
		 	start,
			end,
			cur;

	// -----------------
	public void Reset(float val)
		{
		this.start = this.end = this.cur = val;
		}

	// --------------
	public void MoveTo(float val)
		{	
		this.start 	= this.cur;
		this.end 	= val;
		}

	// --------------
	public void Update(float lerpt)
		{
		this.cur = Mathf.Lerp(this.start, this.end, lerpt);
		}
	}


// ---------------------
public struct AnimColor
	{	
	public Color 
		start,
		end,
		cur;

	// -----------------
	public void Reset(Color val)
		{
		this.start = this.end = this.cur = val;
		}

	// --------------
	public void MoveTo(Color val)
		{	
		this.start 	= this.cur;
		this.end 	= val;
		}

	// --------------
	public void Update(float lerpt)
		{
		this.cur = Color.Lerp(this.start, this.end, lerpt);
		}
	}



// -----------------
// Utilities --------
// -----------------
		
		
// ---------------
static public float SlowDownEase(float t)
	{
	t = 1.0f - t;
	return (1.0f - (t * t));
	}
		

// ------------
static public float SpeedUpEase(float t)
	{
	return ((t * t));
	}



// ---------------
static public Color ScaleAlpha(Color c, float alphaScale)
	{
	c.a *= alphaScale;
	return c;
	}

// -------------------
static public Rect GetCenImgRectAtPos(Vector2 pos, Texture2D img, float scale = 1.0f)
	{
	if (img == null)
		return new Rect(pos.x, pos.y, 1, 1);
	
	pos.x -= ((float)img.width * 0.5f * scale);
	pos.y -= ((float)img.height * 0.5f * scale);
			
	return new Rect(pos.x, pos.y, 
		((float)img.width * scale), ((float)img.height * scale));
	}


// ----------------
static public Rect GetCenRect(Vector2 pos, Vector2 size)
	{
	pos.x -= size.x * 0.5f;
	pos.y -= size.y * 0.5f;
	return new Rect(pos.x, pos.y, size.x, size.y);
	}


// ----------------
static public Rect GetCenRect(Vector2 pos, float size)
	{
	pos.x -= size * 0.5f;
	pos.y -= size * 0.5f;
	return new Rect(pos.x, pos.y, size, size);
	}
		


/// \endcond
		



}

		
//}

// -----------------------------------------------
// Control Freak - The Ultimate Virtual Controller
// Copyright (C) 2013 Dan's Game Tools
// -----------------------------------------------


using UnityEngine;
using UnityEditor;

//namespace TouchControllerLib
//{

/*
 EditorGUIUtility.systemCopyBuffer 

*/

[CustomEditor(typeof(TouchController))]
public class TouchControllerEditor : Editor
{

public Texture2D defaultButtonPressedImg;
	
private SerializedProperty
	screenEmuOnProp,
	screenEmuModeProp,
	screenEmuPortraitProp,
	screenEmuShrinkProp,
	//screenEmuAnchorProp,
	screenEmuHwDpiProp,
	screenEmuHwAspectRatioProp,
	screenEmuHwHorzResProp,
	screenEmuHwVertResProp,
	screenEmuBorderColorProp,
	screenEmuBorderBadColorProp,
	screenEmuPanProp,
		
	//monitorDpiProp,
	monitorDiagonalProp,

	rwUnitProp,
	previewModeProp,

	guiDepthProp,
	guiPressedOfsProp,
	pressAnimDurationProp,
	releaseAnimDurationProp,
	disableAnimDurationProp,
	enableAnimDurationProp,
	cancelAnimDurationProp,
	showAnimDurationProp,
	hideAnimDurationProp,
	

	defaultZoneImgProp,
	defaultStickHatImgProp,
	defaultStickBaseImgProp,

	defaultReleasedZoneColorProp,
	defaultPressedZoneColorProp,
	defaultDisabledZoneColorProp,
	defaultReleasedStickHatColorProp,
	defaultReleasedStickBaseColorProp,
	defaultPressedStickHatColorProp,
	defaultPressedStickBaseColorProp,
	defaultDisabledStickHatColorProp,
	defaultDisabledStickBaseColorProp,

	releasedZoneScaleProp,
	pressedZoneScaleProp,
	disabledZoneScaleProp,
	releasedStickHatScaleProp,
	pressedStickHatScaleProp,
	disabledStickHatScaleProp,
	releasedStickBaseScaleProp,
	pressedStickBaseScaleProp,
	disabledStickBaseScaleProp,


// ------------------
		
	fingerBufferCmProp,
	stickMagnetAngleMarginProp,
	stickDigitalEnterThreshProp,
	stickDigitalLeaveThreshProp,
	touchTapMaxTimeProp,
	doubleTapMaxGapTimeProp,
	//touchHoldMinTimeProp,
	touchTapMaxDistCmProp,
	twistSafeFingerDistCmProp,
	pinchMinDistCmProp,
	strictMultiFingerMaxTimeProp,
	velPreserveTimeProp,	


	//buttonsProp,
	sticksProp,
	touchZonesProp,
	layoutBoxesProp,

	automaticModeProp,
	manualGuiProp,
	//fixedUpdateModeProp,

	autoActivateProp,
	disableWhenNoTouchScreenProp,
	

	//debugSecondTouchMoveKeyProp,
	//debugSecondTouchFlipXKeyProp,
	//debugSecondTouchFlipYKeyProp,
	debugSecondTouchDragModeKeyProp,
	debugSecondTouchPinchModeKeyProp,
	debugSecondTouchTwistModeKeyProp,


	//debugSecondTouchMoveKeyPressToBlockProp,
	debugDrawTouchesProp,
	debugDrawLayoutBoxesProp,
	//debugDrawAreasProp,
	debugTouchSpriteProp,
	debugSecondTouchSpriteProp,
	debugFirstTouchNormalColorProp,
	debugFirstTouchActiveColorProp,
	debugSecondTouchNormalColorProp,
	debugSecondTouchActiveColorProp,
	//debugFrameStyleProp,
	debugCircleImgProp,
	debugRectImgProp,
	

// ------------------
		


// ---------------

	blankProp;


	
static private bool
	//drawDefaultInspector 	= false,
	showDebugSecondTouch	= false,
	//showDebugSection		= false,
	showDebugImages			= false,
	showScreenEmu			= false,
	showSettingsSection		= false,
	showLayoutBoxes			= false,
	//showButtons				= false,
	showSticks				= false,
	showGestureZones		= false,
	showStickCodeSection	= false,
	showZoneCodeSection		= false; 

		
private GUIStyle	
	editSectionStyle,
	sectionToolbarStyle,
	sectionFoldoutStyle;


private const string HIT_DIST_SCALE_TOOLTIP_STR = 
	"When hit-testing controls of the same priority level, actual hit distance will be scaled by this value. Use this to fine-tune the trickier areas...";


public static Color	
	DEFAULT_PRESSED_COLOR	= new Color(1.0f,	1.0f,	1.0f, 1.0f),
	DEFAULT_RELEASED_COLOR	= new Color(0.8f,	0.8f,	0.8f, 0.75f),
	DEFAULT_DISABLED_COLOR	= new Color(0.5f, 	0.5f, 	0.5f, 0.35f),
	
	DEFAULT_STICK_HAT_PRESSED_COLOR		= new Color(1.0f,	1.0f,	1.0f, 1.0f),
	DEFAULT_STICK_HAT_RELEASED_COLOR	= new Color(0.8f,	0.8f,	0.8f, 0.75f),
	DEFAULT_STICK_HAT_DISABLED_COLOR	= new Color(0.8f,	0.8f,	0.8f, 0.35f),
	DEFAULT_STICK_BASE_PRESSED_COLOR	= new Color(1.0f,	1.0f,	1.0f, 1.0f),
	DEFAULT_STICK_BASE_RELEASED_COLOR	= new Color(0.8f,	0.8f,	0.8f, 0.75f),
	DEFAULT_STICK_BASE_DISABLED_COLOR	= new Color(0.8f,	0.8f,	0.8f, 0.35f);


public const float
	DEFAULT_PRESSED_SCALE				= 1.2f,
	DEFAULT_STICK_BASE_PRESSED_SCALE	= 0.8f,

	DEFAULT_PRESS_DURATION				= 0.1f,
	DEFAULT_RELEASE_DURATION			= 0.3f,
	DEFAULT_DISABLE_DURATION			= 0.2f,
	DEFAULT_ENABLE_DURATION				= 0.2f,

	DEFAULT_STICK_FADEOUT				= 2.0f;


// -------------------
public void OnEnable()
	{
	TouchController joy = this.target as TouchController;
	if (joy != null)
		{
		joy.LoadScreenEmuConfig();
		}



	SerializedObject so = this.serializedObject;

	this.screenEmuOnProp			= so.FindProperty("screenEmuOn");	
	this.screenEmuModeProp			= so.FindProperty("screenEmuMode");
	this.screenEmuPortraitProp		= so.FindProperty("screenEmuPortrait");
	this.screenEmuShrinkProp		= so.FindProperty("screenEmuShrink");
	//this.screenEmuAnchorProp		= so.FindProperty("screenEmuAnchor");
	this.screenEmuHwDpiProp			= so.FindProperty("screenEmuHwDpi");
	this.screenEmuHwHorzResProp		= so.FindProperty("screenEmuHwHorzRes");
	this.screenEmuHwVertResProp		= so.FindProperty("screenEmuHwVertRes");
	this.screenEmuBorderColorProp	= so.FindProperty("screenEmuBorderColor");
	this.screenEmuBorderBadColorProp= so.FindProperty("screenEmuBorderBadColor");
	this.screenEmuPanProp			= so.FindProperty("screenEmuPan");

	this.monitorDiagonalProp		= so.FindProperty("monitorDiagonal");

	
	this.previewModeProp			= so.FindProperty("previewMode");
	this.rwUnitProp					= so.FindProperty("rwUnit");
		
	this.guiDepthProp					= so.FindProperty("guiDepth");
	this.guiPressedOfsProp				= so.FindProperty("guiPressedOfs");

	this.pressAnimDurationProp			= so.FindProperty("pressAnimDuration");
	this.releaseAnimDurationProp		= so.FindProperty("releaseAnimDuration");
	this.disableAnimDurationProp		= so.FindProperty("disableAnimDuration");
	this.enableAnimDurationProp			= so.FindProperty("enableAnimDuration");
	this.cancelAnimDurationProp			= so.FindProperty("cancelAnimDuration");
	this.showAnimDurationProp			= so.FindProperty("showAnimDuration");
	this.hideAnimDurationProp			= so.FindProperty("hideAnimDuration");
	
	this.defaultReleasedZoneColorProp	= so.FindProperty("defaultReleasedZoneColor");
	this.defaultPressedZoneColorProp	= so.FindProperty("defaultPressedZoneColor");
	this.defaultDisabledZoneColorProp	= so.FindProperty("defaultDisabledZoneColor");
	this.defaultReleasedStickHatColorProp	= so.FindProperty("defaultReleasedStickHatColor");
	this.defaultReleasedStickBaseColorProp	= so.FindProperty("defaultReleasedStickBaseColor");
	this.defaultPressedStickHatColorProp	= so.FindProperty("defaultPressedStickHatColor");
	this.defaultPressedStickBaseColorProp	= so.FindProperty("defaultPressedStickBaseColor");
	this.defaultDisabledStickHatColorProp	= so.FindProperty("defaultDisabledStickHatColor");
	this.defaultDisabledStickBaseColorProp	= so.FindProperty("defaultDisabledStickBaseColor");
	
	this.releasedZoneScaleProp				= so.FindProperty("releasedZoneScale");
	this.pressedZoneScaleProp				= so.FindProperty("pressedZoneScale");
	this.disabledZoneScaleProp				= so.FindProperty("disabledZoneScale");
	this.releasedStickHatScaleProp			= so.FindProperty("releasedStickHatScale");
	this.pressedStickHatScaleProp			= so.FindProperty("pressedStickHatScale");
	this.disabledStickHatScaleProp			= so.FindProperty("disabledStickHatScale");
	this.releasedStickBaseScaleProp			= so.FindProperty("releasedStickBaseScale");
	this.pressedStickBaseScaleProp			= so.FindProperty("pressedStickBaseScale");
	this.disabledStickBaseScaleProp			= so.FindProperty("disabledStickBaseScale");


	this.defaultZoneImgProp					= so.FindProperty("defaultZoneImg");
	this.defaultStickHatImgProp				= so.FindProperty("defaultStickHatImg");
	this.defaultStickBaseImgProp			= so.FindProperty("defaultStickBaseImg");


	this.fingerBufferCmProp				= so.FindProperty("fingerBufferCm");
	this.stickMagnetAngleMarginProp		= so.FindProperty("stickMagnetAngleMargin");
	this.stickDigitalEnterThreshProp	= so.FindProperty("stickDigitalEnterThresh");
	this.stickDigitalLeaveThreshProp	= so.FindProperty("stickDigitalLeaveThresh");
	this.touchTapMaxTimeProp			= so.FindProperty("touchTapMaxTime");
	this.doubleTapMaxGapTimeProp		= so.FindProperty("doubleTapMaxGapTime");
	this.strictMultiFingerMaxTimeProp	= so.FindProperty("strictMultiFingerMaxTime");
	this.velPreserveTimeProp			= so.FindProperty("velPreserveTime");	

		 
	//this.touchHoldMinTimeProp			= so.FindProperty("touchHoldMinTime");
	this.touchTapMaxDistCmProp			= so.FindProperty("touchTapMaxDistCm");
	this.twistSafeFingerDistCmProp		= so.FindProperty("twistSafeFingerDistCm");
	this.pinchMinDistCmProp				= so.FindProperty("pinchMinDistCm");
		 
	//this.buttonsProp						= so.FindProperty("buttons");
	this.sticksProp							= so.FindProperty("sticks");
	this.touchZonesProp						= so.FindProperty("touchZones");
	this.layoutBoxesProp					= so.FindProperty("layoutBoxes");

	this.automaticModeProp					= so.FindProperty("automaticMode");
	this.manualGuiProp						= so.FindProperty("manualGui");
	//this.fixedUpdateModeProp				= so.FindProperty("fixedUpdateMode");
		
	this.autoActivateProp					= so.FindProperty("autoActivate");
	this.disableWhenNoTouchScreenProp		= so.FindProperty("disableWhenNoTouchScreen");


	//this.debugSecondTouchMoveKeyProp		= so.FindProperty("debugSecondTouchMoveKey");
	//this.debugSecondTouchFlipXKeyProp		= so.FindProperty("debugSecondTouchFlipXKey");
	//this.debugSecondTouchFlipYKeyProp		= so.FindProperty("debugSecondTouchFlipYKey");

	this.debugSecondTouchDragModeKeyProp	= so.FindProperty("debugSecondTouchDragModeKey");
	this.debugSecondTouchPinchModeKeyProp	= so.FindProperty("debugSecondTouchPinchModeKey");
	this.debugSecondTouchTwistModeKeyProp	= so.FindProperty("debugSecondTouchTwistModeKey");

	//this.debugSecondTouchMoveKeyPressToBlockProp	= so.FindProperty("debugSecondTouchMoveKeyPressToBlock");
	this.debugDrawTouchesProp				= so.FindProperty("debugDrawTouches");
	this.debugDrawLayoutBoxesProp			= so.FindProperty("debugDrawLayoutBoxes");
	//this.debugDrawAreasProp					= so.FindProperty("debugDrawAreas");
	this.debugTouchSpriteProp				= so.FindProperty("debugTouchSprite");
	this.debugSecondTouchSpriteProp			= so.FindProperty("debugSecondTouchSprite");
	this.debugFirstTouchNormalColorProp		= so.FindProperty("debugFirstTouchNormalColor");
	this.debugFirstTouchActiveColorProp		= so.FindProperty("debugFirstTouchActiveColor");
	this.debugSecondTouchNormalColorProp	= so.FindProperty("debugSecondTouchNormalColor");
	this.debugSecondTouchActiveColorProp	= so.FindProperty("debugSecondTouchActiveColor");
	//this.debugFrameStyleProp				= so.FindProperty("debugFrameStyle");
	this.debugCircleImgProp					= so.FindProperty("debugCircleImg");
	this.debugRectImgProp					= so.FindProperty("debugRectImg");
	//this.debugFrameStyleProp				= so.FindProperty("debugFrameStyle");

		

	// Upgrade if needed...
	
	//this.UpgradeIfNeeded(joy);

	

	// Prepare styles...

	this.editSectionStyle = new GUIStyle(); //EditorStyles.popup);
	this.sectionToolbarStyle = new GUIStyle(); //EditorStyles.toolbar);

	this.editSectionStyle.fixedHeight = 0;
	this.editSectionStyle.fixedWidth = 0;
	this.editSectionStyle.stretchWidth = false;
	this.editSectionStyle.stretchHeight = false;
	this.editSectionStyle.padding = new RectOffset(10, 2, 4, 4);

	this.sectionToolbarStyle.fixedHeight = 0;
	this.sectionToolbarStyle.fixedWidth = 0;
	this.sectionToolbarStyle.stretchWidth = false;
	this.sectionToolbarStyle.stretchHeight = false;
	this.sectionToolbarStyle.padding = new RectOffset(2, 2, 4, 4);

	this.sectionFoldoutStyle = new GUIStyle();
	this.sectionFoldoutStyle.fixedHeight = 0;
	this.sectionFoldoutStyle.fixedWidth = 0;
	this.sectionFoldoutStyle.stretchWidth = false;
	this.sectionFoldoutStyle.stretchHeight = false;
	//this.sectionFoldoutStyle.padding = new RectOffset(0, 2, 4, 4);
	//this.sectionFoldoutStyle.font	= EditorStyles.boldFont;

	//if (EditorStyles.toolbar != null)
	//	this.sectionToolbarStyle.normal.background = EditorStyles.toolbar.normal.background; 
	//else 
	//	Debug.Log("NULL TOOLBAR!");
	

	// Start...

	//this.SelectButton(this.button_curId);		
	this.SelectStick(this.stick_curId);	
	this.SelectZone(this.zone_curId);	
	this.SelectLayoutBox(this.layoutBox_curId);

	this.OnLayoutBoxNameChange();
	
	this.OnDefaultColorsChange();
	}

	



// ---------------------
override public void OnInspectorGUI()
	{
	TouchController joy = this.target as TouchController;
	if (joy == null)
		return;
		
	//if (GUILayout.Button("Force Layout"))
	//	joy.SetLayoutDirtyFlag();

	bool screenEmuParamsChanged = false;		
		

	this.serializedObject.Update();

	
#if UNITY_3_5
	EditorGUIUtility.LookLikeInspector();
#endif

	EditorGUILayout.Space();

	GUI.backgroundColor = new Color(0.7f, 1.0f, 0.7f, 1);

	if (GUILayout.Button("Generate Code"))
		{
		//TouchControllerConstGenerator tccg = 
		//	ScriptableObject.CreateInstance<TouchControllerConstGenerator>();
		//tccg.InitAndShow(joy);

		TouchControllerCodeTemplateGenerator tccg = 
			ScriptableObject.CreateInstance<TouchControllerCodeTemplateGenerator>();
		tccg.InitAndShow(joy);


		}
		
	GUI.backgroundColor = Color.white;


	DrawEnumPopup(this.rwUnitProp, "Display Units", "Physical length units.");
	
	if (DrawEnumPopup(this.previewModeProp, "Preview Mode", 
		"Control state preview mode (used when not playing)."))
		{ 
		screenEmuParamsChanged = true; 
		this.OnDefaultColorsChange();
		}
	
	EditorGUILayout.Space();


	// Debug Section ...

	//if (showDebugSection = EditorGUILayout.Foldout(showDebugSection, "Debug Settings"))
		{
	

		//EditorGUILayout.Space();

		

	// General Settings ...
	
	if (showSettingsSection = EditorGUILayout.Foldout(showSettingsSection, 
		"General Settings")) //this.sectionFoldoutStyle))
		{
		DrawCheckBox(this.automaticModeProp, "Automatic", "Automatic initialization and updates");
		if (this.automaticModeProp.boolValue)
			DrawCheckBox(this.manualGuiProp, "Manual GUI", "When enabled, DrawControllerGUI() must be called manually. Use this when there are other scripts implementing OnGUI() in the scene. Unity doesn't respect GUI Depth of elements drawn from different OnGUI() and incorrect rendering order may occur.");
				
		
		DrawCheckBox(this.autoActivateProp, "Set as Active", "Automatically set this controller as active (CFInput.ctrl)"); 
		DrawCheckBox(this.disableWhenNoTouchScreenProp, "Auto Disable", "Disactivate this controller when the hardware doesn't support multi-touch input.");


		//if (this.automaticModeProp.boolValue)
		//	DrawCheckBox(this.fixedUpdateModeProp, "Fixed Update", "Automatic update will take place every FixedUpdate() cycle, instead of Update()");

		DrawInt(this.guiDepthProp, 		"GUI Depth", 		"Base GUI Depth for all controls");
		DrawInt(this.guiPressedOfsProp, "GUI Pressed Ofs", 	"GUI Depth offset for controls in their pressed state");


		DrawCmField(this.fingerBufferCmProp, 0, 2, "Finger size", "Finger size");
		DrawFloatEx(this.stickMagnetAngleMarginProp, 0, 11.5f, "Stick thresh (deg)", "Angle threshold when changing digital direction.");
		DrawSlider(this.stickDigitalEnterThreshProp, 0.1f, 0.9f, "Stick enter tilt", "Digital-mode enter tilt threshold (from neutral to non-neutral).");
		DrawSlider(this.stickDigitalLeaveThreshProp, 0.1f, 0.9f, "Stick leave tilt", "Digital-mode leave tilt threshold (from non-neutral to neutral).");

		DrawFloatEx(this.touchTapMaxTimeProp, (1.0f / 60.0f), 1, "Tap max time", "Maximum time in seconds for touch to be treated as a quick tap");
		DrawFloatEx(this.doubleTapMaxGapTimeProp, (1.0f / 60.0f), 1, "Dbl Tap max gap", "Maximum gap time between taps to count as a double tap");
		//DrawFloatEx(this.touchHoldMinTimeProp, (1.0f / 60.0f), 1, "Hold min time", "Minimum time in seconds for touch to be treated as a long hold");

		DrawFloatEx(this.strictMultiFingerMaxTimeProp, (1.0f/60.0f), 1, "Strict Multi Max Time", "Maximal allowed time for the second finger to touch the zone after the first one to start a multi-touch.");
		DrawFloatEx(this.velPreserveTimeProp,	0, 4, "Vel Preserv. Time", "Amount of time that last non-zero drag velocity will be preserved after movement stopped. Use this to velocity-based swapping easier on those high-pixel-density, low-touch-precision devices.");  

		DrawCmField(this.touchTapMaxDistCmProp, 0, 2, "Drag Thresh", "Maximum allowed drag distance for a touch to be considered static.");
		DrawCmField(this.pinchMinDistCmProp, 0, 2, "Pinch Dist Thresh", "Minimal finger distance difference to activate pinch.");
		DrawCmField(this.twistSafeFingerDistCmProp, 0, 2, "Twist Safe Dist", "Minimal finger distance for twist detection.");



	

	// Global colors and images...


	DrawFloat(this.pressAnimDurationProp, 	"Press duration", "Global press animation duration in seconds"); 
	DrawFloat(this.releaseAnimDurationProp, "Release duration", "Global release animation duration in seconds"); 
	DrawFloat(this.disableAnimDurationProp, "Disable duration", "Global disable animation duration in seconds"); 
	DrawFloat(this.enableAnimDurationProp, 	"Enable duration", "Global enable animation duration in seconds"); 
	DrawFloat(this.showAnimDurationProp,	"Show duration", "Global showing animation duration in seconds"); 
	DrawFloat(this.hideAnimDurationProp, 	"Hide duration", "Global hidding animation duration in seconds"); 
	DrawFloat(this.cancelAnimDurationProp,	"Cancel duration", "Global canceling animation duration in seconds"); 
	
	if (this.DrawColorProp(this.defaultPressedZoneColorProp, 	"Zone Pressed", 	"Default Zone Pressed Color")) { this.OnDefaultColorsChange(); }
	if (this.DrawColorProp(this.defaultReleasedZoneColorProp, 	"Zone Released", 	"Default Zone Released Color")) { this.OnDefaultColorsChange(); }
	if (this.DrawColorProp(this.defaultDisabledZoneColorProp, 	"Zone Disabled",	"Default Zone Disabled Color")) { this.OnDefaultColorsChange(); }

	if (this.DrawColorProp(this.defaultPressedStickBaseColorProp, 	"Stick Base Pressed", 	"Default Stick Base Pressed Color")) { this.OnDefaultColorsChange(); }
	if (this.DrawColorProp(this.defaultReleasedStickBaseColorProp, 	"Stick Base Released", 	"Default Stick Base Released Color")) { this.OnDefaultColorsChange(); }
	if (this.DrawColorProp(this.defaultDisabledStickBaseColorProp, 	"Stick Base Disabled",	"Default Stick Base Disabled Color")) { this.OnDefaultColorsChange(); }

	if (this.DrawColorProp(this.defaultPressedStickHatColorProp, 	"Stick Hat Pressed", 	"Default Stick Hat Pressed Color")) { this.OnDefaultColorsChange(); }
	if (this.DrawColorProp(this.defaultReleasedStickHatColorProp, 	"Stick Hat Released", 	"Default Stick Hat Released Color")) { this.OnDefaultColorsChange(); }
	if (this.DrawColorProp(this.defaultDisabledStickHatColorProp, 	"Stick Hat Disabled",	"Default Stick Hat Disabled Color")) { this.OnDefaultColorsChange(); }

	if (DrawSlider(this.releasedZoneScaleProp,	0, 2, "Zone Released Scale", "Global Zone's scale when released.")) { this.OnDefaultColorsChange(); }				
	if (DrawSlider(this.pressedZoneScaleProp, 	0, 2, "Zone Pressed Scale", "Global Zone's scale when pressed.")) { this.OnDefaultColorsChange(); }				
	if (DrawSlider(this.disabledZoneScaleProp, 	0, 2, "Zone Disabled Scale", "Global Zone's scale when disabled.")) { this.OnDefaultColorsChange(); }				

	if (DrawSlider(this.releasedStickHatScaleProp,	0, 2, "Hat Released Scale", "Global Stick Hat's scale when released.")) { this.OnDefaultColorsChange(); }				
	if (DrawSlider(this.pressedStickHatScaleProp, 	0, 2, "Hat Pressed Scale", "Global Stick Hat's scale when pressed.")) { this.OnDefaultColorsChange(); }				
	if (DrawSlider(this.disabledStickHatScaleProp, 	0, 2, "Hat Disabled Scale", "Global Stick Hat's scale when disabled.")) { this.OnDefaultColorsChange(); }				

	if (DrawSlider(this.releasedStickBaseScaleProp,	0, 2, "Base Released Scale", "Global Stick Base's scale when released.")) { this.OnDefaultColorsChange(); }				
	if (DrawSlider(this.pressedStickBaseScaleProp, 	0, 2, "Base Pressed Scale", "Global Stick Base's scale when pressed.")) { this.OnDefaultColorsChange(); }				
	if (DrawSlider(this.disabledStickBaseScaleProp, 0, 2, "Base Disabled Scale", "Global Stick Base's scale when disabled.")) { this.OnDefaultColorsChange(); }				



		// stick dead zones, magnet angles

		EditorGUILayout.Space();
		}
		

	


	// Layout Boxes Settings ...

	if (showLayoutBoxes = EditorGUILayout.Foldout(showLayoutBoxes, "Layout"))
		{
		this.DrawLayoutBoxEditorSection();		
		

//	this.layoutBoxesProp					= so.FindProperty("layoutBoxes");
		

		EditorGUILayout.Space();
		}


			/*
	// Button Settings ...

	if (showButtons = EditorGUILayout.Foldout(showButtons, "Button Settings"))
		{
		this.DrawButtonEditorSection();
		EditorGUILayout.Space();
		}
			 */


	// Stick Settings ...

	if (showSticks = EditorGUILayout.Foldout(showSticks, "Sticks"))
		{
		this.DrawStickEditorSection();
		EditorGUILayout.Space();
		}


	// Gesture Zones Settings ...

	if (showGestureZones = EditorGUILayout.Foldout(showGestureZones, "Touch Zones"))
		{
		this.DrawZoneEditorSection();
		EditorGUILayout.Space();
		}
			
	if (showScreenEmu = EditorGUILayout.Foldout(showScreenEmu, "Screen Emulation"))
		{
		// Screen Emulation Settings...



		if (DrawCheckBox(this.screenEmuOnProp, "Enable Screen Emulation"))
			{
			joy.SetLayoutDirtyFlag();
			screenEmuParamsChanged = true;
			}

		if (joy.screenEmuOn)
			{
			if (GUILayout.Button(new GUIContent("Load Preset", "Load resolution and DPI preset.")))
				this.ShowPresetMenu();

			EditorGUILayout.Space();
				
					
			if (DrawFloatEx(this.monitorDiagonalProp, 10, 65, "Monitor's Diagonal", "Your monitor's diagonal in inches, used to calculate monitor's DPI.")) { joy.SetLayoutDirtyFlag(); screenEmuParamsChanged = true; }
			//if (DrawIntEx(this.monitorDpiProp, 30, 1000, "Monitor DPI", "Your monitor's dots-per-inch")) joy.SetLayoutDirtyFlag();

			if (DrawEnumPopup(this.screenEmuModeProp, 	"Mode", "Choose how the virtual screen will be displayed on your monitor.")) { joy.SetLayoutDirtyFlag(); screenEmuParamsChanged = true; }

			if (DrawCheckBox(this.screenEmuPortraitProp, 	"Portrait Mode", "Simulate portrait mode - swaps horizontal and vertical resolutions."))	{ joy.SetLayoutDirtyFlag(); screenEmuParamsChanged = true; }
			if (DrawCheckBox(this.screenEmuShrinkProp, 		"Shrink to fit", "Scales down the virtual screen if it doesn't fit on the screen.")){ joy.SetLayoutDirtyFlag(); screenEmuParamsChanged = true; }
			//if (DrawEnumPopup(this.screenEmuAnchorProp,		"Anchor Point"))	joy.SetLayoutDirtyFlag();
			DrawGenericProp(this.screenEmuBorderColorProp, 		"Border Color", "Color of the emulated screen's border when displayed correctly."); 
			DrawGenericProp(this.screenEmuBorderBadColorProp, 	"Border Bad Color", "Color of the emulated screen's border when displayed scaled down."); 
					
			if (DrawVec2SlidersProp(this.screenEmuPanProp, 0,1,0,1, 	"Pan", "Pan the virtual screen around the window."))	{ joy.SetLayoutDirtyFlag(); screenEmuParamsChanged = true; }

			if (DrawIntEx(this.screenEmuHwHorzResProp, 	32, 8192,	"Width (px)",	"Emulated hardware's horizontal resolution."))	{ joy.SetLayoutDirtyFlag(); screenEmuParamsChanged = true; }
			if (DrawIntEx(this.screenEmuHwVertResProp,	32, 8192, 	"Height (px)",	"Emulated hardware's vertical resolution."))	{ joy.SetLayoutDirtyFlag(); screenEmuParamsChanged = true; }
			if (DrawIntEx(this.screenEmuHwDpiProp, 		10, 1000,	"PPI",			"Emulated hardware's pixel density (dots per inch)."))		{ joy.SetLayoutDirtyFlag(); screenEmuParamsChanged = true; }
			DrawFloatLabel("PPCM", Mathf.RoundToInt(((float)this.screenEmuHwDpiProp.intValue / 2.54f)));
		
		
			DrawPixValInRealWorldUnitsLabel("Width", joy.screenEmuHwHorzRes, 1);
			DrawPixValInRealWorldUnitsLabel("Height", joy.screenEmuHwVertRes, 1);
			DrawPixValInRealWorldUnitsLabel("Diagonal", 
				Mathf.Sqrt((joy.screenEmuHwHorzRes * joy.screenEmuHwHorzRes) +
					(joy.screenEmuHwVertRes * joy.screenEmuHwVertRes)), 1);
					
			DrawStringLabel("Aspect Ratio", this.GetAspectRatioName(this.screenEmuHwHorzResProp.intValue, this.screenEmuHwVertResProp.intValue));

			DrawFloatLabel("Preview Scale (%)", Mathf.Round(100.0f * joy.GetPreviewScale()));



			}


		EditorGUILayout.Space();
		}
		

		
	// Second Touch Emulation Settings...

	if (showDebugSecondTouch = EditorGUILayout.Foldout(showDebugSecondTouch, 
		"Second Touch Emulation"))
		{
		DrawGenericProp(this.debugSecondTouchDragModeKeyProp, "Move Mode Key", "When pressed, second finger copies the movement of the mouse.");
		DrawGenericProp(this.debugSecondTouchPinchModeKeyProp, "Pinch Mode Key", "When pressed, second finger mirrors horizontal movement of the mouse, allowing horizontal pinch.");
		DrawGenericProp(this.debugSecondTouchTwistModeKeyProp, "Twist Mode Key", "When pressed, second finger mirrors horizontal and vertical movement of the mouse allowing twist.");

		//DrawGenericProp(this.debugSecondTouchMoveKeyProp, 	"Move Enable Key");
		//DrawGenericProp(this.debugSecondTouchFlipXKeyProp, 	"Flip X Key");
		//DrawGenericProp(this.debugSecondTouchFlipYKeyProp, 	"Flip Y Key");
		//DrawGenericProp(this.debugSecondTouchMoveKeyPressToBlockProp, "Hold Move Key to block");

		EditorGUILayout.Space();
		}


	// Debug Images and Colors...

	//EditorGUILayout.Space();

	if (showDebugImages = EditorGUILayout.Foldout(showDebugImages, "Debug Images And Colors"))
		{
		//EditorGUIUtility.LookLikeInspector();

		DrawGenericProp(this.debugDrawTouchesProp, 				"Draw Touches",			"Draw touch markers.");
		DrawGenericProp(this.debugDrawLayoutBoxesProp, 			"Draw Layout Boxes",	"Draw layout box regions.");
		//DrawGenericProp(this.debugDrawAreasProp, 				"Draw Stick Areas",	);
		DrawGenericProp(this.debugFirstTouchNormalColorProp, 	"1st touch Normal", "First finger's neutral color.");
		DrawGenericProp(this.debugFirstTouchActiveColorProp, 	"1st touch Active", "First finger's active color.");
		DrawGenericProp(this.debugSecondTouchNormalColorProp, 	"2nd touch Normal", "Second finger's neutral color.");
		DrawGenericProp(this.debugSecondTouchActiveColorProp, 	"2nd touch Active", "Second finger's active color.");
		DrawGenericProp(this.debugTouchSpriteProp, 				"Primary Touch img",	"Texture used to mark primary debug touch's position.");
		DrawGenericProp(this.debugSecondTouchSpriteProp, 		"Second Touch img",	"Texture used to mark second debug touch's position.");
		DrawGenericProp(this.debugCircleImgProp, 				"Circle img",	"Circular texture used to draw debug GUI.");
		DrawGenericProp(this.debugRectImgProp, 					"Rect img",		"Rectangular texture used to draw debug GUI.");
				
		DrawGenericProp(this.defaultZoneImgProp,				"Default Zone Img",			"Default zone image applied to all newly created zones.");
		DrawGenericProp(this.defaultStickHatImgProp,			"Default Stick Hat Img",	"Default stick hat image applied to all newly created sticks.");
		DrawGenericProp(this.defaultStickBaseImgProp,			"Default Stick Base Img",	"Default stick base image applied to all newly created sticks.");


		//DrawGenericProp(this.screenEmuBorderColorProp, 			"Screen Border");
		
		//EditorGUILayout.PropertyField(this.debugFrameStyleProp);
		//DrawGenericProp(this.debugFrameStyleProp, 				"Border Style");

		//EditorGUIUtility.LookLikeControls();
			

		EditorGUILayout.Space();
		}

	//EditorGUILayout.Space();
	}



		


	// End GUI...
	
	EditorGUILayout.Space();
	this.serializedObject.ApplyModifiedProperties();
		


	// Safety Update...

	//if (!EditorApplication.isPlaying)
	//	joy.OnEditorUpdate();			


	// Save screen emu config to editor config file..

	if (screenEmuParamsChanged)
		joy.SaveScreenEmuConfig();


		
	//if (drawDefaultInspector = EditorGUILayout.Toggle("Default Inspector", drawDefaultInspector)) 
	//	this.DrawDefaultInspector();
	}
		


// -------------------
private void OnDefaultColorsChange()
	{	
	TouchController tc = this.target as TouchController;	
	
	tc.OnGlobalColorChange();	
	}



// *************************

// ---------------------
// Stick Editor -------
// ---------------------

private SerializedProperty
	//stick_enabledProp,
	//stick_visibleProp,
	stick_initiallyDisabledProp,
	stick_initiallyHiddenProp,

	stick_prioProp,

	stick_hitDistScaleProp,
	stick_acceptSharedTouchesProp,
	stick_guiDepthProp,
	stick_disableGuiProp,
	stick_overrideAnimDurationProp,
	stick_pressAnimDurationProp,
	stick_releaseAnimDurationProp,
	stick_disableAnimDurationProp,
	stick_enableAnimDurationProp,
	stick_hideAnimDurationProp,
	stick_showAnimDurationProp,
		
	stick_overrideColorsProp,

	stick_nameProp,
	stick_layoutBoxIdProp,
	//stick_shapeProp,
	stick_posCmProp,
	stick_sizeCmProp,		// float, not vec2!
	//stick_pressedHatScaleProp,
	//stick_pressedBaseScaleProp,
	


	stick_stickVisProp,
	stick_smoothReturnProp,
	stick_dynamicModeProp,
	stick_dynamicRegionPrioProp,
	stick_dynamicClampProp,
	stick_dynamicMaxRelativeSizeProp,
	stick_dynamicMarginCmProp,
	//stick_dynamicFadeOutDelayProp,
	stick_dynamicFadeOutDurationProp,
	stick_dynamicRegionProp,
	//stick_dynamicFadeInDurationProp,
	stick_dynamicAlwaysResetProp,
	//stick_hatScaleProp,
	stick_hatMoveScaleProp,
		
	stick_disableXProp,
	stick_disableYProp,

	stick_releasedHatImgProp,
	stick_releasedHatColorProp,
	stick_releasedBaseImgProp,
	stick_releasedBaseColorProp,
	stick_pressedHatImgProp,
	stick_pressedHatColorProp,
	stick_pressedBaseImgProp,
	stick_pressedBaseColorProp,

	stick_overrideScaleProp,
	stick_releasedHatScaleProp,
	stick_pressedHatScaleProp,
	stick_disabledHatScaleProp,
	stick_releasedBaseScaleProp,
	stick_pressedBaseScaleProp,
	stick_disabledBaseScaleProp,

	stick_disabledBaseColorProp,
	stick_disabledHatColorProp,

	stick_enableGetKeyProp,
	stick_getKeyCodePressProp,
	stick_getKeyCodePressAltProp,
	stick_getKeyCodeUpProp,
	stick_getKeyCodeUpAltProp,
	stick_getKeyCodeDownProp,
	stick_getKeyCodeDownAltProp,
	stick_getKeyCodeLeftProp,
	stick_getKeyCodeLeftAltProp,
	stick_getKeyCodeRightProp,
	stick_getKeyCodeRightAltProp,
	stick_enableGetButtonProp,
	stick_getButtonNameProp,
	stick_enableGetAxisProp,
	stick_axisHorzNameProp,
	stick_axisVertNameProp,
	stick_axisHorzFlipProp,
	stick_axisVertFlipProp,
	stick_codeCustomGUIProp,
	stick_codeCustomLayoutProp;


	
private int stick_curId;

// --------------------
private void SelectStick(int i)
	{
	//UnfocusControls();

	this.sticksProp = this.serializedObject.FindProperty("sticks");

	if ((this.sticksProp == null) || (this.sticksProp.arraySize == 0))
		{	

		this.stick_initiallyDisabledProp	= null;
		this.stick_initiallyHiddenProp		= null;
		//this.stick_enabledProp				= null;
		//this.stick_visibleProp				= null;
		this.stick_prioProp					= null;
		this.stick_hitDistScaleProp			= null;
		this.stick_acceptSharedTouchesProp	= null;
		this.stick_guiDepthProp				= null;
		this.stick_overrideAnimDurationProp	= null;
		this.stick_pressAnimDurationProp	= null;
		this.stick_releaseAnimDurationProp	= null;
		this.stick_disableAnimDurationProp	= null;
		this.stick_enableAnimDurationProp	= null;

		this.stick_overrideColorsProp		= null;

		this.stick_nameProp					= null;
		this.stick_layoutBoxIdProp			= null;
		//this.stick_shapeProp				= null;
		this.stick_posCmProp				= null;
		this.stick_sizeCmProp				= null;
		this.stick_pressedHatScaleProp		= null;
		this.stick_pressedBaseScaleProp		= null;
		this.stick_stickVisProp				= null;
		this.stick_dynamicModeProp			= null;
		this.stick_dynamicClampProp			= null;
		this.stick_dynamicMaxRelativeSizeProp= null;
		this.stick_dynamicMarginCmProp		= null;
		//this.stick_dynamicFadeOutDelayProp	= null;
		this.stick_dynamicFadeOutDurationProp= null;
		this.stick_dynamicRegionProp		= null;
		//this.stick_dynamicFadeInDurationProp= null;
		this.stick_dynamicAlwaysResetProp	= null;
		//this.stick_hatScaleProp				= null;
		this.stick_hatMoveScaleProp			= null;
		this.stick_releasedHatImgProp		= null;
		this.stick_releasedHatColorProp		= null;
		this.stick_releasedBaseImgProp		= null;
		this.stick_releasedBaseColorProp	= null;
		this.stick_pressedHatImgProp		= null;
		this.stick_pressedHatColorProp		= null;
		this.stick_pressedBaseImgProp		= null;
		this.stick_pressedBaseColorProp		= null;
		this.stick_disabledBaseColorProp	= null;
		this.stick_disabledHatColorProp		= null;
	

		return;
		}

	if (i < 0) 
		i = 0;		
	else if (i >= this.sticksProp.arraySize) 
		i = (this.sticksProp.arraySize - 1);

	this.stick_curId = i;

	SerializedProperty b = this.sticksProp.GetArrayElementAtIndex(i);

	this.stick_initiallyDisabledProp	= b.FindPropertyRelative("initiallyDisabled");
	this.stick_initiallyHiddenProp		= b.FindPropertyRelative("initiallyHidden");
	//this.stick_enabledProp				= b.FindPropertyRelative("enabled");
	//this.stick_visibleProp				= b.FindPropertyRelative("visible");
	this.stick_prioProp					= b.FindPropertyRelative("prio");
	this.stick_hitDistScaleProp			= b.FindPropertyRelative("hitDistScale");
	this.stick_acceptSharedTouchesProp	= b.FindPropertyRelative("acceptSharedTouches");
	this.stick_guiDepthProp				= b.FindPropertyRelative("guiDepth");
	this.stick_disableGuiProp			= b.FindPropertyRelative("disableGui");
	this.stick_overrideAnimDurationProp	= b.FindPropertyRelative("overrideAnimDuration");
	this.stick_pressAnimDurationProp	= b.FindPropertyRelative("pressAnimDuration");
	this.stick_releaseAnimDurationProp	= b.FindPropertyRelative("releaseAnimDuration");
	this.stick_disableAnimDurationProp	= b.FindPropertyRelative("disableAnimDuration");
	this.stick_enableAnimDurationProp	= b.FindPropertyRelative("enableAnimDuration");
	this.stick_hideAnimDurationProp		= b.FindPropertyRelative("hideAnimDuration");
	this.stick_showAnimDurationProp		= b.FindPropertyRelative("showAnimDuration");
	this.stick_overrideColorsProp		= b.FindPropertyRelative("overrideColors");
	

	this.stick_nameProp					= b.FindPropertyRelative("name");
	this.stick_layoutBoxIdProp			= b.FindPropertyRelative("layoutBoxId");
	//this.stick_shapeProp				= b.FindPropertyRelative("shape");
	this.stick_posCmProp				= b.FindPropertyRelative("posCm");
	this.stick_sizeCmProp				= b.FindPropertyRelative("sizeCm");
	this.stick_pressedHatScaleProp		= b.FindPropertyRelative("pressedHatScale");
	this.stick_pressedBaseScaleProp		= b.FindPropertyRelative("pressedBaseScale");

	this.stick_stickVisProp				= b.FindPropertyRelative("stickVis");
	this.stick_smoothReturnProp			= b.FindPropertyRelative("smoothReturn");	
	this.stick_dynamicModeProp			= b.FindPropertyRelative("dynamicMode");
	this.stick_dynamicRegionPrioProp	= b.FindPropertyRelative("dynamicRegionPrio");
	this.stick_dynamicClampProp			= b.FindPropertyRelative("dynamicClamp");
	this.stick_dynamicMaxRelativeSizeProp=b.FindPropertyRelative("dynamicMaxRelativeSize");
	this.stick_dynamicMarginCmProp		= b.FindPropertyRelative("dynamicMarginCm");
	//this.stick_dynamicFadeOutDelayProp	= b.FindPropertyRelative("dynamicFadeOutDelay");
	this.stick_dynamicFadeOutDurationProp=b.FindPropertyRelative("dynamicFadeOutDuration");
	this.stick_dynamicRegionProp		= b.FindPropertyRelative("dynamicRegion");
	//this.stick_dynamicFadeInDurationProp= b.FindPropertyRelative("dynamicFadeInDuration");
	this.stick_dynamicAlwaysResetProp	= b.FindPropertyRelative("dynamicAlwaysReset");
	this.stick_hatMoveScaleProp			= b.FindPropertyRelative("hatMoveScale");
	//this.stick_hatScaleProp				= b.FindPropertyRelative("hatScale");
		
	this.stick_disableXProp				= b.FindPropertyRelative("disableX");
	this.stick_disableYProp				= b.FindPropertyRelative("disableY");


	this.stick_releasedHatImgProp		= b.FindPropertyRelative("releasedHatImg");
	this.stick_releasedHatColorProp		= b.FindPropertyRelative("releasedHatColor");
	this.stick_releasedBaseImgProp		= b.FindPropertyRelative("releasedBaseImg");
	this.stick_releasedBaseColorProp	= b.FindPropertyRelative("releasedBaseColor");
	this.stick_pressedHatImgProp		= b.FindPropertyRelative("pressedHatImg");
	this.stick_pressedHatColorProp		= b.FindPropertyRelative("pressedHatColor");
	this.stick_pressedBaseImgProp		= b.FindPropertyRelative("pressedBaseImg");
	this.stick_pressedBaseColorProp		= b.FindPropertyRelative("pressedBaseColor");
	this.stick_disabledBaseColorProp	= b.FindPropertyRelative("disabledBaseColor");
	this.stick_disabledHatColorProp		= b.FindPropertyRelative("disabledHatColor");

	this.stick_overrideScaleProp		= b.FindPropertyRelative("overrideScale");
	this.stick_releasedHatScaleProp		= b.FindPropertyRelative("releasedHatScale");
	this.stick_pressedHatScaleProp		= b.FindPropertyRelative("pressedHatScale");
	this.stick_disabledHatScaleProp		= b.FindPropertyRelative("disabledHatScale");
	this.stick_releasedBaseScaleProp	= b.FindPropertyRelative("releasedBaseScale");
	this.stick_pressedBaseScaleProp		= b.FindPropertyRelative("pressedBaseScale");
	this.stick_disabledBaseScaleProp	= b.FindPropertyRelative("disabledBaseScale");

	this.stick_enableGetKeyProp			= b.FindPropertyRelative("enableGetKey");
	this.stick_getKeyCodePressProp		= b.FindPropertyRelative("getKeyCodePress");
	this.stick_getKeyCodePressAltProp	= b.FindPropertyRelative("getKeyCodePressAlt");
	this.stick_getKeyCodeUpProp			= b.FindPropertyRelative("getKeyCodeUp");
	this.stick_getKeyCodeUpAltProp		= b.FindPropertyRelative("getKeyCodeUpAlt");
	this.stick_getKeyCodeDownProp		= b.FindPropertyRelative("getKeyCodeDown");
	this.stick_getKeyCodeDownAltProp	= b.FindPropertyRelative("getKeyCodeDownAlt");
	this.stick_getKeyCodeLeftProp		= b.FindPropertyRelative("getKeyCodeLeft");
	this.stick_getKeyCodeLeftAltProp	= b.FindPropertyRelative("getKeyCodeLeftAlt");
	this.stick_getKeyCodeRightProp		= b.FindPropertyRelative("getKeyCodeRight");
	this.stick_getKeyCodeRightAltProp	= b.FindPropertyRelative("getKeyCodeRightAlt");
	this.stick_enableGetButtonProp		= b.FindPropertyRelative("enableGetButton");
	this.stick_getButtonNameProp		= b.FindPropertyRelative("getButtonName");
	this.stick_enableGetAxisProp		= b.FindPropertyRelative("enableGetAxis");
	this.stick_axisHorzNameProp			= b.FindPropertyRelative("axisHorzName");
	this.stick_axisVertNameProp			= b.FindPropertyRelative("axisVertName");
	this.stick_axisHorzFlipProp			= b.FindPropertyRelative("axisHorzFlip");
	this.stick_axisVertFlipProp			= b.FindPropertyRelative("axisVertFlip");

				
	this.stick_codeCustomGUIProp		= b.FindPropertyRelative("codeCustomGUI");
	this.stick_codeCustomLayoutProp		= b.FindPropertyRelative("codeCustomLayout");

//SerializedProperty lkp = this.stick_getKeyCodeLeftAltProp;
//Debug.Log("ENUM ["+System.DateTime.Now.ToLongTimeString() +"] LeftArrow:" + (int)KeyCode.LeftArrow + 
//	" left Key Index: " + lkp.intValue.ToString() + 
//	" left key INT : " + lkp.intValue.ToString() + 
//	" (names: [" + lkp.enumNames.Length + "] = {" + lkp.enumNames[lkp.intValue] + ")");
	}


// ---------------------
private void ResetCurrentStick()
	{	
	if (this.stick_prioProp == null)
		return;
	
	UnfocusControls();

	TouchController joy = this.target as TouchController;
	if (joy == null)
		return;


	this.stick_initiallyDisabledProp.boolValue	= false;	
	this.stick_initiallyHiddenProp.boolValue	= false;
	//this.stick_enabledProp.boolValue		= true;	
	//this.stick_visibleProp.boolValue		= true;
	this.stick_prioProp.intValue			= TouchController.DEFAULT_STICK_PRIO;
	this.stick_hitDistScaleProp.floatValue	= 1.0f;
	this.stick_acceptSharedTouchesProp.boolValue = true;
	this.stick_guiDepthProp.intValue		= 0;
	this.stick_disableGuiProp.boolValue		= false;
	this.stick_nameProp.stringValue			= "STICK";
	//this.stick_layoutBoxIdProp.intValue	=(int)TouchController.LayoutBoxId.NONE;
	this.stick_layoutBoxIdProp.intValue		= 0; 
	//this.stick_shapeProp.intValue	= (int)TouchController.ControlShape.CIRCLE;
	this.stick_posCmProp.vector2Value		= new Vector2(0, 0);
	this.stick_sizeCmProp.floatValue		= 2;
	this.stick_pressedHatScaleProp.floatValue	= DEFAULT_PRESSED_SCALE;
	this.stick_pressedBaseScaleProp.floatValue	= DEFAULT_STICK_BASE_PRESSED_SCALE;

	this.stick_overrideAnimDurationProp.boolValue	= false;
	this.stick_pressAnimDurationProp.floatValue 	= DEFAULT_PRESS_DURATION;
	this.stick_releaseAnimDurationProp.floatValue 	= DEFAULT_RELEASE_DURATION;
	this.stick_disableAnimDurationProp.floatValue	= DEFAULT_DISABLE_DURATION;
	this.stick_enableAnimDurationProp.floatValue	= DEFAULT_ENABLE_DURATION;


	this.stick_stickVisProp.intValue			= (int)TouchStick.StickPosMode.FULL_ANALOG;
	this.stick_smoothReturnProp.boolValue			= true;
	this.stick_dynamicModeProp.boolValue			= false;
	this.stick_dynamicClampProp.boolValue			= true;
	this.stick_dynamicMaxRelativeSizeProp.floatValue= 0.3f;
	this.stick_dynamicMarginCmProp.floatValue		= 0.5f;
	//this.stick_dynamicFadeOutDelayProp.floatValue	= 0;
	this.stick_dynamicFadeOutDurationProp.floatValue= DEFAULT_STICK_FADEOUT;
	this.stick_dynamicRegionProp.rectValue			= new Rect(0,0, 0.5f, 1.0f);
	//this.stick_dynamicFadeInDurationProp.floatValue	= 0.5f;
	this.stick_dynamicAlwaysResetProp.boolValue		= false;
	//this.stick_hatScaleProp.floatValue				= 1;
	this.stick_hatMoveScaleProp.floatValue			= 0.5f;
	this.stick_releasedHatColorProp.colorValue		= DEFAULT_STICK_HAT_RELEASED_COLOR;
	this.stick_releasedBaseColorProp.colorValue		= DEFAULT_STICK_BASE_RELEASED_COLOR;
	this.stick_pressedHatColorProp.colorValue		= DEFAULT_STICK_HAT_PRESSED_COLOR;
	this.stick_pressedBaseColorProp.colorValue		= DEFAULT_STICK_BASE_PRESSED_COLOR;
	this.stick_disabledHatColorProp.colorValue		= DEFAULT_STICK_HAT_DISABLED_COLOR;
	this.stick_disabledBaseColorProp.colorValue		= DEFAULT_STICK_BASE_DISABLED_COLOR;
	this.stick_overrideColorsProp.boolValue			= false;

	this.stick_overrideScaleProp.boolValue			= false;
	this.stick_releasedHatScaleProp.floatValue		= 0.75f;
	this.stick_pressedHatScaleProp.floatValue		= 0.90f;
	this.stick_disabledHatScaleProp.floatValue		= 0.75f;
	this.stick_releasedBaseScaleProp.floatValue		= 1.0f;
	this.stick_pressedBaseScaleProp.floatValue		= 0.90f;
	this.stick_disabledBaseScaleProp.floatValue		= 1.0f;
	

	this.stick_enableGetKeyProp.boolValue			= false;
	this.stick_getKeyCodePressProp.intValue	= (int)KeyCode.None;
	this.stick_getKeyCodePressAltProp.intValue= (int)KeyCode.None;
	this.stick_getKeyCodeUpProp.intValue		= (int)KeyCode.W;
	this.stick_getKeyCodeUpAltProp.intValue	= (int)KeyCode.UpArrow;
	this.stick_getKeyCodeDownProp.intValue	= (int)KeyCode.S;
	this.stick_getKeyCodeDownAltProp.intValue	= (int)KeyCode.DownArrow;
	this.stick_getKeyCodeLeftProp.intValue	= (int)KeyCode.A;
	this.stick_getKeyCodeLeftAltProp.intValue	= (int)KeyCode.LeftArrow;
	this.stick_getKeyCodeRightProp.intValue	= (int)KeyCode.D;
	this.stick_getKeyCodeRightAltProp.intValue= (int)KeyCode.RightArrow;
	this.stick_enableGetButtonProp.boolValue		= false;
	this.stick_getButtonNameProp.stringValue		= "Fire";
	this.stick_enableGetAxisProp.boolValue			= false;
	this.stick_axisHorzNameProp.stringValue			= "Horizontal";
	this.stick_axisVertNameProp.stringValue			= "Vertical";
	this.stick_axisHorzFlipProp.boolValue			= false;
	this.stick_axisVertFlipProp.boolValue			= false;


	this.stick_codeCustomGUIProp.boolValue			= false;
	this.stick_codeCustomLayoutProp.boolValue		= false;


	// Assign default images...

	this.stick_pressedBaseImgProp.objectReferenceValue =
	this.stick_releasedBaseImgProp.objectReferenceValue = joy.defaultStickBaseImg;
	this.stick_pressedHatImgProp.objectReferenceValue =
	this.stick_releasedHatImgProp.objectReferenceValue = joy.defaultStickHatImg;
	

	// Copy button images from the first one that have any...

	if (joy.sticks != null)
		{
		foreach (TouchStick s in joy.sticks)
			{
			if ((this.stick_releasedHatImgProp.objectReferenceValue == null) && 
				(s.releasedHatImg != null))
				this.stick_releasedHatImgProp.objectReferenceValue = s.releasedHatImg;
			if ((this.stick_pressedHatImgProp.objectReferenceValue == null) && 
				(s.pressedHatImg != null))
				this.stick_pressedHatImgProp.objectReferenceValue = s.pressedHatImg;

			if ((this.stick_releasedBaseImgProp.objectReferenceValue == null) && 
				(s.releasedBaseImg != null))
				this.stick_releasedBaseImgProp.objectReferenceValue = s.releasedBaseImg;
			if ((this.stick_pressedBaseImgProp.objectReferenceValue == null) && 
				(s.pressedBaseImg != null))
				this.stick_pressedBaseImgProp.objectReferenceValue = s.pressedBaseImg;
				
			// Reuse images...

			if ((this.stick_pressedHatImgProp.objectReferenceValue == null) &&
				(this.stick_releasedHatImgProp.objectReferenceValue != null))
				this.stick_pressedHatImgProp.objectReferenceValue = this.stick_releasedHatImgProp.objectReferenceValue;
			if ((this.stick_pressedHatImgProp.objectReferenceValue != null) &&
				(this.stick_releasedHatImgProp.objectReferenceValue == null))
				this.stick_releasedHatImgProp.objectReferenceValue = this.stick_pressedHatImgProp.objectReferenceValue;

			if ((this.stick_pressedBaseImgProp.objectReferenceValue == null) &&
				(this.stick_releasedBaseImgProp.objectReferenceValue != null))
				this.stick_pressedBaseImgProp.objectReferenceValue = 
					this.stick_releasedBaseImgProp.objectReferenceValue;

			if ((this.stick_pressedBaseImgProp.objectReferenceValue != null) &&
				(this.stick_releasedBaseImgProp.objectReferenceValue == null))
				this.stick_releasedBaseImgProp.objectReferenceValue = 
					this.stick_pressedBaseImgProp.objectReferenceValue;

			
			}
		}

	// ... if still nothing found - use ANY texture

	//if (this.stick_imgCapReleasedProp.objectReferenceValue == null)
	//	this.stick_imgReleasedProp.objectReferenceValue = (Object.FindObjectOfType(typeof(Texture2D)) as Texture2D);
	//if (this.stick_imgPressedProp.objectReferenceValue == null)
	//	this.stick_imgPressedProp.objectReferenceValue = this.stick_imgReleasedProp.objectReferenceValue; //EditorGUIUtility.whiteTexture;
	//if (this.stick_imgDisabledProp.objectReferenceValue == null)
	//	this.stick_imgDisabledProp.objectReferenceValue = this.stick_imgReleasedProp.objectReferenceValue; //EditorGUIUtility.whiteTexture;

	}
	

// -------------------
private void MoveStickSelection(int delta)
	{
	UnfocusControls();

	if ((this.sticksProp == null) || (this.sticksProp.arraySize == 0))
		{
		this.SelectStick(0);
		return;		
		}

	this.stick_curId += delta;
	if (this.stick_curId < 0) 
		this.stick_curId = this.sticksProp.arraySize - 1;
	else if (this.stick_curId >= this.sticksProp.arraySize)
		this.stick_curId = 0;

	this.SelectStick(this.stick_curId);	
	}
	
	
// -------------------
private void OnStickColorChange()
	{	
	TouchController tc = this.target as TouchController;	
	TouchStick ts;
	if (tc == null) 
		return;		
	if ((tc.sticks == null) || (this.stick_curId < 0) || 
		(this.stick_curId >= tc.sticks.Length) || 
		((ts = tc.sticks[this.stick_curId]) == null))
		return;
		
	//ts.OnColorChange();
	ts.SetColorsDirtyFlag();
	}



// ---------------------
private void DrawStickEditorSection()
	{	


	//EditorGUIUtility.LookLikeInspector();

	TouchController joy = this.target as TouchController;
	SerializedProperty arrayProp = this.sticksProp;

	EditorGUILayout.BeginHorizontal(this.sectionToolbarStyle, //EditorStyles.toolbar, 
		GUILayout.ExpandWidth(true), GUILayout.Height(30));
	
	if (GUILayout.Button(new GUIContent("<", "Select previous"), EditorStyles.miniButtonLeft, GUILayout.Width(20)))
		this.MoveStickSelection(-1);
	if (GUILayout.Button(new GUIContent(">", "Select next"), EditorStyles.miniButtonRight, GUILayout.Width(20)))
		this.MoveStickSelection(1);

	EditorGUILayout.LabelField((arrayProp.arraySize == 0) ? "-" : 
		("" + (this.stick_curId + 1)) + "/" + arrayProp.arraySize + 
		" : " + ((this.stick_nameProp == null) ? 
		"-" : this.stick_nameProp.stringValue), EditorStyles.miniLabel, 
		GUILayout.Width(90), GUILayout.ExpandWidth(true));

	//EditorGUILayout.EndHorizontal();
	//EditorGUILayout.BeginHorizontal();
		
	if (GUILayout.Button(new GUIContent("<<", "Move left"), EditorStyles.miniButtonLeft, GUILayout.Width(26)))
		{
		UnfocusControls();

		if (this.stick_curId > 0)
			{
			joy.SetContentDirtyFlag();
			arrayProp.MoveArrayElement(this.stick_curId, this.stick_curId - 1);
			this.SelectStick(this.stick_curId - 1);
			//return;
			}
		}
	if (GUILayout.Button(new GUIContent(">>", "Move right"), EditorStyles.miniButtonRight, GUILayout.Width(26)))
		{
		UnfocusControls();
			
		if (this.stick_curId < (arrayProp.arraySize - 1))
			{
			joy.SetContentDirtyFlag();
			arrayProp.MoveArrayElement(this.stick_curId, this.stick_curId + 1);
			this.SelectStick(this.stick_curId + 1);
			//return;
			}
		}


	if (GUILayout.Button(new GUIContent("+", "Add new stick"), EditorStyles.miniButtonLeft, GUILayout.Width(20)))
		{	
		UnfocusControls();
			
		joy.SetContentDirtyFlag();
		if (arrayProp.arraySize == 0)
			{
			arrayProp.arraySize = 1;
			this.SelectStick(0);
			}
		else 
			{
// HACK : Unity 3.X InsertArrayElementAtIndex() insterts new elem AFTER given index, Unity 4 insterts into goven index and pushes extsing to the next index!!

			arrayProp.InsertArrayElementAtIndex((this.stick_curId)); // + 1));
			this.SelectStick(this.stick_curId + 1);
			}

		this.ResetCurrentStick();
		}
	
	
//GUI.color = Color.red;
//GUI.backgroundColor = Color.red;

	if (GUILayout.Button(new GUIContent("-", "Delete stick"), EditorStyles.miniButtonRight, GUILayout.Width(20)))
		{
		UnfocusControls();

		joy.SetContentDirtyFlag();
		if (this.stick_curId < arrayProp.arraySize)
			arrayProp.DeleteArrayElementAtIndex(this.stick_curId);
		this.SelectStick(this.stick_curId);	
		}
	

	EditorGUILayout.EndHorizontal();
		

	// Draw current button's params...

	if ((this.stick_prioProp != null) && (joy.sticks != null) && 
		(this.stick_curId < joy.sticks.Length) && (this.stick_curId >= 0))
		{
		EditorGUILayout.BeginVertical(this.editSectionStyle, /*EditorStyles.popup,*/ 
			GUILayout.ExpandWidth(true));


		TouchStick	ts = joy.sticks[this.stick_curId];
		
			DrawGenericProp	(this.stick_nameProp, 				"Name");
		//if (DrawCheckBox	(this.stick_enabledProp, 			"Enabled"))  {}
		//if (DrawCheckBox	(this.stick_visibleProp, 			"Visible"))  {}
		if (DrawCheckBox	(this.stick_initiallyDisabledProp,	"Initially Disabled", "When enabled, this control will be automatically disabled after initialization."))   { this.OnStickColorChange(); }
		if (DrawCheckBox	(this.stick_initiallyHiddenProp,	"Initially Hidden",		"When enabled, this control will be automatically hidden after initialization."))   { this.OnStickColorChange(); }
		if (DrawInt			(this.stick_prioProp,				"Priority", "Hit-test priority")) {} 

		if (this.stick_dynamicModeProp.boolValue)
			DrawInt			(this.stick_dynamicRegionPrioProp, 	"Dyn. Region Priority", "Separate hit-test priority of empty dynamic region.");


		if (DrawSlider		(this.stick_hitDistScaleProp,		0.5f, 1.5f, "Hit Dist Scale", HIT_DIST_SCALE_TOOLTIP_STR)) {}
		if (DrawCheckBox	(this.stick_acceptSharedTouchesProp, "Shared Touches", "Accept shared touches")) { }
		if (DrawIntEx		(this.stick_guiDepthProp, -100,100,	"GUI Depth", "GUI Depth offset added to controller base depth")) {}
		if (DrawCheckBox	(this.stick_disableGuiProp, 			"Disable GUI", "Disable default GUI rendering of this control")) {}


		if (DrawCheckBox(this.stick_dynamicModeProp, "Dynamic Mode", "")) { joy.SetLayoutDirtyFlag(); }

		if (this.stick_dynamicModeProp.boolValue)
			{
			DrawCheckBox(this.stick_dynamicClampProp,	"Dynamic Clamp", "Clamp stick inside work area.");
			if (DrawCmField		(this.stick_sizeCmProp, 0, 10,		"Size", "Stick's base diameter.")) { joy.SetLayoutDirtyFlag(); }
			if (DrawFloatEx(this.stick_dynamicMaxRelativeSizeProp, 0.01f, 1.0f, "Max Relative Size", "Maximal stick's size relative to screen's shorter dimension."))  { joy.SetLayoutDirtyFlag(); }
			if (DrawCmField(this.stick_dynamicMarginCmProp,	0, 4, "Dynamic Margin", "Dynamic stick's margin from the edge of the screen."))  { joy.SetLayoutDirtyFlag(); }
			if (DrawRectField(this.stick_dynamicRegionProp,	0,0,1,1, "Dynamic Stick Screen Region", "Define dynamic stick's work-area - normalized screen region where stick can be placed.")) { joy.SetLayoutDirtyFlag(); }
			DrawFloatEx(this.stick_dynamicFadeOutDurationProp, 0, 5,	"Fade-out duration", "Dynamic stick's fade-out animation duration in seconds.");
			DrawCheckBox(this.stick_dynamicAlwaysResetProp,	"Always Reset", "Always reset dynamic stick's origin to touched position, even when pressed on still visible stick.");
			}

		// Static mode...

		else
			{
			//if (DrawEnumPopup	(this.stick_layoutBoxIdProp, 		"Layout Box")) { joy.SetLayoutDirtyFlag(); }
			if (DrawLayoutBoxIdProp(this.stick_layoutBoxIdProp, 		"Layout Box")) { joy.SetLayoutDirtyFlag(); }
			//if (DrawEnumPopup	(this.stick_shapeProp,				"Shape")) {}
			if (DrawCmVec2Field	(this.stick_posCmProp, -100, 100,	"Position")) { joy.SetLayoutDirtyFlag(); }
			if (DrawCmField		(this.stick_sizeCmProp, 0, 10,		"Size", "Stick's base diameter.")) { joy.SetLayoutDirtyFlag(); }
	
			}

		EditorGUILayout.FloatField("Actual size (px)", ts.GetScreenRad() * 2);		
		DrawPixValInRealWorldUnitsLabel("Actual size", ts.GetScreenRad() * 2, 2);


		DrawEnumPopup(this.stick_stickVisProp, "Stick Pos Mode", "Control how stick's position is visualized.");
		DrawCheckBox(this.stick_smoothReturnProp, "Smooth Return", "When enabled, stick will smoothly return to neutral position after being released.");

		//if (DrawFloatEx		(this.stick_hatScaleProp, 		0.1f, 4, "Hat Relative Scale", "Hat's size relative to it's base diameter.")) {}
		if (DrawFloatEx		(this.stick_hatMoveScaleProp, 	0, 2, "Hat Move Scale", "Control how the hat center will follow controlling finger.")) {}
	
		DrawCheckBox(this.stick_disableXProp, "Block X", "Block stick's movement on the X axis.");			
		DrawCheckBox(this.stick_disableYProp, "Block Y", "Block stick's movement on the Y axis.");			

		if (DrawCheckBox(this.stick_overrideScaleProp, "Override scale", "Override global state-specific scale parameters."))
			this.OnStickColorChange();

		if (this.stick_overrideScaleProp.boolValue)
			{
			if (DrawSlider(this.stick_releasedHatScaleProp,		0, 2, "Hat Released Scale", "Stick Hat's scale when released.")) this.OnStickColorChange();
			if (DrawSlider(this.stick_pressedHatScaleProp,		0, 2, "Hat Pressed Scale", "Stick Hat's scale when pressed.")) this.OnStickColorChange();
			if (DrawSlider(this.stick_disabledHatScaleProp,		0, 2, "Hat Disabled Scale", "Stick Hat's scale when disabled.")) this.OnStickColorChange();
	
			if (DrawSlider(this.stick_releasedBaseScaleProp,	0, 2, "Base Released Scale", "Stick Base's scale when released.")) this.OnStickColorChange();
			if (DrawSlider(this.stick_pressedBaseScaleProp,		0, 2, "Base Pressed Scale", "Stick Base's scale when pressed.")) this.OnStickColorChange();
			if (DrawSlider(this.stick_disabledBaseScaleProp,	0, 2, "Base Disabled Scale", "Stick Base's scale when disabled.")) this.OnStickColorChange();
			}				


		if (DrawCheckBox(this.stick_overrideAnimDurationProp, "Custom anim duration", "Override global press/release animation duration with custom values."))
			this.OnStickColorChange();

		if (this.stick_overrideAnimDurationProp.boolValue)
			{
			DrawFloatEx(this.stick_pressAnimDurationProp, 0, 5, "Press duration", "Press animation duration in seconds.");
			DrawFloatEx(this.stick_releaseAnimDurationProp, 0, 5, "Release duration", "Release animation duration in seconds.");
			DrawFloatEx(this.stick_disableAnimDurationProp, 0, 5, "Disable duration", "Disable animation duration in seconds.");
			DrawFloatEx(this.stick_enableAnimDurationProp, 0, 5, "Enable duration", "Enable animation duration in seconds.");
			DrawFloatEx(this.stick_showAnimDurationProp, 0, 5, "Show duration", "Show animation duration in seconds.");
			DrawFloatEx(this.stick_hideAnimDurationProp, 0, 5, "Hide duration", "Hide animation duration in seconds.");
			}




		EditorGUILayout.Space();

		//EditorGUILayout.LabelField("Released State");			
		DrawGenericProp(this.stick_pressedHatImgProp,		"Hat Pressed Tex");
		DrawGenericProp(this.stick_releasedHatImgProp,		"Hat Released Tex");


		DrawGenericProp(		this.stick_pressedBaseImgProp,		"Base Pressed Tex");
		DrawGenericProp(		this.stick_releasedBaseImgProp,		"Base Released Tex");

		if (DrawCheckBox(this.stick_overrideColorsProp, "Override Colors", "When disabled, default colors will be used"))
			this.OnStickColorChange();

		if (this.stick_overrideColorsProp.boolValue)
			{
			if (this.DrawColorProp(	this.stick_pressedHatColorProp, 	"Hat Pressed")) this.OnStickColorChange();
			if (this.DrawColorProp(	this.stick_releasedHatColorProp,	"Hat Released")) this.OnStickColorChange();
			if (this.DrawColorProp(	this.stick_disabledHatColorProp, 	"Hat Disabled")) this.OnStickColorChange();

			if (this.DrawColorProp(	this.stick_pressedBaseColorProp, 	"Base Pressed")) this.OnStickColorChange();
			if (this.DrawColorProp(	this.stick_releasedBaseColorProp,	"Base Released")) this.OnStickColorChange();
			if (this.DrawColorProp(	this.stick_disabledBaseColorProp, 	"Base Disabled")) this.OnStickColorChange();
			}



		EditorGUILayout.Space();

		DrawCheckBox(this.stick_enableGetKeyProp, 		"Enable GetKey()", 	"Include this stick in controller's GetKey() state.");
		if (this.stick_enableGetKeyProp.boolValue)
			{
			DrawEnumField(this.stick_getKeyCodePressProp,	"Pressed Key",		"KeyCode to turn on when stick is pressed. Set to NONE to disable.");
			DrawEnumField(this.stick_getKeyCodePressAltProp,"Pressed Key (Alt)","KeyCode to turn on when stick is pressed. Set to NONE to disable.");
			DrawEnumField(this.stick_getKeyCodeUpProp,		"Up Key",			"KeyCode to turn on when stick is tilted up. Set to NONE to disable.");
			DrawEnumField(this.stick_getKeyCodeUpAltProp,	"Up Key (Alt)",		"KeyCode to turn on when stick is tilted up. Set to NONE to disable.");
			DrawEnumField(this.stick_getKeyCodeDownProp,	"Down Key",			"KeyCode to turn on when stick is tilted down. Set to NONE to disable.");
			DrawEnumField(this.stick_getKeyCodeDownAltProp,	"Down Key (Alt)",	"KeyCode to turn on when stick is tilted down. Set to NONE to disable.");
			DrawEnumField(this.stick_getKeyCodeLeftProp,	"Left Key",			"KeyCode to turn on when stick is tilted left. Set to NONE to disable.");
			DrawEnumField(this.stick_getKeyCodeLeftAltProp,	"Left Key (Alt)",	"KeyCode to turn on when stick is tilted left. Set to NONE to disable.");
			DrawEnumField(this.stick_getKeyCodeRightProp,	"Right Key", 		"KeyCode to turn on when stick is tilted right. Set to NONE to disable.");
			DrawEnumField(this.stick_getKeyCodeRightAltProp,"Right Key (Alt)", 	"KeyCode to turn on when stick is tilted right. Set to NONE to disable.");
			EditorGUILayout.Space();
			}

		DrawCheckBox(this.stick_enableGetButtonProp,	"Enable GetButton()", 	"Include this stick's 'pressed' state in controller's GetButton() state.");
		if (this.stick_enableGetButtonProp.boolValue)
			{
			DrawString(this.stick_getButtonNameProp,		"Button/Axis name");
			}

		DrawCheckBox(this.stick_enableGetAxisProp,		"Enable GetAxis()", "Include this stick in controller's .GetAxis() state.");
		if (this.stick_enableGetAxisProp.boolValue)
			{
			DrawString(this.stick_axisHorzNameProp, 		"Horz Axis", "Name of horizontal axis");
			DrawString(this.stick_axisVertNameProp, 		"Vert Axis", "Name of vertical axis");
			DrawCheckBox(this.stick_axisHorzFlipProp,		"Flip Horz", "Flip horizontal axis");
			DrawCheckBox(this.stick_axisVertFlipProp,		"Flip Vert", "Flip vertical axis");
			}
			

		if (showStickCodeSection = EditorGUILayout.Foldout(showStickCodeSection, 
			"Code Geneneration"))
			{
			DrawCheckBox(this.stick_codeCustomGUIProp, 		"Custom GUI", "Add custom GUI section for this stick.");	
			DrawCheckBox(this.stick_codeCustomLayoutProp, 	"Custom Layout", 	"Add custom layout section for this stick.");	

			}

		EditorGUILayout.EndVertical();			
		}		


	}
	



// ---------------------
// Touch Zone Editor -------
// ---------------------

private SerializedProperty
	zone_initiallyDisabledProp,
	zone_initiallyHiddenProp,
	//zone_enabledProp,
	//zone_visibleProp,
	zone_prioProp,
	zone_nameProp,
	zone_layoutBoxIdProp,
	zone_shapeProp,
	zone_posCmProp,
	zone_sizeCmProp,		// float, not vec2!
	//zone_pressedScaleProp,

	zone_hitDistScaleProp,
	zone_acceptSharedTouchesProp,
	zone_guiDepthProp,
	zone_disableGuiProp,
	zone_overrideAnimDurationProp,
	zone_pressAnimDurationProp,
	zone_releaseAnimDurationProp,

	zone_disableAnimDurationProp,
	zone_enableAnimDurationProp,
	zone_overrideColorsProp,

	zone_enableSecondFingerProp,
	zone_nonExclusiveTouchesProp,
	zone_strictTwoFingerStartProp,
	//zone_strictTwistStartProp,
	zone_freezeTwistWhenTooCloseProp,
	//zone_strictPinchStartProp,
	//zone_startPinchWhenTwistingProp,
	zone_noPinchAfterDragProp,
	zone_noPinchAfterTwistProp,
	zone_noTwistAfterDragProp,
	zone_noTwistAfterPinchProp,
	zone_noDragAfterPinchProp,
	zone_noDragAfterTwistProp,
	zone_startPinchWhenTwistingProp,
	zone_startPinchWhenDraggingProp,
	zone_startDragWhenPinchingProp,
	zone_startDragWhenTwistingProp,
	zone_startTwistWhenDraggingProp,
	zone_startTwistWhenPinchingProp,
	zone_gestureDetectionOrderProp,

	zone_regionRectProp,

	zone_overrideScaleProp,
	zone_releasedScaleProp,
	zone_pressedScaleProp,
	zone_disabledScaleProp,

	zone_releasedImgProp,
	zone_pressedImgProp,
	zone_releasedColorProp,
	zone_pressedColorProp,
	zone_disabledColorProp,

	zone_enableGetKeyProp,
	zone_getKeyCodeProp,
	zone_getKeyCodeAltProp,

	zone_getKeyCodeMultiProp,
	zone_getKeyCodeMultiAltProp,
	zone_getButtonMultiNameProp,

	zone_enableGetButtonProp,
	zone_getButtonNameProp,
	zone_enableGetAxisProp,
	zone_axisHorzNameProp,
	zone_axisVertNameProp,
	zone_axisHorzFlipProp,
	zone_axisVertFlipProp,
	zone_axisValScaleProp,

	zone_emulateMouseProp,
	zone_mousePosFromFirstFingerProp,

	zone_codeUniJustPressedProp,
	zone_codeUniPressedProp,
	zone_codeUniJustReleasedProp,
	zone_codeUniJustDraggedProp,
	zone_codeUniDraggedProp,
	zone_codeUniReleasedDragProp,
	zone_codeMultiJustPressedProp,
	zone_codeMultiPressedProp,
	zone_codeMultiJustReleasedProp,
	zone_codeMultiJustDraggedProp,
	zone_codeMultiDraggedProp,
	zone_codeMultiReleasedDragProp,
	zone_codeJustTwistedProp,
	zone_codeTwistedProp,
	zone_codeReleasedTwistProp,
	zone_codeJustPinchedProp,
	zone_codePinchedProp,
	zone_codeReleasedPinchProp,
	zone_codeSimpleTapProp,
	zone_codeSingleTapProp,
	zone_codeDoubleTapProp,
	zone_codeSimpleMultiTapProp,
	zone_codeMultiSingleTapProp,
	zone_codeMultiDoubleTapProp,
	zone_codeCustomGUIProp,
	zone_codeCustomLayoutProp;

	
private int zone_curId;

// --------------------
private void SelectZone(int i)
	{
	//UnfocusControls();

	this.touchZonesProp = this.serializedObject.FindProperty("touchZones");

	if ((this.touchZonesProp == null) || (this.touchZonesProp.arraySize == 0))
		{	

		this.zone_initiallyDisabledProp		= null;
		this.zone_initiallyHiddenProp		= null;
		//this.zone_enabledProp				= null;
		//this.zone_visibleProp				= null;
		this.zone_prioProp					= null;
		this.zone_nameProp					= null;
		this.zone_layoutBoxIdProp			= null;
		this.zone_shapeProp					= null;
		this.zone_posCmProp					= null;
		this.zone_sizeCmProp				= null;
		this.zone_pressedScaleProp			= null;
		this.zone_regionRectProp			= null;
		this.zone_enableSecondFingerProp	= null;
		this.zone_nonExclusiveTouchesProp	= null;
		this.zone_releasedImgProp			= null;
		this.zone_pressedImgProp			= null;
		this.zone_releasedColorProp			= null;
		this.zone_pressedColorProp			= null;
		this.zone_disabledColorProp			= null;

		this.zone_disableAnimDurationProp	= null;
		this.zone_enableAnimDurationProp	= null;
		this.zone_overrideColorsProp		= null;

		this.zone_strictTwoFingerStartProp	= null;
	
		this.zone_hitDistScaleProp			= null;
		this.zone_acceptSharedTouchesProp	= null;
		this.zone_guiDepthProp				= null;
		this.zone_overrideAnimDurationProp	= null;
		this.zone_pressAnimDurationProp	= null;
		this.zone_releaseAnimDurationProp	= null;

		return;
		}


	if (i < 0) 
		i = 0;		
	else if (i >= this.touchZonesProp.arraySize) 
		i = (this.touchZonesProp.arraySize - 1);


	this.zone_curId = i;

	SerializedProperty b = this.touchZonesProp.GetArrayElementAtIndex(i);

	this.zone_initiallyDisabledProp	= b.FindPropertyRelative("initiallyDisabled");
	this.zone_initiallyHiddenProp	= b.FindPropertyRelative("initiallyHidden");
	//this.zone_enabledProp			= b.FindPropertyRelative("enabled");
	//this.zone_visibleProp			= b.FindPropertyRelative("visible");
	this.zone_prioProp				= b.FindPropertyRelative("prio");
	this.zone_nameProp				= b.FindPropertyRelative("name");
	this.zone_layoutBoxIdProp		= b.FindPropertyRelative("layoutBoxId");
	this.zone_shapeProp				= b.FindPropertyRelative("shape");
	this.zone_posCmProp				= b.FindPropertyRelative("posCm");
	this.zone_sizeCmProp			= b.FindPropertyRelative("sizeCm");
	this.zone_pressedScaleProp		= b.FindPropertyRelative("pressedScale");

	this.zone_hitDistScaleProp		= b.FindPropertyRelative("hitDistScale");
	this.zone_acceptSharedTouchesProp	= b.FindPropertyRelative("acceptSharedTouches");
	this.zone_guiDepthProp			= b.FindPropertyRelative("guiDepth");
	this.zone_disableGuiProp		= b.FindPropertyRelative("disableGui");
	this.zone_overrideAnimDurationProp= b.FindPropertyRelative("overrideAnimDuration");
	this.zone_pressAnimDurationProp	= b.FindPropertyRelative("pressAnimDuration");
	this.zone_releaseAnimDurationProp	= b.FindPropertyRelative("releaseAnimDuration");

	this.zone_disableAnimDurationProp	= b.FindPropertyRelative("disableAnimDuration");
	this.zone_enableAnimDurationProp	= b.FindPropertyRelative("enableAnimDuration");
	this.zone_overrideColorsProp		= b.FindPropertyRelative("overrideColors");

	this.zone_strictTwoFingerStartProp	= b.FindPropertyRelative("strictTwoFingerStart");
	//this.zone_strictTwistStartProp = b.FindPropertyRelative("strictTwistStart");
	this.zone_freezeTwistWhenTooCloseProp= b.FindPropertyRelative("freezeTwistWhenTooClose");
	//this.zone_strictPinchStartProp		= b.FindPropertyRelative("strictPinchStart");
	//this.zone_startPinchWhenTwistingProp= b.FindPropertyRelative("startPinchWhenTwisting");
		
	this.zone_noPinchAfterDragProp		= b.FindPropertyRelative("noPinchAfterDrag");
	this.zone_noPinchAfterTwistProp		= b.FindPropertyRelative("noPinchAfterTwist");
	this.zone_noTwistAfterDragProp		= b.FindPropertyRelative("noTwistAfterDrag");
	this.zone_noTwistAfterPinchProp		= b.FindPropertyRelative("noTwistAfterPinch");
	this.zone_noDragAfterPinchProp		= b.FindPropertyRelative("noDragAfterPinch");
	this.zone_noDragAfterTwistProp		= b.FindPropertyRelative("noDragAfterTwist");
	this.zone_startPinchWhenTwistingProp= b.FindPropertyRelative("startPinchWhenTwisting");
	this.zone_startPinchWhenDraggingProp= b.FindPropertyRelative("startPinchWhenDragging");
	this.zone_startDragWhenPinchingProp	= b.FindPropertyRelative("startDragWhenPinching");
	this.zone_startDragWhenTwistingProp	= b.FindPropertyRelative("startDragWhenTwisting");
	this.zone_startTwistWhenDraggingProp= b.FindPropertyRelative("startTwistWhenDragging");
	this.zone_startTwistWhenPinchingProp= b.FindPropertyRelative("startTwistWhenPinching");
	this.zone_gestureDetectionOrderProp	= b.FindPropertyRelative("gestureDetectionOrder");

	this.zone_regionRectProp			= b.FindPropertyRelative("regionRect");
	this.zone_enableSecondFingerProp	= b.FindPropertyRelative("enableSecondFinger");
	this.zone_nonExclusiveTouchesProp	= b.FindPropertyRelative("nonExclusiveTouches");

	this.zone_releasedImgProp			= b.FindPropertyRelative("releasedImg");
	this.zone_pressedImgProp			= b.FindPropertyRelative("pressedImg");
	this.zone_releasedColorProp			= b.FindPropertyRelative("releasedColor");
	this.zone_pressedColorProp			= b.FindPropertyRelative("pressedColor");
	this.zone_disabledColorProp			= b.FindPropertyRelative("disabledColor");

	this.zone_overrideScaleProp			= b.FindPropertyRelative("overrideScale");
	this.zone_releasedScaleProp			= b.FindPropertyRelative("releasedScale");
	this.zone_pressedScaleProp			= b.FindPropertyRelative("pressedScale");
	this.zone_disabledScaleProp			= b.FindPropertyRelative("disabledScale");


	this.zone_enableGetKeyProp			= b.FindPropertyRelative("enableGetKey");
	this.zone_getKeyCodeProp			= b.FindPropertyRelative("getKeyCode");
	this.zone_getKeyCodeAltProp			= b.FindPropertyRelative("getKeyCodeAlt");
	this.zone_getKeyCodeMultiProp		= b.FindPropertyRelative("getKeyCodeMulti");
	this.zone_getKeyCodeMultiAltProp	= b.FindPropertyRelative("getKeyCodeMultiAlt");
	this.zone_getButtonMultiNameProp	= b.FindPropertyRelative("getButtonMultiName");
	
	this.zone_enableGetButtonProp		= b.FindPropertyRelative("enableGetButton");
	this.zone_getButtonNameProp			= b.FindPropertyRelative("getButtonName");
	this.zone_enableGetAxisProp			= b.FindPropertyRelative("enableGetAxis");
	this.zone_axisHorzNameProp			= b.FindPropertyRelative("axisHorzName");
	this.zone_axisVertNameProp			= b.FindPropertyRelative("axisVertName");
	this.zone_axisHorzFlipProp			= b.FindPropertyRelative("axisHorzFlip");
	this.zone_axisVertFlipProp			= b.FindPropertyRelative("axisVertFlip");
	this.zone_axisValScaleProp			= b.FindPropertyRelative("axisValScale");

	this.zone_emulateMouseProp			= b.FindPropertyRelative("emulateMouse");
	this.zone_mousePosFromFirstFingerProp= b.FindPropertyRelative("mousePosFromFirstFinger");
	
	this.zone_codeUniJustPressedProp	= b.FindPropertyRelative("codeUniJustPressed");
	this.zone_codeUniPressedProp		= b.FindPropertyRelative("codeUniPressed");
	this.zone_codeUniJustReleasedProp	= b.FindPropertyRelative("codeUniJustReleased");
	this.zone_codeUniJustDraggedProp	= b.FindPropertyRelative("codeUniJustDragged");
	this.zone_codeUniDraggedProp		= b.FindPropertyRelative("codeUniDragged");
	this.zone_codeUniReleasedDragProp	= b.FindPropertyRelative("codeUniReleasedDrag");
	this.zone_codeMultiJustPressedProp	= b.FindPropertyRelative("codeMultiJustPressed");
	this.zone_codeMultiPressedProp		= b.FindPropertyRelative("codeMultiPressed");
	this.zone_codeMultiJustReleasedProp	= b.FindPropertyRelative("codeMultiJustReleased");
	this.zone_codeMultiJustDraggedProp	= b.FindPropertyRelative("codeMultiJustDragged");
	this.zone_codeMultiDraggedProp		= b.FindPropertyRelative("codeMultiDragged");
	this.zone_codeMultiReleasedDragProp	= b.FindPropertyRelative("codeMultiReleasedDrag");
	this.zone_codeJustTwistedProp		= b.FindPropertyRelative("codeJustTwisted");
	this.zone_codeTwistedProp			= b.FindPropertyRelative("codeTwisted");
	this.zone_codeReleasedTwistProp		= b.FindPropertyRelative("codeReleasedTwist");
	this.zone_codeJustPinchedProp		= b.FindPropertyRelative("codeJustPinched");
	this.zone_codePinchedProp			= b.FindPropertyRelative("codePinched");
	this.zone_codeReleasedPinchProp		= b.FindPropertyRelative("codeReleasedPinch");
	this.zone_codeSimpleTapProp			= b.FindPropertyRelative("codeSimpleTap");
	this.zone_codeSingleTapProp			= b.FindPropertyRelative("codeSingleTap");
	this.zone_codeDoubleTapProp			= b.FindPropertyRelative("codeDoubleTap");
	this.zone_codeSimpleMultiTapProp	= b.FindPropertyRelative("codeSimpleMultiTap");
	this.zone_codeMultiSingleTapProp	= b.FindPropertyRelative("codeMultiSingleTap");
	this.zone_codeMultiDoubleTapProp	= b.FindPropertyRelative("codeMultiDoubleTap");
	this.zone_codeCustomGUIProp			= b.FindPropertyRelative("codeCustomGUI");
	this.zone_codeCustomLayoutProp		= b.FindPropertyRelative("codeCustomLayout");
	


	}




// ---------------------
private void ResetCurrentZone()
	{	
	if (this.zone_prioProp == null)
		return;
	
	UnfocusControls();

	TouchController joy = this.target as TouchController;
	if (joy == null)
		return;
/*
public const Color	
	DEFAULT_PRESSED_COLOR	= new Color(1.0f,	1.0f,	1.0f, 1.0f),
	DEFAULT_RELEASED_COLOR	= new Color(0.8f,	0.8f,	0.8f, 0.5f),
	DEFAULT_DISABLED_COLOR	= new Color(0.5f, 	0.5f, 	0.5f, 0.2f),
	
	DEFAULT_STICK_HAT_PRESSED_COLOR		= new Color(1.0f,	1.0f,	1.0f, 1.0f),
	DEFAULT_STICK_HAT_RELEASED_COLOR	= new Color(0.8f,	0.8f,	0.8f, 0.5f),
	DEFAULT_STICK_BASE_PRESSED_COLOR	= new Color(1.0f,	1.0f,	1.0f, 1.0f),
	DEFAULT_STICK_BASE_RELEASED_COLOR	= new Color(0.8f,	0.8f,	0.8f, 0.5f);


public const float
	DEFAULT_PRESSED_SCALE				= 1.2f,
	DEFAULT_STICK_BASE_PRESSED_SCALE	= 0.8f,

	DEFAULT_PRESS_DURATION				= 0.1,
	DEFAULT_RELEASE_DURATION			= 0.3f,
	DEFAULT_STICK_FADEOUT				= 2.0f;
*/

	this.zone_initiallyDisabledProp.boolValue	= false;	
	this.zone_initiallyHiddenProp.boolValue		= false;
	//this.zone_enabledProp.boolValue		= true;	
	//this.zone_visibleProp.boolValue		= true;
	this.zone_prioProp.intValue			= TouchController.DEFAULT_ZONE_PRIO;
	this.zone_nameProp.stringValue			= "ZONE";
	//this.zone_layoutBoxIdProp.intValue	= (int)TouchController.LayoutBoxId.NONE;
	this.zone_layoutBoxIdProp.intValue	= 0;
	this.zone_shapeProp.intValue	= (int)TouchController.ControlShape.CIRCLE;
	this.zone_posCmProp.vector2Value		= new Vector2(0, 0);
	this.zone_sizeCmProp.vector2Value		= new Vector2(1,1);
	this.zone_pressedScaleProp.floatValue	= DEFAULT_PRESSED_SCALE;
		
	this.zone_hitDistScaleProp.floatValue			= 1.0f;
	this.zone_acceptSharedTouchesProp.boolValue 	= true;
	this.zone_guiDepthProp.intValue					= 0;
	this.zone_disableGuiProp.boolValue				= false;
	this.zone_overrideAnimDurationProp.boolValue	= false;
	this.zone_pressAnimDurationProp.floatValue 		= DEFAULT_PRESS_DURATION;
	this.zone_releaseAnimDurationProp.floatValue 	= DEFAULT_RELEASE_DURATION;
	this.zone_disableAnimDurationProp.floatValue	= DEFAULT_DISABLE_DURATION;
	this.zone_enableAnimDurationProp.floatValue		= DEFAULT_ENABLE_DURATION;

	this.zone_regionRectProp.rectValue		= new Rect(0, 0, 1, 1);
	this.zone_enableSecondFingerProp.boolValue	= true;
	this.zone_nonExclusiveTouchesProp.boolValue	= false;
		
	this.zone_overrideColorsProp.boolValue		= false;
	this.zone_strictTwoFingerStartProp.boolValue= true;
	//this.zone_strictTwistStartProp.boolValue	= true;
	this.zone_freezeTwistWhenTooCloseProp.boolValue	= true;


	//this.zone_imgpReleasedProp			= b.FindPropertyRelative("imgpReleased");
	//this.zone_imgpPressedProp				= b.FindPropertyRelative("imgpPressed");
	this.zone_releasedColorProp.colorValue 	= DEFAULT_RELEASED_COLOR;
	this.zone_pressedColorProp.colorValue 	= DEFAULT_PRESSED_COLOR;
	this.zone_disabledColorProp.colorValue 	= DEFAULT_DISABLED_COLOR;

	this.zone_overrideScaleProp.boolValue	= false;
	this.zone_releasedScaleProp.floatValue	= 1.0f;
	this.zone_pressedScaleProp.floatValue	= 1.2f;
	this.zone_disabledScaleProp.floatValue	= 1.0f;

		
	this.zone_enableGetKeyProp.boolValue	= false;
	this.zone_getKeyCodeProp.intValue	= (int)KeyCode.None;
	this.zone_getKeyCodeAltProp.intValue	= (int)KeyCode.None;
	this.zone_getKeyCodeMultiProp.intValue = (int)KeyCode.None;
	this.zone_getKeyCodeMultiAltProp.intValue = (int)KeyCode.None;

	this.zone_enableGetButtonProp.boolValue	= false;
	this.zone_getButtonNameProp.stringValue	= "Fire1";
	this.zone_getButtonMultiNameProp.stringValue	= "Fire1Multi";
	this.zone_enableGetAxisProp.boolValue	= false;
	this.zone_axisHorzNameProp.stringValue	= "Mouse X";
	this.zone_axisVertNameProp.stringValue	= "Mouse Y";
	this.zone_axisHorzFlipProp.boolValue	= false;
	this.zone_axisVertFlipProp.boolValue	= false;
	this.zone_axisValScaleProp.floatValue	= 0.1f;

	this.zone_emulateMouseProp.boolValue	= false;
	this.zone_mousePosFromFirstFingerProp.boolValue = false;
		
	this.zone_codeUniJustPressedProp.boolValue		= false;
	this.zone_codeUniPressedProp.boolValue			= true;
	this.zone_codeUniJustReleasedProp.boolValue		= true;
	this.zone_codeUniJustDraggedProp.boolValue		= false;
	this.zone_codeUniDraggedProp.boolValue			= false;
	this.zone_codeUniReleasedDragProp.boolValue		= false;
	this.zone_codeMultiJustPressedProp.boolValue	= false;
	this.zone_codeMultiPressedProp.boolValue		= true;
	this.zone_codeMultiJustReleasedProp.boolValue	= true;
	this.zone_codeMultiJustDraggedProp.boolValue	= false;
	this.zone_codeMultiDraggedProp.boolValue		= false;
	this.zone_codeMultiReleasedDragProp.boolValue	= false;
	this.zone_codeJustTwistedProp.boolValue		= false;
	this.zone_codeTwistedProp.boolValue			= false;
	this.zone_codeReleasedTwistProp.boolValue	= false;
	this.zone_codeJustPinchedProp.boolValue		= false;
	this.zone_codePinchedProp.boolValue			= false;
	this.zone_codeReleasedPinchProp.boolValue	= false;
	this.zone_codeSimpleTapProp.boolValue		= false;
	this.zone_codeSingleTapProp.boolValue		= false;
	this.zone_codeDoubleTapProp.boolValue		= false;
	this.zone_codeSimpleMultiTapProp.boolValue	= false;
	this.zone_codeMultiSingleTapProp.boolValue	= false;
	this.zone_codeMultiDoubleTapProp.boolValue	= false;
	this.zone_codeCustomGUIProp.boolValue		= false;
	this.zone_codeCustomLayoutProp.boolValue	= false;



	// Assign default images...

	this.zone_pressedImgProp.objectReferenceValue =
	this.zone_releasedImgProp.objectReferenceValue = joy.defaultZoneImg;


	// Copy button images from the first one that have any...

	if (joy.touchZones != null)
		{
		foreach (TouchZone s in joy.touchZones)
			{
			if ((this.zone_releasedImgProp.objectReferenceValue == null) && (s.releasedImg != null))
				this.zone_releasedImgProp.objectReferenceValue = s.releasedImg;
			if ((this.zone_pressedImgProp.objectReferenceValue == null) && (s.pressedImg != null))
				this.zone_pressedImgProp.objectReferenceValue = s.pressedImg;
				
			// Reuse images...

			if ((this.zone_pressedImgProp.objectReferenceValue == null) &&
				(this.zone_releasedImgProp.objectReferenceValue != null))
				this.zone_pressedImgProp.objectReferenceValue = this.zone_releasedImgProp.objectReferenceValue;

			}
		}

	}
	

// -------------------
private void MoveZoneSelection(int delta)
	{
	UnfocusControls();

	if ((this.touchZonesProp == null) || (this.touchZonesProp.arraySize == 0))
		{
		this.SelectZone(0);
		return;		
		}

	this.zone_curId += delta;
	if (this.zone_curId < 0) 
		this.zone_curId = this.touchZonesProp.arraySize - 1;
	else if (this.zone_curId >= this.touchZonesProp.arraySize)
		this.zone_curId = 0;

	this.SelectZone(this.zone_curId);	
	}
	

// -------------------
private void OnZoneColorChange()
	{	
	TouchController tc = this.target as TouchController;	
	TouchZone tz;
	if (tc == null) 
		return;		
	if ((tc.touchZones == null) || 
		(this.zone_curId < 0) || 
		(this.zone_curId >= tc.touchZones.Length) || 
		((tz = tc.touchZones[this.zone_curId]) == null))
		return;
		
	tz.SetColorsDirtyFlag(); //.OnColorChange();
	}


// ---------------------
private void DrawZoneEditorSection()
	{	
	//EditorGUIUtility.LookLikeInspector();

	TouchController joy = this.target as TouchController;
	SerializedProperty arrayProp = this.touchZonesProp;

	EditorGUILayout.BeginHorizontal(this.sectionToolbarStyle, //EditorStyles.toolbar, 
		GUILayout.ExpandWidth(true), GUILayout.Height(30));
	
	if (GUILayout.Button(new GUIContent("<", "Select previous"), EditorStyles.miniButtonLeft, GUILayout.Width(20)))
		this.MoveZoneSelection(-1);
	if (GUILayout.Button(new GUIContent(">", "Select next"), EditorStyles.miniButtonRight, GUILayout.Width(20)))
		this.MoveZoneSelection(1);

	EditorGUILayout.LabelField((arrayProp.arraySize == 0) ? "-" : 
		("" + (this.zone_curId + 1)) + "/" + arrayProp.arraySize + 
		" : " + ((this.zone_nameProp == null) ? 
		"-" : this.zone_nameProp.stringValue), EditorStyles.miniLabel, 
		GUILayout.Width(90), GUILayout.ExpandWidth(true));

	//EditorGUILayout.EndHorizontal();
	//EditorGUILayout.BeginHorizontal();
		
	if (GUILayout.Button(new GUIContent("<<", "Move left"), EditorStyles.miniButtonLeft, GUILayout.Width(26)))
		{
		UnfocusControls();

		if (this.zone_curId > 0)
			{
			joy.SetContentDirtyFlag();
			arrayProp.MoveArrayElement(this.zone_curId, this.zone_curId - 1);
			this.SelectZone(this.zone_curId - 1);
			//return;
			}
		}
	if (GUILayout.Button(new GUIContent(">>", "Move right"), EditorStyles.miniButtonRight, GUILayout.Width(26)))
		{
		UnfocusControls();
			
		if (this.zone_curId < (arrayProp.arraySize - 1))
			{
			joy.SetContentDirtyFlag();
			arrayProp.MoveArrayElement(this.zone_curId, this.zone_curId + 1);
			this.SelectZone(this.zone_curId + 1);
			//return;
			}
		}


	if (GUILayout.Button(new GUIContent("+", "Add new zone"), EditorStyles.miniButtonLeft, GUILayout.Width(20)))
		{	
		UnfocusControls();

		joy.SetContentDirtyFlag();

		if (arrayProp.arraySize == 0)
			{
			arrayProp.arraySize = 1;
			this.SelectZone(0);
			}
		else 
			{
			arrayProp.InsertArrayElementAtIndex((this.zone_curId)); // + 1));
			this.SelectZone(this.zone_curId + 1);
			}


//		arrayProp.InsertArrayElementAtIndex(
//			(arrayProp.arraySize == 0) ? 0 : (this.zone_curId + 1));
//		this.SelectZone(this.zone_curId + 1);

		this.ResetCurrentZone();
		}

	if (GUILayout.Button(new GUIContent("-", "Delete zone"), EditorStyles.miniButtonRight, GUILayout.Width(20)))
		{
		UnfocusControls();

		joy.SetContentDirtyFlag();
		if (this.zone_curId < arrayProp.arraySize)
			arrayProp.DeleteArrayElementAtIndex(this.zone_curId);
		this.SelectZone(this.zone_curId);	
		}
	

	EditorGUILayout.EndHorizontal();
		

	// Draw current button's params...

	if ((this.zone_prioProp != null) && (joy.touchZones != null) && 
		(this.zone_curId < joy.touchZones.Length) && (this.zone_curId >= 0))
		{
		EditorGUILayout.BeginVertical(this.editSectionStyle, /*EditorStyles.popup,*/ 
			GUILayout.ExpandWidth(true));


		//TouchZone	tz = joy.touchZones[this.zone_curId];
		
			DrawGenericProp	(this.zone_nameProp, 				"Name");
		if (DrawCheckBox	(this.zone_initiallyDisabledProp,	"Initially Disabled", "When enabled, this control will be automatically disabled after initialization."))   { this.OnZoneColorChange(); }
		if (DrawCheckBox	(this.zone_initiallyHiddenProp,		"Initially Hidden",		"When enabled, this control will be automatically hidden after initialization."))   { this.OnZoneColorChange(); }
		//if (DrawCheckBox	(this.zone_enabledProp, 			"Enabled"))  {}
		//if (DrawCheckBox	(this.zone_visibleProp, 			"Visible"))  {}
		if (DrawInt			(this.zone_prioProp,				"Priority")) {} 
		if (DrawSlider		(this.zone_hitDistScaleProp,		0.5f, 1.5f, "Hit Dist Scale", HIT_DIST_SCALE_TOOLTIP_STR)) {}
		if (DrawCheckBox	(this.zone_acceptSharedTouchesProp, "Shared Touches", "Accept shared touches")) { }
			DrawCheckBox(this.zone_nonExclusiveTouchesProp, 	"Non-Exclusive Touches","Enable touch pass-through. Use with caution!");
		if (DrawIntEx		(this.zone_guiDepthProp, -100,100,	"GUI Depth", "GUI Depth offset added to controller base depth")) {}
		if (DrawCheckBox	(this.zone_disableGuiProp, 			"Disable GUI", "Disable default GUI rendering of this control")) {}


		if (DrawEnumPopup	(this.zone_shapeProp,				"Shape")) { joy.SetLayoutDirtyFlag(); }

		if (this.zone_shapeProp.intValue != (int)TouchController.ControlShape.SCREEN_REGION)
			{
			//if (DrawEnumPopup	(this.zone_layoutBoxIdProp, 		"Layout Box")) { joy.SetLayoutDirtyFlag(); }
			if (DrawLayoutBoxIdProp(this.zone_layoutBoxIdProp, 		"Layout Box")) { joy.SetLayoutDirtyFlag(); }

			if (DrawCmVec2Field	(this.zone_posCmProp, -100, 100,	"Position")) { joy.SetLayoutDirtyFlag(); }

			if (this.zone_shapeProp.intValue == (int)TouchController.ControlShape.CIRCLE)
				{
				if (DrawCmVec2XField(this.zone_sizeCmProp, 0, 10, "Diameter")) { joy.SetLayoutDirtyFlag(); }
				}
			else
				{	
				if (DrawCmVec2Field	(this.zone_sizeCmProp, 0, 10, "Size")) { joy.SetLayoutDirtyFlag(); }
				}
			}
		else
			{
			if (DrawRectField	(this.zone_regionRectProp, 0,0,1,1, "Region Rect")) { joy.SetLayoutDirtyFlag(); }
			}

			//EditorGUILayout.Vector2Field("Actual size (px)", tz.sizePx);		
		//DrawPixValInRealWorldUnitsLabel("Actual size", 2 * tz.radPx, 2);

		//if (DrawFloatEx		(this.zone_pressedScaleProp, 0, 4, "Pressed Scale")) {}


	

	DrawCheckBox(this.zone_enableSecondFingerProp, 	"Multi-touch", 			"Enable two finger gesture support.");
	
	if (this.zone_enableSecondFingerProp.boolValue)
		{
		DrawCheckBox(this.zone_strictTwoFingerStartProp, "Strict Multi-Touch Start", "Enable strict multi-finger start mode - when enabled, multi-touch can be started only when the time between two touches is less than time value specified in General Settings!");
	
		//DrawCheckBox(this.zone_strictTwistStartProp, "Strict Twist Start", "Enable strict twist start detection - no twist allowed after pinching.");
		DrawCheckBox(this.zone_freezeTwistWhenTooCloseProp, "Freeze Twist", "When enabled, twist angle will not be updated, when finger distance drops below safe distance.");
				
		//DrawCheckBox(this.zone_strictPinchStartProp, "Strict Pinch Start", "Pinching will be ignored after twist started.");
		//DrawCheckBox(this.zone_startPinchWhenTwistingProp, "Pinch When Twisting", "Starting twist will also start pinch to eliminate possible jump due to late pinch start.");
	 
		DrawCheckBox(this.zone_noPinchAfterDragProp,	"No Pinch After Drag", "Pinching will be ignored after multi-finger drag.");
		DrawCheckBox(this.zone_noPinchAfterTwistProp,	"No Pinch After Twist", "Pinching will be ignored after twisting.");
		DrawCheckBox(this.zone_noTwistAfterDragProp,	"No Twist After Drag", "Twisting will be ignored after multi-finger dragging.");
		DrawCheckBox(this.zone_noTwistAfterPinchProp,	"No Twist After Pinch", "Twisting will be ignored after pinching.");
		DrawCheckBox(this.zone_noDragAfterPinchProp, 	"No Drag After Pinch", "Multi-finger dragging will be ignored after pinching.");
		DrawCheckBox(this.zone_noDragAfterTwistProp, 	"No Drag After Twist", "Multi-finger dragging will be ignored after twisting.");
		DrawCheckBox(this.zone_startPinchWhenDraggingProp,	"Pinch When Dragging", "Pinch will be marked as moving, when multi-finger drag starts. Use this to eliminate possible pinch jump later on.");
		DrawCheckBox(this.zone_startTwistWhenDraggingProp, 	"Twist When Dragging", "Twist will be marked as moving, when multi-finger drag starts. Use this to eliminate possible twist jump later on.");
		DrawCheckBox(this.zone_startDragWhenPinchingProp, 	"Drag When Pinching", "Multi-finger drag will be marked as moving, when pinch starts. Use this to eliminate possible drag jump later on.");
		DrawCheckBox(this.zone_startTwistWhenPinchingProp, 	"Twist When Pinching", "Twist will be marked as moving, when pinch starts. Use this to eliminate possible twist jump later on.");
		DrawCheckBox(this.zone_startDragWhenTwistingProp, 	"Drag When Twisting", "Multi-finger drag will be marked as moving, when twist starts. Use this to eliminate possible drag jump later on.");
		DrawCheckBox(this.zone_startPinchWhenTwistingProp,	"Pinch When Twisting", "Pinch will be marked as moving, when twist starts. Use this to eliminate possible pinch jump later on.");

		DrawGenericProp(this.zone_gestureDetectionOrderProp, "Gesture Detect. Order", "Two-finger gesture detection order. Use this in conjunction with no[X]after[Y] and start[X]When[Y] variables if you want to prioritize certain gesture(s).");
		}



			


		EditorGUILayout.Space();

		DrawCheckBox(this.zone_overrideAnimDurationProp, "Custom anim duration", "Override global press/release animation duration with custom values.");
		if (this.zone_overrideAnimDurationProp.boolValue)
			{
			DrawFloatEx(this.zone_pressAnimDurationProp, 0, 5, "Press duration", "Press animation duration in seconds.");
			DrawFloatEx(this.zone_releaseAnimDurationProp, 0, 5, "Release duration", "Release animation duration in seconds.");
			DrawFloatEx(this.zone_disableAnimDurationProp, 0, 5, "Disable duration", "Disable animation duration in seconds.");
			DrawFloatEx(this.zone_enableAnimDurationProp, 0, 5, "Enable duration", "Enable animation duration in seconds.");
			}


		//EditorGUILayout.LabelField("Released State");			
		DrawGenericProp(		this.zone_pressedImgProp,	"Pressed Tex");
		DrawGenericProp(		this.zone_releasedImgProp,	"Released Tex");


		if (DrawCheckBox(this.zone_overrideColorsProp, "Override Colors", "When disabled, default colors will be used"))
			this.OnZoneColorChange();

		if (this.zone_overrideColorsProp.boolValue)
			{
			if (this.DrawColorProp(	this.zone_pressedColorProp,	"Pressed")) 		this.OnZoneColorChange();
			if (this.DrawColorProp(	this.zone_releasedColorProp,"Released")) 		this.OnZoneColorChange();
			if (this.DrawColorProp(	this.zone_disabledColorProp, "Disabled Color"))	this.OnZoneColorChange();
			}

			

		if (DrawCheckBox(this.zone_overrideScaleProp, "Override Scale", "Override global state-specific scale parameters."))
			this.OnZoneColorChange();

		if (this.zone_overrideScaleProp.boolValue)
			{
			if (DrawSlider(this.zone_releasedScaleProp,		0, 2, "Released Scale", "Zone's scale when released.")) this.OnZoneColorChange();
			if (DrawSlider(this.zone_pressedScaleProp,		0, 2, "Pressed Scale", "Zone's scale when pressed.")) this.OnZoneColorChange();
			if (DrawSlider(this.zone_disabledScaleProp,		0, 2, "Disabled Scale", "Zone's scale when disabled.")) this.OnZoneColorChange();
			}				


		// Unity Input section...

		EditorGUILayout.Space();

		DrawCheckBox(this.zone_enableGetKeyProp, "Enable GetKey()", "Include this zone in controller's GetKey() state.");
		if (this.zone_enableGetKeyProp.boolValue)
			{
			DrawEnumField(this.zone_getKeyCodeProp, 	"Key (A)", "Primary KeyCode to turn on when this zone is pressed.");
			DrawEnumField(this.zone_getKeyCodeAltProp,  "Key (B)", "Secondary KeyCode to turn on when this zone is pressed.");
			DrawEnumField(this.zone_getKeyCodeMultiProp,"Multi Key (A)", "Primary KeyCode to turn on when this zone is pressed by multiple fingers.");
			DrawEnumField(this.zone_getKeyCodeMultiAltProp,"Multi Key (B)", "Secondary KeyCode to turn on when this zone is pressed by multiple fingers.");
			EditorGUILayout.Space();
			}

		DrawCheckBox(this.zone_enableGetButtonProp, "Enable GetButton()", "Include this zone in controller's GetButton() state.");
		if (this.zone_enableGetButtonProp.boolValue)
			{
			DrawString(this.zone_getButtonNameProp, "Button name", "Name of named button/axis to turn on when this zone is pressed.");
			DrawString(this.zone_getButtonMultiNameProp, "Multi name", "Name of named button/axis to turn on when this zone is pressed by multiple fingers.");
			EditorGUILayout.Space();
			}


		DrawCheckBox(this.zone_enableGetAxisProp, "Enable GetAxis()", "Include this zone's unified drag vector in controller's GetAxis() state.");
		if (this.zone_enableGetAxisProp.boolValue)
			{
			DrawString(this.zone_axisHorzNameProp, "Horz axis", "Name of horizontal axis.");
			DrawString(this.zone_axisVertNameProp, "Vert axis", "Name of vertical axis.");
			DrawCheckBox(this.zone_axisHorzFlipProp,"Flip horz", "Flip horizontal axis.");
			DrawCheckBox(this.zone_axisVertFlipProp,"Flip vert", "Flip vertical axis.");
			DrawFloat(this.zone_axisValScaleProp, "Scale", "Scale of returned pixel delta vector.");
			}
			

		DrawCheckBox(this.zone_emulateMouseProp,	"Emulate mouse pos", "When touched, screen px position will be used as emulated mouse pos for CFInput.mousePosition.");
		if (this.zone_emulateMouseProp.boolValue)
			{
			DrawCheckBox(this.zone_mousePosFromFirstFingerProp, 
				"First finger only", "Mouse pos will be taken from first finger only, instead of unified touch position.");
			}
			

		if (showZoneCodeSection = EditorGUILayout.Foldout(showZoneCodeSection, 
			"Code Geneneration"))
			{
			EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("Check All")) this.SetZoneCodeParams(true);
				if (GUILayout.Button("Clear All")) this.SetZoneCodeParams(false);
			EditorGUILayout.EndHorizontal();

			DrawCheckBox(this.zone_codeUniJustPressedProp,	"Uni Just Pressed",	"Unified-touch 'just pressed' section.");	
			DrawCheckBox(this.zone_codeUniPressedProp, 		"Uni Pressed",		"Unified-touch 'pressed' section.");	
			if (this.zone_codeUniPressedProp.boolValue)
				{
				DrawCheckBox(this.zone_codeUniJustDraggedProp, 	"Uni Just Dragged",	"Unified-touch 'just dragged' section.");	
				DrawCheckBox(this.zone_codeUniDraggedProp, 		"Uni Dragged",		"Unified-touch 'dragged' section.");	
				}

			DrawCheckBox(this.zone_codeUniJustReleasedProp, "Uni Just Released","Unified-touch 'just released' section.");	
			if (this.zone_codeUniJustReleasedProp.boolValue)
				{
				DrawCheckBox(this.zone_codeUniReleasedDragProp, "Uni Released Drag", "Unified-touch 'dragged before release' section.");	
				}

			if (this.zone_enableSecondFingerProp.boolValue)
				{
				DrawCheckBox(this.zone_codeMultiJustPressedProp, "Multi Just Pressed", "Multi-touch 'just pressed' section.");	
				DrawCheckBox(this.zone_codeMultiPressedProp, 	"Multi Pressed", "Multi-touch 'pressed' section.");	
				if (this.zone_codeMultiPressedProp.boolValue)
					{
					DrawCheckBox(this.zone_codeMultiJustDraggedProp, "Multi Just Dragged", "Multi-touch 'just dragged' section.");	
					DrawCheckBox(this.zone_codeMultiDraggedProp, 	"Multi Dragged", "Multi-touch 'dragged' section.");	
					DrawCheckBox(this.zone_codeJustTwistedProp, 	"Just Twisted", "Multi-touch 'just twisted' section.");	
					DrawCheckBox(this.zone_codeTwistedProp, 		"Twisted", "Multi-touch 'twisted' section.");	
					DrawCheckBox(this.zone_codeJustPinchedProp, 	"Just Pinched", "Multi-touch 'just pinched' section.");	
					DrawCheckBox(this.zone_codePinchedProp, 		"Pinched", "Multi-touch 'pinched' section.");	
					}

				DrawCheckBox(this.zone_codeMultiJustReleasedProp,"Multi Just Released", "Multi-touch 'just released' section.");	
				if (this.zone_codeMultiJustReleasedProp.boolValue)
					{
					DrawCheckBox(this.zone_codeMultiReleasedDragProp,"Multi Released Drag", "Multi-touch 'dragged before release' section.");	
					DrawCheckBox(this.zone_codeReleasedTwistProp, 	"Released Twist", "Multi-touch 'twisted' section in multi-touch 'released' section.");	
					DrawCheckBox(this.zone_codeReleasedPinchProp, 	"Released Pinch", "Multi-touch 'pinched' section in multi-touch 'released' section.");	
					}
				}
			
			DrawCheckBox(this.zone_codeDoubleTapProp, 		"Double Tap", "Double tap event section (See \ref TouchZone.JustDoubleTapped()).");	
			DrawCheckBox(this.zone_codeSimpleTapProp, 		"Simple Tap", "Simple tap event section (See \ref TouchZone.JustTapped()).");	
			if (!this.zone_codeSimpleTapProp.boolValue)
				{
				DrawCheckBox(this.zone_codeSingleTapProp, 		"Single Tap", "Single tap event section (See \ref TouchZone.JustSingleTapped()).");	
				}

			if (this.zone_enableSecondFingerProp.boolValue)
				{
				DrawCheckBox(this.zone_codeMultiDoubleTapProp, 	"Multi Double Tap", "Multi-touch Double tap event section (See \ref TouchZone.JustMultiDoubleTapped()).");	
				DrawCheckBox(this.zone_codeSimpleMultiTapProp, 	"Multi Simple Tap", "Simple multi-touch tap event section (See \ref TouchZone.JustMultiTapped()).");	
				if (!this.zone_codeSimpleMultiTapProp.boolValue)
					{
					DrawCheckBox(this.zone_codeMultiSingleTapProp, 	"Multi Single Tap", "Multi-touch Single tap event section (See \ref TouchZone.JustMultiSingleTapped()).");	
					}
				}

			DrawCheckBox(this.zone_codeCustomGUIProp, 		"Custom GUI", "Add custom GUI section for this zone.");	
			DrawCheckBox(this.zone_codeCustomLayoutProp, 	"Custom Layout", "Add custom layout section for this zone.");	
		
			}

		
		EditorGUILayout.EndVertical();			
		}		


	}

	
	
// -------------------
private void SetZoneCodeParams(bool state)
	{
	this.zone_codeUniJustPressedProp.boolValue		= state;
	this.zone_codeUniPressedProp.boolValue			= state;
	this.zone_codeUniJustReleasedProp.boolValue		= state;
	this.zone_codeUniJustDraggedProp.boolValue		= state;
	this.zone_codeUniDraggedProp.boolValue			= state;
	this.zone_codeUniReleasedDragProp.boolValue		= state;
	this.zone_codeMultiJustPressedProp.boolValue	= state;
	this.zone_codeMultiPressedProp.boolValue		= state;
	this.zone_codeMultiJustReleasedProp.boolValue	= state;
	this.zone_codeMultiJustDraggedProp.boolValue	= state;
	this.zone_codeMultiDraggedProp.boolValue		= state;
	this.zone_codeMultiReleasedDragProp.boolValue	= state;
	this.zone_codeJustTwistedProp.boolValue		= state;
	this.zone_codeTwistedProp.boolValue			= state;
	this.zone_codeReleasedTwistProp.boolValue	= state;
	this.zone_codeJustPinchedProp.boolValue		= state;
	this.zone_codePinchedProp.boolValue			= state;
	this.zone_codeReleasedPinchProp.boolValue	= state;
	this.zone_codeSimpleTapProp.boolValue		= state;
	this.zone_codeSingleTapProp.boolValue		= state;
	this.zone_codeDoubleTapProp.boolValue		= state;
	this.zone_codeSimpleMultiTapProp.boolValue	= state;
	this.zone_codeMultiSingleTapProp.boolValue	= state;
	this.zone_codeMultiDoubleTapProp.boolValue	= state;
	this.zone_codeCustomGUIProp.boolValue		= state;
	this.zone_codeCustomLayoutProp.boolValue	= state;
	}



// ((((((((((((((((((((((((
	


// *************************

// ---------------------
// Layout Box Editor -------
// ---------------------

private SerializedProperty
	layoutBox_nameProp,
	layoutBox_anchorProp,
	layoutBox_ignoreLeftHandedModeProp,
	layoutBox_allowNonuniformScaleProp,
	layoutBox_normalizedRectProp,
	layoutBox_horzMarginMaxProp,
	layoutBox_vertMarginMaxProp,
	layoutBox_uniformMarginsProp,
	layoutBox_debugColorProp,
	layoutBox_debugDrawProp;
	
private int layoutBox_curId;

// --------------------
private void SelectLayoutBox(int i)
	{
	//UnfocusControls();

	this.layoutBoxesProp = this.serializedObject.FindProperty("layoutBoxes");

	if ((this.layoutBoxesProp == null) || (this.layoutBoxesProp.arraySize == 0))
		{	
		this.layoutBox_nameProp					= null;
		this.layoutBox_anchorProp				= null;
		this.layoutBox_allowNonuniformScaleProp	= null;
		this.layoutBox_normalizedRectProp		= null;
		this.layoutBox_horzMarginMaxProp		= null;
		this.layoutBox_vertMarginMaxProp		= null;
		this.layoutBox_uniformMarginsProp		= null;
		this.layoutBox_debugColorProp			= null;
		this.layoutBox_debugDrawProp			= null;
		
		return;
		}


	if (i < 0) 
		i = 0;		
	else if (i >= this.layoutBoxesProp.arraySize) 
		i = (this.layoutBoxesProp.arraySize - 1);


	this.layoutBox_curId = i;

	SerializedProperty b = this.layoutBoxesProp.GetArrayElementAtIndex(i);

	this.layoutBox_nameProp					= b.FindPropertyRelative("name");
	this.layoutBox_anchorProp				= b.FindPropertyRelative("anchor");
	this.layoutBox_ignoreLeftHandedModeProp	= b.FindPropertyRelative("ignoreLeftHandedMode");
	this.layoutBox_allowNonuniformScaleProp	= b.FindPropertyRelative("allowNonuniformScale");
	this.layoutBox_normalizedRectProp		= b.FindPropertyRelative("normalizedRect");
	this.layoutBox_horzMarginMaxProp		= b.FindPropertyRelative("horzMarginMax");
	this.layoutBox_vertMarginMaxProp		= b.FindPropertyRelative("vertMarginMax");
	this.layoutBox_uniformMarginsProp		= b.FindPropertyRelative("uniformMargins");
	this.layoutBox_debugColorProp			= b.FindPropertyRelative("debugColor");
	this.layoutBox_debugDrawProp			= b.FindPropertyRelative("debugDraw");
	}


// ---------------------
private void ResetCurrentLayoutBox()
	{	
	if (this.layoutBox_anchorProp == null)
		return;
	
	UnfocusControls();

	TouchController joy = this.target as TouchController;
	if (joy == null)
		return;



	//this.layoutBox_anchorProp				= null;
	this.layoutBox_allowNonuniformScaleProp.boolValue = false;
	this.layoutBox_normalizedRectProp.rectValue	= new Rect(0,0, 1,1);
	this.layoutBox_horzMarginMaxProp.floatValue	= 0.5f;
	this.layoutBox_vertMarginMaxProp.floatValue	= 0.5f;
	this.layoutBox_uniformMarginsProp.boolValue	= true;
	this.layoutBox_debugColorProp.colorValue	= new Color(1,1,1, 0.5f);
	this.layoutBox_debugDrawProp.boolValue		= true;
	}
	

// -------------------
private void MoveLayoutBoxSelection(int delta)
	{
	UnfocusControls();

	if ((this.layoutBoxesProp == null) || (this.layoutBoxesProp.arraySize == 0))
		{
		this.SelectLayoutBox(0);
		return;		
		}

	this.layoutBox_curId += delta;
	if (this.layoutBox_curId < 0) 
		this.layoutBox_curId = this.layoutBoxesProp.arraySize - 1;
	else if (this.layoutBox_curId >= this.layoutBoxesProp.arraySize)
		this.layoutBox_curId = 0;

	this.SelectLayoutBox(this.layoutBox_curId);	
	}
	


// ---------------------
private void DrawLayoutBoxEditorSection()
	{	
	//EditorGUIUtility.LookLikeInspector();

	TouchController joy = this.target as TouchController;
	SerializedProperty arrayProp = this.layoutBoxesProp;

	EditorGUILayout.BeginHorizontal(this.sectionToolbarStyle, //EditorStyles.toolbar, 
		GUILayout.ExpandWidth(true), GUILayout.Height(30));
	
	if (GUILayout.Button(new GUIContent("<", "Select previous"), EditorStyles.miniButtonLeft, GUILayout.Width(20)))
		this.MoveLayoutBoxSelection(-1);
	if (GUILayout.Button(new GUIContent(">", "Select next"), EditorStyles.miniButtonRight, GUILayout.Width(20)))
		this.MoveLayoutBoxSelection(1);

	//EditorGUILayout.LabelField((arrayProp.arraySize == 0) ? "-" : 
	//	("" + (this.layoutBox_curId + 1)) + "/" + arrayProp.arraySize + 
	//	" : " + (((TouchController.LayoutBoxId)this.layoutBox_curId).ToString()), 
	//	EditorStyles.miniLabel, 
	//	GUILayout.Width(90), GUILayout.ExpandWidth(true));

	EditorGUILayout.LabelField((arrayProp.arraySize == 0) ? "-" : 
		("" + (this.layoutBox_curId + 1)) + "/" + arrayProp.arraySize + 
		" : " + ((this.layoutBox_nameProp == null) ? 
		"-" : this.layoutBox_nameProp.stringValue), EditorStyles.miniLabel, 
		GUILayout.Width(90), GUILayout.ExpandWidth(true));



	//EditorGUILayout.EndHorizontal();
	//EditorGUILayout.BeginHorizontal();
		

	EditorGUILayout.EndHorizontal();
		

	// Draw current box' params...

	if ((this.layoutBox_anchorProp != null) && (joy.layoutBoxes != null) && 
		(this.layoutBox_curId < joy.layoutBoxes.Length) && (this.layoutBox_curId >= 0))
		{
		EditorGUILayout.BeginVertical(this.editSectionStyle, 
			GUILayout.ExpandWidth(true));

			
		//TouchController.LayoutBox lbox = joy.layoutBoxes[this.layoutBox_curId];

		if (DrawString(this.layoutBox_nameProp, "Name"))
			{ this.OnLayoutBoxNameChange(); }

		
		if (DrawEnumPopup(this.layoutBox_anchorProp, "Anchor", "Anchor point for content")) { joy.SetLayoutDirtyFlag(); }

		if (DrawRectField(this.layoutBox_normalizedRectProp, 0,0,1,1, 
			"Rect", "Normalized screen rect for this layout box. (0,0 = top left, 1,1 = bottom-right)"))  { joy.SetLayoutDirtyFlag(); }

		if (DrawCheckBox(this.layoutBox_ignoreLeftHandedModeProp, "Ignore Left-Handed Mode", "Ignore left-handed mode and doesn't mirror contained controls' positioning."))  { joy.SetLayoutDirtyFlag(); }

		if (DrawCheckBox(this.layoutBox_allowNonuniformScaleProp, "Non-uniform scale", "Allow non-uniform scaling of contained controls' positions (their sizes will keep correct aspect ratio)"))  { joy.SetLayoutDirtyFlag(); }
			
		if (DrawCheckBox(this.layoutBox_uniformMarginsProp, "Uniform margins", "Uniform margin scaling"))  { joy.SetLayoutDirtyFlag(); }

		if (DrawCmField(this.layoutBox_horzMarginMaxProp, 0, 10, "Horz margin", "Horizontal margin")) {  joy.SetLayoutDirtyFlag(); }
		if (DrawCmField(this.layoutBox_vertMarginMaxProp, 0, 10, "Vert margin", "Vertical margin")) {  joy.SetLayoutDirtyFlag(); }
			
		EditorGUILayout.Space();
		
		DrawCheckBox(this.layoutBox_debugDrawProp, "Draw debug region");
		if (this.layoutBox_debugDrawProp.boolValue)
			DrawColorProp(this.layoutBox_debugColorProp, "Debug color");



		EditorGUILayout.EndVertical();			
		}		
	}



// ---------------
private string[] layoutBoxNameArray;

private void OnLayoutBoxNameChange()
	{
	if ((this.layoutBoxNameArray == null) ||
		(this.layoutBoxNameArray.Length != TouchController.LayoutBoxCount))
		{
		this.layoutBoxNameArray = new string[TouchController.LayoutBoxCount];
		}

	if ((this.layoutBoxesProp != null) && 
		(this.layoutBoxesProp.arraySize == TouchController.LayoutBoxCount))
		{
		for (int i = 0; i < this.layoutBoxesProp.arraySize; ++i)
			this.layoutBoxNameArray[i] = this.layoutBoxesProp.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue;
		}
	else
		{
		for (int i = 0; i < TouchController.LayoutBoxCount; ++i)
			this.layoutBoxNameArray[i] = ("Box" + i.ToString("00"));
		}
	
	}









// ---------------------
// Preset menu ---------
// ---------------------

private GenericMenu presetMenu;


// ---------------------
private class ScreenPreset
	{
	public int 	width,
				height,
				dpi;
	
	// ------------
	public ScreenPreset(int w, int h, int dpi)
		{
		this.width 	= w;
		this.height = h;
		this.dpi 	= dpi;
		}
	}

		
// --------------------
private void ApplyPreset(object presetObj)
	{
//Debug.Log("APPLY!");
	ScreenPreset 	preset 	= presetObj as ScreenPreset;
	TouchController joy 	= this.target as TouchController;
	if ((joy == null) || (preset == null))
		{
		Debug.Log("ERR joy:" + (joy == null) + " pres:" + (preset == null));
		return;
		}	

//Debug.Log("OK : " + preset.width + " " + preset.height);

	this.serializedObject.Update();

	this.screenEmuHwDpiProp.intValue = preset.dpi;
	this.screenEmuHwHorzResProp.intValue = preset.width;
	this.screenEmuHwVertResProp.intValue = preset.height;
			
	this.serializedObject.ApplyModifiedProperties();

	joy.SetLayoutDirtyFlag();
	joy.SaveScreenEmuConfig();
	}
		


	

// -------------------
private void InitPresetMenu()
	{
	this.presetMenu = new GenericMenu();
		
	
	this.AddPreset("iPhone 1, 2, 3", 				480, 	320, 	163);
	this.AddPreset("iPhone 4, 4S", 					960, 	640, 	326);
	this.AddPreset("iPhone 5", 						1136, 	640, 	326);
	this.presetMenu.AddSeparator("iOS Tablets");
	this.AddPreset("iPad 1, 2", 					1024, 	768, 	132);
	this.AddPreset("iPad 3, 4, Air", 				2048, 	1536,	264);
	this.AddPreset("iPad Mini", 					1024, 	768, 	163);
	this.presetMenu.AddSeparator("Google Tablets"); 
	this.AddPreset("Galaxy Nexus",					1280, 	720, 	316);
	this.AddPreset("Nexus 4", 						1280, 	768, 	320);
	this.AddPreset("Nexus 7 (2012)", 				1280, 	800, 	216);
	this.AddPreset("Nexus 7 (2013)", 				1920,	1200,	323);
	this.AddPreset("Nexus 10", 						2560, 	1600, 	299);
	this.presetMenu.AddSeparator("Amazon Tablets");
	this.AddPreset("Kindle Fire", 					1024, 	600, 	169);
	this.AddPreset("Kindle Fire HD 7\"",			1280, 	800, 	216);
	this.AddPreset("Kindle Fire HD 8.9\"",			1920,  1200, 	254);
	this.presetMenu.AddSeparator("Nook Tablets");
	this.AddPreset("Nook", 							1024, 	600, 	169);
	this.AddPreset("Nook HD",						1440, 	900, 	243);
	this.AddPreset("Nook HD Plus",					1920,  1280, 	257);
	this.presetMenu.AddSeparator("Samsung Galaxy Tab 1, 2, 3");
	this.AddPresetDiag("Galaxy Tab X 7.0",			1024,	600,	7);
	this.AddPresetDiag("Galaxy Tab X 8.0",			1280,	800,	8);
	this.AddPresetDiag("Galaxy Tab X 10.1",			1280,	800,	10.1f);
	this.presetMenu.AddSeparator("Samsung Galaxy Note");
	this.AddPresetDiag("Galaxy Note",				1280,	800,	5.3f);
	this.AddPresetDiag("Galaxy Note II",			1280,	720,	5.55f);
	this.AddPresetDiag("Galaxy Note 3",				1920,	1080,	5.7f);
	this.AddPresetDiag("Galaxy Note 8.0",			1280,	800,	8.0f);
	this.AddPresetDiag("Galaxy Note 10.1",			1280,	720,	10.1f);
	this.AddPresetDiag("Galaxy Note 10.01 (2014)",	2560,	1600,	10.1f);

	this.presetMenu.AddSeparator("Windows Phone 8");
	this.AddPresetDiag("HTC 8S", 							800, 	480, 	4.0f);
	this.AddPresetDiag("HTC 8X", 							1280, 	720, 	4.3f);
	this.AddPresetDiag("HTC 8XT", 							800, 	480, 	4.3f);
	this.AddPresetDiag("Huawei Ascend W1", 					800, 	480, 	4.0f);
	this.AddPresetDiag("Huawei Ascend W2",		 			800, 	480, 	4.3f);
	this.AddPresetDiag("Nokia Lumia 520, 521, 620", 		800, 	480, 	3.8f);
	this.AddPresetDiag("Nokia Lumia 625", 					800, 	480, 	4.7f);
	this.AddPresetDiag("Nokia Lumia 720, 810, 820, 822",	800, 	480, 	4.3f);
	this.AddPresetDiag("Nokia Lumia 920, 925, 928, 1020",	1280, 	768, 	4.5f);
	this.AddPresetDiag("Samsung ATIV S, S Neo", 			1280, 	720, 	4.8f);
	this.AddPresetDiag("Samsung ATIV Odyssey", 				800, 	480, 	4.0f);
	
	this.presetMenu.AddSeparator("BlackBerry 10");		
	this.AddPresetDiag("BlackBerry Z10", 					1280, 	768, 	4.2f);
	this.AddPresetDiag("BlackBerry Q5, Q10", 				720, 	720, 	3.1f);

	}
				
// --------------------
private void AddPreset(string name, int w, int h, int dpi)
	{	
	if (this.presetMenu == null)
		this.presetMenu = new GenericMenu();

	bool isCur = false;

	TouchController joy = this.target as TouchController;
	if (joy != null)
		isCur =	((w == joy.screenEmuHwHorzRes) &&
				(h == joy.screenEmuHwVertRes) &&
				(dpi == joy.screenEmuHwDpi));

	this.presetMenu.AddItem(new GUIContent(name), isCur, 
		this.ApplyPreset, new ScreenPreset(w, h, dpi)); 


/*		
	// Gen specifications...
		
string unitSuffix = (((TouchController)this.target).rwUnit == TouchController.RealWorldUnit.CM) ? "cm" : "in";
float specDimDiv = (((TouchController)this.target).rwUnit == TouchController.RealWorldUnit.CM) ?
	((float)dpi / 2.54f) : ((float)dpi);

DEBUG_SPEC_STRING += 	
	"\t<tr>\n" +
	"\t\t<td><b>" + name + "</b></td>\n" +
	"\t\t<td>" + Mathf.RoundToInt(dpi) + "</td>\n" +
	"\t\t<td>" + Mathf.RoundToInt((float)dpi / 2.54f) + "</td>\n" +
	"\t\t<td>" + GetAspectRatioName(w, h) + "</td>\n" +
	"\t\t<td>" + w + "</td>\n" +
	"\t\t<td>" + h + "</td>\n" +
	"\t\t<td>" + (Mathf.Round((10.0f * w) / specDimDiv) / 10.0f)  + unitSuffix + "</td>\n" +
	"\t\t<td>" + (Mathf.Round((10.0f * h) / specDimDiv) / 10.0f)  + unitSuffix + "</td>\n" +
	"\t\t<td>" + (Mathf.Round((10.0f * (Mathf.Sqrt(w*w + h*h))) / specDimDiv) / 10.0f)  + unitSuffix + "</td>\n" +
	"\t</tr>\n";
*/

	}


// ---------------------	
private void AddPresetDiag(string name, int w, int h, float diagonalInches)
	{
	this.AddPreset(name, w, h, 
		Mathf.RoundToInt(Mathf.Sqrt((float)((w*w) + (h*h))) / diagonalInches));
	}

	

//static string DEBUG_SPEC_STRING = "";

// ----------------	
public void ShowPresetMenu()
	{
//DEBUG_SPEC_STRING = "";

	//if (this.presetMenu == null)
		this.InitPresetMenu();

//EditorGUIUtility.systemCopyBuffer = DEBUG_SPEC_STRING;

	this.presetMenu.ShowAsContext();
	}




// ---------------
private string GetAspectRatioName(int w, int h)
	{
	if 		(h == 			w)			return "1:1";
	else if ((h * 15) == 	(w * 9))	return "15:9";
	else if ((h * 16) == 	(w * 9))	return "16:9";
	else if ((h * 16) == 	(w * 10))	return "16:10";
	else if ((h * 3) == 	(w * 2))	return "3:2";
	else if ((h * 4) == 	(w * 3))	return "4:3";
	else if ((h * 1024) ==	(w * 600))	return "17:10";	
	else if ((h * 1136)	== 	(w * 640))	return "16:9";	// iPhone 5 
		

	else if ((h * 9) == 	(w * 15))	return "9:15";
	else if ((h * 9) == 	(w * 16))	return "9:16";
	else if ((h * 10) == 	(w * 16))	return "10:16";
	else if ((h * 2) == 	(w * 3))	return "2:3";
	else if ((h * 3) == 	(w * 4))	return "3:4";
	else if ((h * 600) ==	(w * 1024))	return "10:17";	
	else if ((h * 640) == 	(w * 1136))	return "9:16";	// iPhone 5 
		
	return "Non-standard.";
	}


// ---------------------
// Utils ---------------
// ---------------------
	
	
// ---------------
private bool DrawString(
	SerializedProperty 	prop, 
	string 				label,
	string				tooltip = "",
	params GUILayoutOption[] guiOptions)
	{	
	if (prop == null) 
		return false;
	if (label == null)
		label = prop.name;
		

	string v = prop.stringValue;
	EditorGUILayout.PropertyField(prop, new GUIContent(label, tooltip), guiOptions);

	return (v != prop.stringValue);
	}


// ---------------
static private void DrawGenericProp(
	SerializedProperty	prop,
	string 				label	= null,
	string 				tooltip = "")
	{
	if (prop == null)
		{
		EditorGUILayout.LabelField("ERROR!");
		return;
		}

	if (label == null)
		EditorGUILayout.PropertyField(prop);
	else
		EditorGUILayout.PropertyField(prop, new GUIContent(label, tooltip));
	}



// ---------------
static private bool DrawCheckBox(
	SerializedProperty 	prop, 
	string 				label,
	string				tooltip = "",
	params GUILayoutOption[] guiOptions)
	{	
	if (prop == null) 
		return false;

	bool v = prop.boolValue;
	if (label != null)
		EditorGUILayout.PropertyField(prop, new GUIContent(label, tooltip), guiOptions);
	else		
		EditorGUILayout.PropertyField(prop, guiOptions);

	return (v != prop.boolValue);
	}

	
// ---------------
static private bool DrawEnumPopup(
	SerializedProperty 	prop, 
	string 				label,
	string 				tooltip = "",
	params GUILayoutOption[] guiOptions)
	{	
	if (prop == null) 
		return false;

	int v = prop.intValue;
	if (label != null)
		EditorGUILayout.PropertyField(prop, new GUIContent(label, tooltip), guiOptions);
	else		
		EditorGUILayout.PropertyField(prop, guiOptions);

	return (v != prop.intValue);
	}


// ---------------
static private bool DrawSlider(
	SerializedProperty	prop, 
	float 				vfrom, 
	float 				vto,	
	string 				label,
	string				tooltip = "",
	params GUILayoutOption[] guiOptions)
	{	
	if (prop == null) 
		return false;

	float v = prop.floatValue;
	if (label != null)
		EditorGUILayout.Slider(prop, vfrom, vto, new GUIContent(label, tooltip), guiOptions);
	else		
		EditorGUILayout.Slider(prop, vfrom, vto, guiOptions);

	return (v != prop.floatValue);
	}
	

// ---------------
static private bool DrawIntSlider(
	SerializedProperty 	prop, 
	int 				vfrom, 
	int 				vto, 
	string 				label,
	string				tooltip = "",
	params GUILayoutOption[] guiOptions)
	{	
	if (prop == null) 
		return false;

	int v = prop.intValue;
	if (label != null)
		EditorGUILayout.IntSlider(prop, vfrom, vto, new GUIContent(label, tooltip), guiOptions);
	else		
		EditorGUILayout.IntSlider(prop, vfrom, vto, guiOptions);

	return (v != prop.intValue);
	}

	
// ---------------
static private bool DrawFloat(
	SerializedProperty	prop,
	string 				label,
	string				tooltip = "",
	params GUILayoutOption[] guiOptions)
	{	
	if (prop == null) 
		return false;

	float v = prop.floatValue;
	if (label != null)
		EditorGUILayout.PropertyField(prop, new GUIContent(label, tooltip), guiOptions);
	else		
		EditorGUILayout.PropertyField(prop, guiOptions);

	return (v != prop.floatValue);
	}

	
// ---------------
static private bool DrawFloatEx(
	SerializedProperty	prop,
	float 				vmin,
	float 				vmax,	
	string 				label,
	string				tooltip = "",
	params GUILayoutOption[] guiOptions)
	{	
	if (prop == null) 
		return false;

	float v = prop.floatValue;
	if (label != null)
		EditorGUILayout.PropertyField(prop, new GUIContent(label, tooltip), guiOptions);
	else		
		EditorGUILayout.PropertyField(prop, guiOptions);
		

	if 		(prop.floatValue < vmin) prop.floatValue = vmin;
	else if (prop.floatValue > vmax) prop.floatValue = vmax;

	return (v != prop.floatValue);
	}


// ---------------
static private bool DrawInt(
	SerializedProperty	prop,	
	string 				label,
	string				tooltip = "",
	params GUILayoutOption[] guiOptions)
	{	
	if (prop == null) 
		return false;

	int v = prop.intValue;
	if (label != null)
		EditorGUILayout.PropertyField(prop, new GUIContent(label, tooltip), guiOptions);
	else		
		EditorGUILayout.PropertyField(prop, guiOptions);

	return (v != prop.intValue);
	}
	
// ---------------
static private bool DrawIntEx(
	SerializedProperty	prop,
	int 				vmin,
	int 				vmax,	
	string 				label,
	string				tooltip = "",
	params GUILayoutOption[] guiOptions)
	{	
	if (prop == null) 
		return false;

	int v = prop.intValue;
	if (label != null)
		EditorGUILayout.PropertyField(prop, new GUIContent(label, tooltip), guiOptions);
	else		
		EditorGUILayout.PropertyField(prop, guiOptions);
		

	if 		(prop.intValue < vmin) prop.intValue = vmin;
	else if (prop.intValue > vmax) prop.intValue = vmax;

	return (v != prop.intValue);
	}

// ---------------
static private bool DrawEnumField(
	SerializedProperty	prop,
	string 				label,
	string				tooltip = "",
	params GUILayoutOption[] guiOptions)
	{	
	if (prop == null) 
		return false;

	int v = prop.intValue;
	if (label != null)
		EditorGUILayout.PropertyField(prop, new GUIContent(label, tooltip), guiOptions);
	else		
		EditorGUILayout.PropertyField(prop, guiOptions);

	return (v != prop.intValue);
	}
	

	
// ------------------
private void DrawPixValInRealWorldUnitsLabel(string label, float v, int roundMode = -1)
	{
	TouchController joy = this.target as TouchController;
	if (joy == null)
		return;

	bool initialGuiEnable = GUI.enabled;
	if (GUI.enabled)
		GUI.enabled = false;
		

	switch (joy.rwUnit)
		{
		case TouchController.RealWorldUnit.CM :
			label += " (cm)";
			v = (v / (joy.screenEmuHwDpi / 2.54f)); 
			break;

		case TouchController.RealWorldUnit.INCH :
			label += " (in)";
			v = (v / (joy.screenEmuHwDpi)); 
			break;
		}

		
	if (roundMode >= 0)
		v = RoundVal(v, roundMode);

	EditorGUILayout.FloatField(label, v); 

	GUI.enabled = initialGuiEnable;
	}
	

// ------------------
private void DrawPixVec2InRealWorldUnitsLabel(string label, Vector2 v, int roundMode = -1)
	{
	TouchController joy = this.target as TouchController;
	if (joy == null)
		return;

	bool initialGuiEnable = GUI.enabled;
	if (GUI.enabled)
		GUI.enabled = false;
		

	switch (joy.rwUnit)
		{
		case TouchController.RealWorldUnit.CM :
			label += " (cm)";
			v = (v / (joy.screenEmuHwDpi / 2.54f)); 
			break;

		case TouchController.RealWorldUnit.INCH :
			label += " (in)";
			v = (v / (joy.screenEmuHwDpi)); 
			break;
		}

		
	if (roundMode >= 0)
		{
		v.x = RoundVal(v.x, roundMode);
		v.y = RoundVal(v.y, roundMode);
		}

	EditorGUILayout.Vector2Field(label, v); 

	GUI.enabled = initialGuiEnable;
	}


// ------------------
private void DrawCmValInPixelsLabel(string label, float v)
	{
	TouchController joy = this.target as TouchController;
	if (joy == null)
		return;

	bool initialGuiEnable = GUI.enabled;
	if (GUI.enabled)
		GUI.enabled = false;
		
	EditorGUILayout.FloatField(label, (v * (joy.screenEmuHwDpi / 2.54f))); 

	GUI.enabled = initialGuiEnable;
	}
	

// ------------------
private void DrawCmVec2InPixelsLabel(string label, Vector2 v)
	{
	TouchController joy = this.target as TouchController;
	if (joy == null)
		return;

	bool initialGuiEnable = GUI.enabled;
	if (GUI.enabled)
		GUI.enabled = false;
		
	EditorGUILayout.Vector2Field(label, (v * (joy.screenEmuHwDpi / 2.54f))); 

	GUI.enabled = initialGuiEnable;
	}

	
// ------------------
private void DrawFloatLabel(string label, float v)
	{
	bool initialGuiEnable = GUI.enabled;
	if (GUI.enabled)
		GUI.enabled = false;
		
	EditorGUILayout.FloatField(label, v); 

	GUI.enabled = initialGuiEnable;
	}


// ------------------
private void DrawStringLabel(string label, string v)
	{
	bool initialGuiEnable = GUI.enabled;
	if (GUI.enabled)
		GUI.enabled = false;
		
	EditorGUILayout.TextField(label, v); 

	GUI.enabled = initialGuiEnable;
	}




// ---------------
private bool DrawCmField(
	SerializedProperty	prop,
	float				vmin,
	float 				vmax,
	string 				label,
	string				tooltip = "",
	params GUILayoutOption[] guiOptions)
	{	
	TouchController joy = this.target as TouchController;
	if (joy == null)
		return false;
	if (prop == null) 
		return false;
		
	float initialv = prop.floatValue;
	float v = prop.floatValue;	

	if (joy.rwUnit == TouchController.RealWorldUnit.INCH)
		v /= 2.54f;		

	if (label == null)
		label = prop.name;		
	if (tooltip == null)
		tooltip = "";


	label += ((joy.rwUnit == TouchController.RealWorldUnit.CM) ? " (cm)" : " (in)");
	v = EditorGUILayout.FloatField(new GUIContent(label, tooltip), v, guiOptions);


	if (joy.rwUnit == TouchController.RealWorldUnit.INCH)
		v = v * 2.54f;		
	
	if (v < vmin)	
		v = vmin;
	else if (v > vmax)
		v = vmax;
		
	prop.floatValue = v;

	return (v != initialv);
	}

	
// ---------------
private bool DrawCmVec2Field(
	SerializedProperty	prop,
	float				vmin,
	float 				vmax,
	string 				label,
	string				tooltip = "",
	params GUILayoutOption[] guiOptions)
	{	
	TouchController joy = this.target as TouchController;
	if (joy == null)
		return false;
	if (prop == null) 
		return false;
		
	Vector2 initialv = prop.vector2Value;
	Vector2 v = initialv;	

	if (joy.rwUnit == TouchController.RealWorldUnit.INCH)
		v /= 2.54f;		

	if (label == null)
		label = prop.name;		

	label += ((joy.rwUnit == TouchController.RealWorldUnit.CM) ? " (cm)" : " (in)");
	v = EditorGUILayout.Vector2Field(label, v, guiOptions);


	if (joy.rwUnit == TouchController.RealWorldUnit.INCH)
		v = v * 2.54f;		
	
	if (v.x < vmin)	
		v.x = vmin;
	else if (v.x > vmax)
		v.x = vmax;

	if (v.y < vmin)	
		v.y = vmin;
	else if (v.y > vmax)
		v.y = vmax;
		
	prop.vector2Value = v;

	return (v != initialv);
	}

	
// ---------------
private bool DrawCmVec2XField(
	SerializedProperty	prop,
	float				vmin,
	float 				vmax,
	string 				label,
	string				tooltip = "",
	params GUILayoutOption[] guiOptions)
	{	
	TouchController joy = this.target as TouchController;
	if (joy == null)
		return false;
	if (prop == null) 
		return false;
		
	Vector2 initialv = prop.vector2Value;
	Vector2 v = initialv;	

	if (joy.rwUnit == TouchController.RealWorldUnit.INCH)
		v /= 2.54f;		

	if (label == null)
		label = prop.name;		

	label += ((joy.rwUnit == TouchController.RealWorldUnit.CM) ? " (cm)" : " (in)");
	v.x = EditorGUILayout.Slider(label, v.x, vmin, vmax, guiOptions);


	if (joy.rwUnit == TouchController.RealWorldUnit.INCH)
		v = v * 2.54f;		
	
	v.y = v.x;
		
	prop.vector2Value = v;

	return (v != initialv);
	}





// --------------------
private bool DrawRectField(SerializedProperty prop, 
	float xmin, float ymin, float xmax, float ymax, 
	string 				label,
	string				tooltip = "")
	{
	TouchController joy = this.target as TouchController;
	if (joy == null)
		return false;
	if (prop == null) 
		return false;
		
	Rect initialv = prop.rectValue;
	Rect v = initialv;	

	if (label == null)
		label = prop.name;		

	//label += ((joy.rwUnit == TouchController.RealWorldUnit.CM) ? " (cm)" : " (in)");
	//v = EditorGUILayout.Vector2Field(label, v, guiOptions);
	//v = EditorGUILayout.RectField(new GUIContent(label, tooltip), prop.rectValue);

	v = initialv;

	EditorGUILayout.LabelField(new GUIContent(label, tooltip));	
	v.x 	= EditorGUILayout.Slider(new GUIContent("X", tooltip), v.x, xmin, xmax);
	v.y 	= EditorGUILayout.Slider(new GUIContent("Y", tooltip), v.y, ymin, ymax);
	v.width = EditorGUILayout.Slider(new GUIContent("Width", tooltip), v.width, xmin, xmax);
	v.height= EditorGUILayout.Slider(new GUIContent("Height", tooltip), v.height, ymin, ymax);


	
	// Clamp...
		
//	float x0 = Mathf.Clamp(v.xMin, xmin, xmax);
//	float x1 = Mathf.Clamp(v.xMax, xmin, xmax);
//	float y0 = Mathf.Clamp(v.yMin, ymin, ymax);
//	float y1 = Mathf.Clamp(v.yMax, ymin, ymax);
//	v = Rect.MinMaxRect(x0, y0, x1, y1);
		
	//float x0 = Mathf.Clamp(v.xMin, xmin, xmax);
	//float x1 = Mathf.Clamp(v.xMax, xmin, xmax);
	//float y0 = Mathf.Clamp(v.yMin, ymin, ymax);
	//float y1 = Mathf.Clamp(v.yMax, ymin, ymax);
	//v = //Rect.MinMaxRect(x0, y0, v.width, x1, y1);
		

	float x0 = Mathf.Clamp(v.x, xmin, xmax);
	float y0 = Mathf.Clamp(v.y, xmin, xmax);
	float w = Mathf.Clamp(v.width, ymin, ymax);
	float h = Mathf.Clamp(v.height, ymin, ymax);

	v.x = x0;
	v.y = y0;
	v.width = w;
	v.height = h;


	prop.rectValue = v;

	return (v != initialv);
	}



// -----------------------
private bool DrawVec2SlidersProp(
	SerializedProperty	prop,
	float				xmin,
	float				xmax,
	float				ymin,
	float				ymax,
	string 				label,
	string				tooltip = "",
	params GUILayoutOption[] guiOptions)
	{
	if (prop == null) 
		return false;
		
	Vector2 initialv = prop.vector2Value;
	Vector2 v = initialv;	

	if (label == null)
		label = prop.name;		

	v = initialv;

	EditorGUILayout.LabelField(new GUIContent(label, tooltip));	
	v.x 	= EditorGUILayout.Slider(new GUIContent("X", tooltip), v.x, xmin, xmax);
	v.y 	= EditorGUILayout.Slider(new GUIContent("Y", tooltip), v.y, ymin, ymax);


	prop.vector2Value = v;

	return (v != initialv);
	}

	

// ---------------------
private bool DrawColorProp(	
	SerializedProperty	prop,
	string 				label,
	string				tooltip = "",
	params GUILayoutOption[] guiOptions)
	{	
	if (prop == null) 
		return false;

	Color v = prop.colorValue;
	if (label != null)
		EditorGUILayout.PropertyField(prop, new GUIContent(label, tooltip), guiOptions);
	else		
		EditorGUILayout.PropertyField(prop, guiOptions);

	return (v != prop.colorValue);
	}
	


// -------------------
private bool DrawLayoutBoxIdProp(
	SerializedProperty	prop,
	string 				label,
	string				tooltip = "",
	params GUILayoutOption[] guiOptions)
	{	
	if ((prop == null) || (this.layoutBoxNameArray == null) || 
		(this.layoutBoxNameArray.Length != TouchController.LayoutBoxCount)) 
		return false;
	
	if (label == null)
		label = prop.name;		

	int v = prop.intValue;
	
	if (prop.intValue != (v = EditorGUILayout.Popup(label, //(new GUIContent(label, tooltip), 
		v, this.layoutBoxNameArray, guiOptions)))
		{
		prop.intValue = v;
		return true;
		}

	return false;
	}


// ------------------
static private float RoundVal(float v, int d)
	{	
	if (d <= 0)
		return Mathf.Round(v);
		
	float scale = Mathf.Pow(10, d);
	return ((Mathf.Round(v * scale) / scale));
	}
	


// ----------------
static private void UnfocusControls()
	{
	GUIUtility.hotControl = 0;
	GUIUtility.keyboardControl = 0;
	}
	

	

// --------------------
[MenuItem ("GameObject/Create Other/Control Freak Controller")]
private static void MenuCreateCF()	
	{
	GameObject go = new GameObject("CONTROL-FREAK", typeof(TouchController));
	go.GetComponent<TouchController>().InitController();
	Selection.activeGameObject = go;
	}

}

//}

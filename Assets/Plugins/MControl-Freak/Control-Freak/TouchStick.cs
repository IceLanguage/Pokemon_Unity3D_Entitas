// -----------------------------------------------
// Control Freak - The Ultimate Virtual Controller
// Copyright (C) 2013 Dan's Game Tools
// -----------------------------------------------

#if UNITY_EDITOR || UNITY_WEBPLAYER

#	define DEBUG_KEYBOARD_CONTROL

#endif

using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif




// ---------------------
/*! 
\brief Touch Stick Class.
 
Touch Stick works in three modes at the same time:
\li Analog 
\li 8-way Digital
\li 4-way Digital

**/
// ---------------------
/// \nosubgrouping

[System.Serializable]
public class TouchStick : TouchableControl
	{

// --------------
/// \cond DGT_DOCS_SHOW_PUBLIC_VARS
// --------------

	private float 		safeAngle;		// last stored "safe" angle 

		
	public StickPosMode stickVis;				///< Stick position mode
	public bool			smoothReturn	= true;	///< Smoothly return to neutral after release.

	public Vector2		posCm	= new Vector2(2, 5);	///< static mode position
	public float		sizeCm	= 2.0f;					///< static mode size
	
	private Vector2		layoutPosPx;		///< Static position calculated by layout 
	private float		layoutRadPx;

	private Vector2		posPx = new Vector2(100, 100);	///< Current static/dynamic position
	private float		radPx = 40;


	
	public bool			overrideAnimDuration;					
	public float		pressAnimDuration;
	public float		releaseAnimDuration;
	public float		disableAnimDuration;
	public float		enableAnimDuration;
	public float		hideAnimDuration;
	public float		showAnimDuration;
	
	private AnimTimer					animTimer;
	private TouchController.AnimFloat	animHatScale,
										animBaseScale,
										animAlpha;	
	private TouchController.AnimColor	animHatColor,
										animBaseColor;
	private bool						dynamicFadeOutAnimPending;



	public bool			keyboardEmu = false;
	public KeyCode		keyUp		= KeyCode.W;
	public KeyCode		keyDown		= KeyCode.S;
	public KeyCode		keyLeft 	= KeyCode.A;
	public KeyCode		keyRight	= KeyCode.D;

	public bool			dynamicMode;
	public int 			dynamicRegionPrio;				///< Empty dynamic region's hit-test priority.
	public bool			dynamicClamp;					///< Clamp entire stick inside the screen
	public float		dynamicMaxRelativeSize = 0.2f;	///< Maximal allowed size of shorter screen dimension (0.001 - 1.0) 
	public float		dynamicMarginCm 		= 0.5f;	///< Used only when dynamicClamp is enabled
	public float		dynamicFadeOutDelay 	= 0;
	public float		dynamicFadeOutDuration 	= 2.0f;	
	public Rect			dynamicRegion			= new Rect(0,0, 0.5f, 1.0f);	///< dynamic mode screen work-rectangle (normalized coords) 
	private Rect		dynamicRegionPx			= new Rect(0,0, 1, 1);
	public bool			dynamicVisible			{ get { return (this.animAlpha.cur > 0.01f); } }
	public bool			dynamicAlwaysReset		= false;
	

	private bool		dynamicResetPos;				// internal flag

	//public float		hatScale			= 0.9f;		///< Hat's scale relative to control's screen rect. Any state scaling will be multiplied by this value. 
	//public float		baseScale			= 1.0f;		///< Base's scale relative to control's screen rect. Any state scaling will be multiplied by this value. 
	public float		hatMoveScale		= 0.5f;

	public bool			disableX			= false;	///< Disable stick movement along X axis
	public bool			disableY			= false;	///< Disable stick movement along Y axis

	private Vector2		touchStart;
	private	int			touchId;
	private bool		touchVerified;

	private Vector2		pollPos;	// polled position to be applied.

	private float		angle;		///< Current direction angle
	private Vector2		posRaw;		///< Raw stick position (positive up!)
	private Vector2		dirVec;		///< Normalized direction vector
	private float		tilt;		///< Distance from neutral position (0..1)
						
	private Vector2		displayPosStart,
						displayPos;

	private StickDir	dir8way,		///< 8-way digital joystick
						dir4way,		///< 4-way digital joystick
						dir8wayPrev,
						dir4wayPrev;
	private StickDir	dir8wayLastNonNeutral,
						dir4wayLastNonNeutral; 

	private bool		pressedCur,
						pressedPrev;

	public Texture2D	releasedHatImg;		///< Released stick hat's image 
	public Texture2D	releasedBaseImg;	///< Released stick base image
	public Texture2D	pressedHatImg;		///< Pressed stick hat's image
	public Texture2D	pressedBaseImg;		///< Pressed stick base image



	public bool			overrideScale		= false;	///< When false, TouchController's global zone scales will be used.	
	public float		releasedHatScale	= 1.0f;		///< Hat's scale when released.
	public float		pressedHatScale		= 1.0f;		///< Hat's scale when pressed.
	public float		disabledHatScale	= 1.0f;		///< Hat's scale when disabled.
	public float		releasedBaseScale	= 1.0f;		///< Base's scale when released.
	public float		pressedBaseScale	= 1.0f;		///< Base's scale when pressed.
	public float		disabledBaseScale	= 1.0f;		///< Base's scale when disabled.

	public bool			overrideColors		= false;		
	public Color		releasedHatColor;
	public Color		releasedBaseColor;
	public Color		pressedHatColor;
	public Color		pressedBaseColor;
	public Color		disabledHatColor;
	public Color		disabledBaseColor;
	
	private bool		touchCanceled;

	
	// Unity.Input emulation params...

	public bool			enableGetKey;
	public KeyCode		getKeyCodePress,	getKeyCodePressAlt;
	public KeyCode		getKeyCodeUp, 		getKeyCodeUpAlt;
	public KeyCode		getKeyCodeDown, 	getKeyCodeDownAlt;
	public KeyCode		getKeyCodeLeft,		getKeyCodeLeftAlt;
	public KeyCode		getKeyCodeRight,	getKeyCodeRightAlt;

	public bool			enableGetButton;
	public string		getButtonName;

	public bool			enableGetAxis;
	public string		axisHorzName;
	public string		axisVertName;
	public bool			axisHorzFlip;
	public bool			axisVertFlip;
	
	
	// Code generation parameters...

	public bool			codeCustomGUI,			///< Add custom GUI section for this stick.	
						codeCustomLayout;		///< Add custom layout section for this stick.	

	
// ---------------
/// \endcond
// ---------------

	// ---------------
	/// Stick position mode
	// -----------------

	public enum StickPosMode
		{
		FULL_ANALOG,		///< Full analog
		ANALOG_8WAY,		///< Angle rounded to 45 degrees, analog
		ANALOG_4WAY,		///< Angle rounded to 90 degrees, analog 
		DIGITAL_8WAY,		///< Angle rounded to 45 degrees, on/off mode
		DIGITAL_4WAY		///< Angle rounded to 90 degrees, on/off mode
		}
		


// -----------------
/// 8-way stick direction.
// ----------------

public enum StickDir
	{
	NEUTRAL,	///< Neutral state
	U,			///< Up
	UR,			///< Up-Right
	R,			///< Right
	DR,			///< Down-Right
	D,			///< Down
	DL,			///< Down-Left
	L,			///< Left
	UL			///< Up-Left
	}

private const StickDir
	StickDirFirst 	= StickDir.U,			
	StickDirLast 	= StickDir.UL;	
	


// ----------------
/// \brief %Stick's vector query mode.
///
/// Used by GetStickDir3D() to control returned vector's composition.
// ----------------

public enum Vec3DMode
	{
	XZ,			///< Stick's vertical axis will be placed in Z component (Up - positive Z), stick's horizontal axis goes into X component (Right - positive X) 
	XY			///< Returned vector's X will correspond to stick's horizontal Stick's vertical axis = goes into Y.
	}	

	
	// ---------------------------
	/// \name State Query Methods
	/// \{
	// ---------------------------

	// ---------------
	/// Return true if stick is pressed.
	// ---------------
	public bool Pressed()
		{
#		if DEBUG_KEYBOARD_CONTROL

		if (this.keyboardEmu)
			{
			if (Input.GetKey(this.keyLeft) || 
				Input.GetKey(this.keyRight) ||
				Input.GetKey(this.keyDown)	||
				Input.GetKey(this.keyUp) )
				return true;
			}
#		endif

		return (this.pressedCur); //this.touchId >= 0);
		}			
	

	// ---------------
	/// Return true if stick have just been pressed.
	// ---------------
	public bool JustPressed()
		{
		return (this.pressedCur && !this.pressedPrev);
		}


	// ----------------
	/// Return true if stick have just been released. 
	// ----------------
	public bool JustReleased()
		{	
		return (!this.pressedCur && this.pressedPrev);
		}
	
	// ---------------
	/// Get stick's tilt - hat's distance from the central position (0-1). 
	// ---------------
	public float GetTilt()	
		{
		return this.tilt;
		}
	
	
	// ---------------
	///	\brief 
	/// Get stick's tilt angle.
	/// \note Up direction is angle zero and it goes clockwise.
	// --------------
	public float GetAngle()
		{
		return this.angle;
		}
			
	// ---------------
	/// Get stick's current vector (unnormalized). 
	// ---------------
	public Vector2 GetVec(
		//bool normalized = true		///< Return normalized vector. 
		)
		{
		return this.posRaw; //(normalized ? this.dirVec : this.posRaw);
		}
		
	// ---------------
	/// Get stick's current normalized direction vector. 
	// ---------------
	public Vector2 GetNormalizedVec(
		//bool normalized = true		///< Return normalized vector. 
		)
		{
		return this.dirVec;
		}


	
	// -----------------------
	/// Get stick's current vector in specified mode. 
	// -----------------------
	public Vector2 GetVecEx(
		StickPosMode vis	///< Position query mode.
		)
		{
		float angle = this.angle;
		float tilt 	= this.tilt;

		switch (vis)
			{		
			case StickPosMode.FULL_ANALOG :
				return this.posRaw;
	
			case StickPosMode.ANALOG_8WAY :
				angle = GetDirCodeAngle(this.dir8way);
				tilt = ((this.dir8way != StickDir.NEUTRAL) ? tilt : 0);
				break;

			case StickPosMode.ANALOG_4WAY :
				angle = GetDirCodeAngle(this.dir4way);
				tilt = ((this.dir4way != StickDir.NEUTRAL) ? tilt : 0);
				break;

			case StickPosMode.DIGITAL_8WAY :
				angle = GetDirCodeAngle(this.dir8way);
				tilt = ((this.dir8way != StickDir.NEUTRAL) ? 1 : 0);
				break;

			case StickPosMode.DIGITAL_4WAY :
				angle = GetDirCodeAngle(this.dir4way);
				tilt = ((this.dir4way != StickDir.NEUTRAL) ? 1 : 0);
				break;
			}
				
		Vector2 pos = (RotateVec2(new Vector2(0, 1), angle) * tilt);
		//pos.y = -pos.y;
		return pos;
		}
			

	// -----------------
	/// Get stick's vector as a 3d vector (X, 0, Y).
	// -----------------
	public Vector3 GetVec3d(
		bool		normalized,				///< Normalize result?
		float 		orientByAngle /* = 0 */	///< Optional rotation to apply before conversion to 3d.
		)
		{
		Vector2 v = (normalized ? this.dirVec : this.posRaw); //this.GetVecEx(StickPosMode.FULL_ANALOG);
	
		if (orientByAngle != 0)
			v = RotateVec2(v, orientByAngle);
			
		return new Vector3(v.x, 0, v.y);
		}
	
	// -----------------
	/// Get stick's vector as a 3d vector.
	// -----------------
	public Vector3 GetVec3d(
		Vec3DMode 	vecMode,				///< 3d component mapping mode 
		bool		normalized,				///< Normalize result?
		float 		orientByAngle /* = 0 */	///< Optional rotation to apply before conversion to 3d.
		)
		{
		Vector2 v = (normalized ? this.dirVec : this.posRaw); //this.GetVecEx(StickPosMode.FULL_ANALOG);
	
		if (orientByAngle != 0)
			v = RotateVec2(v, orientByAngle);
			
		switch (vecMode)
			{
			case TouchStick.Vec3DMode.XY : 
				return new Vector3(v.x, v.y, 0);
			case TouchStick.Vec3DMode.XZ :
				return new Vector3(v.x, 0, v.y);
			}
	
		return Vector3.zero;
		}
	// -----------------
	/// Shortcut for GetVec3d(vecMode, normalized, 0)
	// -----------------
	public Vector3 GetVec3d(
		Vec3DMode 	vecMode,			///< 3d component mapping mode 
		bool		normalized			///< Normalize result?
		)
		{
		return this.GetVec3d(vecMode, normalized, 0);
		}
		

	/// \}




	// ---------------------------
	/// \name Digital State Query Methods
	/// \{
	// ---------------------------
	

	


	// -------------	
	/// Get current digital direction. 
	// -------------
	public StickDir GetDigitalDir(
		bool	eightWayMode /* = true */		///< 8-way/4-way mode.
		)
		{
		return (eightWayMode ? this.dir8way : this.dir4way);
		}
	// -------------	
	/// Get current 8-way direction.
	// -------------
	public StickDir GetDigitalDir()
		{
		return this.dir8way;
		}
	
	// -------------	
	/// Get current 4-way digital direction. 
	// -------------
	public StickDir GetFourWayDir()
		{
		return (this.dir4way);
		}

	

	// -------------	
	/// Get previous frame's digital direction. 
	// -------------
	public StickDir GetPrevDigitalDir(
		bool	eightWayMode = true		///< 8-way/4-way mode.
		)
		{
		return (eightWayMode ? this.dir8wayPrev : this.dir4wayPrev);
		}
	// -------------	
	/// Get previous frame's 8-way direction. 
	// -------------
	public StickDir GetPrevDigitalDir()
		{
		return this.dir8wayPrev;
		}

	// -------------	
	/// Get previous frame's 4-way direction. 
	// -------------
	public StickDir GetPrevFourWayDir()
		{
		return this.dir4wayPrev;
		}


	// ----------------
	/// Return true if digital direction just changed.
	// ----------------
	public bool DigitalJustChanged(
		bool	eightWayMode /*= true */		///< 8-way/4-way mode.
		)
		{		
		return (eightWayMode ? 
			(this.dir8way != this.dir8wayPrev) :
			(this.dir4way != this.dir4wayPrev));
		}

	// -------------	
	/// Return true if 8-way digital direction just changed.
	// -------------
	public bool DigitalJustChanged()
		{
		return (this.dir8way != this.dir8wayPrev);
		}
	
	// -------------	
	/// Return true if 4-way digital direction just changed.
	// -------------
	public bool FourWayJustChanged()
		{
		return (this.dir4way != this.dir4wayPrev);
		}
	

	



	// ----------------
	/// Check if KeyCode is part of given StickDir.	
	// ----------------
	static private bool KeyCodeInDir(KeyCode keyCode, StickDir dir)
		{
		if (dir == StickDir.NEUTRAL)
			return false;

		switch (keyCode)
			{
			case KeyCode.W :
			case KeyCode.UpArrow :
				return (
					(dir == StickDir.U) ||	
					(dir == StickDir.UL) || 
					(dir == StickDir.UR));

			case KeyCode.S :
			case KeyCode.DownArrow :
				return (
					(dir == StickDir.D) ||	
					(dir == StickDir.DL) || 
					(dir == StickDir.DR));

			case KeyCode.A :
			case KeyCode.LeftArrow :
				return (
					(dir == StickDir.L) ||	
					(dir == StickDir.DL) || 
					(dir == StickDir.UL));
	 
			case KeyCode.D :
			case KeyCode.RightArrow :
				return (
					(dir == StickDir.R) ||	
					(dir == StickDir.DR) || 
					(dir == StickDir.UR));
			}

		return false;
		}

	/// \}


	/// \cond

	// ---------------------------
	/// \name Unity Input-style Methods
	/// \{
	// ---------------------------

	// --------------
	/// Simulates Input.GetAxis() 
	// --------------
	public float GetAxis(string name)
		{
		bool supportedAxis = false;
		return this.GetAxisEx(name, out supportedAxis);
/*
		if (!this.enableGetAxis)
			return 0;
		
		if (name == this.axisHorzName)
			return (this.axisHorzFlip ? -this.posRaw.x : this.posRaw.x); 
		else if (name == this.axisVertName)
			return (this.axisVertFlip ? -this.posRaw.y : this.posRaw.y); 

		return 0;
*/
		}
	
	// --------------
	public float GetAxisEx(string name, out bool supported)
		{
		if (!this.enableGetAxis)
			{
			supported = false;
			return 0;
			}
		
		if (name == this.axisHorzName)
			{
			supported = true;
			return (this.axisHorzFlip ? -this.displayPos.x : this.displayPos.x); 
			}
		else if (name == this.axisVertName)
			{
			supported = true;
			return (this.axisVertFlip ? -this.displayPos.y : this.displayPos.y); 
			}
		
		supported = false;
		return 0;
		}
	
	

	// ---------------
	/// \brief Simulates Unity's Input.GetButton() by returning true when stick is pressed.
	// ---------------
	public bool GetButton(string buttonName)
		{
		bool buttonSupported = false;
		return this.GetButtonEx(buttonName, out buttonSupported);
		}	

	// ---------------
	/// \brief Simulates Unity's Input.GetButtonDown() by returning true when stick has just pressed.
	// ---------------
	public bool GetButtonDown(string buttonName)
		{
		bool buttonSupported = false;
		return this.GetButtonDownEx(buttonName, out buttonSupported);
		}	

	// ---------------
	/// \brief Simulates Unity's Input.GetButtonUp() by returning true when stick has just released.
	// ---------------
	public bool GetButtonUp(string buttonName)
		{
		bool buttonSupported = false;
		return this.GetButtonUpEx(buttonName, out buttonSupported);
		}	



	// ---------------	
	public bool GetButtonEx(string buttonName, out bool buttonSupported)
		{
		if (buttonSupported = 
			(this.enableGetButton && (buttonName == this.getButtonName)))
			{
			return this.Pressed();
			}

		return false;
		}

	// ---------------	
	public bool GetButtonDownEx(string buttonName, out bool buttonSupported)
		{
		if (buttonSupported = 
			(this.enableGetButton && (buttonName == this.getButtonName)))
			{
			return this.JustPressed();
			}

		return false;
		}

	// ---------------	
	public bool GetButtonUpEx(string buttonName, out bool buttonSupported)
		{
		if (buttonSupported = 
			(this.enableGetButton && (buttonName == this.getButtonName)))
			{
			return this.JustReleased();
			}

		return false;
		}
	



	// ---------------
	/// \brief 
	/// Simulates Unity's Input.GetKey() using stick's digital 8-way state.
	// ----------------
	public bool GetKey(KeyCode key)
		{
		if (!this.enableGetKey || (key == KeyCode.None))
			return false;

		if (((key == this.getKeyCodePress) || (key == this.getKeyCodePressAlt)) &&
			this.Pressed()) 
			{
			return true;
			}
		
		return ((this.dir8way != StickDir.NEUTRAL) && 
			this.CheckKeyCode(key, this.dir8way));

		}





	// ---------------
	/// \brief 
	/// Simulates Unity's Input.GetKey() using stick's digital 8-way state.
	/// 
	/// Accepted KeyCodes:
	/// <ul>
	/// 	<li>KeyCode.W (up)</li>
	/// 	<li>KeyCode.S (down)</li>
	/// 	<li>KeyCode.A (left)</li>
	/// 	<li>KeyCode.D (right)</li>
	/// 	<li>KeyCode.UpArrow</li>
	/// 	<li>KeyCode.DownArrow</li>
	/// 	<li>KeyCode.LeftArrow</li>
	/// 	<li>KeyCode.RightArrow</li>
	/// </ul> 
	// ---------------
//	public bool GetKey(KeyCode key)
//		{
//		return KeyCodeInDir(key, this.dir8way);
//
//		}


	// --------------------
	/// \brief
	/// Simulates Unity's Input.GetKeyDown() using stick's digital 8-way state.
	/// 
	/// See \ref GetKey() for details.
	// ----------------------
	public bool GetKeyDown(KeyCode key)
		{
		if (!this.enableGetKey || (key == KeyCode.None))
			{
			return false;
			}

		if (((key == this.getKeyCodePress) || (key == this.getKeyCodePressAlt)) &&
			this.JustPressed())
			return true;

		//return (
		//	 KeyCodeInDir(key, this.dir8way) && 
		//	!KeyCodeInDir(key, this.dir8wayPrev));

		return ((this.dir8way != this.dir8wayPrev) &&
			 this.CheckKeyCode(key, this.dir8way) && 
			!this.CheckKeyCode(key, this.dir8wayPrev));
		}
	


	// --------------------
	/// \brief
	/// Simulates Unity's Input.GetKeyUp() using stick's digital 8-way state.
	/// 
	/// See \ref GetKey() for details.
	// ----------------------
	public bool GetKeyUp(KeyCode key)
		{
		if (!this.enableGetKey || (key == KeyCode.None))
			return false;

		if (((key == this.getKeyCodePress) || (key == this.getKeyCodePressAlt)) &&
			this.JustReleased())
			return true;

		//return (
		//	!KeyCodeInDir(key, this.dir8way) && 
		//	 KeyCodeInDir(key, this.dir8wayPrev));

		return ((this.dir8way != this.dir8wayPrev) &&
			!this.CheckKeyCode(key, this.dir8way) && 
			 this.CheckKeyCode(key, this.dir8wayPrev));
		}
	


	// -----------------
	public bool GetKeyEx(KeyCode key, out bool keySupported)
		{
		keySupported = this.IsKeySupported(key);
		return this.GetKey(key);
		}

	// -----------------
	public bool GetKeyDownEx(KeyCode key, out bool keySupported)
		{
		keySupported = this.IsKeySupported(key);
		return this.GetKeyDown(key);
		}	

	// -----------------
	public bool GetKeyUpEx(KeyCode key, out bool keySupported)
		{
		keySupported = this.IsKeySupported(key);
		return this.GetKeyUp(key);
		}	
	

	// ----------------
	/// Check if given KeyCode is supported by this stick.
	// ----------------
	public bool IsKeySupported(KeyCode key)
		{
		if (!this.enableGetKey)	
			return true;
		return (
			(key == this.getKeyCodePress) 	||
			(key == this.getKeyCodePressAlt)||
			(key == this.getKeyCodeUp) 		||
			(key == this.getKeyCodeUpAlt) 	||
			(key == this.getKeyCodeDown) 	||
			(key == this.getKeyCodeDownAlt) ||
			(key == this.getKeyCodeLeft) 	||
			(key == this.getKeyCodeLeftAlt) ||
			(key == this.getKeyCodeRight) 	||
			(key == this.getKeyCodeRightAlt));
		}

	// ----------------
	/// Check if KeyCode is part of given StickDir.	
	// ----------------
	private bool CheckKeyCode(KeyCode key, StickDir dir)
		{
		if (dir == StickDir.NEUTRAL)
			return false;
		
		if ((key == this.getKeyCodeUp) || 
			(key == this.getKeyCodeUpAlt))
			return (
				(dir == StickDir.U) ||	
				(dir == StickDir.UL) || 
				(dir == StickDir.UR));
		
		else if ((key == this.getKeyCodeDown) || 
			(key == this.getKeyCodeDownAlt))
			return (
				(dir == StickDir.D) ||	
				(dir == StickDir.DL) || 
				(dir == StickDir.DR));
		
		else if ((key == this.getKeyCodeLeft) || 
			(key == this.getKeyCodeLeftAlt))
			return (
				(dir == StickDir.L) ||	
				(dir == StickDir.DL) || 
				(dir == StickDir.UL));
	 
		else if ((key == this.getKeyCodeRight) || 
			(key == this.getKeyCodeRightAlt))
			return (
				(dir == StickDir.R) ||	
				(dir == StickDir.DR) || 
				(dir == StickDir.UR));

		return false;
		}

	/// \}

	/// \endcond

	

			
	// ---------------------------
	/// \name General Methods
	/// \{
	// ---------------------------

	// ---------------
	/// Switch stick into or out of dynamic mode.
	// ---------------
	public void SetDynamicMode(bool dynamicMode)
		{
		if (this.dynamicMode != dynamicMode)
			{
			this.dynamicMode = dynamicMode;
			this.joy.SetLayoutDirtyFlag();
			}
		}
	



	// -----------------
	/// Enable this stick.
	// -----------------
	override public void Enable(
		bool skipAnimation		///< Skip animation.
		)
		{
		//if (this.enabled)
		//	return;
	
		this.enabled = true;

		this.AnimateParams(
			(this.overrideScale ? this.releasedHatScale : this.joy.releasedStickHatScale), 
			(this.overrideScale ? this.releasedBaseScale : this.joy.releasedStickBaseScale), 
			(this.overrideColors ? this.releasedHatColor : this.joy.defaultReleasedStickHatColor), 
			(this.overrideColors ? this.releasedBaseColor : this.joy.defaultReleasedStickBaseColor), 
			(this.dynamicMode ? 0 : 1),	(skipAnimation ? 0 : 
				(this.overrideAnimDuration ? this.enableAnimDuration : this.joy.enableAnimDuration)));

		}

	// -----------------
	/// Disable this stick and release any active touches.
	// -----------------
	override public void Disable(
		bool skipAnimation		///< Skip animation.
		)
		{
		//if (!this.enabled)
		//	return;
	
		this.enabled = false;

		this.ReleaseTouches();
		
		this.AnimateParams(
			(this.overrideScale ? this.disabledHatScale : this.joy.disabledStickHatScale),
			(this.overrideScale ? this.disabledBaseScale : this.joy.disabledStickBaseScale),
			(this.overrideColors ? this.disabledHatColor : this.joy.defaultDisabledStickHatColor), 
			(this.overrideColors ? this.disabledBaseColor : this.joy.defaultDisabledStickBaseColor), 
			(this.dynamicMode ? 0 : 1),	(skipAnimation ? 0 : 
				(this.overrideAnimDuration ? this.disableAnimDuration : this.joy.disableAnimDuration)));			
		}

	

	// ------------------
	/// Show hidden control.
	// ------------------
	override public void Show(
		bool	skipAnim	/* = false */		///< Skip animation.
		)
		{
		//if (this.visible)
		//	return;

		this.visible = true;

		this.AnimateParams(
			(this.overrideScale ? 
				(this.enabled ? this.releasedHatScale : this.disabledHatScale) : 
				(this.enabled ? this.joy.releasedStickHatScale : this.joy.disabledStickHatScale)), 
			(this.overrideScale ? 
				(this.enabled ? this.releasedBaseScale : this.disabledBaseScale) : 
				(this.enabled ? this.joy.releasedStickBaseScale : this.joy.disabledStickBaseScale)), 
 			(this.overrideColors ? 
				(this.enabled ? this.releasedHatColor : this.disabledHatColor) : 
				(this.enabled ? this.joy.defaultReleasedStickHatColor : 
					this.joy.defaultDisabledStickHatColor)), 
			(this.overrideColors ? 
				(this.enabled ? this.releasedBaseColor : this.disabledBaseColor) : 
				(this.enabled ? this.joy.defaultReleasedStickBaseColor : 
					this.joy.defaultDisabledStickBaseColor)),
			(this.dynamicMode ? (this.Pressed() ? 1 : 0) : 1),  
			(skipAnim ? 0 : (this.overrideAnimDuration ? 
				this.showAnimDuration : this.joy.showAnimDuration)));					
		
		}


	// ------------------
	/// Hide this control and release any active touches.
	// ------------------
	override public void Hide(
		bool	skipAnim	/* = false */		///< Skip animation.
		)
		{
		//if (!this.visible)
		//	return;

		this.visible = false;
		
		this.ReleaseTouches();

		Color hiddenHatColor = this.animHatColor.end;
		Color hiddenBaseColor = this.animBaseColor.end;
		//hiddenHatColor.a = 0;
		//hiddenBaseColor.a = 0;

		this.AnimateParams(this.animHatScale.end, this.animBaseScale.end, 
			hiddenHatColor, hiddenBaseColor, 0, 
			(skipAnim ? 0 : (this.overrideAnimDuration ? 
				this.hideAnimDuration : this.joy.hideAnimDuration)));					
		}
	
	
	/// \}


	
	// ---------------------------
	/// \name Custom Layout Methods
	/// \{
	// ---------------------------

	// ---------------
	/// \brief Set custom positioning.
	/// 
	/// \note Keep in mind that this will be reset to automatic rect on next layout change. 
	/// \note For circular zones, shorter dimension on the rectangle will be used as zone's size. 
	// ---------------
	public void SetRect(Rect r)
		{
		Vector2 pos = r.center;
		float	rad = Mathf.Min(r.width, r.height) / 2.0f;
		

		//if (!this.dynamicMode && ((this.posPx != pos) || (this.radPx != rad)))
		if ((this.radPx != rad) || (!this.dynamicMode && (this.posPx != pos)))
			{
			if (!this.dynamicMode)
				this.posPx 	= pos;		

			this.radPx	= rad;

			this.OnReset();
			}		
		}

	
	// ----------------
	/// Reset positioning and size to automatically calculated.
	// ---------------
	override public void ResetRect()
		{ 
		this.radPx = this.layoutRadPx;

		if (!this.dynamicMode)	
			{
			this.posPx = this.layoutPosPx;
			}
		} 
	
	/// \}


	// ---------------------------
	/// \name Custom GUI Helper Methods
	/// \{
	// ---------------------------

	
	// -------------------
	/// Get current pixel rectangle of this zone.
	// ----------------
	public Rect GetRect(
		bool	getAutoRect /* = false */		///< When true, automatically calculated rect will be returned instead of current pixel rectangle. 
		)	
		{
		return TouchController.GetCenRect(
			(getAutoRect ? this.layoutPosPx : this.posPx),
			(getAutoRect ? this.layoutRadPx : this.radPx) * 2.0f);
		}
	// -------------------
	/// Shortcut for GetRect(false)
	// ----------------
	public Rect GetRect()	
		{
		return this.GetRect(false);
		}
	

	// ---------------------
	/// Get control's screen center position.
	// ---------------------
	public Vector2 GetScreenPos()	
		{
		return this.posPx;
		}
	
	// ---------------------
	/// Get control's current radius in pixels. 
	// ---------------------
	public float GetScreenRad()
		{
		return this.radPx;
		}


	// --------------
	/// Get hat's display rect.
	// ---------------
	public Rect GetHatDisplayRect(
		bool	applyScale /* = true */	// When true current scale animation will be applied.	
		)
		{
		return TouchController.GetCenRect(this.posPx + 	
			(InternalToScreenPos(this.displayPos) * //this.GetVecEx(this.stickVis)) * 
				this.radPx * this.hatMoveScale), 
			(2*this.radPx) * (applyScale ? this.animHatScale.cur : 1));
		}
	// --------------
	/// Shortcut for GetHatDisplayRect(true)
	// ---------------
	public Rect GetHatDisplayRect()
		{
		return this.GetHatDisplayRect(true);
		}



	// --------------
	/// Get base's display rect.
	// ---------------
	public Rect GetBaseDisplayRect(
		bool	applyScale = true	// When true current scale animation will be applied.	
		)
		{
		return TouchController.GetCenRect(this.posPx, this.radPx * 2.0f *  
				(applyScale ? this.animBaseScale.cur : 1.0f));
		}
	// --------------
	/// Shortcut for GetBaseDisplayRect(true)
	// ---------------
	public Rect GetBaseDisplayRect()
		{
		return this.GetBaseDisplayRect(true);
		}
	

	// -----------------
	/// Get hat's current display color.
	// ----------------- 
	public Color GetHatColor()
		{
		return this.animHatColor.cur;
		}

	// -----------------
	/// Get base's current display color.
	// ----------------- 
	public Color GetBaseColor()
		{
		return this.animBaseColor.cur;
		}


	// -----------------
	/// Get GUI depth.
	// -----------------	
	public int GetGUIDepth()
		{
		return (this.joy.guiDepth + this.guiDepth +  
			(this.Pressed() ? this.joy.guiPressedOfs : 0));
		}

		
	// -----------------
	/// Get stick base's current display texture.
	// ----------------
	public Texture2D GetBaseDisplayTex()
		{
		return ((this.enabled && this.Pressed()) ? 	
			this.pressedBaseImg : this.releasedBaseImg);
		}

	// -----------------
	/// Get stick hat's current display texture.
	// ----------------
	public Texture2D GetHatDisplayTex()
		{
		return ((this.enabled && this.Pressed()) ? 	
			this.pressedHatImg : this.releasedHatImg);
		}

	/// \}





	// ---------------------------
	/// \name Utils
	/// \{
	// ---------------------------

	// ---------------
	/// Return true if given direction is one of diagonal directions.
	// ---------------
	static public bool IsDiagonalAxis(
		StickDir dir		///< Direction code.
		)
		{
		return ((((int)dir - (int)StickDirFirst) & 1) == 1);
		}


	// -----------------
	/// Return angle in degrees for given direction code.
	// -----------------
	static public float GetDirCodeAngle(
		StickDir d	///< Direction code.
		)
		{		
		if ((d < StickDirFirst) || (d > StickDirLast))
			return 0;
	
		return ((float)((int)d - (int)StickDirFirst) * 45.0f);	
		}

	// ----------------
	/// Get nearest direction for given angle.
	// ----------------
	static public StickDir GetDirCodeFromAngle(
		float 	ang,		///< Angle in degrees 
		bool 	as8way		///< If true nearest of 8-way directions will be returned, otherwise one of major 4-way directions.
		)	
		{
		ang += (as8way ? 22.5f : 45.0f);
		ang = NormalizeAnglePositive(ang);
		
		if (as8way)
			{
			if 		(ang < 45)	return StickDir.U;
			else if (ang < 90)	return StickDir.UR;
			else if (ang < 135)	return StickDir.R;
			else if (ang < 180)	return StickDir.DR;
			else if (ang < 225)	return StickDir.D;
			else if (ang < 270)	return StickDir.DL;
			else if (ang < 315)	return StickDir.L;
			else 				return StickDir.UL;
			}
		else
			{
			if 		(ang < 90)	return StickDir.U;
			else if (ang < 180)	return StickDir.R;
			else if (ang < 270)	return StickDir.D;
			else 				return StickDir.L;
			}
		}
			
	/// \}

		
	// --------------------------------
	// Hide documantation from here...
	/// \cond
	// --------------------------------
	
	
	// ------------------------
	// Animtion functions 
	// ------------------------

	// ---------------------
	private void AnimateParams(
		float hatScale, 
		float baseScale, 
		Color hatColor,
		Color baseColor, 
		float alpha, 
		float duration)
		{
		if (duration <= 0)
			{
			this.animTimer.Reset(0);
			this.animTimer.Disable();
		
			this.animHatColor.Reset(hatColor);
			this.animHatScale.Reset(hatScale);
			this.animBaseColor.Reset(baseColor);
			this.animBaseScale.Reset(baseScale);
			this.animAlpha.Reset(alpha);
			
			this.displayPosStart = this.displayPos = 
				(this.Pressed() ? this.GetVecEx(this.stickVis) : Vector2.zero);

			}
		else
			{
			this.animTimer.Start(duration);
			this.animHatScale.MoveTo(hatScale);
			this.animHatColor.MoveTo(hatColor);
			this.animBaseScale.MoveTo(baseScale);
			this.animBaseColor.MoveTo(baseColor);
			this.animAlpha.MoveTo(alpha);
			}
		}





	// ---------------
	override public void Init(TouchController joy)
		{
		base.Init(joy);
		


		this.OnReset();

		if (this.initiallyDisabled) 
			this.Disable(true);
		if (this.initiallyHidden) 
			this.Hide(true);
		}

	// ---------------
	override public void OnReset()
		{
		this.pressedCur 			= false;
		this.pressedPrev 			= false;
		this.touchId 				= -1;
		this.touchVerified			= true;
		this.dir4way 				= StickDir.NEUTRAL;
		this.dir8way 				= StickDir.NEUTRAL;
		this.dir4wayLastNonNeutral 	= StickDir.NEUTRAL;
		this.dir8wayLastNonNeutral 	= StickDir.NEUTRAL;
		this.dir4wayPrev			= StickDir.NEUTRAL;
		this.dir8wayPrev			= StickDir.NEUTRAL;

		this.touchCanceled	= false;
	
		this.SetInternalPos(Vector2.zero);		


		this.tilt			= 0;
		this.dirVec 		= Vector2.zero;	
		this.posRaw			= Vector2.zero;	
		this.displayPos		= Vector2.zero;		
		this.displayPosStart= Vector2.zero;		

#if UNITY_EDITOR
		// Ignore dynamic mode in editor view...

		if (!EditorApplication.isPlayingOrWillChangePlaymode) 
			{
			if (this.dynamicMode)
				{
				this.posPx = this.dynamicRegionPx.center;
				//this.radPx = this.dynam
				}

			this.OnColorChange();

			//this.AnimateParams(
			//	(this.overrideScale ? this.releasedHatScale : this.joy.releasedStickHatScale), 
			//	(this.overrideScale ? this.releasedBaseScale : this.joy.releasedStickBaseScale), 
			//	(this.overrideColors ? this.releasedHatColor : this.joy.defaultReleasedStickHatColor), 
			//	(this.overrideColors ? this.releasedBaseColor : this.joy.defaultReleasedStickBaseColor), 
			//	1, 0);
			
			}	
		else
#endif
		// Hide if dynamic...

		//if (this.dynamicMode)
			{
			this.AnimateParams(
				(this.overrideScale ? this.releasedHatScale : this.joy.releasedStickHatScale), 
				(this.overrideScale ? this.releasedBaseScale : this.joy.releasedStickBaseScale), 
				(this.overrideColors ? this.releasedHatColor : this.joy.defaultReleasedStickHatColor), 
				(this.overrideColors ? this.releasedBaseColor : this.joy.defaultReleasedStickBaseColor), 
				(this.dynamicMode ? 0 : 1), 0);

			if (!this.enabled)
				this.Disable(true);
			if (!this.visible)
				this.Hide(true);
			}	
			//this.StartAlphaAnim(0,0);
		
//#if UNITY_EDITOR
//	if (!EditorApplication.isPlaying)
//		this.SetColorsDirtyFlag();
//#endif		

		}
	


	// --------------------
	static public Vector2 InternalToScreenPos(Vector2 internalStickPos)
		{
		internalStickPos.y = -internalStickPos.y;
		return internalStickPos;
		}


	// -------------------		
	private void SetPollPos(Vector2 pos, bool screenPos)
		{
		if (screenPos)		
			{
			pos = (pos - this.posPx) / this.radPx;
			pos.y = -pos.y;
			//if (this.rotation != 0)
			//	pos = RotateVec2(pos, this.rotation);			
			}
		
		this.pollPos = pos;
		}

		
	// -----------------
	private void SetInternalPos(Vector2 pos)
		{
		this.pollPos = pos;

		if (this.disableX)
			pos.x = 0;
		if (this.disableY)
			pos.y = 0;

		float 	mag 	= Mathf.Clamp01(pos.magnitude);
		
		Vector2 dir 	= this.dirVec;	
		float	angle 	= this.safeAngle;
				
		

		if (mag > 0.01f) //this.joy.stickDeadzone)
			{
			dir = pos.normalized;
			angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg; //MathUtils.GetAngleFromDir2D(dir.x, dir.y);	

			}
		

		if (mag > ((this.dir8way == StickDir.NEUTRAL) ? 
			this.joy.stickDigitalEnterThresh : this.joy.stickDigitalLeaveThresh))
			{
			// Starting from neutral - favour main axes in...
 
			if (this.dir8wayLastNonNeutral == StickDir.NEUTRAL)	
				{
				// 4-way test...
				
				this.dir4way = GetDirCodeFromAngle(angle, false);

				// 8-way test...
		
				//if (mag < this.joy.STICK_FAVOUR_MAIN_AXES_BELOW_POW)
				//	this.dir8way = this.dir4way;
				//else
					this.dir8way = GetDirCodeFromAngle(angle, true);
		
					
				}

			// Changing durection...

			else if (mag > this.joy.stickDigitalEnterThresh)
				{
				// 8-way test...
		
				float baseAngle8way = GetDirCodeAngle(this.dir8wayLastNonNeutral);
				if (Mathf.Abs(Mathf.DeltaAngle(baseAngle8way, angle)) > 
					(22.5f + this.joy.stickMagnetAngleMargin))
					this.dir8way = GetDirCodeFromAngle(angle, true);
				else
					this.dir8way = this.dir8wayLastNonNeutral;
	
				// 4-way...	

				float baseAngle4way = GetDirCodeAngle(this.dir4wayLastNonNeutral);
				if (Mathf.Abs(Mathf.DeltaAngle(baseAngle4way, angle)) > 
					(45.0f + this.joy.stickMagnetAngleMargin))
					this.dir4way = GetDirCodeFromAngle(angle, false);
				else
					this.dir4way = this.dir4wayLastNonNeutral;
				}

	
			}

		else
			{
			this.dir4way = StickDir.NEUTRAL;
			this.dir8way = StickDir.NEUTRAL;
			}

				
		if (this.dir4way != StickDir.NEUTRAL)
			this.dir4wayLastNonNeutral = this.dir4way;
		if (this.dir8way != StickDir.NEUTRAL)
			this.dir8wayLastNonNeutral = this.dir8way;

				
		this.tilt	 		= mag;		
		this.angle 			= angle;
		this.safeAngle		= angle;
		this.posRaw 		= dir * mag;
		this.dirVec 		= dir;
		}





	// -------------
#if UNITY_EDITOR

	[System.NonSerialized]
	private bool colorsDirtyFlag;




	// ------------------
	public void SetColorsDirtyFlag()	
		{
		this.colorsDirtyFlag = true;
		}

	// ------------------------
	private void RefreshEditorColors()
		{
		if (!EditorApplication.isPlayingOrWillChangePlaymode)
			{
			if (this.colorsDirtyFlag)
				this.OnColorChange();
			}
		}

	// ------------------------
	public void OnColorChange()
		{
		this.colorsDirtyFlag = false;

		this.displayPos		= Vector2.zero;		
		this.displayPosStart= Vector2.zero;		

		
		if (this.joy.previewMode == TouchController.PreviewMode.PRESSED)
			{
			this.AnimateParams(
				(this.overrideScale ? this.pressedHatScale : this.joy.pressedStickHatScale), 
				(this.overrideScale ? this.pressedBaseScale : this.joy.pressedStickBaseScale), 
				(this.overrideColors ? this.pressedHatColor : this.joy.defaultPressedStickHatColor), 
				(this.overrideColors ? this.pressedBaseColor : this.joy.defaultPressedStickBaseColor), 
				1, 0);
			}

		else 
			{	
			this.AnimateParams(
				(this.overrideScale ? this.releasedHatScale : this.joy.releasedStickHatScale), 
				(this.overrideScale ? this.releasedBaseScale : this.joy.releasedStickBaseScale), 
				(this.overrideColors ? this.releasedHatColor : this.joy.defaultReleasedStickHatColor), 
				(this.overrideColors ? this.releasedBaseColor : this.joy.defaultReleasedStickBaseColor), 
				1, 0);
			}

		if (this.initiallyDisabled || (this.joy.previewMode == TouchController.PreviewMode.DISABLED))
			this.Disable(true);
		//else
		//	this.Enable(true);

		if (this.initiallyHidden)
			this.Hide(true);
		//else
		//	this.Show(true);
			
		}

#endif


	// ---------------
	override public void OnPrePoll()
		{	
		this.touchVerified = false;
		}

	// -------------
	override public void OnPostPoll()
		{
		if (!this.touchVerified && (this.touchId >= 0))	
			{
//Debug.Log("POST POLL CHECK!");
			this.OnTouchEnd(this.touchId); 
			}

#		if DEBUG_KEYBOARD_CONTROL

		if (this.keyboardEmu)
			{
			Vector2 keyboardVec = new Vector2(
				((Input.GetKey(this.keyLeft) ? -1 : 0) + (Input.GetKey(this.keyRight) ? 1 : 0)),
				((Input.GetKey(this.keyDown) ? -1 : 0) + (Input.GetKey(this.keyUp) ? 1 : 0)));

			if (keyboardVec.sqrMagnitude >= 0.00001f)
				{
				this.SetPollPos(keyboardVec, false);
				}

			}
		
#		endif

		}
	
	// ----------------
	override public void ReleaseTouches()
		{
		if ((this.touchId >= 0))
			this.OnTouchEnd(this.touchId, true);
		}
	
	
	// ---------------
	override public void TakeoverTouches(TouchableControl controlToUntouch)
		{
		if (controlToUntouch != null)
			{
			if (this.touchId >= 0)
				controlToUntouch.OnTouchEnd(this.touchId, true);
			}
		}

	

	// ---------------
	override public void OnUpdate() //bool firstUpdate)
		{
#if UNITY_EDITOR
		this.RefreshEditorColors();
#endif

		this.dir8wayPrev	= this.dir8way;
		this.dir4wayPrev	= this.dir4way;

		this.pressedPrev 	= this.pressedCur;	
		this.pressedCur 	= (this.touchId >= 0);


		this.SetInternalPos(this.pollPos);
		
		// Update display pos...

		if (this.pressedCur)
			{
			this.displayPos = this.displayPosStart = this.GetVecEx(this.stickVis);
			}					
		else if (!this.smoothReturn)
			{
			this.displayPos = this.displayPosStart = Vector2.zero;
			}


		// Update color animation...

		if ((this.pressedCur != this.pressedPrev) && this.enabled)
			{	
			if (this.pressedCur)
				{
				this.dynamicFadeOutAnimPending = false;

				this.AnimateParams(
				(this.overrideScale ? this.pressedHatScale : this.joy.pressedStickHatScale), 
				(this.overrideScale ? this.pressedBaseScale : this.joy.pressedStickBaseScale), 
					(this.overrideColors ? this.pressedHatColor : this.joy.defaultPressedStickHatColor), 
					(this.overrideColors ? this.pressedBaseColor : this.joy.defaultPressedStickBaseColor), 
					1.0f,
					(this.overrideAnimDuration ? this.pressAnimDuration : this.joy.pressAnimDuration));
				}
			else
				{
				this.dynamicFadeOutAnimPending = (this.dynamicMode && !this.touchCanceled);


				this.AnimateParams(
					(this.overrideScale ? this.releasedHatScale : this.joy.releasedStickHatScale), 
					(this.overrideScale ? this.releasedBaseScale : this.joy.releasedStickBaseScale), 
					(this.overrideColors ? this.releasedHatColor : this.joy.defaultReleasedStickHatColor), 
					(this.overrideColors ? this.releasedBaseColor : this.joy.defaultReleasedStickBaseColor), 

					//this.releasedHatColor, this.releasedBaseColor,
					(this.dynamicMode ? (this.touchCanceled ? 0 : this.animAlpha.cur) : 1.0f), //(this.dynamicMode ? 0 : 1),
					( //(this.dynamicMode ? this.dynamicFadeOutDuration : 
					this.touchCanceled ? this.joy.cancelAnimDuration :
					(this.overrideAnimDuration ? this.releaseAnimDuration : this.joy.releaseAnimDuration)));
				}
			}

		// Update animation...

		if (this.animTimer.Enabled)
			{

			this.animTimer.Update(this.joy.deltaTime);

			float t = TouchController.SlowDownEase(this.animTimer.Nt);
			
			this.animAlpha.Update(t);
			this.animHatColor.Update(t);
			this.animHatScale.Update(t);
			this.animBaseColor.Update(t);
			this.animBaseScale.Update(t);
			
			if (this.smoothReturn && !this.Pressed())
				{
				this.displayPos = Vector2.Lerp(this.displayPosStart, Vector2.zero, t);
				}

			if (this.animTimer.Completed)
				{	
				// Reset display pos...

				this.displayPosStart = this.displayPos;

				if (this.dynamicMode && this.dynamicFadeOutAnimPending)
					{
					// Start dynamic-mode fade-out...

					this.dynamicFadeOutAnimPending = false;
					this.AnimateParams(
						(this.overrideScale ? this.releasedHatScale : this.joy.releasedStickHatScale), 
						(this.overrideScale ? this.releasedBaseScale : this.joy.releasedStickBaseScale), 
						(this.overrideColors ? this.releasedHatColor : this.joy.defaultReleasedStickHatColor), 
						(this.overrideColors ? this.releasedBaseColor : this.joy.defaultReleasedStickBaseColor), 
						//this.releasedHatColor,this.releasedBaseColor, 
						0, this.dynamicFadeOutDuration);
					}
				else
					this.animTimer.Disable();
				}				
			}


		}

	// ---------------
	override public TouchController.HitTestResult HitTest(Vector2 pos, int touchId)
		{
		if ((this.touchId >= 0) || (!this.enabled || !this.visible))
			return new TouchController.HitTestResult(false); //TouchController.HIT_TEST_OUT_OF_RANGE;
			
		TouchController.HitTestResult hitResult;

		if (this.dynamicMode)
			{
			//float d;
			if (!this.dynamicAlwaysReset && this.dynamicVisible &&
				(hitResult = this.joy.HitTestCircle(this.posPx, this.radPx, pos, true)).hit)
				{ // != TouchController.HIT_TEST_OUT_OF_RANGE))
				hitResult.prio = this.prio;
				this.dynamicResetPos = false;
				return hitResult; //d;
 				}
			//if (!this.dynamicVisible)
			//if (this.alphaCur <= 0.001f)	

			this.dynamicResetPos = true;
			hitResult = this.joy.HitTestRect(this.dynamicRegionPx, pos, true);

			hitResult.prio		= this.dynamicRegionPrio;
			hitResult.distScale = this.hitDistScale;

			return hitResult;	
			}
	
		hitResult	= this.joy.HitTestCircle(this.posPx, this.radPx, pos, true);

		hitResult.prio 		= this.prio;
		hitResult.distScale = this.hitDistScale;
		
		return hitResult;
		}
			


	// ---------------
	override public TouchController.EventResult OnTouchStart(
		int touchId, Vector2 touchPos)
		{
		
		// Position dynamic stick on the screen...

		if (this.dynamicMode && this.dynamicResetPos)
			{
			float shorterScreenDim = 
				Mathf.Min(this.joy.GetScreenHeight(), this.joy.GetScreenWidth());

			if (this.dynamicClamp)
				{
				float marginPx = this.radPx + (this.dynamicMarginCm * this.joy.GetDPCM());
				float maxMarginPx = (shorterScreenDim - (this.radPx * 2)) / 2.0f;
				
				marginPx = ((maxMarginPx <= 0) ? 0 : Mathf.Clamp(marginPx, 0, maxMarginPx));
				

				this.posPx.x = Mathf.Clamp(touchPos.x, 
					this.joy.GetScreenX(0) + marginPx, 
					this.joy.GetScreenX(1) - marginPx); 
				this.posPx.y = Mathf.Clamp(touchPos.y, 
					this.joy.GetScreenY(0) + marginPx, 
					this.joy.GetScreenY(1) - marginPx); 
				}
			else
				{
				this.posPx = touchPos;
				}
			
			// Zero the alpha when resetting center-position..

			//this.StartAlphaAnim(0, 0);

			}
		

		// Fade in...

		//this.StartAlphaAnim(1, this.dynamicFadeInDuration);

		this.touchCanceled	= false;

		
		this.touchId 		= touchId;	
		this.touchVerified 	= true;

		//this.SetInternalPos(touchPos, true);
		this.SetPollPos(touchPos, true);	
	

		return TouchController.EventResult.HANDLED;
		}
	
	
	// ----------------
	override public TouchController.EventResult OnTouchEnd(int touchId, bool cancelMode = false) //, Vector2 touchPos)
		{
		if (this.touchId != touchId)	
			return TouchController.EventResult.NOT_HANDLED;
		
		if (this.dynamicMode)
			{
			}

		this.touchId 		= -1;
		this.touchVerified 	= true;

		this.touchCanceled	= cancelMode;

		//this.SetInternalPos(Vector2.zero, false);
		this.SetPollPos(Vector2.zero, false);
		

		return TouchController.EventResult.HANDLED;
		}
			

	// ----------------
	override public TouchController.EventResult OnTouchMove(
		int touchId, Vector2 touchPos)
		{
		if (this.touchId != touchId)
			return TouchController.EventResult.NOT_HANDLED;
				

		this.touchVerified = true;

		//this.SetInternalPos(touchPos, true);
		this.SetPollPos(touchPos, true);

		return TouchController.EventResult.HANDLED;
		}


	// ---------------
	override public void OnLayoutAddContent()
		{
		//if ((this.layout == null) || (this.layout.shape == TouchController.ControlShape.SCREEN_REGION))
		//	return;
			
//#if UNITY_EDITOR
		// Ignore dynamic mode in editor view...

//		if (EditorApplication.isPlayingOrWillChangePlaymode)	
//#endif
		if (this.dynamicMode)
			return;

		// TODO!!!
		this.joy.layoutBoxes[(int)this.layoutBoxId].AddContent(this.posCm, 
			this.sizeCm); //this.radCm * 2.0f);
		}

 
	// ----------------
	override public void OnLayout()
		{
		
		this.dynamicRegionPx = this.joy.NormalizedRectToPx(this.dynamicRegion);

	
		if (this.dynamicMode)
			{
			this.layoutRadPx	= 
			this.radPx 			= 
				this.CalculateDynamicRad();
			}
		else
			{
			this.layoutPosPx = this.joy.layoutBoxes[(int)this.layoutBoxId].GetScreenPos(this.posCm);
			this.layoutRadPx = this.joy.layoutBoxes[(int)this.layoutBoxId].GetScreenSize(this.sizeCm / 2.0f); 

			this.posPx = this.layoutPosPx;
			this.radPx = this.layoutRadPx;
			}

		// Reset on layout...
		
		

		this.OnReset();
		}


	// ---------------
	override public void DrawGUI()
		{
		if (this.disableGui || ((this.joy.GetAlpha() * this.animAlpha.cur) < 0.001f))
			return;

		GUI.color = Color.white;
				
		bool pressed = this.Pressed(); 

#if UNITY_EDITOR
		if (!EditorApplication.isPlayingOrWillChangePlaymode && (this.joy.previewMode == TouchController.PreviewMode.PRESSED))
			pressed = true;
#endif

		Color 		hatColor	= this.animHatColor.cur; //(this.Pressed() ? this.colorCapPressed 	: this.colorCapReleased);
		Color 		baseColor 	= this.animBaseColor.cur; ////(this.Pressed() ? this.colorBasePressed	: this.colorBaseReleased);
		Texture2D 	hatImg		= (pressed ? this.pressedHatImg	: this.releasedHatImg);		
		Texture2D 	baseImg		= (pressed ? this.pressedBaseImg	: this.releasedBaseImg);		
		

		GUI.depth = this.joy.guiDepth + this.guiDepth + 
			(this.Pressed() ? this.joy.guiPressedOfs : 0);


		if (baseImg != null)
			{		
			GUI.color = TouchController.ScaleAlpha(baseColor, 
				this.joy.GetAlpha() * this.animAlpha.cur); //.alphaCur);

			GUI.DrawTexture(this.GetBaseDisplayRect(true), baseImg);
			}

		if (hatImg != null)
			{	
			GUI.color = TouchController.ScaleAlpha(hatColor, 
				this.joy.GetAlpha() * this.animAlpha.cur); //.alphaCur);

			GUI.DrawTexture(this.GetHatDisplayRect(true), hatImg);
			}
		

		} 

	
	// -------------------
	private float CalculateDynamicRad()
		{
		float shorterScreenDim = 
			Mathf.Min(this.joy.GetScreenHeight(), this.joy.GetScreenWidth());

		return Mathf.Max(4, 0.5f * Mathf.Min(
			(this.sizeCm * this.joy.GetDPCM()), 
			shorterScreenDim * Mathf.Clamp(this.dynamicMaxRelativeSize, 0.01f, 1.0f)));
		}


	

	// ------------------
	static private Vector2 RotateVec2(Vector2 pos, float ang)
		{
		float 	s = Mathf.Sin(-ang * Mathf.Deg2Rad),
				c = Mathf.Cos(-ang * Mathf.Deg2Rad);
		return new Vector2((pos.x * c) - (pos.y * s), (pos.x * s) + (pos.y * c));
		} 
		
	// -------------------------
	// Returns angle between 0 and 360
	// -----------------------	
	static private float NormalizeAnglePositive(float a)
		{
		if (a >= 360.0f) 
			return Mathf.Repeat(a, 360.0f);
		if (a >= 0)
			return a;
		if (a <= -360.0f)
			a = Mathf.Repeat(a, 360.0f);
		return (360.0f + a);
		}
		
		
	

	// -----------------
	// End hidden documantation.
	/// \endcond
	// -----------------

	}
	

//}
// -----------------------------------------------
// Control Freak - The Ultimate Virtual Controller
// Copyright (C) 2013 Dan's Game Tools
// -----------------------------------------------

//#define CF_EXTREME_QUERY_FUNCTIONS

#if UNITY_EDITOR 
using UnityEditor;
#endif

using UnityEngine;
using System.Collections;


/// \nosubgrouping

// ---------------------
/*! 

\brief Touch Zone Class.

Touch Zone is a configurable control able to track a variety of touch gestures:
\li Press
\li Tap and Double Tap (Single- and Multi-Finger)
\li Drag (Single- and Multi-Finger)
\li Pinch
\li Twist 
 
Many of methods listed below come in three versions :
\li Finger-Specific (eg. <i>JustPressed(fingerId, ...)</i>)
\li Unified-Touch (eg. <i>JustUniPressed(...)</i>)
\li Multi-Touch (eg. <i>JustMultiPressed(...)</i>)
 
\section sectTouchZoneMultiTouch Multi-Touch

Multi-Touch is active when two fingers are touching a zone.\n
It begins when second finger touches the zone and ends when any of the two fingers is released.\n
Multi-Touch position is a central point between two touching fingers.

\section sectTouchZoneUnifiedTouch	Unified-Touch

Unified-Touch is active when <b>at least one</b> finger is touching a zone.\n
It begins when the first finger touches the zone and ends when the last of the fingers is released.\n
Unified-Touch position is calculated as follows:
<ul>
<li>If only one finger is touching the zone, it's position is used as Unified-Touch position.</li>
<li>If two fingers are touching the zone, Unified-Touch position is at the central point between fingers (equal to Multi-Touch position).</li>
</ul>
Unified-Touch Drag vector is calculated differently from Single- and Multi-Touch. It will not "jump" when second finger is pressed or released. Please, keep in mind, that Unified-Touch Drag Vector may not always be equal to the subtraction of start position from current position!

Unified-Touch is most useful in situations when you supporting multi-finger gestures and want to implement intuitive dragging.   
 
 
\section sectTouchZoneMidFrames Mid-frame input

Rarely, especially when the framerate drops, a situation may occur 
when the user touches and quickly lifts his finger between frames (released-pressed-released - <b>mid-frame press</b>).
The opposite situation is when used lifts his finger for a split of a second 
(pressed-released-pressed - <b>mid-frame release</b>).\n
To check the order of press and release in such situations use following functions:
<ul>
	<li>JustMidFramePressed()</li>
	<li>JustMidFrameUniPressed()</li>
	<li>JustMidFrameMultiPressed()</li>
	<li>JustMidFrameReleased()</li>
	<li>JustMidFrameUniReleased()</li>
	<li>JustMidFrameMultiReleased()</li>
</ul>
\n
For simplicity, possible number of mid-frame presses or mid-frame releases 
have been limited to one (either mid-frame press or mid-frame release).\n
   

**/
// ---------------------
	
[System.Serializable]
public class TouchZone : TouchableControl 
	{
// --------------
/// \cond DGT_DOCS_SHOW_PUBLIC_VARS
// --------------

	public TouchController.ControlShape	shape;		///< Control's shape.
	public Vector2		posCm;							///< Position in centimeters used for layout.
	public Vector2		sizeCm;							///< Size in centimeters used for layout.
	public Rect			regionRect;						///< Normalized screen rect used when shape set to SCREEN_REGION

	private Vector2	posPx,			// position currently used
					sizePx,

					layoutPosPx,	// position and size calculated by layout system.
					layoutSizePx;

	private Rect	screenRectPx,	// used by SCREEN_REGION shape
					layoutRectPx;

	public bool			enableSecondFinger;			///< Enable two-finger gesture detection.
	public bool			nonExclusiveTouches;		///< Allow touch sharing.
	public bool			strictTwoFingerStart;		///< Two fingers need to start at the same time.
	
	public bool			freezeTwistWhenTooClose;	///< When fingers distance drops below safe distance, twist angle will not be updated.  
	
	public bool			noPinchAfterDrag;			///< Pinching will be ignored after multi-finger drag.
	public bool			noPinchAfterTwist;			///< Pinching will be ignored after twisting.
					
	public bool			noTwistAfterDrag;			///< Twisting will be ignored after multi-finger dragging.
	public bool			noTwistAfterPinch;			///< Twisting will be ignored after pinching.
			
	public bool			noDragAfterPinch;			///< Multi-finger dragging will be ignored after pinching.
	public bool			noDragAfterTwist;			///< Multi-finger dragging will be ignored after twisting.

	public bool			startPinchWhenTwisting;		///< Pinch will be marked as moving, when twist starts. Use this to eliminate possible pinch jump later on.
	public bool			startPinchWhenDragging;		///< Pinch will be marked as moving, when multi-finger drag starts. Use this to eliminate possible pinch jump later on.
						
	public bool			startDragWhenPinching;		///< Multi-finger drag will be marked as moving, when pinch starts. Use this to eliminate possible drag jump later on.
	public bool			startDragWhenTwisting;		///< Multi-finger drag will be marked as moving, when twist starts. Use this to eliminate possible drag jump later on.
			
	public bool			startTwistWhenDragging;		///< Twist will be marked as moving, when multi-finger drag starts. Use this to eliminate possible twist jump later on.
	public bool			startTwistWhenPinching;		///< Twist will be marked as moving, when pinch starts. Use this to eliminate possible twist jump later on.

	public GestureDetectionOrder gestureDetectionOrder;		///< Two-finger gesture detection order. Use this in conjunction with no[X]after[Y] and start[X]When[Y] variables if you want to prioritize certain gesture(s). 

	
	public KeyCode		debugKey;					///< Assign a keycode to control this by keyboard as a simple button (Editor-only)

	public Texture2D	releasedImg;				///< Control's GUI image when released.
	public Texture2D	pressedImg;					///< Control's GUI image when pressed.
	

	public bool			overrideScale;				///< When false, TouchController's global zone scales will be used.	
	public float		releasedScale;				///< Control's scale when released.
	public float		pressedScale;				///< Control's scale when pressed.
	public float		disabledScale;				///< Control's scale when disabled.



	public bool			overrideColors;				///< When false, TouchController's global colors will be used.
	public Color		releasedColor;				///< Control's custom color when released.
	public Color		pressedColor;				///< Control's custom color when pressed.	
	public Color		disabledColor;				///< Control's custom color when disabled.
	

	public bool			overrideAnimDuration;		///< When false, TouchController's global animation duration times will be used.
	public float		pressAnimDuration;			///< Control's custom press animation duration in seconds.
	public float		releaseAnimDuration;		///< Control's custom release animation duration in seconds.
	public float		disableAnimDuration;		///< Control's custom disable animation duration in seconds.
	public float		enableAnimDuration;			///< Control's custom enable animation duration in seconds.
	public float		showAnimDuration;			///< Control's custom showing animation duration in seconds.
	public float		hideAnimDuration;			///< Control's custom hiding animation duration in seconds.
	
	private AnimTimer					animTimer;
	private TouchController.AnimFloat	animScale;	
	private TouchController.AnimColor	animColor;


	private	Finger	fingerA,
					fingerB;
	

	// Multi-touch variables...

	private bool	multiCur,
					multiPrev,
					multiMoved,
					multiJustMoved,

					multiMidFrameReleased,
					multiMidFramePressed;

	private float	multiStartTime;
	private Vector2	multiPosCur,
					multiPosPrev,
					multiPosStart;
	private float	multiExtremeCurDist;
	private Vector2	multiExtremeCurVec;
#if CF_EXTREME_QUERY_FUNCTIONS
	[System.NonSerialized]
	private Vector2	multiExtremePrevVec;
	[System.NonSerialized]
	private float	multiExtremePrevDist;
#endif	
	private float	multiLastMoveTime;
	private Vector2	multiDragVel;			// current frame velocity in pixels per second



	


	// Multi-touch tap variables...

	private bool	justMultiTapped,
					justMultiDoubleTapped,
					justMultiDelayTapped,
					waitForMultiDelyedTap;
	private float	lastMultiTapTime;
	private bool	nextTapCanBeMultiDoubleTap;
	private Vector2	lastMultiTapPos;

	

	// Twist and Pinch variables...

	[System.NonSerialized]
	private float	twistStartAbs,
					twistCurAbs,	// absolute finger angle
					twistPrevAbs,
					twistCur,		// finger angle relative to start orientation
					twistPrev,
					twistCurRaw,	// actual angle, no matter how dangerously close the fingers are  
					twistStartRaw,
					twistExtremeCur,
#if CF_EXTREME_QUERY_FUNCTIONS
					twistExtremePrev,	// twist most extreme angle since start... 
#endif
					//twistRecentVel,
					twistLastMoveTime,
					twistVel;
	[System.NonSerialized]
	private float 	pinchDistStart,
					pinchCurDist,
					//pinchCurScale,
					pinchPrevDist,
					//pinchPrevScale,
					pinchExtremeCurDist,
#if CF_EXTREME_QUERY_FUNCTIONS
					pinchExtremePrevDist,
#endif
					//pinchRecentDistVel,
					pinchLastMoveTime,
					pinchDistVel;
	
	// Latest ended two finger gesture params...
	
	private bool	endedMultiMoved,
					endedTwistMoved,	
					endedPinchMoved;
	private float	endedMultiStartTime,
					endedMultiEndTime,

					endedPinchDistStart,
					endedPinchDistEnd,
					//endedPinchRecentDistVel,
					endedPinchDistVel,
					endedTwistAngle,
					//endedTwistRecentVel,
					endedTwistVel;

#if CF_EXTREME_QUERY_FUNCTIONS
	[System.NonSerialized]
	private float	endedMultiExtremeDist,
					//endedMultiExtremeX,
					//endedMultiExtremeY,
					endedTwistExtremeAngle,
					endedPinchExtremeDistDelta;
	[System.NonSerialized]
	private Vector2	endedMultiExtremeVec;
#endif


	private Vector2	endedMultiPosStart,
					endedMultiPosEnd,
					endedMultiDragVel;


	private bool	pollMultiInitialState,
					pollMultiReleasedInitial,
					pollMultiTouched,
					pollMultiReleased;
	private Vector2	pollMultiPosEnd,
					pollMultiPosStart,	
					pollMultiPosCur;



	private bool	twistMoved,		// set to true, when each param goes over it's respective static threshold
					twistJustMoved,
					pinchMoved,
					pinchJustMoved;
					

	// Unified-touch variables...

	private bool	uniMoved,		// TODO : is this of any use!?
					uniJustMoved,
					
					uniCur,
					uniPrev;
	private float	uniStartTime;	
	private Vector2	uniPosCur,
					uniPosStart,
					uniTotalDragCur,
					uniTotalDragPrev;
	private float	uniExtremeDragCurDist;	// most extreme drag dist since start 
	private Vector2	uniExtremeDragCurVec;

#if CF_EXTREME_QUERY_FUNCTIONS
	[System.NonSerialized]
	private float	uniExtremeDragPrevDist;
	[System.NonSerialized]
	private Vector2	uniExtremeDragPrevVec;
#endif

	private float	uniLastMoveTime;
	private Vector2	uniDragVel;			// current frame velocity in pixels per second
	

	// Latest ended uni touch params...

	private float	endedUniStartTime,
					endedUniEndTime;

	private Vector2	endedUniPosStart,
					endedUniPosEnd,
					endedUniTotalDrag,
					endedUniDragVel;

	private bool	endedUniMoved;

#if CF_EXTREME_QUERY_FUNCTIONS
	[System.NonSerialized]
	private float	endedUniExtremeDragDist;
	[System.NonSerialized]
	private Vector2	endedUniExtremeDragVec;
#endif

	
	

	// unified touch poll state...
 
	private bool	uniMidFrameReleased,
					uniMidFramePressed;

	private bool	pollUniInitialState,
					pollUniReleasedInitial,
					pollUniTouched,
					pollUniReleased,
					pollUniWaitForDblStart,
					pollUniWaitForDblEnd;
	private Vector2	pollUniPosEnd,
					pollUniPosStart,	
					pollUniPosCur,
					pollUniPosPrev,		// used for delta accum calculation
					pollUniDeltaAccum,
					pollUniDblEndPos,
					pollUniDeltaAccumAtEnd;
	
	private bool	touchCanceled;


	
	private const float MIN_PINCH_DIST_PX	= 2.0f;

	private const float
		PIXEL_POS_EPSILON_SQR		= 0.1f,
		PIXEL_DIST_EPSILON			= 0.1f,
		TWIST_ANGLE_EPSILON			= 0.5f;

	

	// Unity.Input emulation params...

	public bool			enableGetKey;
	public KeyCode		getKeyCode, 		getKeyCodeAlt;
	public KeyCode		getKeyCodeMulti,	getKeyCodeMultiAlt; 

	public bool			enableGetButton;
	public string		getButtonName,
						getButtonMultiName;
	
	public bool			emulateMouse;				///< When touched, screen px position will be used as emulated mouse pos for CGInput.mousePosition
	public bool			mousePosFromFirstFinger;	///< mouse pos will be taken from first finger only, instead of unified touch position.

	public bool			enableGetAxis;
	public string		axisHorzName;
	public string		axisVertName;
	public bool			axisHorzFlip;
	public bool			axisVertFlip;
	public float		axisValScale;

 

	// ----------------
	// Code Generation Options
	// ----------------

	public bool			codeUniJustPressed,		///< Unified-touch 'just pressed' section
						codeUniPressed,			///< Unified-touch 'pressed' section
						codeUniJustReleased,	///< Unified-touch 'just released' section
						codeUniJustDragged,		///< Unified-touch 'just dragged' section 
						codeUniDragged,			///< Unified-touch 'dragged' section 
						codeUniReleasedDrag,	///< Unified-touch 'dragged before release' section 

						codeMultiJustPressed,	///< Multi-touch 'just pressed' section
						codeMultiPressed,		///< Multi-touch 'pressed' section
						codeMultiJustReleased,	///< Multi-touch 'just released' section
						codeMultiJustDragged,	///< Multi-touch 'just dragged' section 
						codeMultiDragged,		///< Multi-touch 'dragged' section
						codeMultiReleasedDrag,	///< Multi-touch 'dragged before release' section 

						codeJustTwisted,		///< Multi-touch 'just twisted' section
						codeTwisted,			///< Multi-touch 'twisted' section	
						codeReleasedTwist,		///< Multi-touch 'twisted' section in multi-touch 'released' section
	
						codeJustPinched,		///< Multi-touch 'just pinched' section
						codePinched,			///< Multi-touch 'pinched' section	
						codeReleasedPinch,		///< Multi-touch 'pinched' section in multi-touch 'released' section
						
						codeSimpleTap,			///< Simple tap event section (See \ref TouchZone.JustTapped())
						codeSingleTap,			///< Single tap event section (See \ref TouchZone.JustSingleTapped()) 
						codeDoubleTap,			///< Double tap event section (See \ref TouchZone.JustDoubleTapped())

						codeSimpleMultiTap,		///< Simple multi-touch tap event section (See \ref TouchZone.JustMultiTapped())
						codeMultiSingleTap,		///< Multi-touch Single tap event section (See \ref TouchZone.JustMultiSingleTapped()) 
						codeMultiDoubleTap,		///< Multi-touch Double tap event section (See \ref TouchZone.JustMultiDoubleTapped())
	
						codeCustomGUI,			///< Add custom GUI section for this zone.	
						codeCustomLayout;		///< Add custom layout section for this zone.	
			
						//codeDragRegionData;		///< Prepare screen region variables for 'Released' sections (See \ref TouchZone.;



	// ---------------------
	/// Gesture Detection Order
	// ---------------------
	public enum GestureDetectionOrder
		{
		TWIST_PINCH_DRAG,
		TWIST_DRAG_PINCH,
		
		PINCH_TWIST_DRAG,
		PINCH_DRAG_TWIST,

		DRAG_TWIST_PINCH,
		DRAG_PINCH_TWIST		
		}

	
	/// \endcond
	


	// ---------------------------
	/// \name Box region IDs 
	/// (See \ref GetBoxPortion()).
	/// \{
	// ---------------------------
	public const int BOX_LEFT 		=	(1 << 0);		///< Left side of the box.
	public const int BOX_CEN		=	(1 << 1);		///< Horizontally central portion of the box.
	public const int BOX_RIGHT 		=	(1 << 2);		///< Right side of the box. 
	public const int BOX_TOP 		=	(1 << 3);		///< Top portion of the box.
	public const int BOX_MID		=	(1 << 4);		///< Vertically central portion of the box.
	public const int BOX_BOTTOM		=	(1 << 5);		///< Bottom portion of the box. 
		
	public const int BOX_TOP_LEFT	= 	(BOX_TOP | BOX_LEFT);	///< Top-left portion on the box.
	public const int BOX_TOP_CEN	= 	(BOX_TOP | BOX_CEN);	///< Top-central portion on the box.
	public const int BOX_TOP_RIGHT	= 	(BOX_TOP | BOX_RIGHT);	///< Top-right portion on the box.

	public const int BOX_MID_LEFT	= 	(BOX_MID | BOX_LEFT);	///< Mid-left portion on the box.
	public const int BOX_MID_CEN	= 	(BOX_MID | BOX_CEN);	///< Mid-central portion on the box.
	public const int BOX_MID_RIGHT	= 	(BOX_MID | BOX_RIGHT);	///< Mid-right portion on the box.

	public const int BOX_BOTTOM_LEFT= 	(BOX_BOTTOM | BOX_LEFT);	///< Bottom-left portion on the box.
	public const int BOX_BOTTOM_CEN	= 	(BOX_BOTTOM | BOX_CEN);		///< Bottom-central portion on the box.
	public const int BOX_BOTTOM_RIGHT= 	(BOX_BOTTOM	| BOX_RIGHT);	///< Bottom-right portion on the box.


	public const int BOX_H_MASK		=	(BOX_LEFT | BOX_CEN | BOX_RIGHT);	///< Horizotnal bitmask.
	public const int BOX_V_MASK		=	(BOX_TOP | BOX_MID | BOX_BOTTOM);	///< Vertical bitmask.


	/// \}



#if CF_EXTREME_QUERY_FUNCTIONS
	// ------------------------
	/// Movement's extreme query mode  
	// -----------------------
	public enum ExtremeMode
		{
		DIST,			///< Get extreme distance since start
		X,				///< Get X axis extreme since start 
		Y				///< Get Y axis extreme since start 
		}
#endif
		
	
	// ---------------
	private Finger GetFinger(int i)
		{
		return ((i == 0) ? this.fingerA : (i == 1) ? this.fingerB : null);
		}
	
		
	
	
	
	
	// ---------------------------
	/// \name Finger Pressed/Released State
	/// \{
	// ---------------------------

	// --------------------
	///	Return true if a finger is touching the screen.
	// -------------------
	public bool Pressed(
		int		fingerId, 								///< Finger ID (0 or 1)
		bool	trueOnMidFramePress		/* = true */,	///< Return true on mid-frame press (released during previous frame, then quickly pressed and released between frames)
		bool	falseOnMidFrameRelease	/*= true */		///< Return false on mid-frame release (pressed during previous frame and then quickly released and pressed again between frames)
		) 	
		{
		Finger finger = ((fingerId == 1) ? this.fingerB : this.fingerA); 
		return finger.Pressed(trueOnMidFramePress, falseOnMidFrameRelease);
		}	
	// ---------------------
	/// Shortcut for Pressed(fingerId, false, false)
	// ---------------------
	public bool Pressed(
		int		fingerId 						///< Finger ID (0 or 1)
		)
		{
		return this.Pressed(fingerId, false, false);
		}
	 	

	
	// --------------------
	///	Return true if any finger is touching the screen.
	// -------------------
	public bool UniPressed(
		bool	trueOnMidFramePress		/* = true */,	///< Return true on mid-frame press (released during previous frame, then quickly pressed and released between frames)
		bool	falseOnMidFrameRelease	/* = true */	///< Return false on mid-frame release (pressed during previous frame and then quickly released and pressed again between frames)
		) 	
		{
		return ((this.uniCur || (trueOnMidFramePress && this.uniMidFramePressed)) && 
			(!falseOnMidFrameRelease || !this.uniMidFrameReleased));
		}	
	// --------------------
	///	Shortcut for UniPressed(false, false)
	// -------------------
	public bool UniPressed() 	
		{
		return this.UniPressed(false, false);
		}
	


	// --------------------
	///	Return true if two fingers are touching the screen.
	// -------------------
	public bool MultiPressed(
		bool	trueOnMidFramePress		/* = true */,	///< Return true on mid-frame press (released during previous frame, then quickly pressed and released between frames)
		bool	falseOnMidFrameRelease	/* = true */	///< Return false on mid-frame release (pressed during previous frame and then quickly released and pressed again between frames)
		) 	
		{
		return ((this.multiCur || (trueOnMidFramePress && this.multiMidFramePressed)) && 
			(!falseOnMidFrameRelease || !this.multiMidFrameReleased));
		}
	// --------------------
	///	Shortcut for MultiPressed(false, false)
	// -------------------
	public bool MultiPressed() 	
		{
		return this.MultiPressed(false, false);
		}


	// --------------------
	///	Return true if a finger just pressed the screen.
	// -------------------
	public bool JustPressed(
		int		fingerId, 								///< Finger ID (0 or 1)
		bool	trueOnMidFramePress		/* = true */,	///< Return true on mid-frame press (released during previous frame, then quickly pressed and released between frames)
		bool	trueOnMidFrameRelease	/* = false */	///< Return true on mid-frame release (pressed during previous frame and then quickly released and pressed again between frames)
		)
		{
		Finger finger = ((fingerId == 1) ? this.fingerB : this.fingerA); 
		return (finger.JustPressed(trueOnMidFramePress, trueOnMidFrameRelease));
		}
	
	// --------------------
	///	Shortcut for JustPressed(fingerId, false, false)
	// -------------------
	public bool JustPressed(
		int		fingerId 						///< Finger ID (0 or 1)
		)
		{
		return this.JustPressed(fingerId, false, false);
		}
	

	// --------------------
	///	Return true if first finger just pressed the screen.
	// -------------------
	public bool JustUniPressed(
		bool	trueOnMidFramePress		/* = true */,	///< Return true on mid-frame press (released during previous frame, then quickly pressed and released between frames)
		bool	trueOnMidFrameRelease	/* = false */	///< Return true on mid-frame release (pressed during previous frame and then quickly released and pressed again between frames)
		)
		{
		return ((!this.uniPrev && this.uniCur) || 
			(trueOnMidFramePress && this.uniMidFramePressed) ||
			(trueOnMidFrameRelease && this.uniMidFrameReleased));
		}

	// --------------------
	///	Shortcut for JustUniPressed(false, false)
	// -------------------
	public bool JustUniPressed()
		{
		return this.JustUniPressed(false, false);
		}
	
	

	// --------------------
	///	Return true if two fingers just pressed the screen.
	// -------------------
	public bool JustMultiPressed(
		bool	trueOnMidFramePress		/* = true */,	///< Return true on mid-frame press (released during previous frame, then quickly pressed and released between frames)
		bool	trueOnMidFrameRelease	/* = false */	///< Return true on mid-frame release (pressed during previous frame and then quickly released and pressed again between frames)
		)
		{
		return ((!this.multiPrev && this.multiCur) || 
			(trueOnMidFramePress && this.multiMidFramePressed) ||
			(trueOnMidFrameRelease && this.multiMidFrameReleased));
		}
	
	// --------------------
	///	Shortcut for JustMultiPressed(false, false)
	// -------------------
	public bool JustMultiPressed()
		{
		return this.JustMultiPressed(false, false);
		}


	
	// --------------------
	///	Return true if a finger just stopped touching the screen.
	// -------------------
	public bool JustReleased(
		int		fingerId, 								///< Finger ID (0 or 1)
		bool	trueOnMidFramePress		/* = true */,	///< Return true on mid-frame press (released during previous frame, then quickly pressed and released between frames)
		bool	trueOnMidFrameRelease	/* = true */	///< Return true on mid-frame release (pressed during previous frame and then quickly released and pressed again between frames)
		)
		{
		Finger finger = ((fingerId == 1) ? this.fingerB : this.fingerA); 
		return (finger.JustPressed(trueOnMidFramePress, trueOnMidFrameRelease));
		}

	// --------------------
	///	Shortcut for JustReleased(fingerId, false, false)
	// -------------------
	public bool JustReleased(
		int		fingerId 						///< Finger ID (0 or 1)
		)
		{
		return this.JustReleased(fingerId, false, false);
		}


	// --------------------
	///	Return true if last remaining finger just stopped touching the screen.
	// -------------------
	public bool JustUniReleased(
		bool	trueOnMidFramePress		/* = true */,	///< Return true on mid-frame press (released during previous frame, then quickly pressed and released between frames)
		bool	trueOnMidFrameRelease	/* = true */	///< Return true on mid-frame release (pressed during previous frame and then quickly released and pressed again between frames)
		)
		{
		return ((this.uniPrev && !this.uniCur) ||
			(trueOnMidFramePress && this.uniMidFramePressed) ||
			(trueOnMidFrameRelease && this.uniMidFrameReleased)); 
		}
	
	// --------------------
	///	Shortcut for JustUniReleased(false, false)
	// -------------------
	public bool JustUniReleased()
		{
		return this.JustUniReleased(false, false);
		}


	// --------------------
	///	Return true if at least one of two fingers just stopped touching the screen.
	// -------------------
	public bool JustMultiReleased(
		bool	trueOnMidFramePress		/* = true */,	///< Return true on mid-frame press (released during previous frame, then quickly pressed and released between frames)
		bool	trueOnMidFrameRelease	/* = true */	///< Return true on mid-frame release (pressed during previous frame and then quickly released and pressed again between frames)
		)
		{
		return ((this.multiPrev && !this.multiCur) ||
			(trueOnMidFramePress && this.multiMidFramePressed) ||
			(trueOnMidFrameRelease && this.multiMidFrameReleased)); 
		}
	
	// --------------------
	///	Shortcut for JustMultiReleased(false, false)
	// -------------------
	public bool JustMultiReleased()
		{
		return this.JustMultiReleased(false, false);
		}

	
	// -----------------
	///	\brief 
	/// Return true if a finger was released during last frame and 
	/// is released at current frame, 
	/// but it was pressed for a split of a second between frames.  
	// -----------------
	public bool JustMidFramePressed(
		int 	fingerId		///< Finger ID (0 or 1)
		)
		{
		Finger finger = ((fingerId == 1) ? this.fingerB : this.fingerA); 
		return (finger.midFramePressed);
		}
	
	// -----------------
	///	\brief 
	///	Return true if a finger was pressed during last frame and 
	/// is pressed at current frame, 
	/// but it was released for a split of a second between frames.  
	// -----------------
	public bool JustMidFrameReleased(
		int 	fingerId		///< Finger ID (0 or 1)
		)
		{
		Finger finger = ((fingerId == 1) ? this.fingerB : this.fingerA); 
		return (finger.midFrameReleased);
		}


	// -----------------
	///	\brief 
	///	Return true if unified-touch was released during last frame and 
	/// is released at current frame, 
	/// but it was pressed for a split of a second between frames.  
	// -----------------
	public bool JustMidFrameUniPressed()
		{
		return (this.uniMidFramePressed);
		}

	// -----------------
	///	\brief 
	///	Return true if unified-touch was pressed during last frame and 
	/// is pressed at current frame, 
	/// but it was released for a split of a second between frames.  
	// -----------------
	public bool JustMidFrameUniReleased()
		{
		return (this.uniMidFrameReleased);
		}

	
	// -----------------
	///	\brief 
	///	Return true if multi-touch was released during last frame and 
	/// is released at current frame, 
	/// but it was pressed for a split of a second between frames.  
	// -----------------
	public bool JustMidFrameMultiPressed()
		{
		return (this.multiMidFramePressed);
		}

	// -----------------
	///	\brief 
	///	Return true if multi-touch was pressed during last frame and 
	/// is pressed at current frame, 
	/// but it was released for a split of a second between frames.  
	// -----------------
	public bool JustMidFrameMultiReleased()
		{
		return (this.multiMidFrameReleased);
		}



	// ---------------------
	/// Get finger touch duration in seconds.
	// ---------------------
	public float GetTouchDuration(
		int 		fingerId 		///< Finger ID (0 or 1)
		)
		{
		Finger finger = ((fingerId == 1) ? this.fingerB : this.fingerA); 
		return (finger.curState ? (this.joy.curTime - finger.startTime) : 0);
		}
	
	// ---------------------
	/// Get unified-touch duration in seconds.
	// ---------------------
	public float GetUniTouchDuration()
		{
		return (this.uniCur ? (this.joy.curTime - this.uniStartTime) : 0);
		}

	// ---------------------
	/// Get multi-touch duration in seconds.
	// ---------------------
	public float GetMultiTouchDuration()
		{
		return (this.multiCur ? (this.joy.curTime - this.multiStartTime) : 0);
		}
	
	/// \}
	


	// ---------------------------
	/// \name Touch Position
	/// \{
	// ---------------------------


	// --------------------------
	/// Get touch current position
	// --------------------------- 
	public Vector2 GetPos(
		int 						fingerId, 		///< Finger ID (0 or 1)
		TouchCoordSys 	cs /* = TouchCoordSys.SCREEN_PX	*/ ///< Coordinate system
		)
		{		
		Finger finger = ((fingerId == 1) ? this.fingerB : this.fingerA); 
		return this.TransformPos(finger.posCur, cs, false);
		}

	// --------------------------
	/// Shortcut for GetPos(fingerId, TouchCoordSys.SCREEN_PX)
	// --------------------------- 
	public Vector2 GetPos(
		int 			fingerId 		///< Finger ID (0 or 1)
		)
		{		
		return this.GetPos(fingerId, TouchCoordSys.SCREEN_PX);
		}



	// --------------------------
	/// Get unified-touch current position
	// --------------------------- 
	public Vector2 GetUniPos(
		TouchCoordSys 	cs 	/* = TouchCoordSys.SCREEN_PX */	///< Coordinate system
		)
		{		
		return this.TransformPos(this.uniPosCur, cs, false);
		}
	
	// --------------------------
	/// Shortcut for GetUniPos(TouchCoordSys.SCREEN_PX)
	// --------------------------- 
	public Vector2 GetUniPos()
		{		
		return this.GetUniPos(TouchCoordSys.SCREEN_PX);
		}



	// --------------------------
	/// Get multi-touch current position
	// --------------------------- 
	public Vector2 GetMultiPos(
		TouchCoordSys 	cs /* = TouchCoordSys.SCREEN_PX */	///< Coordinate system
		)
		{		
		return this.TransformPos(this.multiPosCur, cs, false);
		}
	
	// --------------------------
	/// Shortcut for GetMultiPos(TouchCoordSys.SCREEN_PX)
	// --------------------------- 
	public Vector2 GetMultiPos()
		{		
		return this.GetMultiPos(TouchCoordSys.SCREEN_PX);
		}


	// --------------------------
	/// Get touch start position
	// --------------------------- 
	public Vector2 GetStartPos(
		int 			fingerId, 							///< Finger ID (0 or 1)
		TouchCoordSys 	cs /* = TouchCoordSys.SCREEN_PX */	///< Coordinate system
		)
		{		
		Finger finger = ((fingerId == 1) ? this.fingerB : this.fingerA); 
		return this.TransformPos(finger.startPos, cs, false);
		}
	
	// --------------------------
	/// Shortcut for GetStartPos(fingerId, TouchCoordSys.SCREEN_PX)
	// --------------------------- 
	public Vector2 GetStartPos(
		int 			fingerId 							///< Finger ID (0 or 1)
		)
		{		
		return this.GetStartPos(fingerId, TouchCoordSys.SCREEN_PX);
		}


	// --------------------------
	/// Get unified-touch start position
	// --------------------------- 
	public Vector2 GetUniStartPos(
		TouchCoordSys 	 cs /* = TouchCoordSys.SCREEN_PX */	///< Coordinate system
		)
		{		
		return this.TransformPos(this.uniPosStart, cs, false);
		}
	
	// --------------------------
	/// Shortcut for GetUniStartPos(TouchCoordSys.SCREEN_PX)
	// --------------------------- 
	public Vector2 GetUniStartPos()
		{		
		return this.GetUniStartPos(TouchCoordSys.SCREEN_PX);
		}


	// --------------------------
	/// Get multi-touch start position
	// --------------------------- 
	public Vector2 GetMultiStartPos(
		TouchCoordSys 	cs /* = TouchCoordSys.SCREEN_PX */	///< Coordinate system
		)
		{		
		return this.TransformPos(this.multiPosStart, cs, false);
		}
	
	// --------------------------
	/// Shortcut for GetMultiStartPos(TouchCoordSys.SCREEN_PX)
	// --------------------------- 
	public Vector2 GetMultiStartPos()
		{		
		return this.GetMultiStartPos(TouchCoordSys.SCREEN_PX);
		}

	/// \}

	

	// ---------------------------
	/// \name Touch Drag State 
	/// \{
	// ---------------------------


	// --------------------------
	/// Get touch total drag vector of specified finger.
	// --------------------------- 
	public Vector2 GetDragVec(
		int 			fingerId, 									///< Finger ID (0 or 1)
		TouchCoordSys 	cs 		/* = TouchCoordSys.SCREEN_PX */,	///< Coordinate system
		bool			raw 	/* = false */						///< When not in raw mode and drag didn't passed the threshold, zero vector will be returned. 
		)
		{		
		Finger finger = ((fingerId == 1) ? this.fingerB : this.fingerA);
		if (!raw && !finger.moved)
			return Vector2.zero;
 
		return this.TransformPos((finger.posCur - finger.startPos), cs, true);
		}
	
	// --------------------------
	/// Shortcut for GetDragVec(fingerId, TouchCoordSys.SCREEN_PX, false)
	// --------------------------- 
	public Vector2 GetDragVec(
		int 			fingerId		///< Finger ID (0 or 1)
		)
		{		
		return this.GetDragVec(fingerId, TouchCoordSys.SCREEN_PX, false);
		}
	// --------------------------
	/// Shortcut for GetDragVec(fingerId, cs, false)
	// --------------------------- 
	public Vector2 GetDragVec(
		int 			fingerId,	///< Finger ID (0 or 1)
		TouchCoordSys 	cs			///< Coordinate system
		)
		{		
		return this.GetDragVec(fingerId, cs, false);
		}
	// --------------------------
	/// Shortcut for GetDragVec(fingerId, TouchCoordSys.SCREEN_PX, raw)
	// --------------------------- 
	public Vector2 GetDragVec(
		int 			fingerId,		///< Finger ID (0 or 1)
		bool			raw 			///< When not in raw mode and drag didn't passed the threshold, zero vector will be returned. 
		)
		{		
		return this.GetDragVec(fingerId, TouchCoordSys.SCREEN_PX, raw);
		}

	
	// --------------------------
	/// Shortcut for GetDragVec(fingerId, TouchCoordSys.SCREEN_PX, true)
	// --------------------------- 
	public Vector2 GetDragVecRaw(
		int 			fingerId		///< Finger ID (0 or 1)
		)
		{		
		return this.GetDragVec(fingerId, TouchCoordSys.SCREEN_PX, true);
		}
	// --------------------------
	/// Shortcut for GetDragVec(fingerId, cs, true)
	// --------------------------- 
	public Vector2 GetDragVecRaw(
		int 			fingerId,	///< Finger ID (0 or 1)
		TouchCoordSys 	cs			///< Coordinate system
		)
		{		
		return this.GetDragVec(fingerId, cs, true);
		}



	// --------------------------
	/// Get unified-touch total drag vector.
	// --------------------------- 
	public Vector2 GetUniDragVec(
		TouchCoordSys 	cs 	/* = TouchCoordSys.SCREEN_PX */,	///< Coordinate system
		bool			raw /* = false */						///< When not in raw mode and drag didn't passed the threshold, zero vector will be returned. 
		)
		{		
		if (!raw && !this.uniMoved)
			return Vector2.zero;
 
		return this.TransformPos(this.uniTotalDragCur, cs, true);
		}
	// --------------------------
	/// Shortcut for GetUniDragVec(TouchCoordSys.SCREEN_PX, false)
	// --------------------------- 
	public Vector2 GetUniDragVec()
		{		
		return this.GetUniDragVec(TouchCoordSys.SCREEN_PX, false);
		}
	// --------------------------
	/// Shortcut for GetUniDragVec(cs, false)
	// --------------------------- 
	public Vector2 GetUniDragVec(
		TouchCoordSys 	cs			///< Coordinate system
		)
		{		
		return this.GetUniDragVec(cs, false);
		}
	// --------------------------
	/// Shortcut for GetUniDragVec(TouchCoordSys.SCREEN_PX, raw)
	// --------------------------- 
	public Vector2 GetUniDragVec(
		bool			raw 			///< When not in raw mode and drag didn't passed the threshold, zero vector will be returned. 
		)
		{		
		return this.GetUniDragVec(TouchCoordSys.SCREEN_PX, raw);
		}

	// --------------------------
	/// Shortcut for GetUniDragVec(TouchCoordSys.SCREEN_PX, true)
	// --------------------------- 
	public Vector2 GetUniDragVecRaw()
		{		
		return this.GetUniDragVec(TouchCoordSys.SCREEN_PX, true);
		}
	// --------------------------
	/// Shortcut for GetUniDragVec(cs, true)
	// --------------------------- 
	public Vector2 GetUniDragVecRaw(
		TouchCoordSys 	cs			///< Coordinate system
		)
		{		
		return this.GetUniDragVec(cs, true);
		}



	// --------------------------- 
	/// Get multi-touch total drag vector.
	// --------------------------- 
	public Vector2 GetMultiDragVec(
		TouchCoordSys 	cs 	/* = TouchCoordSys.SCREEN_PX */,	///< Coordinate system
		bool			raw /* = false */						///< When not in raw mode and drag didn't passed the threshold, zero vector will be returned. 
		)
		{		
		if (!raw && !this.uniMoved)
			return Vector2.zero;
 
		return this.TransformPos((this.multiPosCur - this.multiPosStart), cs, true);
		}
	// --------------------------
	/// Shortcut for GetMultiDragVec(TouchCoordSys.SCREEN_PX, false)
	// --------------------------- 
	public Vector2 GetMultiDragVec()
		{		
		return this.GetMultiDragVec(TouchCoordSys.SCREEN_PX, false);
		}
	// --------------------------
	/// Shortcut for GetMultiDragVec(cs, false)
	// --------------------------- 
	public Vector2 GetMultiDragVec(
		TouchCoordSys 	cs			///< Coordinate system
		)
		{		
		return this.GetMultiDragVec(cs, false);
		}
	// --------------------------
	/// Shortcut for GetMultiDragVec(TouchCoordSys.SCREEN_PX, raw)
	// --------------------------- 
	public Vector2 GetMultiDragVec(
		bool			raw 			///< When not in raw mode and drag didn't passed the threshold, zero vector will be returned. 
		)
		{		
		return this.GetMultiDragVec(TouchCoordSys.SCREEN_PX, raw);
		}
	// --------------------------
	/// Shortcut for GetMultiDragVec(TouchCoordSys.SCREEN_PX, true)
	// --------------------------- 
	public Vector2 GetMultiDragVecRaw()
		{		
		return this.GetMultiDragVec(TouchCoordSys.SCREEN_PX, true);
		}
	// --------------------------
	/// Shortcut for GetMultiDragVec(cs, true)
	// --------------------------- 
	public Vector2 GetMultiDragVecRaw(
		TouchCoordSys 	cs			///< Coordinate system
		)
		{		
		return this.GetMultiDragVec(cs, true);
		}
	

	// --------------------------
	/// Get finger's drag delta vector relative to previous update.
	// --------------------------- 
	public Vector2 GetDragDelta(
		int 			fingerId, 									///< Finger ID (0 or 1)
		TouchCoordSys 	cs 		/* = TouchCoordSys.SCREEN_PX */,	///< Coordinate system
		bool			raw 	/* = false */						///< When not in raw mode and drag didn't passed the threshold, zero vector will be returned. 
		)
		{		
		Finger finger = ((fingerId == 1) ? this.fingerB : this.fingerA);
		if (!raw && !finger.moved)
			return Vector2.zero;
 
		return TransformPos((!raw && finger.justMoved) ? 
			(finger.posCur - finger.startPos) :
			(finger.posCur - finger.posPrev), cs, true);
		}
	
	// --------------------------
	/// Shortcut for GetDragDelta(fingerId, TouchCoordSys.SCREEN_PX, false)
	// --------------------------- 
	public Vector2 GetDragDelta(
		int 			fingerId 							///< Finger ID (0 or 1)
		)
		{
		return this.GetDragDelta(fingerId, TouchCoordSys.SCREEN_PX, false);
		}
	// --------------------------
	/// Shortcut for GetDragDelta(fingerId, cs, false)
	// --------------------------- 
	public Vector2 GetDragDelta(
		int 			fingerId, 		///< Finger ID (0 or 1)
		TouchCoordSys 	cs 				///< Coordinate system
		)
		{
		return this.GetDragDelta(fingerId, cs, false);
		}
	// --------------------------
	/// Shortcut for GetDragDelta(fingerId, TouchCoordSys.SCREEN_PX, raw)
	// --------------------------- 
	public Vector2 GetDragDelta(
		int 			fingerId, 		///< Finger ID (0 or 1)
		bool			raw 			///< When not in raw mode and drag didn't passed the threshold, zero vector will be returned. 
		)
		{
		return this.GetDragDelta(fingerId, TouchCoordSys.SCREEN_PX, raw);
		}
	// --------------------------
	/// Shortcut for GetDragDelta(fingerId, TouchCoordSys.SCREEN_PX, true)
	// --------------------------- 
	public Vector2 GetDragDeltaRaw(
		int 			fingerId 							///< Finger ID (0 or 1)
		)
		{
		return this.GetDragDelta(fingerId, TouchCoordSys.SCREEN_PX, true);
		}
	// --------------------------
	/// Shortcut for GetDragDelta(fingerId, cs, true)
	// --------------------------- 
	public Vector2 GetDragDeltaRaw(
		int 			fingerId, 		///< Finger ID (0 or 1)
		TouchCoordSys 	cs 				///< Coordinate system
		)
		{
		return this.GetDragDelta(fingerId, cs, true);
		}
		



	
	// --------------------------
	/// Get unified-touch drag delta vector relative to previous update.
	// --------------------------- 
	public Vector2 GetUniDragDelta(
		TouchCoordSys 	cs	/* = TouchCoordSys.SCREEN_PX */,	///< Coordinate system
		bool			raw /* = false */		///< When not in raw mode and drag didn't passed the threshold, zero vector will be returned. 
		)
		{		
		if (!raw && !this.uniMoved)
			return Vector2.zero;
 
		return TransformPos((!raw && this.uniJustMoved) ? this.uniTotalDragCur :	
			(this.uniTotalDragCur - this.uniTotalDragPrev), cs, true);
		}
	// --------------------------
	/// Shortcut for GetUniDragDelta(TouchCoordSys.SCREEN_PX, false)
	// --------------------------- 
	public Vector2 GetUniDragDelta()
		{
		return this.GetUniDragDelta(TouchCoordSys.SCREEN_PX, false);
		}
	// --------------------------
	/// Shortcut for GetUniDragDelta(cs, false)
	// --------------------------- 
	public Vector2 GetUniDragDelta(
		TouchCoordSys 	cs 				///< Coordinate system
		)
		{
		return this.GetUniDragDelta(cs, false);
		}
	// --------------------------
	/// Shortcut for GetUniDragDelta(TouchCoordSys.SCREEN_PX, raw)
	// --------------------------- 
	public Vector2 GetUniDragDelta(
		bool			raw 			///< When not in raw mode and drag didn't passed the threshold, zero vector will be returned. 
		)
		{
		return this.GetUniDragDelta(TouchCoordSys.SCREEN_PX, raw);
		}
	// --------------------------
	/// Shortcut for GetUniDragDelta(TouchCoordSys.SCREEN_PX, true)
	// --------------------------- 
	public Vector2 GetUniDragDeltaRaw()
		{
		return this.GetUniDragDelta(TouchCoordSys.SCREEN_PX, true);
		}
	// --------------------------
	/// Shortcut for GetUniDragDelta(cs, true)
	// --------------------------- 
	public Vector2 GetUniDragDeltaRaw(
		TouchCoordSys 	cs 				///< Coordinate system
		)
		{
		return this.GetUniDragDelta(cs, true);
		}
	
	
	// --------------------------
	/// Get multi-touch drag delta vector relative to previous update.
	// --------------------------- 
	public Vector2 GetMultiDragDelta(
		TouchCoordSys 	cs	/* = TouchCoordSys.SCREEN_PX */,	///< Coordinate system
		bool			raw /* = false */						///< When not in raw mode and drag didn't passed the threshold, zero vector will be returned. 
		)
		{		
		if (!raw && !this.multiMoved)
			return Vector2.zero;
 
		return TransformPos((!raw && this.multiJustMoved) ? 
			(this.multiPosCur - this.multiPosStart) : 
			(this.multiPosCur - this.multiPosPrev), cs, true);
		}
	// --------------------------
	/// Shortcut for GetMultiDragDelta(TouchCoordSys.SCREEN_PX, false)
	// --------------------------- 
	public Vector2 GetMultiDragDelta()
		{
		return this.GetMultiDragDelta(TouchCoordSys.SCREEN_PX, false);
		}
	// --------------------------
	/// Shortcut for GetMultiDragDelta(cs, false)
	// --------------------------- 
	public Vector2 GetMultiDragDelta(
		TouchCoordSys 	cs 				///< Coordinate system
		)
		{
		return this.GetMultiDragDelta(cs, false);
		}
	// --------------------------
	/// Shortcut for GetMultiDragDelta(TouchCoordSys.SCREEN_PX, raw)
	// --------------------------- 
	public Vector2 GetMultiDragDelta(
		bool			raw 			///< When not in raw mode and drag didn't passed the threshold, zero vector will be returned. 
		)
		{
		return this.GetMultiDragDelta(TouchCoordSys.SCREEN_PX, raw);
		}
	// --------------------------
	/// Shortcut for GetMultiDragDelta(TouchCoordSys.SCREEN_PX, true)
	// --------------------------- 
	public Vector2 GetMultiDragDeltaRaw()
		{
		return this.GetMultiDragDelta(TouchCoordSys.SCREEN_PX, true);
		}
	// --------------------------
	/// Shortcut for GetMultiDragDelta(cs, true)
	// --------------------------- 
	public Vector2 GetMultiDragDeltaRaw(
		TouchCoordSys 	cs 				///< Coordinate system
		)
		{
		return this.GetMultiDragDelta(cs, true);
		}
	


	// --------------------------
	/// Return true if finger is pressed and moved since start.
	// --------------------------- 
	public bool Dragged(
		int 			fingerId 		///< Finger ID (0 or 1)
		)
		{		
		Finger finger = ((fingerId == 1) ? this.fingerB : this.fingerA); 
		return (finger.curState && finger.moved);
		}

	// --------------------------
	/// Return true if unified-touch is pressed and moved since start.
	// --------------------------- 
	public bool UniDragged()
		{		
		return (this.uniCur && this.uniMoved);
		}


	// --------------------------
	/// Return true if multi-touch is active and moved since start.
	// --------------------------- 
	public bool MultiDragged()
		{		
		return (this.multiCur && this.multiMoved);
		}
	

	// --------------------------
	/// Return true if finger is pressed and just started moving.
	// --------------------------- 
	public bool JustDragged(
		int 		fingerId 		///< Finger ID (0 or 1)
		)
		{		
		Finger finger = ((fingerId == 1) ? this.fingerB : this.fingerA); 
		return (finger.curState && finger.justMoved);
		}


	// --------------------------
	/// Return true if uni-touch just started moving.
	// --------------------------- 
	public bool JustUniDragged()
		{		
		return (this.uniCur && this.uniJustMoved);
		}


	// --------------------------
	/// Return true if multi-touch just started moving.
	// --------------------------- 
	public bool JustMultiDragged()
		{		
		return (this.multiCur && this.multiJustMoved);
		}






	

#if CF_EXTREME_QUERY_FUNCTIONS

	// --------------------------
	/// Return true if finger is pressed and moved past custom threshold.
	// --------------------------- 
	public bool MovedEx(
		int 		fingerId, 		///< Finger ID (0 or 1)
		float 		thresh			///< Custom threshold
		)
		{		
		Finger finger = ((fingerId == 1) ? this.fingerB : this.fingerA); 
		return (finger.curState && (finger.extremeDragCurDist > thresh));
		}

	// --------------------------
	/// Return true if finger is pressed and just started moving.
	// --------------------------- 
	public bool JustMovedEx(
		int 		fingerId, 		///< Finger ID (0 or 1)
		float 		thresh			///< Custom threshold
		)
		{		
		Finger finger = ((fingerId == 1) ? this.fingerB : this.fingerA); 
		return (finger.curState && 
			(finger.extremeDragCurDist > thresh) &&	
			(finger.extremeDragPrevDist <= thresh));

		}

		
	// -----------------------
	/// Get the most extreme drag distance since start
	// ------------------------
	public float GetExtremeDist(
		int							fingerId, 	///< Finger ID (0 or 1)
		TouchCoordSys	cs			///< Coordinate system	
		)
		{
		Finger finger = ((fingerId == 1) ? this.fingerB : this.fingerA); 
		return TransformPos(finger.extremeDragCurDist, cs);
		}

	// -----------------------
	/// Get the most extreme drag along each axis since start
	// ------------------------
	public Vector2 GetExtremeVec(
		int							fingerId, 	///< Finger ID (0 or 1)
		TouchCoordSys	cs			///< Coordinate system	
		)
		{
		Finger finger = ((fingerId == 1) ? this.fingerB : this.fingerA); 
		return TransformPos(finger.extremeDragCurVec, cs, true);
		}
	
#endif


	/// \}


	// ---------------------------
	/// \name Touch Drag Velocity 
	/// \{
	// ---------------------------

	// -----------------------
	/// Get drag velocity vector of specified finger.
	// ------------------------
	public Vector2 GetDragVel(
		int				fingerId, 									///< Finger ID (0 or 1)
		TouchCoordSys	cs 			/* = TouchCoordSys.SCREEN_PX */	///< Coordinate system	
		)
		{
		Finger finger = ((fingerId == 1) ? this.fingerB : this.fingerA); 
		return TransformPos(finger.dragVel, cs, true);
		}
	// -----------------------
	/// Shortcut for GetDragVel(fingerId, TouchCoordSys.SCREEN_PX)
	// ------------------------
	public Vector2 GetDragVel(
		int				fingerId 		///< Finger ID (0 or 1)
		)
		{
		return this.GetDragVel(fingerId, TouchCoordSys.SCREEN_PX);
		}


	// -----------------------
	/// Get unified-touch drag velocity vector.
	// ------------------------
	public Vector2 GetUniDragVel(
		TouchCoordSys	cs 			/* = TouchCoordSys.SCREEN_PX */	///< Coordinate system.	
		)
		{
		return TransformPos(this.uniDragVel, cs, true);
		}
	// -----------------------
	/// Shortcut for GetUniDragVel(TouchCoordSys.SCREEN_PX)
	// ------------------------
	public Vector2 GetUniDragVel()
		{
		return this.GetUniDragVel(TouchCoordSys.SCREEN_PX);
		}

	// -----------------------
	/// Get multi-touch drag velocity vector.
	// ------------------------
	public Vector2 GetMultiDragVel(
		TouchCoordSys	cs 			/* = TouchCoordSys.SCREEN_PX */	///< Coordinate system.	
		)
		{
		return TransformPos(this.multiDragVel, cs, true);
		}
	// -----------------------
	/// Shortcut for GetMultiDragVel(TouchCoordSys.SCREEN_PX)
	// ------------------------
	public Vector2 GetMultiDragVel()
		{
		return this.GetMultiDragVel(TouchCoordSys.SCREEN_PX);
		}
	

	/// \}

	

	// ---------------------------
	/// \name Twist State
	/// \{
	// ---------------------------


	// --------------
	/// Return true if multi-finger touch was twisted.
	// ---------------
	public bool Twisted()
		{
		return this.twistMoved;
		}

	// --------------
	/// Return true if multi-finger touch was just twisted.
	// ---------------
	public bool JustTwisted()
		{
		return this.twistJustMoved;
		}
		
#if CF_EXTREME_QUERY_FUNCTIONS
	// ----------------
	public bool JustTwistedEx(float threshold)
		{
		return ((this.twistExtremeCur 	>= threshold) && 
				(this.twistExtremePrev	< threshold));
		}	




	// --------------
	/// Get current multi-touch <u>absulute</u> extreme twist angle .
	// ---------------
	public float GetTwistExtremeAngle()
		{
		return this.twistExtremeCur;
		}
#endif	

	
	// -------------
	/// Get twist velocity in degrees per second.
	// -------------
	public float GetTwistVel()
		{
		return this.twistVel;
		}
	

	// ------------------
	/// Get total twist angle since start in degrees (clockwise)
	// --------------------
	public float GetTotalTwist(
		bool raw /*= false */	///< When raw set to false and twist didn't moved, 0 will be returned. 
		)
		{		
		if (!raw && !this.twistMoved)
			return 0;

		return this.twistCur;
		}
	// ------------------
	/// Shortcut for GetTotalTwist(false)
	// --------------------
	public float GetTotalTwist()
		{		
		return this.GetTotalTwist(false);
		}
	// ------------------
	/// Shortcut for GetTotalTwist(true)
	// --------------------
	public float GetTotalTwistRaw()
		{		
		return this.GetTotalTwist(true);
		}


	// ---------------
	/// Get twist delta since relative to last frame.
	// ---------------
	public float GetTwistDelta(
		bool 	raw /* = false */	///< When not in raw mode, 0 will be returned if twist didn't passed the threshold.
		)
		{
		if (!raw && this.twistJustMoved)
			return this.twistCur;

		return Mathf.DeltaAngle(this.twistPrev, this.twistCur);
		}
	// ---------------
	/// Shortcut for GetTwistDelta(false)
	// ---------------
	public float GetTwistDelta()
		{
		return this.GetTwistDelta(false);
		}
	// ---------------
	/// Shortcut for GetTwistDelta(true)
	// ---------------
	public float GetTwistDeltaRaw()
		{
		return this.GetTwistDelta(true);
		}

	/// \}





	// ---------------------------
	/// \name Pinch State 
	/// \{
	// ---------------------------

	// --------------
	/// Return true if two-finger pinch passed the threshold.
	// ---------------
	public bool Pinched()
		{
		return this.pinchMoved;
		}

	// --------------
	/// Return true if multi-finger pinch just started.
	// ---------------
	public bool JustPinched()
		{
		return this.pinchJustMoved;
		}


	
	// --------------
	/// Get finger distance.	
	// --------------
	public float GetPinchDist(
		TouchCoordSys 	cs 	/* = TouchCoordSys.SCREEN_PX */,	///< Coordinate system. 	
		bool 			raw /* = false */						///< When not in raw mode, value of 0 will be returned until pinch moves.
		)
		{
		if (!this.multiCur || (!raw && !this.pinchMoved))
			return 0;

		return TransformPos(this.pinchCurDist, cs);
		}
	// --------------
	/// Shortcut for GetPinchDist(TouchCoordSys.SCREEN_PX, false)	
	// --------------
	public float GetPinchDist()
		{
		return this.GetPinchDist(TouchCoordSys.SCREEN_PX, false);
		}
	// --------------
	/// Shortcut for GetPinchDist(cs, false)	
	// --------------
	public float GetPinchDist(
		TouchCoordSys 	cs 			///< Coordinate system. 	
		)
		{
		return this.GetPinchDist(cs, false);
		}
	// --------------
	/// Shortcut for GetPinchDist(TouchCoordSys.SCREEN_PX, raw)	
	// --------------
	public float GetPinchDist(
		bool 	raw 		///< When not in raw mode, value of 0 will be returned until pinch moves.
		)
		{
		return this.GetPinchDist(TouchCoordSys.SCREEN_PX, raw);
		}
	// --------------
	/// Shortcut for GetPinchDist(TouchCoordSys.SCREEN_PX, true)	
	// --------------
	public float GetPinchDistRaw()
		{
		return this.GetPinchDist(TouchCoordSys.SCREEN_PX, true);
		}
	// --------------
	/// Shortcut for GetPinchDist(cs, true)	
	// --------------
	public float GetPinchDistRaw(
		TouchCoordSys 	cs 			///< Coordinate system. 	
		)
		{
		return this.GetPinchDist(cs, true);
		}

	

	// --------------
	/// Get finger distance delta relative to last update.	
	// --------------
	public float GetPinchDistDelta(
		TouchCoordSys 	cs	/* = TouchCoordSys.SCREEN_PX */,	///< Coordinate system. 	
		bool 			raw /* = false */						///< When not in raw mode, value of 0 will be returned until pinch moves.
		)
		{
		if (!this.multiCur || (!raw && !this.pinchMoved))
			return 0;
		
		return this.TransformPos(
			(this.pinchCurDist - (!raw && this.pinchJustMoved ? this.pinchDistStart : 
			this.pinchPrevDist)), cs);
		}
	// --------------
	/// Shortcut for GetPinchDistDelta(TouchCoordSys.SCREEN_PX, false)	
	// --------------
	public float GetPinchDistDelta()
		{
		return this.GetPinchDistDelta(TouchCoordSys.SCREEN_PX, false);
		}
	// --------------
	/// Shortcut for GetPinchDistDelta(cs, false)	
	// --------------
	public float GetPinchDistDelta(
		TouchCoordSys 	cs		///< Coordinate system. 	
		)
		{
		return this.GetPinchDistDelta(cs, false);
		}
	// --------------
	/// Shortcut for GetPinchDistDelta(TouchCoordSys.SCREEN_PX, raw)	
	// --------------
	public float GetPinchDistDelta(
		bool 			raw		///< When not in raw mode, value of 0 will be returned until pinch moves.
		)
		{
		return this.GetPinchDistDelta(TouchCoordSys.SCREEN_PX, raw);
		}
	// --------------
	/// Shortcut for GetPinchDistDelta(TouchCoordSys.SCREEN_PX, true)	
	// --------------
	public float GetPinchDistDeltaRaw()
		{
		return this.GetPinchDistDelta(TouchCoordSys.SCREEN_PX, true);
		}
	// --------------
	/// Shortcut for GetPinchDistDelta(cs, true)	
	// --------------
	public float GetPinchDistDeltaRaw(
		TouchCoordSys 	cs		///< Coordinate system. 	
		)
		{
		return this.GetPinchDistDelta(cs, true);
		}
		
	
	// ---------------
	/// Get pinch scale relative to start position.
	// ----------------
	public float GetPinchScale(
		bool raw /* = false */	///< When not in raw mode, value of 1 will be returned until pinch moves.
		)
		{
		if (!this.multiCur || (!raw && !this.pinchMoved))
			return 1.0f;
		
		return (this.pinchCurDist / this.pinchDistStart);
		}	
	// ---------------
	/// Shortcut for GetPinchScale(false)
	// ----------------
	public float GetPinchScale()
		{
		return this.GetPinchScale(false);
		}	
	// ---------------
	/// Shortcut for GetPinchScale(true)
	// ----------------
	public float GetPinchScaleRaw()
		{
		return this.GetPinchScale(true);
		}	
	


	// ---------------
	/// Get pinch scale relative to previous update.
	// ----------------
	public float GetPinchRelativeScale(
		bool raw /* = false */	///< When not in raw mode, value of 1 will be returned until pinch moves.
		)
		{
		if (!this.multiCur || (!raw && !this.pinchMoved))
			return 1.0f;
		
		return (this.pinchCurDist / (!raw && this.pinchJustMoved ? 
			this.pinchDistStart : this.pinchPrevDist));
		}	
	// ---------------
	/// Shortcut for GetPinchRelativeScale(false)
	// ----------------
	public float GetPinchRelativeScale()
		{
		return this.GetPinchRelativeScale(false);
		}
	// ---------------
	/// Shortcut for GetPinchRelativeScale(true)
	// ----------------
	public float GetPinchRelativeScaleRaw()
		{
		return this.GetPinchRelativeScale(true);
		}


#if CF_EXTREME_QUERY_FUNCTIONS

	// --------------
	/// Return true if multi-finger touch pinch just passed the threshold.
	// ---------------
	public bool PinchJustMovedEx(
		float threshold			///< Custom distance threshold in pixels.
		)
		{
		return ((this.pinchExtremeCurDist 	>= threshold) && 
				(this.pinchExtremePrevDist	< threshold));
		}	
	// ----------------
	/// Get the most extreme distance difference relative to start.
	// ----------------
	public float GetPinchExtreme()
		{
		return this.pinchExtremeCurDist;
		}	
#endif

	
	// -------------------
	/// Get Pinch Distance Velocity in pixels per second.
	// -------------------
	public float GetPinchDistVel()
		{
		return this.pinchDistVel;
		}
	

	/// \}
	

	
	// ---------------------------
	/// \name Tap and Double Tap
	/// \{
	// ---------------------------

	
	// -------------------
	///	\brief 
	/// Return true if zone just have been tapped with one finger. 
	///
	/// Use this function if you don't care if this is a single or double tap.
	// -------------------
	public bool JustTapped(
		//bool onlyOnce = false	//< When true, this function will return true when the possiblity of a double tap expires. 
		// When set to false, true will be returned immediately after any type of tap - even if it's a double tap.
		// If you want to distinguish beteween single and double taps - set this paramater to true.  
		)
		{
		return this.fingerA.JustTapped(false); //onlyOnce);
		//return (onlyOnce ? this.justDelayTapped : this.justTapped);	
		}

	//public bool JustTapped() {	return this.JustTapped(false); }

	// -------------------
	///	\brief 
	/// Return true if zone just have been tapped with two fingers. 
	///
	/// Use this function if you don't care if this is a single or double tap.
	// -------------------
	public bool JustMultiTapped(
		//bool onlyOnce = false	//< When true, this function will return true when the possiblity of a double tap expires. 
		// When set to false, true will be returned immediately after any type of tap - even if it's a double tap.
		// If you want to distinguish beteween single and double taps - set this paramater to true.  
		)
		{
		//return (onlyOnce ? this.justMultiDelayTapped : this.justMultiTapped);	
		return (this.justMultiTapped);	
		}

	// -------------------
	///	\brief 
	/// Return true if zone just have been SINGLE tapped with one finger.
	/// 
	/// This function will not return true unless there's no further possibility of a double tap -
	/// some time must pass after the actual tap (maximal double tap gap time).
	/// Use this function if you want to distinguish between double and single tap.    
	// -------------------
	public bool JustSingleTapped(
		//bool onlyOnce = false	//< When true, this function will return true when the possiblity of a double tap expires. 
		// When set to false, true will be returned immediately after any type of tap - even if it's a double tap.
		// If you want to distinguish beteween single and double taps - set this paramater to true.  
		)
		{
		return this.fingerA.JustTapped(true);
		//return (onlyOnce ? this.justDelayTapped : this.justTapped);	
		}


	// -------------------
	///	\brief 
	/// Return true if zone just have been SINGLE tapped with two fingers. 	
	///
	/// This function will not return true unless there's no further possibility of a double tap -	
	/// some time must pass after the actual tap (maximal double tap gap time).	
	/// Use this function if you want to distinguish between double and single tap.    
	// -------------------
	public bool JustMultiSingleTapped(
		//bool onlyOnce = false	//< When true, this function will return true when the possiblity of a double tap expires. 
		// When set to false, true will be returned immediately after any type of tap - even if it's a double tap.
		// If you want to distinguish beteween single and double taps - set this paramater to true.  
		)
		{
		return (this.justMultiDelayTapped);	
		}


	// -------------------
	/// Return true if zone just have been double tapped with one finger. 
	// -------------------
	public bool JustDoubleTapped()
		{
		return this.fingerA.JustDoubleTapped();
		}
	
	// -------------------
	/// Return true if zone just have been double tapped with two fingers. 
	// -------------------
	public bool JustMultiDoubleTapped()
		{
		return this.justMultiDoubleTapped;
		}
	

	// --------------------
	public bool JustLongTapped()
		{
		const float LONG_TAP_MIN_DURATION = 1.0f;	// minimal hold time in seconds

		return (this.JustUniReleased() && !this.ReleasedUniMoved() && 
			(this.GetReleasedUniDuration() > LONG_TAP_MIN_DURATION));
		}
	

	const float LONG_PRESS_MIN_DURATION = 1.0f;

	// ------------------
	public bool JustLongPressed(int fingerId)		
		{

		return (this.Pressed(fingerId) && !this.Dragged(fingerId) &&
			 (this.GetTouchDuration(fingerId) > LONG_PRESS_MIN_DURATION) &&
			((this.GetTouchDuration(fingerId) - this.joy.deltaTime) <= LONG_PRESS_MIN_DURATION));
		}
	
	// ----------------
	public bool JustUniLongPressed()		
		{
		return (this.UniPressed() && !this.UniDragged() &&
			 (this.GetUniTouchDuration() > LONG_PRESS_MIN_DURATION) &&
			((this.GetUniTouchDuration() - this.joy.deltaTime) <= LONG_PRESS_MIN_DURATION));
		}
	
	// ------------------
	public bool JustMultiLongPressed()		
		{
		return (this.MultiPressed() && !this.MultiDragged() &&
			 (this.GetMultiTouchDuration() > LONG_PRESS_MIN_DURATION) &&
			((this.GetMultiTouchDuration() - this.joy.deltaTime) <= LONG_PRESS_MIN_DURATION));
		}


	// ---------------
	/// Get last single-finger tap's position.
	// ------------------ 
	public Vector2 GetTapPos(
		TouchCoordSys	cs /* = TouchCoordSys.SCREEN_PX */	///< Coordinate system
		)
		{
		return this.fingerA.GetTapPos(cs);
		}
	// ---------------
	/// Shortcut for GetTapPos(TouchCoordSys.SCREEN_PX)
	// ------------------ 
	public Vector2 GetTapPos()
		{
		return this.GetTapPos(TouchCoordSys.SCREEN_PX);
		}
	

	// ---------------
	/// Get last two-finger tap's position.
	// ------------------ 
	public Vector2 GetMultiTapPos(
		TouchCoordSys	cs	/* = TouchCoordSys.SCREEN_PX */	///< Coordinate system
		)
		{
		return TransformPos(this.lastMultiTapPos, cs, false);
		}
	// ---------------
	/// Shortcut for GetMultiTapPos(TouchCoordSys.SCREEN_PX)
	// ------------------ 
	public Vector2 GetMultiTapPos()
		{
		return this.GetMultiTapPos(TouchCoordSys.SCREEN_PX);
		}

	/// \}





	// ---------------------------
	/// \name Released Touch Position 
	/// \{
	// ---------------------------

	// ---------------------
	/// Get released touch start position.
	// ---------------------
	public Vector2 GetReleasedStartPos(	
		int				fingerId, 									///< Finger ID (0 or 1)
		TouchCoordSys	cs 			/* = TouchCoordSys.SCREEN_PX */	///< Coordinate system	
		)
		{
		Finger finger = ((fingerId == 1) ? this.fingerB : this.fingerA); 
		return TransformPos(finger.endedPosStart, cs, false);
		}
	// ---------------------
	/// Shortcut for GetReleasedStartPos(fingerId, TouchCoordSys.SCREEN_PX)
	// ---------------------
	public Vector2 GetReleasedStartPos(	
		int				fingerId			///< Finger ID (0 or 1)
		)
		{
		return this.GetReleasedStartPos(fingerId, TouchCoordSys.SCREEN_PX);
		}
	

	// ---------------------
	/// Get released unified-touch start position.
	// ---------------------
	public Vector2 GetReleasedUniStartPos(	
		TouchCoordSys	cs 	/* = TouchCoordSys.SCREEN_PX */	///< Coordinate system	
		)
		{
		return TransformPos(this.endedUniPosStart, cs, false);
		}
	// ---------------------
	/// Shortcut for GetReleasedUniStartPos(TouchCoordSys.SCREEN_PX)
	// ---------------------
	public Vector2 GetReleasedUniStartPos()
		{
		return this.GetReleasedUniStartPos(TouchCoordSys.SCREEN_PX);
		}
	

	// ---------------------
	/// Get released multi-touch start position.
	// ---------------------
	public Vector2 GetReleasedMultiStartPos(	
		TouchCoordSys	cs	/* = TouchCoordSys.SCREEN_PX */	///< Coordinate system	
		)
		{
		return TransformPos(this.endedMultiPosStart, cs, false);
		}
	// ---------------------
	/// Shortcut for GetReleasedMultiStartPos(TouchCoordSys.SCREEN_PX)
	// ---------------------
	public Vector2 GetReleasedMultiStartPos()
		{
		return this.GetReleasedMultiStartPos(TouchCoordSys.SCREEN_PX);
		}

	// ---------------------
	/// Get released touch end position.
	// ---------------------
	public Vector2 GetReleasedEndPos(	
		int				fingerId, 									///< Finger ID (0 or 1)
		TouchCoordSys	cs 			/* = TouchCoordSys.SCREEN_PX */	///< Coordinate system	
		)
		{
		Finger finger = ((fingerId == 1) ? this.fingerB : this.fingerA); 
		return TransformPos(finger.endedPosEnd, cs, false);
		}
	// ---------------------
	/// Shortcut for GetReleasedEndPos(fingerId, TouchCoordSys.SCREEN_PX)
	// ---------------------
	public Vector2 GetReleasedEndPos(	
		int				fingerId			///< Finger ID (0 or 1)
		)
		{
		return this.GetReleasedEndPos(fingerId, TouchCoordSys.SCREEN_PX);
		}

	
	// ---------------------
	/// Get released unified-touch end position.
	// ---------------------
	public Vector2 GetReleasedUniEndPos(	
		TouchCoordSys	cs 	/* = TouchCoordSys.SCREEN_PX */	///< Coordinate system	
		)
		{
		return TransformPos(this.endedUniPosEnd, cs, false);
		}
	// ---------------------
	/// Shortcut for GetReleasedUniEndPos(TouchCoordSys.SCREEN_PX)
	// ---------------------
	public Vector2 GetReleasedUniEndPos()
		{
		return this.GetReleasedUniEndPos(TouchCoordSys.SCREEN_PX);
		}
	
	// ---------------------
	/// Get released multi-touch end position.
	// ---------------------
	public Vector2 GetReleasedMultiEndPos(	
		TouchCoordSys	cs 	/* = TouchCoordSys.SCREEN_PX */	///< Coordinate system	
		)
		{
		return TransformPos(this.endedMultiPosEnd, cs, false);
		}
	// ---------------------
	/// Shortcut for GetReleasedMultiEndPos(TouchCoordSys.SCREEN_PX)
	// ---------------------
	public Vector2 GetReleasedMultiEndPos()
		{
		return this.GetReleasedMultiEndPos(TouchCoordSys.SCREEN_PX);
		}
	
	/// \}


	// ---------------------------
	/// \name Released Touch's Drag 
	/// \{
	// ---------------------------


	// ---------------------
	/// Return true if released touch moved.
	// ---------------------
	public bool ReleasedDragged(	
		int							fingerId 	///< Finger ID (0 or 1)
		)
		{
		Finger finger = ((fingerId == 1) ? this.fingerB : this.fingerA); 
		return (finger.endedMoved);
		}

	// ---------------------
	/// Return true if released unified-touch moved.
	// ---------------------
	public bool ReleasedUniDragged()
		{
		return (this.endedUniMoved);
		}

	// ---------------------
	/// Return true if released multi-touch moved.
	// ---------------------
	public bool ReleasedMultiDragged()
		{
		return (this.endedMultiMoved);
		}


	/// \cond
	
	// ---------------------
	public bool ReleasedMoved(int fingerId)
		{ return this.ReleasedDragged(fingerId); }

	// ---------------------
	public bool ReleasedUniMoved()
		{ return this.ReleasedUniDragged(); }

	// ---------------------
	public bool ReleasedMultiMoved()
		{
		return this.ReleasedMultiDragged();
		}


	/// \endcond


#if CF_EXTREME_QUERY_FUNCTIONS

	// -----------------------
	/// Get the most extreme drag distance of last released touch.
	// ------------------------
	public float GetReleasedExtremeDist(
		int							fingerId, 		///< Finger ID (0 or 1)
		TouchCoordSys	cs = 
			TouchCoordSys.SCREEN_PX	///< Coordinate system	
		)
		{
		Finger finger = ((fingerId == 1) ? this.fingerB : this.fingerA); 
		return TransformPos(finger.endedExtremeDragDist, cs);
		}

	// -----------------------
	/// Get the most extreme drag along each axis of last released touch.
	// ------------------------
	public Vector2 GetReleasedExtremeVec(
		int							fingerId, 		///< Finger ID (0 or 1)
		TouchCoordSys	cs = 
			TouchCoordSys.SCREEN_PX	///< Coordinate system	
		)
		{
		Finger finger = ((fingerId == 1) ? this.fingerB : this.fingerA); 
		return TransformPos(finger.endedExtremeDragVec, cs, true);
		}
#endif


	// ---------------------
	/// Get released touch drag vector.
	// ---------------------
	public Vector2 GetReleasedDragVec(	
		int				fingerId, 										///< Finger ID (0 or 1)
		TouchCoordSys	cs 			/* = TouchCoordSys.SCREEN_PX */,	///< Coordinate system	
		bool			raw 		/* = false */						///< When not in raw mode, zero-vector will be returned if released drag didn't passed the threshold.
		)
		{
		Finger finger = ((fingerId == 1) ? this.fingerB : this.fingerA); 
		if (!raw && !finger.endedMoved)
			return Vector2.zero;

		return TransformPos((finger.endedPosEnd - finger.endedPosStart), cs, true);
		}
	// ---------------------
	/// Shortcut for GetReleasedDragVec(fingerId, TouchCoordSys.SCREEN_PX, false);
	// ---------------------
	public Vector2 GetReleasedDragVec(	
		int				fingerId 						///< Finger ID (0 or 1)
		)
		{
		return this.GetReleasedDragVec(fingerId, TouchCoordSys.SCREEN_PX, false);
		}
	// ---------------------
	/// Shortcut for GetReleasedDragVec(fingerId, cs, false);
	// ---------------------
	public Vector2 GetReleasedDragVec(	
		int				fingerId, 		///< Finger ID (0 or 1)
		TouchCoordSys	cs				///< Coordinate system	
		)
		{
		return this.GetReleasedDragVec(fingerId, cs, false);
		}
	// ---------------------
	/// Shortcut for GetReleasedDragVec(fingerId, TouchCoordSys.SCREEN_PX, raw);
	// ---------------------
	public Vector2 GetReleasedDragVec(	
		int				fingerId, 		///< Finger ID (0 or 1)
		bool			raw 			///< When not in raw mode, zero-vector will be returned if released drag didn't passed the threshold.
		)
		{
		return this.GetReleasedDragVec(fingerId, TouchCoordSys.SCREEN_PX, raw);
		}
	// ---------------------
	/// Shortcut for GetReleasedDragVec(fingerId, TouchCoordSys.SCREEN_PX, true);
	// ---------------------
	public Vector2 GetReleasedDragVecRaw(	
		int				fingerId 						///< Finger ID (0 or 1)
		)
		{
		return this.GetReleasedDragVec(fingerId, TouchCoordSys.SCREEN_PX, true);
		}
	// ---------------------
	/// Shortcut for GetReleasedDragVec(fingerId, cs, true);
	// ---------------------
	public Vector2 GetReleasedDragVecRaw(	
		int				fingerId, 		///< Finger ID (0 or 1)
		TouchCoordSys	cs				///< Coordinate system	
		)
		{
		return this.GetReleasedDragVec(fingerId, cs, true);
		}

	
	// ---------------------
	/// Get released unified-touch drag vector.
	// ---------------------
	public Vector2 GetReleasedUniDragVec(	
		TouchCoordSys	cs 	/* = TouchCoordSys.SCREEN_PX */,	///< Coordinate system	
		bool			raw /* = false */						///< When not in raw mode, zero-vector will be returned if released drag didn't passed the threshold.
		)
		{
		if (!raw && !this.endedUniMoved)
			return Vector2.zero;

		return TransformPos((this.endedUniTotalDrag), cs, true);
		}
	// ---------------------
	/// Shortcut for GetReleasedUniDragVec(TouchCoordSys.SCREEN_PX, false);
	// ---------------------
	public Vector2 GetReleasedUniDragVec()
		{
		return this.GetReleasedUniDragVec(TouchCoordSys.SCREEN_PX, false);
		}
	// ---------------------
	/// Shortcut for GetReleasedUniDragVec(cs, false);
	// ---------------------
	public Vector2 GetReleasedUniDragVec(	
		TouchCoordSys	cs				///< Coordinate system	
		)
		{
		return this.GetReleasedUniDragVec(cs, false);
		}
	// ---------------------
	/// Shortcut for GetReleasedUniDragVec(TouchCoordSys.SCREEN_PX, raw);
	// ---------------------
	public Vector2 GetReleasedUniDragVec(	
		bool			raw 			///< When not in raw mode, zero-vector will be returned if released drag didn't passed the threshold.
		)
		{
		return this.GetReleasedUniDragVec(TouchCoordSys.SCREEN_PX, raw);
		}
	// ---------------------
	/// Shortcut for GetReleasedUniDragVec(TouchCoordSys.SCREEN_PX, true);
	// ---------------------
	public Vector2 GetReleasedUniDragVecRaw()
		{
		return this.GetReleasedUniDragVec(TouchCoordSys.SCREEN_PX, true);
		}
	// ---------------------
	/// Shortcut for GetReleasedUniDragVec(cs, true);
	// ---------------------
	public Vector2 GetReleasedUniDragVecRaw(	
		TouchCoordSys	cs				///< Coordinate system	
		)
		{
		return this.GetReleasedUniDragVec(cs, true);
		}
	
	// ---------------------
	/// Get released multi-touch drag vector.
	// ---------------------
	public Vector2 GetReleasedMultiDragVec(	
		TouchCoordSys	cs 	/* = TouchCoordSys.SCREEN_PX */,	///< Coordinate system	
		bool			raw /* = false */						///< When not in raw mode, zero-vector will be returned if released drag didn't passed the threshold.
		)
		{
		if (!raw && !this.endedMultiMoved)
			return Vector2.zero;

		return TransformPos((this.endedMultiPosEnd - this.endedMultiPosStart), cs, true);
		}
	// ---------------------
	/// Shortcut for GetReleasedMultiDragVec(TouchCoordSys.SCREEN_PX, false);
	// ---------------------
	public Vector2 GetReleasedMultiDragVec()
		{
		return this.GetReleasedMultiDragVec(TouchCoordSys.SCREEN_PX, false);
		}
	// ---------------------
	/// Shortcut for GetReleasedMultiDragVec(cs, false);
	// ---------------------
	public Vector2 GetReleasedMultiDragVec(	
		TouchCoordSys	cs				///< Coordinate system	
		)
		{
		return this.GetReleasedMultiDragVec(cs, false);
		}
	// ---------------------
	/// Shortcut for GetReleasedMultiDragVec(TouchCoordSys.SCREEN_PX, raw);
	// ---------------------
	public Vector2 GetReleasedMultiDragVec(	
		bool			raw 			///< When not in raw mode, zero-vector will be returned if released drag didn't passed the threshold.
		)
		{
		return this.GetReleasedMultiDragVec(TouchCoordSys.SCREEN_PX, raw);
		}
	// ---------------------
	/// Shortcut for GetReleasedMultiDragVec(TouchCoordSys.SCREEN_PX, true);
	// ---------------------
	public Vector2 GetReleasedMultiDragVecRaw()
		{
		return this.GetReleasedMultiDragVec(TouchCoordSys.SCREEN_PX, true);
		}
	// ---------------------
	/// Shortcut for GetReleasedMultiDragVec(cs, true);
	// ---------------------
	public Vector2 GetReleasedMultiDragVecRaw(	
		TouchCoordSys	cs				///< Coordinate system	
		)
		{
		return this.GetReleasedMultiDragVec(cs, true);
		}

	/// \}


	// ---------------------------
	/// \name Released Touch's Duration 
	/// \{
	// ---------------------------


	// ---------------------
	/// Get released touch duration.
	// ---------------------
	public float GetReleasedDuration(	
		int							fingerId 	///< Finger ID (0 or 1)
		)
		{
		Finger finger = ((fingerId == 1) ? this.fingerB : this.fingerA); 
		return (finger.endedEndTime - finger.endedStartTime);
		}

	// ---------------------
	/// Get released unified-touch duration.
	// ---------------------
	public float GetReleasedUniDuration()
		{
		return (this.endedUniEndTime - this.endedUniStartTime);
		}

	// ---------------------
	/// Get released multi-touch duration.
	// ---------------------
	public float GetReleasedMultiDuration()
		{
		return (this.endedMultiEndTime - this.endedMultiStartTime);
		}

	/// \}


	// ---------------------------
	/// \name Released Touch's Drag Velocity 
	/// \{
	// ---------------------------
	
	// -----------------------
	/// Get released touch drag velocity vector.
	// ------------------------
	public Vector2 GetReleasedDragVel(
		int				fingerId, 								///< Finger ID (0 or 1)
		TouchCoordSys	cs 		/* = TouchCoordSys.SCREEN_PX */,///< Coordinate system	
		bool			raw 	/* = false */					///< When not in raw mode, zero-vector will be returned if released drag didn't passed the threshold.
		)
		{
		Finger finger = ((fingerId == 1) ? this.fingerB : this.fingerA);

		if (!raw && !finger.endedMoved)
			return Vector2.zero;
 
		return TransformPos(finger.endedDragVel, cs, true);
		}
	// -----------------------
	/// Shortcut for GetReleasedDragVel(fingerId, TouchCoordSys.SCREEN_PX, false)
	// ------------------------
	public Vector2 GetReleasedDragVel(
		int				fingerId 	///< Finger ID (0 or 1)
		)
		{
		return this.GetReleasedDragVel(fingerId, TouchCoordSys.SCREEN_PX, false);
		}
	// -----------------------
	/// Shortcut for GetReleasedDragVel(fingerId, cs, false)
	// ------------------------
	public Vector2 GetReleasedDragVel(
		int				fingerId, 	///< Finger ID (0 or 1)
		TouchCoordSys	cs			///< Coordinate system	
		)
		{
		return this.GetReleasedDragVel(fingerId, cs, false);
		}
	// -----------------------
	/// Shortcut for GetReleasedDragVel(fingerId, TouchCoordSys.SCREEN_PX, raw)
	// ------------------------
	public Vector2 GetReleasedDragVel(
		int				fingerId, 	///< Finger ID (0 or 1)
		bool			raw			///< When not in raw mode, zero-vector will be returned if released drag didn't passed the threshold.
		)
		{
		return this.GetReleasedDragVel(fingerId, TouchCoordSys.SCREEN_PX, raw);
		}
	// -----------------------
	/// Shortcut for GetReleasedDragVel(fingerId, TouchCoordSys.SCREEN_PX, true)
	// ------------------------
	public Vector2 GetReleasedDragVelRaw(
		int				fingerId 	///< Finger ID (0 or 1)
		)
		{
		return this.GetReleasedDragVel(fingerId, TouchCoordSys.SCREEN_PX, true);
		}
	// -----------------------
	/// Shortcut for GetReleasedDragVel(fingerId, cs, true)
	// ------------------------
	public Vector2 GetReleasedDragVelRaw(
		int				fingerId, 	///< Finger ID (0 or 1)
		TouchCoordSys	cs			///< Coordinate system	
		)
		{
		return this.GetReleasedDragVel(fingerId, cs, true);
		}

	
	// -----------------------
	/// Get released unified-touch drag velocity vector.
	// ------------------------
	public Vector2 GetReleasedUniDragVel(
		TouchCoordSys	cs 	/* = TouchCoordSys.SCREEN_PX */,	///< Coordinate system	
		bool			raw /* = false */						///< When not in raw mode, zero-vector will be returned if released drag didn't passed the threshold.

		)
		{
		if (!raw && !this.endedUniMoved)
			return Vector2.zero;
 
		return TransformPos(this.endedUniDragVel, cs, true);
		}
	// -----------------------
	/// Shortcut for GetReleasedUniDragVel(TouchCoordSys.SCREEN_PX, false)
	// ------------------------
	public Vector2 GetReleasedUniDragVel()
		{
		return this.GetReleasedUniDragVel(TouchCoordSys.SCREEN_PX, false);
		}
	// -----------------------
	/// Shortcut for GetReleasedUniDragVel(cs, false)
	// ------------------------
	public Vector2 GetReleasedUniDragVel(
		TouchCoordSys	cs			///< Coordinate system	
		)
		{
		return this.GetReleasedUniDragVel(cs, false);
		}
	// -----------------------
	/// Shortcut for GetReleasedUniDragVel(TouchCoordSys.SCREEN_PX, raw)
	// ------------------------
	public Vector2 GetReleasedUniDragVel(
		bool			raw			///< When not in raw mode, zero-vector will be returned if released drag didn't passed the threshold.
		)
		{
		return this.GetReleasedUniDragVel(TouchCoordSys.SCREEN_PX, raw);
		}
	// -----------------------
	/// Shortcut for GetReleasedUniDragVel(TouchCoordSys.SCREEN_PX, true)
	// ------------------------
	public Vector2 GetReleasedUniDragVelRaw()
		{
		return this.GetReleasedUniDragVel(TouchCoordSys.SCREEN_PX, true);
		}
	// -----------------------
	/// Shortcut for GetReleasedUniDragVel(cs, true)
	// ------------------------
	public Vector2 GetReleasedUniDragVelRaw(
		TouchCoordSys	cs			///< Coordinate system	
		)
		{
		return this.GetReleasedUniDragVel(cs, true);
		}


	// -----------------------
	/// Get released multi-touch drag velocity vector.
	// ------------------------
	public Vector2 GetReleasedMultiDragVel(
		TouchCoordSys	cs 	/* = TouchCoordSys.SCREEN_PX */	,	///< Coordinate system	
		bool			raw	/* = false */						///< When not in raw mode, zero-vector will be returned if released drag didn't passed the threshold.

		)
		{
		if (!raw && !this.endedMultiMoved)
			return Vector2.zero;
 
		return TransformPos(this.endedMultiDragVel, cs, true);
		}
	// -----------------------
	/// Shortcut for GetReleasedMultiDragVel(TouchCoordSys.SCREEN_PX, false)
	// ------------------------
	public Vector2 GetReleasedMultiDragVel()
		{
		return this.GetReleasedMultiDragVel(TouchCoordSys.SCREEN_PX, false);
		}
	// -----------------------
	/// Shortcut for GetReleasedMultiDragVel(cs, false)
	// ------------------------
	public Vector2 GetReleasedMultiDragVel(
		TouchCoordSys	cs			///< Coordinate system	
		)
		{
		return this.GetReleasedMultiDragVel(cs, false);
		}
	// -----------------------
	/// Shortcut for GetReleasedMultiDragVel(TouchCoordSys.SCREEN_PX, raw)
	// ------------------------
	public Vector2 GetReleasedMultiDragVel(
		bool			raw			///< When not in raw mode, zero-vector will be returned if released drag didn't passed the threshold.
		)
		{
		return this.GetReleasedMultiDragVel(TouchCoordSys.SCREEN_PX, raw);
		}
	// -----------------------
	/// Shortcut for GetReleasedMultiDragVel(TouchCoordSys.SCREEN_PX, true)
	// ------------------------
	public Vector2 GetReleasedMultiDragVelRaw()
		{
		return this.GetReleasedMultiDragVel(TouchCoordSys.SCREEN_PX, true);
		}
	// -----------------------
	/// Shortcut for GetReleasedMultiDragVel(cs, true)
	// ------------------------
	public Vector2 GetReleasedMultiDragVelRaw(
		TouchCoordSys	cs			///< Coordinate system	
		)
		{
		return this.GetReleasedMultiDragVel(cs, true);
		}

	/// \}

	

	// ---------------------------
	/// \name Released Touch's Twist State
	/// \{
	// ---------------------------

#if CF_EXTREME_QUERY_FUNCTIONS
	// --------------
	/// Get released multi-touch' <u>absulute</u> extreme twist angle.
	// ---------------
	public float GetReleasedTwistExtremeAngle()
		{
		return this.endedTwistExtremeAngle;
		}
#endif	
	
	// --------------
	/// Return true if released multi-finger touch was twisted.
	// ---------------
	public bool ReleasedTwisted()
		{
		return this.endedTwistMoved;
		}
	
	/// \cond

	// ---------------
	public bool ReleasedTwistMoved()
		{
		return this.ReleasedTwisted();
		}

	/// \endcond

	// -------------
	/// Get released twist's angle in degrees.
	// -------------
	public float GetReleasedTwistAngle(
		bool	raw		/* = false */	///< When not in raw mode, zero will be returned if released twist didn't passed the threshold.
		)
		{
		if (!raw && !this.endedTwistMoved)
			return 0;

		return this.endedTwistAngle;
		}
	// -------------
	/// Shortcut for GetReleasedTwistAngle(false)
	// -------------
	public float GetReleasedTwistAngle()
		{
		return this.GetReleasedTwistAngle(false);
		}
	// -------------
	/// Shortcut for GetReleasedTwistAngle(true)
	// -------------
	public float GetReleasedTwistAngleRaw()
		{
		return this.GetReleasedTwistAngle(true);
		}


	// -------------
	/// Get released twist's velocity in degrees per second.
	// -------------
	public float GetReleasedTwistVel(
		bool	raw		/* = false */	///< When not in raw mode, zero will be returned if released twist didn't passed the threshold.
		)
		{
		if (!raw && !this.endedTwistMoved)
			return 0;

		return this.endedTwistVel;
		}
	// -------------
	/// Shortcut for GetReleasedTwistVel(false)
	// -------------
	public float GetReleasedTwistVel()
		{
		return this.GetReleasedTwistVel(false);
		}
	// -------------
	/// Shortcut for GetReleasedTwistVel(true)
	// -------------
	public float GetReleasedTwistVelRaw()
		{
		return this.GetReleasedTwistVel(true);
		}
	
	/// \}

	// ---------------------------
	/// \name Released Touch's Pinch State 
	/// \{
	// ---------------------------


	// --------------
	/// Return true if released multi-finger touch was pinched.
	// ---------------
	public bool ReleasedPinched()
		{
		return this.endedPinchMoved;
		}
	
	/// \cond
	// -------------
	public bool ReleasedPinchMoved()
		{
		return this.ReleasedPinched();
		}
	/// \endcond

	// ---------------
	/// Get released multi-finger touch last pinch scale.  
	// ---------------
	public float GetReleasedPinchScale(
		bool	raw		/* = false */	///< When not in raw mode, value of one will be returned if released pinch didn't passed the threshold.
		)
		{
		if (!raw && !this.endedPinchMoved)
			return 1;

		return (this.endedPinchDistEnd / this.endedPinchDistStart);
		}
	// -------------
	/// Shortcut for GetReleasedPinchScale(false)
	// -------------
	public float GetReleasedPinchScale()
		{
		return this.GetReleasedPinchScale(false);
		}
	// -------------
	/// Shortcut for GetReleasedPinchScale(true)
	// -------------
	public float GetReleasedPinchScaleRaw()
		{
		return this.GetReleasedPinchScale(true);
		}

	// ---------------
	/// Get released multi-finger touch initial finger distance.  
	// ---------------
	public float GetReleasedPinchStartDist(
		TouchCoordSys	cs	/* = TouchCoordSys.SCREEN_PX */		///< Coordinate system.
		)
		{
		return (this.TransformPos(this.endedPinchDistStart, cs));
		}
	// -------------
	/// Shortcut for GetReleasedPinchStartDist(TouchCoordSys.SCREEN_PX)
	// -------------
	public float GetReleasedPinchStartDist()
		{
		return this.GetReleasedPinchStartDist(TouchCoordSys.SCREEN_PX);
		}
	
	// ---------------
	/// Get released multi-finger touch final finger distance.  
	// ---------------
	public float GetReleasedPinchEndDist(
		TouchCoordSys cs	/* = TouchCoordSys.SCREEN_PX */		///< Coordinate system.
		)
		{
		return (this.TransformPos(this.endedPinchDistEnd, cs));
		}
	// -------------
	/// Shortcut for GetReleasedPinchEndDist(TouchCoordSys.SCREEN_PX)
	// -------------
	public float GetReleasedPinchEndDist()
		{
		return this.GetReleasedPinchEndDist(TouchCoordSys.SCREEN_PX);
		}
	
#if CF_EXTREME_QUERY_FUNCTIONS
	// -------------
	/// Get most extreme finger distance difference since start.
	// -------------
	public float GetReleasedPinchExtremeDistDiff(
		TouchCoordSys cs = 
			TouchCoordSys.SCREEN_PX	///< Coordinate system.
		)
		{
		return (this.TransformPos(this.endedPinchExtremeDistDelta, cs));
		}
#endif
	
	// ---------------
	/// Get released multi-finger pinch distance velocity.
	// ---------------
	public float GetReleasedPinchDistVel(
		TouchCoordSys 	cs 	/* = TouchCoordSys.SCREEN_PX */,	///< Coordinate system.
		bool			raw /* = false */						///< When not in raw mode, zero will be returned if released pinch didn't passed the threshold.
		)
		{
		if (!raw && !this.endedPinchMoved)
			return 0;

		return (this.TransformPos(this.endedPinchDistVel, cs));
		}
	// ---------------
	/// Shortcut for GetReleasedPinchDistVel(TouchCoordSys.SCREEN_PX, false);
	// ---------------
	public float GetReleasedPinchDistVel()
		{
		return this.GetReleasedPinchDistVel(TouchCoordSys.SCREEN_PX, false);
		}
	// ---------------
	/// Shortcut for GetReleasedPinchDistVel(cs, false);
	// ---------------
	public float GetReleasedPinchDistVel(
		TouchCoordSys 	cs		///< Coordinate system.
		)
		{
		return this.GetReleasedPinchDistVel(cs, false);
		}
	// ---------------
	/// Shortcut for GetReleasedPinchDistVel(TouchCoordSys.SCREEN_PX, raw);
	// ---------------
	public float GetReleasedPinchDistVel(
		bool			raw		///< When not in raw mode, zero will be returned if released pinch didn't passed the threshold.
		)
		{
		return this.GetReleasedPinchDistVel(TouchCoordSys.SCREEN_PX, raw);
		}
	// ---------------
	/// Shortcut for GetReleasedPinchDistVel(TouchCoordSys.SCREEN_PX, true);
	// ---------------
	public float GetReleasedPinchDistVelRaw()
		{
		return this.GetReleasedPinchDistVel(TouchCoordSys.SCREEN_PX, true);
		}
	// ---------------
	/// Shortcut for GetReleasedPinchDistVel(cs, true);
	// ---------------
	public float GetReleasedPinchDistVelRaw(
		TouchCoordSys 	cs		///< Coordinate system.
		)
		{
		return this.GetReleasedPinchDistVel(cs, true);
		}



	/// \}



	// ---------------------------
	/// \name General Methods
	/// \{
	// ---------------------------


	// ------------------
	/// Go through every other control in this controller and totally takeover currently used touches.
	// ------------------
	public void TotalTakeover()
		{
		this.joy.EndTouch(this.fingerA.touchId, this);
		this.joy.EndTouch(this.fingerB.touchId, this);
		}




	// -----------------
	/// Enable this control.
	// ------------------
	override public void Enable(
		bool skipAnimation	/*= false */		///< Skin enabling animation.
		)
		{
		//if (this.enabled)
		//	return;
	
		this.enabled = true;

		this.AnimateParams(
			(this.overrideScale ? this.releasedScale : this.joy.releasedZoneScale),
			TouchController.ScaleAlpha((this.overrideColors ? 
			this.releasedColor : this.joy.defaultReleasedZoneColor),
			(this.visible ? 1 : 0)), 
			(skipAnimation ? 0 : (this.overrideAnimDuration ? 
				this.enableAnimDuration: this.joy.enableAnimDuration)));
		} 

	// -----------------
	/// Disable this control and release any active touches.
	// -----------------
	override public void Disable(
		bool skipAnim /* = false */	///< Skip the animation.
		)
		{
		//if (!this.enabled)
		//	return;

		this.enabled = false;

		this.ReleaseTouches();
		
		this.AnimateParams(
			(this.overrideScale ? this.disabledScale : this.joy.disabledZoneScale), 
			TouchController.ScaleAlpha((this.overrideColors ? 
			this.disabledColor : this.joy.defaultDisabledZoneColor),
			(this.visible ? 1 : 0)), 
			(skipAnim ? 0 : (this.overrideAnimDuration ? 
				this.disableAnimDuration : this.joy.disableAnimDuration)));					
		} 


	

	
	// ------------------
	/// Show hidden control.
	// ------------------
	override public void Show(
		bool	skipAnim	/*= false */		///< Skip animation.
		)
		{
		//if (this.visible)
		//	return;

		this.visible = true;

		this.AnimateParams(
			(this.overrideScale ? 
				(this.enabled ? this.releasedScale : this.disabledScale) : 
				(this.enabled ? this.joy.releasedZoneScale : this.joy.disabledZoneScale)),
			(this.overrideColors ? 
				(this.enabled ? this.releasedColor : this.disabledColor) : 
				(this.enabled ? this.joy.defaultReleasedZoneColor :	this.joy.defaultDisabledZoneColor)), 
			(skipAnim ? 0 : (this.overrideAnimDuration ? 
				this.showAnimDuration : this.joy.showAnimDuration)));					
		
		}


	// ------------------
	/// Hide this control and release any active touches.
	// ------------------
	override public void Hide(
		bool	skipAnim	/* = false */	 	///< Skip animation.
		)
		{
		//if (!this.visible)
		//	return;

		this.visible = false;
		
		this.ReleaseTouches();

		Color hiddenColor = this.animColor.end;
		hiddenColor.a = 0;

		this.AnimateParams(this.animScale.end,  
			hiddenColor, 
			(skipAnim ? 0 : (this.overrideAnimDuration ? 
				this.hideAnimDuration : this.joy.hideAnimDuration)));					
		}
	
	
	/// \}
	


	// ---------------------------
	/// \name Custom Layout Methods
	/// \{
	// ---------------------------


	// ---------------
	/// Set custom positioning. 
	/// \note Keep in mind that this will be reset to automatic rect on next layout change. 
	/// \note For circular zones, shorter dimension on the rectangle will be used as zone's size. 
	// ---------------
	public void SetRect(Rect r)
		{
		if (this.screenRectPx != r)
			{
			this.screenRectPx = r;
		
			this.posPx = r.center;

			if (this.shape == TouchController.ControlShape.CIRCLE)
				this.sizePx.x = this.sizePx.y = Mathf.Min(r.width, r.height);
			else
				{
				this.sizePx.x = r.width;
				this.sizePx.y = r.height; 
				}

			this.OnReset();
			}		
		}

	
	// ----------------
	/// Reset positioning and size to automatically calculated.
	// ---------------
	override public void ResetRect()
		{ 
		this.SetRect(this.layoutRectPx);	
		} 

	
	// ------------------
	/// Get current pixel rectangle of this zone.
	// ----------------
	public Rect GetRect(
		bool	getAutoRect /* = false */	///< When true, automatically calculated rect will be returned instead of current pixel rectangle. 
		)	
		{
		Rect r = (getAutoRect ? this.layoutRectPx : this.screenRectPx);
		return r;
		}
	// ------------------
	/// Shortcut for GetRect(false)
	// ----------------
	public Rect GetRect()	
		{
		return this.GetRect(false);	
		}
	
	/// \}



	// ---------------------------
	/// \name Custom GUI Helper Methods 
	/// \{
	// ---------------------------


	// --------------
	/// Get display rect.
	// ---------------
	public Rect GetDisplayRect(
		bool	applyScale /* = true */	// When true current scale animation will be applied.	
		)
		{
		Rect rect = this.screenRectPx;

		if ((this.shape == TouchController.ControlShape.CIRCLE) ||	
			(this.shape == TouchController.ControlShape.RECT))	
			rect = TouchController.GetCenRect(this.posPx, this.sizePx * 
				(applyScale ? this.animScale.cur : 1.0f));

		return rect;
		}
	// --------------
	/// Shortcut for GetDisplayRect(true)
	// ---------------
	public Rect GetDisplayRect()
		{
		return this.GetDisplayRect(true);
		}	

	// -----------------
	/// Get current display color.
	// ----------------- 
	public Color GetColor()
		{
		return this.animColor.cur;
		}


	// -----------------
	/// Get GUI depth.
	// -----------------	
	public int GetGUIDepth()
		{
		return (this.joy.guiDepth + this.guiDepth + 
			(this.UniPressed() ? this.joy.guiPressedOfs : 0));
		}


	// -----------------
	/// Get zone's current display texture.
	// ----------------
	public Texture2D GetDisplayTex()
		{
		return ((this.enabled && this.UniPressed()) ? 	
			this.pressedImg : this.releasedImg);
		}
			

	/// \}
	

	/// \cond

	// ---------------------------
	/// \name Unity Input class replacement methods.
	/// \{
	// ---------------------------
	

	// -------------
	public bool GetKey(KeyCode key)
		{
		bool keySupported = false;
		return this.GetKeyEx(key, out keySupported);
		}

	// -------------
	public bool GetKeyDown(KeyCode key)
		{
		bool keySupported = false;
		return this.GetKeyDownEx(key, out keySupported);
		}

	// -------------
	public bool GetKeyUp(KeyCode key)
		{
		bool keySupported = false;
		return this.GetKeyUpEx(key, out keySupported);
		}



	// --------------
	public bool GetKeyEx(KeyCode key, out bool keySupported)
		{
		keySupported = false;

		if (!this.enableGetKey || (key == KeyCode.None))
			return false;

		if ((key == this.getKeyCode) || (key == this.getKeyCodeAlt))
			{
			keySupported = true;

			if (this.UniPressed())
				return true;
			}

		if ((key == this.getKeyCodeMulti) || (key == this.getKeyCodeMultiAlt))
			{
			keySupported = true;	
			if (this.MultiPressed())		
				return true;
			}
		
		return false;
		}


	// --------------
	public bool GetKeyDownEx(KeyCode key, out bool keySupported)
		{
		keySupported = false;

		if (!this.enableGetKey || (key == KeyCode.None))
			return false;

		if ((key == this.getKeyCode) || (key == this.getKeyCodeAlt))
			{
			keySupported = true;

			if (this.JustUniPressed())
				return true;
			}

		if ((key == this.getKeyCodeMulti) || (key == this.getKeyCodeMultiAlt))
			{
			keySupported = true;	
			if (this.JustMultiPressed())		
				return true;
			}
		
		return false;
		}

	// --------------
	public bool GetKeyUpEx(KeyCode key, out bool keySupported)
		{
		keySupported = false;

		if (!this.enableGetKey || (key == KeyCode.None))
			return false;

		if ((key == this.getKeyCode) || (key == this.getKeyCodeAlt))
			{
			keySupported = true;

			if (this.JustUniReleased())
				return true;
			}

		if ((key == this.getKeyCodeMulti) || (key == this.getKeyCodeMultiAlt))
			{
			keySupported = true;	
			if (this.JustMultiReleased())		
				return true;
			}
		
		return false;
		}

	

	// -------------
	public bool GetButton(string buttonName)
		{
		bool buttonSupported = false;
		return this.GetButtonEx(buttonName, out buttonSupported);
		}

	// -------------
	public bool GetButtonDown(string buttonName)
		{
		bool buttonSupported = false;
		return this.GetButtonDownEx(buttonName, out buttonSupported);
		}

	// -------------
	public bool GetButtonUp(string buttonName)
		{
		bool buttonSupported = false;
		return this.GetButtonUpEx(buttonName, out buttonSupported);
		}




	// --------------
	public bool GetButtonEx(string buttonName, out bool buttonSupported)
		{
		buttonSupported = false;

		if (!this.enableGetButton)
			return false;

		if (buttonName == this.getButtonName)
			{
			buttonSupported = true;
			if (this.UniPressed())
				return true;
			}

		if (buttonName == this.getButtonMultiName)
			{
			buttonSupported = true;	
			if (this.MultiPressed())		
				return true;
			}
		
		return false;
		}

	// --------------
	public bool GetButtonDownEx(string buttonName, out bool buttonSupported)
		{
		buttonSupported = false;

		if (!this.enableGetButton)
			return false;

		if (buttonName == this.getButtonName)
			{
			buttonSupported = true;
			if (this.JustUniPressed())
				return true;
			}

		if (buttonName == this.getButtonMultiName)
			{
			buttonSupported = true;	
			if (this.JustMultiPressed())		
				return true;
			}
		
		return false;
		}

	// --------------
	public bool GetButtonUpEx(string buttonName, out bool buttonSupported)
		{
		buttonSupported = false;

		if (!this.enableGetButton)
			return false;

		if (buttonName == this.getButtonName)
			{
			buttonSupported = true;
			if (this.JustUniReleased())
				return true;
			}

		if (buttonName == this.getButtonMultiName)
			{
			buttonSupported = true;	
			if (this.JustMultiReleased())		
				return true;
			}
		
		return false;
		}
	


	// --------------
	public float GetAxis(string axisName)
		{	
		bool supported = false;
		return this.GetAxisEx(axisName, out supported);

/*
		if (!this.enableGetAxis || !this.UniPressed())
			return 0;

		if (this.axisHorzName == axisName)
			return this.GetUniDragDelta(true).x * this.axisValScale; 
		if (this.axisVertName == axisName)
			return this.GetUniDragDelta(true).y * this.axisValScale; 

		return 0;
*/
		}
	
	// --------------
	public float GetAxisEx(string axisName, out bool supported)
		{
		
		//if (!this.enableGetAxis || !this.UniPressed())
		//	{
		//	supported = false;
		//	return 0;
		//	}
		
		if (this.enableGetAxis)
			{
			if (this.axisHorzName == axisName)
				{
				supported = true;
				return this.GetUniDragDelta(true).x * this.axisValScale * (this.axisHorzFlip ? -1.0f : 1.0f);
				} 
	
			if (this.axisVertName == axisName)
				{
				// HACK : vertical inverted by default to match Unity mouse delta (up = negative y)

				supported = true;
				return this.GetUniDragDelta(true).y * -this.axisValScale * (this.axisVertFlip ? -1.0f : 1.0f); 
				}
			}

		supported = false;
		return 0;
		}



	/// \}
	
	/// \endcond


	// ---------------------------
	/// \name Utils
	/// \{
	// ---------------------------


	// ---------------
	/// Get normalized position's box portion id/mask. 
	/*!
		\retval Returned value is a bitmask of 
		<ul>
			<li>horizontal portion bit
				<ul>
				<li>zero for 1 (or less) sections</li>
				<li>BOX_LEFT or BOX_RIGHT for 2 sections</li>
				<li>BOX_LEFT, BOX_RIGHT or BOX_CEN for 3 (or more) sections</li>
				</ul>
			</li>
			<li>and vertical portion bit
				<ul>	
					<li>zero for 1 (or less) sections</li>
					<li>BOX_TOP or BOX_BOTTOM for 2 sections</li>
					<li>BOX_TOP, BOX_BOTTOM or BOX_MID for 3 (or more) sections</li>
				</ul>
			</li>
		</ul> 
		Predefined bitmasks:
		<ul>
			<li>BOX_TOP_LEFT</li>
			<li>BOX_TOP_CEN</li>
			<li>BOX_TOP_RIGHT</li>
			<li>BOX_MID_LEFT</li>
			<li>BOX_MID_CEN</li>
			<li>BOX_MID_RIGHT</li>
			<li>BOX_BOTTOM_LEFT</li>
			<li>BOX_BOTTOM_CEN</li>
			<li>BOX_BOTTOM_RIGHT</li>
		</ul>
	 
	Example:
	\code
	if (zone.JustReleased(0) && zone.ReleasedDragged(0))
		{
		int portion = TouchZone.GetBoxPortion(2, 2, zone.GetReleasedEndPos(0, TouchCoordSys.SCREEN_NORMALIZED));
 		if (portion == TouchZone.BOX_TOP_RIGHT)
			{
			// Swipe ended at top-right screen quarter - Perform Right High Attack!!
			}			
		}
	\endcode
	**/

	// ---------------
	static public int GetBoxPortion(
		int 		horzSections,		///< Number of horizontal sections (1, 2 or 3). 
		int 		vertSections,		///< Number of vertical sections (1, 2 or 3).
		Vector2		normalizedPos		///< Position in normalized coord. space.
		)
		{
		int hcode = 0;
		int vcode = 0;

		if (horzSections == 2)
			{
			hcode = ((normalizedPos.x < 0.5f) ? BOX_LEFT : BOX_RIGHT);
			}
		else if (horzSections >= 3)
			{
			hcode = ((normalizedPos.x < 0.333f) ? BOX_LEFT :
				(normalizedPos.x > 0.666f) ? BOX_RIGHT : BOX_CEN); 
			}
		
		if (vertSections == 2)
			{
			vcode = ((normalizedPos.y < 0.5f) ? BOX_TOP : BOX_BOTTOM);
			}
		else if (vertSections >= 3)
			{
			vcode = ((normalizedPos.y < 0.333f) ? BOX_TOP :
				(normalizedPos.y > 0.666f) ? BOX_BOTTOM : BOX_MID); 
			}

		return (hcode | vcode);
		}
	


	/// \}


	// -----------------
	// Hide documentation
	///	\cond 
	// ------------------


	// ---------------------
	override public void Init(TouchController joy)
		{	
		base.Init(joy);

		this.joy = joy;
		this.fingerA = new Finger(this);
		this.fingerB = new Finger(this);	
		

		this.AnimateParams(
			(this.overrideScale ? this.releasedScale : this.joy.releasedZoneScale), 	
			(this.overrideColors ? this.releasedColor : this.joy.defaultReleasedZoneColor), 
			0);


		this.OnReset();

		if (this.initiallyDisabled) 
			this.Disable(true);
		if (this.initiallyHidden) 
			this.Hide(true);

		}




	// ----------
	public int GetFingerNum()
		{ 
		return ((this.fingerA.curState ? 1 : 0) + 
				(this.fingerB.curState ? 1 : 0));
		//return (((this.fingerA.touchId >= 0) ? 1 : 0) + 
		//		((this.fingerB.touchId >= 0) ? 1 : 0));
		}
		
	
	// ------------------------
	// Animtion functions 
	// ------------------------

	// ---------------------
	private void AnimateParams(float scale, Color color, float duration)
		{
		if (duration <= 0)
			{
			this.animTimer.Reset(0);
			this.animTimer.Disable();
		
			this.animColor.Reset(color);
			this.animScale.Reset(scale);
			}
		else
			{
			this.animTimer.Start(duration);
			this.animScale.MoveTo(scale);
			this.animColor.MoveTo(color);
			}
		}


		
	
	// ------------------
	override public void OnReset()
		{
		this.fingerA.Reset();
		this.fingerB.Reset();

		//this.nextTapCanBeDoubleTap 	= false;
		//this.lastTapTime 			= -100;
		//this.uniStartTime 		= -100;
		//this.uniCur			= false;
		//this.uniHolding		= false;
		//this.uniJustStartedHold= false;
		
		this.multiCur				= 
		this.multiPrev				= 
		//this.multiMidFrame			= 
	//	this.justStartedMultiHold	= 
	//	this.multiHold				= 
		this.justMultiTapped		= 
		this.justMultiDelayTapped	= 
		this.justMultiDoubleTapped	= 
		this.nextTapCanBeMultiDoubleTap	= false;
		this.twistMoved					= 
		this.twistJustMoved				= 
		this.pinchMoved					= 
		this.pinchJustMoved				= 
		this.uniMoved				= 
		this.uniJustMoved			= 
	//	this.uniJustStartedHold	= 
	//	this.uniHolding			= 
		//this.uniNeedReset			= 
		this.uniCur				= 
		this.uniPrev				= false;

		//this.justTapped					= 
		//this.justDelayTapped			= 
		//this.justDoubleTapped			= 
		//this.nextTapCanBeDoubleTap		= 


		this.multiStartTime			= 
		//this.multiEndTime			= 
		//this.multiMidFrameTime		= 
		//this.lastTapTime				= 
		this.lastMultiTapTime		= 
		this.uniStartTime			= -100;
		//this.uniEndTime			= -100;
		//this.uniMidFrameClickTime	= -100;



		this.multiPosCur			= 
		this.multiPosPrev			= 
		this.multiPosStart			= 
		//this.lastTapPos					= 
		this.lastMultiTapPos		= Vector2.zero;

		this.multiDragVel	= Vector2.zero;
		this.uniDragVel		= Vector2.zero;
		this.twistVel		= 0;
		this.pinchDistVel	= 0;

		//this.multiRecentDragVel	= 0;	
		//this.uniRecentVel			= 0;

		this.twistStartAbs				= 
		this.twistCurAbs				= 
		this.twistCur					= 
		this.twistCurRaw 				=
		//this.twistPrevRaw				=
		this.twistPrevAbs				= 
		this.twistPrev					= 0; 
		//this.twistRecentVel				= 0;
		this.twistVel					= 0;

		this.pinchDistStart				= 
		this.pinchCurDist				= 
		//this.pinchCurScale				= 
		this.pinchPrevDist				= 
		//this.pinchPrevScale				= 
		//this.pinchRecentDistVel			= 0;
		this.pinchDistVel				= 0;


		this.uniPosCur				= 
		//this.uniPosPrev				=	 
		this.uniPosStart			= 
		this.uniTotalDragCur		= 
		this.uniTotalDragPrev		= Vector2.zero;
	
		this.touchCanceled 			= false;

	
#if UNITY_EDITOR
		if (!EditorApplication.isPlayingOrWillChangePlaymode)
			{
			this.OnColorChange();
			//this.SetColorsDirtyFlag();
			}
		else
#endif		
			{
			this.AnimateParams(
				(this.overrideScale ? this.releasedScale : this.joy.releasedZoneScale), 	
				(this.overrideColors ? this.releasedColor : this.joy.defaultReleasedZoneColor), 
				0);

			if (!this.enabled)
				this.Disable(true);
			if (!this.visible)
				this.Hide(true);

			}

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
	private void OnColorChange()
		{
		this.colorsDirtyFlag = false;
		
//Debug.Log("ZONE ["+this.name+"] OnColChange!");

		if (this.joy.previewMode == TouchController.PreviewMode.PRESSED)
			{
			this.AnimateParams(
				(this.overrideScale ? this.pressedScale : this.joy.pressedZoneScale),
				(this.overrideColors ? this.pressedColor : this.joy.defaultPressedZoneColor), 
				0);
			}
		else 
			{
			this.AnimateParams(
				(this.overrideScale ? this.releasedScale : this.joy.releasedZoneScale),
				(this.overrideColors ? this.releasedColor : this.joy.defaultReleasedZoneColor), 
				0);
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
		this.fingerA.OnPrePoll();
		this.fingerB.OnPrePoll();
		}

	// ---------------
	override public void OnPostPoll()
		{	
		if ((this.fingerA.touchId >= 0) && !this.fingerA.touchVerified)
			{
//this.joy.debugLastBreakElapsed = 0;

//Debug.Log("POST POLL CHECK!");
			this.OnTouchEnd(this.fingerA.touchId);
			}

		if ((this.fingerB.touchId >= 0) && !this.fingerB.touchVerified)
			{
//Debug.Log("POST POLL CHECK!");
			this.OnTouchEnd(this.fingerB.touchId);
			}

#if UNITY_EDITOR

		// Debug key...
		if ((this.fingerA.touchId < 0) && (this.fingerB.touchId < 0))
			{
			
			}
#endif

		}

	
	// ----------------
	override public void ReleaseTouches()
		{
		if ((this.fingerA.touchId >= 0))
			this.OnTouchEnd(this.fingerA.touchId, true);

		if ((this.fingerB.touchId >= 0))
			this.OnTouchEnd(this.fingerB.touchId, true);
		
		}

	

	// -------------------
	private void OnMultiStart(Vector2 startPos, Vector2 curPos)
		{
		
		//finger.curState = true;
		//finger.justPressed = true;
	
		this.multiCur 			= true;
		//this.multiJustPressed = true;		

		this.multiStartTime 	= this.joy.curTime;	
		this.multiPosStart		= startPos;	//this.pollMultiPosStart;
		this.multiPosPrev		= 
		this.multiPosCur		= curPos;	//this.pollMultiPosCur;

		this.multiMoved = false;

		this.multiLastMoveTime	= 0;
		this.multiDragVel		= Vector2.zero;

		this.multiExtremeCurVec		= Vector2.zero; 
		this.multiExtremeCurDist 	= 0;

#if CF_EXTREME_QUERY_FUNCTIONS
		this.multiExtremePrevDist 	= 0;
		this.multiExtremePrevVec 	= Vector2.zero; 
#endif		
		//this.multiRecentDragVel	= 0;


		this.pinchCurDist 		=
		this.pinchPrevDist		=
		this.pinchDistStart 	= this.GetFingerDist();
		//this.pinchCurScale		= 1.0f;
		//this.pinchPrevScale		= 1.0f;
		this.pinchMoved			= false;
		this.pinchJustMoved		= false;
		this.pinchLastMoveTime 	= 0;
		this.pinchDistVel		= 0;
		this.pinchExtremeCurDist = 0;

		this.twistCurAbs		=
		this.twistPrevAbs		=
		this.twistStartAbs 		= this.GetFingerAbsAngle();
		this.twistCur 			=
		this.twistPrev			= 0;
		this.twistMoved 		= false;
		this.twistJustMoved		= false;
		this.twistLastMoveTime	= 0;
		this.twistVel			= 0;
		this.twistExtremeCur	= 0;

#if CF_EXTREME_QUERY_FUNCTIONS
		this.pinchExtremePrevDist = 0;
		this.twistExtremePrev	= 0;
#endif		

		}

	// --------------------
	private void OnMultiEnd(Vector2 endPos)
		{
		this.multiPosCur	= endPos;	// ?!

		this.UpdateMultiTouchState(true);
		

		this.multiCur 			= false;
		//this.multiJustReleased	= true;		
		
		this.endedMultiStartTime= this.multiStartTime;
		this.endedMultiEndTime	= this.joy.curTime;
		this.endedMultiPosEnd	= endPos;	//this.pollMultiPosEnd;
		this.endedMultiPosStart	= this.multiPosStart;
		this.endedMultiDragVel 	= this.multiDragVel;
		//this.endedMultiRecentDragVel 	= this.multiRecentDragVel;
		//this.endedMultiRecentDragVelX 	= this.multiRecentDragVelX;
		//this.endedMultiRecentDragVelY	= this.multiRecentDragVelY;

		this.endedTwistAngle		= this.twistCur;
		this.endedTwistVel 			= this.twistVel;
		//this.endedTwistRecentVel 	= this.twistRecentVel;
		
		this.endedPinchDistStart	= this.pinchDistStart;
		this.endedPinchDistEnd		= this.pinchCurDist;	
		//this.endedPinchRecentDistVel= this.pinchRecentDistVel;
		this.endedPinchDistVel		= this.pinchDistVel;

		this.endedMultiMoved		= this.multiMoved;
		this.endedTwistMoved		= this.twistMoved;
		this.endedPinchMoved		= this.pinchMoved;

#if CF_EXTREME_QUERY_FUNCTIONS
		this.endedMultiExtremeDist	= this.multiExtremeCurDist;
		this.endedMultiExtremeVec	= this.multiExtremeCurVec;
		this.endedTwistExtremeAngle		= this.twistExtremeCur;
		this.endedPinchExtremeDistDelta	= this.pinchExtremeCurDist;
#endif
		}


	// -----------------
	private void OnUniStart(Vector2 startPos, Vector2 curPos)
		{
		this.uniCur			= true;
		//this.uniJustPressed	= true;		

		this.uniStartTime 		= this.joy.curTime;	
		this.uniPosStart		= startPos;	//this.pollUniPosStart;
		//this.uniPosPrev			= startPos;
		this.uniPosCur			= curPos;	//this.pollUniPosCur;

		this.uniMoved 			= false;
		this.uniJustMoved 		= false;

		this.uniExtremeDragCurVec = Vector2.zero;
		this.uniExtremeDragCurDist 	=  0;
		
#if CF_EXTREME_QUERY_FUNCTIONS
		this.uniExtremeDragPrevVec = Vector2.zero; 
		this.uniExtremeDragPrevDist =  0;
#endif

		this.uniDragVel	= Vector2.zero;
		//this.uniRecentVel	= 0;
		//this.uniRecentVelX	= 0;
		//this.uniRecentVelY	= 0;

	//	this.uniHolding 			= false;
	//	this.uniJustStartedHold 	= false;

		this.uniTotalDragPrev 		= Vector2.zero;
		this.uniTotalDragCur 		= Vector2.zero;

		}

	// ----------------
	private void OnUniEnd(Vector2 endPos, Vector2 endDeltaAccum)
		{
		this.uniTotalDragCur 	+= endDeltaAccum;	
		this.uniPosCur			= endPos;		//?!

		this.UpdateUniTouchState(true);


		this.uniCur 			= false;
		//this.uniJustReleased	= true;		
		this.endedUniPosEnd 	= endPos;	//this.poll.uniPosCur;



		// TODO : update!!!	


		this.endedUniStartTime 		= this.uniStartTime;
		this.endedUniEndTime 		= this.joy.curTime;
		this.endedUniDragVel 		= this.uniDragVel;
		//this.endedUniRecentDragVel 	= this.uniRecentVel;
		//this.endedUniRecentDragVelX 	= this.uniRecentVelX;
		//this.endedUniRecentDragVelY 	= this.uniRecentVelY;
		this.endedUniPosStart 			= this.uniPosStart;
		this.endedUniTotalDrag 			= this.uniTotalDragCur;
		this.endedUniMoved 				= this.uniMoved;

#if CF_EXTREME_QUERY_FUNCTIONS
		this.endedUniExtremeDragDist 	= this.uniExtremeDragCurDist;
		this.endedUniExtremeDragVec 	= this.uniExtremeDragCurVec;
		//this.endedUniExtremeDragX 		= this.uniExtremeDragCurX;
		//this.endedUniExtremeDragY 		= this.uniExtremeDragCurY;
#endif		
		}


	// ---------------
	private void OnPinchStart()
		{
		if (!this.pinchMoved)
			{
			this.pinchMoved = true;
			this.pinchJustMoved = true;

			if (this.startDragWhenPinching)
				this.OnMultiDragStart();

			if (this.startTwistWhenPinching)
				this.OnTwistStart();
			}
		}
	
	// ---------------
	private void OnTwistStart()	
		{
		if (!this.twistMoved)
			{
			this.twistMoved 	= true;
			this.twistJustMoved = true;
			this.twistStartRaw 	= this.twistCurRaw;
			this.twistCur 		= 0;


			if (this.startDragWhenTwisting)
				this.OnMultiDragStart();

			if (this.startPinchWhenTwisting)
				this.OnPinchStart();
			}
		}

	// --------------
	private void OnMultiDragStart()
		{
		if (!this.multiMoved)
			{
			this.multiMoved 	= true;
			this.multiJustMoved = true;

			if (this.startTwistWhenDragging)
				this.OnTwistStart();

			if (this.startPinchWhenDragging)
				this.OnPinchStart();
			}
		}



	
	// -----------------
	private void UpdateUniTouchState(bool lastUpdateMode = false)
		{
		if (lastUpdateMode)
			return;		// Nothing to do...

		// Update unified drag extremes...
		
		//this.uniExtremeDragPrevX 	= this.uniExtremeDragCurX;
		//this.uniExtremeDragPrevY 	= this.uniExtremeDragCurY;
		//this.uniExtremeDragPrevVec 	= this.uniExtremeDragCurVec;
		//this.uniExtremeDragPrevDist= this.uniExtremeDragCurDist;

		this.uniExtremeDragCurVec.x = 
			Mathf.Max(Mathf.Abs(this.uniTotalDragCur.x), this.uniExtremeDragCurVec.x);
		this.uniExtremeDragCurVec.y = 
			Mathf.Max(Mathf.Abs(this.uniTotalDragCur.y), this.uniExtremeDragCurVec.y);
		this.uniExtremeDragCurDist	= 
			Mathf.Max(this.uniTotalDragCur.magnitude, this.uniExtremeDragCurDist);
		

		
		this.uniJustMoved  = false;

		if (!this.uniMoved && (this.uniExtremeDragCurDist > 
			(this.joy.touchTapMaxDistPx))) // * this.joy.touchTapMaxDistPx)))
			{
			this.uniMoved 	= true;
			this.uniJustMoved = true;
			}
		

		//if (!this.uniMoved && (this.uniTotalDragCur.sqrMagnitude > 
		//	(this.joy.touchTapMaxDistPx * this.joy.touchTapMaxDistPx)))
		//	{
		//	this.uniMoved 	= true;
		//	this.uniJustMoved = true;
		//	}
/*
		// unified-touch "hold" state...

		if (!this.uniHolding)
			{
			if ((this.joy.curTime - this.uniStartTime) > this.joy.touchHoldMinTime)
				{
				this.uniJustStartedHold = true;
				this.uniHolding = true;
				}
			}
		else
			{
			this.uniJustStartedHold = false;
			}
*/
		

		// Update uni-touch recent velocity...
		
		if (this.uniCur)
			{
			if (TouchZone.PxPosEquals(this.uniTotalDragCur, this.uniTotalDragPrev))
				{
				if ((this.joy.curTime - this.uniLastMoveTime) > this.joy.velPreserveTime)
					this.uniDragVel = Vector2.zero;
				}
			else 
				{
				this.uniLastMoveTime = this.joy.curTime;
				this.uniDragVel = (this.uniTotalDragCur - this.uniTotalDragPrev) * this.joy.invDeltaTime;
 				}
			

			}

		}


	// ------------------
	private void UpdateMultiTouchState(bool lastUpdateMode = false)
		{
		if (lastUpdateMode)
			return;		// nothing to do at the end...
			



		// Update two-finger drag extremes...
		
		
		this.multiJustMoved = false;
		this.pinchJustMoved = false;
		this.twistJustMoved = false;

		if (this.multiCur)
			{	
			bool justDragged = false;
			bool justPinched = false;
			bool justTwisted = false;

			// Update multi-drag...

			Vector2 multiDrag = (this.multiPosCur - this.multiPosStart);
	
			this.multiExtremeCurVec.x = 
				Mathf.Max(Mathf.Abs(multiDrag.x), this.multiExtremeCurVec.x);
			this.multiExtremeCurVec.y = 
				Mathf.Max(Mathf.Abs(multiDrag.y), this.multiExtremeCurVec.y);
			this.multiExtremeCurDist = 
				Mathf.Max(multiDrag.magnitude, this.multiExtremeCurDist);
	
			
			if (!this.multiMoved && (this.multiExtremeCurDist > 
				(this.joy.touchTapMaxDistPx)) ) //(this.joy.touchTapMaxDistPx * this.joy.touchTapMaxDistPx)))
				{
				justDragged = true;

				//this.multiMoved 	= true;
				//this.multiJustMoved = true;
				}
			

			// Update pinch tracking...
	
			this.pinchJustMoved 	= false;
			this.pinchCurDist		= this.GetFingerDist();
			//this.pinchCurScale		= this.pinchCurDist / this.pinchDistStart;
	
			this.pinchExtremeCurDist	= 
				Mathf.Max(Mathf.Abs(this.pinchCurDist - this.pinchDistStart), this.pinchExtremeCurDist);

			if (!this.pinchMoved && 
				//(!this.strictPinchStart || !this.twistMoved) &&
				(pinchExtremeCurDist > this.joy.pinchMinDistPx))
				{
				justPinched = true;

				//this.pinchMoved 	= true;
				//this.pinchJustMoved = true;
				}

	



			// Update twist tracking...
	
//this.strictTwistStart
//this.ignoreTwistWhenTooClose

			this.twistJustMoved 	= false;
	//		this.twistPrevRaw		= this.twistCurRaw;
	//		this.twistPrevAbs 		= this.twistCurAbs;
	//		this.twistPrev			= this.twistCur;
			this.twistCurAbs		= this.GetFingerAbsAngle(this.twistPrevAbs);
			this.twistCurRaw		= Mathf.DeltaAngle(this.twistCurAbs, this.twistStartAbs);
	
			bool distIsSafe = (this.pinchCurDist > this.joy.twistSafeFingerDistPx);

			if (!this.twistMoved && 
				distIsSafe &&
				//(!this.strictTwistStart || (!this.pinchMoved || this.pinchJustMoved)) &&
				//(Mathf.Abs(this.twistCurRaw) > this.joy.twistThresh))
				((Mathf.Abs(this.twistCurRaw) * Mathf.Deg2Rad * 2.0f * this.pinchCurDist) > 
					this.joy.pinchMinDistPx))
				{
				justTwisted = true;

				//this.twistStartRaw 	= this.twistCurRaw;
				//this.twistMoved	 	= true;
				//this.twistJustMoved = true;

				//if (this.startPinchWhenTwisting && !this.pinchMoved)
				//	{
				//	this.pinchMoved = true;
				//	this.pinchJustMoved = true;
				//	}
				}

			if (this.twistMoved && (distIsSafe || !this.freezeTwistWhenTooClose))
				{
				this.twistCur = (this.twistCurRaw - this.twistStartRaw);
#if CF_EXTREME_QUERY_FUNCTIONS
				this.twistExtremePrev 	= this.twistExtremeCur;
#endif
				this.twistExtremeCur	= Mathf.Max(Mathf.Abs(this.twistCur), this.twistExtremeCur);
				}

			
			// Resolve detection order...
		
			int orderCode = 0;
			switch (this.gestureDetectionOrder)
				{
				case GestureDetectionOrder.TWIST_PINCH_DRAG :
					orderCode = ((0 	<< 0) | (1 		<< 3) | (2 	<< 6)); break;
				case GestureDetectionOrder.TWIST_DRAG_PINCH :
					orderCode = ((0		<< 0) | (2 		<< 3) | (1 	<< 6)); break;
				case GestureDetectionOrder.PINCH_TWIST_DRAG :
					orderCode = ((1		<< 0) | (0 		<< 3) | (2 	<< 6)); break;
				case GestureDetectionOrder.PINCH_DRAG_TWIST :
					orderCode = ((1		<< 0) | (2 		<< 3) | (0 	<< 6)); break;
				case GestureDetectionOrder.DRAG_TWIST_PINCH :
					orderCode = ((2		<< 0) | (0 		<< 3) | (1 	<< 6)); break;
				case GestureDetectionOrder.DRAG_PINCH_TWIST :
					orderCode = ((2		<< 0) | (1 		<< 3) | (0 	<< 6)); break;
				}

			
			for (int i = 0; i < 3; ++i)
				{
				int gestureCode = (orderCode >> (i * 3)) & ((1 << 3) - 1);
				switch (gestureCode)
					{
					// Twist...
					case 0 : 
						if (this.twistMoved || justTwisted)
							{
							if (this.noDragAfterTwist)
								justDragged = false;
							if (this.noPinchAfterTwist)
								justPinched = false;
							}
						break;

					// Pinch...
					case 1 : 
						if (this.pinchMoved || justPinched)
							{
							if (this.noDragAfterPinch)
								justDragged = false;
							if (this.noTwistAfterPinch)
								justTwisted = false;
							}
						break;

					// Multi-drag...
					case 2 : 
						if (this.multiMoved || justDragged)
							{
							if (this.noTwistAfterDrag)
								justTwisted = false;
							if (this.noPinchAfterDrag)
								justPinched = false;
							}
						break;
					}
				}
			


			// Officially start drag, pinch and/or twist...

			if (justDragged)
				this.OnMultiDragStart();

			if (justPinched)
				this.OnPinchStart();

			if (justTwisted)
				this.OnTwistStart();

			}


		// Update two-finger recent velocity...
		
		if (this.multiCur)
			{
			// Multi-touch drag velocity...

			if (TouchZone.PxPosEquals(this.multiPosCur, this.multiPosPrev))
				{
				if ((this.joy.curTime - this.multiLastMoveTime) > this.joy.velPreserveTime)
					this.multiDragVel = Vector2.zero;
				}
			else 
				{
				this.multiLastMoveTime = this.joy.curTime;
				this.multiDragVel 	= (this.multiPosCur - this.multiPosPrev) * this.joy.invDeltaTime;
				}
			


			// Pinch velocity...

			if (TouchZone.PxDistEquals(this.pinchCurDist, this.pinchPrevDist))
				{
				if ((this.joy.curTime - this.pinchLastMoveTime) > this.joy.velPreserveTime)
					this.pinchDistVel = 0;
				}
			else 
				{
				this.pinchLastMoveTime = this.joy.curTime;
				this.pinchDistVel 	= (this.pinchCurDist - this.pinchPrevDist) * this.joy.invDeltaTime;
				}

			// Twist velocity...
		
			if (TouchZone.TwistAngleEquals(this.twistCur, this.twistPrev))
				{
				if ((this.joy.curTime - this.twistLastMoveTime) > this.joy.velPreserveTime)
					this.twistVel = 0;
				}
			else 
				{
				this.twistLastMoveTime = this.joy.curTime;
				this.twistVel		= (this.twistCur - this.twistPrev) * this.joy.invDeltaTime; 			
				}


			}

		}


	// -------------
	override public void OnUpdate() //bool firstUpdate)
		{
		
#if UNITY_EDITOR
		this.RefreshEditorColors();
#endif

		this.fingerA.PreUpdate(); //firstUpdate);
		this.fingerB.PreUpdate(); //firstUpdate);


// ========================
// UNIFIED TOUCH (new system)

			this.uniPrev 				= this.uniCur;
			//this.uniPosPrev				= this.uniPosCur;
			this.uniTotalDragPrev 		= this.uniTotalDragCur;

#if CF_EXTREME_QUERY_FUNCTIONS
			this.uniExtremeDragPrevDist	= this.uniExtremeDragCurDist;
			this.uniExtremeDragPrevVec 	= this.uniExtremeDragCurVec;
#endif

			this.uniMidFramePressed	= false;
			this.uniMidFrameReleased= false;



			// Initial release..

			if (this.uniCur && this.pollUniReleasedInitial)
				{
				this.OnUniEnd(this.pollUniPosEnd, this.pollUniDeltaAccumAtEnd);
				}

			// Mid-frame press...

			if (this.pollUniTouched) // && (!this.pollMultiInitialState || (this.pollMultitouchId >= 0)))
				{
				this.OnUniStart(this.pollUniPosStart, this.pollUniPosCur); //, this.pollUniDeltaAccum);
				}
			
			
			// Final press or release...

			if (((this.fingerA.touchId >= 0) || (this.fingerB.touchId >= 0)) != 
				this.uniCur)
				{
				if (this.uniCur)
					this.OnUniEnd(this.pollUniPosEnd, this.pollUniDeltaAccumAtEnd);
				else
					this.OnUniStart(this.pollUniPosStart, this.pollUniPosCur); //, this.pollUniDeltaAccum);
				}

			// Check special mid-frame cases...

			this.uniMidFramePressed = 
				(!this.pollUniInitialState && this.pollUniTouched && !this.uniCur);
			this.uniMidFrameReleased = 
				(this.pollUniInitialState && this.pollUniReleasedInitial && this.uniCur);

		


			if (this.uniCur)
				{
				this.uniPosCur = this.pollUniPosCur;
				}
			
		
		
			// On finger up/down reset position delta to zero...
		
			//if (this.uniNeedReset)
			//	{
			//	this.uniNeedReset 	= false;
			//	this.uniPosPrev 	= this.uniPosCur;
			//	}
		
			//this.uniTotalDragPrev 	= this.uniTotalDragCur;
			this.uniTotalDragCur	+= this.pollUniDeltaAccum; //(this.uniPosCur - this.uniPosPrev); 
		

			// Update uni touch...

			this.UpdateUniTouchState();


			
			// Init poll state...

			this.pollUniReleasedInitial = false;
			this.pollUniReleased 		= false;
			this.pollUniTouched 		= false;
			this.pollUniInitialState 	= this.uniCur;
			this.pollUniPosCur			= 
			this.pollUniPosPrev			=
			this.pollUniPosStart		= 
			this.pollUniPosEnd			= this.uniPosCur;
			this.pollUniWaitForDblStart = false;
			this.pollUniWaitForDblEnd	= false;
			this.pollUniDeltaAccum 		= 
			this.pollUniDblEndPos		= 
			this.pollUniDeltaAccumAtEnd	= Vector2.zero;



// ================================
// ================================
// Two fingers new handling...
// ================================


			this.multiPrev 				= this.multiCur;		//??!
			this.multiPosPrev			= this.multiPosCur;
#if CF_EXTREME_QUERY_FUNCTIONS
			this.multiExtremePrevDist	= this.multiExtremeCurDist;
			this.multiExtremePrevVec 	= this.multiExtremeCurVec;
#endif
			this.pinchPrevDist 			= this.pinchCurDist;
			//this.pinchPrevScale			= this.pinchCurScale;
	
			//this.twistPrevRaw		= this.twistCurRaw;
			this.twistPrevAbs 		= this.twistCurAbs;
			this.twistPrev			= this.twistCur;

#if CF_EXTREME_QUERY_FUNCTIONS
			this.pinchExtremePrevDist 	= this.pinchExtremeCurDist;
			this.twistExtremePrev	= this.twistExtremeCur;
#endif
			
			this.multiMidFramePressed	= false;
			this.multiMidFrameReleased	= false;



			// Initial release..

			if (this.multiCur && this.pollMultiReleasedInitial)
				{
				this.OnMultiEnd(this.pollMultiPosEnd);
				}

			// Mid-frame press...

			if (this.pollMultiTouched) // && (!this.pollMultiInitialState || (this.pollMultitouchId >= 0)))
				{
				this.OnMultiStart(this.pollMultiPosStart, this.pollMultiPosCur);
				}
			
			
			// Final press or release...

			if (((this.fingerA.touchId >= 0) && (this.fingerB.touchId >= 0)) != 
				this.multiCur)
				{
				if (this.multiCur)
					this.OnMultiEnd(this.pollMultiPosEnd);
				else
					this.OnMultiStart(this.pollMultiPosStart, this.pollMultiPosCur);
				}

			// Check special mid-frame cases...

			this.multiMidFramePressed = 
				(!this.pollMultiInitialState && this.pollMultiTouched && !this.multiCur);
			this.multiMidFrameReleased = 
				(this.pollMultiInitialState && this.pollMultiReleasedInitial && this.multiCur);


			if (this.multiCur)
				{
				this.multiPosCur	= this.pollMultiPosCur;
				}


			this.UpdateMultiTouchState();

			
			// Init poll state...

			this.pollMultiReleasedInitial 	= false;
			this.pollMultiReleased 			= false;
			this.pollMultiTouched 			= false;
			this.pollMultiInitialState 		= this.multiCur;
			this.pollMultiPosCur			= 
			this.pollMultiPosStart			= 
			this.pollMultiPosEnd			= this.multiPosCur;





		// Check quick tap...


		this.justMultiDoubleTapped 	= false;
		this.justMultiTapped		= false;
		this.justMultiDelayTapped	= false;
		


		// Check two finger tap...

		//if (!this.multiCur && this.multiPrev)
		if (this.JustMultiReleased(true, true))
			{
			if (!this.endedMultiMoved && //!this.endedPinchMoved && !this.endedTwistMoved &&
				((this.endedMultiEndTime - this.endedMultiStartTime) <= 
				this.joy.touchTapMaxTime))
				{
				//bool isDoubleTap = (this.nextTapCanBeMultiDoubleTap && 
				//	((this.joy.curTime - this.lastMultiTapTime) <= 
				//	(2.0f * this.joy.touchTapMaxTime)) );
				bool isDoubleTap = (this.nextTapCanBeMultiDoubleTap && 
					((this.endedMultiStartTime - this.lastMultiTapTime) <= //.lastTapTime) <= 
					this.joy.doubleTapMaxGapTime) ); //(2.0f * this.joy.touchTapMaxTime)) );

				this.waitForMultiDelyedTap	= !isDoubleTap;
				this.justMultiDoubleTapped 	= isDoubleTap;
				this.justMultiTapped		= true;
				this.lastMultiTapPos		= this.endedMultiPosStart; //this.multiPosCur;
				this.lastMultiTapTime 		= this.joy.curTime;
				this.nextTapCanBeMultiDoubleTap	= !isDoubleTap;
				
				this.fingerA.CancelTap();
				this.fingerB.CancelTap();
				}
			else
				{
				//this.lastTapTime = -100;
				this.waitForMultiDelyedTap = false;
				this.nextTapCanBeMultiDoubleTap = false;
				}			

			//this.delyedMultiTapDetected = false;
			}

		
		// On new two finger press, stop waiting for delayed tap...

		else if (this.JustMultiPressed(true, true)) //this.multiCur && !this.multiPrev)
			{
			this.waitForMultiDelyedTap = false;
			}

		// Detect delayed multi-touch single taps...

		else 
			{
			if (this.waitForMultiDelyedTap && //this.nextTapCanBeMultiDoubleTap && !this.delyedMultiTapDetected &&
				((this.joy.curTime - this.lastMultiTapTime) > this.joy.doubleTapMaxGapTime))
				{
				this.justMultiDelayTapped		= true;
				this.waitForMultiDelyedTap 		= false;
				this.nextTapCanBeMultiDoubleTap	= true;
				}
			}

		


		// Update controller's mouse position...

		if (this.emulateMouse)
			{
			this.joy.SetInternalMousePos(this.mousePosFromFirstFinger ? 
				this.GetPos(0, TouchCoordSys.SCREEN_PX) : 
				this.GetUniPos(TouchCoordSys.SCREEN_PX));
			}


		// Update animation...

		if ((this.uniCur != this.uniPrev) && this.enabled)
			{	
			if (this.uniCur)
				this.AnimateParams(
					(this.overrideScale ? this.pressedScale : this.joy.pressedZoneScale), 
					(this.overrideColors ? this.pressedColor : this.joy.defaultPressedZoneColor), 
					(this.overrideAnimDuration ? this.pressAnimDuration : this.joy.pressAnimDuration));
			else
				this.AnimateParams(
					(this.overrideScale ? this.releasedScale : this.joy.releasedZoneScale), 
					(this.overrideColors ? this.releasedColor : this.joy.defaultReleasedZoneColor), 
					(this.touchCanceled ? this.joy.cancelAnimDuration : 
					(this.overrideAnimDuration ? this.releaseAnimDuration : this.joy.releaseAnimDuration)));
			}

		if (this.animTimer.Enabled)
			{
			this.animTimer.Update(this.joy.deltaTime);

			float t = TouchController.SlowDownEase(this.animTimer.Nt);

			this.animColor.Update(t);
			this.animScale.Update(t);

			if (this.animTimer.Completed)
				{
				this.animTimer.Disable();
				}			
			}


		}	

	

	// -------------
	//override public void OnPostUpdate() //bool firstUpdate)
	//	{
	//	this.fingerA.PostUpdate(); //firstUpdate);
	//	this.fingerB.PostUpdate(); //firstUpdate);
	//	}

				
	
	// ---------------
	override public void OnLayoutAddContent()
		{
		if (this.shape == TouchController.ControlShape.SCREEN_REGION)
			return;
		
		
		TouchController.LayoutBox layoutBox = this.joy.layoutBoxes[(int)this.layoutBoxId];

		switch (this.shape)
			{
			case TouchController.ControlShape.CIRCLE :
				layoutBox.AddContent(this.posCm, this.sizeCm.x);
				break;

			case TouchController.ControlShape.RECT :
				layoutBox.AddContent(this.posCm, this.sizeCm);
				break;
			}
		}

	// ---------------
	override public void OnLayout()
		{
		//


		// Shape dimensions...

		switch (this.shape)
			{
			case TouchController.ControlShape.CIRCLE :
			case TouchController.ControlShape.RECT :
				TouchController.LayoutBox layoutBox = this.joy.layoutBoxes[(int)this.layoutBoxId];

				this.layoutPosPx 	= layoutBox.GetScreenPos(this.posCm);
				this.layoutSizePx 	= layoutBox.GetScreenSize(this.sizeCm);
					
				this.layoutRectPx = new Rect(
					(this.layoutPosPx.x - (0.5f * this.layoutSizePx.x)),
					(this.layoutPosPx.y - (0.5f * this.layoutSizePx.y)),
					this.layoutSizePx.x,
					this.layoutSizePx.y);

				break;

			case TouchController.ControlShape.SCREEN_REGION :
			
				this.layoutRectPx 	= this.joy.NormalizedRectToPx(this.regionRect);
	
				this.layoutPosPx 	= this.layoutRectPx.center;
				this.layoutSizePx.x = this.layoutRectPx.width;
				this.layoutSizePx.y = this.layoutRectPx.height;

				this.screenRectPx = this.layoutRectPx;

				//this.screenRectPx = Rect.MinMaxRect(
				//	this.joy.GetScreenX(this.regionRect.xMin),
				//	this.joy.GetScreenY(this.regionRect.yMin),
				//	this.joy.GetScreenX(this.regionRect.xMax),
				//	this.joy.GetScreenY(this.regionRect.yMax));
				break;		
			}

		// Reset any custom positioning to automatic!

		this.posPx 			= this.layoutPosPx;
		this.sizePx 		= this.layoutSizePx;
		this.screenRectPx 	= this.layoutRectPx;


		// Reset on layout...

		this.OnReset();

		}

	
	
	// ------------------
	override public void DrawGUI()
		{	
		if (this.disableGui)
			return;

		bool pressed = this.UniPressed(true, false);

#if UNITY_EDITOR
		if (!EditorApplication.isPlayingOrWillChangePlaymode && (this.joy.previewMode == TouchController.PreviewMode.PRESSED))
			pressed = true;
#endif

		Texture2D img = (pressed ? this.pressedImg : this.releasedImg);		

		if (img != null)
			{
			GUI.depth = this.joy.guiDepth + this.guiDepth + 
				(pressed ? this.joy.guiPressedOfs : 0);

			
			/*
			Rect rect = this.screenRectPx; 
	
			if ((this.shape == TouchController.ControlShape.CIRCLE) ||	
				(this.shape == TouchController.ControlShape.RECT))	
				rect = TouchController.GetCenRect(this.posPx, this.sizePx * this.animScale.cur);
			*/
			
			Rect rect = this.GetDisplayRect(true);

			GUI.color = TouchController.ScaleAlpha(this.animColor.cur, this.joy.GetAlpha());
			GUI.DrawTexture(rect, img);
			}

		// Draw Debug GUI...

#if UNITY_EDITOR

#endif
		}

	


	

	// ---------------
	override public void TakeoverTouches(
		TouchableControl controlToUntouch
		)
		{
		if (controlToUntouch != null)
			{
			if (this.fingerA.touchId >= 0)
				controlToUntouch.OnTouchEnd(this.fingerA.touchId, true);
			if (this.fingerB.touchId >= 0)
				controlToUntouch.OnTouchEnd(this.fingerB.touchId, true);
			}
		}

	

	
	// ---------------
	public bool MultiTouchPossible()
		{
		return (this.enableSecondFinger && 
			(this.fingerA.touchId >= 0) && 
			(this.fingerB.touchId < 0) &&
			(!this.strictTwoFingerStart ||
				((this.joy.curTime - this.fingerA.startTime) < this.joy.strictMultiFingerMaxTime))
			//.touchTapMaxTime))
			);
		}


	// ----------------
	override public TouchController.HitTestResult HitTest(Vector2 touchPos, int touchId)
		{
		if ((!this.enabled || !this.visible) ||
			((this.fingerA.touchId >= 0) && 
			(!this.enableSecondFinger || ((this.fingerB.touchId >= 0) ||
				(this.strictTwoFingerStart && 
					(!this.fingerA.pollTouched &&
						((this.joy.curTime - this.fingerA.startTime) > 
							this.joy.strictMultiFingerMaxTime)))))) ||
			(touchId == this.fingerA.touchId) ||
			(touchId == this.fingerB.touchId))
			{
			return new TouchController.HitTestResult(false); //TouchController.HIT_TEST_OUT_OF_RANGE;
			}

		TouchController.HitTestResult hitResult;

		switch (this.shape)
			{
			case TouchController.ControlShape.CIRCLE :
				hitResult = this.joy.HitTestCircle(this.posPx, 0.5f * this.sizePx.x, touchPos, true);
				break;

			case TouchController.ControlShape.RECT :
				hitResult = this.joy.HitTestBox(this.posPx, this.sizePx, touchPos, true);
				break;

			case TouchController.ControlShape.SCREEN_REGION :	
				hitResult = this.joy.HitTestRect(this.screenRectPx, touchPos, true);
				break;

			default :
				hitResult = new TouchController.HitTestResult(false);
				break;
//return (this.screenRectPx.Contains(touchPos) ? 
				//	(TouchController.HIT_TEST_MIN_DIST + 0.01f) : 
				//	TouchController.HIT_TEST_OUT_OF_RANGE);
				
			}
		
		hitResult.prio 		= this.prio;
		hitResult.distScale = this.hitDistScale;

		return hitResult;
		//return TouchController.HIT_TEST_OUT_OF_RANGE;
		}

		


	// ---------------
	override public TouchController.EventResult OnTouchStart(
		int touchId, Vector2 pos)
		{
		Finger finger = (	(this.fingerA.touchId < 0) ? this.fingerA :
							(this.fingerB.touchId < 0) ? this.fingerB : null);
	
		if (finger == null)
			return TouchController.EventResult.NOT_HANDLED;
		
		// force unified touch delta reset...

		//this.uniNeedReset = true;

	
		this.touchCanceled = false;		


		Finger otherFinger = ((finger == this.fingerA) ? this.fingerB : this.fingerA);
		
		finger.touchId 			= touchId;
		finger.touchVerified	= true;

		//finger.pollClicked		= true;
		//finger.midFrameAccum	= true;
		finger.touchPos			= pos;
		

		finger.pollTouched 			= true;
		finger.pollPosStart			= pos;
		finger.pollPosCur 			= pos;
		
//Debug.Log("Finger start ["+Time.frameCount+"] F["+((finger==this.fingerA)?"A":"B")+"]");

		// Set unified touch start...

		if (otherFinger.touchId < 0)
			{
			this.pollUniTouched 		= true;
			this.pollUniPosStart		= pos;
			this.pollUniPosCur 			= pos;
			this.pollUniWaitForDblStart = true;
			this.pollUniWaitForDblEnd 	= false;
			this.pollUniDeltaAccum		= Vector2.zero;
			}
		
		// Set two-finger start...

		else
			{

			otherFinger.CancelTap();


			this.pollMultiTouched 	= true;
			this.pollMultiPosStart =
			this.pollMultiPosCur	= (this.fingerA.pollPosCur + this.fingerB.pollPosCur) / 2.0f; //this.GetCenterPos(); 

			this.pollUniPosCur 		= this.pollMultiPosCur; //GetCenterPos();
			if (this.pollUniWaitForDblStart)
				{
				this.pollUniPosStart 			= this.pollUniPosCur;
				this.pollUniWaitForDblStart  	= false;
				this.pollUniWaitForDblEnd 		= true;
				}
			}
		
		// Reset uni poll prev to skip delta on finger count change...

		this.pollUniPosPrev = this.pollUniPosCur;	

	


		return (this.nonExclusiveTouches ? TouchController.EventResult.SHARED :
			TouchController.EventResult.HANDLED);
		}
		

	// ---------------
	override public TouchController.EventResult OnTouchEnd(int touchId, bool canceled = false) //, Vector2 pos)
		{
		Finger finger = ((this.fingerA.touchId == touchId) ? this.fingerA : 	
			(this.fingerB.touchId == touchId) ? this.fingerB : null);
		
		if (finger == null)
			return TouchController.EventResult.NOT_HANDLED;
		
		// force unified touch delta reset...

		//this.uniNeedReset = true;


		Finger otherFinger = ((finger == this.fingerA) ? this.fingerB : this.fingerA);
	
		//int baseEventFlags = 0;
		//if (otherFinger.touchId >= 0)
		//	baseEventFlags |= TouchController.TOUCH_END_WAS_SECOND_FINGER;


		// Disable this finger...

		//thisFinger.posCur			= pos;
		finger.touchId 			= -1;
		finger.touchVerified 	= true;


		if (!finger.pollReleased)
			{
			finger.pollReleased = true;
			finger.pollPosEnd	= finger.pollPosCur;

			if (finger.pollInitialState)
				finger.pollReleasedInitial = true;			
			}

		//else if (finger.pollTouched)
		//	{	
		//	finger.pollReleasedMidFrame = true;
		//	}

		finger.pollTouched = false;

	

		// Update unified touch release state...
		
		if (otherFinger.touchId >= 0)
			{
			this.pollUniPosCur = otherFinger.pollPosCur;
			// TODO : mark skip delta

			this.pollUniWaitForDblEnd 	= true;
			this.pollUniDblEndPos		= (this.fingerA.pollPosCur + this.fingerB.pollPosCur) / 2.0f; //this.GetCenterPos();
			}

		else
			{
			if (!this.pollUniReleased)
				{
				this.pollUniReleased	= true;

				if (this.pollUniWaitForDblEnd)
					{
					this.pollUniPosEnd			= this.pollUniDblEndPos;
					this.pollUniWaitForDblEnd 	= false;
					}
				else
					{
					this.pollUniPosEnd		= this.pollUniPosCur;
					}

				this.pollUniDeltaAccumAtEnd = this.pollUniDeltaAccum;
				this.pollUniDeltaAccum		= Vector2.zero;

				if (this.pollUniInitialState)
					this.pollUniReleasedInitial = true;			
				}
	
			this.pollUniTouched = false;

			}
		
		// Reset to skip delta calc...

		this.pollUniPosPrev = this.pollUniPosCur;

		
		// Update two fingers release...
		
		if (otherFinger.touchId >= 0)
			{
			if (!this.pollMultiReleased)
				{
				this.pollMultiReleased	= true;
				this.pollMultiPosEnd	= this.pollMultiPosCur;
	
				if (this.pollMultiInitialState)
					this.pollMultiReleasedInitial = true;			
				}
	
			this.pollMultiTouched = false;
			}
	

		// Reorder the fingers, so the other (potentially still active) is first...

		//this.fingerA = otherFinger;
		//this.fingerB = thisFinger;

		return (this.nonExclusiveTouches ? TouchController.EventResult.SHARED :
			TouchController.EventResult.HANDLED);
		}


	// ---------------
	override public TouchController.EventResult OnTouchMove(int touchId, Vector2 pos)
		{
		Finger finger = ((this.fingerA.touchId == touchId) ? this.fingerA : 	
			(this.fingerB.touchId == touchId) ? this.fingerB : null);
		if (finger == null)
			return TouchController.EventResult.NOT_HANDLED;
			
		Finger otherFinger = ((finger == this.fingerA) ? this.fingerB : this.fingerA);
			
		//finger.touchPos			= pos;
		//finger.posCur 			= pos;
		
		finger.touchVerified 	= true;
		finger.pollPosCur		= pos;


		// Update unified and two-fingers touch poll state...
		
		//Vector2 uniPosPrev = this.pollUniPosCur;

		if (otherFinger.touchId >= 0)
			{
			this.pollMultiPosCur = (this.fingerA.pollPosCur + this.fingerB.pollPosCur) / 2.0f;
			this.pollUniPosCur = this.pollMultiPosCur;
			}
		else
			{
			this.pollUniPosCur = pos;
			}
	
		// Update unified touch delta accum...

		if (this.pollUniPosCur != this.pollUniPosPrev) //uniPosPrev)
			{
			this.pollUniWaitForDblEnd 	= false;
			this.pollUniWaitForDblStart = false;	
		
			this.pollUniDeltaAccum += (this.pollUniPosCur - this.pollUniPosPrev); //uniPosPrev);
			this.pollUniPosPrev = this.pollUniPosCur;
			}





		return (this.nonExclusiveTouches ? TouchController.EventResult.SHARED :
			TouchController.EventResult.HANDLED);
		}
	
	// ---------------
	/// Get two-finger center poition
	// ---------------
	private Vector2 GetCenterPos()
		{
		return ((this.fingerA.posCur + this.fingerB.posCur) * 0.5f);
		}
	
	// ---------------
	/// Get two-finger distance
	// ---------------
	private float GetFingerDist()
		{
		return Mathf.Max(MIN_PINCH_DIST_PX, 
			Vector2.Distance(this.fingerA.posCur, this.fingerB.posCur));
		}

	// ---------------
	/// Get two-finger angle
	// ---------------
	private float GetFingerAbsAngle(float lastAngle = 0)
		{
		Vector2 d = (this.fingerB.posCur - this.fingerA.posCur);
		if (d.sqrMagnitude < 0.00001f)
			return lastAngle;

		d.Normalize();

		return Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg;
		}

	
	

	// ---------------
	private Vector2 TransformPos(
		Vector2 					screenPosPx, 
		TouchCoordSys 	posType,
		bool						deltaMode)
		{
		Vector2 v = screenPosPx;
		
		if (!deltaMode && (
			(posType == TouchCoordSys.LOCAL_CM) || 
			(posType == TouchCoordSys.LOCAL_INCH) || 
			(posType == TouchCoordSys.LOCAL_NORMALIZED) || 
			(posType == TouchCoordSys.LOCAL_PX)) )
			{
			v.x -= this.screenRectPx.xMin;
			v.y -= this.screenRectPx.yMin;
			} 
		

		switch (posType)
			{
			case TouchCoordSys.SCREEN_PX :
			case TouchCoordSys.LOCAL_PX :
				return v;


			case TouchCoordSys.LOCAL_CM :
			case TouchCoordSys.SCREEN_CM :
				return (v / this.joy.GetDPCM());

			case TouchCoordSys.LOCAL_INCH :
			case TouchCoordSys.SCREEN_INCH :
				return (v / this.joy.GetDPI());

			case TouchCoordSys.SCREEN_NORMALIZED :
				v.x /= this.joy.GetScreenWidth();
				v.y /= this.joy.GetScreenHeight();
				return v;

			case TouchCoordSys.LOCAL_NORMALIZED :
				v.x /= this.screenRectPx.width;
				v.y /= this.screenRectPx.height;
				return v;
			}

		return v;
		}

	
	// ---------------
	private float TransformPos(
		float 						screenPosPx, 
		TouchCoordSys 	posType)
		{
		float v = screenPosPx;
			

		switch (posType)
			{
			case TouchCoordSys.SCREEN_PX :
			case TouchCoordSys.LOCAL_PX :
				return v;


			case TouchCoordSys.LOCAL_CM :
			case TouchCoordSys.SCREEN_CM :
				return (v / this.joy.GetDPCM());

			case TouchCoordSys.LOCAL_INCH :
			case TouchCoordSys.SCREEN_INCH :
				return (v / this.joy.GetDPI());

			case TouchCoordSys.SCREEN_NORMALIZED :
				v /= Mathf.Max(this.joy.GetScreenWidth(), this.joy.GetScreenHeight());
				return v;

			case TouchCoordSys.LOCAL_NORMALIZED :
				v /= this.screenRectPx.width;
				//v.y /= this.screenRectPx.height;
				return v;
			}

		return v;
		}

	// ---------------
	private float TransformPosX(
		float 						screenPosPx, 
		TouchCoordSys 	posType)
		{
		float v = screenPosPx;
			

		switch (posType)
			{
			case TouchCoordSys.SCREEN_PX :
			case TouchCoordSys.LOCAL_PX :
				return v;


			case TouchCoordSys.LOCAL_CM :
			case TouchCoordSys.SCREEN_CM :
				return (v / this.joy.GetDPCM());

			case TouchCoordSys.LOCAL_INCH :
			case TouchCoordSys.SCREEN_INCH :
				return (v / this.joy.GetDPI());

			case TouchCoordSys.SCREEN_NORMALIZED :
				v /= this.joy.GetScreenWidth();
				return v;

			case TouchCoordSys.LOCAL_NORMALIZED :
				v /= this.screenRectPx.width;
				//v.y /= this.screenRectPx.height;
				return v;
			}

		return v;
		}


	// ---------------
	private float TransformPosY(
		float 						screenPosPx, 
		TouchCoordSys 	posType)
		{
		float v = screenPosPx;
			

		switch (posType)
			{
			case TouchCoordSys.SCREEN_PX :
			case TouchCoordSys.LOCAL_PX :
				return v;


			case TouchCoordSys.LOCAL_CM :
			case TouchCoordSys.SCREEN_CM :
				return (v / this.joy.GetDPCM());

			case TouchCoordSys.LOCAL_INCH :
			case TouchCoordSys.SCREEN_INCH :
				return (v / this.joy.GetDPI());

			case TouchCoordSys.SCREEN_NORMALIZED :
				v /= this.joy.GetScreenHeight();
				return v;

			case TouchCoordSys.LOCAL_NORMALIZED :
				v /= this.screenRectPx.height;
				return v;
			}

		return v;
		}

	

	// Compare pixel-positions ---------------------
	static public bool PxPosEquals(Vector2 p0, Vector2 p1)
		{
		return ((p0 - p1).sqrMagnitude < PIXEL_POS_EPSILON_SQR);
		}

	// ----------------
	static public bool PxDistEquals(float d0, float d1)
		{
		return (Mathf.Abs(d0 - d1) < PIXEL_DIST_EPSILON);	
		}

	// ---------------
	static public bool TwistAngleEquals(float a0, float a1)
		{
		return (Mathf.Abs(Mathf.DeltaAngle(a0, a1)) < TWIST_ANGLE_EPSILON); 
		}






	// ----------------
	private  class Finger
		{
		private TouchZone zone;

		public	int 	touchId;
		public	bool	touchVerified;

		public	Vector2 touchPos,
						startPos,
						posPrev,
						posCur;
		public 	float	startTime;
		public bool		//midFrameAccum,	// when polling this will hold mid-frame touch (should not be used by state queries)
						//midFrameClick,	// official mid-frame flag
						moved,			// gone over static-threshold (at least at one time)	
						justMoved,
						//justTapped,
						//justPressed,
						//justReleased,
						prevState,		// prev frame touched state
						curState;		// current trouched state
						//prePollState; 
						//pollClicked;

		public Vector2	extremeDragCurVec,
						extremeDragPrevVec;
		public float	extremeDragCurDist,
						extremeDragPrevDist;
			
		public float 	lastMoveTime;
		public Vector2	dragVel;			// current frame velocity in pixels per second

		//public float	recentDragVel;
		//public Vector2	recentDragVelVec;
		
		public bool		endedMoved,
						endedWasTapCanceled;
		public float	endedStartTime,
						endedEndTime;
		//				endedRecentVel;
		//public Vector2	endedRecentVelVec;
		public Vector2	endedDragVel;
		public Vector2	endedPosStart,
						endedPosEnd;
		public Vector2	endedExtremeDragVec;
		public float	endedExtremeDragDist;

		private bool 	justTapped,		
						justDoubleTapped,
						justDelayTapped,
						waitForDelyedTap;	
		private float	lastTapTime;
		private bool	nextTapCanBeDoubleTap;
		private Vector2	lastTapPos;
		private bool	tapCanceled;


		
		public bool		midFrameReleased,
						midFramePressed;

		public bool		pollInitialState,
						pollReleasedInitial,
						pollTouched,
						pollReleased;
		public Vector2	pollPosEnd,
						pollPosStart,	
						pollPosCur;

	
		// --------------
		public Finger(TouchZone tzone)
			{
			this.zone 		= tzone;

			this.Reset();
			}
		

		// ----------------
		public bool JustPressed(bool includeMidFramePress, bool includeMidFrameRelease)
			{
			return ((this.curState && !this.prevState) || 
				(includeMidFramePress && this.midFramePressed) ||
				(includeMidFrameRelease && this.midFrameReleased) );
			}

		// ----------------
		public bool JustReleased(bool includeMidFramePress, bool includeMidFrameRelease)
			{
			return ((!this.curState && this.prevState) || 
				(includeMidFramePress && this.midFramePressed) ||
				(includeMidFrameRelease && this.midFrameReleased) );
			}

		

		// ---------------
		public bool Pressed(
			bool includeMidFramePress, 	
			bool returnFalseOnMidFrameRelease)
			{
			return ((this.curState || (includeMidFramePress && this.midFramePressed)) &&
				(!returnFalseOnMidFrameRelease || !this.midFrameReleased)); 
				//.justPressed));	
			}
		

		// -------------------
		public bool JustTapped(bool onlyOnce = false)
			{
			return (onlyOnce ? this.justDelayTapped : this.justTapped);	
			}
	
		// -----------------
		public bool JustDoubleTapped()
			{
			return this.justDoubleTapped;
			}
	
		// ---------------
		public Vector2 GetTapPos(TouchCoordSys	cs)
			{
			//return TransformPos(this.lastTapPos, cs, false);
			return this.zone.TransformPos(this.lastTapPos, cs, false);
			}
		
	

		// ---------------
		//public bool JustTapped()
		//	{
		//	}
	
		
		// ----------------	
		public void OnTouchStart(Vector2 startPos, Vector2 curPos)
			{
			this.startTime		= this.zone.joy.curTime;
			this.startPos		= startPos;	//this.pollPosStart;
			this.posPrev		= startPos;	//this.pollPosStart;		// TODO : ?
			this.posCur			= curPos;	//this.pollPosCur;
			//this.justPressed	= true;
			this.curState		= true;
			this.tapCanceled 	= false;

			this.moved			= false;
			this.justMoved		= false;

			this.lastMoveTime	= 0;
			this.dragVel		= Vector2.zero;

			//this.recentDragVel	= 0;
			//this.recentDragVelVec = Vector2.zero;
			this.dragVel		= Vector2.zero;

			this.extremeDragCurVec = 
			this.extremeDragPrevVec = Vector2.zero; 
			this.extremeDragCurDist = 
			this.extremeDragPrevDist = 0; 


			}

		// ----------------
		public void OnTouchEnd(Vector2 endPos)
			{
			this.posCur = endPos;
			
//Debug.Log("End["+Time.frameCount+"] cur["+this.posCur+"] prev["+this.posPrev+"]");
			this.UpdateState(true);

			this.endedMoved			= this.moved;
			this.endedStartTime 	= this.startTime;
			this.endedEndTime		= this.zone.joy.curTime;
			this.endedPosStart		= this.startPos;
			this.endedPosEnd		= endPos; //this.pollPosEnd;	//.posCur;
			this.endedDragVel		= this.dragVel;
			//this.endedRecentVel		= this.recentDragVel;
			//this.endedRecentVelVec	= this.recentDragVelVec;
			this.endedExtremeDragVec= this.extremeDragCurVec;
			this.endedExtremeDragDist= this.extremeDragCurDist;

			//this.justReleased		= true;
			this.endedWasTapCanceled = this.tapCanceled;
			

			this.curState 			= false;		// TODO : set this BEFORE update?!
			}

		// ---------------
		public void Reset()
			{
			this.touchId 		= -1;
			this.curState 		= false;
			this.prevState 		= false;
			//this.midFrameClick	= false;
			//this.midFrameAccum	= false;
			this.moved 			= false;
			this.justMoved 		= false;
			this.touchVerified 	= true;
			
			this.dragVel		= Vector2.zero;

			//this.justReleased 			= false;
			//this.justPressed			= false;
			this.pollInitialState 		= false;
			this.pollReleasedInitial 	= false;
			this.pollTouched 			= false;
			this.pollReleased			= false;

			this.tapCanceled 			= false;
			this.lastTapPos 			= Vector2.zero;
			this.lastTapTime 			= -100;
			this.nextTapCanBeDoubleTap 	= false;
			}

		// --------------
		public void OnPrePoll()
			{
			//this.prePollState 	= (this.touchId >= 0);
			//this.midFrame 		= false;
			this.touchVerified 	= false;
			
			//this.pollInitialState 		= this.curState;
			//this.pollReleasedInitial 	= false;
			//this.pollReleasedMidFrame 	= false;
			//this.pollTouched 			= false;
			}

		// ---------------
		private void UpdateState(bool lastUpdateMode = false)
			{
			// Extreme detection...
			
			this.justMoved = false;

			//this.extremeDragPrevVec		= this.extremeDragCurVec;
			//this.extremeDragPrevDist 	= this.extremeDragCurDist;

			Vector2 drag = (this.posCur - this.startPos);
	
			this.extremeDragCurVec.x= Mathf.Max(Mathf.Abs(drag.x), this.extremeDragCurVec.x);
			this.extremeDragCurVec.y= Mathf.Max(Mathf.Abs(drag.y), this.extremeDragCurVec.y);
			this.extremeDragCurDist = Mathf.Max(drag.magnitude, this.extremeDragCurDist);
	
			
			if (!this.moved && (this.extremeDragCurDist > 
				(this.zone.joy.touchTapMaxDistPx)) ) //(this.joy.touchTapMaxDistPx * this.joy.touchTapMaxDistPx)))
				{
				this.moved 	= true;
				this.justMoved = true;
				}
			

			if (lastUpdateMode)
				return;

			// Update recent vel...

			if (this.curState)
				{
				//this.recentDragVel = this.zone.joy.CalcRecentVel(this.recentDragVel, 	
				//	(this.posCur - this.posPrev).magnitude);
				//this.recentDragVelVec.x = this.zone.joy.CalcRecentVel(this.recentDragVelVec.x, 	
				//	(this.posCur.x - this.posPrev.x));
				//this.recentDragVelVec.y = this.zone.joy.CalcRecentVel(this.recentDragVelVec.y, 	
				//	(this.posCur.y - this.posPrev.y));
				if (TouchZone.PxPosEquals(this.posCur, this.posPrev))
					{
					if ((this.zone.joy.curTime - this.lastMoveTime) > this.zone.joy.velPreserveTime)
						this.dragVel = Vector2.zero;

					}
				else 
					{
					this.lastMoveTime = this.zone.joy.curTime;
					this.dragVel = (this.posCur - this.posPrev) * this.zone.joy.invDeltaTime;
					}
				}	
			else
				{
				this.dragVel = Vector2.zero;	// TODO ??
				}
			
			}

		// -------------
		public void PreUpdate() //bool firstUpdate)
			{
			Finger finger = this;
			
			// Update...

			finger.prevState 			= finger.curState;
			finger.posPrev				= finger.posCur;
			finger.extremeDragPrevDist 	= finger.extremeDragCurDist;
			finger.extremeDragPrevVec 	= finger.extremeDragCurVec;
			
			finger.midFramePressed	= false;
			finger.midFrameReleased	= false;



			// Finger released..

			if (finger.curState && finger.pollReleasedInitial)
				{
				finger.OnTouchEnd(this.pollPosEnd);
//				finger.endedStartTime 	= finger.startTime;
//				finger.endedEndTime		= this.joy.curTime;
//				finger.endedPosStart	= finger.startPos;
//				finger.endedPosEnd		= finger.posCur;
//				finger.endedRecentVel	= finger.recentDragVel;
//				finger.justReleased		= true;
				
				//finger.curState = false;
				//this.justReleased = true;
				}

			// Finger pressed...

			if (finger.pollTouched && (!finger.pollInitialState || (finger.touchId >= 0)))
				{
				finger.OnTouchStart(this.pollPosStart, this.pollPosCur);
				
				//finger.curState = true;
				//finger.justPressed = true;
				}
			
			
			// Final press or release...

			if ((finger.touchId >= 0) != this.curState)
				{
				if (this.curState)
					this.OnTouchEnd(this.pollPosEnd);	
				else
					this.OnTouchStart(this.pollPosStart, this.pollPosCur);	// TODO : OR CURR STORED?!
				}

			//if (finger.touchId < 0)
			//	{
			//	finger.OnTouchRelease();
			//	}


			// Update...
	
			//finger.curState		= (finger.touchId >= 0);

			if (finger.touchId >= 0)
				{
				finger.posCur = finger.pollPosCur;
				}

			
			// Check special mid-frame cases...

			this.midFramePressed = 
				(!this.pollInitialState && this.pollTouched && !this.curState);
			this.midFrameReleased = 
				(this.pollInitialState && this.pollReleasedInitial && this.curState);


			// Update state...

			this.UpdateState();
			
	
			


// ===========================
// TAP detection
// ===========================
	
		this.justDelayTapped 	= false;
		this.justTapped 		= false;
		this.justDoubleTapped 	= false;


		if (this.JustReleased(true, true)) //(false))
			{
			if (!this.endedMoved && !this.endedWasTapCanceled && 
				((this.zone.joy.curTime - this.endedStartTime) <= this.zone.joy.touchTapMaxTime))
				{
				bool isDoubleTap = (this.nextTapCanBeDoubleTap && 
					((this.endedStartTime - this.lastTapTime) <= //.lastTapTime) <= 
					this.zone.joy.doubleTapMaxGapTime) ); //(2.0f * this.joy.touchTapMaxTime)) );
				
				this.waitForDelyedTap		= !isDoubleTap;
				this.justDoubleTapped 		= isDoubleTap;
				this.justTapped				= true;
				this.lastTapPos				= this.endedPosStart; //this.uniPosPrev;
				this.lastTapTime 			= this.zone.joy.curTime;
				this.nextTapCanBeDoubleTap	= !isDoubleTap;
				}
			else
				{
//Debug.Log("\tnot a tap! : " + (this.uniMoved ? " Moved" : "") + 
//	((this.GetTouchDuration() <= this.joy.touchTapMaxTime) ? "" : (" Dur: " + this.GetTouchDuration())));
				//this.lastTapTime = -100;
				this.waitForDelyedTap = false;
				this.nextTapCanBeDoubleTap = false;
				}			
			}
		
		// Stop waiiting for delayed tap on press...

		else if (this.JustPressed(false, false)) //this.uniCur && !this.uniPrev)
			{
			this.waitForDelyedTap = false;
			}

		// Detect delayed single taps...

		else
			{
			if (this.waitForDelyedTap && //this.nextTapCanBeDoubleTap && !this.delyedTapDetected &&
				((this.zone.joy.curTime - this.lastTapTime) > 
					this.zone.joy.doubleTapMaxGapTime))
				{
				this.justDelayTapped		= true;
				this.waitForDelyedTap 		= false;
				this.nextTapCanBeDoubleTap 	= false;
				}
			}



// =========================

			// Init poll state...

			//finger.pollReleasedInitial	= false;
			//finger.pollTouched			= false;

			this.pollInitialState 		= this.curState;
			this.pollReleasedInitial 	= false;
			this.pollReleased 			= false;
			this.pollTouched 			= false;
			this.pollPosCur				= this.posCur;
			this.pollPosStart			= this.posCur;
			this.pollPosEnd				= this.posCur;

			//if (!firstUpdate)
			//	this.midFrame = false;
			}

		//// -----------
		//public void PostUpdate() //bool firstUpdate)
		//	{
		//	//this.clicked = false;
		//	}


		// -------------
		public void CancelTap()
			{
			this.waitForDelyedTap		= false;	// ?!

			this.tapCanceled 			= true;
			this.nextTapCanBeDoubleTap 	= false;
			}
		}
	

	// End doxygen hidden docs...
	/// \endcond


	}

//}

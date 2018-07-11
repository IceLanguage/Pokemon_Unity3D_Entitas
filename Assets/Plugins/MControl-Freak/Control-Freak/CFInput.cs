// -----------------------------------------------
// Control Freak - The Ultimate Virtual Controller
// Copyright (C) 2013 Dan's Game Tools
// -----------------------------------------------

// Comment-out the line below to enable "unknown axis name" exceptions thrown by Input.GetButton() and Input.GetAxis()
  
#define CATCH_UNITY_AXIS_EXCEPTIONS		


//#if UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8 || UNITY_BLACKBERRY
//#define UNITY_MOBILE
//#endif


using UnityEngine;


// ----------------------------
/// \brief Unity Input class wrapper.
/// 
/// \p Use this class to easily develop cross-platform games. 
/// \p When 'ctrl' static variable is set to null or assigned controller is disabled, 
/// or when given KeyCode, button or axis name isn't supported by active controller -  
/// CFInput will return result of 'normal' Input function.
/// \note To bind controller to mouse buttons, enable control's GetKey() and choose <i>Mouse0</i> (LMB), <i>Mouse1</i> (RMB) or <i>Mouse2</i> (MMB) KeyCodes.
///
/// \note Use CFInput.ControllerActive() to determinate if CF is currently used.
///
/// \note Keep in mind the physical limitations of touch-screen controls - for example, try to avoid situations when the player is required to hold two buttons at the same time (like in Bootcamp demo - Aiming (RMB) and Shooting (LMB)). 


// -----------------------------
public class CFInput
	{
	
	static public TouchController	ctrl;	///<	Active TouchController instance.

	
	// ------------------
	/// Returns true if there's an active, enabled Control Freak controller in the scene.
	// ------------------
	static public bool ControllerActive()
		{
		return ((ctrl != null) && (ctrl.enabled));		
		}


	// -------------------
	/// \brief Emulated mouse position.
	///
	/// If there's an active controller in the scene, it's emulated mouse position will be returned (See \ref sectEditorZones).
	// -------------------
	static public Vector3 mousePosition
		{
		get {
			if (ControllerActive())
				return ctrl.GetMousePos();

			return Input.mousePosition;
			}
		}

	
	


	// ------------------
	/// \brief Input.GetKey() replacement.
	/// 
	/// Returns true when active controller's control with matching KeyCode OR an actual KeyCode is pressed.
	// ------------------
	static public bool GetKey(KeyCode key)
		{	
		if (ControllerActive())
			{
			bool keySupported = false;
			bool keyState = ctrl.GetKeyEx(key, out keySupported);
			if (keySupported)
				return keyState;
			}

		return Input.GetKey(key);
		}
	

	// ------------------
	/// \brief Input.GetKeyDown() replacement.
	/// 
	/// Returns true when active controller's control with matching KeyCode OR an actual KeyCode was just pressed.
	// ------------------
	static public bool GetKeyDown(KeyCode key)
		{
		if (ControllerActive())
			{
			bool keySupported = false;
			bool keyState = ctrl.GetKeyDownEx(key, out keySupported);
			if (keySupported)
				return keyState;
			}

		return Input.GetKeyDown(key);
		}
	

	// ------------------
	/// \brief Input.GetKeyUp() replacement.
	/// 
	/// Returns true when active controller's control with matching KeyCode OR an actual KeyCode was just released.
	// ------------------
	static public bool GetKeyUp(KeyCode key)
		{
		if (ControllerActive())
			{
			bool keySupported = false;
			bool keyState = ctrl.GetKeyUpEx(key, out keySupported);
			if (keySupported)
				return keyState;
			}

		return Input.GetKeyUp(key);
		}
	

	// ------------------
	/// \brief Input.GetButton() replacement.
	/// 
	/// Returns true when active controller's control with matching GetButton name OR an actual button/axis is pressed.
	// ------------------
	static public bool GetButton(string axisName)
		{
		if (ControllerActive())
			{
			bool buttonSupported = false;
			bool buttonState = ctrl.GetButtonEx(axisName, out buttonSupported);
			if (buttonSupported)
				return buttonState;
			}
		 
#		if CATCH_UNITY_AXIS_EXCEPTIONS
		try {
#		endif

			return Input.GetButton(axisName);

#		if CATCH_UNITY_AXIS_EXCEPTIONS
			} catch (UnityException ) {}

		return false;
#		endif
		}
	

	// ------------------
	/// \brief Input.GetButtonDown() replacement.
	/// 
	/// Returns true when active controller's control with matching GetButton name OR an actual button/axis was just pressed.
	// ------------------
	static public bool GetButtonDown(string axisName)
		{
		if (ControllerActive())
			{
			bool buttonSupported = false;
			bool buttonState = ctrl.GetButtonDownEx(axisName, out buttonSupported);
			if (buttonSupported)
				return buttonState;
			}

#		if CATCH_UNITY_AXIS_EXCEPTIONS
		try {
#		endif

			return Input.GetButtonDown(axisName);

#		if CATCH_UNITY_AXIS_EXCEPTIONS
			} catch (UnityException ) {}

		return false;
#		endif	
		}
	

	// ------------------
	/// \brief Input.GetButtonUp() replacement.
	/// 
	/// Returns true when active controller's control with matching GetButton name OR an actual button/axis was just released.
	// ------------------
	static public bool GetButtonUp(string axisName)
		{
		if (ControllerActive())
			{
			bool buttonSupported = false;
			bool buttonState = ctrl.GetButtonUpEx(axisName, out buttonSupported);
			if (buttonSupported)
				return buttonState;
			}

#		if CATCH_UNITY_AXIS_EXCEPTIONS
		try	{
#		endif

			return Input.GetButtonUp(axisName);

#		if CATCH_UNITY_AXIS_EXCEPTIONS
			} catch (UnityException ) {}

		return false;
#		endif
		}



	// ------------------
	/// \brief Input.GetAxis() replacement.
	/// 
	/// Returns value of active controller's named axis (or sum of them, if there's more than one sharing the same GetAxis name).\n
	/// When axis of given name is not supported by active controller, this function will return value of real Input.GetAxis().
	// ------------------
	static public float GetAxis(string axisName)
		{
		if (ControllerActive())
			{
			bool supportedAxis = false;
			float v = ctrl.GetAxisEx(axisName, out supportedAxis);
			if (supportedAxis)
				return v;
			}

#		if CATCH_UNITY_AXIS_EXCEPTIONS
		try	{
#		endif

			return Input.GetAxis(axisName);

#		if CATCH_UNITY_AXIS_EXCEPTIONS
			} catch (UnityException ) {}

		return 0;
#		endif
		}
	

	// ------------------
	/// \brief Input.GetAxisRaw() replacement.
	/// 
	/// Returns value of active controller's named axis (or sum of them, if there's more than one sharing the same GetAxis name).\n
	/// When axis of given name is not supported by active controller, this function will return value of real Input.GetAxisRaw().
	// ------------------
	static public float GetAxisRaw(string axisName)
		{
		if (ControllerActive())
			{
			bool supportedAxis = false;
			float v = ctrl.GetAxisEx(axisName, out supportedAxis);
			if (supportedAxis)
				return v;
			}

#		if CATCH_UNITY_AXIS_EXCEPTIONS 
		try { 
#		endif

			return Input.GetAxisRaw(axisName);

#		if CATCH_UNITY_AXIS_EXCEPTIONS
			} catch (UnityException ) {}

		return 0;
# 		endif
		}

	

	// ---------------
	// ---------------
	static public float GetAxisPx(
		string 	axisName, 
		float 	refResolution		= 1280.0f,	///< Screen resolution for which the original mouse code was optimized 
		float 	maxDragInches		= 1.0f		///< Touchscreen drag distance (in inches) equivalent to a full side-to-side mouse cursor movement (refResolution) 
		)
		{	
		if (CFInput.ControllerActive() && TouchController.IsSupported())
			{
			float dpi = CFInput.ctrl.GetActualDPI();
			float v = CFInput.GetAxis(axisName);
			return ((v / (dpi * maxDragInches)) * refResolution);
			}		
		else
			{
			int curResolution = Screen.currentResolution.width;
			return (CFInput.GetAxis(axisName) * 
				((curResolution == 0) ? 1.0f : (refResolution / curResolution)));
			}			
		}


	// --------------
	/// Input.GetMouseButton() wrapper.
	// --------------
	static public bool GetMouseButton(
		int i ///< Mouse Button Id (0, 1 or 2)
		)
		{
		if (ControllerActive())
			return ctrl.GetMouseButton(i);
		
		return Input.GetMouseButton(i);
		}
	
	// --------------
	/// Input.GetMouseButtonDown() replacement.
	// --------------
	static public bool GetMouseButtonDown(
		int i ///< Mouse Button Id (0, 1 or 2)
		)
		{
		if (ControllerActive())
			return ctrl.GetMouseButtonDown(i);
		
		return Input.GetMouseButtonDown(i);
		}
	
	
	// --------------
	/// Input.GetMouseButtonUp() replacement.
	// --------------
	static public bool GetMouseButtonUp(
		int i ///< Mouse Button Id (0, 1 or 2)
		)
		{
		if (ControllerActive())
			return ctrl.GetMouseButtonUp(i);
		
		return Input.GetMouseButtonUp(i);
		}
	

	// ---------------
	/// Internally calls Input.ResetInputAxes() and releases all active touches of active Control Freak controller. 
	// ---------------
	static public void ResetInputAxes()
		{
		Input.ResetInputAxes();

		if (ControllerActive())
			{
			ctrl.ReleaseTouches();
			}
		}
	

	
	}




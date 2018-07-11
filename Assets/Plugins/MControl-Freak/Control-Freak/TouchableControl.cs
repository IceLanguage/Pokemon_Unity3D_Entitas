using UnityEngine;

// ----------------------
/// Base class for touchable controls.
// ----------------------

[System.Serializable]
public class TouchableControl
	{
// --------------
/// \cond DGT_DOCS_SHOW_PUBLIC_VARS
// --------------
	
	public 		bool	initiallyDisabled;
	public		bool	initiallyHidden;

	protected	bool	enabled;		///< When disabled, control will not respond to input.
	protected	bool	visible;		///< When hidden, control will be still controllable.
	public 		int 	prio;			///< Hit-detection priority.
	public 		float	hitDistScale;	///< Hit distance scale, used when testing two controls of the same priority level.
	public 		string 	name;			///< Control's name.
	public 		bool	disableGui;		///< Disable default GUI drawing.
	public 		int 	guiDepth;		///< GUI depth offset to controller's base GUI depth
			
	public 		int		layoutBoxId;	///< Layout Box id.
	public 		bool	acceptSharedTouches;	///< When enabled, this control will accept new touch even if it's already used by higher priority control. Use with caution!


/// \endcond

/// \cond

	protected 	TouchController	joy;


	// ------------------		
	virtual public void Init(TouchController joy)
		{
		this.joy = joy;

		this.visible	= true;
		this.enabled	= true;

		//this.OnReset();
		}
		

	// ------------------
	virtual public TouchController.HitTestResult HitTest(Vector2 pos, int touchId)
		{
		return new TouchController.HitTestResult(false);
		}
		
	// --------------
	virtual public TouchController.EventResult OnTouchStart(int touchId, Vector2 pos)
		{
		return TouchController.EventResult.NOT_HANDLED;
		}
		
	// --------------
	virtual public TouchController.EventResult	OnTouchEnd(int touchId, bool cancel = false) //, Vector2 pos)	
		{
		return TouchController.EventResult.NOT_HANDLED;
		}		

	// ---------------
	virtual public TouchController.EventResult	OnTouchMove(int touchId, Vector2 pos)
		{
		return TouchController.EventResult.NOT_HANDLED;
		}


	// -----------------	
	virtual public void OnReset()
		{
		}



	// ---------------
	virtual public void OnPrePoll()
		{
		}

	// ---------------
	virtual public void OnPostPoll()
		{
		}


	// -------------
	virtual public void OnUpdate() //bool firstPostPollUpdate)
		{
		}

	// -------------
	//virtual public void OnPostUpdate() //bool firstPostPollUpdate)
	//	{
	//	}
				
	
	// ---------------
	virtual public void OnLayoutAddContent()
		{
		}

	// ---------------
	virtual public void OnLayout()
		{
		}

	// --------------
	virtual public void DrawGUI()	
		{
		}

		

/// \endcond

	// ---------------
	/// Release all touches assigned to this control. 
	// ---------------
	virtual public void ReleaseTouches()
		{
		}
		

	// ------------
	/// Make any shared touches exlusive to this control.
	// ------------
	virtual public void TakeoverTouches(TouchableControl controlToUntouch)
		{
		
		}

	// -------------
	/// Reset this control's screen position and size to it's default position (automatic layout).
	// -------------
	virtual public void ResetRect()
		{
		}


	// -------------
	/// Disable default GUI rendering.
	// -------------
	public void DisableGUI()
		{
		this.disableGui = true;
		}

	// -------------
	/// Enable default GUI rendering.
	// -------------
	public void EnableGUI()
		{
		this.disableGui = false;
		}

	// ---------------
	/// Returns true if this control is rendered as a part of automatic GUI.
	// ---------------
	public bool DefaultGUIEnabled()
		{	
		return !this.disableGui;
		}
	


	// ---------------
	/// Returns true if this control is enabled.
	// ---------------
	public bool Enabled()
		{
		return this.enabled;
		}

	// -----------------
	/// Enable this stick.
	// -----------------
	virtual public void Enable(
		bool skipAnimation		///< Skip animation.
		)
		{
		this.enabled = true;
		}
	// -----------------
	/// Shortcut for Enable(false)
	// -----------------
	public void Enable()
		{
		this.Enable(false);
		}

	// -----------------
	/// Disable this stick and release any active touches.
	// -----------------
	virtual public void Disable(
		bool skipAnimation		///< Skip animation.
		)
		{
		this.enabled = false;

		this.ReleaseTouches();
		}
	// -----------------
	/// Shortcut for Disable(false)
	// -----------------
	public void Disable()
		{
		this.Disable(false);
		}
	

	// ------------------
	/// Show hidden control.
	// ------------------
	virtual public void Show(
		bool	skipAnim	/* = false */		///< Skip animation.
		)
		{
		this.visible = true;
		}
	// -----------------
	/// Shortcut for Show(false)
	// -----------------
	public void Show()
		{
		this.Show(false);
		}


	// ------------------
	/// Hide this control and release any active touches.
	// ------------------
	virtual public void Hide(
		bool	skipAnim	/* = false */		///< Skip animation.
		)
		{
		this.visible = false;
		
		this.ReleaseTouches();
		}
	// -----------------
	/// Shortcut for Hide(false)
	// -----------------
	public void Hide()
		{
		this.Hide(false);
		}
	
	


	}
	


	


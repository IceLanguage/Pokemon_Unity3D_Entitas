// --------------------------
// FPP Demo -----------------
// Dan's Game Tools 2013 ----
// ------------------------------------
// Copyright (C) 2013 Dan's Game Tools
// ------------------------------------
#pragma strict 

@script AddComponentMenu("ControlFreak-Demos-JS/DemoFppCharaJS")

public class DemoFppCharaJS extends MonoBehaviour 
	{
	public var	playerViewCam	: Camera;
	public var	charaCtrl		: CharacterController;

	public var	scopeTex		: Texture2D;

	public var	gun				: GunJS;
	public var	headBone		: Transform;			// camera will be placed there
	public var	upperBodyBone	: Transform;		// this bone will be rotated when verticl aiming
	public var	gunBone			: Transform;			// gun will be attached to this bone

	public var	normalFov		: float		= 40;
	public var	zoomFovMin		: float		= 20;
	public var	zoomFovMax		: float		= 10;
	private var	zoomActive		: boolean	= false;
	private var	zoomFactor		: float		= 0;
	public var	zoomFactorPerCm	: float		= 0.5f;		// how much zoom factor will change per centimeter of drag

	public var	walkSpeed		: float	= 8.0f;
	public var	strafeSpeed		: float = 5.0f;
	public var	aimSensitivity	: float	= 1.0f;
	public var	horzAimSpeed	: float	= 20.0f;	/// degrees per cm
	public var	vertAimSpeed	: float	= 10.0f;	/// degrees per cm

	public var	camOfs				: Vector3 = new Vector3(0, 1.6f, 0);


	private	var	aimHorz	: float;			// raw target angles
	private	var	aimVert	: float;
				
	private	var	aimHorzDisplay		: float;	// smoothed angles used for display
	private	var	aimVertDisplay		: float;

	public var	aimSmoothingTime	: float	 = 0.1f;
	private var	aimHorzDampVel	: float	;
	private var	aimVertDampVel	: float	;

	private var	isAiming	: boolean;		// vars used by the controller functions
	private var	aimVertRaw	: float;	
	private var	aimHorzRaw	: float;

	private var	isZoomDragging	: boolean;	 
	private var	zoomFactorRaw	: float;
	
	private static var WALK_DEADZONE	: float	 = 0.1f;
	
	private var	walkTargetDir	: Vector2;
	private var	walkCurDir		: Vector2;	// current walk direction (-1..1)
	private var	walkingCur		: boolean;
	private var walkingPrev		: boolean;
	private var	walkElapsed		: float;
	public var 	walkVertBobScale: float	  	= 0.1f;	// bob 10 cm
	public var	walkVertBobFreq	: float		= 0.5f;	// bob cycle = 500 ms

	public var	walkSmoothingTime	: float	 	= 0.2f;		// 0..1
	private var walkDirDampVel		: Vector3;

	private var walkBobVec			: Vector3;

	private var	upperBodyInitialPos	: Vector3;


	private var	touchCtrl			: TouchController;


	private static var MIN_AIM_ANGLE : float	= -50.0f;
	private static var MAX_AIM_ANGLE : float	= 70.0f;




	// ---------------
	function Start()
		{
		if (this.charaCtrl == null)
			this.charaCtrl = this.gameObject.GetComponent(CharacterController);

		if (this.upperBodyBone != null)
			this.upperBodyInitialPos = this.upperBodyBone.localPosition;
		}



	// ----------------
	public function ControlByTouch() : void 
		{
		if (this.touchCtrl == null)
			return;

		// Get controls' references...

		var stickWalk	: TouchStick	= this.touchCtrl.GetStick(DemoFppGameJS.STICK_WALK);
		var	zoneAim 	: TouchZone		= this.touchCtrl.GetZone(DemoFppGameJS.ZONE_AIM);
		var	zoneFire 	: TouchZone		= this.touchCtrl.GetZone(DemoFppGameJS.ZONE_FIRE);
		var	zoneZoom 	: TouchZone		= this.touchCtrl.GetZone(DemoFppGameJS.ZONE_ZOOM);
		var	zoneReload 	: TouchZone		= this.touchCtrl.GetZone(DemoFppGameJS.ZONE_RELOAD);
		

		// If Walk stick is pressed... 
		
		if (stickWalk.Pressed())
			{
			// ... use it's unnormalized direction vector to control walking.

			var moveVec : Vector2 = stickWalk.GetVec(); 

			this.SetWalkSpeed(moveVec.y, moveVec.x);
			}
	
		// Stop walking when stick is released...
		else
			{
			this.SetWalkSpeed(0, 0);
			}


		// Firing...
		// Set weapons trigger by getting the Fire zone pressed state
		// (include mid-frame press)
		 
		this.SetTriggerState(zoneFire.UniPressed(true, false));


		// Reload on Reload zone press (including mid-frame press and release situations).
	
		if (zoneReload.JustUniPressed(true, true))
			this.ReloadWeapon();

		
		// Toggle zoom mode, when tapped on zoom-zone...
		
		if (zoneZoom.JustTapped())
			{
			this.zoomActive = !this.zoomActive;
			}
		
		// Zoom dragging...

		if (this.zoomActive && 
			zoneZoom.UniPressed(false, false))
			{
			// Store inital zoom factor...

			if (!this.isZoomDragging)
				{
				this.isZoomDragging = true;
				this.zoomFactorRaw 	= this.zoomFactor;
				}
	
			// Change zoom factor by RAW vertical unified-touch drag delta
			// queried in centimeters and scaled...

			this.zoomFactorRaw += (this.zoomFactorPerCm * 
				zoneZoom.GetUniDragDelta(TouchCoordSys.SCREEN_CM, true).y);

			this.zoomFactor = Mathf.Clamp(this.zoomFactorRaw, 0, 1);
			}
		else
			{
			this.isZoomDragging = false;
			}




		// Aim, when either aim or fire zone if pressed by at least one finger 
		// (ignoring mid-frame presses and releases)... 
		
		if (zoneAim.UniPressed(false, false) ||
			zoneFire.UniPressed(false, false))
			{
			// If just started aiming, store initial aim angles...

			if (!this.isAiming)
				{
				this.isAiming 	= true;
				this.aimVertRaw = this.aimVert;
				this.aimHorzRaw = this.aimHorz;
				}
		
			// Get aim delta adding aim-zone and fire-zone's 
			// unified-touch RAW drag deltas in centimeters...

			var aimDelta : Vector2 = 
				zoneAim.GetUniDragDelta(TouchCoordSys.SCREEN_CM, true) + 
				zoneFire.GetUniDragDelta(TouchCoordSys.SCREEN_CM, true);
	

			// Apply aim-sensitivity and speed...
		
			aimDelta *= Mathf.Lerp(0.1f, 1.0f, this.aimSensitivity);
			
			aimDelta.x *= this.horzAimSpeed;
			aimDelta.y *= this.vertAimSpeed;
			
			// Add calculated delta to current our raw, non-clamped aim angles
			// and pass them to Aim() function.
			// By keeping separate, non-clamped angles we prevent that 
			// uncomfortable effect when dragging past the limit and back again. 
 
			this.aimHorzRaw += aimDelta.x;
			this.aimVertRaw += aimDelta.y;
			
			// 
			this.Aim(this.aimHorzRaw, this.aimVertRaw);
			}
		else
			{
			this.isAiming = false;
			}

		// When double tapped the aim zone - level horizonal aim angle...

		if (zoneAim.JustDoubleTapped())
			this.Aim(this.aimHorz, 0);



		}
	



	// --------------
	public function UpdateChara() : void
		{
		if (this.touchCtrl != null)
			this.ControlByTouch();

		
		// Smooth aim angles...


		this.aimHorzDisplay = Mathf.SmoothDamp(this.aimHorzDisplay, this.aimHorz, 
			this.aimHorzDampVel, this.aimSmoothingTime);
		this.aimVertDisplay = Mathf.SmoothDamp(this.aimVertDisplay, this.aimVert, 
			this.aimVertDampVel, this.aimSmoothingTime);
		

		// Update walk...

		this.walkCurDir = Vector3.SmoothDamp(this.walkCurDir, this.walkTargetDir, 
			this.walkDirDampVel, this.walkSmoothingTime);

		var walkPow : float = this.walkCurDir.magnitude;

		this.walkingPrev 	= this.walkingCur;
		this.walkingCur 	= (walkPow > WALK_DEADZONE);
		
		if (this.walkingCur)
			{
			if (!this.walkingPrev)
				this.walkElapsed = 0;				
			else
				this.walkElapsed += Time.deltaTime;
			}

		// Calculate the bob vector...

		if (this.walkingCur)
			{	
			this.walkBobVec = Vector3.zero;
			this.walkBobVec.y = SineBobPositive(this.walkElapsed, this.walkVertBobFreq) * this.walkVertBobScale;

			this.walkBobVec *= Mathf.Clamp01(walkPow);
			}
		else
			this.walkBobVec = Vector3.zero;


		// Move the character...
	
		var moveDelta : Vector3 = new Vector3(this.walkCurDir.x, 0, this.walkCurDir.y);
		moveDelta.z *= this.walkSpeed;
		moveDelta.x *= this.strafeSpeed;
		moveDelta *= Time.deltaTime;
		moveDelta = Quaternion.Euler(0, this.aimHorz, 0) * moveDelta;

		if (this.charaCtrl != null)
			this.charaCtrl.Move(moveDelta);
		else 
			this.transform.position += moveDelta;

	

			

		// Transform bones
		
		this.transform.localRotation = Quaternion.Euler(0, this.aimHorzDisplay, 0);
		
		if (this.upperBodyBone != null)
			{
			this.upperBodyBone.localRotation = Quaternion.Euler(this.aimVertDisplay, 0, 0);
			this.upperBodyBone.localPosition = this.upperBodyInitialPos + this.walkBobVec;
			}

		}
	
	



	// ----------------
	public function OnPauseStart() : void
		{
		// Very important - when entering pause menu and 
		// since this isn't real PAUSE (because game continues underneeth)
		// - stop the gun's trigger.
  
		if (this.gun != null)
			this.gun.SetTriggerState(false);
		}


	// -----------------
	public function OnPauseEnd() : void
		{
		}




	// ----------------
	static private function SineBobPositive(t : float, freq : float) : float
		{
		return ((Mathf.Sin(((t / freq) + 0.75f) * Mathf.PI * 2.0f) + 1.0f) / 2.0f);
		}

	// ----------------
	//static private function SineBob(t : float, freq : float) : float
	//	{
	//	return (Mathf.Sin(t / freq) * Mathf.PI * 2.0f);
	//	}

	


	// ---------------
	public function OnInventoryChange() : void
		{
		// modify touch controller...
		}

	// ---------------
	public function SetWalkSpeed(forward : float, side : float) : void
		{	
		this.walkTargetDir.y = Mathf.Clamp(forward, -1, 1);
		this.walkTargetDir.x = Mathf.Clamp(side,	-1, 1);
		}

	public function SetWalkSpeed(vec : Vector2) : void
		{	
		this.SetWalkSpeed(vec.x, vec.y);
		}



	// ---------------
	public function Aim(horzAngle : float, vertAngle : float) : void 
		{
		vertAngle = Mathf.Clamp(vertAngle, MIN_AIM_ANGLE, MAX_AIM_ANGLE); 
		
		this.aimVert 	= vertAngle;
		this.aimHorz 	= horzAngle;
		}
	

	// ----------------
	public function PickupAmmo(amount : int) : void 
		{
		}
	
	
	// ----------------
	public function PerformAction() : void
		{
		}

	// ---------------
	public function ReloadWeapon() : void
		{
		if (this.gun != null)
			this.gun.Reload();
		}

	// ---------------
	public function SetTriggerState(triggerOn : boolean) : void
		{
		if (this.gun != null)		
			this.gun.SetTriggerState(triggerOn);
		}



	// -------------------
	public function SetTouchController(joy : TouchController) : void
		{	
		this.touchCtrl = joy;
		}


	// ----------------
	public function ChangeWeapon(delta : int) : void
		{
		}

	// --------------
#if UNITY_EDITOR
@ContextMenu("Position Camera")
#endif
	public function PositionCamera() : void
		{
		if (this.playerViewCam == null)
			return;
	
		var camtf : Transform = this.playerViewCam.transform;
	

		if (this.headBone != null)
			{
			camtf.position = this.headBone.position;
			camtf.rotation	= this.headBone.rotation;
			}
		else
			{
			camtf.position = this.transform.position + this.camOfs;
			camtf.rotation = Quaternion.Euler(this.aimVertDisplay, this.aimHorzDisplay, 0);
			}

	
		// -------------
		if (this.zoomActive)
			{
			this.playerViewCam.fieldOfView = Mathf.Lerp(this.zoomFovMin, this.zoomFovMax, this.zoomFactor);
			}
		else
			{
			this.playerViewCam.fieldOfView = this.normalFov;
			}

		
		}
				


	// ---------------
	function LateUpdate()
		{
		this.PositionCamera();
		}			
	


	// -----------------
	public function DrawGUIBG() : void
		{
		if (!this.zoomActive)
			return;

		if ((this.scopeTex != null) && (Event.current.type == EventType.Repaint))
			{
			GUI.color = Color.white;
			GUI.DrawTexture(new Rect(0,0, Screen.width, Screen.height), this.scopeTex, ScaleMode.ScaleAndCrop);
			}
		}


	// -----------------
	public function DrawCustomGUI() : void
		{
		if (this.touchCtrl != null)
			{
			
			}

		if (this.gun != null)
			{
			if (Event.current.type == EventType.Repaint)
				{
				GUI.color = ((this.gun.bulletCount > 0) ? Color.white : Color.red);
				GUI.depth = 1;
				GUI.Label(
					new Rect(Screen.width - 50, 10, 50, 30), 
					("" + this.gun.bulletCount + "/" + this.gun.bulletCapacity));

				
				// Print zoom info just above Zoom touch zone...

				if (this.zoomActive)
					{
					var zoomWidth : float = 100;
					var zoomHeight: float = 30;
	
					// Get ZOOM button's display rect...

					var rect : Rect = this.touchCtrl.GetZone(DemoFppGameJS.ZONE_ZOOM).GetDisplayRect(true);
					
					rect = new Rect(rect.center.x - (zoomWidth * 0.5f), rect.y - zoomHeight,
						zoomWidth, zoomHeight);  
	
					GUI.color = Color.white;
					GUI.Label(rect, "ZOOM " + (Mathf.FloorToInt(this.zoomFactor * 100)).ToString("00") + "%");
					}
				}
			}
		}
	}

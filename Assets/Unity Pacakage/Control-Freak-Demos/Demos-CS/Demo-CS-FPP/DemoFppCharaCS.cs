// --------------------------
// FPP Demo -----------------
// Dan's Game Tools 2013 ----
// ------------------------------------
// Copyright (C) 2013 Dan's Game Tools
// ------------------------------------

using UnityEngine;

[AddComponentMenu("ControlFreak-Demos-CS/DemoFppCharaCS")]
public class DemoFppCharaCS : MonoBehaviour 
	{ 
	public Camera 				playerViewCam;
	public CharacterController 	charaCtrl;
	
	public Texture2D			scopeTex;

	public GunCS				gun;
	public Transform 			headBone;			// camera will be placed there
	public Transform			upperBodyBone;		// this bone will be rotated when verticl aiming
	public Transform			gunBone;			// gun will be attached to this bone

	public float	normalFov	= 40;
	public float	zoomFovMin	= 20;
	public float	zoomFovMax	= 10;
	private bool	zoomActive	= false;
	private float	zoomFactor	= 0;
	public float	zoomFactorPerCm	= 0.5f;		// how much zoom factor will change per centimeter of drag

	public float	walkSpeed 		= 8.0f;
	public float	strafeSpeed		= 5.0f;
	public float	aimSensitivity	= 1.0f;
	public float	horzAimSpeed	= 20.0f;	/// degrees per cm
	public float	vertAimSpeed	= 10.0f;	/// degrees per cm

	public Vector3	camOfs			= new Vector3(0, 1.6f, 0);


	private	float	aimHorz;			// raw target angles
	private	float	aimVert;
				
	private	float	aimHorzDisplay;		// smoothed angles used for display
	private	float	aimVertDisplay;

	public float	aimSmoothingTime = 0.1f;
	private float	aimHorzDampVel;
	private float	aimVertDampVel;

	private bool	isAiming;		// vars used by the controller functions
	private float	aimVertRaw;	
	private float	aimHorzRaw;

	private bool	isZoomDragging;	 
	private float	zoomFactorRaw;
	
	private const float WALK_DEADZONE = 0.1f;
	
	private Vector2	walkTargetDir;
	private Vector2	walkCurDir;			// current walk direction (-1..1)
	private bool	walkingCur;
	private bool	walkingPrev;
	private float	walkElapsed;
	public float 	walkVertBobScale  	= 0.1f;	// bob 10 cm
	public float	walkVertBobFreq		= 0.5f;	// bob cycle = 500 ms

	public float	walkSmoothingTime 	= 0.2f;		// 0..1
	private Vector3 walkDirDampVel;

	private Vector3 walkBobVec;

	private Vector3	upperBodyInitialPos;


	private TouchController	touchCtrl;
	

	private const float MIN_AIM_ANGLE = -50.0f;
	private const float MAX_AIM_ANGLE = 70.0f;

	



	// ---------------
	private void Start()
		{
		if (this.charaCtrl == null)
			this.charaCtrl = this.gameObject.GetComponent<CharacterController>();

		if (this.upperBodyBone != null)
			this.upperBodyInitialPos = this.upperBodyBone.localPosition;
		}



	// ----------------
	public void ControlByTouch()
		{
		if (this.touchCtrl == null)
			return;

		// Get controls' references...

		TouchStick	stickWalk	= this.touchCtrl.GetStick(DemoFppGameCS.STICK_WALK);
		TouchZone 	zoneAim		= this.touchCtrl.GetZone(DemoFppGameCS.ZONE_AIM);
		TouchZone 	zoneFire	= this.touchCtrl.GetZone(DemoFppGameCS.ZONE_FIRE);
		TouchZone 	zoneZoom	= this.touchCtrl.GetZone(DemoFppGameCS.ZONE_ZOOM);
		TouchZone 	zoneReload	= this.touchCtrl.GetZone(DemoFppGameCS.ZONE_RELOAD);
		

		// If Walk stick is pressed... 
		
		if (stickWalk.Pressed())
			{
			// ... use it's unnormalized direction vector to control walking.

			Vector2 moveVec = stickWalk.GetVec(); 

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

			Vector2 aimDelta = 
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
	public void UpdateChara()
		{
		if (this.touchCtrl != null)
			this.ControlByTouch();

		
		// Smooth aim angles...


		this.aimHorzDisplay = Mathf.SmoothDamp(this.aimHorzDisplay, this.aimHorz, 
			ref this.aimHorzDampVel, this.aimSmoothingTime);
		this.aimVertDisplay = Mathf.SmoothDamp(this.aimVertDisplay, this.aimVert, 
			ref this.aimVertDampVel, this.aimSmoothingTime);
		

		// Update walk...

		this.walkCurDir = Vector3.SmoothDamp(this.walkCurDir, this.walkTargetDir, 
			ref this.walkDirDampVel, this.walkSmoothingTime);

		float walkPow = this.walkCurDir.magnitude;

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
	
		Vector3 moveDelta = new Vector3(this.walkCurDir.x, 0, this.walkCurDir.y);
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
	public void OnPauseStart()
		{
		// Very important - when entering pause menu and 
		// since this isn't real PAUSE (because game continues underneeth)
		// - stop the gun's trigger.
  
		if (this.gun != null)
			this.gun.SetTriggerState(false);
		}


	// -----------------
	public void OnPauseEnd()
		{
		}




	// ----------------
	static private float SineBobPositive(float t, float freq)
		{
		return ((Mathf.Sin(((t / freq) + 0.75f) * Mathf.PI * 2.0f) + 1.0f) / 2.0f);
		}

	// ----------------
	static private float SineBob(float t, float freq)
		{
		return (Mathf.Sin(t / freq) * Mathf.PI * 2.0f);
		}

	


	// ---------------
	public void OnInventoryChange()
		{
		// modify touch controller...
		}

	// ---------------
	public void SetWalkSpeed(float forward, float side)
		{	
		this.walkTargetDir.y = Mathf.Clamp(forward, -1, 1);
		this.walkTargetDir.x = Mathf.Clamp(side,	-1, 1);
		}

	public void SetWalkSpeed(Vector2 vec)
		{	
		this.SetWalkSpeed(vec.x, vec.y);
		}



	// ---------------
	public void Aim(float horzAngle, float vertAngle)
		{
		vertAngle = Mathf.Clamp(vertAngle, MIN_AIM_ANGLE, MAX_AIM_ANGLE);
		
		this.aimVert 	= vertAngle;
		this.aimHorz 	= horzAngle;
		}
	

	// ----------------
	public void PickupAmmo(int amount)
		{
		}
	
	
	// ----------------
	public void PerformAction()
		{
		}

	// ---------------
	public void ReloadWeapon()
		{
		if (this.gun != null)
			this.gun.Reload();
		}

	// ---------------
	public void SetTriggerState(bool on)
		{
		if (this.gun != null)		
			this.gun.SetTriggerState(on);
		}



	// -------------------
	public void SetTouchController(TouchController joy)
		{	
		this.touchCtrl = joy;
		}


	// ----------------
	public void ChangeWeapon(int delta)
		{
		}

	// --------------
#	if UNITY_EDITOR
	[ContextMenu("Position Camera")]
#	endif
	public void PositionCamera()
		{
		if (this.playerViewCam == null)
			return;
	
		Transform camtf = this.playerViewCam.transform;
	

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
	private void LateUpdate()
		{
		this.PositionCamera();
		}			
	


	// -----------------
	public void DrawGUIBG()
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
	public void DrawCustomGUI()
		{

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
					const float zoomWidth = 100;
					const float zoomHeight = 30;
	
					// Get ZOOM button's display rect...

					Rect rect = this.touchCtrl.GetZone(DemoFppGameCS.ZONE_ZOOM).GetDisplayRect(true);
					
					rect = new Rect(rect.center.x - (zoomWidth * 0.5f), rect.y - zoomHeight,
						zoomWidth, zoomHeight);  
	
					GUI.color = Color.white;
					GUI.Label(rect, "ZOOM " + ((int)(this.zoomFactor * 100)).ToString("00") + "%");
					}
				}
			}
		}
	}

using UnityEngine;
//using TouchControllerLib;

[AddComponentMenu("ControlFreak-Demos-CS/DemoRpgCharaCS")]
public class DemoRpgCharaCS : MonoBehaviour 
{
public GunCS				gun;
	
	
public float	walkSpeed			= 7.0f;
public float	runSpeed			= 15.0f;
public float	walkAnimSpeed		= 0.8f;
public float	runAnimSpeed		= 1.6f;
public float	walkStickThreshold	= 0.2f;	
public float	runStickThreshold 	= 0.9f;
			
public float	angle				= 0;	
						
public float	angleSmoothingTime	= 0.6f;
public float	turnMaxSpeed		= 400;		// degrees per second


private Animation			charaAnim;
private CharacterController	charaCtrl;
	
private CharaState		charaState		= CharaState.NONE;
	
private float			useAnimDuration = 1.0f;
private float			useElapsed;
	

public AudioClip		actionSound;



// ----------------
// Character state enum.
// ----------------

public enum CharaState
	{
	IDLE,
	WALK,
	RUN,
	USE,
	NONE
	}
	
// ----------------------
// Animation clips.
// ----------------------

public string 
	ANIM_IDLE	= "idle",
	ANIM_WALK_F	= "run_forward",
	ANIM_RUN_F	= "run_forward",
	ANIM_USE	= "use";



// ---------------------
private void Start()
	{
	this.charaAnim = this.GetComponent<Animation>();
	if (this.charaAnim == null)
		Debug.LogError("Character Animation Component is not available!");
	
	this.charaCtrl = this.GetComponent<CharacterController>();
	if (this.charaCtrl == null)
		Debug.LogError("CharacterController is not assigned!");
		
	// Get use anim duration...
		
	
	if ((this.charaAnim != null) && (this.charaAnim.GetClip(this.ANIM_USE) != null))
		{
		this.charaAnim[this.ANIM_USE].layer = 2;	// set "USE" layer to higher value
		this.useAnimDuration = this.charaAnim.GetClip(this.ANIM_USE).length;
		}
	


	this.StartIdle();
	}



// --------------------
public void ControlByTouch(TouchController ctrl, DemoRpgGameCS game)
	{
	TouchStick 
		stickWalk 	= ctrl.GetStick(DemoRpgGameCS.STICK_WALK);
	TouchZone
		zoneScreen	= ctrl.GetZone(DemoRpgGameCS.ZONE_SCREEN),
		zoneAction	= ctrl.GetZone(DemoRpgGameCS.ZONE_ACTION),
		zoneFire	= ctrl.GetZone(DemoRpgGameCS.ZONE_FIRE);



	// Get stick's normalized direction in world space relative to camera's angle...

	Vector3 moveWorldDir	= stickWalk.GetVec3d(TouchStick.Vec3DMode.XZ, true, game.camOrbitalAngle);


	// Get stick's angle in world space by adding it to camera's angle..
 
	float	stickWorldAngle	= stickWalk.GetAngle() + game.camOrbitalAngle;


	// Get walking speed from stick's current tilt...

	float 	speed 			= stickWalk.GetTilt();
		

	// Get gun's trigger state by checking Fire zone's state (including mid-frame press)

	bool	gunTriggerState	= zoneFire.UniPressed(true, false);


	// Check if Action should be performed - either by pressing the Action button or
	// by tapping on the right side of the screen...

	bool	performAction	= 
		zoneAction.JustUniPressed(true, true) || 
		zoneScreen.JustTapped();
		


	if ((this.charaState == CharaState.IDLE) ||
		(this.charaState == CharaState.WALK) ||
		(this.charaState == CharaState.RUN))
		{
		if (speed > this.walkStickThreshold)
			{
			float moveSpeed = 0;
	
			// Walk...
	
			if (speed < this.runStickThreshold)
				{
				moveSpeed = this.walkSpeed;					
	
				if (this.charaState != CharaState.WALK)
					this.StartWalking();

				// Set animation speed...

				this.charaAnim[this.ANIM_WALK_F].speed = this.walkAnimSpeed; 
				}

			// Run!

			else
				{	
				moveSpeed = this.runSpeed;

				if (this.charaState != CharaState.RUN)
					this.StartRunning();
					
				// Set animation speed...

				this.charaAnim[this.ANIM_RUN_F].speed = this.runAnimSpeed;
				}
	


			// Update player's angle...

			this.angle = DampAngle(this.angle, stickWorldAngle, this.angleSmoothingTime, this.turnMaxSpeed, Time.deltaTime); 
			
			// Move player's collider...

			this.charaCtrl.Move(moveWorldDir * moveSpeed * Time.deltaTime);
			}
	
		else if ((this.charaState == CharaState.WALK) || (this.charaState == CharaState.RUN))
			{
			// Stop...
	
			this.StartIdle();
			}
			
		
		// Perform Action...

		if (performAction)	
			this.PerformUseAction();

		}		


	// Every other state than USE...

	if (this.charaState != CharaState.USE)
		{
		if (this.gun != null)
			this.gun.SetTriggerState(gunTriggerState); 

		if (performAction)
			this.PerformUseAction();
		}
	
	// USE state updates...

	else
		{
		if (this.gun != null)
			this.gun.SetTriggerState(false);

		this.useElapsed += Time.deltaTime;
		if (this.useElapsed > this.useAnimDuration)
			this.StartIdle();
		}
	
	}

	
				
// -------------------
static private float DampAngle(float cur, float target, float smoothingTime, float maxSpeed, float dt)
	{			
	float a = Mathf.DeltaAngle(cur, target);
		
	smoothingTime *= 0.2f;

	if (dt < smoothingTime)
		a = Mathf.Lerp(0, a, dt / smoothingTime);	
					
	maxSpeed *= dt;
	
	if (a > maxSpeed)	
		a = maxSpeed;
	else if (a < -maxSpeed)
		a = -maxSpeed;

	return cur + a;		
	}


// --------------------
public void StartIdle()
	{
	this.charaState = CharaState.IDLE;
	this.charaAnim.CrossFade(this.ANIM_IDLE, 0.3f);
	}

// --------------------
public void StartWalking()
	{
	this.charaState = CharaState.WALK;
	this.charaAnim.CrossFade(this.ANIM_WALK_F, 0.3f);
	}

// -------------------
public void StartRunning()
	{
	this.charaState = CharaState.RUN;
	this.charaAnim.CrossFade(this.ANIM_RUN_F, 0.3f);
	}


// ---------------------
public void PerformUseAction()
	{
	this.charaState = CharaState.USE;

	this.charaAnim.CrossFade(this.ANIM_IDLE, 0.3f);		// idle on base layer
	this.charaAnim.CrossFade(this.ANIM_USE, 0.3f, PlayMode.StopSameLayer);		// use on it's own layer
	this.useElapsed = 0;
	
	//Debug.Log("[" + Time.frameCount + "] USE!"); 

		
	// Play sound effect...

	AudioSource audioSource = this.GetComponent<AudioSource>();		

		if ((this.actionSound != null) && (audioSource != null))
			audioSource.PlayOneShot(this.actionSound);


	// look for useable items in range!
	}
	


// ---------------------
public void UpdateChara()
	{
	this.transform.localRotation = Quaternion.Euler(0, this.angle, 0);
	}
}

#pragma strict


@script AddComponentMenu("ControlFreak-Demos-JS/DemoRpgCharaJS")
public class DemoRpgCharaJS extends MonoBehaviour 
{
public var	gun					: GunJS;
	
public var	walkSpeed			: float	= 7.0f;
public var	runSpeed			: float	= 15.0f;
public var	walkAnimSpeed		: float	= 0.8f;
public var	runAnimSpeed		: float	= 1.6f;
public var	walkStickThreshold	: float	= 0.2f;	
public var	runStickThreshold	: float = 0.9f;
			
public var	angle				: float	= 0;	
						
public var	angleSmoothingTime	: float	= 0.6f;
public var	turnMaxSpeed		: float	= 400;		// degrees per second


private var	charaAnim			: Animation;
private var charaCtrl			: CharacterController;
	
private var	charaState			: CharaState = CharaState.NONE;
	
private var	useAnimDuration		: float 	= 1.0f;
private var	useElapsed			: float;
	

public var	actionSound			: AudioClip;


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

public var ANIM_IDLE	: String	= "idle";
public var ANIM_WALK_F	: String	= "run_forward";
public var ANIM_RUN_F	: String	= "run_forward";
public var ANIM_USE		: String	= "use";




// ---------------------
function Start()
	{
	this.charaAnim = this.GetComponent(Animation);
	if (this.charaAnim == null)
		Debug.LogError("Character Animation Component is not available!");
	
	this.charaCtrl = this.GetComponent(CharacterController);
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



// ---------------------
public function UpdateChara() : void
	{
	this.transform.localRotation = Quaternion.Euler(0, this.angle, 0);
	}



// --------------------
public function ControlByTouch(ctrl : TouchController, game : DemoRpgGameJS) : void
	{
	var stickWalk	: TouchStick	= ctrl.GetStick(DemoRpgGameJS.STICK_WALK);
	var zoneScreen	: TouchZone		= ctrl.GetZone(DemoRpgGameJS.ZONE_SCREEN);
	var	zoneAction	: TouchZone		= ctrl.GetZone(DemoRpgGameJS.ZONE_ACTION);
	var zoneFire	: TouchZone		= ctrl.GetZone(DemoRpgGameJS.ZONE_FIRE);



	// Get stick's normalized direction in world space relative to camera's angle...

	var moveWorldDir : Vector3	= stickWalk.GetVec3d(TouchStick.Vec3DMode.XZ, true, game.camOrbitalAngle);


	// Get stick's angle in world space by adding it to camera's angle..
 
	var stickWorldAngle	: float	= stickWalk.GetAngle() + game.camOrbitalAngle;


	// Get walking speed from stick's current tilt...

	var	speed : float	= stickWalk.GetTilt();
		

	// Get gun's trigger state by checking Fire zone's state (including mid-frame press)

	var gunTriggerState : boolean	= zoneFire.UniPressed(true, false);


	// Check if Action should be performed - either by pressing the Action button or
	// by tapping on the right side of the screen...

	var performAction	: boolean	= 
		zoneAction.JustUniPressed(true, true) || 
		zoneScreen.JustTapped();
		


	if ((this.charaState == CharaState.IDLE) ||
		(this.charaState == CharaState.WALK) ||
		(this.charaState == CharaState.RUN))
		{
		if (speed > this.walkStickThreshold)
			{
			var moveSpeed : float = 0;
	
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

	
// --------------------
public function StartIdle() : void
	{
	this.charaState = CharaState.IDLE;
	this.charaAnim.CrossFade(this.ANIM_IDLE, 0.3f);
	}

// --------------------
public function StartWalking() : void
	{
	this.charaState = CharaState.WALK;
	this.charaAnim.CrossFade(this.ANIM_WALK_F, 0.3f);
	}

// -------------------
public function StartRunning() : void
	{
	this.charaState = CharaState.RUN;
	this.charaAnim.CrossFade(this.ANIM_RUN_F, 0.3f);
	}


// ---------------------
public function PerformUseAction() : void
	{
	this.charaState = CharaState.USE;

	this.charaAnim.CrossFade(this.ANIM_IDLE, 0.3f);		// idle on base layer
	this.charaAnim.CrossFade(this.ANIM_USE, 0.3f, PlayMode.StopSameLayer);		// use on it's own layer
	this.useElapsed = 0;
	
	//Debug.Log("[" + Time.frameCount + "] USE!"); 

		
	// Play sound effect...

	var audioSource : AudioSource = this.GetComponent(AudioSource);
	if ((this.actionSound != null) && (audioSource != null))
		audioSource.PlayOneShot(this.actionSound);


	// look for useable items in range!
	}
	



				
// -------------------
static private function DampAngle(	
	cur 			: float, 
	target 			: float, 
	smoothingTime 	: float, 
	maxSpeed 		: float, 
	dt 				: float) : float
	{			
	var a : float = Mathf.DeltaAngle(cur, target);
		
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


}

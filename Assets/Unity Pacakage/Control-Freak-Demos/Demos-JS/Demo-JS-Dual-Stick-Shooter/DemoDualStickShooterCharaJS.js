// --------------------------
// Dual Stick Shoter Demo ---
// Dan's Game Tools 2013 ----
// --------------------------

#pragma strict


@script AddComponentMenu("ControlFreak-Demos-JS/DemoDualStickShooterCharaJS")

public class DemoDualStickShooterCharaJS extends MonoBehaviour 
	{
	public	var	gun			: GunJS;			// gun component


	public	var	spineBone	: Transform;		// upperbody bone to be animated by procedural sway 

	public	var swayFreq	: float 	= 0.3f;						// upperbody's sway animation frequency
	public	var	swayAngles	: Vector3 	= new Vector3(0, 0, 10.0f);	// Euler angles of sway pose
	public	var	swayFadeTime: float		= 0.4f;						// sway fade-in/-out time for smooth start/stop
	private	var	swayBlend	: float;								// current sway blend factor
	
	private	var	spineInitialRot		: Quaternion;


	public	var	runForwardSpeed		: float	= 6.0f;		// max speed when running forward
	public	var	runSideSpeed		: float	= 4.0f;		// max speed when running to the side
	public	var	runBackSpeed		: float	= 3.0f;		// max speed when running back

	public	var	maxTurnSpeed		: float	= 500.0f;	// max turn smoothing speed 
	public	var	turnSmoothingTime	: float	= 0.3f;		// turn smoothing time

	public	var	aimStickDeadZone	: float	= 0.2f;
	public	var	aimStickMinSpeed	: float	= 0.0f;		// aim locking speed just above dead zone (degs/sec) 
	public	var	aimStickMaxSpeed	: float	= 500.0f;	// aim locking speed at maximum stick tilt (degs/sec)
	
	private var aimInputAngle 		: float;	// target aiming angle set by input handler
	private var	aimInputPow 		: float;	// aiming inoput power (0 - no aiming input)



	private	var	isShooting 			: boolean;	// shooting input flag
	private var	isWalking 			: boolean;
	private var	orientCur			: float;		// current chacarter orientation
	private	var orientTarget		: float;	// target orientation used for smoothing
	private var	orientVel			: float;		// current smoothing velocity
	private var	moveSpeed			: float;		// current speed factor (0..1)
	private var	worldMoveVec		: Vector3;	// current world-space movement vector (per second)
					//localMoveDir;	// local-space movement direction vec.

	//private DemoDualStickShooterGameCS 	game;
	//private Animation					charaAnim;
	private var	charaCtrl		: CharacterController;		// Character controller (collider)




	// ---------------
	public function Init(game : DemoDualStickShooterGameJS) : void
		{
		//this.game = game;


		// Get Character Controller...

		this.charaCtrl = this.gameObject.GetComponent(CharacterController);

		// Store spine's initial transform...

		if (this.spineBone != null)
			{
			this.spineInitialRot = this.spineBone.localRotation;
			}
		}


	// ---------------	
	public function Move(
		worldDir	: Vector3,	// normalized world-space vector 
		speed		: float		// value between 0 and 1.
		) : void
		{
		this.moveSpeed = Mathf.Clamp01(speed);

		if (this.moveSpeed < 0.001f)
			{
			// Stop.

			this.worldMoveVec = Vector3.zero;
			//this.localMoveDir = Vector3.zero;
			}
		else
			{
			// Transform world vec to local space...

			var localDir : Vector3 = RotateVec(worldDir, -this.orientCur);

			//this.localMoveDir = localDir;

			// Apply Forward/Back/Side speed modifiers...
				
			if (localDir.z > 0) 
				localDir.z *= this.runForwardSpeed;
			else
				localDir.z *= this.runBackSpeed;
			
			localDir.x *= this.runSideSpeed;

			// Transform back to world space...

			this.worldMoveVec = RotateVec(localDir * speed, this.orientCur);
			}
			
		}

	// ---------------
	public function Aim(angle : float , pow : float) : void
		{
		this.aimInputAngle	= angle;
		this.aimInputPow	= pow;
		}

	// ---------------
	public function SetTriggerState(on : boolean) : void
		{
		this.isShooting = on;
		}
	

	// ------------
	public function UpdateChara() : void
		{	
		// Control gun's trigger...

		if (this.gun != null)
			this.gun.SetTriggerState(this.isShooting);
		


		// Apply aiming input...
		
		if ((this.aimInputPow > this.aimStickDeadZone) &&
			(this.aimInputPow > 0.0001f))
			{
			var lockingSpeed : float = Mathf.Clamp01((this.aimInputPow - this.aimStickDeadZone) / (1.0f - this.aimStickDeadZone));


			this.orientTarget = Mathf.MoveTowardsAngle(this.orientTarget,
				this.aimInputAngle, Time.deltaTime * 
				Mathf.Lerp(this.aimStickMinSpeed, this.aimStickMaxSpeed, lockingSpeed));	
			}
		
		// Smooth character's orientation...

		this.orientCur = Mathf.SmoothDampAngle(this.orientCur, 	
			this.orientTarget, this.orientVel, this.turnSmoothingTime  * 0.2f, 
			this.maxTurnSpeed);		

		

		// Update sway...

		this.swayBlend = Mathf.MoveTowards(this.swayBlend, moveSpeed, 
			Time.deltaTime * (1.0f / this.swayFadeTime));
 
		// Animate spine...

		if (this.spineBone != null)
			{
			this.spineBone.localRotation = this.spineInitialRot * 
				Quaternion.Slerp(Quaternion.identity, 
					Quaternion.Euler(this.swayAngles * Mathf.Sin(Mathf.PI * (Time.time / this.swayFreq))),
					this.swayBlend); // * this.moveSpeed);;

			}
		

		
		// Rotate the character...

		this.transform.localRotation = Quaternion.Euler(0, this.orientCur, 0);
		
	
		// Move the character...

		if (this.charaCtrl != null)
			this.charaCtrl.Move(this.worldMoveVec * Time.deltaTime);
		else
			this.transform.position += (this.worldMoveVec * Time.deltaTime);
		}

	

	// --------------
	public function OnPause() 
		{
		// Stop moving and shooting when pausing...

		this.Move(Vector3.zero, 0);
		this.SetTriggerState(false);
		}

	// -------------
	public function OnUnpause()
		{
		}

	// --------------
	static private function RotateVec(vec : Vector3, angle : float) : Vector3
		{
		return Quaternion.Euler(0, angle, 0) * vec;
		}
	}

using UnityEngine;

[AddComponentMenu("ControlFreak-Demos-CS/DemoDualStickShooterCharaCS")]
public class DemoDualStickShooterCharaCS : MonoBehaviour 
	{
	public	GunCS		gun;			// gun component


	public	Transform 	spineBone;		// upperbody bone to be animated by procedural sway 

	public	float		swayFreq 	= 0.3f;						// upperbody's sway animation frequency
	public	Vector3		swayAngles 	= new Vector3(0, 0, 10.0f);	// Euler angles of sway pose
	public	float		swayFadeTime= 0.4f;						// sway fade-in/-out time for smooth start/stop
	private	float		swayBlend;								// current sway blend factor
	
	private	Quaternion	spineInitialRot;


	public	float	runForwardSpeed		= 6.0f,		// max speed when running forward
					runSideSpeed		= 4.0f,		// max speed when running to the side
					runBackSpeed		= 3.0f;		// max speed when running back

	public	float	maxTurnSpeed		= 500.0f;	// max turn smoothing speed 
	public	float	turnSmoothingTime	= 0.3f;		// turn smoothing time

	public	float	aimStickDeadZone	= 0.2f;
	public	float	aimStickMinSpeed	= 0.0f;		// aim locking speed just above dead zone (degs/sec) 
	public  float	aimStickMaxSpeed	= 500.0f;	// aim locking speed at maximum stick tilt (degs/sec)
	
	private float	aimInputAngle;	// target aiming angle set by input handler
	private float	aimInputPow;	// aiming inoput power (0 - no aiming input)



	private bool	isShooting;		// shooting input flag
	private bool	isWalking;
	private float	orientCur,		// current chacarter orientation
					orientTarget;	// target orientation used for smoothing
	private float 	orientVel;		// current smoothing velocity
	private float	moveSpeed;		// current speed factor (0..1)
	private Vector3	worldMoveVec;	// current world-space movement vector (per second)
					//localMoveDir;	// local-space movement direction vec.

	//private DemoDualStickShooterGameCS 	game;
	//private Animation					charaAnim;
	private CharacterController			charaCtrl;		// Character controller (collider)




	// ---------------
	public void Init(DemoDualStickShooterGameCS game)
		{
		//this.game = game;


		// Get Character Controller...

		this.charaCtrl = (CharacterController)this.gameObject.GetComponent(
			typeof(CharacterController));

		// Store spine's initial transform...

		if (this.spineBone != null)
			{
			this.spineInitialRot = this.spineBone.localRotation;
			}
		}


	// ---------------	
	public void Move(
		Vector3 worldDir,	// normalized world-space vector 
		float 	speed		// value between 0 and 1.
		)
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

			Vector3 localDir = RotateVec(worldDir, -this.orientCur);

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
	public void Aim(float angle, float pow)
		{
		this.aimInputAngle	= angle;
		this.aimInputPow	= pow;
		}

	// ---------------
	public void SetTriggerState(bool on)
		{
		this.isShooting = on;
		}
	

	// ------------
	public void UpdateChara()
		{	
		// Control gun's trigger...

		if (this.gun != null)
			this.gun.SetTriggerState(this.isShooting);
		


		// Apply aiming input...
		
		if ((this.aimInputPow > this.aimStickDeadZone) &&
			(this.aimInputPow > 0.0001f))
			{
			float lockingSpeed = Mathf.Clamp01((this.aimInputPow - this.aimStickDeadZone) / (1.0f - this.aimStickDeadZone));


			this.orientTarget = Mathf.MoveTowardsAngle(this.orientTarget,
				this.aimInputAngle, Time.deltaTime * 
				Mathf.Lerp(this.aimStickMinSpeed, this.aimStickMaxSpeed, lockingSpeed));	
			}
		
		// Smooth character's orientation...

		this.orientCur = Mathf.SmoothDampAngle(this.orientCur, 	
			this.orientTarget, ref this.orientVel, this.turnSmoothingTime  * 0.2f, 
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
	public void OnPause()
		{
		// Stop moving and shooting when pausing...

		this.Move(Vector3.zero, 0);
		this.SetTriggerState(false);
		}

	// -------------
	public void OnUnpause()
		{
		}

	// --------------
	static private Vector3 RotateVec(Vector3 vec, float angle)
		{
		return Quaternion.Euler(0, angle, 0) * vec;
		}
	}

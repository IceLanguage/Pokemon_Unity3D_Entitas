#pragma strict 


@script AddComponentMenu("ControlFreak-Demos-JS/GunJS")

public class GunJS extends MonoBehaviour 
{
public var	shotParticles	: ParticleSystem;

public var	shotSound 		: AudioClip;
public var 	emptySound		: AudioClip;

public var	reloadSound		: AudioClip;

private var isFiring		: boolean;	
public var	shotInterval	: float	= 0.175f;	
	
private var	lastShotTime 	: float;	
	
public var	unlimitedAmmo	: boolean	= false;
public var	bulletCapacity	: int	= 40;
public var	bulletCount		: int	= 40;

public var 	projectileOrigin: Transform;		// Transform where projectiles will be fired from.
public var	bulletPrefab	: BulletJS;			// Bullet prefab reference. 
private var audioSource 	: AudioSource;


// --------------------
function  Start() : void
	{
	this.audioSource = this.GetComponent(AudioSource);
	this.isFiring = false;
	}


// -----------------------
public function SetTriggerState(fire : boolean) : void
	{	
	if (fire != this.isFiring)
		{
		this.isFiring = fire;
		
		if (fire)
			{
			// Fire first bullet...

			this.FireBullet();	
			}
		}
	}

	

// --------------------
function FixedUpdate() : void 
	{
	if (this.isFiring)
		this.FireBullet();
	
	}

 

// ------------------	
public function Reload() : void
	{
	this.bulletCount = this.bulletCapacity;
	
	if ((this.audioSource != null) && (this.reloadSound != null))
		{
		this.audioSource.loop = false;
		this.audioSource.PlayOneShot(this.reloadSound);
		}
	}


// ---------------------
private function FireBullet() : void
	{
	if ((Time.time - this.lastShotTime) >= this.shotInterval)
		{
		this.lastShotTime = Time.time;
	

		// Shoot...
			
		
		if (this.unlimitedAmmo || (this.bulletCount > 0))
			{
			if (!this.unlimitedAmmo)
				--this.bulletCount;	

			// Emit particles...
				
			if ((this.shotParticles != null) )
				{
				this.shotParticles.Play();
				}
	
	
			// Fire projectile.
	
			if ((this.projectileOrigin != null) && (this.bulletPrefab != null))
				{
				var bullet : BulletJS = Instantiate(this.bulletPrefab, 
					this.projectileOrigin.position, this.projectileOrigin.rotation) as BulletJS;

				if (bullet != null)
					bullet.Init(this); 
				} 


			// Play sound...
	
			if ((this.audioSource != null) && (this.shotSound != null)) // && (!this.shotSoundLooped))
				{	
				this.audioSource.loop = false;
				this.audioSource.PlayOneShot(this.shotSound);	
				}
			}

		// No bullets left!!

		else
			{
			// Play sound...
	
			if ((this.audioSource != null) && (this.emptySound != null)) // && (!this.emptySoundLooped))
				{	
				this.audioSource.loop = false;
				this.audioSource.PlayOneShot(this.emptySound);	
				}
			}

		}
	}

}

#pragma strict


@script AddComponentMenu("ControlFreak-Demos-JS/BulletJS")

public class BulletJS extends MonoBehaviour 
	{
	private var	gun	: GunJS;		// reference to a gun that fired this bullet.
	

	public var	maxLifetime	: float	= 5.0f;
	private var	lifetime	: float;


	// --------------------
	public function Init(gun : GunJS) : void
		{
		this.gun = gun;
		this.lifetime = 0;
		}
		

	// ---------------
	function FixedUpdate() : void
		{
		// Destroy this bullet if it didn't hit anything...

		this.lifetime += Time.deltaTime;
		if (this.lifetime > this.maxLifetime)
			Destroy(this.gameObject);
		}


	// ------------------
	function OnTriggerEnter(objectHit : Collider) : void
		{
		// TODO : explode, inflict damage, etc.
		
		if (this.gun != null)
			{
			// ...
			}
		
#if UNITY_EDITOR
		Debug.Log("Fr[" + Time.frameCount + "] bullet ["+this.name+"] hit [" + objectHit.name + "]!");
#endif	
	
		// Destroy on impact.

		Destroy(this.gameObject);
		} 
	}

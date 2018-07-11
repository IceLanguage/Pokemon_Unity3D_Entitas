using UnityEngine;

[AddComponentMenu("ControlFreak-Demos-CS/BulletCS")]
public class BulletCS : MonoBehaviour 
	{
	private GunCS	gun;		// reference to a gun that fired this bullet.
	

	public float 	maxLifetime = 5.0f;
	private float	lifetime;


	// --------------------
	public void Init(GunCS gun)
		{
		this.gun = gun;
		this.lifetime = 0;
		}
		

	// ---------------
	void FixedUpdate()
		{
		// Destroy this bullet if it didn't hit anything...

		if ((this.lifetime += Time.deltaTime) > this.maxLifetime)
			Destroy(this.gameObject);
		}


	// ------------------
	void OnTriggerEnter(Collider objectHit)
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

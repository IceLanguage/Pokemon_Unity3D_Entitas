//using UnityEngine;

// ---------------
/// Simple Animation Timer Struct.
// ---------------
public struct AnimTimer
	{
	private bool 	enabled;
	private bool	running;
	private float 	elapsed;
	private float 	duration;
	private float 	nt;
	private float 	ntPrev;
	

	// ---------------
	public bool		Enabled 	{ get { return this.enabled; } }	///< True if timer's enabled (running or not).
	public bool		Running 	{ get { return this.running; } }	///< True if timer's running.
	public bool		Completed	{ get { return !this.running; } }	///< True if timer's completed.
	public float	Elapsed 	{ get { return this.elapsed; } }	///< Elapsed time in seconds.
	public float	Duration 	{ get { return this.duration; } }	///< Timer's duration.
	public float	Nt 			{ get { return this.nt; } }			///< Current normalized time.
	public float	NtPrev		{ get { return this.ntPrev; } }		///< Previous frame's normalized time.

	// --------------
	/// Stop and disable timer.
	// --------------
	public void Reset(
		float t = 0		///< Timer's new normalized time. 
		)
		{
		this.enabled 	= false;
		this.running 	= false;
		this.elapsed 	= 0;
		this.nt 		= t;	
		this.ntPrev		= t;
		}


	// ---------------
	/// Enable and start timer.
	// ---------------
	public void Start(
		float duration		///< Timer's duration in seconds.
		)
		{
		this.enabled 	= true;
		this.running 	= true;
		this.nt			= 0;
		this.ntPrev 	= 0;
		this.elapsed 	= 0;
		this.duration 	= (duration > 0) ? duration : 0;
		}

	// ---------------
	/// Update timer.	
	// ---------------
	public void Update(float dt)
		{	
		if (!this.enabled)
			return;
		
		this.ntPrev = this.nt;
		
		if (this.running)
			{
			this.elapsed += dt;
			if (this.elapsed > this.duration)
				{
				this.nt = 1.0f;
				this.running = false;	
				}
			else
				{
				if (this.duration > 0.0001f)
					this.nt = (this.elapsed / this.duration);
				else
					this.nt = 0; 
				}	
			}
		}
	
	// --------------------
	/// Disable Timer.
	// --------------------
	public void Disable()
		{
		this.enabled = false;	
		this.running = false;
		}
	}



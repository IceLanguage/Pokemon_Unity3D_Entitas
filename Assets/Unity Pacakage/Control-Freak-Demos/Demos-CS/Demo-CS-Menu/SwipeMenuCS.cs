
using UnityEngine;

[AddComponentMenu("ControlFreak-Demos-CS/SwipeMenuCS")]
public class SwipeMenuCS : MonoBehaviour 
	{
	public float	snapBackTime 		= 0.2f;
	public float	completionDuration 	= 1.0f;
	public bool		cyclical			= true;
	private float	targetPos			= 0;
	public float	swipeSpeedInchesPerSec = 1;		///< Inches per second
	public float 	minChangeDragInches		= 1;	///< 
	[System.NonSerializedAttribute]
	public float	displayPos			= 0,
					windowSize 			= 100,
					curPos				= 0;
	private float	dpi					= 72;

	private float	dampVel				= 0;
	
	[System.NonSerializedAttribute]
	public int 		curItem,
					itemCount;

	private bool	pressed;	
	
	private bool	selected,
					complete,
					justCompleted;
	private float	timeSinceSelection;



	// ---------------
	public void Init(int itemCount, int curItem, float windowSize, float dpi, bool cyclical)
		{
		this.complete 		= false;
		this.justCompleted	= false;
		this.pressed		= false;
		this.selected 		= false;
		//this.curVel			= 0;
		this.windowSize 	= Mathf.Max(2, windowSize);
		this.dpi			= Mathf.Max(40, dpi);
		this.itemCount 		= Mathf.Max(1, itemCount);
		this.curItem 		= Mathf.Clamp(curItem, 0, this.itemCount - 1);;
		this.displayPos 	=
		this.targetPos 		= 
		this.curPos 		= this.curItem * this.windowSize;
		}
	
	
	// -------------------
	public void SetWindowSize(float windowSize, float dpi)
		{
		if ((this.windowSize != windowSize) || (this.dpi != dpi))
			{	
			this.displayPos = 
			this.targetPos 	= this.curItem * this.windowSize;

			this.windowSize = windowSize;
			this.dpi		= dpi;

			this.pressed	= false;
			}
		}


	// ------------
	public int GetCurItem()
		{
		return this.curItem;
		}
	

	// ---------
	public bool Completed()
		{
		return this.complete;
		}

	// ----------
	public bool JustCompleted()
		{
		return this.justCompleted;
		}
	

	// -----------
	public bool Selected()
		{	
		return this.selected;
		}
	
	// -----------
	public bool Pressed()
		{
		return this.pressed;
		}
	
	// -----------
	public float GetTimeSinceSelection()
		{
		return this.timeSinceSelection;
		}


	// ---------------
	public void OnPress()	
		{
		if (this.selected)
			return;

		this.pressed = true;
		//this.curVel = 0;
		}


	// -------------
	public void OnRelease(float vel)
		{
		if (this.selected)
			return;

		this.pressed = false;

		//this.curItem = Mathf.RoundToInt(((this.curPos ) / this.windowSize));
		
		float dragDist = (this.curPos - ((float)this.curItem * this.windowSize));
		
		// Dragged past the half of the window...

		if (Mathf.Abs(dragDist) > (this.windowSize * 0.5f))
			{
			this.curItem = Mathf.RoundToInt(((this.curPos ) / this.windowSize));			
			}

		else if (Mathf.Abs(dragDist) > (this.minChangeDragInches * this.dpi))	
			{
			this.curItem += ((dragDist > 0) ? 1 : -1);
			}

		else
			{
			// "Flick" change...

			int flickItemDelta = Mathf.FloorToInt(vel / (this.swipeSpeedInchesPerSec * this.dpi));
				
			this.curItem += Mathf.Clamp(flickItemDelta, -1, 1);

//Debug.Log("FLick: VEL("+vel+") DELTA("+flickItemDelta+") PxSpeedPerItem("+(this.swipeSpeedInchesPerSec * this.dpi)+")");
	
			//if (vel > 200)
			//	this.curItem += 1;
			//else if (swipeItemDelta < 0) //vel < -200)
			//	this.curItem -= 1;
			}

		if (this.cyclical)
			{
			if (this.curItem >= this.itemCount)
				this.curItem = this.curItem % this.itemCount;
			else if (this.curItem < 0)	
				this.curItem = (this.curItem % this.itemCount) + this.itemCount;	
			}
		else
			{
			this.curItem = Mathf.Clamp(this.curItem, 0, this.itemCount - 1);
			}
		
		this.targetPos = this.curItem * this.windowSize;
		

	
		}
			
	// --------------
	public void OnTap()
		{
		
		if (this.selected)
			return;
	
		this.complete 			= false;
		this.justCompleted 		= false;
		this.timeSinceSelection = 0;
		this.selected 			= true;
		this.pressed 			= false;
		}
	
	


	// ------------
	public void Move(float delta)
		{
		if (this.selected)
			return;

		this.curPos += delta;
		}

	// --------------
	public void UpdateMenu()
		{
		if (this.selected)
			{
			this.justCompleted = false;

			if (!this.complete)
				{
				if (this.timeSinceSelection >= this.completionDuration)
					{
					this.complete = true;
					this.justCompleted = true;
					}
				else
					{
					this.timeSinceSelection += Time.deltaTime;
					}
				}			
			}

		if (!this.pressed)
			{
			this.curPos = this.targetPos;
			}
		
		this.displayPos = Mathf.SmoothDamp(this.displayPos, this.curPos, 
					ref this.dampVel, this.snapBackTime);


		}
	
	


	}
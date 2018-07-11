#pragma strict 

@script AddComponentMenu("ControlFreak-Demos-JS/SwipeMenuJS")

public class SwipeMenuJS extends MonoBehaviour 
	{
	public var	snapBackTime			: float	= 0.2f;
	public var	completionDuration		: float	= 1.0f;
	public var	cyclical				: boolean	= true;
	private var	targetPos				: float	= 0;
	public var	swipeSpeedInchesPerSec 	: float = 2;		// Minimal Flick speed in Inches per second
	public var	minChangeDragInches		: float = 1;	 	// Minimal drag in inches to change current item

	public var	windowSize				: float	= 100;
	public var	curPos					: float = 0;
	public var	displayPos				: float	= 0;

	private var	dpi						: float = 72;

	private var	dampVel				: float = 0;
	
	public var	curItem				: int;
	public var	itemCount			: int;

	private var	pressed				: boolean;	
	
	private var	selected			: boolean;
	private var	complete			: boolean;
	private var	justCompleted		: boolean;
	private var	timeSinceSelection	: float;



	// ---------------
	public function Init(
		itemCount 	: int, 
		curItem 	: int, 
		windowSize 	: float, 
		dpi 		: float, 
		cyclical 	: boolean) : void
		{
		this.complete 		= false;
		this.justCompleted	= false;
		this.pressed		= false;
		this.selected 		= false;
		//this.curVel			= 0;
		this.windowSize 	= Mathf.Max(2, windowSize);
		this.dpi			= Mathf.Max(40, dpi);
		this.itemCount 		= Mathf.Max(1, itemCount);
		this.curItem 		= Mathf.Clamp(curItem, 0, this.itemCount - 1);
		this.displayPos		= 
		this.targetPos 		= 
		this.curPos 		= this.curItem * this.windowSize;
		}


	// -------------------
	public function SetWindowSize(windowSize : float, dpi : float) : void 
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
	public function GetCurItem() : int
		{
		return this.curItem;
		}
	

	// ---------
	public function Completed() : boolean
		{
		return this.complete;
		}

	// ----------
	public function JustCompleted() : boolean
		{
		return this.justCompleted;
		}
	

	// -----------
	public function Selected() : boolean
		{	
		return this.selected;
		}
	
	// -----------
	public function Pressed() : boolean
		{
		return this.pressed;
		}
	
	// -----------
	public function GetTimeSinceSelection() : float
		{
		return this.timeSinceSelection;
		}


	// ---------------
	public function OnPress() : void	
		{
		if (this.selected)
			return;

		this.pressed = true;
		}


	// -------------
	public function OnRelease(vel : float) : void
		{
		if (this.selected)
			return;

		this.pressed = false;


		
		var dragDist : float = (this.curPos - (this.curItem * this.windowSize));
		
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

			var flickItemDelta : int = Mathf.FloorToInt(vel / (this.swipeSpeedInchesPerSec * this.dpi));
			this.curItem += Mathf.Clamp(flickItemDelta, -1, 1);
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
	public function OnTap() : void
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
	public function Move(delta : float) : void
		{
		if (this.selected)
			return;

		this.curPos += delta;
		}

	// --------------
	public function UpdateMenu() : void
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
					this.dampVel, this.snapBackTime);


		}
	


	}
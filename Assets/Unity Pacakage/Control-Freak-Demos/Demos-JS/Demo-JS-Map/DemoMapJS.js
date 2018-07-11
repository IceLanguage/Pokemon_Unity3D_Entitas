
#pragma strict 

@script AddComponentMenu("ControlFreak-Demos-JS/DemoMapJS")

public class DemoMapJS extends MonoBehaviour 
	{
	public var	touchCtrl 	: TouchController;

	public var 	mapTexture 	: Texture2D;

	public var 	mapWidth 	: float		= 1800;	
	public var 	mapHeight 	: float 	= 1050;
	
	public var	mapMinScale : float		= 0.1f;
	public var	mapMaxScale	: float 	= 20.0f;
	
	public var	smoothingTime : float	= 0.15f;

	public var	keepMapInside	: boolean	= true;	
	public var	marginFactor	: float		= 0.4f;	// relative to screen width/height (0.0 - 0.5)


	public var	guiSkin 	: GUISkin;
	public var	popupBox	: PopupBoxJS;


	private var	mapOfs		: Vector2;			// target map transform
	private var mapScale	: float;
	private var mapAngle	: float;	
	
	private var displayOfs		: Vector2;		// map transform used for display and smoothing.
	private var displayScale	: float;
	private var displayAngle	: float;	

	private var guiMatrix		: Matrix4x4;



	

	
	
	// ---------------
	private function SnapDisplayTransform() : void
 		{
		this.displayOfs 	= this.mapOfs;
		this.displayAngle 	= this.mapAngle;
		this.displayScale 	= this.mapScale;
		}



	// --------------------
	function Start() : void
		{
		if (this.touchCtrl == null)
			{
			Debug.LogError("Touch Controller not assigned!!");	
			return;
			}
		

		if (this.mapTexture == null)
			{
			Debug.LogError("Map texture not assigned!");
			return;
			}

		//if (this.mapObject == null)
		//	{
		//	Debug.LogError("Map object not assigned!");
		//	return;
		//	}

		// Init scale and rotation...
		
		this.mapScale 	= 1.0f;
		this.mapAngle 	= 0;

		// Center map on the screen...

		this.mapOfs 	= new Vector2(
			((Screen.width 	/ 2.0f) - (this.mapWidth 	/ 2.0f)),
			((Screen.height / 2.0f) - (this.mapHeight 	/ 2.0f))  );
		
		
		this.SnapDisplayTransform();
		

		// Init controller...

		this.touchCtrl.InitController();	
		}

	

	// ---------------------
	function Update() : void
		{
		if (Input.GetKeyUp(KeyCode.Escape))
			{
			DemoSwipeMenuJS.LoadMenuScene();
			//DemoMenu.LoadMenuLevel();
			return;
			}
		

		// Popup box update...

		if (this.popupBox != null)
			{
			if (!this.popupBox.IsVisible())
				{
				if (Input.GetKeyDown(KeyCode.Space))
					this.popupBox.Show(
						INSTRUCTIONS_TITLE, 
						INSTRUCTIONS_TEXT, 
						INSTRUCTIONS_BUTTON_TEXT);
				}
			else
				{
				if (Input.GetKeyDown(KeyCode.Space))
					this.popupBox.Hide();
				}
			}


		// Touch controls...

		// Get first zone, since there's only one...

		var zone : TouchZone = this.touchCtrl.GetZone(0);
		

		// If two fingers are touching, handle twisting and pinching...

		if (zone.MultiPressed(false, true))
			{
			// Get current mulit-touch position (center) as a pivot point for zoom and rotation...

			var pivot : Vector2 = zone.GetMultiPos(TouchCoordSys.SCREEN_PX);

			
			// If pinched, scale map by non-raw relative pinch scale..

			if (zone.Pinched())
				this.ScaleMap(zone.GetPinchRelativeScale(false), pivot); 
			

			// If twisted, rotate map by this frame's angle delta...

			if (zone.Twisted())
				this.RotateMap(zone.GetTwistDelta(false), pivot);
			}

		// If one finger is touching the screen...

		else
			{

			// Single touch...

			if (zone.UniPressed(false, true))
				{
				if (zone.UniDragged())	
					{
					// Drag the map by this frame's unified touch drag delta...

					var delta : Vector2= zone.GetUniDragDelta(TouchCoordSys.SCREEN_PX, false);

					this.SetMapOffset(this.mapOfs + delta);
					}
				}
			
			// Double tap with two fingers to zoom-out...
			
			if (zone.JustMultiDoubleTapped())
				{
				this.ScaleMap(0.5f, zone.GetMultiTapPos(TouchCoordSys.SCREEN_PX));
				}

			// Double tap with one finger to zoom in...

			else if (zone.JustDoubleTapped())
				{	
				this.ScaleMap(2.0f, zone.GetTapPos(TouchCoordSys.SCREEN_PX));
				}

			}
		
		
		// Keep map on the screen...
		
		if (this.keepMapInside)
			{
			this.marginFactor = Mathf.Clamp(this.marginFactor, 0, 0.5f);

			var safeRect : Rect = new Rect(
				Screen.width * this.marginFactor, 
				Screen.height * this.marginFactor,
				Screen.width * (1.0f - (2.0f * this.marginFactor)),	
				Screen.height * (1.0f - (2.0f * this.marginFactor)));	
	
	
			var mapRect : Rect = this.GetMapBoundingBox();
			
	
			if (mapRect.xMax < safeRect.xMin) 
				this.mapOfs.x -= (mapRect.xMax - safeRect.xMin);
	
			else if (mapRect.xMin > safeRect.xMax) 
				this.mapOfs.x -= (mapRect.xMin - safeRect.xMax);
	
			if (mapRect.yMax < safeRect.yMin) 
				this.mapOfs.y -= (mapRect.yMax - safeRect.yMin);
	
			else if (mapRect.yMin > safeRect.yMax) 
				this.mapOfs.y -= (mapRect.yMin - safeRect.yMax);
			}


		// Smooth map transform...
		
		if ((Time.deltaTime >= this.smoothingTime))
			this.SnapDisplayTransform();
		else
			{
			var st : float = (Time.deltaTime / this.smoothingTime);

			this.displayOfs 	= Vector2.Lerp(	this.displayOfs, 	this.mapOfs, 	st);
			this.displayScale 	= Mathf.Lerp(	this.displayScale, 	this.mapScale, 	st);
			this.displayAngle 	= Mathf.Lerp(	this.displayAngle,	this.mapAngle, 	st);
			}

		//this.TransformMap();
		}



	// --------------------
	function OnGUI() : void
		{
		GUI.skin = this.guiSkin;

		var initialMatrix : Matrix4x4 = GUI.matrix;
		
		GUI.matrix = this.GetMapDisplayMatrix();
		
		GUI.DrawTexture(new Rect(0,0, this.mapWidth, this.mapHeight), this.mapTexture);

		GUI.matrix = initialMatrix;

		//GUILayout.Box("Map Demo.\nPress Escape / Back to return to main menu.");
		if ((this.popupBox != null) && this.popupBox.IsVisible())
			this.popupBox.DrawGUI();
		else
			{
			GUI.color = Color.white;
			GUI.Label(new Rect(10, 10, Screen.width - 100, 100),
				"Map Demo - Press [Space] for help, [Esc] to quit."); 
			}

		
		}


	// --------------------
	private function RotateMap(angleDelta : float , pivotPos : Vector2) : void
		{
		var v : Vector3 = (Quaternion.Euler(0, 0, -angleDelta) * (this.mapOfs - pivotPos));
		this.mapOfs.x = pivotPos.x + v.x;
		this.mapOfs.y = pivotPos.y + v.y;
	
		this.SetMapOffset(this.mapOfs);
		this.mapAngle -= angleDelta; 
		}
	


	// ---------------------
	private function ScaleMap(relativeScale : float, pivotPos : Vector2) : void
		{
		var prevScale : float = this.mapScale;
		this.SetScale(this.mapScale * relativeScale);

		this.SetMapOffset(pivotPos + ((this.mapOfs - pivotPos) * (this.mapScale / prevScale)));
		}


	// -------------------
	private function SetScale(scale : float) : void
		{
		this.mapScale = Mathf.Clamp(scale, this.mapMinScale, this.mapMaxScale);
		}


	// ---------------
	private function SetMapOffset(ofs : Vector2 ) : void 
		{
		this.mapOfs.x = ofs.x; //Mathf.Clamp(ofs.x, -this.mapWidth, this.mapWidth);
		this.mapOfs.y = ofs.y; //Mathf.Clamp(ofs.y, -this.mapHeight, this.mapHeight);
		}

	
	// --------------
	private function GetMapDisplayMatrix() : Matrix4x4
		{
		return Matrix4x4.TRS(
			this.displayOfs, 
		 	Quaternion.Euler(0, 0, this.displayAngle),
			new Vector3(this.displayScale, this.displayScale, this.displayScale));
		}
	
	// --------------
	private function GetMapTargetMatrix() : Matrix4x4
		{
		return Matrix4x4.TRS(
			this.mapOfs, 
		 	Quaternion.Euler(0, 0, this.mapAngle),
			new Vector3(this.mapScale, this.mapScale, this.mapScale));
		}
	

	// --------------
	private function GetMapBoundingBox() : Rect
		{
		var mat : Matrix4x4 = this.GetMapTargetMatrix();

		var box : Bounds = new Bounds(
			mat.MultiplyPoint3x4(new Vector3(0,	0, 0)), Vector3.zero);

		box.Encapsulate(mat.MultiplyPoint3x4(new Vector3(this.mapWidth,	0,				0)));
		box.Encapsulate(mat.MultiplyPoint3x4(new Vector3(0,				this.mapHeight,	0)));
		box.Encapsulate(mat.MultiplyPoint3x4(new Vector3(this.mapWidth,	this.mapHeight,	0)));

		return new Rect(box.min.x, box.min.y, box.size.x, box.size.y);
		}


	
	// ---------------

#if UNITY_3_5
	private static var	CAPTION_COLOR_BEGIN 		: String	= "";
	private static var	CAPTION_COLOR_END 			: String	= "";
#else
	private static var	CAPTION_COLOR_BEGIN 		: String	= "<color='#FF0000'>";
	private static var	CAPTION_COLOR_END 			: String	= "</color>";
#endif

	private static var INSTRUCTIONS_TITLE 		: String =	"Inctructions";
	private static var INSTRUCTIONS_BUTTON_TEXT : String = 	"";
	private static var INSTRUCTIONS_TEXT			: String = 
		CAPTION_COLOR_BEGIN +
		"* Map Pan.\n" + 
		CAPTION_COLOR_END +
		"Drag to pan the map.\n" +
		"\n" +
		CAPTION_COLOR_BEGIN +
		"* Map Zoom.\n" +
		CAPTION_COLOR_END +
		"Place two fingers on the screen and spread them away to zoom out or pinch to zoom in.\n" +			"\n" +
		CAPTION_COLOR_BEGIN +
		"* Map Rotation.\n" + 
		CAPTION_COLOR_END +
		"Place two fingers on the screen and twist them to rotate the map.\n" +
		"\n" +
		CAPTION_COLOR_BEGIN +	
		"* Point Zoom-in.\n" +
		CAPTION_COLOR_END +
		"Double tap with one finger on the map to zoom to that point.\n" +
		"\n" +
		CAPTION_COLOR_BEGIN +
		"* Point Zoom-out.\n" +	
		CAPTION_COLOR_END +
		"Double tap with TWO fingers to zoom out.";



	}

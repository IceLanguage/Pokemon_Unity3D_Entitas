using UnityEngine;

[AddComponentMenu("ControlFreak-Demos-CS/DemoMapCS")]
public class DemoMapCS : MonoBehaviour 
	{
	public TouchController 	touchCtrl;

	public Texture2D		mapTexture;

	public float			mapWidth	= 1800,	
							mapHeight 	= 1050;
	
	public float			mapMinScale	= 0.1f,
							mapMaxScale	= 20.0f;
	
	public float 			smoothingTime = 0.15f;

	public bool				keepMapInside	= true;	
	public float			marginFactor	= 0.4f;	// relative to screen width/height (0.0 - 0.5)

	public GUISkin			guiSkin;
	public PopupBoxCS		popupBox;


	private Vector2			mapOfs;			// target map transform
	private float			mapScale,
							mapAngle;	
	
	private Vector2			displayOfs;		// map transform used for display and smoothing.
	private float			displayScale,
							displayAngle;	

	private Matrix4x4		guiMatrix;





	
	
	// ---------------
	private void SnapDisplayTransform()
		{
		this.displayOfs 	= this.mapOfs;
		this.displayAngle 	= this.mapAngle;
		this.displayScale 	= this.mapScale;
		}



	// --------------------
	private void Start()
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

	
	
public Rect debugBox, debugSafeRect;
 
	// ---------------------
	private void Update()
		{
		if (Input.GetKeyUp(KeyCode.Escape))
			{
			DemoSwipeMenuCS.LoadMenuScene();
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

		TouchZone zone = this.touchCtrl.GetZone(0);
		

		// If two fingers are touching, handle twisting and pinching...

		if (zone.MultiPressed(false, true))
			{
			// Get current mulit-touch position (center) as a pivot point for zoom and rotation...

			Vector2 pivot = zone.GetMultiPos(TouchCoordSys.SCREEN_PX);

			
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

					Vector2 delta = zone.GetUniDragDelta(TouchCoordSys.SCREEN_PX, false);

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

			Rect safeRect = new Rect(
				Screen.width * this.marginFactor, 
				Screen.height * this.marginFactor,
				Screen.width * (1.0f - (2.0f * this.marginFactor)),	
				Screen.height * (1.0f - (2.0f * this.marginFactor)));	
	
	
			Rect mapRect = this.GetMapBoundingBox();
			
	
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
			float st = (Time.deltaTime / this.smoothingTime);

			this.displayOfs 	= Vector2.Lerp(	this.displayOfs, 	this.mapOfs, 	st);
			this.displayScale 	= Mathf.Lerp(	this.displayScale, 	this.mapScale, 	st);
			this.displayAngle 	= Mathf.Lerp(	this.displayAngle,	this.mapAngle, 	st);
			}

		//this.TransformMap();
		}



	// --------------------
	private void OnGUI()
		{
		GUI.skin = this.guiSkin;

		Matrix4x4 initialMatrix = GUI.matrix;

		// Draw the map...
		
		GUI.matrix = this.GetMapDisplayMatrix();
		
		GUI.DrawTexture(new Rect(0,0, this.mapWidth, this.mapHeight), this.mapTexture);
		

		// Draw the markers...

		GUI.matrix = this.GetMarkerMatrix();
			//Matrix4x4.TRS(this.displayOfs, Quaternion.Euler(0,0, this.displayAngle), Vector3.one);

		GUI.Button(this.GetMarkerRect(new Vector2(900, 400), new Vector2(50, 50)), "HEY!!");
		

		// Reset matrix...

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
	private void RotateMap(float angleDelta, Vector2 pivotPos)
		{
		Vector3 v = (Quaternion.Euler(0, 0, -angleDelta) * (this.mapOfs - pivotPos));
		this.mapOfs.x = pivotPos.x + v.x;
		this.mapOfs.y = pivotPos.y + v.y;
	
		this.SetMapOffset(this.mapOfs);
		this.mapAngle -= angleDelta; 
		}
	


	// ---------------------
	private void ScaleMap(float relativeScale, Vector2 pivotPos)
		{
		float prevScale = this.mapScale;
		this.SetScale(this.mapScale * relativeScale);

		this.SetMapOffset(pivotPos + ((this.mapOfs - pivotPos) * (this.mapScale / prevScale)));
		}


	// -------------------
	private void SetScale(float scale)
		{
		this.mapScale = Mathf.Clamp(scale, this.mapMinScale, this.mapMaxScale);
		}


	// ---------------
	private void SetMapOffset(Vector2 ofs)
		{
		this.mapOfs.x = ofs.x; //Mathf.Clamp(ofs.x, -this.mapWidth, this.mapWidth);
		this.mapOfs.y = ofs.y; //Mathf.Clamp(ofs.y, -this.mapHeight, this.mapHeight);
		}


	
	// --------------
	private Matrix4x4 GetMapTargetMatrix()
		{
		return Matrix4x4.TRS(
			this.mapOfs, 
		 	Quaternion.Euler(0, 0, this.mapAngle),
			new Vector3(this.mapScale, this.mapScale, this.mapScale));
		}
	
	// --------------
	private Matrix4x4 GetMapDisplayMatrix()
		{
		return Matrix4x4.TRS(
			this.displayOfs, 
		 	Quaternion.Euler(0, 0, this.displayAngle),
			new Vector3(this.displayScale, this.displayScale, this.displayScale));
		}

	// -------------
	private Matrix4x4 GetMarkerMatrix()
		{
		return Matrix4x4.TRS(
			this.displayOfs, 
		 	Quaternion.Euler(0, 0, this.displayAngle),
			Vector3.one);
		}

	// -------------
	private Rect GetMarkerRect(
		Vector2 pos, 
		Vector2 size, 
		Vector2 markerRelAnchor)	
		{
		return new Rect(
			((pos.x * this.displayScale) - (size.x * markerRelAnchor.x)),
			((pos.y * this.displayScale) - (size.y * markerRelAnchor.y)),
			size.x, 
			size.y);			
		}

	// -------------
	private Rect GetMarkerRect(Vector2 pos, Vector2 size)
		{
		return this.GetMarkerRect(pos, size, new Vector2(0.5f, 0.5f));	
		}


	// --------------
	private Rect GetMapBoundingBox()
		{
		Matrix4x4	mat = this.GetMapTargetMatrix();

		Bounds		box = new Bounds(
			mat.MultiplyPoint3x4(new Vector3(0,	0, 0)), Vector3.zero);

		box.Encapsulate(mat.MultiplyPoint3x4(new Vector3(this.mapWidth,	0,				0)));
		box.Encapsulate(mat.MultiplyPoint3x4(new Vector3(0,				this.mapHeight,	0)));
		box.Encapsulate(mat.MultiplyPoint3x4(new Vector3(this.mapWidth,	this.mapHeight,	0)));

		return new Rect(box.min.x, box.min.y, box.size.x, box.size.y);
		}



	// ---------------

#if UNITY_3_5
	private const string	CAPTION_COLOR_BEGIN 		= "";	
	private const string	CAPTION_COLOR_END 			= "";
#else
	private const string	CAPTION_COLOR_BEGIN 		= "<color='#FF0000'>";	
	private const string	CAPTION_COLOR_END 			= "</color>";
#endif

	private const string 
		INSTRUCTIONS_TITLE 			= "Inctructions",
		INSTRUCTIONS_BUTTON_TEXT 	= "",
		INSTRUCTIONS_TEXT  			= 
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

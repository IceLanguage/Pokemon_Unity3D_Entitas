#pragma strict 

@script AddComponentMenu("ControlFreak-Demos-JS/PopupBoxJS")

public class PopupBoxJS extends MonoBehaviour 
	{
	public var	guiSkin 	: GUISkin;
	public var	guiDepth	: int 		= 100;
	
	
	private var 	visible 	: boolean;
	private var 	complete 	: boolean;
	private var 	text		: String;
	private var 	titleText	: String;
	private var 	buttonText	: String;
	private var 	textSize	: Vector2;
	private var		scrollPos	: Vector2;
	
	private var		boxRect		: Rect;
					
	
	// ------------------
	public function Show(
		rect		: Rect,
		title		: String, 
		text		: String) : void
		{
		this.Show(rect, title, text, "");
		}
	

	// ---------------
	public function Show(
		rect		: Rect,
		title		: String, 
		text		: String,
		buttonText : String) : void
		{
		if (this.guiSkin == null)
			return;

		this.visible = true;
		this.complete = false;

		this.titleText = title;
		this.buttonText = buttonText;
		this.text = text;

		this.boxRect = rect;

		
		}

	// --------------
	public function Show(title : String, text : String) : void
		{
		this.Show(title, text);
		}


	// --------------
	public function Show(title : String, text : String, buttonText : String) : void
		{
		this.Show(new Rect(
			Screen.width 	* 0.05f, 
			Screen.height 	* 0.05f,
			Screen.width 	* 0.9f,
			Screen.height 	* 0.9f), 
			title,
			text,
			buttonText);
		}	


	// -------------
	public function End() : void
		{	
		this.complete = true;
		}


	
	// --------------
	public function IsComplete() : boolean
		{
		return this.complete;
		}


	// -------------
	public function Hide() : void
		{
		this.visible = false;
		}


	// ------------
	public function IsVisible() : boolean
		{
		return this.visible;
		}
	

	// -------------
	public function DrawGUI() : void
		{	
		if (!this.visible)
			return;
		
		var initialGuiEnabled 	: boolean 	= GUI.enabled;
		var initialSkin			: GUISkin 	= GUI.skin;
		var	initialDepth		: int		= GUI.depth;
		
		GUI.enabled 		= !this.complete;
		GUI.skin 			= this.guiSkin;
		GUI.depth			= this.guiDepth;

		GUI.color 			= Color.white;
		GUI.backgroundColor = Color.white;
		GUI.contentColor 	= Color.white;


		GUI.Box(this.boxRect, "");
		GUILayout.BeginArea(this.boxRect);

		GUILayout.Label(this.titleText);	
		
		this.scrollPos = GUILayout.BeginScrollView(this.scrollPos);
		
		GUILayout.Box(this.text, this.guiSkin.customStyles[0]);

		GUILayout.EndScrollView();
	
		if ((this.buttonText != null) && (this.buttonText.Length > 0))
			{
			if (GUILayout.Button(this.buttonText))
				this.End();
			}

		GUILayout.EndArea();


		GUI.depth	= initialDepth;
		GUI.skin 	= initialSkin;
		GUI.enabled = initialGuiEnabled;
		}
	}
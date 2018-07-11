
using UnityEngine;


[AddComponentMenu("ControlFreak-Demos-CS/PopupBoxCS")]
public class PopupBoxCS : MonoBehaviour 
	{
	public GUISkin 	guiSkin;
	public int		guiDepth = 100;
	
	private bool 	visible,
					complete;
	private string 	text,
					titleText,
					buttonText;
	private Vector2 textSize,
					scrollPos;
	
	private Rect 	boxRect,
					titleRect,
					buttonRect,
					textRect;
					

	// ---------------
	public void Show(
		Rect 		rect,
		string		title, 
		string		text, 
		string		buttonText = "")
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
	public void Show(string title, string text, string buttonText = "")
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
	public void End()
		{	
		this.complete = true;
		}


	
	// --------------
	public bool IsComplete()
		{
		return this.complete;
		}


	// -------------
	public void Hide()
		{
		this.visible = false;
		}


	// ------------
	public bool IsVisible()
		{
		return this.visible;
		}
	

	// -------------
	public void DrawGUI()
		{	
		if (!this.visible)
			return;
		
		bool 	initialGuiEnabled 	= GUI.enabled;
		GUISkin initialSkin 		= GUI.skin;
		int		initialDepth		= GUI.depth;
		
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
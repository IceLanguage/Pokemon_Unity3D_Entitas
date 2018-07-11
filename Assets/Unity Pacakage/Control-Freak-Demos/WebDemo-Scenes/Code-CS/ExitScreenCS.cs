using UnityEngine;


public class ExitScreenCS : MonoBehaviour 
	{
	public float forcedDuration = 2;
	const string siteUrl	= "https://www.facebook.com/DansGameTools/app_251458316228"; //"http://facebook.com/DansGameTools";
	const string assetStoreUrl = "http://u3d.as/5GT";
	public GUISkin guiSkin;

	private float elapsed;

	
	// ---------------
	private void Start () 
		{
		this.elapsed = 0;
		}
	
	// ----------------
	private void Update () 
		{ 
		this.elapsed += Time.deltaTime;
		if (this.elapsed > this.forcedDuration)
			{
			if (Input.GetKeyUp(KeyCode.Escape))
				Application.Quit();
			}
	
		}


	// -----------------
	private void OnGUI()
		{
		GUI.skin = this.guiSkin;
		

		GUI.color = new Color(0.4f, 0.4f, 0.4f, 1.0f);

		//GUILayout.BeginArea(new Rect(0, Screen.height * 0.1f, Screen.width, 200));

		//GUI.Box(new Rect(0, Screen.height * 0.1f, Screen.width, 200), 
		GUI.Label(new Rect(0, Screen.height * 0.05f, Screen.width, 200), 
		//GUILayout.Box(	
			"Thank you for trying our demo!");
		
		//GUILayout.EndArea();


		GUI.color = Color.white;

		//if (GUI.Button(new Rect(Screen.width * 0.1f, Screen.height - 100,
		//	Screen.width * 0.8f, 80), "Go to Asset Store!"))
		//	{
		//	Application.OpenURL(this.siteUrl);
		//	Application.Quit();
		//	}  
		
		GUILayout.BeginArea(new Rect(Screen.width * 0.1f, Screen.height - 150,
			Screen.width * 0.8f, 130));

		//GUILayout.BeginHorizontal();
		
//#if (!UNITY_WEBPLAYER || UNITY_EDITOR)
		


		if (GUILayout.Button("Visit our Shop"))
			{
			Application.OpenURL(ExitScreenCS.siteUrl);
			Application.Quit();
			}  

		if (GUILayout.Button("Visit Asset Store"))
			{
			Application.OpenURL(ExitScreenCS.assetStoreUrl);
			Application.Quit();
			}  
//#endif
		
		if (GUILayout.Button("Return to Main Menu"))
			{	
			DemoSwipeMenuCS.LoadMenuScene();
			return;
			}

		GUILayout.EndArea();

		}
}

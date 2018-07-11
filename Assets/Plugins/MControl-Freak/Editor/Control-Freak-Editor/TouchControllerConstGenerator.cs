// -----------------------------------------------
// Control Freak - The Ultimate Virtual Controller
// Copyright (C) 2013 Dan's Game Tools
// -----------------------------------------------

using UnityEngine;
using UnityEditor;


// -------------------------
// Constant Generation Dialog
// -------------------------

public class TouchControllerConstGenerator  : EditorWindow
	{
	private TouchController joy;

	private string 
		buttonLabel,
		stickLabel,
		zoneLabel;

	private bool
		prefixMode,	
		uppercaseMode,
		spacesToUnderscores;
		
	private bool
		privateMode,
		jsMode;




	// -------------------
	//public TouchControllerConstGenerator(TouchController joy)
	public void InitAndShow(TouchController joy)
		{
		this.joy = joy;

		this.buttonLabel 			= "BTN";
		this.stickLabel				= "STICK";
		this.zoneLabel 				= "ZONE";

		this.prefixMode 			= true;
		this.uppercaseMode 			= true;
		this.spacesToUnderscores	= true;

		this.ShowUtility();
		}


	// ---------------------
	private void OnGUI()
		{
		this.buttonLabel 	= EditorGUILayout.TextField("Button label", this.buttonLabel);
		this.stickLabel 	= EditorGUILayout.TextField("Stick label", this.stickLabel);
		this.zoneLabel 		= EditorGUILayout.TextField("Zone label", this.zoneLabel);

		this.prefixMode		= EditorGUILayout.Toggle("Prefix with labels", this.prefixMode);
		this.uppercaseMode	= EditorGUILayout.Toggle("Uppercase", this.uppercaseMode);
		this.spacesToUnderscores = 
								EditorGUILayout.Toggle("Spaces to underscores", this.spacesToUnderscores);
		this.jsMode 		= EditorGUILayout.Toggle("JavaScript Version", this.jsMode);
		this.privateMode	= EditorGUILayout.Toggle("Private Constants", this.privateMode);

		if (GUILayout.Button("Generate"))
			{
			string s = this.Generate();
	
			if (EditorUtility.DisplayDialog("Control Freak Const Generator",
				"Generated text:\n" + s + "\n\nPress OK to copy it to clipboard.",
				"OK", "Back"))
				{
				EditorGUIUtility.systemCopyBuffer = s;
				this.Close();
				}
			}

		//if (GUILayout.Button("Close"))
		//	this.Close();
		}
			


		
	// -------------------
	private string BuildString(string name, string label, int val)
		{
		string s = "";
		if (this.prefixMode)
			s += label + " ";
		

		s += name;
	
		if (!this.prefixMode)
			s += " " + label;
					
		s = s.Replace(" ", (this.spacesToUnderscores ? "_" : ""));
		s = s.Replace("-", (this.spacesToUnderscores ? "_" : ""));
		
		if (this.uppercaseMode)
			s = s.ToUpper();
		
		if (this.jsMode)
			{
			s = (this.privateMode ? "private" : "public") + 
				" static var " + s + "\t: int = " + val + ";"; 
			}
		else
			{
			s = (this.privateMode ? "private" : "public") + 
				" const int " + s + "\t= " + val + ";"; 			
			}

		return s;
		} 
	
	// ----------------------
	private string Generate()
		{
		string s = "";


		// Sticks...

		if ((this.joy.sticks != null) || (this.joy.sticks.Length > 0))	
			{
			for (int si = 0; si < this.joy.sticks.Length; ++si)
				{
				s += "\t" + this.BuildString(this.joy.sticks[si].name, 
					this.stickLabel, si) + "\n";
				}
			}

		// Zones...

		if ((this.joy.touchZones != null) || (this.joy.touchZones.Length > 0))	
			{
			for (int zi = 0; zi < this.joy.touchZones.Length; ++zi)
				{
				s += "\t" + this.BuildString(this.joy.touchZones[zi].name, 
					this.zoneLabel, zi) + "\n";
				}
			}

		return s;
		}
	}



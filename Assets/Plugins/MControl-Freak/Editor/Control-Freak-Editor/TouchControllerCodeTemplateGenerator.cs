// -----------------------------------------------
// Control Freak - The Ultimate Virtual Controller
// Copyright (C) 2013 Dan's Game Tools
// -----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


// -------------------------
// Code Template Generation Dialog
// -------------------------

public class TouchControllerCodeTemplateGenerator  : EditorWindow
	{
	private TouchController joy;

	private string 
		stickLabel,
		zoneLabel;

	public bool
		genCtrlVar,
		skipThisKeyword,
		genLayoutSection,
		genCustomGuiSection;

	public string
		ctrlVarName;

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

		this.stickLabel				= "STICK";
		this.zoneLabel 				= "ZONE";

		this.ctrlVarName			= "ctrl";

		this.prefixMode 			= true;
		this.uppercaseMode 			= true;
		this.spacesToUnderscores	= true;
		
		this.genCtrlVar				= true;
		this.genCustomGuiSection	= false;
		this.genLayoutSection		= false;

		this.LoadSettings();
		
		this.ShowUtility();

		this.title = "Control Freak Code Generator";


		}


	// ---------
	private enum CodeLang
		{
		CS,
		JAVSCRIPT
		}
	
	private static string[] LANG_NAMES = new string[]{ "C#", "JavaScript" };

	// ---------------------
	private void OnGUI()
		{
		//this.jsMode 		= EditorGUILayout.Toggle("JavaScript Version", this.jsMode);
		this.jsMode	= (EditorGUILayout.Popup("Language", 
			(this.jsMode ? 1 : 0), LANG_NAMES) == 1);	


		EditorGUILayout.Space();
		EditorGUILayout.LabelField("[ Constant Generation Settings ]");
		EditorGUILayout.Space();

		this.stickLabel 	= EditorGUILayout.TextField("Stick label", this.stickLabel);
		this.zoneLabel 		= EditorGUILayout.TextField("Zone label", this.zoneLabel);

		this.prefixMode		= EditorGUILayout.Toggle("Prefix with labels", this.prefixMode);
		this.uppercaseMode	= EditorGUILayout.Toggle("Uppercase", this.uppercaseMode);
		this.spacesToUnderscores = 
								EditorGUILayout.Toggle("Spaces to underscores", this.spacesToUnderscores);
		this.privateMode	= EditorGUILayout.Toggle("Private Constants", this.privateMode);

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("[ Code Generation Settings ]");
		EditorGUILayout.Space();
		
		this.ctrlVarName		= EditorGUILayout.TextField("Controller var", this.ctrlVarName);
		this.skipThisKeyword	= EditorGUILayout.Toggle("Skip 'this.' keyword", this.skipThisKeyword);
		this.genCtrlVar			= EditorGUILayout.Toggle("Gen. Controller Member Var", this.genCtrlVar);
		this.genLayoutSection	= EditorGUILayout.Toggle("Gen. Custom Layout Section", this.genLayoutSection);
		this.genCustomGuiSection= EditorGUILayout.Toggle("Gen. Custom GUI Section", this.genCustomGuiSection);
	
	
		EditorGUILayout.Space();
	

		if (GUILayout.Button("Generate Code"))
			{
			string s = this.Generate(true);
	
			if (EditorUtility.DisplayDialog("Control Freak Code Generator",
				"Generated text:\n" + ClipStr(s, 20) + "\n\nPress OK to copy it to clipboard.",
				"OK", "Back"))
				{
				EditorGUIUtility.systemCopyBuffer = s;
				//this.Close();
				this.SaveSettings();
				}
			}
		
		if (GUILayout.Button("Generate Constants Only"))
			{
			string s = this.Generate(false);
	
			if (EditorUtility.DisplayDialog("Control Freak Code Generator",
				"Generated text:\n" + ClipStr(s, 20) + "\n\nPress OK to copy it to clipboard.",
				"OK", "Back"))
				{
				EditorGUIUtility.systemCopyBuffer = s;
				this.SaveSettings();
				//this.Close();
				}
			}

		//if (GUILayout.Button("Close"))
		//	this.Close();
		}
			
	
	
	// ---------------------
	private const string
		CFG_JS_MODE			= "CF.Code.JsMode",
		CFG_UPPER			= "CF.Code.UpperCase",
		CFG_UNDERSCORES		= "CF.Code.Underscores",
		CFG_CONST_PREFIX	= "CF.Code.ConstPrefix",
		CFG_PRIVATE_CONST	= "CF.Code.PrivateConst",	
		CFG_STICK_LABEL		= "CF.Code.StickLabel",	
		CFG_ZONE_LABEL		= "CF.Code.ZoneLabel",	
		CFG_LAYOUT_CODE		= "CF.Code.LayoutCode",
		CFG_SKIP_THIS		= "CF.Code.SkipThis",
		CFG_GUI_CODE		= "CF.Code.GuiCode",
		CFG_CTRL_NAME		= "CF.Code.CtrlName",
		CFG_CTRL_VAR		= "CF.Code.CtrlVar";

	// ---------------------
	private void LoadSettings()
		{
		this.jsMode				= EditorPrefs.GetBool(CFG_JS_MODE,		this.jsMode);
		this.uppercaseMode		= EditorPrefs.GetBool(CFG_UPPER, 		this.uppercaseMode);
		this.spacesToUnderscores= EditorPrefs.GetBool(CFG_UNDERSCORES, 	this.spacesToUnderscores);
		this.prefixMode			= EditorPrefs.GetBool(CFG_CONST_PREFIX,	this.prefixMode);
		this.privateMode		= EditorPrefs.GetBool(CFG_PRIVATE_CONST,this.privateMode);
		this.genLayoutSection	= EditorPrefs.GetBool(CFG_LAYOUT_CODE,	this.genLayoutSection);
		this.genCustomGuiSection= EditorPrefs.GetBool(CFG_GUI_CODE,		this.genCustomGuiSection);
		this.skipThisKeyword	= EditorPrefs.GetBool(CFG_SKIP_THIS,	this.skipThisKeyword);
		this.stickLabel			= EditorPrefs.GetString(CFG_STICK_LABEL,	this.stickLabel);
		this.zoneLabel			= EditorPrefs.GetString(CFG_ZONE_LABEL,	this.zoneLabel);
		this.ctrlVarName		= EditorPrefs.GetString(CFG_CTRL_NAME,	this.ctrlVarName);
		this.genCtrlVar			= EditorPrefs.GetBool(CFG_CTRL_VAR, 	this.genCtrlVar);
		}

	// -------------------
	private void SaveSettings()
		{
		EditorPrefs.SetBool(CFG_JS_MODE,		this.jsMode);
		EditorPrefs.SetBool(CFG_UPPER,	 		this.uppercaseMode);
		EditorPrefs.SetBool(CFG_UNDERSCORES, 	this.spacesToUnderscores);
		EditorPrefs.SetBool(CFG_CONST_PREFIX,	this.prefixMode);
		EditorPrefs.SetBool(CFG_PRIVATE_CONST,	this.privateMode);
		EditorPrefs.SetBool(CFG_LAYOUT_CODE,	this.genLayoutSection);
		EditorPrefs.SetBool(CFG_GUI_CODE,		this.genCustomGuiSection);
		EditorPrefs.SetBool(CFG_SKIP_THIS,		this.skipThisKeyword);
		EditorPrefs.SetBool(CFG_CTRL_VAR, 		this.genCtrlVar);
		EditorPrefs.SetString(CFG_STICK_LABEL,	this.stickLabel);
		EditorPrefs.SetString(CFG_ZONE_LABEL,	this.zoneLabel);
		EditorPrefs.SetString(CFG_CTRL_NAME,	this.ctrlVarName);
		}




	// ---------------------
	private string AdaptString(string s)
		{
		string ctrlVarStr = (this.skipThisKeyword ? "" : "this.") + this.ctrlVarName;

		s = s.Replace("%justctrl%", this.ctrlVarName);
		s = s.Replace("%ctrl%", 	ctrlVarStr);

		s = s.Replace("CTRL_VAR", 	ctrlVarStr);

		return s;
		}

		/*
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

*/
	
	// ----------------------
	private string Generate(bool fullCode)
		{
		// Build controls...

		List<ControlParams>	controls = new List<ControlParams>();

		for (int si = 0; si < this.joy.sticks.Length; ++si)
			controls.Add(new ControlParams(this, this.joy.sticks[si], si));
		for (int zi = 0; zi < this.joy.touchZones.Length; ++zi)
			controls.Add(new ControlParams(this, this.joy.touchZones[zi], zi));


		
		string s = "";

		// Add controller var...
		
		if (fullCode)
			{
			s += this.AdaptString(CTRL_VAR_HEADER);
			s += this.AdaptString(this.jsMode ? JS_CTRL_VAR_DECL : CS_CTRL_VAR_DECL);
			s += SEPARATOR;
			}

		// Add constants...
	
		s += this.AdaptString(CONST_HEADER);
		
		for (int i = 0; i < controls.Count; ++i)
			s += controls[i].GenConstDecl(); 
		
		s += SEPARATOR;



		// Add Start function...
	
		if (fullCode && !this.joy.automaticMode)
			{
			s += this.AdaptString(START_HEADER);
			s += this.AdaptString(this.jsMode ? JS_START_BEGIN : CS_START_BEGIN);		
			s += this.AdaptString(this.jsMode ? JS_MANUAL_INIT : CS_MANUAL_INIT);
			s += this.AdaptString(this.jsMode ? JS_START_END : CS_START_END);
			s += SEPARATOR;
			}


		if (fullCode)
			{
			s += this.AdaptString(UPDATE_HEADER);
			s += this.AdaptString(this.jsMode ? JS_UPDATE_BEGIN :  CS_UPDATE_BEGIN);
			

			// Add Variable declarations...
	
			s += this.AdaptString(VAR_DECL_HEADER);
	
			for (int i = 0; i < controls.Count; ++i)
				s += controls[i].GenVarDecl(); 
			
			s += SEPARATOR;
			
			
			// Manual Poll...

			if (!this.joy.automaticMode)
				{
				s += this.AdaptString(this.jsMode ? JS_MANUAL_POLL : CS_MANUAL_POLL);
				s += SEPARATOR;
				}


			// Add Custom Layout Code...
			
			if (fullCode && this.genLayoutSection)
				{
				s += this.AdaptString(LAYOUT_CODE_HEADER);
				s += this.AdaptString(this.jsMode ? JS_LAYOUT_CODE_BEGIN : CS_LAYOUT_CODE_BEGIN);
				
				for (int i = 0; i < controls.Count; ++i)
					s += controls[i].GenLayoutCode();
	
				s += this.AdaptString(this.jsMode ? JS_LAYOUT_CODE_END : CS_LAYOUT_CODE_END);
				s += SEPARATOR;
				}
			

			// Manual Update...

			if (!this.joy.automaticMode)
				{
				s += this.AdaptString(this.jsMode ? JS_MANUAL_UPDATE : CS_MANUAL_UPDATE);
				s += SEPARATOR;
				}



	
			// Add Input Code...
			
			s += this.AdaptString(INPUT_CODE_HEADER);
			
			for (int i = 0; i < controls.Count; ++i)
				s += controls[i].GenInputCode();
		
			s += SEPARATOR;
			
			// End of Update()...

			s += this.AdaptString(this.jsMode ? JS_UPDATE_END :  CS_UPDATE_END);
			s += SEPARATOR;

			}
		

		// GUI Code..

		if (fullCode && (!this.joy.automaticMode || this.joy.manualGui))
			{
			s += this.AdaptString(ONGUI_HEADER);
			s += this.AdaptString(this.jsMode ? JS_ONGUI_BEGIN : CS_ONGUI_BEGIN);
			
			
			// Add manual GUI drawing...

			if (!this.joy.automaticMode || this.joy.manualGui)
				{
				s += this.AdaptString(this.jsMode ? JS_MANUAL_DRAW_GUI : CS_MANUAL_DRAW_GUI);
				s += SEPARATOR;
				}

			// Custom GUI section...

			if (this.genCustomGuiSection)
				{
				// Add Variable declarations...
		
				s += this.AdaptString(VAR_DECL_HEADER);
		
				for (int i = 0; i < controls.Count; ++i)
					s += controls[i].GenVarDecl(); 
				
				s += SEPARATOR;


				// Custom GUI Drawing code...

				s += this.AdaptString(CUSTOM_GUI_CODE_HEADER);

				for (int i = 0; i < controls.Count; ++i)
					s += controls[i].GenCustomGUICode();

				s += SEPARATOR;
				}

			
			// End of OnGUI()...

			s += this.AdaptString(this.jsMode ? JS_ONGUI_END : CS_ONGUI_END);
			s += SEPARATOR;
			}

		return s;
		}


	// ------------------
	private struct ControlParams
		{	
		public string 		srcName;
		public string 		varName;
		public string		constName;
		public int			constVal;
		public string		prefix;		// name used to prefix temp variables.

		public TouchControllerCodeTemplateGenerator cfg;

		public TouchStick 	stick;
		public TouchZone	zone;
		
		// -------------------
		const string ILLEGAL_CHARS = " \t-+/\\#$%^&()[]{}|@!?><,.";


		// ----------------
		public ControlParams(TouchControllerCodeTemplateGenerator cfg, 
			TouchStick stick, int id)
			{
			this.cfg		= cfg;
			this.stick 		= stick;
			this.zone		= null;
			this.constVal 	= id;
			this.srcName	= stick.name;
			this.varName	= "";
			this.constName	= "";
			this.prefix		= "";
		
			this.GenNames();
			}

		// ----------------
		public ControlParams(TouchControllerCodeTemplateGenerator cfg, 
			TouchZone zone, int id)
			{
			this.cfg		= cfg;
			this.zone 		= zone;
			this.stick		= null;
			this.constVal 	= id;
			this.srcName	= zone.name;
			this.varName	= "";
			this.constName	= "";
			this.prefix		= "";
	
			this.GenNames();
			}

		// --------------
		public string AddaptString(string templateString)
			{
			string s = templateString;
			
			s = this.cfg.AdaptString(s);

			//s = s.Replace("%ctrl%", 	(this.cfg.skipThisKeyword ? "" : "this.") + this.cfg.ctrlVarName);
			s = s.Replace("%name%",		((this.stick != null) ? this.stick.name : this.zone.name)); 
			s = s.Replace("%var%", 		this.varName);
			s = s.Replace("%prefix%", 	this.prefix);
			s = s.Replace("%const%", 	this.constName);
			s = s.Replace("%id%", 		this.constVal.ToString());
			s = s.Replace("%type%", 	((this.stick != null) ? "TouchStick" : "TouchZone"));

			s = s.Replace("ZONE_VAR", 	this.varName);
			s = s.Replace("STICK_VAR",	this.varName);
			s = s.Replace("Z_PREFIX_",	this.prefix);
			s = s.Replace("S_PREFIX_",	this.prefix);

			return s;
			}


		// -------------
		private void GenNames()
			{
			this.prefix 	= "";
			this.varName	= "";
			this.constName 	= "";
				
			string rawName 		= (this.stick != null) ? this.stick.name : this.zone.name;

			// Generate constant's name...

			string constLabel 	= (this.stick != null) ? "Stick" : "Zone";	
			string spaceReplacement = (this.cfg.spacesToUnderscores ? "_" : "");

			this.constName = (this.cfg.prefixMode ? 
				(constLabel + " " + rawName) : (rawName + " " + constLabel));
			
			this.constName = ReplaceIllegalChars(this.constName, ILLEGAL_CHARS, spaceReplacement);

			if (this.cfg.uppercaseMode)
				this.constName = this.constName.ToUpper();
			

			// Generate variable name...

			string varLabel 	= (this.stick != null) ? "stick" : "zone";	
			 
			this.varName = CamelCase(rawName + " " + varLabel); //varLabel + " " + rawName);


			// Generate temp var prefix...

			this.prefix = CamelCase(rawName);
			}
		


		// ------------
		public string GenConstDecl()
			{
			return this.AddaptString(this.cfg.privateMode ? 
				(this.cfg.jsMode ? JS_PRIVATE_CONST_DECL 	: CS_PRIVATE_CONST_DECL) :
				(this.cfg.jsMode ? JS_PUBLIC_CONST_DECL		: CS_PUBLIC_CONST_DECL) );
			}

		// -------------
		public string GenVarDecl()
			{
			return this.AddaptString((this.stick != null) ? 
				(this.cfg.jsMode ? JS_STICK_VAR_DECL 	: CS_STICK_VAR_DECL) :
				(this.cfg.jsMode ? JS_ZONE_VAR_DECL 	: CS_ZONE_VAR_DECL) );
			}

		// -------------
		public string GenInputCode()
			{
			if (this.stick != null)
				{
				return this.AddaptString(STICK_INPUT_CODE_HEADER + 
					(this.cfg.jsMode ? JS_STICK_CODE : CS_STICK_CODE));
				}
			else
				{
				if (!this.zone.codeUniPressed &&
				 	!this.zone.codeUniJustPressed &&
					!this.zone.codeUniJustReleased &&
					!this.zone.codeSimpleTap &&
					!this.zone.codeSingleTap &&
					!this.zone.codeDoubleTap &&
					(!this.zone.enableSecondFinger || (
						!this.zone.codeMultiPressed &&
						!this.zone.codeMultiJustPressed &&
						!this.zone.codeMultiJustReleased &&
						!this.zone.codeSimpleMultiTap &&
						!this.zone.codeMultiSingleTap &&
						!this.zone.codeMultiDoubleTap)
						))
					return "";
				

				string s = this.AddaptString(ZONE_INPUT_CODE_HEADER);
		

				// Just Pressed

				if (this.zone.enableSecondFinger)
					{
					if (this.zone.codeMultiJustPressed)
						s += this.AddaptString(this.cfg.jsMode ? 	
							JS_ZONE_MULTI_JUST_PRESSED_CODE : CS_ZONE_MULTI_JUST_PRESSED_CODE);							
					}

				if (this.zone.codeUniJustPressed)
					s += this.AddaptString(this.cfg.jsMode ? 	
						JS_ZONE_UNI_JUST_PRESSED_CODE : CS_ZONE_UNI_JUST_PRESSED_CODE);							
				

				// Multi-Pressed...
				
				if (this.zone.enableSecondFinger)
					{
					if (this.zone.codeMultiPressed)
						{
						s += this.AddaptString(this.cfg.jsMode ? 	
							JS_ZONE_MULTI_PRESSED_BEGIN_CODE : CS_ZONE_MULTI_PRESSED_BEGIN_CODE);
	
						// Multi-touch drag...

						if (this.zone.codeMultiJustDragged)
							s += this.AddaptString(this.cfg.jsMode ? 	
								JS_ZONE_JUST_MULTI_DRAGGED_CODE : CS_ZONE_JUST_MULTI_DRAGGED_CODE);

						if (this.zone.codeMultiDragged)
							s += this.AddaptString(this.cfg.jsMode ? 	
								JS_ZONE_MULTI_DRAGGED_CODE : CS_ZONE_MULTI_DRAGGED_CODE);


						// Twist...

						if (this.zone.codeJustTwisted)
							s += this.AddaptString(this.cfg.jsMode ? 	
								JS_ZONE_JUST_TWISTED_CODE : CS_ZONE_JUST_TWISTED_CODE);

						if (this.zone.codeTwisted)
							s += this.AddaptString(this.cfg.jsMode ? 	
								JS_ZONE_TWISTED_CODE : CS_ZONE_TWISTED_CODE);

						// Pinch...

						if (this.zone.codeJustPinched)
							s += this.AddaptString(this.cfg.jsMode ? 	
								JS_ZONE_JUST_PINCHED_CODE : CS_ZONE_JUST_PINCHED_CODE);

						if (this.zone.codePinched)
							s += this.AddaptString(this.cfg.jsMode ? 	
								JS_ZONE_PINCHED_CODE : CS_ZONE_PINCHED_CODE);
		
	
						// End...

						s += this.AddaptString(this.cfg.jsMode ? 	
							JS_ZONE_MULTI_PRESSED_END_CODE : CS_ZONE_MULTI_PRESSED_END_CODE);
						}							
					}
				
				// Uni-touch pressed...

				if (this.zone.codeUniPressed)
					{
					s += this.AddaptString(this.cfg.jsMode ? 	
						JS_ZONE_UNI_PRESSED_BEGIN_CODE : CS_ZONE_UNI_PRESSED_BEGIN_CODE);

					// Uni-touch drag...

					if (this.zone.codeUniJustDragged)
						s += this.AddaptString(this.cfg.jsMode ? 	
							JS_ZONE_JUST_UNI_DRAGGED_CODE : CS_ZONE_JUST_UNI_DRAGGED_CODE);

					if (this.zone.codeUniDragged)
						s += this.AddaptString(this.cfg.jsMode ? 	
							JS_ZONE_UNI_DRAGGED_CODE : CS_ZONE_UNI_DRAGGED_CODE);

					// End...

					s += this.AddaptString(this.cfg.jsMode ? 	
						JS_ZONE_UNI_PRESSED_END_CODE : CS_ZONE_UNI_PRESSED_END_CODE);
					}							




				// Multi-Touch Just Released
				
				if (this.zone.enableSecondFinger && this.zone.codeMultiJustReleased)	
					{
					s += this.AddaptString(this.cfg.jsMode ? 	
						JS_ZONE_MULTI_RELEASED_BEGIN_CODE : CS_ZONE_MULTI_RELEASED_BEGIN_CODE);
					
					// Multi-drag on release...

					if (this.zone.codeMultiReleasedDrag)
						{
						s += this.AddaptString(this.cfg.jsMode ? 	
							JS_ZONE_MULTI_RELEASED_DRAG_CODE : CS_ZONE_MULTI_RELEASED_DRAG_CODE);
						}
					
					// Twist on release...

					if (this.zone.codeReleasedTwist)
						{
						s += this.AddaptString(this.cfg.jsMode ? 	
							JS_ZONE_RELEASED_TWIST_CODE : CS_ZONE_RELEASED_TWIST_CODE);
						}
					
					// Pinch on release..

					if (this.zone.codeReleasedPinch)
						{
						s += this.AddaptString(this.cfg.jsMode ? 	
							JS_ZONE_RELEASED_PINCH_CODE : CS_ZONE_RELEASED_PINCH_CODE);
						}

					// End...

					s += this.AddaptString(this.cfg.jsMode ? 	
						JS_ZONE_MULTI_RELEASED_END_CODE : CS_ZONE_MULTI_RELEASED_END_CODE);
					}

				
				// Uni-Touch Just Released
		
				if (this.zone.codeUniJustReleased)
					{
					s += this.AddaptString(this.cfg.jsMode ? 	
						JS_ZONE_UNI_RELEASED_BEGIN_CODE : CS_ZONE_UNI_RELEASED_BEGIN_CODE);
					
					// Uni-drag on release...

					if (this.zone.codeUniReleasedDrag)
						{
						s += this.AddaptString(this.cfg.jsMode ? 	
							JS_ZONE_UNI_RELEASED_DRAG_CODE : CS_ZONE_UNI_RELEASED_DRAG_CODE);
						}
					

					// End...

					s += this.AddaptString(this.cfg.jsMode ? 	
						JS_ZONE_UNI_RELEASED_END_CODE : CS_ZONE_UNI_RELEASED_END_CODE);
					}
		


				
				/*
				// Press..

				if (this.zone.enableSecondFinger)
					{
					s += this.AddaptString(this.cfg.jsMode ? 
						JS_ZONE_MULTI_PRESS_CODE : CS_ZONE_MULTI_PRESS_CODE);					
					}
		
				s += this.AddaptString(this.cfg.jsMode ? 
					JS_ZONE_PRESS_CODE : CS_ZONE_PRESS_CODE);
				
				// Release...

				if (this.zone.enableSecondFinger)
					{
					s += this.AddaptString(this.cfg.jsMode ? 
						JS_ZONE_MULTI_RELEASE_CODE : CS_ZONE_MULTI_RELEASE_CODE);					
					}
		
				s += this.AddaptString(this.cfg.jsMode ? 
					JS_ZONE_RELEASE_CODE : CS_ZONE_RELEASE_CODE);
					*/

			
				bool prevTapSection = false;

				// Multi-touch taps...
				
				if (this.zone.enableSecondFinger)
					{
					if (this.zone.codeMultiDoubleTap)
						{
						prevTapSection = true;
						s += this.AddaptString(this.cfg.jsMode ? 
							JS_ZONE_MULTI_DOUBLE_TAP_CODE : CS_ZONE_MULTI_DOUBLE_TAP_CODE);
						}

					if (this.zone.codeSimpleMultiTap)
						{
						if (prevTapSection)	
							s += ZONE_TAP_ELSE_CODE;
						else
							prevTapSection = true;

						s += this.AddaptString(this.cfg.jsMode ? 
							JS_ZONE_SIMPLE_MULTI_TAP_CODE : CS_ZONE_SIMPLE_MULTI_TAP_CODE);
						}

					else if (this.zone.codeMultiSingleTap)
						{
						if (prevTapSection)	
							s += ZONE_TAP_ELSE_CODE;
						else
							prevTapSection = true;

						s += this.AddaptString(this.cfg.jsMode ? 
							JS_ZONE_MULTI_SINGLE_TAP_CODE : CS_ZONE_MULTI_SINGLE_TAP_CODE);
						}
					}	
	
				// Single finger taps...	

				if (this.zone.codeDoubleTap)
					{
					if (prevTapSection)	
						s += ZONE_TAP_ELSE_CODE;
					else
						prevTapSection = true;

					s += this.AddaptString(this.cfg.jsMode ? 
						JS_ZONE_DOUBLE_TAP_CODE : CS_ZONE_DOUBLE_TAP_CODE);
					}

				if (this.zone.codeSimpleTap)
					{
					if (prevTapSection)	
						s += ZONE_TAP_ELSE_CODE;
					else
						prevTapSection = true;

					s += this.AddaptString(this.cfg.jsMode ? 
						JS_ZONE_SIMPLE_TAP_CODE : CS_ZONE_SIMPLE_TAP_CODE);
					}

				else if (this.zone.codeSingleTap)
					{
					if (prevTapSection)	
						s += ZONE_TAP_ELSE_CODE;
					else
						prevTapSection = true;

					s += this.AddaptString(this.cfg.jsMode ? 
						JS_ZONE_SINGLE_TAP_CODE : CS_ZONE_SINGLE_TAP_CODE);
					}


				if (prevTapSection)
					s += this.AddaptString(ZONE_TAP_END_CODE);				

				
				return s;
				}
			}
		

		// -------------
		public string GenLayoutCode()
			{
			if (this.stick != null)
				{
				if (this.stick.codeCustomLayout)
					return this.AddaptString(this.cfg.jsMode ? 
						JS_STICK_LAYOUT_CODE : CS_STICK_LAYOUT_CODE);
				else
					return "";
				}
			else
				{
				if (this.zone.codeCustomLayout)
					return this.AddaptString(this.cfg.jsMode ? 
						JS_ZONE_LAYOUT_CODE : CS_ZONE_LAYOUT_CODE);
				else
					return "";
				}
			}

 
		// -----------
		public string GenCustomGUICode()
			{
			if (this.stick != null)
				{
				if (this.stick.codeCustomGUI)
					return this.AddaptString(this.cfg.jsMode ? 
						JS_STICK_GUI_DRAW_CODE : CS_STICK_GUI_DRAW_CODE);
				else
					return "";
				}
			else
				{
				if (this.zone.codeCustomGUI)
					return this.AddaptString(this.cfg.jsMode ? 
						JS_ZONE_GUI_DRAW_CODE : CS_ZONE_GUI_DRAW_CODE);
				else
					return "";
				}
			}

		// -------------------
		static public string CamelCase(string s)
			{
			string o = "";

			bool lastWasSpace = false;
			
			for (int i = 0; i < s.Length; ++i)
				{
				char c = s[i];
				if (ILLEGAL_CHARS.IndexOf(c) >= 0)
					lastWasSpace = true;
				else
					{
					if (o.Length == 0)
						o += c.ToString().ToLower();
					else if (lastWasSpace)
						o += c.ToString().ToUpper();
					else
						o += c.ToString().ToLower();
	
					lastWasSpace = false;
					}
				}

			return o;
			}

		// ---------------
		static public string ReplaceIllegalChars(string s, string illegalChars, string replacement)
			{	
			string o = "";

			for (int i = 0; i < s.Length; ++i)
				{
				char c = s[i];
				if (illegalChars.IndexOf(c) >= 0)
					o += replacement;
				else
					o += c;
				}

			return o;
			}
		}
	
	// -------------------
	static public string ClipStr(string s, int maxLines, int maxChars = 16383)
		{
		bool 	broken 	= false;
		string 	o 		= "";
		
		for (int i = 0; i < s.Length; ++i)
			{
			char c = s[i];
			o += c;

			if (c == '\n')
				{
				if (--maxLines <= 0)
					{
					broken = true;
					break;	
					}	
				} 

			if (--maxChars <= 0)
				{
				broken = true;
				break;		
				}
			}
		
		if (broken)
			o += "(...)";
		
		return o;
		}

	
	// -----------------------
	// Strings 
	// -----------------------

	public const string 
		SEPARATOR				= "\n\n",

		CTRL_VAR_HEADER	 		= 
			"// ----------------------\n" +
			"// Controller Variable \n" +
			"// ----------------------\n" +
			"\n",

		JS_CTRL_VAR_DECL		= "public\tvar %justctrl%\t: TouchController;\n",
		CS_CTRL_VAR_DECL		= "public\tTouchController\t%justctrl%;\n",


		CONST_HEADER	 		= 
			"// ----------------------\n" +
			"// Constants\n" +
			"// ----------------------\n" +
			"\n",

		JS_PRIVATE_CONST_DECL 	= "private static var %const%\t: int = %id%;\n",
		JS_PUBLIC_CONST_DECL 	= "public static var %const%\t: int = %id%;\n",
		CS_PRIVATE_CONST_DECL 	= "private const int %const%\t= %id%;\n",
		CS_PUBLIC_CONST_DECL 	= "public const int %const%\t= %id%;\n",
		

		START_HEADER			=
			"// ----------------------\n" +
			"// Start()\n" +
			"// ----------------------\n" +
			"\n",

		JS_START_BEGIN			= "function Start() : void\n\t{\n",
		CS_START_BEGIN			= "void Start()\n\t{\n",
		JS_START_END			= "\t}\n\n",
		CS_START_END			= "\t}\n\n",

		MANUAL_INIT_COMMENT		= "\t// Manual init...\n\n",
 
		JS_MANUAL_INIT			= MANUAL_INIT_COMMENT + "\t%ctrl%.InitController();\n",
		CS_MANUAL_INIT			= MANUAL_INIT_COMMENT + "\t%ctrl%.InitController();\n",



		UPDATE_HEADER			=
			"// ----------------------\n" +
			"// Update()\n" +
			"// ----------------------\n" +
			"\n",

		JS_UPDATE_BEGIN			= 
			"function Update() : void\n" +
			"\t{\n" +
			"\tif (%ctrl% != null)\n" +
			"\t\t{\n" +
			"",
		CS_UPDATE_BEGIN			= 
			"void Update()\n" +
			"\t{\n" +
			"\tif (%ctrl% != null)\n" +
			"\t\t{\n" +
			"",

		JS_UPDATE_END			= "\t\t}\n" + "\t}\n\n",
		CS_UPDATE_END			= "\t\t}\n" + "\t}\n\n",
		
		MANUAL_POLL_COMMENT		= "\t\t// Manual poll...\n\n",
		JS_MANUAL_POLL			= MANUAL_POLL_COMMENT + "\t\t%ctrl%.PollController();\n",
		CS_MANUAL_POLL			= MANUAL_POLL_COMMENT + "\t\t%ctrl%.PollController();\n",

		MANUAL_UPDATE_COMMENT	= "\t\t// Manual update...\n\n",
		JS_MANUAL_UPDATE		= MANUAL_UPDATE_COMMENT + "\t\t%ctrl%.UpdateController();\n",
		CS_MANUAL_UPDATE		= MANUAL_UPDATE_COMMENT + "\t\t%ctrl%.UpdateController();\n",

		
		JS_LAYOUT_CODE_BEGIN	= 
			"\t\tif (%ctrl%.LayoutChanged())\n" + 
			"\t\t\t{\t\n" + 
			"\t\t\tvar guiScreenRect\t: Rect\t= %ctrl%.GetScreenEmuRect(false);\n" + 
			"\t\t\tvar camViewportRect\t: Rect\t= %ctrl%.GetScreenEmuRect(true);\n" + 
			"\t\t\t\t\n" + 
			"\n",

		CS_LAYOUT_CODE_BEGIN	= 
			"\t\tif (%ctrl%.LayoutChanged())\n" + 
			"\t\t\t{\t\n" + 
			"\t\t\tRect guiScreenRect \t\t= %ctrl%.GetScreenEmuRect(false);\n" + 
			"\t\t\tRect camViewportRect\t= %ctrl%.GetScreenEmuRect(true);\n" + 
			"\t\t\t\t\n" + 
			"\n",

			
		JS_LAYOUT_CODE_END		=
			"\t\t\t// End of custom layout.\n" + 
			"\t\n" + 
			"\t\t\t%ctrl%.LayoutChangeHandled();\n" + 
			"\t\t\t}\n",

		CS_LAYOUT_CODE_END		=
			"\t\t\t// End of custom layout.\n" + 
			"\t\n" + 
			"\t\t\t%ctrl%.LayoutChangeHandled();\n" + 
			"\t\t\t}\n",
		

		ONGUI_HEADER			=
			"// ----------------------\n" +
			"// OnGUI()\n" +
			"// ----------------------\n" +
			"\n",


		JS_ONGUI_BEGIN			= 
			"function OnGUI() : void\n" +
			"\t{\n" +
			"\tif (%ctrl% != null)\n" +
			"\t\t{\n" +
			"",

		CS_ONGUI_BEGIN			= 
			"void OnGUI()\n" +
			"\t{\n" +
			"\tif (%ctrl% != null)\n" +
			"\t\t{\n" +
			"",

		JS_ONGUI_END			= "\t\t}\n" + "\t}\n\n",
		CS_ONGUI_END			= "\t\t}\n" + "\t}\n\n",
		
		MANUAL_GUI_COMMENT		= "\t\t// Manually draw controller GUI...\n\n",
		JS_MANUAL_DRAW_GUI		= MANUAL_GUI_COMMENT + "\t\t%ctrl%.DrawControllerGUI();\n",
		CS_MANUAL_DRAW_GUI		= MANUAL_GUI_COMMENT + "\t\t%ctrl%.DrawControllerGUI();\n",
		

		VAR_DECL_HEADER			= 
			"\t\t// ----------------------\n" +
			"\t\t// Stick and Zone Variables\n" +
			"\t\t// ----------------------\n" +
			"\n",


		VAR_DECL_INDENT			= "\t\t",

		JS_STICK_VAR_DECL 		= VAR_DECL_INDENT + "var %var%\t: %type%\t= %ctrl%.GetStick(%const%);\n",
		JS_ZONE_VAR_DECL 		= VAR_DECL_INDENT + "var %var%\t: %type%\t= %ctrl%.GetZone(%const%);\n",
		CS_STICK_VAR_DECL 		= VAR_DECL_INDENT + "%type%\t%var%\t= %ctrl%.GetStick(%const%);\n",
		CS_ZONE_VAR_DECL 		= VAR_DECL_INDENT + "%type%\t%var%\t= %ctrl%.GetZone(%const%);\n",
		
		LAYOUT_CODE_HEADER		= 
			"\t\t// ----------------------\n" +
			"\t\t// Custom Layout Code\n" +
			"\t\t// ----------------------\n" +
			"\n",
		
		LAYOUT_INDENT 			= "\t\t\t",

		JS_STICK_LAYOUT_CODE =
			"\t\t\t// -----------------\n" + 
			"\t\t\t// \'%name%\' Stick Layout\n" + 
			"\t\t\t// -----------------\n" + 
			"\t\n" + 
			"\t\t\tvar S_PREFIX_Rect\t: Rect\t= STICK_VAR.GetRect();\n" + 
			"\n" + 
			"\t\t\t// (Modify or replace automatic rect here!)\n" + 
			"\n" + 
			"\t\t\tSTICK_VAR.SetRect(S_PREFIX_Rect);\n" + 
			"\n" + 
			"\t\t\t\n",

		CS_STICK_LAYOUT_CODE =
			"\t\t\t// -----------------\n" + 
			"\t\t\t// \'%name%\' Stick Layout\n" + 
			"\t\t\t// -----------------\n" + 
			"\t\n" + 
			"\t\t\tRect S_PREFIX_Rect = STICK_VAR.GetRect();\n" + 
			"\t\t\t\n" + 
			"\t\t\t// (Modify or replace automatic rect here!)\n" + 
			"\t\t\t\n" + 
			"\t\t\tSTICK_VAR.SetRect(S_PREFIX_Rect);\n" + 
			"\n" + 
			"\t\t\t\n",


		JS_ZONE_LAYOUT_CODE =
			"\t\t\t// -----------------\n" + 
			"\t\t\t// \'%name%\' Zone Layout\n" + 
			"\t\t\t// -----------------\n" + 
			"\n" + 
			"\t\t\tvar Z_PREFIX_Rect\t: Rect\t= ZONE_VAR.GetRect();\n" + 
			"\n" + 
			"\t\t\t// (Modify or replace automatic rect here!)\n" + 
			"\n" + 
			"\t\t\tZONE_VAR.SetRect(Z_PREFIX_Rect);\n" + 
			"\n" + 
			"\n",

		CS_ZONE_LAYOUT_CODE =
			"\t\t\t// -----------------\n" + 
			"\t\t\t// \'%name%\' Zone Layout\n" + 
			"\t\t\t// -----------------\n" + 
			"\t\t\t\n" + 
			"\t\t\tRect Z_PREFIX_Rect = ZONE_VAR.GetRect();\n" + 
			"\n" + 
			"\t\t\t// (Modify or replace automatic rect here!)\n" + 
			"\n" + 
			"\t\t\tZONE_VAR.SetRect(Z_PREFIX_Rect);\n" + 
			"\n" + 
			"\n",




		INPUT_CODE_HEADER		= 
			"\t\t// ----------------------\n" +
			"\t\t// Input Handling Code\n" +
			"\t\t// ----------------------\n" +
			"\n",

		INPUT_INDENT 			= "\t\t",			

		STICK_INPUT_CODE_HEADER	= 
			INPUT_INDENT + "// ----------------\n" +
			INPUT_INDENT + "// Stick \'%name%\'...\n" +
			INPUT_INDENT + "// ----------------\n" +
			INPUT_INDENT + "\n",			

		CS_STICK_CODE			=
			INPUT_INDENT + "if (%var%.Pressed())\n" +
			INPUT_INDENT + "\t{" + "\n" + 
			INPUT_INDENT + "\tVector2\t%prefix%Vec\t\t\t\t= %var%.GetVec();" + "\n" +
			INPUT_INDENT + "\tfloat\t%prefix%Tilt\t\t\t= %var%.GetTilt();" + "\n" +
			INPUT_INDENT + "\tfloat\t%prefix%Angle\t\t\t= %var%.GetAngle();" + "\n" +
			INPUT_INDENT + "\tTouchStick.StickDir\t%prefix%Dir\t= %var%.GetDigitalDir(true);" + "\n" +
			INPUT_INDENT + "\tVector3\t%prefix%WorldVec\t\t= %var%.GetVec3d(false, 0);" + "\n" +
			INPUT_INDENT + "\t" + "\n" +
			INPUT_INDENT + "\t// Your code here." + "\n" +
			INPUT_INDENT + "\t}" + "\n" +
			"\n" +
			//"\telse" + "\n" +
			//"\t\t{" + "\n" +
			//"\t\t// " + "\n" +
			//"\t\t}" + "\n" +
			"",
		JS_STICK_CODE			=
			INPUT_INDENT + "if (%var%.Pressed())\n" +
			INPUT_INDENT + "\t{" + "\n" +
			INPUT_INDENT + "\tvar %prefix%Vec\t\t\t\t: Vector2\t= %var%.GetVec();" + "\n" +
			INPUT_INDENT + "\tvar %prefix%Tilt\t\t\t: float\t= %var%.GetTilt();" + "\n" +
			INPUT_INDENT + "\tvar %prefix%Angle\t\t\t: float\t= %var%.GetAngle();" + "\n" +
			INPUT_INDENT + "\tvar %prefix%Dir\t: TouchStick.StickDir\t= %var%.GetDigitalDir(true);" + "\n" +
			INPUT_INDENT + "\tvar %prefix%WorldVec\t\t: Vector3\t= %var%.GetVec3d(false, 0);" + "\n" +
			INPUT_INDENT + "\t" + "\n" +
			INPUT_INDENT + "\t// Your code here." + "\n" +
			INPUT_INDENT + "\t}" + "\n" +
			//"\telse" + "\n" +
			//"\t\t{" + "\n" +
			//"\t\t// " + "\n" +
			//"\t\t}" + "\n" +
			"\n" +
			"",


			

		
		ZONE_INPUT_CODE_HEADER	= 
			INPUT_INDENT + "// ----------------\n" +
			INPUT_INDENT + "// Zone \'%name%\'...\n" +
			INPUT_INDENT + "// ----------------\n" +
			INPUT_INDENT + "\n",			
			
		CS_ZONE_MULTI_JUST_PRESSED_CODE =
			"\t\tif (ZONE_VAR.JustMultiPressed())\n" + 
			"\t\t\t{\n" + 
			"\t\t\tVector2 Z_PREFIX_MultiPressStartPos\t= ZONE_VAR.GetMultiStartPos(TouchCoordSys.SCREEN_PX);\n" + 
			"\t\t\t\n" + 
			"\t\t\t}\n" + 
			"\t\t\t\n",

		CS_ZONE_UNI_JUST_PRESSED_CODE =
			"\t\tif (ZONE_VAR.JustUniPressed())\n" + 
			"\t\t\t{\n" + 
			"\t\t\tVector2 Z_PREFIX_PressStartPos\t= ZONE_VAR.GetUniStartPos(TouchCoordSys.SCREEN_PX);\n" + 
			"\n" + 
			"\t\t\t}\n" + 
			"\t\t\t\n",

		CS_ZONE_MULTI_PRESSED_BEGIN_CODE =
			"\t\t// Multi-Pressed...\n" + 
			"\t\t\n" + 
			"\t\tif (ZONE_VAR.MultiPressed())\n" + 
			"\t\t\t{\n" + 
			"\t\t\tfloat\tZ_PREFIX_MultiDur\t\t\t= ZONE_VAR.GetMultiTouchDuration();\n" + 
			"\t\t\tVector2\tZ_PREFIX_MultiPos\t\t\t= ZONE_VAR.GetMultiPos();\n" + 
			"\t\t\tVector2\tZ_PREFIX_MultiDragDelta\t\t= ZONE_VAR.GetMultiDragDelta();\n" + 
			"\t\t\tVector2\tZ_PREFIX_MultiRawDragDelta\t= ZONE_VAR.GetMultiDragDelta(true);\n" + 
			"\n" + 
			"\n",

		CS_ZONE_JUST_MULTI_DRAGGED_CODE =
			"\t\t\t// Multi-touch drag...\n" + 
			"\t\t\t\t\n" + 
			"\t\t\tif (ZONE_VAR.JustMultiDragged())\n" + 
			"\t\t\t\t{\n" + 
			"\t\t\t\t}\n" + 
			"\n",

		CS_ZONE_MULTI_DRAGGED_CODE =
			"\t\t\tif (ZONE_VAR.MultiDragged())\n" + 
			"\t\t\t\t{\n" + 
			"\t\t\t\t}\n" + 
			"\t\t\t\t\n",

		CS_ZONE_JUST_TWISTED_CODE =
			"\t\t\t// Just Twisted...\n" + 
			"\n" + 
			"\t\t\tif (ZONE_VAR.JustTwisted())\n" + 
			"\t\t\t\t{\n" + 
			"\t\t\t\t}\n" + 
			"\t\t\t\t\n",

		CS_ZONE_TWISTED_CODE =
			"\t\t\t// Twisted...\n" + 
			"\n" + 
			"\t\t\tif (ZONE_VAR.Twisted())\n" + 
			"\t\t\t\t{\n" + 
			"\t\t\t\tfloat\tZ_PREFIX_TwistDelta\t= ZONE_VAR.GetTwistDelta();\n" + 
			"\t\t\t\tfloat\tZ_PREFIX_TwistTotal\t= ZONE_VAR.GetTotalTwist();\n" + 
			" \n" + 
			"\t\t\t\t}\n" + 
			"\n",

		CS_ZONE_JUST_PINCHED_CODE =
			"\t\t\t// Just Pinched...\n" + 
			"\n" + 
			"\t\t\tif (ZONE_VAR.JustPinched())\n" + 
			"\t\t\t\t{\n" + 
			"\t\t\t\t}\n" + 
			"\n",

		CS_ZONE_PINCHED_CODE =
			"\t\t\t// Pinched...\n" + 
			"\n" + 
			"\t\t\tif (ZONE_VAR.Pinched())\n" + 
			"\t\t\t\t{\n" + 
			"\t\t\t\tfloat\tZ_PREFIX_PinchRelScale\t= ZONE_VAR.GetPinchRelativeScale();\n" + 
			"\t\t\t\tfloat\tZ_PREFIX_PinchAbsScale\t= ZONE_VAR.GetPinchScale();\n" + 
			"\t\t\t\tfloat\tZ_PREFIX_FingerDist\t\t= ZONE_VAR.GetPinchDist();\n" + 
			"\n" + 
			"\t\t\t\t}\n" + 
			"\n",

		CS_ZONE_MULTI_PRESSED_END_CODE =
			"\t\t\t}\n" + 
			"\t\t\n",

		CS_ZONE_UNI_PRESSED_BEGIN_CODE =
			"\t\t// Uni-touch pressed...\n" + 
			"\t\t\t\n" + 
			"\t\tif (ZONE_VAR.UniPressed())\n" + 
			"\t\t\t{\n" + 
			"\t\t\tfloat\tZ_PREFIX_UniDur\t\t\t\t= ZONE_VAR.GetUniTouchDuration();\n" + 
			"\t\t\tVector2\tZ_PREFIX_UniPos\t\t\t\t= ZONE_VAR.GetUniPos();\n" + 
			"\t\t\tVector2\tZ_PREFIX_UniDragDelta\t\t= ZONE_VAR.GetUniDragDelta();\n" + 
			"\t\t\tVector2\tZ_PREFIX_UniRawDragDelta\t= ZONE_VAR.GetUniDragDelta(true);\n" + 
			"\n" + 
			"\n",

		CS_ZONE_JUST_UNI_DRAGGED_CODE =
			"\t\t\t// Just Uni-touch dragged...\n" + 
			"\t\t\t\t\n" + 
			"\t\t\tif (ZONE_VAR.JustUniDragged())\n" + 
			"\t\t\t\t{\n" + 
			"\t\t\t\t}\n" + 
			"\t\t\t\t\n",

		CS_ZONE_UNI_DRAGGED_CODE =
			"\t\t\t// Uni-Touch drag...\n" + 
			"\n" + 
			"\t\t\tif (ZONE_VAR.UniDragged())\n" + 
			"\t\t\t\t{\n" + 
			"\t\t\t\t}\n",

		CS_ZONE_UNI_PRESSED_END_CODE =
			"\t\t\t}\n" + 
			"\n" + 
			"\n",

		CS_ZONE_MULTI_RELEASED_BEGIN_CODE =
			"\t\t// Multi-Touch Just Released\n" + 
			"\n" + 
			"\t\tif (ZONE_VAR.JustMultiReleased())\n" + 
			"\t\t\t{\n" + 
			"\t\t\tVector2\tZ_PREFIX_MultiRelStartPos\t= ZONE_VAR.GetReleasedMultiStartPos();\n" + 
			"\t\t\tVector2\tZ_PREFIX_MultiRelEndPos\t\t= ZONE_VAR.GetReleasedMultiEndPos();\n" + 
			"\t\t\tint \tZ_PREFIX_MultiRelStartBox\t= TouchZone.GetBoxPortion(2, 2, ZONE_VAR.GetReleasedMultiStartPos(TouchCoordSys.SCREEN_NORMALIZED)); \n" + 
			"\t\t\tint \tZ_PREFIX_MultiRelEndBox\t\t= TouchZone.GetBoxPortion(2, 2, ZONE_VAR.GetReleasedMultiEndPos(TouchCoordSys.SCREEN_NORMALIZED)); \n" + 
			"\n" + 
			"\t\t\tVector2\tZ_PREFIX_MultiRelDragVel\t= ZONE_VAR.GetReleasedMultiDragVel();\n" + 
			"\t\t\tVector2 Z_PREFIX_MultiRelDragVec\t= ZONE_VAR.GetReleasedMultiDragVec();\n" + 
			"\n",

		CS_ZONE_MULTI_RELEASED_DRAG_CODE =
			"\t\t\t// Released multi-touch was dragged...\n" + 
			"\n" + 
			"\t\t\tif (ZONE_VAR.ReleasedMultiDragged())\n" + 
			"\t\t\t\t{\n" + 
			"\t\t\t\t}\n" + 
			"\n",

		CS_ZONE_RELEASED_TWIST_CODE =
			"\t\t\t// Released multi-touch was twisted...\n" + 
			"\t\t\n" + 
			"\t\t\tif (ZONE_VAR.ReleasedTwisted())\n" + 
			"\t\t\t\t{\n" + 
			"\t\t\t\tfloat\tZ_PREFIX_RelTwistAngle\t= ZONE_VAR.GetReleasedTwistAngle();\n" + 
			"\t\t\t\tfloat\tZ_PREFIX_RelTwistVel\t\t= ZONE_VAR.GetReleasedTwistVel();\n" + 
			"\n" + 
			"\t\t\t\t}\n" + 
			"\n",

		CS_ZONE_RELEASED_PINCH_CODE =
			"\t\t\t// Released multi-touch was pinched...\n" + 
			"\n" + 
			"\t\t\tif (ZONE_VAR.ReleasedPinched())\n" + 
			"\t\t\t\t{\n" + 
			"\t\t\t\tfloat\tZ_PREFIX_RelPinchStartDist\t= ZONE_VAR.GetReleasedPinchStartDist();\n" + 
			"\t\t\t\tfloat\tZ_PREFIX_RelPinchEndDist\t\t= ZONE_VAR.GetReleasedPinchEndDist();\n" + 
			"\t\t\t\tfloat\tZ_PREFIX_RelPinchScale\t\t= ZONE_VAR.GetReleasedPinchScale();\n" + 
			"\n" + 
			"\t\t\t\t}\n" + 
			"\t\n",

		CS_ZONE_MULTI_RELEASED_END_CODE =
			"\t\t\t}\n" + 
			"\t\t\n" + 
			"\t\t\n",

		CS_ZONE_UNI_RELEASED_BEGIN_CODE =
			"\t\t// Uni-Touch Just Released\n" + 
			"\n" + 
			"\t\tif (ZONE_VAR.JustUniReleased())\n" + 
			"\t\t\t{\n" + 
			"\t\t\tVector2\tZ_PREFIX_UniRelStartPos\t= ZONE_VAR.GetReleasedUniStartPos();\n" + 
			"\t\t\tVector2\tZ_PREFIX_UniRelEndPos\t= ZONE_VAR.GetReleasedUniEndPos();\n" + 
			"\t\t\tint \tZ_PREFIX_UniRelStartBox\t= TouchZone.GetBoxPortion(2, 2, ZONE_VAR.GetReleasedUniStartPos(TouchCoordSys.SCREEN_NORMALIZED)); \n" + 
			"\t\t\tint \tZ_PREFIX_UniRelEndBox\t= TouchZone.GetBoxPortion(2, 2, ZONE_VAR.GetReleasedUniEndPos(TouchCoordSys.SCREEN_NORMALIZED)); \n" + 
			"\n" + 
			"\t\t\tVector2\tZ_PREFIX_UniRelDragVel\t= ZONE_VAR.GetReleasedUniDragVel();\n" + 
			"\t\t\tVector2 Z_PREFIX_UniRelDragVec\t= ZONE_VAR.GetReleasedUniDragVec();\n" + 
			"\t\t\t\t\t\n" + 
			"\n",

		CS_ZONE_UNI_RELEASED_DRAG_CODE =
			"\t\t\t// Released uni-touch was dragged...\n" + 
			"\n" + 
			"\t\t\tif (ZONE_VAR.ReleasedUniDragged())\n" + 
			"\t\t\t\t{\n" + 
			"\t\t\t\t}\n" + 
			"\t\t\t\t\t\n",

		CS_ZONE_UNI_RELEASED_END_CODE =
			"\t\t\t}\n" + 
			"\n" + 
			"\n",

		CS_ZONE_SIMPLE_MULTI_TAP_CODE =
			"\t\t// Simple multi-finger tap...\n" + 
			"\n" + 
			"\t\tif (ZONE_VAR.JustMultiTapped())\n" + 
			"\t\t\t{\n" + 
			"\t\t\tVector2\tZ_PREFIX_MultiTapPos\t= ZONE_VAR.GetMultiTapPos();\n" + 
			"\n" + 
			"\t\t\t}\t\n",

		CS_ZONE_MULTI_DOUBLE_TAP_CODE =
			"\t\t// Double multi-finger tap...\n" + 
			"\n" + 
			"\t\tif (ZONE_VAR.JustMultiDoubleTapped())\n" + 
			"\t\t\t{\t\t\n" + 
			"\t\t\tVector2\tZ_PREFIX_MultiDblTapPos = ZONE_VAR.GetMultiTapPos();\n" + 
			"\n" + 
			"\t\t\t}\n",

		CS_ZONE_MULTI_SINGLE_TAP_CODE =
			"\t\t// Single multi-finger tap (delayed)...\n" + 
			"\t\n" + 
			"\t\tif (ZONE_VAR.JustMultiSingleTapped())\n" + 
			"\t\t\t{\t\t\n" + 
			"\t\t\tVector2\tZ_PREFIX_MultiTapPos\t= ZONE_VAR.GetMultiTapPos();\n" + 
			"\n" + 
			"\t\t\t}\n",

		CS_ZONE_SIMPLE_TAP_CODE =
			"\t\t// Simple one-finger tap...\n" + 
			"\n" + 
			"\t\tif (ZONE_VAR.JustTapped())\n" + 
			"\t\t\t{\n" + 
			"\t\t\tVector2\tZ_PREFIX_TapPos\t= ZONE_VAR.GetTapPos();\n" + 
			"\n" + 
			"\t\t\t}\n",

		CS_ZONE_DOUBLE_TAP_CODE =
			"\t\t// Double one-finger tap...\n" + 
			"\n" + 
			"\t\tif (ZONE_VAR.JustDoubleTapped())\n" + 
			"\t\t\t{\t\t\n" + 
			"\t\t\tVector2\tZ_PREFIX_DblTapPos = ZONE_VAR.GetTapPos();\n" + 
			"\n" + 
			"\t\t\t}\n",

		CS_ZONE_SINGLE_TAP_CODE =
			"\t\t// Single one-finger tap (delayed)...\n" + 
			"\t\n" + 
			"\t\tif (ZONE_VAR.JustSingleTapped())\n" + 
			"\t\t\t{\t\t\n" + 
			"\t\t\tVector2\tZ_PREFIX_TapPos\t= ZONE_VAR.GetTapPos();\n" + 
			"\n" + 
			"\t\t\t}\n",

		



		JS_ZONE_MULTI_JUST_PRESSED_CODE =
			"\t\tif (ZONE_VAR.JustMultiPressed())\n" + 
			"\t\t\t{\n" + 
			"\t\t\tvar Z_PREFIX_MultiPressStartPos\t: Vector2\t= ZONE_VAR.GetMultiStartPos(TouchCoordSys.SCREEN_PX);\n" + 
			"\t\t\t\n" + 
			"\t\t\t}\n" + 
			"\t\t\t\n",

		JS_ZONE_UNI_JUST_PRESSED_CODE =
			"\t\tif (ZONE_VAR.JustUniPressed())\n" + 
			"\t\t\t{\n" + 
			"\t\t\tvar Z_PREFIX_PressStartPos\t: Vector2\t= ZONE_VAR.GetUniStartPos(TouchCoordSys.SCREEN_PX);\n" + 
			"\n" + 
			"\t\t\t}\n" + 
			"\t\t\t\n",

		JS_ZONE_MULTI_PRESSED_BEGIN_CODE =
			"\t\t// Multi-Pressed...\n" + 
			"\t\t\n" + 
			"\t\tif (ZONE_VAR.MultiPressed())\n" + 
			"\t\t\t{\n" + 
			"\t\t\tvar Z_PREFIX_MultiDur\t\t\t: float\t\t= ZONE_VAR.GetMultiTouchDuration();\n" + 
			"\t\t\tvar Z_PREFIX_MultiPos\t\t\t: Vector2\t= ZONE_VAR.GetMultiPos();\n" + 
			"\t\t\tvar Z_PREFIX_MultiDragDelta\t\t: Vector2\t= ZONE_VAR.GetMultiDragDelta();\n" + 
			"\t\t\tvar Z_PREFIX_MultiRawDragDelta\t: Vector2\t= ZONE_VAR.GetMultiDragDelta(true);\n" + 
			"\n" + 
			"\n",

		JS_ZONE_JUST_MULTI_DRAGGED_CODE =
			"\t\t\t// Just Multi-touch dragged...\n" + 
			"\t\t\t\t\n" + 
			"\t\t\tif (ZONE_VAR.JustMultiDragged())\n" + 
			"\t\t\t\t{\n" + 
			"\t\t\t\t}\n" + 
			"\n",

		JS_ZONE_MULTI_DRAGGED_CODE =
			"\t\t\t// Multi-touch drag...\n" + 
			"\n" + 
			"\t\t\tif (ZONE_VAR.MultiDragged())\n" + 
			"\t\t\t\t{\n" + 
			"\t\t\t\t}\n" + 
			"\t\t\t\t\n",

		JS_ZONE_JUST_TWISTED_CODE =
			"\t\t\t// Just Twisted...\n" + 
			"\n" + 
			"\t\t\tif (ZONE_VAR.JustTwisted())\n" + 
			"\t\t\t\t{\n" + 
			"\t\t\t\t}\n" + 
			"\t\t\t\t\n",

		JS_ZONE_TWISTED_CODE =
			"\t\t\t// Twisted...\n" + 
			"\n" + 
			"\t\t\tif (ZONE_VAR.Twisted())\n" + 
			"\t\t\t\t{\n" + 
			"\t\t\t\tvar Z_PREFIX_TwistDelta\t: float\t= ZONE_VAR.GetTwistDelta();\n" + 
			"\t\t\t\tvar Z_PREFIX_TwistTotal\t: float\t= ZONE_VAR.GetTotalTwist();\n" + 
			" \n" + 
			"\t\t\t\t}\n" + 
			"\n",

		JS_ZONE_JUST_PINCHED_CODE =
			"\t\t\t// Just Pinched...\n" + 
			"\n" + 
			"\t\t\tif (ZONE_VAR.JustPinched())\n" + 
			"\t\t\t\t{\n" + 
			"\t\t\t\t}\n" + 
			"\n",

		JS_ZONE_PINCHED_CODE =
			"\t\t\t// Pinched...\n" + 
			"\n" + 
			"\t\t\tif (ZONE_VAR.Pinched())\n" + 
			"\t\t\t\t{\n" + 
			"\t\t\t\tvar Z_PREFIX_PinchRelScale\t: float\t= ZONE_VAR.GetPinchRelativeScale();\n" + 
			"\t\t\t\tvar Z_PREFIX_PinchAbsScale\t: float\t= ZONE_VAR.GetPinchScale();\n" + 
			"\t\t\t\tvar Z_PREFIX_FingerDist\t\t: float = ZONE_VAR.GetPinchDist();\n" + 
			"\n" + 
			"\t\t\t\t}\n" + 
			"\n",

		JS_ZONE_MULTI_PRESSED_END_CODE =
			"\t\t\t}\n" + 
			"\n" + 
			"\t\t\n",

		JS_ZONE_UNI_PRESSED_BEGIN_CODE =
			"\t\t// Uni-touch pressed...\n" + 
			"\t\t\t\n" + 
			"\t\tif (ZONE_VAR.UniPressed())\n" + 
			"\t\t\t{\n" + 
			"\t\t\tvar Z_PREFIX_UniDur\t\t\t: float\t\t= ZONE_VAR.GetUniTouchDuration();\n" + 
			"\t\t\tvar Z_PREFIX_UniPos\t\t\t: Vector2\t= ZONE_VAR.GetUniPos();\n" + 
			"\t\t\tvar Z_PREFIX_UniDragDelta\t: Vector2\t= ZONE_VAR.GetUniDragDelta();\n" + 
			"\t\t\tvar Z_PREFIX_UniRawDragDelta\t: Vector2\t= ZONE_VAR.GetUniDragDelta(true);\n" + 
			"\n" + 
			"\n",

		JS_ZONE_JUST_UNI_DRAGGED_CODE =
			"\t\t\t// Just Uni-touch dragged...\n" + 
			"\t\t\t\t\n" + 
			"\t\t\tif (ZONE_VAR.JustUniDragged())\n" + 
			"\t\t\t\t{\n" + 
			"\t\t\t\t}\n" + 
			"\t\t\t\t\n",

		JS_ZONE_UNI_DRAGGED_CODE =
			"\t\t\t// Uni-Touch drag...\n" + 
			"\n" + 
			"\t\t\tif (ZONE_VAR.UniDragged())\n" + 
			"\t\t\t\t{\n" + 
			"\t\t\t\t}\n",

		JS_ZONE_UNI_PRESSED_END_CODE =
			"\t\t\t}\t\t\t\t\t\t\t\n" + 
			"\n" + 
			"\n",

		JS_ZONE_MULTI_RELEASED_BEGIN_CODE =
			"\t\t// Multi-Touch Just Released\n" + 
			"\n" + 
			"\t\tif (ZONE_VAR.JustMultiReleased())\n" + 
			"\t\t\t{\n" + 
			"\t\t\tvar Z_PREFIX_MultiRelStartPos\t: Vector2\t= ZONE_VAR.GetReleasedMultiStartPos();\n" + 
			"\t\t\tvar Z_PREFIX_MultiRelEndPos\t\t: Vector2\t= ZONE_VAR.GetReleasedMultiEndPos();\n" + 
			"\t\t\tvar Z_PREFIX_MultiRelStartBox\t: int\t\t= TouchZone.GetBoxPortion(2, 2, ZONE_VAR.GetReleasedMultiStartPos(TouchCoordSys.SCREEN_NORMALIZED)); \n" + 
			"\t\t\tvar Z_PREFIX_MultiRelEndBox\t\t: int\t\t= TouchZone.GetBoxPortion(2, 2, ZONE_VAR.GetReleasedMultiEndPos(TouchCoordSys.SCREEN_NORMALIZED)); \n" + 
			"\n" + 
			"\t\t\tvar Z_PREFIX_MultiRelDragVel\t: Vector2\t= ZONE_VAR.GetReleasedMultiDragVel();\n" + 
			"\t\t\tvar Z_PREFIX_MultiRelDragVec\t: Vector2\t= ZONE_VAR.GetReleasedMultiDragVec();\n" + 
			"\n" + 
			"\n",

		JS_ZONE_MULTI_RELEASED_DRAG_CODE =
			"\t\t\t// Released multi-touch was dragged...\n" + 
			"\n" + 
			"\t\t\tif (ZONE_VAR.ReleasedMultiDragged())\n" + 
			"\t\t\t\t{\n" + 
			"\t\t\t\t}\n" + 
			"\n",

		JS_ZONE_RELEASED_TWIST_CODE =
			"\t\t\t// Released multi-touch was twisted...\n" + 
			"\t\t\n" + 
			"\t\t\tif (ZONE_VAR.ReleasedTwisted())\n" + 
			"\t\t\t\t{\n" + 
			"\t\t\t\tvar Z_PREFIX_RelTwistAngle\t: float\t= ZONE_VAR.GetReleasedTwistAngle();\n" + 
			"\t\t\t\tvar Z_PREFIX_RelTwistVel\t: float\t= ZONE_VAR.GetReleasedTwistVel();\n" + 
			"\n" + 
			"\t\t\t\t}\n" + 
			"\n",

		JS_ZONE_RELEASED_PINCH_CODE =
			"\t\t\t// Released multi-touch was pinched...\n" + 
			"\n" + 
			"\t\t\tif (ZONE_VAR.ReleasedPinched())\n" + 
			"\t\t\t\t{\n" + 
			"\t\t\t\tvar\tZ_PREFIX_RelPinchStartDist\t: float\t= ZONE_VAR.GetReleasedPinchStartDist();\n" + 
			"\t\t\t\tvar Z_PREFIX_RelPinchEndDist\t: float\t= ZONE_VAR.GetReleasedPinchEndDist();\n" + 
			"\t\t\t\tvar Z_PREFIX_RelPinchScale\t\t: float\t= ZONE_VAR.GetReleasedPinchScale();\n" + 
			"\n" + 
			"\t\t\t\t}\n" + 
			"\t\n",

		JS_ZONE_MULTI_RELEASED_END_CODE =
			"\t\t\t}\n" + 
			"\t\t\n" + 
			"\t\t\n",

		JS_ZONE_UNI_RELEASED_BEGIN_CODE =
			"\t\t// Uni-Touch Just Released\n" + 
			"\n" + 
			"\t\tif (ZONE_VAR.JustUniReleased())\n" + 
			"\t\t\t{\n" + 
			"\t\t\tvar Z_PREFIX_UniRelStartPos\t: Vector2\t= ZONE_VAR.GetReleasedUniStartPos();\n" + 
			"\t\t\tvar Z_PREFIX_UniRelEndPos\t: Vector2\t= ZONE_VAR.GetReleasedUniEndPos();\n" + 
			"\t\t\tvar Z_PREFIX_UniRelStartBox\t: int\t\t= TouchZone.GetBoxPortion(2, 2, ZONE_VAR.GetReleasedUniStartPos(TouchCoordSys.SCREEN_NORMALIZED)); \n" + 
			"\t\t\tvar Z_PREFIX_UniRelEndBox\t: int\t\t= TouchZone.GetBoxPortion(2, 2, ZONE_VAR.GetReleasedUniEndPos(TouchCoordSys.SCREEN_NORMALIZED)); \n" + 
			"\n" + 
			"\t\t\tvar Z_PREFIX_UniRelDragVel\t: Vector2\t= ZONE_VAR.GetReleasedUniDragVel();\n" + 
			"\t\t\tvar Z_PREFIX_UniRelDragVec\t: Vector2\t= ZONE_VAR.GetReleasedUniDragVec();\n" + 
			"\t\t\t\t\t\n" + 
			"\n",

		JS_ZONE_UNI_RELEASED_DRAG_CODE =
			"\t\t\t// Released uni-touch was dragged...\n" + 
			"\n" + 
			"\t\t\tif (ZONE_VAR.ReleasedUniDragged())\n" + 
			"\t\t\t\t{\n" + 
			"\t\t\t\t}\n",

		JS_ZONE_UNI_RELEASED_END_CODE =
			"\t\t\t}\n" + 
			"\n" + 
			"\n",

		ZONE_TAP_ELSE_CODE = 
			"\t\telse \n" + 
			"\n",
			
		ZONE_TAP_END_CODE = 
			"\n" + 
			"\n" + 
			"\n",


		JS_ZONE_SIMPLE_MULTI_TAP_CODE =
			"\t\t// Simple multi-finger tap...\n" + 
			"\n" + 
			"\t\tif (ZONE_VAR.JustMultiTapped())\n" + 
			"\t\t\t{\n" + 
			"\t\t\tvar Z_PREFIX_MultiSimpleTapPos\t: Vector2\t= ZONE_VAR.GetMultiTapPos();\n" + 
			"\n" + 
			"\t\t\t}\t\n",

		JS_ZONE_MULTI_DOUBLE_TAP_CODE =
			"\t\t// Double multi-finger tap...\n" + 
			"\n" + 
			"\t\tif (ZONE_VAR.JustMultiDoubleTapped())\n" + 
			"\t\t\t{\t\t\n" + 
			"\t\t\tvar Z_PREFIX_MultiDblTapPos\t: Vector2\t= ZONE_VAR.GetMultiTapPos();\n" + 
			"\n" + 
			"\t\t\t}\n",

		JS_ZONE_MULTI_SINGLE_TAP_CODE =
			"\t\t// Single multi-finger tap (delayed)...\n" + 
			"\t\n" + 
			"\t\tif (ZONE_VAR.JustMultiSingleTapped())\n" + 
			"\t\t\t{\t\t\n" + 
			"\t\t\tvar Z_PREFIX_MultiTapPos\t: Vector2\t= ZONE_VAR.GetMultiTapPos();\n" + 
			"\n" + 
			"\t\t\t}\n",

		JS_ZONE_SIMPLE_TAP_CODE =
			"\t\t// Simple one-finger tap...\n" + 
			"\n" + 
			"\t\tif (ZONE_VAR.JustTapped())\n" + 
			"\t\t\t{\n" + 
			"\t\t\tvar Z_PREFIX_SimpleTapPos\t: Vector2\t= ZONE_VAR.GetTapPos();\n" + 
			"\n" + 
			"\t\t\t}\n",

		JS_ZONE_DOUBLE_TAP_CODE =
			"\t\t// Double one-finger tap...\n" + 
			"\n" + 
			"\t\tif (ZONE_VAR.JustDoubleTapped())\n" + 
			"\t\t\t{\t\t\n" + 
			"\t\t\tvar Z_PREFIX_DblTapPos\t: Vector2\t= ZONE_VAR.GetTapPos();\n" + 
			"\n" + 
			"\t\t\t}\n",

		JS_ZONE_SINGLE_TAP_CODE =
			"\t\t// Single one-finger tap (delayed)...\n" + 
			"\t\n" + 
			"\t\tif (ZONE_VAR.JustSingleTapped())\n" + 
			"\t\t\t{\t\t\n" + 
			"\t\t\tvar Z_PREFIX_TapPos\t: Vector2\t= ZONE_VAR.GetTapPos();\n" + 
			"\n" + 
			"\t\t\t}\n",



		CUSTOM_GUI_CODE_HEADER		= 
			"\t// ----------------------\n" +
			"\t// Custom GUI Code\n" +
			"\t// ----------------------\n" +
			"\n",

			
		GUI_INDENT		= "\t\t",

		JS_STICK_GUI_DRAW_CODE = 
			GUI_INDENT + "// Draw custom GUI for \'%name%\' Stick" + "\n" +
			GUI_INDENT + "\n" +
			GUI_INDENT + "if (!%var%.DefaultGUIEnabled())" + "\n" +
			GUI_INDENT + "\t{" + "\n" +
			GUI_INDENT + "\tvar %prefix%BaseRect\t: Rect\t\t= %var%.GetBaseDisplayRect(true);" + "\n" +
			GUI_INDENT + "\tvar %prefix%HatRect\t: Rect\t\t= %var%.GetHatDisplayRect(true);" + "\n" +
			GUI_INDENT + "\tvar %prefix%BaseColor\t: Color\t\t= %var%.GetBaseColor();" + "\n" +
			GUI_INDENT + "\tvar %prefix%HatColor\t: Color\t\t= %var%.GetHatColor();" + "\n" +
			GUI_INDENT + "\tvar %prefix%GUIDepth\t: int\t\t= %var%.GetGUIDepth();" + "\n" +
			GUI_INDENT + "\tvar %prefix%BaseTex\t: Texture2D\t= %var%.GetBaseDisplayTex();" + "\n" +
			GUI_INDENT + "\tvar %prefix%HatTex\t: Texture2D\t= %var%.GetHatDisplayTex();" + "\n" +
			GUI_INDENT + "\tvar %prefix%Pressed\t: boolean\t= %var%.Pressed();" + "\n" +
			GUI_INDENT + "\tvar %prefix%Enabled\t: boolean\t= %var%.Enabled();" + "\n" +
			GUI_INDENT + "\n" +
			GUI_INDENT + "\t// (Your stick drawing code here.)" + "\n" +
			GUI_INDENT + "\t}" +
			GUI_INDENT + "\n" +
			"",

		CS_STICK_GUI_DRAW_CODE =
			GUI_INDENT + "// Draw custom GUI for \'%name%\' Stick" + "\n" +
			GUI_INDENT + "\n" +
			GUI_INDENT + "if (!%var%.DefaultGUIEnabled())" + "\n" +
			GUI_INDENT + "\t{" + "\n" +
			GUI_INDENT + "\tRect\t\t%prefix%BaseRect\t= %var%.GetBaseDisplayRect(true);" + "\n" +
			GUI_INDENT + "\tRect\t\t%prefix%HatRect\t= %var%.GetHatDisplayRect(true);" + "\n" +
			GUI_INDENT + "\tColor\t\t%prefix%BaseColor\t= %var%.GetBaseColor();" + "\n" +
			GUI_INDENT + "\tColor\t\t%prefix%HatColor\t= %var%.GetHatColor();" + "\n" +
			GUI_INDENT + "\tint\t\t\t%prefix%GUIDepth\t= %var%.GetGUIDepth();" + "\n" +
			GUI_INDENT + "\tTexture2D\t%prefix%BaseTex\t= %var%.GetBaseDisplayTex();" + "\n" +
			GUI_INDENT + "\tTexture2D\t%prefix%HatTex\t= %var%.GetHatDisplayTex();" + "\n" +
			GUI_INDENT + "\tbool\t\t%prefix%Pressed\t= %var%.Pressed();" + "\n" +
			GUI_INDENT + "\tbool\t\t%prefix%Enabled\t= %var%.Enabled();" + "\n" +
			GUI_INDENT + "\t" + "\n" +
			GUI_INDENT + "\t// (Your stick drawing code here.)" + "\n" +
			GUI_INDENT + "\t}" + "\n" +
			GUI_INDENT + "\n" +
			"",

		JS_ZONE_GUI_DRAW_CODE =
			GUI_INDENT + "// Draw custom GUI for \'%name%\' Zone" + "\n" +
			GUI_INDENT + "\n" +
			GUI_INDENT + "if (!%var%.DefaultGUIEnabled())" + "\n" +
			GUI_INDENT + "\t{" + "\n" +
			GUI_INDENT + "\tvar %prefix%BaseRect\t: Rect\t\t= %var%.GetDisplayRect(true);" + "\n" +
			GUI_INDENT + "\tvar %prefix%BaseColor\t: Color\t\t= %var%.GetColor();" + "\n" +
			GUI_INDENT + "\tvar %prefix%GUIDepth\t: int\t\t= %var%.GetGUIDepth();" + "\n" +
			GUI_INDENT + "\tvar %prefix%BaseTex\t: Texture2D\t= %var%.GetDisplayTex();" + "\n" +
			GUI_INDENT + "\tvar %prefix%Pressed\t: boolean\t= %var%.Pressed();" + "\n" +
			GUI_INDENT + "\tvar %prefix%Enabled\t: boolean\t= %var%.Enabled();" + "\n" +
			GUI_INDENT + "\t" + "\n" +
			GUI_INDENT + "\t// (Your zone drawing code here.)" + "\n" +
			GUI_INDENT + "\t}" + "\n" +
			GUI_INDENT + "\n" +
			"",

		CS_ZONE_GUI_DRAW_CODE =
			GUI_INDENT + "// Draw custom GUI for \'%name%\' Zone" + "\n" +
			GUI_INDENT + "\n" +
			GUI_INDENT + "if (!%var%.DefaultGUIEnabled())" + "\n" +
			GUI_INDENT + "\t{" + "\n" +
			GUI_INDENT + "\tRect\t\t%prefix%Rect\t\t= %var%.GetDisplayRect(true);" + "\n" +
			GUI_INDENT + "\tColor\t\t%prefix%Color\t\t= %var%.GetColor();" + "\n" +
			GUI_INDENT + "\tint\t\t\t%prefix%GUIDepth\t= %var%.GetGUIDepth();" + "\n" +
			GUI_INDENT + "\tTexture2D\t%prefix%Tex\t\t= %var%.GetDisplayTex();" + "\n" +
			GUI_INDENT + "\tbool\t\t%prefix%Pressed\t= %var%.UniPressed();" + "\n" +
			GUI_INDENT + "\tbool\t\t%prefix%Enabled\t= %var%.Enabled();" + "\n" +
			GUI_INDENT + "\t" + "\n" +
			GUI_INDENT + "\t// (Your zone drawing code here.)" + "\n" +
			GUI_INDENT + "\t}" + "\n" +
			GUI_INDENT + "\n" +
			"",
			

		BLANK_STR = "";
	
	}



----------------------------
CONTROL FREAK 
Version: 1.1.0-r8
(c) Dan's Game Tools 2013-2015

Facebook:
	http://facebook.com/DansGameTools

Asset Store Page :
	http://u3d.as/5GT

Online Documentation (ver. 1.1.0) :
	https://googledrive.com/host/0B34CAJx43C8CSDNnOV9Jd3RReTQ/index.html
	
Playmaker Actions:
	https://drive.google.com/file/d/0B34CAJx43C8CbkVzcXY4U0t3aEk/view?usp=sharing
	
----------------------------
Thank you for purchasing our product! 
Please, remember to back up your project before upgrading!



-----------------------
Getting Started
-----------------------

Please refer to the documentation for step-by-step guides and API reference.


-----------------------
Change Log
-----------------------

Version 1.1.0-r8
	Added a package with Playmaker Actions. 
	Updated demos to Unity 5 API.
	Added a fix for devices that don't return real Screen.dpi.  
	Fixed WP8 warnings in the Editor.
	Fixed Code Generator's Custom Layout Section template.
	Set "Dynamic Region Priority" of all controller presets' sticks to 5.	
	Moved Readme.txt to Plugins/Control-Freak/.


Version 1.1.0-r7

    Fixed show-stopping Windows Phone 8 bug! 
    Added DPI approximation code for WP8. 


Version 1.1.0-r6

    Added simple bullet sctipts (FPP Demo).
    Added option to prevent the map to be moved off the screen (Map Demo).
    Eliminated compiler warnings when building for mobile platforms.


Version 1.1.0-r5

    Added workaround for buggy Input.multiTouchEnabled on iOS.


Version 1.1.0-r4

    Added Official Unity Action Game Kit (UAGK) Preset.
    Added options to block movement on stick's axes.
    Various bug fixes.


Version 1.1.0

    Unity 3.5.7 compatibility!
    Source Code Generator.
    Controller Presets.
    CFInput, a replacement class for Unity's Input.
    Added overloaded functions to replace functions with default parameters and make things easier for JS programmers.
    Ability to preview all control states without pressing Start. (Display Settings)
    New functions: TouchZone.GetBoxPortion(), TouchZone.JustMidFramePressed(), etc.
    Added "Smooth Return" option to TouchStick. (Sticks)
    New "Manual GUI" option in "Automatic Mode". (General Settings)
    Ability to create an empty Controller Game Object from Main Menu (GameObject -> Create Other -> Control Freak Controller)
    Improved documentation with grouped methods.
    Minor Editor improvements and bug-fixes.


Version 1.0.2-r2

    First public release.


---------------------
Note for users upgrading from ver. 1.0.2
---------------------
GUI Images have been converted to PNGs for faster import and their folder (Control-Freak-Sample-GUI-Images/) is now located inside Plugins/ folder. 
Unfortunately, old PSD files will remain after update and it may be hard to distinguish the old ones from the new ones when picking texture from 'Select Texture' dialog, since Unity strips file extensions from asset names. 
If you choose to delete the old folder (Assets/Control-Freak-Sample-GUI-Images/) be prepared to assign all your controller textures by yourself. 
Sorry for the inconvenience.


--------------------------
Package Contents
--------------------------

* Essential Files 
	- ./Plugins/Control-Freak/ (Plugin Code)
	- ./Plugins/Editor/Control-Freak-Editor/ (Custom Inspector)

* Sample Textures (Recommended import)
	- ./Plugins/Control-Freak-Sample-GUI-Images/ (Sample GUI and debug textures) 

* Presets 
	- ./Plugins/Control-Freak-Presets/	 	(Controller Presets)

* Demos (Optional Import)
	- ./Control-Freak-Demos/Demos-CS/	(C# demos)
	- ./Control-Freak-Demos/Demos-JS/	(Javascript demos)
	- ./Control-Freak-Demos/Shared-Demo-Resources/	(models, textures, etc.)
	- ./Control-Freak-Demos/WebDemo-Scenes/		(WebDemo end screen, can be excluded from import).


 
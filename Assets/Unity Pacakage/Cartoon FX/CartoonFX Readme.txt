Cartoon FX Pack, version 2.51
2013/04/23
© 2013, Jean Moreno
=============================


PREFABS
-------
Particle Systems prefabs are located in "CFX_Prefabs" folder.
Particle Systems optimized for Mobile are located in "CFX Prefabs (Mobile)" folder.
They should work out of the box for most needs. If you need looping effect, check the according "Looping" checkbox for each Particle System.
All Assets have a CFX_ (Desktop) or CFXM_ (Mobile) prefix so that you don't mix them with your own Assets.


MOBILE OPTIMIZED PREFABS
------------------------
As of v1.3, Prefabs have all been converted to optimized versions for mobile devices.
Changes are:
- Added a particle additive shader that uses only the alpha channel to save up on texture memory usage
- Monochrome textures' format has been set to Alpha8 to get a much smaller memory size while retaining the same quality
- Other textures' formats have been set to PVRTC compression
- Textures have all been resized to small resolution sizes through Unity; you can however scale them up if you need better quality
- Particle Systems have been changed accordingly to retain color/transparency and overall quality compared to their desktop counterparts
- Swapped alpha blended shaders to additive ones when possible (alpha blending should be avoided as much as possible on mobile)

It is recommended that you use your own object recycling system instead of the included Auto-Destruct scripts for better optimization on mobile devices (Instantiating objects can be expensive performance-wise on mobile).


CARTOON FX EASY EDITOR
----------------------
New in v1.4! You can find the "Cartoon FX Easy Editor" in the menu:
GameObject -> CartoonFX Easy Editor
It allows you to easily change one or several Particle Systems properties:
"Scale Size" to change the size of your Particle Systems (changing speed, velocity, gravity, etc. values to get an accurate scaled up version of the system; also, if the ParticleSystem uses a Mesh as Shape, it will automatically create a new scaled Mesh).
It will also scale lights' intensity accordingly if any are found.
Tip: If you don't want to scale a particular module, disable it before scaling the system and re-enable it afterwards!
"Set Duration" to change the duration of your Particle Systems in percentage according to the base effect duration (i.e. if you want longer or quicker effects). 100% = normal duration.
"Loop/Unloop Effect" to set Looping on/off for all the selected Particle Systems at once.
"Set Start Color(s)" to change the start color(s) of your Particle Systems.
"Tint Colors" to change the hue only of the colors of your Particle Systems (including gradients).

The "Copy Modules" section allows you to copy all values/curves/gradients/etc. from one or several Shuriken modules to one or several other Particle Systems.
Just select which modules you want to copy, choose the source Particle System to copy values from, select the GameObjects you want to change, and click on "Copy properties to selected GameObject(s)".

"Include Children" works for both Properties and Copy Modules sections!


CARTOON FX SPAWN SYSTEM
-----------------------
CFX_SpawnSystem allows you to easily preload your effects at the beginning of a Scene and get them later, avoiding the need to call Instantiate. It is highly recommended for mobile platforms!
Create an empty GameObject and drag the script on it. You can then add GameObjects to it with its custom interface.
To get an object in your code, use CFX_SpawnSystem.GetNextObject(object), where 'object' is the original reference to the GameObject (same as if you used Instantiate).
Use the CFX_SpawnSystem.AllObjectsLoaded boolean value to check when objects have finished loading.


TROUBLESHOOTING
---------------
* Almost all prefabs have auto-destruction scripts for the Demo scene; remove them if you do not want your particle system to destroy itself upon completion.
* If you have problems with z-sorting (transparent objects appearing in front of other when their position is actually behind), try changing the values in the Particle System -> Renderer -> Sorting Fudge; as long as the relative order is respected between the different particle systems of a same prefab, it should work ok.
* CFX_ElectricMesh is meant to be edited with whatever Mesh you want; Replace it in the Particle System Inspector -> Shape -> Mesh.
* Sometimes when instantiating a Particle System, it would start to emit before being translated, thus creating particles in between its original and desired positions. Drag and drop the CFX_ShurikenThreadFix script on the parent object to fix this problem.

PLEASE LEAVE A REVIEW OR RATE THE PACKAGE IF YOU FIND IT USEFUL!
Enjoy! :)


CONTACT
-------
Questions, suggestions, help needed?
Contact me at:

jean.moreno.public+unity@gmail.com

I'd be happy to see any effects used in your project, so feel free to drop me a line about that! :)

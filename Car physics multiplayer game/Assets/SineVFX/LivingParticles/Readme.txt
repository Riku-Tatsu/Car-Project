Living Particles
Version 1.0 (27.11.2017)


IMPORTANT NOTES:

- Turn on "HDR" on your Camera, Shaders requires it
- This VFX Asset looks much better in "Linear Rendering", but there is also optimized Prefabs for "Gamma Rendering" Mode
- Image Effects are necessary in order to make a great looking game, as well as our asset. Be sure you using "Tone Mapping" and "Bloom"
- We also recommend using Deferred Rendering for better performance


HOW TO USE:

First of all, check for Demo Scene in Scenes folder. Also, there is a Prefabs folder with complete effects.
Just Drag and Drop prefabs from "Prefabs" folder into your scene, then assign your affector GameObject in "Living Particle Controller" component.
We made all Shaders very tweakable, so you can create your own unique effects.

ARRAY SHADERS:

If you planning to use more then one object to affect your particles, you need to use array shader variants. You can create an array variant from a regular prefab very easily.
Simply change the script from "LivingParticleController" to "LivingParticleArrayController", then assign all of your objects to it. And finally, change the
shader of a material to the one that has an "Array" in its name.


SHADERS CONTROL:

Final Color - Emission color of affected particles
Final Color 2 - Emission color of unaffected particles
Final Power - Final brightness of the image, you need to lower this value if you using "Gamma Rendering" Mode
Final Mask Multiply - Multiply result mask by this value

Ramp Enabled - Use ramp gradient to colorize particles
Ramp - Gradient texture, located in "VFXTextures" folder

Distance - Affected particles distance
Distance Power - Multiply distance mask by this value
Offset Power - Offsetting particle towards Affector

Camera Fade - Fade particles when near Camera
Clode Fade - Fade particles when near Affector
Vertex Distortion - Distort vertices of particle mesh, suitable for paper or leafs effects

Affector Count - If you use more than one Affector, make sure that this value is the same as in the "Living Particle Array Controller" component
Noise - Values for creating a noise mask, used in floor particles




Support email "sinevfx@gmail.com"
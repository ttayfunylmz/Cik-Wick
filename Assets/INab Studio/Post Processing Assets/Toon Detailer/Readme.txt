Support: Discord (https://discord.gg/K88zmyuZFD) 
Online Documentation: https://inabstudios.gitbook.io/toon-detailer/

### IMPORTING

After downloading the asset, import the appropriate `.unitypackage` file based on your Unity version and SRP (either `Built-in.unitypackage`, `URP 6.unitypackage`, or `URP 2022.unitypackage`).
In the built-in, make sure to open the demo scene after importing needed files.

### HOW TO USE

### Demo Scenes

Demo scenes can be found: \Common (URP or Built-In)\Demo Scenes

You can find camera with properly setup Post Process Detailer for Built-in piepeline in Scene Demo.
For URP you can see how the effect works in the Demo Scene, after you setup the effect in URP settings.

#### BUILT-IN
1. Navigate to your main camera object in your scene.
2. Add the `PostProcessDetailer.cs` script to your camera.
3. Use the 'Update' button to see the effect in the editor scene.

#### URP
1. Enable the Depth Texture and Opaque Texture options in your URP settings.
2. Open your current Universal Render Pipeline asset.
3. Add the "Toon Detailer Feature".
4. If you encounter issues during setup, refer to the provided example URP settings asset. Adjust your current URP settings via the Graphics section in your project settings.

### ISSUES
If you encounter any difficulties using, implementing, or understanding the asset, please feel free to contact me.

## Masking

### Built-in (uses stencil buffer)

#### Stencil Use 
- Equal: The effect will only be rendered where the stencil value is 1.
- NotEqual: The effect will be rendered everywhere except where the stencil value is 1.

#### Stencil Mask Layer
Choose the layer for your mesh renderers that will act as a stencil mask. The `PostProcessDetailer.cs` script will automatically render all objects within this layer.

#### Update Button
Use this button to immediately see changes after modifying the stencil settings or adding mesh renderers to the Stencil Mask Layer.

### URP 2021 (uses stencil buffer)

#### Stencil Use
- Equal: The effect will be rendered only where the stencil value is 1.
- NotEqual: The effect will be rendered everywhere except where the stencil value is 1.

#### Render Objects Feature
Utilize this URP feature to update the stencil values.

#### Layer Selection
Choose the layer for your mesh renderers that will serve as a stencil mask.

#### Stencil Override
Enable this option.

#### Event
Set the event to "After Rendering Opaques".

#### Stencil Settings
- Value: Set to 1
- Compare Function: Set to "Always"
- Pass: Set to "Replace"
- Other States: Set to "Keep"

### URP 2022+ (uses custom mask)

#### Custom Masking
A custom mask render texture can be used.

#### Mask Use
- Equal: The effect will only be rendered where the mask is active.
- NotEqual: The effect will be rendered everywhere except where the mask is active.

#### Mask Layer
Specify which layer should act as a mask. All renderers within that layer will be drawn to the custom mask texture.
After changing the mask layer, you'll need to click the 'Play' button to make the custom mask work in the scene editor.

#### Note
The Detailer.shader was inspired by MalyaWla (Pavel Pustovalov) idea and Blender's viewport cavity effect.

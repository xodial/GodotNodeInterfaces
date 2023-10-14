namespace Chickensoft.GodotNodeInterfaces;

using Godot;
using System;


/// <summary>
/// <para>Light3D is the <i>abstract</i> base class for light nodes. As it can't be instantiated, it shouldn't be used directly. Other types of light nodes inherit from it. Light3D contains the common variables and parameters used for lighting.</para>
/// </summary>
public interface ILight3D : IVisualInstance3D {
    /// <summary>
    /// <para>Returns the <see cref="Color" /> of an idealized blackbody at the given <see cref="Light3D.LightTemperature" />. This value is calculated internally based on the <see cref="Light3D.LightTemperature" />. This <see cref="Color" /> is multiplied by <see cref="Light3D.LightColor" /> before being sent to the <see cref="RenderingServer" />.</para>
    /// </summary>
    Color GetCorrelatedColor();
    /// <summary>
    /// <para>Used by positional lights (<see cref="OmniLight3D" /> and <see cref="SpotLight3D" />) when <c>ProjectSettings.rendering/lights_and_shadows/use_physical_light_units</c> is <c>true</c>. Sets the intensity of the light source measured in Lumens. Lumens are a measure of luminous flux, which is the total amount of visible light emitted by a light source per unit of time.</para>
    /// <para>For <see cref="SpotLight3D" />s, we assume that the area outside the visible cone is surrounded by a perfect light absorbing material. Accordingly, the apparent brightness of the cone area does not change as the cone increases and decreases in size.</para>
    /// <para>A typical household lightbulb can range from around 600 lumens to 1,200 lumens, a candle is about 13 lumens, while a streetlight can be approximately 60,000 lumens.</para>
    /// </summary>
    float LightIntensityLumens { get; set; }
    /// <summary>
    /// <para>Used by <see cref="DirectionalLight3D" />s when <c>ProjectSettings.rendering/lights_and_shadows/use_physical_light_units</c> is <c>true</c>. Sets the intensity of the light source measured in Lux. Lux is a measure of luminous flux per unit area, it is equal to one lumen per square meter. Lux is the measure of how much light hits a surface at a given time.</para>
    /// <para>On a clear sunny day a surface in direct sunlight may be approximately 100,000 lux, a typical room in a home may be approximately 50 lux, while the moonlit ground may be approximately 0.1 lux.</para>
    /// </summary>
    float LightIntensityLux { get; set; }
    /// <summary>
    /// <para>Sets the color temperature of the light source, measured in Kelvin. This is used to calculate a correlated color temperature which tints the <see cref="Light3D.LightColor" />.</para>
    /// <para>The sun on a cloudy day is approximately 6500 Kelvin, on a clear day it is between 5500 to 6000 Kelvin, and on a clear day at sunrise or sunset it ranges to around 1850 Kelvin.</para>
    /// </summary>
    float LightTemperature { get; set; }
    /// <summary>
    /// <para>The light's color. An <i>overbright</i> color can be used to achieve a result equivalent to increasing the light's <see cref="Light3D.LightEnergy" />.</para>
    /// </summary>
    Color LightColor { get; set; }
    /// <summary>
    /// <para>The light's strength multiplier (this is not a physical unit). For <see cref="OmniLight3D" /> and <see cref="SpotLight3D" />, changing this value will only change the light color's intensity, not the light's radius.</para>
    /// </summary>
    float LightEnergy { get; set; }
    /// <summary>
    /// <para>Secondary multiplier used with indirect light (light bounces). Used with <see cref="VoxelGI" /> and SDFGI (see <see cref="Environment.SdfgiEnabled" />).</para>
    /// <para><b>Note:</b> This property is ignored if <see cref="Light3D.LightEnergy" /> is equal to <c>0.0</c>, as the light won't be present at all in the GI shader.</para>
    /// </summary>
    float LightIndirectEnergy { get; set; }
    /// <summary>
    /// <para>Secondary multiplier multiplied with <see cref="Light3D.LightEnergy" /> then used with the <see cref="Environment" />'s volumetric fog (if enabled). If set to <c>0.0</c>, computing volumetric fog will be skipped for this light, which can improve performance for large amounts of lights when volumetric fog is enabled.</para>
    /// <para><b>Note:</b> To prevent short-lived dynamic light effects from poorly interacting with volumetric fog, lights used in those effects should have <see cref="Light3D.LightVolumetricFogEnergy" /> set to <c>0.0</c> unless <see cref="Environment.VolumetricFogTemporalReprojectionEnabled" /> is disabled (or unless the reprojection amount is significantly lowered).</para>
    /// </summary>
    float LightVolumetricFogEnergy { get; set; }
    /// <summary>
    /// <para><see cref="Texture2D" /> projected by light. <see cref="Light3D.ShadowEnabled" /> must be on for the projector to work. Light projectors make the light appear as if it is shining through a colored but transparent object, almost like light shining through stained-glass.</para>
    /// <para><b>Note:</b> Unlike <see cref="BaseMaterial3D" /> whose filter mode can be adjusted on a per-material basis, the filter mode for light projector textures is set globally with <c>ProjectSettings.rendering/textures/light_projectors/filter</c>.</para>
    /// <para><b>Note:</b> Light projector textures are only supported in the Forward+ and Mobile rendering methods, not Compatibility.</para>
    /// </summary>
    Texture2D LightProjector { get; set; }
    /// <summary>
    /// <para>The size of the light in Godot units. Only available for <see cref="OmniLight3D" />s and <see cref="SpotLight3D" />s. Increasing this value will make the light fade out slower and shadows appear blurrier (also called percentage-closer soft shadows, or PCSS). This can be used to simulate area lights to an extent. Increasing this value above <c>0.0</c> for lights with shadows enabled will have a noticeable performance cost due to PCSS.</para>
    /// <para><b>Note:</b> <see cref="Light3D.LightSize" /> is not affected by <see cref="Node3D.Scale" /> (the light's scale or its parent's scale).</para>
    /// <para><b>Note:</b> PCSS for positional lights is only supported in the Forward+ and Mobile rendering methods, not Compatibility.</para>
    /// </summary>
    float LightSize { get; set; }
    /// <summary>
    /// <para>The light's angular size in degrees. Increasing this will make shadows softer at greater distances (also called percentage-closer soft shadows, or PCSS). Only available for <see cref="DirectionalLight3D" />s. For reference, the Sun from the Earth is approximately <c>0.5</c>. Increasing this value above <c>0.0</c> for lights with shadows enabled will have a noticeable performance cost due to PCSS.</para>
    /// <para><b>Note:</b> <see cref="Light3D.LightAngularDistance" /> is not affected by <see cref="Node3D.Scale" /> (the light's scale or its parent's scale).</para>
    /// <para><b>Note:</b> PCSS for directional lights is only supported in the Forward+ rendering method, not Mobile or Compatibility.</para>
    /// </summary>
    float LightAngularDistance { get; set; }
    /// <summary>
    /// <para>If <c>true</c>, the light's effect is reversed, darkening areas and casting bright shadows.</para>
    /// </summary>
    bool LightNegative { get; set; }
    /// <summary>
    /// <para>The intensity of the specular blob in objects affected by the light. At <c>0</c>, the light becomes a pure diffuse light. When not baking emission, this can be used to avoid unrealistic reflections when placing lights above an emissive surface.</para>
    /// </summary>
    float LightSpecular { get; set; }
    /// <summary>
    /// <para>The light's bake mode. This will affect the global illumination techniques that have an effect on the light's rendering. See <see cref="Light3D.BakeMode" />.</para>
    /// <para><b>Note:</b> Meshes' global illumination mode will also affect the global illumination rendering. See <see cref="GeometryInstance3D.GIMode" />.</para>
    /// </summary>
    Light3D.BakeMode LightBakeMode { get; set; }
    /// <summary>
    /// <para>The light will affect objects in the selected layers.</para>
    /// </summary>
    uint LightCullMask { get; set; }
    /// <summary>
    /// <para>If <c>true</c>, the light will cast real-time shadows. This has a significant performance cost. Only enable shadow rendering when it makes a noticeable difference in the scene's appearance, and consider using <see cref="Light3D.DistanceFadeEnabled" /> to hide the light when far away from the <see cref="Camera3D" />.</para>
    /// </summary>
    bool ShadowEnabled { get; set; }
    /// <summary>
    /// <para>Used to adjust shadow appearance. Too small a value results in self-shadowing ("shadow acne"), while too large a value causes shadows to separate from casters ("peter-panning"). Adjust as needed.</para>
    /// </summary>
    float ShadowBias { get; set; }
    /// <summary>
    /// <para>Offsets the lookup into the shadow map by the object's normal. This can be used to reduce self-shadowing artifacts without using <see cref="Light3D.ShadowBias" />. In practice, this value should be tweaked along with <see cref="Light3D.ShadowBias" /> to reduce artifacts as much as possible.</para>
    /// </summary>
    float ShadowNormalBias { get; set; }
    /// <summary>
    /// <para>If <c>true</c>, reverses the backface culling of the mesh. This can be useful when you have a flat mesh that has a light behind it. If you need to cast a shadow on both sides of the mesh, set the mesh to use double-sided shadows with <see cref="GeometryInstance3D.ShadowCastingSetting.DoubleSided" />.</para>
    /// </summary>
    bool ShadowReverseCullFace { get; set; }

    float ShadowTransmittanceBias { get; set; }
    /// <summary>
    /// <para>The opacity to use when rendering the light's shadow map. Values lower than <c>1.0</c> make the light appear through shadows. This can be used to fake global illumination at a low performance cost.</para>
    /// </summary>
    float ShadowOpacity { get; set; }
    /// <summary>
    /// <para>Blurs the edges of the shadow. Can be used to hide pixel artifacts in low-resolution shadow maps. A high value can impact performance, make shadows appear grainy and can cause other unwanted artifacts. Try to keep as near default as possible.</para>
    /// </summary>
    float ShadowBlur { get; set; }
    /// <summary>
    /// <para>If <c>true</c>, the light will smoothly fade away when far from the active <see cref="Camera3D" /> starting at <see cref="Light3D.DistanceFadeBegin" />. This acts as a form of level of detail (LOD). The light will fade out over <see cref="Light3D.DistanceFadeBegin" /> + <see cref="Light3D.DistanceFadeLength" />, after which it will be culled and not sent to the shader at all. Use this to reduce the number of active lights in a scene and thus improve performance.</para>
    /// <para><b>Note:</b> Only effective for <see cref="OmniLight3D" /> and <see cref="SpotLight3D" />.</para>
    /// </summary>
    bool DistanceFadeEnabled { get; set; }
    /// <summary>
    /// <para>The distance from the camera at which the light begins to fade away (in 3D units).</para>
    /// <para><b>Note:</b> Only effective for <see cref="OmniLight3D" /> and <see cref="SpotLight3D" />.</para>
    /// </summary>
    float DistanceFadeBegin { get; set; }
    /// <summary>
    /// <para>The distance from the camera at which the light's shadow cuts off (in 3D units). Set this to a value lower than <see cref="Light3D.DistanceFadeBegin" /> + <see cref="Light3D.DistanceFadeLength" /> to further improve performance, as shadow rendering is often more expensive than light rendering itself.</para>
    /// <para><b>Note:</b> Only effective for <see cref="OmniLight3D" /> and <see cref="SpotLight3D" />, and only when <see cref="Light3D.ShadowEnabled" /> is <c>true</c>.</para>
    /// </summary>
    float DistanceFadeShadow { get; set; }
    /// <summary>
    /// <para>Distance over which the light and its shadow fades. The light's energy and shadow's opacity is progressively reduced over this distance and is completely invisible at the end.</para>
    /// <para><b>Note:</b> Only effective for <see cref="OmniLight3D" /> and <see cref="SpotLight3D" />.</para>
    /// </summary>
    float DistanceFadeLength { get; set; }
    /// <summary>
    /// <para>If <c>true</c>, the light only appears in the editor and will not be visible at runtime.</para>
    /// </summary>
    bool EditorOnly { get; set; }

}
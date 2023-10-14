namespace Chickensoft.GodotNodeInterfaces;

using Godot;
using System;


/// <summary>
/// <para>A traversable 3D region based on a <see cref="NavigationMesh" /> that <see cref="NavigationAgent3D" />s can use for pathfinding.</para>
/// <para>Two regions can be connected to each other if they share a similar edge. You can set the minimum distance between two vertices required to connect two edges by using <see cref="M:Godot.NavigationServer3D.MapSetEdgeConnectionMargin(Godot.Rid,System.Single)" />.</para>
/// <para><b>Note:</b> Overlapping two regions' navigation meshes is not enough for connecting two regions. They must share a similar edge.</para>
/// <para>The cost of entering this region from another region can be controlled with the <see cref="NavigationRegion3D.EnterCost" /> value.</para>
/// <para><b>Note:</b> This value is not added to the path cost when the start position is already inside this region.</para>
/// <para>The cost of traveling distances inside this region can be controlled with the <see cref="NavigationRegion3D.TravelCost" /> multiplier.</para>
/// <para><b>Note:</b> This node caches changes to its properties, so if you make changes to the underlying region <see cref="Rid" /> in <see cref="NavigationServer3D" />, they will not be reflected in this node's properties.</para>
/// </summary>
public interface INavigationRegion3D : INode3D {
    /// <summary>
    /// <para>Sets the <see cref="Rid" /> of the navigation map this region should use. By default the region will automatically join the <see cref="World3D" /> default navigation map so this function is only required to override the default map.</para>
    /// </summary>
    void SetNavigationMap(Rid navigationMap);
    /// <summary>
    /// <para>Returns the current navigation map <see cref="Rid" /> used by this region.</para>
    /// </summary>
    Rid GetNavigationMap();
    /// <summary>
    /// <para>Based on <paramref name="value" />, enables or disables the specified layer in the <see cref="NavigationRegion3D.NavigationLayers" /> bitmask, given a <paramref name="layerNumber" /> between 1 and 32.</para>
    /// </summary>
    void SetNavigationLayerValue(int layerNumber, bool value);
    /// <summary>
    /// <para>Returns whether or not the specified layer of the <see cref="NavigationRegion3D.NavigationLayers" /> bitmask is enabled, given a <paramref name="layerNumber" /> between 1 and 32.</para>
    /// </summary>
    bool GetNavigationLayerValue(int layerNumber);
    /// <summary>
    /// <para>Returns the <see cref="Rid" /> of this region on the <see cref="NavigationServer3D" />. Combined with <see cref="M:Godot.NavigationServer3D.MapGetClosestPointOwner(Godot.Rid,Godot.Vector3)" /> can be used to identify the <see cref="NavigationRegion3D" /> closest to a point on the merged navigation map.</para>
    /// </summary>
    Rid GetRegionRid();
    /// <summary>
    /// <para>Bakes the <see cref="NavigationMesh" />. If <paramref name="onThread" /> is set to <c>true</c> (default), the baking is done on a separate thread. Baking on separate thread is useful because navigation baking is not a cheap operation. When it is completed, it automatically sets the new <see cref="NavigationMesh" />. Please note that baking on separate thread may be very slow if geometry is parsed from meshes as async access to each mesh involves heavy synchronization. Also, please note that baking on a separate thread is automatically disabled on operating systems that cannot use threads (such as Web with threads disabled).</para>
    /// </summary>
    void BakeNavigationMesh(bool onThread);
    /// <summary>
    /// <para>The <see cref="NavigationMesh" /> resource to use.</para>
    /// </summary>
    NavigationMesh NavigationMesh { get; set; }
    /// <summary>
    /// <para>Determines if the <see cref="NavigationRegion3D" /> is enabled or disabled.</para>
    /// </summary>
    bool Enabled { get; set; }
    /// <summary>
    /// <para>If enabled the navigation region will use edge connections to connect with other navigation regions within proximity of the navigation map edge connection margin.</para>
    /// </summary>
    bool UseEdgeConnections { get; set; }
    /// <summary>
    /// <para>A bitfield determining all navigation layers the region belongs to. These navigation layers can be checked upon when requesting a path with <see cref="M:Godot.NavigationServer3D.MapGetPath(Godot.Rid,Godot.Vector3,Godot.Vector3,System.Boolean,System.UInt32)" />.</para>
    /// </summary>
    uint NavigationLayers { get; set; }
    /// <summary>
    /// <para>When pathfinding enters this region's navigation mesh from another regions navigation mesh the <see cref="NavigationRegion3D.EnterCost" /> value is added to the path distance for determining the shortest path.</para>
    /// </summary>
    float EnterCost { get; set; }
    /// <summary>
    /// <para>When pathfinding moves inside this region's navigation mesh the traveled distances are multiplied with <see cref="NavigationRegion3D.TravelCost" /> for determining the shortest path.</para>
    /// </summary>
    float TravelCost { get; set; }

}
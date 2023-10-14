namespace Chickensoft.GodotNodeInterfaces;

using Godot;
using System;
/// <summary>
/// <para>An animatable 2D physics body. It can't be moved by external forces or contacts, but can be moved manually by other means such as code, <see cref="AnimationMixer" />s (with <see cref="AnimationMixer.CallbackModeProcess" /> set to <see cref="AnimationMixer.AnimationCallbackModeProcess.Physics" />), and <see cref="RemoteTransform2D" />.</para>
/// <para>When <see cref="AnimatableBody2D" /> is moved, its linear and angular velocity are estimated and used to affect other physics bodies in its path. This makes it useful for moving platforms, doors, and other moving objects.</para>
/// </summary>
public class AnimatableBody2DAdapter : StaticBody2DAdapter, IAnimatableBody2D {
  private readonly AnimatableBody2D _node;

  public AnimatableBody2DAdapter(AnimatableBody2D node) : base(node) { _node = node; }

    /// <summary>
    /// <para>If <c>true</c>, the body's movement will be synchronized to the physics frame. This is useful when animating movement via <see cref="AnimationPlayer" />, for example on moving platforms. Do <b>not</b> use together with <see cref="PhysicsBody2D.MoveAndCollide(Godot.Vector2,System.Boolean,System.Single,System.Boolean)" />.</para>
    /// </summary>
    public bool SyncToPhysics { get => _node.SyncToPhysics; set => _node.SyncToPhysics = value; }

}
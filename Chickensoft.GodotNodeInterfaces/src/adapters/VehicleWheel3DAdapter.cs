namespace Chickensoft.GodotNodeInterfaces;

using Godot;
using System;
/// <summary>
/// <para>A node used as a child of a <see cref="VehicleBody3D" /> parent to simulate the behavior of one of its wheels. This node also acts as a collider to detect if the wheel is touching a surface.</para>
/// <para><b>Note:</b> This class has known issues and isn't designed to provide realistic 3D vehicle physics. If you want advanced vehicle physics, you may need to write your own physics integration using another <see cref="PhysicsBody3D" /> class.</para>
/// </summary>
public class VehicleWheel3DAdapter : Node3DAdapter, IVehicleWheel3D {
  private readonly VehicleWheel3D _node;

  public VehicleWheel3DAdapter(VehicleWheel3D node) : base(node) { _node = node; }

    /// <summary>
    /// <para>Slows down the wheel by applying a braking force. The wheel is only slowed down if it is in contact with a surface. The force you need to apply to adequately slow down your vehicle depends on the <see cref="RigidBody3D.Mass" /> of the vehicle. For a vehicle with a mass set to 1000, try a value in the 25 - 30 range for hard braking.</para>
    /// </summary>
    public float Brake { get => _node.Brake; set => _node.Brake = value; }
    /// <summary>
    /// <para>The damping applied to the spring when the spring is being compressed. This value should be between 0.0 (no damping) and 1.0. A value of 0.0 means the car will keep bouncing as the spring keeps its energy. A good value for this is around 0.3 for a normal car, 0.5 for a race car.</para>
    /// </summary>
    public float DampingCompression { get => _node.DampingCompression; set => _node.DampingCompression = value; }
    /// <summary>
    /// <para>The damping applied to the spring when relaxing. This value should be between 0.0 (no damping) and 1.0. This value should always be slightly higher than the <see cref="VehicleWheel3D.DampingCompression" /> property. For a <see cref="VehicleWheel3D.DampingCompression" /> value of 0.3, try a relaxation value of 0.5.</para>
    /// </summary>
    public float DampingRelaxation { get => _node.DampingRelaxation; set => _node.DampingRelaxation = value; }
    /// <summary>
    /// <para>Accelerates the wheel by applying an engine force. The wheel is only sped up if it is in contact with a surface. The <see cref="RigidBody3D.Mass" /> of the vehicle has an effect on the acceleration of the vehicle. For a vehicle with a mass set to 1000, try a value in the 25 - 50 range for acceleration.</para>
    /// <para><b>Note:</b> The simulation does not take the effect of gears into account, you will need to add logic for this if you wish to simulate gears.</para>
    /// <para>A negative value will result in the wheel reversing.</para>
    /// </summary>
    public float EngineForce { get => _node.EngineForce; set => _node.EngineForce = value; }
    /// <summary>
    /// <para>Returns the contacting body node if valid in the tree, as <see cref="Node3D" />. At the moment, <see cref="GridMap" /> is not supported so the node will be always of type <see cref="PhysicsBody3D" />.</para>
    /// <para>Returns <c>null</c> if the wheel is not in contact with a surface, or the contact body is not a <see cref="PhysicsBody3D" />.</para>
    /// </summary>
    public Node3D GetContactBody() => _node.GetContactBody();
    /// <summary>
    /// <para>Returns the rotational speed of the wheel in revolutions per minute.</para>
    /// </summary>
    public float GetRpm() => _node.GetRpm();
    /// <summary>
    /// <para>Returns a value between 0.0 and 1.0 that indicates whether this wheel is skidding. 0.0 is skidding (the wheel has lost grip, e.g. icy terrain), 1.0 means not skidding (the wheel has full grip, e.g. dry asphalt road).</para>
    /// </summary>
    public float GetSkidinfo() => _node.GetSkidinfo();
    /// <summary>
    /// <para>Returns <c>true</c> if this wheel is in contact with a surface.</para>
    /// </summary>
    public bool IsInContact() => _node.IsInContact();
    /// <summary>
    /// <para>The steering angle for the wheel, in radians. Setting this to a non-zero value will result in the vehicle turning when it's moving.</para>
    /// </summary>
    public float Steering { get => _node.Steering; set => _node.Steering = value; }
    /// <summary>
    /// <para>The maximum force the spring can resist. This value should be higher than a quarter of the <see cref="RigidBody3D.Mass" /> of the <see cref="VehicleBody3D" /> or the spring will not carry the weight of the vehicle. Good results are often obtained by a value that is about 3× to 4× this number.</para>
    /// </summary>
    public float SuspensionMaxForce { get => _node.SuspensionMaxForce; set => _node.SuspensionMaxForce = value; }
    /// <summary>
    /// <para>This value defines the stiffness of the suspension. Use a value lower than 50 for an off-road car, a value between 50 and 100 for a race car and try something around 200 for something like a Formula 1 car.</para>
    /// </summary>
    public float SuspensionStiffness { get => _node.SuspensionStiffness; set => _node.SuspensionStiffness = value; }
    /// <summary>
    /// <para>This is the distance the suspension can travel. As Godot units are equivalent to meters, keep this setting relatively low. Try a value between 0.1 and 0.3 depending on the type of car.</para>
    /// </summary>
    public float SuspensionTravel { get => _node.SuspensionTravel; set => _node.SuspensionTravel = value; }
    /// <summary>
    /// <para>If <c>true</c>, this wheel will be turned when the car steers. This value is used in conjunction with <see cref="VehicleBody3D.Steering" /> and ignored if you are using the per-wheel <see cref="VehicleWheel3D.Steering" /> value instead.</para>
    /// </summary>
    public bool UseAsSteering { get => _node.UseAsSteering; set => _node.UseAsSteering = value; }
    /// <summary>
    /// <para>If <c>true</c>, this wheel transfers engine force to the ground to propel the vehicle forward. This value is used in conjunction with <see cref="VehicleBody3D.EngineForce" /> and ignored if you are using the per-wheel <see cref="VehicleWheel3D.EngineForce" /> value instead.</para>
    /// </summary>
    public bool UseAsTraction { get => _node.UseAsTraction; set => _node.UseAsTraction = value; }
    /// <summary>
    /// <para>This determines how much grip this wheel has. It is combined with the friction setting of the surface the wheel is in contact with. 0.0 means no grip, 1.0 is normal grip. For a drift car setup, try setting the grip of the rear wheels slightly lower than the front wheels, or use a lower value to simulate tire wear.</para>
    /// <para>It's best to set this to 1.0 when starting out.</para>
    /// </summary>
    public float WheelFrictionSlip { get => _node.WheelFrictionSlip; set => _node.WheelFrictionSlip = value; }
    /// <summary>
    /// <para>The radius of the wheel in meters.</para>
    /// </summary>
    public float WheelRadius { get => _node.WheelRadius; set => _node.WheelRadius = value; }
    /// <summary>
    /// <para>This is the distance in meters the wheel is lowered from its origin point. Don't set this to 0.0 and move the wheel into position, instead move the origin point of your wheel (the gizmo in Godot) to the position the wheel will take when bottoming out, then use the rest length to move the wheel down to the position it should be in when the car is in rest.</para>
    /// </summary>
    public float WheelRestLength { get => _node.WheelRestLength; set => _node.WheelRestLength = value; }
    /// <summary>
    /// <para>This value affects the roll of your vehicle. If set to 1.0 for all wheels, your vehicle will be prone to rolling over, while a value of 0.0 will resist body roll.</para>
    /// </summary>
    public float WheelRollInfluence { get => _node.WheelRollInfluence; set => _node.WheelRollInfluence = value; }

}
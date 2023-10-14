namespace Chickensoft.GodotNodeInterfaces;

using Godot;
using System;


/// <summary>
/// <para><see cref="CenterContainer" /> is a container that keeps all of its child controls in its center at their minimum size.</para>
/// </summary>
public interface ICenterContainer : IContainer {
    /// <summary>
    /// <para>If <c>true</c>, centers children relative to the <see cref="CenterContainer" />'s top left corner.</para>
    /// </summary>
    bool UseTopLeft { get; set; }

}
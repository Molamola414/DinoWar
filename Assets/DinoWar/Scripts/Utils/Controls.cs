using System;
using UnityEngine;

/// <summary>
/// Common controls used to control characters
/// </summary>
public class Controls
{
    /// <summary>
    /// Move direction
    /// </summary>
    public Vector2 Direction;

    /// <summary>
    /// Jump button pressed
    /// </summary>
    public bool Jump;

    /// <summary>
    /// Attack direction
    /// </summary>
    public Vector2 AttackDirection;

    /// <summary>
    /// Normalize direction vector
    /// </summary>
    public void Normalize()
    {
        Direction.x = Math.Abs(Direction.x) > 0.01f ? Mathf.Sign(Direction.x) : 0;
        Direction.y = Math.Abs(Direction.y) > 0.01f ? Mathf.Sign(Direction.y) : 0;
    }
}

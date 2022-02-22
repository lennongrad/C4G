using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The file of each extension method, which are methods that can be added to existing types to extend their functionality.
/// Unfortunately, the XML comments in Visual Studio don't seem to work fo rthese
/// </summary>
public static class ExtensionMethods
{
    // Returns the Vector2 rotated by some angle
    public static Vector2 Rotated(this Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    // Returns the transform position of a MonoBehaviour object projected onto the xz plane
    public static Vector2 FlatPosition(this MonoBehaviour obj)
    {
        return new Vector2(obj.transform.position.x, obj.transform.position.z);
    }

    // Sets the Euler angle y rotation of a MonoBehaviour object to face the angle 
    public static void SetRotation(this MonoBehaviour obj, float newAngle)
    {
        obj.transform.localEulerAngles = new Vector3(obj.transform.localEulerAngles.x, newAngle, obj.transform.localEulerAngles.z);
    }

    // Rotates the MonoBehaviour object to face in the TileDirection direction using SetRotation above
    public static void RotateToFace(this MonoBehaviour obj, Tile.TileDirection facingDirection)
    {
        switch (facingDirection)
        {
            case Tile.TileDirection.Down: obj.SetRotation(0f); break;
            case Tile.TileDirection.Left: obj.SetRotation(90f); break;
            case Tile.TileDirection.Up: obj.SetRotation(180f); break;
            case Tile.TileDirection.Right: obj.SetRotation(270f); break;
        }
    }

    // Inverses the tile direction, I.E. right becomes left
    static public Tile.TileDirection Inversed(this Tile.TileDirection direction)
    {
        switch (direction)
        {
            case Tile.TileDirection.Right: return Tile.TileDirection.Left;
            case Tile.TileDirection.Down: return Tile.TileDirection.Up;
            case Tile.TileDirection.Left: return Tile.TileDirection.Right;
            case Tile.TileDirection.Up: return Tile.TileDirection.Down;
        }
        return Tile.TileDirection.None;
    }

    // Rotates the tile direction clockwise by one rotation
    static public Tile.TileDirection RotatedCW(this Tile.TileDirection direction)
    {
        switch (direction)
        {
            case Tile.TileDirection.Right: return Tile.TileDirection.Down;
            case Tile.TileDirection.Down: return Tile.TileDirection.Left;
            case Tile.TileDirection.Left: return Tile.TileDirection.Up;
            case Tile.TileDirection.Up: return Tile.TileDirection.Right;
        }
        return Tile.TileDirection.None;
    }

    // Rotates the tile direction counterclockwise by one rotation
    static public Tile.TileDirection RotatedCCW(this Tile.TileDirection direction)
    {
        switch (direction)
        {
            case Tile.TileDirection.Right: return Tile.TileDirection.Up;
            case Tile.TileDirection.Down: return Tile.TileDirection.Right;
            case Tile.TileDirection.Left: return Tile.TileDirection.Down;
            case Tile.TileDirection.Up: return Tile.TileDirection.Left;
        }
        return Tile.TileDirection.None;
    }
}

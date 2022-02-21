using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
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

    public static Vector2 FlatPosition(this MonoBehaviour obj)
    {
        return new Vector2(obj.transform.position.x, obj.transform.position.z);
    }

    public static void SetRotation(this MonoBehaviour obj, float newAngle)
    {
        obj.transform.localEulerAngles = new Vector3(obj.transform.localEulerAngles.x, newAngle, obj.transform.localEulerAngles.z);
    }

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

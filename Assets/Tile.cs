using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

static public class Tile
{
    public enum TileType { Floor, Wall, Raised, Barrier, Entrance, Exit };

    [Flags]
    public enum TileDirection
    {
        None = 0,
        Left = 1 << 0,
        Up = 1 << 1,
        Right = 1 << 2,
        Down = 1 << 3,
    };

    static public Dictionary<TileDirection, TileDirection> InverseDirection = new Dictionary<TileDirection, TileDirection>()
    {
        {TileDirection.Left, TileDirection.Right }, {TileDirection.Up, TileDirection.Down }, {TileDirection.Right, TileDirection.Left }, {TileDirection.Down, TileDirection.Up }
    };
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface Tile
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
}

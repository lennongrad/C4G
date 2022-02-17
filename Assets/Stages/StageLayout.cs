using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class StageLayout
{
    public Tile.TileType[] tiles;

    public StageLayout(Tile.TileType[] tiles)
    {
        this.tiles = tiles;
    }
}

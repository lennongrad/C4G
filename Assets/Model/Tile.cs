using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tile{
    public enum TileType { Floor, Wall, Raised, Barrier, Entrance, Exit };
    public enum TileDirection { None = -1, Left = 0, Up = 90, Right = 180, Down = 270 };
    static public Dictionary<TileDirection, TileDirection> InverseDirection = new Dictionary<TileDirection, TileDirection>()
    {
        {TileDirection.Left, TileDirection.Right }, {TileDirection.Up, TileDirection.Down }, {TileDirection.Right, TileDirection.Left }, {TileDirection.Down, TileDirection.Up }
    };

    Action<Tile> cbTypeChanged;
    Action<Tile> cbDirectionChanged;

    TileType type = TileType.Floor;
    public TileType Type{
        get
        {
            return type;
        }

        set
        {
            type = value;
            cbTypeChanged(this);
        }
    }

    TileDirection direction = TileDirection.None;
    public TileDirection Direction
    {
        get
        {
            return direction;
        }

        set
        {
            direction = value;
            cbDirectionChanged(this);
        }
    }

    public Dictionary<TileDirection, Tile> neighbors;

    // for debug
    public void printneighbors()
    {
        foreach (KeyValuePair<TileDirection, Tile> kvp in neighbors)
        {
            Debug.Log(kvp.Key + ": " + kvp.Value);
        }
    }

    Tower tower;

    World world;
    int x;
    int y;

    public Tile(World world, int x, int y){
        this.world = world;
        this.x = x;
        this.y = y;
    }

    public void RegisterTypeChangedCB(Action<Tile> cb)
    {
        cbTypeChanged += cb;
    }

    public void RegisterDirectionChangedCB(Action<Tile> cb)
    {
        cbDirectionChanged += cb;
    }
}

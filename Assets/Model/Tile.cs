using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tile {
    public enum TileType { Floor, Wall, Raised, Barrier, Entrance, Exit };

    [Flags] public enum TileDirection {
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

    Action<Tile> cbTypeChanged;
    Action<Tile> cbDirectionsChanged;

    TileType type = TileType.Floor;
    public TileType Type {
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

    (TileDirection from, TileDirection to) directions = (TileDirection.None, TileDirection.None);
    public (TileDirection from, TileDirection to) Directions
    {
        get
        {
            return directions;
        }

        set
        {
            directions = value;
            cbDirectionsChanged(this);
        }
    }

    public Dictionary<TileDirection, Tile> neighbors;
    public Tower presentTower;

    World world;
    int x;
    int y;
    float collisionWidth = 1f;
    float collisionHeight = 1f;
    Rect collisionRect;
    public Rect CollisionRect
    {
        get
        {
            return collisionRect;
        }
    }

    public Tile(World world, int x, int y){
        this.world = world;
        this.x = x;
        this.y = y;

        collisionRect = new Rect(GetPosition().x - collisionWidth / 2, GetPosition().y - collisionHeight / 2, collisionWidth, collisionHeight);
    }

    public Vector2 GetPosition()
    {
        return world.GetTilePosition(x, y);
    }

    public void RegisterTypeChangedCB(Action<Tile> cb)
    {
        cbTypeChanged += cb;
    }

    public void RegisterDirectionsChangedCB(Action<Tile> cb)
    {
        cbDirectionsChanged += cb;
    }
}

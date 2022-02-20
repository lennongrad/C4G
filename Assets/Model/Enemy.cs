using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy
{
    Action<Enemy> cbPositionChanged;

    Tile fromTile = null;
    Tile toTile = null;

    float distance = 0f;

    Vector2 position = new Vector2(0f, 0f);
    public Vector2 Position
    {
        get
        {
            return position;
        }

        set
        {
            position = value;
            cbPositionChanged(this);
        }
    }

    public Enemy(Tile spawnTile)
    {
        this.fromTile = spawnTile;
    }

    public void LogicTick()
    {

        if (toTile == null)
        {
            if(fromTile.Directions.to != Tile.TileDirection.None)
                toTile = fromTile.neighbors[fromTile.Directions.to];
            Position = fromTile.GetPosition();
        }
        else
        {
            if (Vector2.Distance(toTile.GetPosition(), position) < .02f)
            {
                fromTile = toTile;
                toTile = null;
                distance = 0f;
            }
            else
            {
                distance += .002f;
                Position = Vector2.Lerp(fromTile.GetPosition(), toTile.GetPosition(), distance);
            }
        }
    }

    public void RegisterPositionChangedCB(Action<Enemy> cb)
    {
        cbPositionChanged += cb;
    }
}

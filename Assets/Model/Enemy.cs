using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy
{
    Action<Enemy> cbPositionChanged;
    Action<Enemy> cbDespawned;

    Tile fromTile = null;
    Tile toTile = null;
    float distance = 0f;

    float hp = 1;

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
            collisionRect.x = value.x - collisionWidth / 2;
            collisionRect.y = value.y - collisionHeight / 2;
            cbPositionChanged(this);
        }
    }

    float collisionWidth = .5f;
    float collisionHeight = .5f;
    Rect collisionRect;
    public Rect CollisionRect
    {
        get
        {
            return collisionRect;
        }
    }

    public Enemy(Tile spawnTile)
    {
        this.fromTile = spawnTile;
        collisionRect = new Rect(spawnTile.GetPosition().x, spawnTile.GetPosition().y, collisionWidth, collisionHeight);
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

        if (hp < 0f)
            cbDespawned(this);
    }
    
    public void ProjectileDamage(Projectile projectile)
    {
        hp -= 2f;
    }

    public void RegisterPositionChangedCB(Action<Enemy> cb)
    {
        cbPositionChanged += cb;
    }
    public void RegisterDespawnedCB(Action<Enemy> cb)
    {
        cbDespawned += cb;
    }
}

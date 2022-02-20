using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Projectile
{

    Action<Projectile> cbPositionChanged;
    Action<Projectile> cbRotationChanged;
    Action<Projectile> cbDespawned;

    float collisionWidth = .25f;
    float collisionHeight = .25f;
    Rect collisionRect;
    public Rect CollisionRect
    {
        get
        {
            return collisionRect;
        }
    }

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

            if (cbPositionChanged != null)
                cbPositionChanged(this);
        }
    }

    float rotationAngle = 0f;
    public float RotationAngle
    {
        get
        {
            return rotationAngle;
        }
        set
        {
            rotationAngle = value;

            if(cbRotationChanged != null)
                cbRotationChanged(this);
        }
    }

    Tile.TileDirection facingDirection = Tile.TileDirection.None;
    public Tile.TileDirection FacingDirection
    {
        get
        {
            return facingDirection;
        }

        set
        {
            facingDirection = value;
        }
    }

    public bool NotDespawned = true;

    public Projectile(Tile spawnTile, Tile.TileDirection direction)
    {
        position = spawnTile.GetPosition();
        FacingDirection = direction; 
        collisionRect = new Rect(0, 0, collisionWidth, collisionHeight);
    }

    public void LogicTick()
    {
        switch (facingDirection)
        {
            case Tile.TileDirection.Left: RotationAngle = 0f; break;
            case Tile.TileDirection.Up: RotationAngle = 90f; break;
            case Tile.TileDirection.Right: RotationAngle = 180f; break;
            case Tile.TileDirection.Down: RotationAngle = 270f; break;
        }

        Position = Position + Vector2.right.Rotated(-rotationAngle) * .1f;
    }

    public void Despawn()
    {
        if(cbDespawned != null && NotDespawned)
        {
            cbDespawned(this);
            NotDespawned = false;
        }
    }

    public void RegisterPositionChangedCB(Action<Projectile> cb)
    {
        cbPositionChanged += cb;
    }
    public void RegisterRotationChangedCB(Action<Projectile> cb)
    {
        cbRotationChanged += cb;
    }
    public void RegisterDespawnedCB(Action<Projectile> cb)
    {
        cbDespawned += cb;
    }
}

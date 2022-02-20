using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tower
{
    Action<Tower> cbPositionChanged;
    Action<Tower> cbRotationChanged;
    Action<Tower, Projectile> cbProjectileReleased;

    Tile parentTile = null;

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

            if(cbPositionChanged != null)
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

    public Tower(Tile spawnTile, Tile.TileDirection direction)
    {
        this.parentTile = spawnTile;
        FacingDirection = direction;
    }

    int timer = 70;
    public void LogicTick()
    {
        Position = parentTile.GetPosition();
        switch (facingDirection)
        {
            case Tile.TileDirection.Left: RotationAngle = 0f; break;
            case Tile.TileDirection.Up: RotationAngle = 90f; break;
            case Tile.TileDirection.Right: RotationAngle = 180f; break;
            case Tile.TileDirection.Down: RotationAngle = 270f; break;
        }

        timer += 1;
        if(timer > 100)
        {
            timer = 0;
            Projectile newProjectile = new Projectile(parentTile, facingDirection);
            cbProjectileReleased(this, newProjectile);
        }
    }

    public void RegisterPositionChangedCB(Action<Tower> cb)
    {
        cbPositionChanged += cb;
    }
    public void RegisterRotationChangedCB(Action<Tower> cb)
    {
        cbRotationChanged += cb;
    }
    public void RegisterProjectileReleasedCB(Action<Tower, Projectile> cb)
    {
        cbProjectileReleased += cb;
    }
}

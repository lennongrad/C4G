using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyController : MonoBehaviour
{
    /// <summary>
    /// The tile the enemy started wallking from
    /// </summary>
    TileController fromTile = null;
    /// <summary>
    /// The tile the enemy is walking to currently
    /// </summary>
    TileController toTile = null;
    /// <summary>
    /// The last tile that the enemy stopped at, and which it is travelling from. 
    /// Setting this publically automatically changes its destination tile if valid
    /// </summary>
    public TileController FromTile
    {
        set
        {
            fromTile = value;
            transform.position = value.transform.position;

            if (value.Directions.to != Tile.TileDirection.None)
                toTile = value.Neighbors[fromTile.Directions.to];
        }
    }

    /// <summary>
    /// The percentage distance along the path from the last to next tile that the enemy has travelled
    /// </summary>
    float distance = 0f;
    /// <summary>
    /// The enemy's health points. When they reach 0, FixedUpdate() will despawn the enemy
    /// </summary>
    float hp = 1;

    Action<EnemyController> cbDespawned;
    /// <summary>
    /// Register a function to be called when this enemy is set to despawn from being destroyed
    /// </summary>
    public void RegisterDespawnedCB(Action<EnemyController> cb){ cbDespawned += cb; }

    void OnEnable()
    {
        hp = 1;
        distance = 0f;
    }

    void FixedUpdate()
    {
        if (Vector2.Distance(toTile.FlatPosition(), this.FlatPosition()) < .02f)
        {
            FromTile = toTile;
            distance = 0f;
        }
        else
        {
            distance += .002f;
                
            Vector2 flatPosition = Vector2.Lerp(fromTile.FlatPosition(), toTile.FlatPosition(), distance);
            transform.position = new Vector3(flatPosition.x, 0, flatPosition.y);
        }

        if (hp < 0f)
            cbDespawned(this);
    }

    /// <summary>
    /// Call when the enemy collides with a projectile.
    /// </summary>
    void projectileDamage(ProjectileController projectile)
    {
        hp -= 2f;
        projectile.HitEnemy();
    }

    public void OnTriggerEnter(Collider trigger)
    {
        ProjectileController projectileHitBy = trigger.transform.parent.GetComponent<ProjectileController>();
        projectileDamage(projectileHitBy);
    }
}

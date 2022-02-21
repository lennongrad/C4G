using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyController : MonoBehaviour
{
    TileController fromTile = null;
    TileController toTile = null;
    public TileController FromTile
    {
        set
        {
            fromTile = value;
            transform.position = value.transform.position;

            if (value.Directions.to != Tile.TileDirection.None)
                toTile = value.neighbors[fromTile.Directions.to];
        }
    }

    float distance = 0f;

    float hp = 1;

    Action<EnemyController> cbDespawned;
    public void RegisterDespawnedCB(Action<EnemyController> cb){ cbDespawned += cb; }

    void OnEnable()
    {
        hp = 1;
        distance = 0f;
    }

    void Update()
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

    public void OnTriggerEnter(Collider trigger)
    {
        ProjectileController projectileHitBy = trigger.transform.parent.GetComponent<ProjectileController>();
        projectileDamage(projectileHitBy);
    }

    void projectileDamage(ProjectileController projectile)
    {
        hp -= 2f;
        projectile.HitEnemy();
    }
}

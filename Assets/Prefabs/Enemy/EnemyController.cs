using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyController : MonoBehaviour
{
    public GameObject enemyModel;
    public HPBarController hpBar;

    /// <summary>
    /// The behaviours for the enemy to carry out, such as spawning materials, generating mana, etc.
    /// </summary>
    EnemyBehaviour[] behaviours;

    bool performBehaviours = true;
    /// <summary>
    /// Whether or not the enemy should have its behaviours active. 
    /// </summary>
    public bool PerformBehaviours
    {
        get { return performBehaviours; }
        set
        {
            performBehaviours = value;
            GetComponent<Collider>().enabled = performBehaviours;
        }
    }

    /// <summary>
    /// The HP the enemy starts at
    /// </summary>
    public float baseHP;
    /// <summary>
    /// The enemy's health points. When they reach 0, FixedUpdate() will despawn the enemy
    /// </summary>
    float hp = 1;

    /// <summary>
    /// The speed the enemy will move at across the stage if they have a variance of 0
    /// </summary>
    public float BaseSpeed = 1f;
    /// <summary>
    /// The the maximum additional speed an enemy can spawn with randomly
    /// </summary>
    public float SpeedVariance = 1f;

    /// <summary>
    /// Specifier of the enemy type. used to call the correct animation for each enemy.
    /// </summary>
    public string enemyType;



    /// <summary>
    /// The tile the enemy started wallking from
    /// </summary>
    public TileController fromTile = null;
    /// <summary>
    /// The tile the enemy is walking to currently
    /// </summary>
    public TileController toTile = null;
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
                if (value.Neighbors.ContainsKey(fromTile.Directions.to))
                    toTile = value.Neighbors[fromTile.Directions.to];
                else
                    cbDespawned(this);
        }
    }

    /// <summary>
    /// The percentage distance along the path from the last to next tile that the enemy has travelled
    /// </summary>
    float distance = 0f;
    /// <summary>
    /// When enemy is created, give it random additioanl speed so that enemies don''t all move the same exactly
    /// </summary>
    float randomSpeed;

    Action<EnemyController> cbDespawned;
    /// <summary>
    /// Register a function to be called when this enemy is set to despawn from being destroyed
    /// </summary>
    public void RegisterDespawnedCB(Action<EnemyController> cb) { cbDespawned += cb; }

    /// <summary>
    /// Set to true when the enemy is hovered over which is then monitored in the next FixedUpdate()
    /// </summary>
    bool hovered = false;

    /// <summary>
    /// The current tower that the enemy is blocked from moving forward by and which it will attack.
    /// </summary>
    public TowerController currentTowerColliding;

    Action<EnemyController> cbHovered;
    /// <summary>
    /// Register a method to be called when the enemy is hovered over by the user's mouse cursor
    /// </summary>
    public void RegisterHoveredCB(Action<EnemyController> cb) { cbHovered += cb; }

    void OnEnable()
    {
        hp = baseHP;
        distance = 0f;
        cbDespawned = null;
        randomSpeed = (UnityEngine.Random.Range(0, SpeedVariance) + BaseSpeed) * .01f;
        hpBar.Maximum = baseHP;

        //Walker running animation
        if (this.enemyType == "Walker")
            enemyModel.GetComponent<Animator>().Play("infantry_03_run");

        //Wizard running animation
        if (this.enemyType == "Wizard")
            enemyModel.GetComponent<Animator>().Play("BattleWalkForward");

        behaviours = GetComponents<EnemyBehaviour>();
    }

    void Initiate()
    {
        foreach (EnemyBehaviour behaviour in behaviours)
        {
            behaviour.OnInitiate();
        }
    }

    void FixedUpdate()
    {
        if (toTile == null || fromTile == null)
        {
            cbDespawned(this);
        }

        if (currentTowerColliding == null)
        {
            // move
            if (Vector2.Distance(toTile.FlatPosition(), this.FlatPosition()) < .02f)
            {
                FromTile = toTile;
                distance = 0f;
            }

            /*
            // Ranged enemy detection range
            // Moved this to the shoot projectile behaviour
            if ((this.enemyType == "Wizard" | this.enemyType == "Paladin") && isProjectileUser() && enemyModel.GetComponent<ShootProjectile>().projectileTimer < 0)
            {
                DetectionRange();
            }*/
            distance += randomSpeed;

            Vector2 flatPosition = Vector2.Lerp(fromTile.FlatPosition(), toTile.FlatPosition(), distance);
            transform.position = new Vector3(flatPosition.x, 0, flatPosition.y);

            this.RotateToFace(fromTile.Directions.to);
            hpBar.transform.localPosition = (new Vector3(-.25f, 0, .25f)).Rotated(fromTile.Directions.to);
        }
        else
        {
            // attack tower
            currentTowerColliding.DirectDamage(1f * Time.deltaTime);
        }


        if (hovered)
        {
            transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        hovered = false;

        if (hp <= 0f)
        {
            cbDespawned(this);
        }
        else
        {
            hpBar.Amount = hp;
            hpBar.gameObject.SetActive(hp < baseHP);
        }
    }

    /// <summary>
    /// Called when the tower is hovered over by the user's mouse cursor. Sets hovered to true and calls hovered callback
    /// </summary>
    public void Hover()
    {
        hovered = true;
        if (cbHovered != null)
            cbHovered(this);
    }

    /// <summary>
    /// Call when something like a card effect deals damage directly to the enemy, rather than through a projectile
    /// </summary>
    public void DirectDamage(float damage)
    {
        hp -= damage;
    }

    /// <summary>
    /// Call when the enemy collides with a projectile.
    /// </summary>
    void projectileDamage(ProjectileController projectile)
    {
        hp -= projectile.GetDamage(this);
        projectile.Hit();
    }

    void towerStartAttacking(TowerController tower)
    {
        currentTowerColliding = tower;
        currentTowerColliding.RegisterDespawnedCB(towerStopAttacking);

        //Walker attack animation
        if (this.enemyType == "Walker")
            enemyModel.GetComponent<Animator>().Play("infantry_04_attack_A");

        //Wizard attack animation
        if (this.enemyType == "Wizard")
            enemyModel.GetComponent<Animator>().SetBool("Attacking", true);
    }

    void towerStopAttacking(TowerController tower)
    {
        if (tower == currentTowerColliding)
        {
            if (currentTowerColliding != null)
            {
                currentTowerColliding.UnregisterDespawnedCB(towerStopAttacking);
                currentTowerColliding = null;
            }

            //Walker running animation
            if (this.enemyType == "Walker")
                enemyModel.GetComponent<Animator>().Play("infantry_03_run");

            //Wizard running animation
            if (this.enemyType == "Wizard")
                enemyModel.GetComponent<Animator>().SetBool("Attacking", false);
        }
    }

    /// <summary>
    ///Check if unit is able to fire projectiles
    /// <summary>
    private bool isProjectileUser()
    {
        if (enemyModel.GetComponent<ShootProjectile>() != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public void OnTriggerEnter(Collider trigger)
    {
        ProjectileController projectileColliding = trigger.transform.parent.GetComponent<ProjectileController>();
        if (projectileColliding != null && !projectileColliding.isEvil)
            projectileDamage(projectileColliding);

        TowerController towerColliding = trigger.GetComponent<TowerController>();
        if (towerColliding != null)
            towerStartAttacking(towerColliding);
    }

    public void OnTriggerExit(Collider trigger)
    {
        TowerController towerColliding = trigger.GetComponent<TowerController>();
        towerStopAttacking(towerColliding);
    }
}
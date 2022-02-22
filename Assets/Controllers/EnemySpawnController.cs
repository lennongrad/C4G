using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Class that manages spawning enemies in waves
/// </summary>
public class EnemySpawnController : MonoBehaviour
{
    //debug variables
    public GameObject enemyPrefab;
    int enemySpawnTimer = 90;

    public List<TileController> activeEntrances;
    /// <summary>
    /// The entrances that are available to spawn enemies out of
    /// </summary>
    public List<TileController> ActiveEntrances
    {
        set { activeEntrances = value; }
    }

    /// <summary>
    /// The list of active enemies on screen
    /// </summary>
    List<EnemyController> enemies = new List<EnemyController>();

    Action<EnemyController> cbEnemySpawned;
    /// <summary>
    /// Register a function to be called whenever the an enemy is spawned
    /// </summary>
    public void RegisterEnemySpawnedCB(Action<EnemyController> cb) { cbEnemySpawned += cb; }

    void Start()
    {
        // Preload a certain number of each enemy type
        SimplePool.Preload(enemyPrefab, 20, this.transform);
    }

    void FixedUpdate()
    {
        enemySpawnTimer += 1;
        if (enemySpawnTimer > 100)
        {
            if (enemies.Count < 3 && activeEntrances.Count != 0)
            {
                enemySpawn(activeEntrances[UnityEngine.Random.Range(0, activeEntrances.Count)]);
            }
            enemySpawnTimer = 0;
        }
    }

    /// <summary>
    /// Spawns an enemy at a specified entrance
    /// </summary>
    void enemySpawn(TileController entrance)
    {
        Vector3 enemyPosition = new Vector3(0f, 0f, 0f);
        GameObject enemyObject = SimplePool.Spawn(enemyPrefab, enemyPosition, Quaternion.identity);
        EnemyController enemyController = enemyObject.GetComponent<EnemyController>();

        enemyController.FromTile = entrance;

        enemyObject.transform.parent = this.transform;
        enemyController.RegisterDespawnedCB((enemy) => { SimplePool.Despawn(enemyObject); enemies.Remove(enemy); });

        enemies.Add(enemyController);

        if(cbEnemySpawned != null)
            cbEnemySpawned(enemyController);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemySpawnController : MonoBehaviour
{
    public GameObject enemyPrefab;

    public List<TileController> activeEntrances;
    /// <summary>
    /// The entrances that
    /// </summary>
    public List<TileController> ActiveEntrances
    {
        set { activeEntrances = value; }
    }

    int enemySpawnTimer = 90;
    List<EnemyController> enemies = new List<EnemyController>();

    Action<EnemyController> cbEnemySpawned;
    public void RegisterEnemySpawnedCB(Action<EnemyController> cb) { cbEnemySpawned += cb; }

    void Start()
    {
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

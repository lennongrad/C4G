using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

/// <summary>
/// Class that manages spawning enemies in waves
/// </summary>
public class EnemySpawnController : MonoBehaviour
{
    public TargetSelectionController targetSelectionController;
    public CycleController cycleController;
    public PlayerResourceManager playerResourceManager;
    public GameObject startRoundButton;
    public GameObject startCycleButton;

    int enemySpawnTimer = 0;
    public float beginRange = 160f;

    public RoundData GameRound;
    public RoundData DebugRound;

    public GameEvent roundBegin;
    public GameEvent roundEnd;

    RoundData currentRound;
    float timeSinceRoundBegin = 0f;
    public int roundIndex = -1;
    public bool spawnedAllEnemies = true;

    int waveIndex;
    WaveData CurrentWave
    {
        get { return currentRound.EnemyWaves[waveIndex]; }
    }

    public List<TileController> activeEntrances;
    /// <summary>
    /// The entrances that are available to spawn enemies out of
    /// </summary>
    public List<TileController> ActiveEntrances
    {
        set { activeEntrances = value; }
    }

    /// <summary>
    /// The count of active enemies on screen
    /// </summary>
    int enemiesCount = 0;

    Action<EnemyController> cbEnemySpawned;
    /// <summary>
    /// Register a function to be called whenever the an enemy is spawned
    /// </summary>
    public void RegisterEnemySpawnedCB(Action<EnemyController> cb) { cbEnemySpawned += cb; }

    void Start()
    {
        if (playerResourceManager.isDebug)
            currentRound = DebugRound;
        else
            currentRound = GameRound;

        setWave(0);
    }

    void FixedUpdate()
    {
        if (!spawnedAllEnemies)
        {
            timeSinceRoundBegin += Time.deltaTime * .5f;
            if (UnityEngine.Random.Range(0f, beginRange) < timeSinceRoundBegin)
                enemySpawnTimer += 1;

            if (enemySpawnTimer > (20 + enemiesCount * 60))
            {
                if (CurrentWave.Enemies.Where(x => x.canSpawnMore(roundIndex)).Count() >= 1)
                {
                    // one or more enemies have remaining enemies to spawn
                    if (attemptSpawn())
                        enemySpawnTimer = 0;
                }
                else
                {
                    // no enemies are left to spawn in this wave
                    if (waveIndex + 1 < currentRound.EnemyWaves.Count())
                    {
                        timeSinceRoundBegin = 0;
                        setWave(waveIndex + 1);
                    }
                    else if (enemiesCount == 0)
                    {
                        spawnedAllEnemies = true;
                        startRoundButton.SetActive(true);
                        startCycleButton.SetActive(false);
                        roundEnd.Raise();
                    }
                }
            }
        }
    }

    void setWave(int index)
    {
        waveIndex = index;

        foreach (WaveEnemyData data in CurrentWave.Enemies)
            data.Restart();
    }

    public void startRound()
    {
        if (spawnedAllEnemies)
        {
            roundIndex += 1;
            setWave(0);
            timeSinceRoundBegin = 0f;
            spawnedAllEnemies = false;
            startRoundButton.SetActive(false);
            startCycleButton.SetActive(true);

            roundBegin.Raise();
        }
    }

    // try to spawn an enemy from the wave parameters; returns true if an enemy was spawned
    bool attemptSpawn()
    {
        if (activeEntrances.Count == 0)
            return false;

        WaveEnemyData chosenEnemy = CurrentWave.Enemies[UnityEngine.Random.Range(0, CurrentWave.Enemies.Count)];
        TileController chosenEntrance = activeEntrances[UnityEngine.Random.Range(0, activeEntrances.Count)];

        if (!chosenEnemy.canSpawnMore(roundIndex))
            return false;

        enemySpawn(chosenEnemy.prefab, chosenEntrance);
        chosenEnemy.enemySpawned();
        return true;
    }

    /// <summary>
    /// Spawns an enemy at a specified entrance
    /// </summary>
    void enemySpawn(GameObject enemyPrefab, TileController entrance)
    {
        Vector3 enemyPosition = new Vector3(0f, 0f, 0f);
        GameObject enemyObject = Instantiate(enemyPrefab, enemyPosition, Quaternion.identity);
        EnemyController enemyController = enemyObject.GetComponent<EnemyController>();

        enemyObject.transform.SetParent(this.transform);
        enemyController.RegisterDespawnedCB(OnEnemyDespawn);
        enemyController.RegisterHoveredCB(targetSelectionController.EnemyHovered);

        enemyController.FromTile = entrance;

        enemiesCount += 1;

        if(cbEnemySpawned != null)
            cbEnemySpawned(enemyController);
    }

    void OnEnemyDespawn(EnemyController enemy, bool escaped)
    {
        if (escaped)
        {
            playerResourceManager.LifeTotal -= enemy.LifeLossAmount;
        }

        Destroy(enemy.gameObject);
        enemiesCount -= 1;
    }
}

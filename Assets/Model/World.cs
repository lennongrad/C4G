using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
///     The class that manages all of the inner game logic, including passing along LogicTick() calls.
/// </summary>
public class World {
    Tile[,] tiles;
    StageData stageData;
    List<Tile> entrances = new List<Tile>();
    List<Tile> exits = new List<Tile>();

    List<Enemy> activeEnemies = new List<Enemy>();
    List<Tower> activeTowers = new List<Tower>();
    List<Tile> activeEntrances = new List<Tile>();
    List<Projectile> activeProjectiles = new List<Projectile>();

    /// <summary>
    /// Projectiles that will get despawned at the end of each LogicTick()
    /// </summary>
    List<Projectile> projectilesToDespawn = new List<Projectile>();
    /// <summary>
    /// Enemies that will get despawned at the end of each LogicTick()
    /// </summary>
    List<Enemy> enemiesToDespawn = new List<Enemy>();

    /// <summary>
    /// Public accessor for the width by tiles of the stage.
    /// </summary>
    public int Width
    {
        get
        {
            return stageData.Width;
        }
    }

    /// <summary>
    /// Public accessor for the height by tiles of the stage.
    /// </summary>
    public int Height
    {
        get
        {
            return stageData.Height;
        }
    }

    /// <summary>
    /// Functions that are called when the world spawns a new enemy.
    /// </summary>
    Action<Enemy> cbEnemySpawn;
    /// <summary>
    /// Functions that are called when the world spawns a new tower.
    /// </summary>
    Action<Tower> cbTowerSpawn;

    public World(StageData stageData){
        this.stageData = stageData;

        tiles = new Tile[stageData.Width, stageData.Height];

        for(int x = 0; x < stageData.Width; x++)
        {
            for(int y = 0; y < stageData.Height; y++)
            {
                tiles[x, y] = new Tile(this, x, y);
            }
        }
    }

    /// <summary>
    /// A second initializer that sets up the values of all the tiles once their game objects have been created
    /// Have to wait to make visual objects before their callbacks can be registered and changes recognized
    /// </summary>
    public void SetWorld()
    {
        for (int x = 0; x < stageData.Width; x++)
        {
            for (int y = 0; y < stageData.Height; y++)
            {
                tiles[x, y].Type = stageData.Layout[y].tiles[x];
                tiles[x, y].Directions = (Tile.TileDirection.None, Tile.TileDirection.None);

                if (tiles[x, y].Type == Tile.TileType.Entrance)
                    entrances.Add(tiles[x, y]);
                if (tiles[x, y].Type == Tile.TileType.Exit)
                    exits.Add(tiles[x, y]);

                Dictionary<Tile.TileDirection, Tile> neighbors = new Dictionary<Tile.TileDirection, Tile>();
                if (x != 0)
                    neighbors[Tile.TileDirection.Left] = tiles[x - 1, y];
                if (x != stageData.Width - 1)
                    neighbors[Tile.TileDirection.Right] = tiles[x + 1, y];
                if (y != 0)
                    neighbors[Tile.TileDirection.Up] = tiles[x, y - 1];
                if (y != stageData.Height - 1)
                    neighbors[Tile.TileDirection.Down] = tiles[x, y + 1];

                tiles[x, y].neighbors = neighbors;
            }
        }

        RandomizePaths();
    }

    /// <summary>
    /// Sets the enemy travel paths at random. Always tries to make exactly three paths. Uses GeneratePath().
    /// </summary>
    public void RandomizePaths()
    {
        int totalSuccesses = 0;
        int totalAttempts = 0;

        while (totalSuccesses != 3 && totalAttempts < 50)
        {
            totalSuccesses = 0;
            for (int x = 0; x < stageData.Width; x++)
            {
                for (int y = 0; y < stageData.Height; y++)
                {
                    tiles[x, y].Directions = (Tile.TileDirection.None, Tile.TileDirection.None);
                }
            }

            for (int i = 0; i < 100 && totalSuccesses < 3; i++)
            {
                if (GeneratePath())
                {
                    totalSuccesses += 1;
                }
            }

            totalAttempts++;
        }

        activeEntrances.Clear();
        for (int x = 0; x < stageData.Width; x++)
        {
            for (int y = 0; y < stageData.Height; y++)
            {
                if(tiles[x,y].Type == Tile.TileType.Entrance && tiles[x,y].Directions.to != Tile.TileDirection.None)
                {
                    activeEntrances.Add(tiles[x,y]);
                }
            }
        }
    }

    /// <summary>
    /// Create one enemy walking path for RandomizePath().
    /// </summary>
    /// <returns></returns>
    private bool GeneratePath()
    { 
        Tile entrance = entrances[UnityEngine.Random.Range(0, entrances.Count)];
        HashSet<Tile> unvisited = new HashSet<Tile>();
        HashSet<Tile> visited = new HashSet<Tile>();
        Dictionary<Tile, Tile.TileDirection> fromDirection = new Dictionary<Tile, Tile.TileDirection>();

        foreach(Tile tile in tiles)
        {
            unvisited.Add(tile);
        }
        unvisited.Remove(entrance);
        visited.Add(entrance);

        Queue<Tile> queue = new Queue<Tile>();
        queue.Enqueue(entrance);
        bool stopSearching = false;

        Tile goalTile = null;
        List<Tile> path = new List<Tile>();
        while(queue.Count > 0 && !stopSearching)
        {
            Tile nextTile = queue.Dequeue();
            List<Tile.TileDirection> keys = new List<Tile.TileDirection>(nextTile.neighbors.Keys);

            foreach (Tile.TileDirection direction in keys)
            {
                Tile tile = nextTile.neighbors[direction];

                if(tile.Type == Tile.TileType.Exit)
                {
                    stopSearching = true;
                    fromDirection[tile] = Tile.InverseDirection[direction];
                    goalTile = tile;
                    path.Add(tile);
                    goalTile.Directions = (Tile.InverseDirection[direction], direction);
                }
                else if(!visited.Contains(tile) && tile.Type == Tile.TileType.Floor && tile.Directions.from == Tile.TileDirection.None && UnityEngine.Random.Range(0,5) != 0)
                {
                    visited.Add(tile);
                    unvisited.Remove(tile);
                    queue.Enqueue(tile);
                    fromDirection[tile] = Tile.InverseDirection[direction];
                }
            }
        }

        if(goalTile != null)
        {
            stopSearching = false;
            while (!stopSearching)
            {

                Tile nextTile = goalTile.neighbors[fromDirection[goalTile]];
                path.Add(nextTile);

                if (nextTile.Type == Tile.TileType.Entrance)
                {
                    stopSearching = true;
                    nextTile.Directions = (fromDirection[goalTile], Tile.InverseDirection[fromDirection[goalTile]]);
                }
                else
                {
                    nextTile.Directions = (fromDirection[nextTile], Tile.InverseDirection[fromDirection[goalTile]]);
                }

                goalTile = nextTile;
            }
            path.Reverse();

            return true;
        }
        else
        {
            return false;
        }
    }

    int enemySpawnTimer = 90; // debug
    /// <summary>
    /// Called once per physics update call to WorldController, propogates to all other logical objects
    /// </summary>
    public void LogicTick()
    {
        enemySpawnTimer += 1;
        if(enemySpawnTimer > 100)
        {
            if(activeEnemies.Count < 1 && activeEntrances.Count != 0)
            {
                EnemySpawn(activeEntrances[UnityEngine.Random.Range(0, activeEntrances.Count)]);
            }
            enemySpawnTimer = 0;
        }

        foreach(Enemy enemy in activeEnemies)
        {
            enemy.LogicTick();
        }

        foreach (Tower tower in activeTowers)
        {
            tower.LogicTick();
        }

        foreach (Projectile projectile in activeProjectiles)
        {
            projectile.LogicTick();
        }

        activeProjectiles.RemoveAll(x => projectilesToDespawn.Contains(x));
        projectilesToDespawn = new List<Projectile>();
        activeEnemies.RemoveAll(x => enemiesToDespawn.Contains(x));
        enemiesToDespawn = new List<Enemy>();
    }
    
    List<Tile.TileType> canPlaceTiles = new List<Tile.TileType>() { Tile.TileType.Floor, Tile.TileType.Raised }; // temporary, will probably be per tower type
    /// <summary>
    /// Registers a click to place a tower. Probably debug
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="direction">The direction the tower should be facing</param>
    public void Click(int x, int y, Tile.TileDirection direction)
    {
        if(GetTileAt(x,y) != null && canPlaceTiles.Contains(GetTileAt(x, y).Type) && GetTileAt(x,y).presentTower == null)
        {
            TowerSpawn(GetTileAt(x, y), direction);
        }
    }

    /// <summary>
    /// Spawns a new enemy at a certain entrance
    /// </summary>
    /// <param name="entrance">The entrance the enemy will walk out of</param>
    public void EnemySpawn(Tile entrance)
    {
        Enemy newEnemy = new Enemy(entrance);
        activeEnemies.Add(newEnemy);
        cbEnemySpawn(newEnemy);
        newEnemy.RegisterDespawnedCB((enemy) => { enemiesToDespawn.Add(enemy); });
    }

    /// <summary>
    /// Creates a new tower at a certain tile
    /// </summary>
    /// <param name="parentTile">The tile that tower rests on</param>
    /// <param name="direction">The direction the tower should be facing</param>
    public void TowerSpawn(Tile parentTile, Tile.TileDirection direction)
    {
        Tower newTower = new Tower(parentTile, direction);
        activeTowers.Add(newTower);
        cbTowerSpawn(newTower);
        parentTile.presentTower = newTower;
        newTower.RegisterProjectileReleasedCB((tower, projectile) => { ProjectileSpawn(projectile); });
    }

    /// <summary>
    /// Callback function called when a projectile is spanwed
    /// </summary>
    /// <param name="newProjectile">The projectile being spawned</param>
    void ProjectileSpawn(Projectile newProjectile)
    {
        activeProjectiles.Add(newProjectile);
        newProjectile.RegisterPositionChangedCB(ProjectileCollisionCheck);
        newProjectile.RegisterDespawnedCB((projectile) => { projectilesToDespawn.Add(projectile); });
    }

    /// <summary>
    /// Called whenever a projectile moves to determine if it should collide with anything
    /// </summary>
    void ProjectileCollisionCheck(Projectile projectile)
    {
        bool inBounds = false;
        for (int x = 0; x < stageData.Width; x++)
        {
            for (int y = 0; y < stageData.Height; y++)
            {
                if (tiles[x, y].CollisionRect.Overlaps(projectile.CollisionRect))
                {
                    if (tiles[x, y].Type == Tile.TileType.Wall)
                        projectile.Despawn();
                    inBounds = true;
                }
            }
        }

        foreach(Enemy enemy in activeEnemies)
        {
            if(enemy.CollisionRect.Overlaps(projectile.CollisionRect))
            {
                projectile.Despawn();
                enemy.ProjectileDamage(projectile);
            }
        }

        if (!inBounds && projectile.NotDespawned)
            projectile.Despawn();
    }

    /// <summary>
    /// Checks if a tile is in bound then returns it if it is
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Tile GetTileAt(int x, int y){
        if(x < 0 || x > stageData.Width || y < 0 || y > stageData.Height)
        {
            Debug.LogError("Tile (" + x + ", " + y + ") is out of range.");
            return null;
        }
        return tiles[x, y];
    }

    /// <summary>
    /// Gets the physical position of the middle of a tile off of its x/y coordinates
    /// </summary>
    public Vector2 GetTilePosition(int x, int y)
    {
        return new Vector2(-x + stageData.Width / 2 - (float)(1 - stageData.Width % 2) / 2, y - stageData.Height / 2 + (float)(1 - stageData.Height % 2) / 2);
    }

    /// <summary>
    /// Registers a new function to be called when an enemy spawns
    /// </summary>
    public void RegisterEnemySpawnCB(Action<Enemy> cb)
    {
        cbEnemySpawn += cb;
    }

    /// <summary>
    /// Register a new function to be called when a tower spawns
    /// </summary>
    /// <param name="cb"></param>
    public void RegisterTowerSpawnCB(Action<Tower> cb)
    {
        cbTowerSpawn += cb;
    }
}

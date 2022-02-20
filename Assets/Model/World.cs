using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class World {
    Tile[,] tiles;
    StageData stageData;
    List<Enemy> activeEnemies = new List<Enemy>();
    List<Tower> activeTowers = new List<Tower>();
    List<Tile> activeEntrances = new List<Tile>();
    List<Projectile> activeProjectiles = new List<Projectile>();

    List<Tile> entrances = new List<Tile>();
    List<Tile> exits = new List<Tile>();

    List<Projectile> projectilesToDespawn = new List<Projectile>();
    List<Enemy> enemiesToDespawn = new List<Enemy>();

    int width;
    public int Width{
        get
        {
            return width;
        }
    }

    int height;
    public int Height{
        get
        {
            return height;
        }
    }

    Action<Enemy> cbEnemySpawn;
    Action<Tower> cbTowerSpawn;

    public World(StageData stageData){
        this.stageData = stageData;
        width = stageData.layout[0].tiles.Length;
        height = stageData.layout.Length;

        tiles = new Tile[width, height];

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                tiles[x, y] = new Tile(this, x, y);
            }
        }
    }

    public void SetWorld()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles[x, y].Type = stageData.layout[y].tiles[x];
                tiles[x, y].Directions = (Tile.TileDirection.None, Tile.TileDirection.None);

                if (tiles[x, y].Type == Tile.TileType.Entrance)
                    entrances.Add(tiles[x, y]);
                if (tiles[x, y].Type == Tile.TileType.Exit)
                    exits.Add(tiles[x, y]);

                Dictionary<Tile.TileDirection, Tile> neighbors = new Dictionary<Tile.TileDirection, Tile>();
                if (x != 0)
                    neighbors[Tile.TileDirection.Left] = tiles[x - 1, y];
                if (x != width - 1)
                    neighbors[Tile.TileDirection.Right] = tiles[x + 1, y];
                if (y != 0)
                    neighbors[Tile.TileDirection.Up] = tiles[x, y - 1];
                if (y != height - 1)
                    neighbors[Tile.TileDirection.Down] = tiles[x, y + 1];

                tiles[x, y].neighbors = neighbors;
            }
        }

        RandomizePaths();
    }

    public void RandomizePaths()
    {
        int totalSuccesses = 0;
        int totalAttempts = 0;

        while (totalSuccesses != 3 && totalAttempts < 50)
        {
            totalSuccesses = 0;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
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
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(tiles[x,y].Type == Tile.TileType.Entrance && tiles[x,y].Directions.to != Tile.TileDirection.None)
                {
                    activeEntrances.Add(tiles[x,y]);
                }
            }
        }
    }

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
            keys = ShuffleList<Tile.TileDirection>(keys);

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

    List<T> ShuffleList<T>(List<T> inputList)
    {
        for (int i = 0; i < inputList.Count - 1; i++)
        {
            T temp = inputList[i];
            int rand = UnityEngine.Random.Range(i, inputList.Count);
            inputList[i] = inputList[rand];
            inputList[rand] = temp;
        }
        return inputList;
    }

    public void RandomizeTiles()
    {
        for(int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (UnityEngine.Random.Range(0, 2) == 0)
                {
                    // mostly flat tiles
                    GetTileAt(x, y).Type = Tile.TileType.Floor;

                    // but at the edges, possible change to entrances/exits or wall if corner
                    if ((x == 0 || x == width - 1) && (y == 0 || y == height - 1))
                    {
                        // at corner
                        GetTileAt(x, y).Type = Tile.TileType.Wall;
                    }
                    else if (x == 0 || y == 0)
                    {
                        GetTileAt(x, y).Type = Tile.TileType.Entrance;
                    }
                    else if (x == width - 1 || y == height - 1)
                    {
                        GetTileAt(x, y).Type = Tile.TileType.Exit;
                    }
                }
                else
                {
                    // tall tile
                    if (UnityEngine.Random.Range(0, 2) == 0 || (x == 0 || x == width - 1) && (y == 0 || y == height - 1))
                    {
                        GetTileAt(x, y).Type = Tile.TileType.Wall;
                    }
                    else
                    {
                        GetTileAt(x, y).Type = Tile.TileType.Barrier;
                    }
                }
            }
        }
    }

    int enemySpawnTimer = 90; // debug
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

    bool IsInBounds(Vector2 position)
    {
        return !(position.x < -10 || position.x > 10 || position.y < -10 || position.y > 10);
    }

    List<Tile.TileType> canPlaceTiles = new List<Tile.TileType>() { Tile.TileType.Floor, Tile.TileType.Raised }; // temporary, will probably be per tower type
    public void Click(int x, int y, Tile.TileDirection direction)
    {
        if(GetTileAt(x,y) != null && canPlaceTiles.Contains(GetTileAt(x, y).Type) && GetTileAt(x,y).presentTower == null)
        {
            TowerSpawn(GetTileAt(x, y), direction);
        }
    }

    public void EnemySpawn(Tile entrance)
    {
        Enemy newEnemy = new Enemy(entrance);
        activeEnemies.Add(newEnemy);
        cbEnemySpawn(newEnemy);
        newEnemy.RegisterDespawnedCB((enemy) => { enemiesToDespawn.Add(enemy); });
    }

    public void TowerSpawn(Tile parentTile, Tile.TileDirection direction)
    {
        Tower newTower = new Tower(parentTile, direction);
        activeTowers.Add(newTower);
        cbTowerSpawn(newTower);
        parentTile.presentTower = newTower;
        newTower.RegisterProjectileReleasedCB((tower, projectile) => { ProjectileSpawn(projectile); });
    }

    void ProjectileSpawn(Projectile newProjectile)
    {
        activeProjectiles.Add(newProjectile);
        newProjectile.RegisterPositionChangedCB(ProjectileCollisionCheck);
        newProjectile.RegisterDespawnedCB((projectile) => { projectilesToDespawn.Add(projectile); });
    }

    void ProjectileCollisionCheck(Projectile projectile)
    {
        bool inBounds = false;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
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

    public Tile GetTileAt(int x, int y){
        if(x < 0 || x > width || y < 0 || y > height)
        {
            Debug.LogError("Tile (" + x + ", " + y + ") is out of range.");
            return null;
        }
        return tiles[x, y];
    }

    public void RegisterEnemySpawnCB(Action<Enemy> cb)
    {
        cbEnemySpawn += cb;
    }

    public void RegisterTowerSpawnCB(Action<Tower> cb)
    {
        cbTowerSpawn += cb;
    }
}

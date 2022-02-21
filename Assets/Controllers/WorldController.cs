using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldController : MonoBehaviour
{
    public GameObject tilePrefab;
    public GameObject enemyPrefab;
    public GameObject towerPrefab;
    public GameObject projectilePrefab;
    public StageData stageData;

    List<EnemyController> enemies = new List<EnemyController>();

    public Camera minimapCamera;
    public GameObject cameraController;
    public Image minimapBorder;
    public RawImage minimapImage;

    public TileController[,] Tiles;
    List<TileController> entrances = new List<TileController>();
    List<TileController> exits = new List<TileController>();
    public List<TileController> activeEntrances = new List<TileController>();

    GameObject previewTower;
    Tile.TileDirection previewDirection = Tile.TileDirection.Left;

    // Start is called before the first frame update
    void Start()
    {

        Tiles = new TileController[stageData.Width, stageData.Height];
        // preview tower (will be elsewhere)
        previewTower = (GameObject)Instantiate(towerPrefab, Vector3.zero, Quaternion.identity);
        previewTower.transform.parent = this.transform;
        previewTower.GetComponent<TowerController>().SetTransparent(true);

        // fill in tile objects
        for (int y = 0; y < stageData.Height; y++)
        {
            for (int x = 0; x < stageData.Width; x++)
            {
                GameObject tileObject = (GameObject)Instantiate(tilePrefab, Vector3.zero, Quaternion.identity);
                TileController tile = tileObject.GetComponent<TileController>();

                tile.x = x;
                tile.y = y;
                tile.World = this;
                tile.transform.position = new Vector3(-x + stageData.Width / 2 - (float)(1 - stageData.Width % 2) / 2, 0, y - stageData.Height / 2 + (float)(1 - stageData.Height % 2) / 2);
                tile.name = "Tile_" + x + "_" + y;
                tile.transform.parent = this.transform;

                tile.RegisterHoveredCB((tileController) => {
                    previewTower.GetComponent<TowerController>().transform.position = tile.transform.position;
                });

                Tiles[x, y] = tile;
            }
        }

        // set up minimap
        RenderTexture minimapRenderTexture;
        float minimapWidth = stageData.Width * 12;
        float minimapHeight = stageData.Height * 12;

        minimapRenderTexture = new RenderTexture((int)minimapWidth * 10, (int)minimapHeight * 10, 0);
        minimapRenderTexture.Create();
        minimapCamera.targetTexture = minimapRenderTexture;
        minimapImage.texture = minimapRenderTexture;

        minimapBorder.rectTransform.sizeDelta = new Vector2(minimapWidth + 5, minimapHeight + 5);
        minimapImage.rectTransform.sizeDelta = new Vector2(minimapWidth, minimapHeight);
        minimapCamera.orthographicSize = Mathf.Min(minimapWidth, minimapHeight) / 24;

        // preload a bunch of prefabs
        SimplePool.Preload(projectilePrefab, 60, this.transform);
        SimplePool.Preload(enemyPrefab, 20, this.transform);
        SimplePool.Preload(towerPrefab, 20, this.transform);


        for (int x = 0; x < stageData.Width; x++)
        {
            for (int y = 0; y < stageData.Height; y++)
            {
                Tiles[x, y].Type = stageData.Layout[y].tiles[x];
                Tiles[x, y].Directions = (Tile.TileDirection.None, Tile.TileDirection.None);

                if (Tiles[x, y].Type == Tile.TileType.Entrance)
                    entrances.Add(Tiles[x, y]);
                if (Tiles[x, y].Type == Tile.TileType.Exit)
                    exits.Add(Tiles[x, y]);

                Dictionary<Tile.TileDirection, TileController> neighbors = new Dictionary<Tile.TileDirection, TileController>();
                if (x != 0)
                    neighbors[Tile.TileDirection.Left] = Tiles[x - 1, y];
                if (x != stageData.Width - 1)
                    neighbors[Tile.TileDirection.Right] = Tiles[x + 1, y];
                if (y != 0)
                    neighbors[Tile.TileDirection.Up] = Tiles[x, y - 1];
                if (y != stageData.Height - 1)
                    neighbors[Tile.TileDirection.Down] = Tiles[x, y + 1];

                Tiles[x, y].neighbors = neighbors;
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
                    Tiles[x, y].Directions = (Tile.TileDirection.None, Tile.TileDirection.None);
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
                if (Tiles[x, y].Type == Tile.TileType.Entrance && Tiles[x, y].Directions.to != Tile.TileDirection.None)
                {
                    activeEntrances.Add(Tiles[x, y]);
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
        TileController entrance = entrances[UnityEngine.Random.Range(0, entrances.Count)];
        HashSet<TileController> unvisited = new HashSet<TileController>();
        HashSet<TileController> visited = new HashSet<TileController>();
        Dictionary<TileController, Tile.TileDirection> fromDirection = new Dictionary<TileController, Tile.TileDirection>();

        foreach (TileController tile in Tiles)
        {
            unvisited.Add(tile);
        }
        unvisited.Remove(entrance);
        visited.Add(entrance);

        Queue<TileController> queue = new Queue<TileController>();
        queue.Enqueue(entrance);
        bool stopSearching = false;

        TileController goalTile = null;
        List<TileController> path = new List<TileController>();
        while (queue.Count > 0 && !stopSearching)
        {
            TileController nextTile = queue.Dequeue();
            List<Tile.TileDirection> keys = new List<Tile.TileDirection>(nextTile.neighbors.Keys);

            foreach (Tile.TileDirection direction in keys)
            {
                TileController tile = nextTile.neighbors[direction];

                if (tile.Type == Tile.TileType.Exit)
                {
                    stopSearching = true;
                    fromDirection[tile] = Tile.InverseDirection[direction];
                    goalTile = tile;
                    path.Add(tile);
                    goalTile.Directions = (Tile.InverseDirection[direction], direction);
                }
                else if (!visited.Contains(tile) && tile.Type == Tile.TileType.Floor && tile.Directions.from == Tile.TileDirection.None && UnityEngine.Random.Range(0, 5) != 0)
                {
                    visited.Add(tile);
                    unvisited.Remove(tile);
                    queue.Enqueue(tile);
                    fromDirection[tile] = Tile.InverseDirection[direction];
                }
            }
        }

        if (goalTile != null)
        {
            stopSearching = false;
            while (!stopSearching)
            {

                TileController nextTile = goalTile.neighbors[fromDirection[goalTile]];
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
    // Update is called once per frame
    void Update()
    {
        enemySpawnTimer += 1;
        if (enemySpawnTimer > 100)
        {
            if (enemies.Count < 1 && activeEntrances.Count != 0)
            {
                EnemySpawn(activeEntrances[UnityEngine.Random.Range(0, activeEntrances.Count)]);
            }
            enemySpawnTimer = 0;
        }
    }

    List<Tile.TileType> canPlaceTiles = new List<Tile.TileType>() { Tile.TileType.Floor, Tile.TileType.Raised }; // temporary, will probably be per tower type
    public void Click(TileController hoveredTile)
    {
        if (GetTileAt(hoveredTile.x, hoveredTile.y) != null && canPlaceTiles.Contains(GetTileAt(hoveredTile.x, hoveredTile.y).Type) && GetTileAt(hoveredTile.x, hoveredTile.y).presentTower == null)
        {
            TowerSpawn(GetTileAt(hoveredTile.x, hoveredTile.y), previewDirection);
        }
    }

    void EnemySpawn(TileController entrance)
    {
        Vector3 enemyPosition = new Vector3(0f, 0f, 0f);
        GameObject enemyObject = SimplePool.Spawn(enemyPrefab, enemyPosition, Quaternion.identity);
        EnemyController enemyController = enemyObject.GetComponent<EnemyController>();

        enemyController.FromTile = entrance;

        enemyObject.transform.parent = this.transform;
        enemyController.RegisterDespawnedCB((enemy) => { SimplePool.Despawn(enemyObject); });

        enemies.Add(enemyController);
    }

    void TowerSpawn(TileController parentTile, Tile.TileDirection facingDirection)
    {
        Vector3 towerPosition = new Vector3(0f, 0f, 0f);
        GameObject towerObject = SimplePool.Spawn(towerPrefab, towerPosition, Quaternion.identity);
        TowerController towerController = towerObject.GetComponent<TowerController>();

        parentTile.presentTower = towerController;
        towerController.ParentTile = parentTile;
        towerController.FacingDirection = facingDirection;
        towerController.ProjectilePrefab = projectilePrefab;

        towerObject.transform.parent = this.transform;
    }

    public void OnTower_RotateRight()
    {
        switch (previewDirection)
        {
            case Tile.TileDirection.Right: previewDirection = Tile.TileDirection.Down; break;
            case Tile.TileDirection.Down:  previewDirection = Tile.TileDirection.Left; break;
            case Tile.TileDirection.Left:  previewDirection = Tile.TileDirection.Up; break;
            case Tile.TileDirection.Up:    previewDirection = Tile.TileDirection.Right; break;
        }
        previewTower.GetComponent<TowerController>().FacingDirection = previewDirection;
    }

    /// <summary>
    /// Checks if a tile is in bound then returns it if it is
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public TileController GetTileAt(int x, int y)
    {
        if (x < 0 || x > stageData.Width || y < 0 || y > stageData.Height)
        {
            Debug.LogError("Tile (" + x + ", " + y + ") is out of range.");
            return null;
        }
        return Tiles[x, y];
    }
}


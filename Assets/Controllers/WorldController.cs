using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldController : MonoBehaviour
{
    public GameObject tilePrefab;
    public StageData stageData;

    public GameObject cameraController;
    public GameObject towerPreviewController;
    public GameObject enemySpawnController;
    public GameObject minimapCameraController;
    public GameObject tilesContainer;

    public TileController[,] Tiles;
    List<TileController> entrances = new List<TileController>();
    List<TileController> exits = new List<TileController>();

    void Start()
    {
        // fill in tile objects
        Tiles = new TileController[stageData.Width, stageData.Height];
        for (int y = 0; y < stageData.Height; y++)
        {
            for (int x = 0; x < stageData.Width; x++)
            {
                // instantiate tile object
                GameObject tileObject = (GameObject)Instantiate(tilePrefab, Vector3.zero, Quaternion.identity);
                TileController tile = tileObject.GetComponent<TileController>();
                Tiles[x, y] = tile;

                // set personal variables
                tile.Parity = (x + y) % 2 == 0;
                tile.name = "Tile_" + x + "_" + y;
                Tiles[x, y].Type = stageData.TileTypes[x,y];

                // position in space
                tile.transform.position = new Vector3(-x + stageData.Width / 2 - (float)(1 - stageData.Width % 2) / 2, 0, y - stageData.Height / 2 + (float)(1 - stageData.Height % 2) / 2);
                tile.transform.parent = tilesContainer.transform;

                // register callbacks
                tile.RegisterHoveredCB(towerPreviewController.GetComponent<TowerPreviewController>().TileHovered);

                // add to relevant lists for easy access
                if (Tiles[x, y].Type == Tile.TileType.Entrance)
                    entrances.Add(Tiles[x, y]);
                if (Tiles[x, y].Type == Tile.TileType.Exit)
                    exits.Add(Tiles[x, y]);
            }
        }

        // assign each tiles neighbors
        // has to be done after filling array as otherwise some neighbors wouldnt exist yet
        for (int x = 0; x < stageData.Width; x++)
        {
            for (int y = 0; y < stageData.Height; y++)
            {
                if (x != 0)
                    Tiles[x, y].Neighbors[Tile.TileDirection.Left] = Tiles[x - 1, y];
                if (x != stageData.Width - 1)
                    Tiles[x, y].Neighbors[Tile.TileDirection.Right] = Tiles[x + 1, y];
                if (y != 0)
                    Tiles[x, y].Neighbors[Tile.TileDirection.Up] = Tiles[x, y - 1];
                if (y != stageData.Height - 1)
                    Tiles[x, y].Neighbors[Tile.TileDirection.Down] = Tiles[x, y + 1];
            }
        }

        // set up minimap
        minimapCameraController.GetComponent<MinimapController>().UpdateStageData(stageData);

        // generate enemy paths
        RandomizePaths();
    }

    /// <summary>
    /// Set up new enemy paths and pass them to the enemy spawner.
    /// </summary>
    void RandomizePaths()
    {
        /// Most of the work is done by the PathGenerator class
        List<TileController> activeEntrances = new List<TileController>();
        PathGenerator.RandomizePaths(Tiles, entrances, exits, activeEntrances);
        enemySpawnController.GetComponent<EnemySpawnController>().ActiveEntrances = activeEntrances;
    }
}
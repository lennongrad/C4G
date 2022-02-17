using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World {
    Tile[,] tiles;
    StageData stageData;

    List<Tile> entrances = new List<Tile>();
    List<Tile> exits = new List<Tile>();

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
                tiles[x, y].Direction = Tile.TileDirection.None;

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

        while(totalSuccesses != 3)
        {
            totalSuccesses = 0;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    tiles[x, y].Direction = Tile.TileDirection.None;
                }
            }

            for (int i = 0; i < 100 && totalSuccesses < 3; i++)
            {
                if (GeneratePath())
                {
                    totalSuccesses += 1;
                }
            }
        }
    }

    private bool GeneratePath()
    { 
        Tile entrance = entrances[Random.Range(0, entrances.Count)];
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
                    fromDirection[tile] = direction;
                    goalTile = tile;
                    path.Add(tile);
                    goalTile.Direction = direction;
                }
                else if(!visited.Contains(tile) && tile.Type == Tile.TileType.Floor && tile.Direction == Tile.TileDirection.None && Random.Range(0,5) != 0)
                {
                    visited.Add(tile);
                    unvisited.Remove(tile);
                    queue.Enqueue(tile);
                    fromDirection[tile] = direction;
                }
            }
        }

        if(goalTile != null)
        {
            stopSearching = false;
            while (!stopSearching)
            {

                Tile nextTile = goalTile.neighbors[Tile.InverseDirection[fromDirection[goalTile]]];
                path.Add(nextTile);
                nextTile.Direction = fromDirection[goalTile];

                if (nextTile.Type == Tile.TileType.Entrance)
                    stopSearching = true;
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
            int rand = Random.Range(i, inputList.Count);
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
                if (Random.Range(0, 2) == 0)
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
                    if (Random.Range(0, 2) == 0 || (x == 0 || x == width - 1) && (y == 0 || y == height - 1))
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

    public Tile GetTileAt(int x, int y){
        if(x < 0 || x > width || y < 0 || y > height)
        {
            Debug.LogError("Tile (" + x + ", " + y + ") is out of range.");
            return null;
        }
        return tiles[x, y];
    }
}

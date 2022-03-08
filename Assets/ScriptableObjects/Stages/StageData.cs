using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable Object that holds the data for each separate stage, notably the tile types and dimensions of the tiles for the stage
/// Only serializable information can be stored here because its a ScriptableObject so beware
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/StageData", order = 1), System.Serializable]
public class StageData : ScriptableObject
{
    /// <summary>
    /// The in-game name of the stage; filename is not stored with StageData object
    /// </summary>
    public string StageTitle;

    public int Width;
    public int Height;

    /// <summary>
    /// Hidden array of tile information using wrapper class for serialization.
    /// Yes, I know it's public and thus not hidden, but it won't serialize otherwise.
    /// </summary>
    public StageLayoutRow[] tileTypeMatrix;

    /// <summary>
    /// Public accessor to tile information multidimensional array
    /// </summary>
    public Tile.TileType[,] TileTypes
    {
        get
        {
            Tile.TileType[,] tiles = new Tile.TileType[100, 100];
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    tiles[x, y] = tileTypeMatrix[y].tileTypes[x];
            return tiles;
        }
    }

    /// <summary>
    /// Transforms a multidimensional array of tile information into a serialization format and stores it
    /// </summary>
    public void SetData(Tile.TileType[,] tileTypes, Vector2Int stageDimensions)
    {
        Width = stageDimensions.x;
        Height = stageDimensions.y;

        tileTypeMatrix = new StageLayoutRow[stageDimensions.y];
        for (int y = 0; y < stageDimensions.y; y++)
        {
            Tile.TileType[] row = new Tile.TileType[stageDimensions.x];
            for (int x = 0; x < stageDimensions.x; x++)
                row[x] = tileTypes[x, y];
            tileTypeMatrix[y] = new StageLayoutRow(row);
        }
    }

    /// <summary>
    /// Since we can't serialize multidimensional arrays, we make a helper class to essentially store an array in a wrapper
    /// </summary>
    [System.Serializable]
    public class StageLayoutRow
    {
        public Tile.TileType[] tileTypes;

        public StageLayoutRow(Tile.TileType[] tileTypes)
        {
            this.tileTypes = tileTypes;
        }
    }
}


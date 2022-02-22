using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// A Unity editor window to make stage data
/// </summary>
public class StageGenerator : EditorWindow
{
    /// <summary>
    /// Path which stages are automatically saved at
    /// </summary>
    private string stagePath = "Assets/Stages/";

    /// <summary>
    /// Stage name used in filename of the stage
    /// </summary>
    private string stageName;
    /// <summary>
    /// The in-game name of the stage; different from stage name which is for backend use
    /// </summary>
    private string stageTitle;
    /// <summary>
    /// The width and height of the stage
    /// </summary>
    private Vector2Int stageDimensions = new Vector2Int(10, 10);
    /// <summary>
    /// The multidimensional array of tile types for the stage
    /// </summary>
    private Tile.TileType[,] tiles = new Tile.TileType[100, 100];

    /// <summary>
    /// The current tile type that the user has selected to draw with
    /// </summary>
    private Tile.TileType paintingType = Tile.TileType.Floor;

    /// <summary>
    /// The color associated with each tile type to be displayed in each button
    /// </summary>
    private Dictionary<Tile.TileType, Color> tileColor = new Dictionary<Tile.TileType, Color>()
    {
        { Tile.TileType.Floor, Color.white },
        { Tile.TileType.Wall, Color.black },
        { Tile.TileType.Raised, new Color(.75f, .75f, .75f) }, // a lighter grey than the barrier
        { Tile.TileType.Barrier, Color.grey },
        { Tile.TileType.Entrance, Color.green },
        { Tile.TileType.Exit, Color.red },
    };

    [MenuItem("Utilities/Stage Generator")]
    public static void ShowWindow()
    {
        EditorWindow window = GetWindow<StageGenerator>("Stage Generator");
        window.minSize = new Vector2(400, 200);
    }

    void CreateGUI()
    {
        if (stageName != "")
            LoadStage();
    }

    void OnGUI()
    {
        // Basic stage data entry fields
        stageName = EditorGUILayout.TextField("Stage Name:", stageName);
        stageTitle = EditorGUILayout.TextField("Stage Title:", stageTitle);
        stageDimensions = EditorGUILayout.Vector2IntField("", stageDimensions);
        EditorGUILayout.Space(5);

        // Limited the stage width and height to 0-100 
        stageDimensions.x = Mathf.Clamp(stageDimensions.x, 0, 100);
        stageDimensions.y = Mathf.Clamp(stageDimensions.y, 0, 100);

        // User selection for which tile type to draw with
        GUI.backgroundColor = tileColor[paintingType];
        paintingType = (Tile.TileType)EditorGUILayout.EnumPopup(paintingType);

        // Create a matrix of buttons, each for one tile. Clicking them changes their tile type to the user's selected
        for(int y = 0; y < stageDimensions.y; y++)
        {
            // have to make a new horizontal display for each line of buttons
            EditorGUILayout.BeginHorizontal();
            for(int x = 0; x < stageDimensions.x; x++)
            {
                // set button colour to current tile type
                GUI.backgroundColor = tileColor[tiles[x, y]];
                if (GUILayout.Button("", GUILayout.MaxWidth(30), GUILayout.Height(Mathf.Min(30, position.width / stageDimensions.x))))
                {
                    //if they already have this tile type, switch it back to floor instead
                    if (tiles[x, y] == paintingType)
                        tiles[x, y] = Tile.TileType.Floor;
                    else
                        tiles[x, y] = paintingType;
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.Space(5);

        // Rest of the buttons are a boring grey
        GUI.backgroundColor = Color.grey;
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Load Stage"))
            LoadStage();
        if (GUILayout.Button("Reset Stage"))
            ResetStage();
        if (GUILayout.Button("Save Stage"))
            SaveStage();
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Load data from any existing stage file using the stage name for filename
    /// </summary>
    void LoadStage()
    {
        // list of assets that have the correct name
        string[] result = AssetDatabase.FindAssets(stageName);

        if(result.Length != 0)
        {
            // just use the first result, shouldnt be multiple generally
            string path = AssetDatabase.GUIDToAssetPath(result[0]);
            StageData loadedData = (StageData)AssetDatabase.LoadAssetAtPath(path, typeof(StageData));

            // use loadedData to get information about stage file
            stageTitle = loadedData.StageTitle;
            stageDimensions.y = loadedData.Height;
            stageDimensions.x = loadedData.Width;
            tiles = loadedData.TileTypes;
        }
    }

    /// <summary>
    /// Reset the values of the stage, mostly tiletypes
    /// </summary>
    void ResetStage()
    {
        // set each tiletype to default (floor for now)
        for (int y = 0; y < stageDimensions.y; y++)
            for (int x = 0; x < stageDimensions.x; x++)
                tiles[x,y] = Tile.TileType.Floor;
    }

    void SaveStage()
    {
        StageData stageData;

        // list of assets that have the stage name
        string[] result = AssetDatabase.FindAssets(stageName);

        // if one already exists with this name
        if (result.Length != 0)
        {
            // just use the first result, shouldnt be multiple generally
            string path = AssetDatabase.GUIDToAssetPath(result[0]);
            stageData = (StageData)AssetDatabase.LoadAssetAtPath(path, typeof(StageData));
        }
        else // no files already have this stage name so make a new asset
        {
            stageData = CreateInstance<StageData>();
        }

        stageData.StageTitle = stageTitle;
        stageData.SetData(tiles, stageDimensions);

        // if we have to create this new asset from scratch rather than saving to existing
        if(result.Length == 0)
        {
            string fileName = stagePath + stageName + ".asset";
            AssetDatabase.CreateAsset(stageData, fileName);
        }

        // have to set it as dirty so that Unity knows that it actually needs to save it
        EditorUtility.SetDirty(stageData);
        AssetDatabase.SaveAssets();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class StageGenerator : EditorWindow
{
    private string stagePath = "Assets/Stages/";
    private string stageName;

    private string stageTitle;
    private Vector2Int stageDimensions = new Vector2Int(10, 10);
    private Tile.TileType[,] tiles = new Tile.TileType[100, 100];

    private Tile.TileType paintingType = Tile.TileType.Floor;

    private Dictionary<Tile.TileType, Color> tileColor = new Dictionary<Tile.TileType, Color>();

    [MenuItem("Utilities/Stage Generator")]
    public static void ShowWindow()
    {
        EditorWindow window = GetWindow<StageGenerator>("Stage Generator");
        window.minSize = new Vector2(400, 200);
    }

    private void OnGUI()
    {
        if(!tileColor.ContainsKey(Tile.TileType.Floor))
        {
            tileColor.Add(Tile.TileType.Floor, Color.white);
            tileColor.Add(Tile.TileType.Wall, Color.black);
            tileColor.Add(Tile.TileType.Raised, new Color(.75f, .75f, .75f));
            tileColor.Add(Tile.TileType.Barrier, Color.grey);
            tileColor.Add(Tile.TileType.Entrance, Color.green);
            tileColor.Add(Tile.TileType.Exit, Color.red);
        }

        EditorGUILayout.BeginVertical();

        stageName = EditorGUILayout.TextField("Stage Name:", stageName);
        stageTitle = EditorGUILayout.TextField("Stage Title:", stageTitle);
        stageDimensions = EditorGUILayout.Vector2IntField("", stageDimensions);
        EditorGUILayout.Space(5);

        stageDimensions.x = Mathf.Clamp(stageDimensions.x, 0, 100);
        stageDimensions.y = Mathf.Clamp(stageDimensions.y, 0, 100);

        GUI.backgroundColor = tileColor[paintingType];
        paintingType = (Tile.TileType)EditorGUILayout.EnumPopup(paintingType);

        GUI.backgroundColor = Color.yellow;
        for(int y = 0; y < stageDimensions.y; y++)
        {
            EditorGUILayout.BeginHorizontal();
            for(int x = 0; x < stageDimensions.x; x++)
            {
                GUI.backgroundColor = tileColor[tiles[x, y]];

                if (GUILayout.Button("", GUILayout.MaxWidth(30), GUILayout.Height(Mathf.Min(30, position.width / stageDimensions.x))))
                {
                    if(tiles[x,y] == paintingType)
                    {
                        tiles[x, y] = Tile.TileType.Floor; ;
                    } 
                    else
                    {
                        tiles[x, y] = paintingType;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.Space(5);

        GUI.backgroundColor = Color.grey;
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Load Stage"))
            LoadStage();
        if (GUILayout.Button("Reset Stage"))
            ResetStage();
        if (GUILayout.Button("Save Stage"))
            SaveStage();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    private void LoadStage()
    {
        string[] result = AssetDatabase.FindAssets(stageName);
        if(result.Length != 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(result[0]);
            StageData loadedData = (StageData)AssetDatabase.LoadAssetAtPath(path, typeof(StageData));
            stageTitle = loadedData.StageTitle;

            if(loadedData.Layout.Length != 0)
            {
                stageDimensions.y = loadedData.Layout.Length;
                stageDimensions.x = loadedData.Layout[0].tiles.Length;
                for (int y = 0; y < stageDimensions.y; y++)
                {
                    for (int x = 0; x < stageDimensions.x; x++)
                    {
                        tiles[x,y] = loadedData.Layout[y].tiles[x];
                    }
                }
            }
        }
    }

    private void ResetStage()
    {
        for (int y = 0; y < stageDimensions.y; y++)
        {
            for (int x = 0; x < stageDimensions.x; x++)
            {
                tiles[x, y] = Tile.TileType.Floor;
            }
        }
    }

    private void SaveStage()
    {
        StageData stageData = CreateInstance<StageData>();

        stageData.StageTitle = stageTitle;

        stageData.Layout = new StageLayout[stageDimensions.y];
        for (int y = 0; y < stageDimensions.y; y++)
        {
            Tile.TileType[] row = new Tile.TileType[stageDimensions.x];
            for (int x = 0; x < stageDimensions.x; x++)
            {
                row[x] = tiles[x, y];
            }
            stageData.Layout[y] = new StageLayout(row);
        }

        stageData.Width = stageData.Layout[0].tiles.Length;
        stageData.Height = stageData.Layout.Length;

        string fileName = stagePath + stageName + ".asset";
        AssetDatabase.CreateAsset(stageData, fileName);
        EditorUtility.SetDirty(stageData);
        AssetDatabase.SaveAssets();
    }
}

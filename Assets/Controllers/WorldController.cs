using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldController : MonoBehaviour
{
    public GameObject tilePrefab;
    public StageData stageData;

    public Camera minimapCamera;
    public GameObject cameraController;
    public Image minimapBorder;
    public RawImage minimapImage;

    List<GameObject> tileObjects;
    World world;

    // Start is called before the first frame update
    void Start()
    {
        world = new World(stageData);
        tileObjects = new List<GameObject>();

        for (int y = 0; y < world.Height; y++)
        {
            for (int x = 0; x < world.Width; x++)
            {
                Tile tileData = world.GetTileAt(x, y);
                Vector3 tilePosition = new Vector3(-x + world.Width / 2 - (float)(1 - world.Width % 2) / 2, 0, y - world.Height / 2 + (float)(1 - world.Height % 2) / 2);
                GameObject tile = (GameObject)Instantiate(tilePrefab, tilePosition, Quaternion.identity);
                TileController tileController = tile.GetComponent<TileController>();
                tileObjects.Add(tile);

                tile.name = "Tile_" + x + "_" + y;
                tile.transform.parent = this.transform;
                tileData.RegisterTypeChangedCB((tile) => { tileController.SetType(tile.Type); });
                tileData.RegisterDirectionChangedCB((tile) => { tileController.SetDirection(tile.Direction); });
                tileController.x = x;
                tileController.y = y;
            }
        }

        world.SetWorld();

        // set up minimap
        RenderTexture minimapRenderTexture;
        float minimapWidth = world.Width * 12;
        float minimapHeight = world.Height * 12;

        minimapRenderTexture = new RenderTexture((int)minimapWidth * 10, (int)minimapHeight * 10, 0);
        minimapRenderTexture.Create();
        minimapCamera.targetTexture = minimapRenderTexture;
        minimapImage.texture = minimapRenderTexture;

        minimapBorder.rectTransform.sizeDelta = new Vector2(minimapWidth + 5, minimapHeight + 5);
        minimapImage.rectTransform.sizeDelta = new Vector2(minimapWidth, minimapHeight);
        minimapCamera.orthographicSize = Mathf.Min(minimapWidth, minimapHeight) / 24;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void RandomizePaths()
    {
        world.RandomizePaths();
    }

    void OnMouseOver()
    {
        Debug.Log("Mouse in     ");
    }
}


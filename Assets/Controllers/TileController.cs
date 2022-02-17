using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    Tile.TileType type;
    Material cubeMaterial;
    Color baseColor;

    public GameObject cube;
    public GameObject arrow;
    bool hovered = false;

    const float wallHeight = 1.522f;
    const float barrierHeight = .75f;
    const float raisedHeight = .2f;
    const float floorHeight = .1f;

    public int x;
    public int y;

    // Start is called before the first frame update
    void Start()
    {
        cubeMaterial = new Material(Shader.Find("Standard"));
        cube.GetComponent<Renderer>().material = cubeMaterial;
    }

    public void SetType(Tile.TileType type)
    {
        this.type = type;
        float height;

        // set height
        switch (type)
        {
            case Tile.TileType.Wall:
                height = wallHeight;
                baseColor = Color.black;
                break;
            case Tile.TileType.Barrier:
                height = barrierHeight;
                baseColor = new Color(.25f, .25f, .25f);
                break;
            case Tile.TileType.Raised:
                height = raisedHeight;
                baseColor = new Color(.7f, .5f, .2f);
                break;
            case Tile.TileType.Entrance:
                height = floorHeight;
                baseColor = Color.green;
                break;
            case Tile.TileType.Exit:
                height = floorHeight;
                baseColor = Color.red;
                break;
            default:
                height = floorHeight;
                baseColor = Color.white;
                break;
        }

        cube.transform.localScale = new Vector3(1f, height, 1f); ;
        cube.transform.localPosition = new Vector3(0, height / 2, 0);
        arrow.transform.localPosition = new Vector3(0, height + .01f, 0);

        if ((x + y) % 2 == 0)
        {
            baseColor *= new Color(.8f, .8f, .8f);
        }
    }

    public void SetDirection(Tile.TileDirection direction)
    {
        if(direction != Tile.TileDirection.None)
        {
            arrow.transform.localEulerAngles = new Vector3(arrow.transform.localEulerAngles.x, (float)direction + 180f, arrow.transform.localEulerAngles.z);
            arrow.SetActive(true);
        }
        else
        {
            arrow.SetActive(false);
        }
    }

    public void Hover()
    {
        hovered = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (hovered)
        {
            cubeMaterial.color = new Color(.3f, .3f, 1);
            cubeMaterial.color *= baseColor;
        }
        else
        {
            cubeMaterial.color = baseColor;
        }
        hovered = false;
    }
}

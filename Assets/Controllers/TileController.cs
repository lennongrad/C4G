using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TileController : MonoBehaviour
{
    Tile.TileType type;
    Material cubeMaterial;
    Color baseColor;

    public GameObject cube;
    public GameObject directionsDisplay;
    public Material cornerMaterial;
    public Material acrossMaterial;
    bool hovered = false;

    const float wallHeight = 1.522f;
    const float barrierHeight = .75f;
    const float raisedHeight = .2f;
    const float floorHeight = .1f;

    public int x;
    public int y;

    Action<TileController> cbHovered;

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
        directionsDisplay.transform.localPosition = new Vector3(0, height + .001f, 0);

        if ((x + y) % 2 == 0)
        {
            baseColor *= new Color(.8f, .8f, .8f);
        }
    }

    public void SetDirections((Tile.TileDirection from, Tile.TileDirection to) directions)
    {
        if(directions.from == Tile.TileDirection.None || directions.to == Tile.TileDirection.None)
        {
            directionsDisplay.SetActive(false);
        }
        else
        {
            float baseRotation = 0f;

            switch (directions.from | directions.to)
            {
                case Tile.TileDirection.Left | Tile.TileDirection.Right:
                    baseRotation = 90f;
                    directionsDisplay.GetComponent<Renderer>().material = acrossMaterial;
                    break;
                case Tile.TileDirection.Up | Tile.TileDirection.Down:
                    baseRotation = 0f;
                    directionsDisplay.GetComponent<Renderer>().material = acrossMaterial;
                    break;
                case Tile.TileDirection.Down | Tile.TileDirection.Left:
                    baseRotation = 0f;
                    directionsDisplay.GetComponent<Renderer>().material = cornerMaterial;
                    break;
                case Tile.TileDirection.Left | Tile.TileDirection.Up:
                    baseRotation = 90f;
                    directionsDisplay.GetComponent<Renderer>().material = cornerMaterial;
                    break;
                case Tile.TileDirection.Up | Tile.TileDirection.Right:
                    baseRotation = 180f;
                    directionsDisplay.GetComponent<Renderer>().material = cornerMaterial;
                    break;
                case Tile.TileDirection.Right | Tile.TileDirection.Down:
                    baseRotation = 270f;
                    directionsDisplay.GetComponent<Renderer>().material = cornerMaterial;
                    break;
            }

            directionsDisplay.transform.localEulerAngles = new Vector3(directionsDisplay.transform.localEulerAngles.x, baseRotation, directionsDisplay.transform.localEulerAngles.z);
            directionsDisplay.SetActive(true);
        }
    }

    public void Hover()
    {
        hovered = true;
        cbHovered(this);
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

    public void RegisterHoveredCB(Action<TileController> cb)
    {
        cbHovered += cb;
    }
}

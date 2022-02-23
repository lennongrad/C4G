using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TileController : MonoBehaviour
{
    Material cubeMaterial;

    const float wallHeight = 1.522f;
    const float barrierHeight = .75f;
    const float raisedHeight = .2f;
    const float floorHeight = .1f;

    /// <summary>
    /// The color of the tile based on its current TileType. Multiplied with other values each frame
    /// </summary>
    Color baseColor;

    bool parity = false;
    /// <summary>
    /// Whether the X + Y of the tile is even or odd. Used to create the checker pattern of tiles
    /// </summary>
    public bool Parity
    {
        set
        {
            parity = value;
        }
    }

    /// <summary>
    /// The main cube of the tile graphic
    /// </summary>
    public GameObject Cube;
    /// <summary>
    /// The flat quad used to display the enemy path direction of the tower
    /// </summary>
    public GameObject DirectionsDisplay;

    /// <summary>
    /// The material for the enemy path display going from one side of the tile to one not straight across
    /// </summary>
    public Material CornerMaterial;
    /// <summary>
    /// The material for the enemy path display for going straight across the tile
    /// </summary>
    public Material AcrossMaterial;

    /// <summary>
    /// Set to true when the tile is hovered over which is then monitored in the next FixedUpdate()
    /// </summary>
    bool hovered = false;

    Action<TileController> cbHovered;
    /// <summary>
    /// Register a method to be called when the tile is hovered over by the user's mouse cursor
    /// </summary>
    public void RegisterHoveredCB(Action<TileController> cb){ cbHovered += cb; }

    Tile.TileType type = Tile.TileType.Floor;
    /// <summary>
    /// The TileType of the tile. Setting this publically automatically updates the tile's graphics accordingly
    /// </summary>
    public Tile.TileType Type
    {
        get { return type; }
        set
        {
            type = value;
            UpdateTypeDisplay(); ;
        }
    }

    (Tile.TileDirection from, Tile.TileDirection to) directions = (Tile.TileDirection.None, Tile.TileDirection.None);
    /// <summary>
    /// The pair of TileDirections in and out of the tile for an enemy path going over the tile. Setting this automatically updates the tile's enemy path display graphics accordingly
    /// </summary>
    public (Tile.TileDirection from, Tile.TileDirection to) Directions
    {
        get { return directions; }
        set
        {
            directions = value;
            UpdateDirectionDisplay();
        }
    }

    /// <summary>
    /// The tiles in each direction from the tile.
    /// </summary>
    public Dictionary<Tile.TileDirection, TileController> Neighbors = new Dictionary<Tile.TileDirection, TileController>();

    /// <summary>
    /// The tower currently placed on this tile. Should only support one at a time probably
    /// </summary>
    public TowerController PresentTower;

    // Start is called before the first frame update
    void Start()
    {
        cubeMaterial = new Material(Shader.Find("Standard"));
        Cube.GetComponent<Renderer>().material = cubeMaterial;
    }

    void FixedUpdate()
    {
        if (hovered)
        {
            // multiply cube base colour by blue if it was hovered recently
            cubeMaterial.color = new Color(.3f, .3f, 1);
            cubeMaterial.color *= baseColor;
        }
        else
        {
            cubeMaterial.color = baseColor;
        }
        hovered = false;
    }

    /// <summary>
    /// Update the visual type of the tile, including the cube height and coloured
    /// </summary>
    void UpdateTypeDisplay()
    {
        float height;

        // set height and base colour of cube according to type
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

        // increase height of cube and adjust its position
        Cube.transform.localScale = new Vector3(1f, height, 1f); ;
        Cube.transform.localPosition = new Vector3(0, height / 2, 0);

        // increase height of visual elements above cube oso they are not inside
        DirectionsDisplay.transform.localPosition = new Vector3(0, height + .001f, 0);

        // darken colour for every other tile to create checker pattern
        if (parity)
        {
            baseColor *= new Color(.8f, .8f, .8f);
        }
    }

    /// <summary>
    /// Update the visual directions display of the tile to show enemy paths
    /// </summary>
    void UpdateDirectionDisplay()
    {
        if (directions.from == Tile.TileDirection.None || directions.to == Tile.TileDirection.None)
        {
            DirectionsDisplay.SetActive(false);
        }
        else
        {
            float baseRotation = 0f;

            // Tile.TileDirection is a bit flag enum, so you can bitwise-OR them like this
            // which allows us to ignore which one is from and which one is to, since it doesn't matter here
            switch (directions.from | directions.to)
            {
                case Tile.TileDirection.Left | Tile.TileDirection.Right:
                    baseRotation = 90f;
                    DirectionsDisplay.GetComponent<Renderer>().material = AcrossMaterial;
                    break;
                case Tile.TileDirection.Up | Tile.TileDirection.Down:
                    baseRotation = 0f;
                    DirectionsDisplay.GetComponent<Renderer>().material = AcrossMaterial;
                    break;
                case Tile.TileDirection.Down | Tile.TileDirection.Left:
                    baseRotation = 0f;
                    DirectionsDisplay.GetComponent<Renderer>().material = CornerMaterial;
                    break;
                case Tile.TileDirection.Left | Tile.TileDirection.Up:
                    baseRotation = 90f;
                    DirectionsDisplay.GetComponent<Renderer>().material = CornerMaterial;
                    break;
                case Tile.TileDirection.Up | Tile.TileDirection.Right:
                    baseRotation = 180f;
                    DirectionsDisplay.GetComponent<Renderer>().material = CornerMaterial;
                    break;
                case Tile.TileDirection.Right | Tile.TileDirection.Down:
                    baseRotation = 270f;
                    DirectionsDisplay.GetComponent<Renderer>().material = CornerMaterial;
                    break;
            }

            DirectionsDisplay.transform.localEulerAngles = new Vector3(DirectionsDisplay.transform.localEulerAngles.x, baseRotation, DirectionsDisplay.transform.localEulerAngles.z);
            DirectionsDisplay.SetActive(true);
        }
    }

    /// <summary>
    /// Called when the tile is hovered over by the user's mouse cursor. Sets hovered to true and calls hovered callback
    /// </summary>
    public void Hover()
    {
        hovered = true;
        cbHovered(this);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using System;

/// <summary>
/// Class that moves the preview tower and manages placing the tower when the user clicks
/// </summary>
public class TowerPreviewController : MonoBehaviour
{
    // debug variables
    List<Tile.TileType> canPlaceTiles = new List<Tile.TileType>() { Tile.TileType.Floor, Tile.TileType.Raised }; // temporary, will probably be per tower type
    public GameObject TowerPrefab;
    public GameObject handDisplayController;

    /// <summary>
    /// A TowerController that is placed at the tile selected by the user as a preview
    /// </summary>
    GameObject previewTower;

    /// <summary>
    /// The tile that the user has hovered over most recently, 
    /// may not necessarily be a valid tile I.E. can be wrong type or already have a tower
    /// </summary>
    TileController tileHovered;
    
    /// <summary>
    /// Time since the last time a tile was hovered over 
    /// </summary>
    int lastHovered = 0;

    /// <summary>
    /// The direction that the user has input for the preview tower to face,
    /// also used when the actual tower is placed downo
    /// </summary>
    Tile.TileDirection previewDirection = Tile.TileDirection.Left;

    /// <summary>
    /// Whether tower selection is enabled; is disabled while hovering other UI elements
    /// </summary>
    bool canPlace = true;

    void Start()
    {
        previewTower = (GameObject)Instantiate(TowerPrefab, Vector3.zero, Quaternion.identity);
        previewTower.transform.parent = this.transform;
        previewTower.GetComponent<TowerController>().PerformBehaviours = false;
        previewTower.GetComponent<TowerController>().FacingDirection = previewDirection;

        // Preload a certain number of each type of tower
        SimplePool.Preload(TowerPrefab, 15, this.transform);

        // set up function to receive info on whether the hand display is active (if so, disable user input on stage)
        handDisplayController.GetComponent<HandDisplayController>().RegisterHoveredChanged(OnHandDisplayHoveredChanged);
    }

    void FixedUpdate()
    {
        // we reset the last tile hovered if the user hasnt hovered recently since that likely means their mouse is off screen
        lastHovered -= 1;
        if(lastHovered == 0)
            tileHovered = null;
    }

    /// <summary>
    /// Creates a new tower object with the parameters as settings and places it in the world
    /// </summary>
    void towerSpawn(TileController parentTile, Tile.TileDirection facingDirection)
    {
        Vector3 towerPosition = parentTile.transform.position;
        GameObject towerObject = SimplePool.Spawn(TowerPrefab, towerPosition, Quaternion.identity);
        TowerController towerController = towerObject.GetComponent<TowerController>();

        parentTile.PresentTower = towerController;
        towerController.ParentTile = parentTile;
        towerController.FacingDirection = facingDirection;
        towerController.PerformBehaviours = true;

        towerObject.transform.parent = this.transform;
    }

    /// <summary>
    /// Called when the user hovers over a tile, sets it as the most recently hovered tile and moves the preview tower
    /// </summary>
    public void TileHovered(TileController tile)
    {
        if (tile.PresentTower == null && canPlaceTiles.Contains(tile.Type))
        {
            previewTower.SetActive(true);
            previewTower.GetComponent<TowerController>().transform.position = tile.transform.position;
            tileHovered = tile;
        }
        else
        {
            previewTower.SetActive(false);
            tileHovered = null;
        }
        lastHovered = 5;
    }

    public void OnTowerRotateCW()
    {
        previewDirection = previewDirection.RotatedCW();
        previewTower.GetComponent<TowerController>().FacingDirection = previewDirection;
    }

    public void OnTowerRotateCCW()
    {
        previewDirection = previewDirection.RotatedCCW();
        previewTower.GetComponent<TowerController>().FacingDirection = previewDirection;
    }

    void OnSelect(InputValue value)
    {
        if (tileHovered != null && canPlaceTiles.Contains(tileHovered.Type) && tileHovered.PresentTower == null && canPlace)
            towerSpawn(tileHovered, previewDirection);
    }

    /// <summary>
    /// Called when the hand display is hovered over or unhovered
    /// </summary>
    /// <param name="isHovered"></param>
    void OnHandDisplayHoveredChanged(bool isHovered)
    {
        canPlace = !isHovered;
    }
}

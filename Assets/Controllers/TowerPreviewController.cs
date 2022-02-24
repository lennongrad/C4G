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
    /// The direction that the user has input for the preview tower to face,
    /// also used when the actual tower is placed downo
    /// </summary>
    Tile.TileDirection previewDirection = Tile.TileDirection.Left;

    //Action<TileController, Tile.TileDirection> cbTowerSpawned;
    /// <summary>
    /// Register a function to be called when a new tower is placed
    /// </summary>
    //public void RegisterTowerSpawnedCB(Action<TileController, Tile.TileDirection> cb) { cbTowerSpawned += cb; }

    void Start()
    {
        previewTower = (GameObject)Instantiate(TowerPrefab, Vector3.zero, Quaternion.identity);
        previewTower.transform.parent = this.transform;
        previewTower.GetComponent<TowerController>().PerformBehaviours = false;
        previewTower.GetComponent<TowerController>().FacingDirection = previewDirection;

        // Preload a certain number of each type of tower
        SimplePool.Preload(TowerPrefab, 15, this.transform);
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
        if (tileHovered != null && canPlaceTiles.Contains(tileHovered.Type) && tileHovered.PresentTower == null)
        {
            towerSpawn(tileHovered, previewDirection);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using System;

public class TowerPreviewController : MonoBehaviour
{
    public GameObject towerPrefab;
    public GameObject projectilePrefab;

    GameObject previewTower;

    TileController tileHovered;
    Tile.TileDirection previewDirection = Tile.TileDirection.Left;

    List<Tile.TileType> canPlaceTiles = new List<Tile.TileType>() { Tile.TileType.Floor, Tile.TileType.Raised }; // temporary, will probably be per tower type

    //Action<TileController, Tile.TileDirection> cbTowerSpawned;
    //public void RegisterTowerSpawnedCB(Action<TileController, Tile.TileDirection> cb) { cbTowerSpawned += cb; }

    void Start()
    {
        previewTower = (GameObject)Instantiate(towerPrefab, Vector3.zero, Quaternion.identity);
        previewTower.transform.parent = this.transform;
        previewTower.GetComponent<TowerController>().SetTransparent(true);
        previewTower.GetComponent<TowerController>().FacingDirection = previewDirection;

        SimplePool.Preload(towerPrefab, 20, this.transform);
        SimplePool.Preload(projectilePrefab, 20, this.transform);
    }

    void placeTower(TileController hoveredTile)
    {
        if (canPlaceTiles.Contains(hoveredTile.Type) && hoveredTile.PresentTower == null)
        {
            towerSpawn(hoveredTile, previewDirection);
        }
    }

    void towerSpawn(TileController parentTile, Tile.TileDirection facingDirection)
    {
        Vector3 towerPosition = parentTile.transform.position;
        GameObject towerObject = SimplePool.Spawn(towerPrefab, towerPosition, Quaternion.identity);
        TowerController towerController = towerObject.GetComponent<TowerController>();

        parentTile.PresentTower = towerController;
        towerController.ParentTile = parentTile;
        towerController.FacingDirection = facingDirection;
        towerController.ProjectilePrefab = projectilePrefab;

        towerObject.transform.parent = this.transform;
    }

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
        if (tileHovered != null)
        {
            placeTower(tileHovered);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldController : MonoBehaviour
{
    public GameObject tilePrefab;
    public GameObject enemyPrefab;
    public GameObject towerPrefab;
    public GameObject projectilePrefab;
    public StageData stageData;

    public Camera minimapCamera;
    public GameObject cameraController;
    public Image minimapBorder;
    public RawImage minimapImage;

    List<GameObject> tileObjects = new List<GameObject>();
    List<GameObject> enemyObjects = new List<GameObject>();
    List<GameObject> towerObjects = new List<GameObject>();
    List<GameObject> projectileObjects = new List<GameObject>();
    World world;

    GameObject previewTower;
    Tile.TileDirection previewDirection = Tile.TileDirection.Left;

    // Start is called before the first frame update
    void Start()
    {
        // preview tower (will be elsewhere)
        previewTower = (GameObject)Instantiate(towerPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
        previewTower.transform.parent = this.transform;
        previewTower.GetComponent<TowerController>().SetTransparent(true);

        // create world
        world = new World(stageData);

        // fill in tile objects
        for (int y = 0; y < world.Height; y++)
        {
            for (int x = 0; x < world.Width; x++)
            {
                Tile tileData = world.GetTileAt(x, y);
                Vector3 tilePosition = new Vector3(tileData.GetPosition().x, 0, tileData.GetPosition().y);
                GameObject tile = (GameObject)Instantiate(tilePrefab, tilePosition, Quaternion.identity);
                TileController tileController = tile.GetComponent<TileController>();
                tileObjects.Add(tile);

                tile.name = "Tile_" + x + "_" + y;
                tile.transform.parent = this.transform;
                tileData.RegisterTypeChangedCB((tile) => { tileController.SetType(tile.Type); });
                tileData.RegisterDirectionsChangedCB((tile) => { tileController.SetDirections(tile.Directions); });
                tileController.x = x;
                tileController.y = y;

                tileController.RegisterHoveredCB((tileController) => { previewTower.GetComponent<TowerController>().SetPosition(tileData.GetPosition()); });
            }
        }

        world.SetWorld();
        world.RegisterEnemySpawnCB(EnemySpawn);
        world.RegisterTowerSpawnCB(TowerSpawn);

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

        // preload a bunch of prefabs
        SimplePool.Preload(projectilePrefab, 60);
        SimplePool.Preload(enemyPrefab, 20);
        SimplePool.Preload(towerPrefab, 20);
    }

    // Update is called once per frame
    void Update()
    {
        world.LogicTick();
    }

    public void Click(TileController hoveredTile)
    {
        world.Click(hoveredTile.x, hoveredTile.y, previewDirection);
    }

    void EnemySpawn(Enemy newEnemy)
    {
        Vector3 enemyPosition = new Vector3(0f, 0f, 0f);
        GameObject enemyObject = SimplePool.Spawn(enemyPrefab, enemyPosition, Quaternion.identity);
        EnemyController enemyController = enemyObject.GetComponent<EnemyController>();
        enemyObjects.Add(enemyObject);

        enemyObject.transform.parent = this.transform;
        newEnemy.RegisterPositionChangedCB((enemy) => { enemyController.SetPosition(enemy.Position); });
    }

    void TowerSpawn(Tower newTower)
    {
        Vector3 towerPosition = new Vector3(0f, 0f, 0f);
        GameObject towerObject = SimplePool.Spawn(towerPrefab, towerPosition, Quaternion.identity);
        TowerController towerController = towerObject.GetComponent<TowerController>();
        towerObjects.Add(towerObject);

        towerObject.transform.parent = this.transform;
        newTower.RegisterPositionChangedCB((tower) => { towerController.SetPosition(tower.Position); });
        newTower.RegisterRotationChangedCB((tower) => { towerController.SetRotation(tower.RotationAngle); });
        newTower.RegisterProjectileReleasedCB((tower, projectile) => { ProjectileSpawn(projectile); });
    }

    void ProjectileSpawn(Projectile newProjectile)
    {
        float yPosition = 1f;
        Vector3 projectPosition = new Vector3(0f, yPosition, 0f);
        //GameObject projectileObject = (GameObject)Instantiate(projectilePrefab, projectPosition, Quaternion.identity);
        GameObject projectileObject = SimplePool.Spawn(projectilePrefab, projectPosition, Quaternion.identity);
        ProjectileController projectileController = projectileObject.GetComponent<ProjectileController>();
        projectileObjects.Add(projectileObject);

        projectileObject.transform.parent = this.transform;
        newProjectile.RegisterPositionChangedCB((projectile) => { projectileController.SetPosition(projectile.Position); });
        newProjectile.RegisterRotationChangedCB((projectile) => { projectileController.SetRotation(projectile.RotationAngle); });
        newProjectile.RegisterDespawnedCB((projectile) => { SimplePool.Despawn(projectileObject) ; });
        projectileController.yPosition = yPosition;
    }

    public void OnTower_RotateRight()
    {
        switch (previewDirection)
        {
            case Tile.TileDirection.Right: previewDirection = Tile.TileDirection.Down; break;
            case Tile.TileDirection.Down:  previewDirection = Tile.TileDirection.Left; break;
            case Tile.TileDirection.Left:  previewDirection = Tile.TileDirection.Up; break;
            case Tile.TileDirection.Up:    previewDirection = Tile.TileDirection.Right; break;
        }
        previewTower.GetComponent<TowerController>().SetRotationByDirection(previewDirection);
    }

    public void RandomizePaths()
    {
        world.RandomizePaths();
    }
}


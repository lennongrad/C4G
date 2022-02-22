using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TowerController : MonoBehaviour
{
    public GameObject ProjectilePrefab;
    public GameObject Cube;

    /// <summary>
    /// The material of the tower's model by default. Must be stored in case it is changed to become transparent or otherwise
    /// </summary>
    Material defaultMaterial;

    /// <summary>
    /// The tile that the tower is sitting on
    /// </summary>
    public TileController ParentTile = null;

    Tile.TileDirection facingDirection = Tile.TileDirection.None;
    /// <summary>
    /// The direction the tower is facing, usually used for the tower's attacks and graphics;
    /// Sstting it publically automatically changed its graphics
    /// </summary>
    public Tile.TileDirection FacingDirection
    {
        get { return facingDirection; }
        set
        {
            facingDirection = value;
            this.RotateToFace(value);
        }
    }

    void Start()
    {
        defaultMaterial = Cube.GetComponent<MeshRenderer>().material;
    }

    int timer = 70;
    void FixedUpdate()
    {
        if(ParentTile != null)
        {
            transform.position = ParentTile.transform.position;

            timer += 1;
            if (timer > 100)
            {
                timer = 0;
                SpawnProjectile();
            }
        }
    }

    /// <summary>
    /// Creates a projectile object and shoots it
    /// </summary>
    void SpawnProjectile()
    {
        float yPosition = 1f;

        Vector3 projectilePosition = new Vector3(0f, yPosition, 0f);
        GameObject projectileObject = SimplePool.Spawn(ProjectilePrefab, projectilePosition, Quaternion.identity);
        ProjectileController projectileController = projectileObject.GetComponent<ProjectileController>();

        projectileController.transform.position = new Vector3(transform.position.x, yPosition, transform.position.z);
        projectileController.FacingDirection = facingDirection;
    }

    /// <summary>
    /// Makes the tower's material slightly transparent. Intended primarily for the placement preview tower
    /// </summary>
    /// <param name="isTransparent"></param>
    public void SetTransparent(bool isTransparent)
    {
        if(!isTransparent)
        {
            Cube.GetComponent<MeshRenderer>().material = defaultMaterial;
        }
        else
        {
            Material material = Cube.GetComponent<MeshRenderer>().material;

            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.DisableKeyword("_ALPHABLEND_ON");
            material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = 3000;

            material.color = new Color(material.color.r, material.color.g, material.color.b, 0.1f);

            Cube.GetComponent<MeshRenderer>().material = material;
        }
    }
}

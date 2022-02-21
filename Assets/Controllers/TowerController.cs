using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TowerController : MonoBehaviour
{
    public GameObject ProjectilePrefab;

    public GameObject cube;
    Material defaultMaterial;

    public TileController ParentTile = null;

    Tile.TileDirection facingDirection = Tile.TileDirection.None;
    public Tile.TileDirection FacingDirection
    {
        get
        {
            return facingDirection;
        }

        set
        {
            facingDirection = value;
            this.RotateToFace(value);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        defaultMaterial = cube.GetComponent<MeshRenderer>().material;
    }

    int timer = 70;
    // Update is called once per frame
    void Update()
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

    void SpawnProjectile()
    {
        float yPosition = 1f;

        Vector3 projectilePosition = new Vector3(0f, yPosition, 0f);
        GameObject projectileObject = SimplePool.Spawn(ProjectilePrefab, projectilePosition, Quaternion.identity);
        ProjectileController projectileController = projectileObject.GetComponent<ProjectileController>();

        projectileController.SetPosition(new Vector2(transform.position.x, transform.position.z));
        projectileController.FacingDirection = facingDirection;
        projectileController.yPosition = yPosition;
    }

    public void SetTransparent(bool isTransparent)
    {
        if(!isTransparent)
        {
            cube.GetComponent<MeshRenderer>().material = defaultMaterial;
        }
        else
        {
            Material material = cube.GetComponent<MeshRenderer>().material;

            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.DisableKeyword("_ALPHABLEND_ON");
            material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = 3000;

            material.color = new Color(material.color.r, material.color.g, material.color.b, 0.1f);

            cube.GetComponent<MeshRenderer>().material = material;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TowerController : MonoBehaviour
{
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

    /// <summary>
    /// The behaviours for the tower to carry out, such as spawning materials, generating mana, etc.
    /// </summary>
    TowerBehaviour[] behaviours;

    bool performBehaviours = false;
    /// <summary>
    /// Whether or not the tower should have its behaviours active. 
    /// Setting false publically also causes the tower to become transparent
    /// </summary>
    public bool PerformBehaviours
    {
        get { return performBehaviours; }
        set
        {
            performBehaviours = value;

            // change towers transparency based on whether its enabled or not
            if (performBehaviours)
                Cube.GetComponent<MeshRenderer>().material = defaultMaterial;
            else
            {
                // disabled so make transparenty
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

    void Start()
    {
        defaultMaterial = Cube.GetComponent<MeshRenderer>().material;
        behaviours = GetComponents<TowerBehaviour>();
    }

    void FixedUpdate()
    {
        if(ParentTile != null)
            transform.position = ParentTile.transform.position;
    }
}

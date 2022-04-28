using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class TowerController : MonoBehaviour
{
    public HPBarController hpBar;

    /// <summary>
    /// The name of the prefab asset for this tower
    /// </summary>
    public string Name;

    /// <summary>
    /// The card data of the card that created this tower.
    /// </summary>
    public CardData CardParent;

    /// The amount of HP that the tower begins with by default
    public float baseHP = 10;
    float hp;

    /// <summary>
    /// Association of each status effect with its current duration
    /// </summary>
    Dictionary<Card.Status, float> statusDuration = new Dictionary<Card.Status, float>();

    /// <summary>
    /// The tile that the tower is sitting on
    /// </summary>
    public TileController ParentTile = null;

    Tile.TileDirection facingDirection = Tile.TileDirection.None;
    /// <summary>
    /// The direction the tower is facing, usually used for the tower's attacks and graphics;
    /// Setting it publically automatically changed its graphics
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
<<<<<<< HEAD

            // change towers transparency based on whether its enabled or not
            if (performBehaviours)
                Cube.GetComponent<MeshRenderer>().sharedMaterial = defaultMaterial;
            else
            {
                // disabled so make transparenty
                Material material = Cube.GetComponent<MeshRenderer>().sharedMaterial;

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
=======
            GetComponent<Collider>().enabled = performBehaviours;
>>>>>>> main
        }
    }

    /// <summary>
    /// Set to true when the tower is hovered over which is then monitored in the next FixedUpdate()
    /// </summary>
    bool hovered = false;

    Action<TowerController> cbHovered;
    /// <summary>
    /// Register a method to be called when the tower is hovered over by the user's mouse cursor
    /// </summary>
    public void RegisterHoveredCB(Action<TowerController> cb) { cbHovered -= cb; cbHovered += cb; }

    Action<TowerController> cbDespawned;
    /// <summary>
    /// Register a function to be called when this tower is set to despawn from being destroyed
    /// </summary>
    public void RegisterDespawnedCB(Action<TowerController> cb) { cbDespawned -= cb; cbDespawned += cb; }
    /// <summary>
    /// Unregister a function to be called when this tower is set to despawn from being destroyed
    /// </summary>
    public void UnregisterDespawnedCB(Action<TowerController> cb) { cbDespawned -= cb; }

    void OnEnable()
    {
<<<<<<< HEAD
        defaultMaterial = Cube.GetComponent<MeshRenderer>().sharedMaterial;
=======
>>>>>>> main
        behaviours = GetComponents<TowerBehaviour>();
        hp = baseHP;
        hpBar.Maximum = baseHP;
    }

    public void Initiate()
    {
        foreach (TowerBehaviour behaviour in behaviours)
        {
            behaviour.OnInitiate();
        }
    }

    void FixedUpdate()
    {
        hpBar.transform.localPosition = (new Vector3(-.25f, 0, .25f)).Rotated(facingDirection);
        //switch (FacingDirection)
        //{
        //    case Tile.TileDirection.Up: hpBar.localPosition = new Vector3(); break;
        //}

        if (ParentTile != null)
        {
            transform.position = ParentTile.transform.position + new Vector3(0, ParentTile.Height, 0);

            if (hovered)
                transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            else
                transform.localScale = new Vector3(1, 1, 1);
        }
        hovered = false;

        Card.Status[] activeStatuses = statusDuration.Keys.ToArray();
        foreach (Card.Status status in activeStatuses)
        {
            Debug.Log(status);

            statusDuration[status] -= Time.deltaTime;
            if (statusDuration[status] < 0f)
                statusDuration.Remove(status);
        }

        if (hp <= 0f)
        {
            transform.position = new Vector3(10000, 10000, 10000);
            cbDespawned(this);
        }
        else
        {
            hpBar.Amount = hp;
            hpBar.gameObject.SetActive(hp < baseHP);
        }
    }

    /// <summary>
    /// Called when the tower is hovered over by the user's mouse cursor. Sets hovered to true and calls hovered callback
    /// </summary>
    public void Hover()
    {
        hovered = true;
        if (cbHovered != null)
            cbHovered(this);
    }

<<<<<<< HEAD
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
=======
    /// <summary>
    /// Called when tower takes damage from usually a projectile
    /// </summary>
    /// <param name="damage"></param>
    public void DirectDamage(float damage)
    {
        hp -= damage;
    }

    /// <summary>
    /// Call when the tower collides with a projectile.
    /// </summary>
    void projectileDamage(ProjectileController projectile)
    {
        DirectDamage(projectile.GetDamage(this));
        projectile.Hit();
    }

    /// <summary>
    /// Add a new status to the list or continue its duration if its already present
    /// </summary>
    public void AddStatus(Card.Status status, float duration)
    {
        // if doesnt already have status or existing status has shorter duration
        if (!statusDuration.ContainsKey(status) || statusDuration[status] < duration)
            statusDuration[status] = duration;
    }

    public void OnTriggerEnter(Collider trigger)
    {
        ProjectileController projectileColliding = trigger.transform.parent.GetComponent<ProjectileController>();
        if (projectileColliding != null && projectileColliding.isEvil)
            projectileDamage(projectileColliding);
    }
}
>>>>>>> main

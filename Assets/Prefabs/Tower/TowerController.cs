using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
            GetComponent<Collider>().enabled = performBehaviours;
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
    public void RegisterHoveredCB(Action<TowerController> cb) { cbHovered -= cb;  cbHovered += cb; }

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
        behaviours = GetComponents<TowerBehaviour>();
        hp = baseHP;
        hpBar.Maximum = baseHP;
    }

    public void Initiate()
    {
        foreach(TowerBehaviour behaviour in behaviours)
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

        if(ParentTile != null)
        {
            transform.position = ParentTile.transform.position + new Vector3(0, ParentTile.Height, 0);

            if (hovered)
                transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            else
                transform.localScale = new Vector3(1, 1, 1);
        }
        hovered = false;

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
        if(cbHovered != null)
            cbHovered(this);
    }

    public void DirectDamage(float damage)
    {
        hp -= damage;
    }
}

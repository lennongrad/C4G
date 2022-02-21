using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    Tile.TileDirection facingDirection;
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
    void OnEnable()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    void FixedUpdate()
    {
        if(GetComponent<Rigidbody>().velocity.magnitude < .01)
        {
            GetComponent<Rigidbody>().AddForce(100 * transform.forward);
        }

        TileController underTile;
        if(detectTile(out underTile))
        {
            //underTile.Hover();
        }
        else
        {
            Despawn();
        }
    }

    bool detectTile(out TileController tile)
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, Vector3.down);
        LayerMask mask = LayerMask.GetMask("Tile");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {
            tile = hit.transform.parent.GetComponent<TileController>();
            return true;
        }

        tile = null;
        return false;
    }

    void Despawn()
    {
        SimplePool.Despawn(gameObject);
    }

    public void HitEnemy()
    {
        Despawn();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public float yPosition = 0;

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
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        SetPosition(new Vector2(transform.position.x, transform.position.z) + Vector2.right.Rotated(transform.localEulerAngles.y) * .1f);
    }

    public void SetPosition(Vector2 newPosition)
    {
        transform.position = new Vector3(newPosition.x, yPosition, newPosition.y);
    }
}

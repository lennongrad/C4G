using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public float yPosition = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetPosition(Vector2 newPosition)
    {
        transform.position = new Vector3(newPosition.x, yPosition, newPosition.y);
    }

    public void SetRotationByDirection(Tile.TileDirection facingDirection)
    {
        switch (facingDirection)
        {
            case Tile.TileDirection.Left: SetRotation(0f); break;
            case Tile.TileDirection.Up: SetRotation(90f); break;
            case Tile.TileDirection.Right: SetRotation(180f); break;
            case Tile.TileDirection.Down: SetRotation(270f); break;
        }
    }

    public void SetRotation(float newAngle)
    {
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, newAngle, transform.localEulerAngles.z);
    }
}

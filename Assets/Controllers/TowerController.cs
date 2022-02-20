using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    public GameObject cube;

    Material defaultMaterial;

    // Start is called before the first frame update
    void Start()
    {
        defaultMaterial = cube.GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetPosition(Vector2 newPosition)
    {
        transform.position = new Vector3(newPosition.x, 0, newPosition.y);
    } 

    public void SetRotationByDirection(Tile.TileDirection facingDirection)
    {
        switch(facingDirection)
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SortOptionController : MonoBehaviour
{
    public Card.SortOption sortOption;

    public Sprite activeTexture;
    public Sprite inactiveTexture;

    public void SetCurrentOption(bool isCurrent)
    {
        if (isCurrent)
        {
            GetComponent<Image>().sprite = activeTexture;
        }
        else
        {
            GetComponent<Image>().sprite = inactiveTexture;
        }
    }
}

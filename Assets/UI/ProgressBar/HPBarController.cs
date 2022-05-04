using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBarController : MonoBehaviour
{
    public ProgressBarController bar;
    public Image frontImage;
    public Image backImage;

    /// <summary>
    /// if is a tower, use blue bar instead of red
    /// </summary>
    public bool isTower = true;
    public Sprite blueFrontTexture;
    public Sprite blueBackTexture;

    public float Maximum
    {
        set
        {
            bar.Maximum = value;
        }
    }

    public float Amount
    {
        set
        {
            bar.Amount = value;
        }
    }

    void Start()
    {
        if(isTower)
        {
            frontImage.sprite = blueFrontTexture;
            backImage.sprite = blueBackTexture;
        }
    }
}

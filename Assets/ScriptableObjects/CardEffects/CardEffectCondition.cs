using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class CardEffectCondition
{
    public abstract void InputGUI();
    public abstract bool CheckCondition();
}

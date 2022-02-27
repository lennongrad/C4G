using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CardEffectCondition", order = 1), System.Serializable]
public class CardEffectCondition : ScriptableObject
{
    public string text;
}

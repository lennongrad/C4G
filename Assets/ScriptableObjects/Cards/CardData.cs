using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CardData", order = 1), System.Serializable]
public class CardData : ScriptableObject
{
    public string CardTitle = "";

    public CardEffect cardEffect;
}

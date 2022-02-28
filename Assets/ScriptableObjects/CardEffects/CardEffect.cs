using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[System.Serializable]
public class CardEffect 
{
    public MonoScript conditionScript;
    [SerializeReference] public CardEffectCondition condition;
}

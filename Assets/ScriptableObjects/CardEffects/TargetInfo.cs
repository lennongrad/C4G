using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Structure holding lists of different data type to enable
/// predicates to requst different target types
/// </summary>
public struct TargetInfo
{
    public TargetInfo(
        List<TileController> tiles = null,
        List<TowerController> towers = null,
        List<EnemyController> enemies = null,
        List<CardModel> cards = null)
    {
        this.Tiles = tiles;
        this.Towers = towers;
        this.Enemies = enemies;
        this.Cards = cards;
    }

    public List<TileController> Tiles { get; }
    public List<TowerController> Towers { get; }
    public List<EnemyController> Enemies { get; }
    public List<CardModel> Cards { get; }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Structure holding lists of different data type to enable
/// predicates to requst different target types
/// </summary>
public class TargetInfo
{
    public List<TileController> Tiles { get; } = new List<TileController>();
    public List<TowerController> Towers { get; } = new List<TowerController>();
    public List<EnemyController> Enemies { get; } = new List<EnemyController>();
    public List<CardController> Cards { get; } = new List<CardController>();

    public void Add(TileController tile) { Tiles.Add(tile); }
    public void Add(TowerController tower) { Towers.Add(tower); }
    public void Add(EnemyController enemy) { Enemies.Add(enemy); }
    public void Add(CardController card) { Cards.Add(card); }

    public Tile.TileDirection Direction { get; set; } = Tile.TileDirection.None;
    public bool StoppedBeforeMax = false;

    public int GetTargetCount(Card.TargetType type)
    {
        switch (type)
        {
            case Card.TargetType.Tiles:   return Tiles.Count;
            case Card.TargetType.Towers:  return Towers.Count;
            case Card.TargetType.Enemies: return Enemies.Count;
            case Card.TargetType.Cards:   return Cards.Count;
        }
        return 0;
    }
}

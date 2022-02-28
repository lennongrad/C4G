using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Card
{
    [Flags]
    public enum CardType
    {
        None = 0,
        Tower = 1 << 0,
        Spell = 1 << 1,
        Skill = 1 << 2
    };

    [Flags]
    public enum TowerSubtype
    {
        None = 0,
        Mana = 1 << 0,
        Damage = 1 << 1
    }

    [Flags]
    public enum SpellSubtype
    {
        None = 0
    }

    [Flags]
    public enum SkillSubtype
    {
        None = 0,
        Injury = 1 << 0
    }
}

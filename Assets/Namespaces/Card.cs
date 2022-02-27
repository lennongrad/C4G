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
}

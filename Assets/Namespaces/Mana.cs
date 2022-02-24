using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Mana
{
    [Flags]
    public enum ManaType
    {
        None = 0,
        Clubs = 1 << 0,
        Spades = 1 << 1,
        Hearts = 1 << 2,
        Diamonds = 1 << 3
    };
}

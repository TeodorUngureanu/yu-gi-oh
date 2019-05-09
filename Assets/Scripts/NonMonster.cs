using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonMonster : Card {

    public enum NonMonsterType { Spell, Trap };

    private NonMonsterType type;

    public NonMonster(string vCardName, string vDescription, string vEffectKey, NonMonsterType vType)
        : base(vCardName, vDescription, vEffectKey)
    {
        type = vType;
    }

    public NonMonsterType getType()
    {
        return type;
    }
}

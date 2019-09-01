using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonMonster : Card {

    private int type;

    public NonMonster(string vCardNumber, byte[] vImage, string vCardName, string vDescription, int vEffectKey, int vType)
        : base(vCardNumber, vImage, vCardName, vDescription, vEffectKey, false)
    {
        type = vType;
    }

    public int getType()
    {
        return type;
    }
}

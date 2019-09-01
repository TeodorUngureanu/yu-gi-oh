using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Monster : Card {

    private int attribute;
    private int type;
    private int attackPoints;
    private int defensePoints;
    private int rarity; // Number of stars
    private bool hasEffect;
    private bool isFusion;
    private string position;

    public Monster(string vCardNumber, byte[] vImage, string vCardName, string vDescription, int vEffectKey, int vAttribute, int vType, int vAttackPoints, int vDefensePoints, int vRarity, bool vIsFusion)
        : base(vCardNumber, vImage, vCardName, vDescription, vEffectKey, true)
    {
        attribute = vAttribute;
        type = vType;
        attackPoints = vAttackPoints;
        defensePoints = vDefensePoints;
        rarity = vRarity;
        isFusion = vIsFusion;
    }

    public int getAttribute()
    {
        return attribute;
    }

    public int getType()
    {
        return type;
    }

    public int getAttackPoints()
    {
        return attackPoints;
    }

    public int getDefensePoints()
    {
        return defensePoints;
    }

    public int getRarity()
    {
        return rarity;
    }

    public bool hasAnEffect()
    {
        return hasEffect;
    }

    public bool isFusionCard()
    {
        return isFusion;
    }
    public string getPosition()
    {
        return position;
    }

    public void setPosition(string pos)
    {
        position = pos;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Monster : Card {

    public enum Attribute { Dark, Divine, Earth, Fire, Light, Water, Wind };
    //TODO: change this
    public enum MonsterType { Aqua, Beast, [Description("Beast-Warrior")] BW, Cyberse, Dinosaur,
                [Description("Divine-Beast")] DB, Dragon, Fairy, Fiend, Fish, Insect,
                Machine, Plant, Psychic, Pyro, Reptile, Rock, [Description("Sea Serpent")] SS,
                Spellcaster, Thunder, Warrior, [Description("Winged Beast")] WB, Wyrm, Zombie };

    private Attribute attribute;
    private MonsterType type;
    private int attackPoints;
    private int defensePoints;
    private int rarity; //number of stars
    private bool hasEffect;
    private bool isForbidden;
    private bool isFusion;

    public Monster(string vCardName, string vDescription, string vEffectKey, Attribute vAttribute, MonsterType vType, int vAttackPoints,
            int vDefensePoints, int vRarity, bool vHasEffect, bool vIsForbidden, bool vIsFusion)
        : base(vCardName, vDescription, vEffectKey)
    {
        attribute = vAttribute;
        type = vType;
        attackPoints = vAttackPoints;
        defensePoints = vDefensePoints;
        rarity = vRarity;
        hasEffect = vHasEffect;
        isForbidden = vIsForbidden;
        isFusion = vIsFusion;
    }

    public Attribute getAttribute()
    {
        return attribute;
    }

    public MonsterType getType()
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

    public bool isForbiddenCard()
    {
        return isForbidden;
    }

    public bool isFusionCard()
    {
        return isFusion;
    }
}

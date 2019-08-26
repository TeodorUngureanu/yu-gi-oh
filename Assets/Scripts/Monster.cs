using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Monster : Card {

    public enum Attribute { Dark, Divine, Earth, Fire, Light, Water, Wind };
    public enum MonsterType { Aqua, Beast, [Description("Beast-Warrior")] BW, Cyberse, Dinosaur,
                [Description("Divine-Beast")] DB, Dragon, Fairy, Fiend, Fish, Insect,
                Machine, Plant, Psychic, Pyro, Reptile, Rock, [Description("Sea Serpent")] SS,
                Spellcaster, Thunder, Warrior, [Description("Winged Beast")] WB, Wyrm, Zombie };

    [JsonProperty("attribute")]
    private Attribute attribute;
    [JsonProperty("type")]
    private MonsterType type;
    [JsonProperty("attack")]
    private int attackPoints;
    [JsonProperty("defense")]
    private int defensePoints;
    [JsonProperty("rarity")]
    private int rarity; //number of stars
    [JsonProperty("hasEffect")]
    private bool hasEffect;
    [JsonProperty("isFusion")]
    private bool isFusion;
    [JsonIgnore]
    private string position;

    public Monster(string vCardName, string vDescription, string vEffectKey, Attribute vAttribute, MonsterType vType,
        int vAttackPoints, int vDefensePoints, int vRarity, bool vHasEffect, bool vIsFusion)
        : base(vCardName, vDescription, vEffectKey, true)
    {
        attribute = vAttribute;
        type = vType;
        attackPoints = vAttackPoints;
        defensePoints = vDefensePoints;
        rarity = vRarity;
        hasEffect = vHasEffect;
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

    public bool isFusionCard()
    {
        return isFusion;
    }

    public void setPosition(string pos)
    {
        position = pos;
    }
}

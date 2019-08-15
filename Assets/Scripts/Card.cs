using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card {
    [JsonProperty("name")]
    private string cardName;
    [JsonProperty("description")]
    private string description;
    [JsonProperty("effectKey")]
    private string effectKey;
    [JsonProperty("isMonster")]
    private bool monster;
    [JsonIgnore]
    private int turnPlayed;

    public Card(string vCardName, string vDescription, string vEffectKey, bool vMonster)
    {
        cardName = vCardName;
        description = vDescription;
        effectKey = vEffectKey;
        monster = vMonster;
    }

    public string getCardName()
    {
        return cardName;
    }
    
    public string getDescription()
    {
        return description;
    }

    public string getEffectKey()
    {
        return effectKey;
    }

    public int getTurnPlayed()
    {
        return turnPlayed;
    }

    public void setTurnPlayed(int crtTurn)
    {
        turnPlayed = crtTurn;
    }

    public bool isMonster()
    {
        return monster;
    }

    public void setMonster(bool vMonster)
    {
        monster = vMonster;
    }
}

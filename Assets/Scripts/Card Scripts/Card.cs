using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card {

    private string cardNumber;
    private byte[] image;
    private string cardName;
    private string description;
    private int effectKey;

    private int turnPlayed;
    private bool monster;

    public Card(string vCardNumber, byte[] vImage, string vCardName, string vDescription, int vEffectKey, bool vMonster)
    {
        cardNumber = vCardNumber;
        image = vImage;
        cardName = vCardName;
        description = vDescription;
        effectKey = vEffectKey;
        monster = vMonster;
    }

    public string GetCardNumber()
    {
        return cardNumber;
    }

    public byte[] GetImage()
    {
        return image;
    }

    public string GetCardName()
    {
        return cardName;
    }
    
    public string GetDescription()
    {
        return description;
    }

    public int GetEffectKey()
    {
        return effectKey;
    }

    public int GetTurnPlayed()
    {
        return turnPlayed;
    }

    public void SetTurnPlayed(int crtTurn)
    {
        turnPlayed = crtTurn;
    }

    public bool IsMonster()
    {
        return monster;
    }

    public void SetMonster(bool vMonster)
    {
        monster = vMonster;
    }
}

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

    public string getCardNumber()
    {
        return cardNumber;
    }

    public byte[] getImage()
    {
        return image;
    }

    public string getCardName()
    {
        return cardName;
    }
    
    public string getDescription()
    {
        return description;
    }

    public int getEffectKey()
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

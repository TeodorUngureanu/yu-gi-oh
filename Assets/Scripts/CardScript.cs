using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardScript : InteractibleElementScript {

    public enum Location { HAND, FIELD, DISK, GRAVEYARD };

    private Location location;
    private int cardIndex;
    private bool highlight = false, isMonster;
    public int turnPlayed;

    protected override void Awake()
    {
        base.Awake();
        objRenderer = GetComponent<Renderer>();
    }

    public Location GetLocation()
    {
        return location;
    }

    public void SetData(Location vLocation, int vCardIndex, bool vIsMonster)
    {
        location = vLocation;
        cardIndex = vCardIndex;
        isMonster = vIsMonster;
    }

    public int GetCardIndex()
    {
        return cardIndex;
    }

    public void SetCardIndex(int vCardIndex)
    {
        cardIndex = vCardIndex;
    }

    public void SetHighlight(bool vHighlight)
    {
        highlight = vHighlight;
        if(!highlight)
        {
            unhighlightObject();
        }
    }
    
    public bool IsMonster()
    {
        return isMonster;
    }

    public int GetTurnPlayed()
    {
        return turnPlayed;
    }

    public void SetTurnPlayed(int vTurnPlayed)
    {
        turnPlayed = vTurnPlayed;
    }

    void OnMouseEnter()
    {
        if (highlight)
        {
            highlightObject();
        }
    }

    void OnMouseExit()
    {
        if (highlight)
        {
            unhighlightObject();
        }
    }

    void OnMouseDown()
    {
        if (highlight)
        {
            interactWithElement();
        }
    }

    public override void interactWithElement()
    {
        if(location.Equals(Location.HAND))
        {
            //place it on the disk
            if(isMonster)
            {
                GameManager.Get().PlaceMonsterOnDisk(cardIndex);
            }
            else
            {
                GameManager.Get().PlaceSpellOnDisk(cardIndex);
            }
        }
        if(location.Equals(Location.DISK))
        {
            //prepare attack, give tribute, select for spell/effect usage
            setAttackMode(true);
        }
    }
}

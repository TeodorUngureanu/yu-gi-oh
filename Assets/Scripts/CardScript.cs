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
    public GameObject frontImagePlane;
    private string position = "DEF";

    protected override void Awake()
    {
        base.Awake();
        objRenderer = GetComponent<Renderer>();
    }

    public Location GetLocation()
    {
        return location;
    }

    public void SetData(Location vLocation, int vCardIndex, bool vIsMonster, string cardName)
    {
        location = vLocation;
        cardIndex = vCardIndex;
        isMonster = vIsMonster;

        if(objRenderer == null)
        {
            objRenderer = GetComponent<Renderer>();
        }

        Texture2D texture = Resources.Load<Texture2D>("Images/Card Images/" + cardName);
        frontImagePlane.GetComponent<Renderer>().material.mainTexture = texture;
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
            if (GameManager.Get().IsPlayerDiscarding())
            {
                //send card to Graveyard
            }
            else
            {
                //place it on the disk
                if (isMonster)
                {
                    GameManager.Get().PlaceMonsterOnDisk(cardIndex);
                }
                else
                {
                    GameManager.Get().PlaceSpellOnDisk(cardIndex);
                }
            }
        }
        if(location.Equals(Location.DISK))
        {
            //prepare attack, give tribute, select for spell/effect usage
            setAttackMode(true);

            //testing position switch
            GameManager.Get().SwitchMonsterPosition(cardIndex, (position == "DEF") ? "ATK" : "DEF");
        }
    }
}

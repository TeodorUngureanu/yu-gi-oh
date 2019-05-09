using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardScript : InteractibleElementScript {

    public enum Location { HAND, FIELD, DECK, GRAVEYARD };

    private Location location;
    private int cardIndex;
    private bool highlight = false;

    protected override void Awake()
    {
        base.Awake();
        objRenderer = GetComponent<Renderer>();
    }

    public Location GetLocation()
    {
        return location;
    }

    public void SetLocation(Location vLocation)
    {
        location = vLocation;
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

    public override void interactWithElement()
    {
        
    }
}

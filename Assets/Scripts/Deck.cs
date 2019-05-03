using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : InteractibleElementScript {
    private List<Card> mainDeck;
    private bool isDrawPhase = false;

    void Awake()
    {
        mainDeck = new List<Card>();
        objRenderer = GetComponent<Renderer>();
    }

    public void LoadDeck(string deckKey)
    {
        //To implement

        //temporarily adding some cards - TO BE REMOVED
        Card card = new Card("Card1", "blah blah card1", "NO_EFFECT");
        Card card2 = new Card("Card2", "blah blah card2", "NO_EFFECT");
        Card card3 = new Card("Card3", "blah blah card3", "NO_EFFECT");
        Card card4 = new Card("Card4", "blah blah card4", "EFFECT_9");
        mainDeck.Add(card);
        mainDeck.Add(card2);
        mainDeck.Add(card3);
    }

    public void ShuffleCards()
    {
        int noCards = mainDeck.Count;
        for(int i = 0; i < noCards - 1; i++)
        {
            int randomPosition = UnityEngine.Random.Range(i, noCards);
            Card auxCard = mainDeck[i];
            mainDeck[i] = mainDeck[randomPosition];
            mainDeck[randomPosition] = auxCard;
        }
    }

    public int CardsLeft()
    {
        return mainDeck.Count;
    }

    public Card DrawCard()
    {
        Card firstCard = mainDeck[0];
        mainDeck.RemoveAt(0);
        return firstCard;
    }

    public void setIsDrawPhase(bool drawPhase)
    {
        Debug.Log("Draw Phase: " + drawPhase);
        isDrawPhase = drawPhase;
    }

    void OnMouseEnter()
    {
        if(isDrawPhase)
        {
            highlightObject();
        }
    }

    void OnMouseExit()
    {
        if (isDrawPhase)
        {
            unhighlightObject();
        }
    }

    public override void interactWithElement()
    {
        
    }
}

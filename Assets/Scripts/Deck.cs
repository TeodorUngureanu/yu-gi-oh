using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : InteractibleElementScript {
    private List<Card> mainDeck;
    private bool isDrawPhase = false;

    protected override void Awake()
    {
        base.Awake();
        Debug.Log("Deck object awaken");
        mainDeck = new List<Card>();
        objRenderer = GetComponent<Renderer>();
    }

    public void LoadDeck(string deckKey)
    {
        //To implement

        //temporarily adding some cards - TO BE REMOVED
        Monster card = new Monster("Card1", "blah blah card1", "NO_EFFECT", true, Monster.Attribute.Dark, Monster.MonsterType.Beast, 2000, 500, 5, false, false, false);
        NonMonster card2 = new NonMonster("Card2", "blah blah card2", "NO_EFFECT", false, NonMonster.NonMonsterType.Spell);
        Monster card3 = new Monster("Card3", "blah blah card3", "NO_EFFECT", true, Monster.Attribute.Light, Monster.MonsterType.Beast, 200, 500, 1, false, false, false);
        NonMonster card4 = new NonMonster("Card4", "blah blah card4", "EFFECT_9", false, NonMonster.NonMonsterType.Trap);
        mainDeck.Add(card);
        mainDeck.Add(card2);
        mainDeck.Add(card3);
        mainDeck.Add(card4);

        Debug.Log("Deck loaded. No. cards: " + mainDeck.Count);
    }

    public void ShuffleCards()
    {
        int noCards = mainDeck.Count;
        for (int i = 0; i < noCards - 1; i++)
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
        Debug.Log("Cards remaining in deck: " + mainDeck.Count);
        return firstCard;
    }

    public void setIsDrawPhase(bool drawPhase)
    {
        isDrawPhase = drawPhase;
    }

    void OnMouseEnter()
    {
        if (isDrawPhase)
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

    void OnMouseDown()
    {
        if(isDrawPhase)
        {
            interactWithElement();
        }
    }

    public override void interactWithElement()
    {
        GameManager.Get().DrawCard();
        unhighlightObject();
    }
}

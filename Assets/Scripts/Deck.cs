using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Deck : InteractibleElementScript {
    private List<Card> mainDeck = new List<Card>();
    private bool isDrawPhase = false;

    protected override void Awake()
    {
        base.Awake();
        Debug.Log("Deck object awaken");
        objRenderer = GetComponent<Renderer>();
    }

    public void LoadDeck(int deckKey)
    {
        if (Config.Get()._Deck_Cards.ContainsKey(deckKey)) {
            List<Dictionary<int, Constants.CardInfo>> _auxList = Config.Get()._Deck_Cards[deckKey];

            foreach (Dictionary<int, Constants.CardInfo> _dictionary in _auxList)
            {
                foreach (KeyValuePair<int, Constants.CardInfo> _card in _dictionary)
                {
                    // Debug.Log(string.Format("ID_Deck = {0}, ID_Card = {1}, Card_Type = {2}, Card_Order = {3}", deckKey, _card.Key, _card.Value.Card_Type, _card.Value.Card_Order));

                    // Monster
                    if (_card.Value.Card_Type == 1)
                    {
                        if (Config.Get()._Monster_Cards.ContainsKey(_card.Key))
                        {
                            mainDeck.Add(Config.Get()._Monster_Cards[_card.Key]);
                        }
                    }
                    // Magic
                    else if (_card.Value.Card_Type == 2)
                    {
                        if (Config.Get()._Magic_Cards.ContainsKey(_card.Key))
                        {
                            mainDeck.Add(Config.Get()._Magic_Cards[_card.Key]);
                        }
                    }
                }
            }
        }

        Debug.Log("Deck loaded. No. cards: " + mainDeck.Count);
    }

    public void ShuffleCards()
    {
        int noCards = mainDeck.Count;
        for (int i = 0; i < noCards - 1; i++)
        {
            int randomPosition = UnityEngine.Random.Range(0, noCards);
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

    public void SetIsDrawPhase(bool drawPhase)
    {
        isDrawPhase = drawPhase;
    }

    void OnMouseEnter()
    {
        if (isDrawPhase)
        {
            HighlightObject();
        }
    }

    void OnMouseExit()
    {
        if (isDrawPhase)
        {
            UnhighlightObject();
        }
    }

    void OnMouseDown()
    {
        if(isDrawPhase)
        {
            InteractWithElement();
        }
    }

    public override void InteractWithElement()
    {
        GameManager.Get().DrawCard();
        UnhighlightObject();
    }
}

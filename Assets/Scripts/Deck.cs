using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour {

    private List<Card> mainDeck;

    private void ShuffleCards()
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

    void Start()
    {
        //init deck
        ShuffleCards();
    }
}

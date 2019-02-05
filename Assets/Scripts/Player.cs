using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private Deck myDeck;
    private List<Card> hand;
    //maybe move this outside
    private List<Card> graveyard;

    private long lifePoints = 8000;
}

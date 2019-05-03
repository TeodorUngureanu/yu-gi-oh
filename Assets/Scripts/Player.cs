using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private Deck deckScript;
    private List<Card> hand;
    private int cardsLeftInDeck;
    private Turn turn = new Turn();

    private long lifePoints;

    public GameObject disk;
    public GameObject deck;
    private bool isReadyForDuel, hasDrawnHand;

    void Awake()
    {
        lifePoints = 8000;
        isReadyForDuel = false;
        hasDrawnHand = false;
        deckScript = deck.GetComponent<Deck>();
    }

    // Use this for initialization
    void Start()
    {
        //init deck
        deckScript.LoadDeck("Yugi");
        deckScript.ShuffleCards();
        deck.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //used only at the beginning, maybe it can be moved
        if(Input.GetKeyDown(KeyCode.Space) && !isReadyForDuel)
        {
            isReadyForDuel = true;
            disk.GetComponent<Animation>()["Take 001"].speed = 2.0f;
            disk.GetComponent<Animation>().Play();

            Invoke("ShowDeck", 3.0f);
        }
        if(Input.GetKeyDown(KeyCode.Space) && !hasDrawnHand)
        {
            //Draw first hand
                    }
        //implement OnHover on cards, OnGraveyard
        
        //temporarily skipping enemy's turn - TO BE REMOVED
        if(turn.getCurrentPhase() == Turn.Phase.Hold && hasDrawnHand)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                OnPhaseTrigger();
            }
        }

        if (turn.getCurrentPhase() == Turn.Phase.Draw)
        {
            cardsLeftInDeck = deckScript.CardsLeft();
            if (cardsLeftInDeck > 0)
            {
                //add card in hand (TODO: animation)
                Card nextCard = deckScript.DrawCard();
            }
            else
            {
                OnPhaseTrigger();
            }
        }
        if(turn.isMainPhase())
        {
            //player is able to play cards

            //highlight these cards
            List<Card> canBePlayed = CanBePlayed();
            
        }
        if(turn.getCurrentPhase() == Turn.Phase.Battle)
        {
            //player is only able to attack
        }

    }

    private void ShowDeck()
    {
        Debug.Log("Showing deck...");
        deck.SetActive(true);
        //temporarily, while not using headset
        Cursor.visible = true;
    }

    // To be called from controller (on draw trigger, on battle trigger, after all monsters attack/play card after battle, on end turn)
    private void OnPhaseTrigger()
    {
        if (turn.getCurrentPhase() == Turn.Phase.Draw)
        {
            deckScript.setIsDrawPhase(false);
        }

        turn.goToNextPhase();

        if(turn.getCurrentPhase() == Turn.Phase.Draw)
        {
            deckScript.setIsDrawPhase(true);
        }
    }

    private List<Card> CanBePlayed()
    {
        List<Card> canBePlayed = hand;
        foreach(Card card in hand)
        {
            //if cannot be played, remove it
        }
        return canBePlayed;
    }
}

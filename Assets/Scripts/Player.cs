using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private Deck deckScript;
    private HandScript handScript;
    private List<Card> cardsInHand;
    private int cardsLeftInDeck;
    private Turn turn = new Turn();

    private long lifePoints;

    public GameObject disk, deck, card, hand;
    private bool isReadyForDuel, hasDrawnHand;
    private bool isFirst = true; //only set for testing
    private bool playedMonsterThisTurn = false;

    void Awake()
    {
        lifePoints = 8000;
        isReadyForDuel = false;
        hasDrawnHand = false;
        deckScript = deck.GetComponent<Deck>();
        handScript = hand.GetComponent<HandScript>();
        cardsInHand = new List<Card>();
    }

    // Use this for initialization
    void Start()
    {
        //init deck
        deckScript.LoadDeck("Yugi");
        deckScript.ShuffleCards();
        deck.SetActive(false);
        handScript.SetDefaultCard(card);
    }

    // Update is called once per frame
    void Update()
    {
        

        if (Input.GetKeyDown(KeyCode.Space) && isReadyForDuel && !hasDrawnHand)
        {
            Debug.Log("Drawing first hand");
            //to change this to 6; currently in testing
            if (cardsInHand.Count == 0)
            {
                for (int index = 0; index < 2; index++)
                {
                    DrawCard();
                }
                Invoke("SetHandDrawn", 3.0f);
            }
        }

        //used only at the beginning, maybe it can be moved
        if (Input.GetKeyDown(KeyCode.Space) && !isReadyForDuel)
        {
            InitDuel();
        }

        //implement OnHover on cards, OnGraveyard

        //temporarily skipping enemy's turn - TO BE REMOVED
        if (turn.getCurrentPhase() == Turn.Phase.Hold && hasDrawnHand)
        {
            if (Input.GetKeyDown(KeyCode.Space))
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
        if(turn.getCurrentPhase() == Turn.Phase.End)
        {
            if(cardsInHand.Count > 6)
            {
                //discard cards
            }
            else
            {
                OnPhaseTrigger();
            }
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

        Debug.Log(turn.getCurrentPhase() + " phase");
    }

    private List<Card> CanBePlayed()
    {
        List<Card> canBePlayed = cardsInHand;
        for (int index = 0; index < cardsInHand.Count; index ++)
        {
            Card crtCard = cardsInHand[index];
            //if cannot be played, remove it

            //else
            handScript.HighlightCard(index);
        }
        return canBePlayed;
    }

    //to be called after connection is made with another player (randomly or host first)
    public void SetIsFirst(bool vIsFirst)
    {
        isFirst = vIsFirst;
    }
    
    public void SetIsReadyForDuel(bool vIsReadyForDuel)
    {
        isReadyForDuel = vIsReadyForDuel;
    }

    public void DrawCard()
    {
        Card nextCard = deckScript.DrawCard();
        cardsInHand.Add(nextCard);
        //animation
        handScript.AddCard(cardsInHand.Count - 1);

        if(turn.getCurrentPhase() == Turn.Phase.Draw)
        {
            OnPhaseTrigger();
        }
    }

    private void SetHandDrawn()
    {
        hasDrawnHand = true;
    }

    public void InitDuel()
    {
        isReadyForDuel = true;
        disk.GetComponent<Animation>()["Take 001"].speed = 2.0f;
        disk.GetComponent<Animation>().Play();

        Invoke("ShowDeck", 3.0f);
    }
}

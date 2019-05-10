using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private Deck deckScript;
    private HandScript handScript;
    private DiskScript diskScript;
    private List<Card> cardsInHand, monstersOnDisk, spellsOnDisk;
    private List<Card> monstersOnField, spellsOnField;
    private int cardsLeftInDeck;
    private Turn turn = new Turn();

    private long lifePoints;
    private int turnCount = 0;

    //to change this to 5; currently in testing
    public int initialHandSize = 3;

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
        diskScript = disk.GetComponent<DiskScript>();
        cardsInHand = new List<Card>();
        monstersOnDisk = new List<Card> { null, null, null, null, null };
        spellsOnDisk = new List<Card> { null, null, null, null, null };
        monstersOnField = new List<Card> { null, null, null, null, null };
        spellsOnField = new List<Card> { null, null, null, null, null };
    }

    // Use this for initialization
    void Start()
    {
        //init deck
        deckScript.LoadDeck("Yugi");
        //deckScript.ShuffleCards();
        deck.SetActive(false);
        card.SetActive(false);
        handScript.SetDefaultCard(card);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isReadyForDuel && !hasDrawnHand)
        {
            Debug.Log("Drawing first hand");
            
            if (cardsInHand.Count == 0)
            {
                for (int index = 0; index < initialHandSize; index++)
                {
                    DrawCard();
                }
                hasDrawnHand = true;
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
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ProcessUsableHandCards(false);
                OnPhaseTrigger();
            }
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
        card.SetActive(true);
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
            playedMonsterThisTurn = false;
            turnCount++;
        }

        if(turn.isMainPhase())
        {
            //highlight playable cards
            ProcessUsableHandCards(true);
        }

        if(turn.getCurrentPhase() == Turn.Phase.Battle)
        {

        }

        Debug.Log(turn.getCurrentPhase() + " phase");
    }

    private void ProcessUsableHandCards(bool highlight)
    {
        for (int index = 0; index < cardsInHand.Count; index ++)
        {
            Card crtCard = cardsInHand[index];
            if (highlight)
            {
                //if can be played, add it
                if((crtCard.isMonster() && monstersOnDisk.IndexOf(null) != -1 && !playedMonsterThisTurn) ||
                    (!crtCard.isMonster() && spellsOnDisk.IndexOf(null) != -1))
                {
                    handScript.HighlightCard(index);
                }
            } else
            {
                //the other way
                handScript.UnhighlightCard(index);
            }
        }
    }

    private void ProcessBattleReadyMonsters(bool highlight)
    {
        for (int index = 0; index < monstersOnDisk.Count; index++)
        {
            Card crtCard = monstersOnDisk[index];
            if (highlight)
            {
                //if can be played, add it
                if (crtCard.getTurnPlayed() != turnCount)
                {
                    handScript.HighlightCard(index);
                }
            }
            else
            {
                //the other way
                handScript.UnhighlightCard(index);
            }
        }
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
        handScript.AddCard(cardsInHand.Count - 1, nextCard.isMonster());

        if(turn.getCurrentPhase() == Turn.Phase.Draw)
        {
            OnPhaseTrigger();
        }
    }

    public void SetMonsterOnDisk(int index)
    {
        int diskIndex = monstersOnDisk.IndexOf(null);
        Card card = cardsInHand[index];
        card.setTurnPlayed(turnCount);
        monstersOnDisk[diskIndex] = card;
        monstersOnField[diskIndex] = card;
        cardsInHand.RemoveAt(index);
        handScript.RecalculateIndex(index, true);
        diskScript.SetMonster(diskIndex);
        GameManager.Get().PlaceMonsterOnField(diskIndex);
        playedMonsterThisTurn = true;
    }

    public void SetSpellOnDisk(int index)
    {
        int diskIndex = spellsOnDisk.IndexOf(null);
        spellsOnDisk[diskIndex] = cardsInHand[index];
        spellsOnField[diskIndex] = cardsInHand[index];
        cardsInHand.RemoveAt(index);
        handScript.RecalculateIndex(index, false);
        diskScript.SetSpell(diskIndex);
        GameManager.Get().PlaceSpellOnField(diskIndex);
    }

    public void InitDuel()
    {
        isReadyForDuel = true;
        disk.GetComponent<Animation>()["Take 001"].speed = 2.0f;
        disk.GetComponent<Animation>().Play();

        Invoke("ShowDeck", 3.0f);
    }
}

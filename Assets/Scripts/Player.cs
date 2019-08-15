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
    public int initialHandSize = 5, maxHandSize = 6;

    public GameObject disk, deck, card, hand;
    private bool isReadyForDuel, hasDrawnHand;
    private bool isFirst = true; //only set for testing
    private bool playedMonsterThisTurn = false;
    private bool canActivateCardsDuringOpponentTurn = false;
    private bool canOpponentActivateCards = false;

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
        deckScript.ShuffleCards();
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
            //get value for canOpponentActivateCards from the opponent
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
            if (Input.GetKeyDown(KeyCode.Space))
            {
                OnPhaseTrigger();
            }
        }
        if(turn.getCurrentPhase() == Turn.Phase.End)
        {
            OnPhaseTrigger();
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
            ProcessBattleReadyMonsters(false);
            ProcessUsableHandCards(true);
            ProcessUsableDiskSpells(true);
        }

        if(turn.getCurrentPhase() == Turn.Phase.Battle)
        {
            ProcessBattleReadyMonsters(true);
            ProcessUsableHandCards(false);
            ProcessUsableDiskSpells(false);
        }

        if(turn.getCurrentPhase() == Turn.Phase.End)
        {
            ProcessBattleReadyMonsters(false);
            ProcessUsableHandCards(false);
            ProcessUsableDiskSpells(false);

            if(cardsInHand.Count > maxHandSize)
            {
                //discard cards
                ProcessDiscardableHandCards(true);
            }
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

    private void ProcessDiscardableHandCards(bool highlight)
    {
        for (int index = 0; index < cardsInHand.Count; index++)
        {
            if (highlight)
            {
                handScript.HighlightCard(index);
            }
            else
            {
                handScript.UnhighlightCard(index);
            }
        }
    }

    private void ProcessUsableDiskSpells(bool highlight)
    {
        for (int index = 0; index < cardsInHand.Count; index++)
        {
            Card crtCard = spellsOnDisk[index];
            if (highlight)
            {
                //if can be played, add it
                if (crtCard != null)
                {
                    diskScript.HighlightSpell(index);
                }
            }
            else
            {
                //the other way
                diskScript.HighlightSpell(index);
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
                if (crtCard != null && crtCard.getTurnPlayed() != turnCount)
                {
                    diskScript.HighlightMonster(index);
                }
            }
            else
            {
                //the other way
                diskScript.HighlightMonster(index);
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
        handScript.AddCard(cardsInHand.Count - 1, nextCard.isMonster(), nextCard.getCardName());

        if(turn.getCurrentPhase() == Turn.Phase.Draw)
        {
            OnPhaseTrigger();
        }
    }

    public void SetMonsterOnDisk(int index)
    {
        int diskIndex = monstersOnDisk.IndexOf(null);
        Card card = cardsInHand[index];
        card.setTurnPlayed(turnCount + 1);
        //card.setPosition("DEF");

        monstersOnDisk[diskIndex] = card;
        monstersOnField[diskIndex] = card;
        cardsInHand.RemoveAt(index);
        handScript.RecalculateIndex(index, true);
        diskScript.SetMonster(diskIndex, "DEF", card.getCardName());
        GameManager.Get().PlaceMonsterOnField(diskIndex, card.getCardName());
        playedMonsterThisTurn = true;
    }

    public void SwitchMonsterPosition(int index, string position)
    {
        diskScript.ChangeMonsterPosition(index, position);
    }

    public void SetSpellOnDisk(int index)
    {
        int diskIndex = spellsOnDisk.IndexOf(null);
        spellsOnDisk[diskIndex] = cardsInHand[index];
        spellsOnField[diskIndex] = cardsInHand[index];
        cardsInHand.RemoveAt(index);
        handScript.RecalculateIndex(index, false);
        diskScript.SetSpell(diskIndex, cardsInHand[index].getCardName());
        GameManager.Get().PlaceSpellOnField(diskIndex, cardsInHand[index].getCardName());
    }

    public void InitDuel()
    {
        isReadyForDuel = true;
        disk.GetComponent<Animation>()["Take 001"].speed = 2.0f;
        disk.GetComponent<Animation>().Play();

        Invoke("ShowDeck", 3.0f);
    }

    private void EndTurn()
    {
        //trigger the opponent's next turn
    }

    private void WaitForTheOpponent()
    {
        //send value of "canActivateCardsDuringOpponentTurn" to the opponent
    }

    public void TriggerQuickActivation()
    {

    }
}

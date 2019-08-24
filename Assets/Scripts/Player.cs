using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private Deck deckScript;
    private HandScript handScript;
    private DiskScript diskScript;
    private List<Card> cardsInHand, monstersOnDisk, spellsOnDisk;
    private List<Card> monstersOnField, spellsOnField;
    private Turn turn = new Turn();

    private long lifePoints;
    private int turnCount = 0;
    private readonly int initialHandSize = 5, maxHandSize = 6;
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
        if (isReadyForDuel && !hasDrawnHand && Input.GetKeyDown(KeyCode.Space))
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
        if (!isReadyForDuel && Input.GetKeyDown(KeyCode.Space))
        {
            InitDuel();
        }

        //implement OnHover on cards, OnGraveyard
        
        if (turn.getCurrentPhase() == Turn.Phase.Hold && hasDrawnHand)
        {
            //calculate whatever needs to be calculated, then proceed to draw phase
            RefreshStuffWhenTurnStarts();
            OnPhaseTrigger();
        }

        if (turn.isMainPhase() || turn.getCurrentPhase() == Turn.Phase.Battle)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                OnPhaseTrigger();
            }
        }

        if (turn.getCurrentPhase() == Turn.Phase.End)
        {
            if (cardsInHand.Count <= maxHandSize)
            {
                GameManager.Get().SetPlayerDiscarding(false);
                ProcessDiscardableHandCards(false);
                //after cards are discarded, send a message to the enemy to start their turn

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
        isReadyForDuel = true;
    }

    // To be called from controller (on draw trigger, on battle trigger, after all monsters attack/play card after battle, on end turn)
    private void OnPhaseTrigger()
    {
        if (turn.getCurrentPhase() == Turn.Phase.Draw)
        {
            deckScript.setIsDrawPhase(false);
        }

        turn.goToNextPhase();
        Debug.Log(turn.getCurrentPhase() + " phase");

        if (turn.getCurrentPhase() == Turn.Phase.Draw)
        {
            deckScript.setIsDrawPhase(true);
            playedMonsterThisTurn = false;
            turnCount++;
            //get value for canOpponentActivateCards from the opponent (if so wait)
        }

        HighlightPlayerCards();

        if (turn.getCurrentPhase() == Turn.Phase.End)
        {
            UnhighlightEverything();

            if (cardsInHand.Count > maxHandSize)
            {
                //discard cards
                Debug.Log("Discarding cards..");
                GameManager.Get().SetPlayerDiscarding(true);
                ProcessDiscardableHandCards(true);
            }
        }
    }

    private void HighlightPlayerCards()
    {
        if (turn.isMainPhase())
        {
            ProcessBattleReadyMonsters(false);
            ProcessPosChangeableDiskMonsters(true);
            ProcessUsableHandCards(true);
            ProcessUsableDiskSpells(true);
        }

        if (turn.getCurrentPhase() == Turn.Phase.Battle)
        {
            ProcessBattleReadyMonsters(true);
            ProcessPosChangeableDiskMonsters(false);
            ProcessUsableHandCards(false);
            ProcessUsableDiskSpells(false);
        }
    }

    private void UnhighlightEverything()
    {
        ProcessBattleReadyMonsters(false);
        ProcessPosChangeableDiskMonsters(false);
        ProcessUsableHandCards(false);
        ProcessUsableDiskSpells(false);
    }

    private void RefreshStuffWhenTurnStarts()
    {
        for(int index = 0; index < monstersOnDisk.Count; index++)
        {
            if(monstersOnDisk[index] != null)
            {
                diskScript.RefreshVariablesForIndex(index);
            }
        }
    }

    private void ProcessUsableHandCards(bool highlight)
    {
        for (int index = 0; index < cardsInHand.Count; index++)
        {
            Card crtCard = cardsInHand[index];
            if (highlight)
            {
                //if can be played, add it
                if ((crtCard.isMonster() && monstersOnDisk.IndexOf(null) != -1 && !playedMonsterThisTurn) ||
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
                handScript.GetCardScriptForIndex(index).ChangeText();
            }
            else
            {
                handScript.UnhighlightCard(index);
                handScript.GetCardScriptForIndex(index).ChangeText();
            }
        }
    }

    private void ProcessUsableDiskSpells(bool highlight)
    {
        for (int index = 0; index < spellsOnDisk.Count; index++)
        {
            Card crtCard = spellsOnDisk[index];
            if (highlight)
            {
                //if can be played, highlight it
                if (crtCard != null && diskScript.GetTypeForIndex(index) == "Spell")
                {
                    diskScript.HighlightSpell(index);
                }
            }
            else
            {
                diskScript.UnhighlightSpell(index);
            }
        }
    }

    private void ProcessPosChangeableDiskMonsters(bool highlight)
    {
        for (int index = 0; index < monstersOnDisk.Count; index++)
        {
            Card crtCard = monstersOnDisk[index];
            if (highlight)
            {
                //if the position can be changed, highlight the card
                if (crtCard != null && !diskScript.HasPositionBeenChangedForIndex(index))
                {
                    diskScript.HighlightMonster(index);
                }
            }
            else
            {
                diskScript.UnhighlightMonster(index);
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
                //if can attack, highlight it
                if (crtCard != null && crtCard.getTurnPlayed() != turnCount && diskScript.GetPositionForIndex(index) == "ATK")
                {
                    diskScript.HighlightMonster(index);
                }
            }
            else
            {
                //the other way
                diskScript.UnhighlightMonster(index);
            }
            diskScript.SwitchAttackModeForIndex(index);
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
        if (deckScript.CardsLeft() != 0)
        {
            Card nextCard = deckScript.DrawCard();
            cardsInHand.Add(nextCard);
            //animation
            string cardType = nextCard.isMonster() ? "Monster" : ((NonMonster)nextCard).getType().ToString();
            handScript.AddCard(cardsInHand.Count - 1, cardType, nextCard.getCardName());
        }

        if (turn.getCurrentPhase() == Turn.Phase.Draw)
        {
            OnPhaseTrigger();
        }
    }

    public void SetMonsterOnDisk(int index, string face)
    {
        int diskIndex = monstersOnDisk.IndexOf(null);
        Card card = cardsInHand[index];
        card.setTurnPlayed(turnCount + 1);
        //card.setPosition("DEF");

        monstersOnDisk[diskIndex] = card;
        monstersOnField[diskIndex] = card;
        diskScript.SetMonster(diskIndex, face, card.getCardName());
        GameManager.Get().PlaceMonsterOnField(diskIndex, card.getCardName());
        playedMonsterThisTurn = true;
        RemoveCardFromHand(index, true);
    }

    public void SwitchMonsterPosition(int index, string position)
    {
        diskScript.ChangeMonsterPosition(index, position);
    }

    public void SetSpellOnDisk(int index, string face)
    {
        //activate effect if face is UP
        int diskIndex = spellsOnDisk.IndexOf(null);
        spellsOnDisk[diskIndex] = cardsInHand[index];
        spellsOnField[diskIndex] = cardsInHand[index];
        diskScript.SetSpell(diskIndex, cardsInHand[index].getCardName(), ((NonMonster) cardsInHand[index]).getType().ToString(), face);
        GameManager.Get().PlaceSpellOnField(diskIndex, cardsInHand[index].getCardName());
        RemoveCardFromHand(index, false);
    }

    //to call this if needed
    public void DiscardRandomCard()
    {
        int randomPosition = UnityEngine.Random.Range(0, cardsInHand.Count - 1);
        RemoveCardFromHand(randomPosition, false);
    }

    public void RemoveCardFromHand(int index, bool isMonsterIfRelevant)
    {
        cardsInHand.RemoveAt(index);
        handScript.RecalculateIndex(index, isMonsterIfRelevant);
    }

    public void InitDuel()
    {
        disk.GetComponent<Animation>()["Take 001"].speed = 2.0f;
        disk.GetComponent<Animation>().Play();

        Invoke("ShowDeck", 3.0f);
    }

    public Turn.Phase GetCurrentPhase()
    {
        return turn.getCurrentPhase();
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

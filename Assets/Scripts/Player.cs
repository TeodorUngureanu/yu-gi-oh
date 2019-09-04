using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public GameObject disk, deck, hand;

    private long lifePoints;
    private int turnCount;
    private bool isReadyForDuel, hasDrawnHand;
    private bool canPlayMonster;
    private bool canActivateCardsDuringOpponentTurn;
    private bool canOpponentActivateCards;
    private bool isMyTurn = true; //only set for testing

    private Deck deckScript;
    private HandScript handScript;
    private DiskScript diskScript;
    private List<Card> monstersOnDisk, spellsOnDisk;
    private Turn turn = new Turn();

    void Awake()
    {
        lifePoints = Constants.STARTING_LIFE_POINTS;
        turnCount = 0;
        isReadyForDuel = false;
        hasDrawnHand = false;
        canPlayMonster = true;
        canActivateCardsDuringOpponentTurn = false;
        canOpponentActivateCards = false;

        deckScript = deck.GetComponent<Deck>();
        handScript = hand.GetComponent<HandScript>();
        diskScript = disk.GetComponent<DiskScript>();
        monstersOnDisk = new List<Card> { null, null, null, null, null };
        spellsOnDisk = new List<Card> { null, null, null, null, null };
    }
    
    void Start()
    {
        deckScript.LoadDeck(1); // YUGI
        deckScript.ShuffleCards();
        deck.SetActive(false);
    }

    public void InitDuel()
    {
        disk.GetComponent<Animation>()["Take 001"].speed = 2.0f;
        disk.GetComponent<Animation>().Play();

        Invoke("ShowDeck", 3.0f);
    }

    private void ShowDeck()
    {
        deck.SetActive(true);
        isReadyForDuel = true;

        //temporarily, while not using headset
        Cursor.visible = true;
    }

    void Update()
    {
        if (isReadyForDuel && !hasDrawnHand && Input.GetKeyDown(KeyCode.Space))
        {
            for (int index = 0; index < Constants.INITIAL_HAND_SIZE; index++)
            {
                DrawCard();
            }
            hasDrawnHand = true;
        }

        //used only at the beginning, maybe it can be moved
        if (!isReadyForDuel && Input.GetKeyDown(KeyCode.Space))
        {
            InitDuel();
        }
        
        //if (turn.getCurrentPhase() == Turn.Phase.Hold && hasDrawnHand && isMyTurn)
        if (turn.getCurrentPhase() == Turn.Phase.Hold && hasDrawnHand)
        {
            //apply any needed effects or restrictions, then proceed to draw phase
            canPlayMonster = true;

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
            if (handScript.GetNoOfCards() <= Constants.MAX_HAND_SIZE)
            {
                SetDiscardingProperties(false);
                isMyTurn = false;

                //after cards are discarded, send a message to the enemy to start their turn
                GameManager.Get().SendInformation("End");

                OnPhaseTrigger();
            }
        }
    }
    
    private void OnPhaseTrigger()
    {
        if (turn.getCurrentPhase() == Turn.Phase.Hold)
        {
            RefreshStuffWhenTurnStarts();
        }

        if (turn.getCurrentPhase() == Turn.Phase.Draw)
        {
            deckScript.SetIsDrawPhase(false);
        }

        turn.goToNextPhase();
        Debug.Log(turn.getCurrentPhase() + " phase");

        if (turn.getCurrentPhase() == Turn.Phase.Draw)
        {
            deckScript.SetIsDrawPhase(true);
            turnCount++;

            //get value for canOpponentActivateCards from the opponent (if so wait)
        }

        HighlightPlayerCards();

        if (turn.getCurrentPhase() == Turn.Phase.End)
        {
            UnhighlightEverything();

            if (handScript.GetNoOfCards() > Constants.MAX_HAND_SIZE)
            {
                Debug.Log("Discarding cards..");
                SetDiscardingProperties(true);
            }
        }
        HighlightPlayerCards();
    }

    private void SetDiscardingProperties(bool start)
    {
        GameManager.Get().SetPlayerDiscarding(start);
        ProcessDiscardableHandCards(start);
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
            ProcessPosChangeableDiskMonsters(false);
            ProcessBattleReadyMonsters(true);
            ProcessUsableHandCards(false);
            ProcessUsableDiskSpells(false);
        }
    }

    private void UnhighlightEverything()
    {
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
        for (int index = 0; index < handScript.GetNoOfCards(); index++)
        {
            Card crtCard = handScript.GetCardInfoForIndex(index);
            if (highlight)
            {
                if ((crtCard.IsMonster() && monstersOnDisk.IndexOf(null) != -1 && canPlayMonster))
                {
                    int needsTribute = Utils.NeedsTribute(((Monster)crtCard).getRarity());
                    int noMonstersOnDisk = monstersOnDisk.FindAll(monster => monster != null).Count;
                    if(noMonstersOnDisk >= needsTribute)
                    {
                        handScript.SetCardHighlightable(index);
                    }
                }
                if(!crtCard.IsMonster() && spellsOnDisk.IndexOf(null) != -1)
                {
                    handScript.SetCardHighlightable(index);
                }
            } else
            {
                handScript.SetCardUnhighlightable(index);
            }
        }
    }

    private void RehighlightHandCards()
    {
        ProcessUsableHandCards(false);
        ProcessUsableHandCards(true);
    }

    private void ProcessDiscardableHandCards(bool highlight)
    {
        for (int index = 0; index < handScript.GetNoOfCards(); index++)
        {
            if (highlight)
            {
                handScript.SetCardHighlightable(index);
            }
            else
            {
                handScript.SetCardUnhighlightable(index);
            }
            handScript.ChangeTextForIndex(index);
        }
    }

    private void ProcessUsableDiskSpells(bool highlight)
    {
        for (int index = 0; index < spellsOnDisk.Count; index++)
        {
            Card crtCard = spellsOnDisk[index];
            if (crtCard != null)
            {
                if (highlight)
                {
                    if (diskScript.GetTypeForIndex(index) == Enums.CardType.Spell || crtCard.GetTurnPlayed() != turnCount)
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
    }

    private void ProcessPosChangeableDiskMonsters(bool highlight)
    {
        for (int index = 0; index < monstersOnDisk.Count; index++)
        {
            Card crtCard = monstersOnDisk[index];
            if (crtCard != null)
            {
                if (highlight)
                {
                    if (diskScript.CanChangePositionForIndex(index))
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
    }

    private void ProcessBattleReadyMonsters(bool highlight)
    {
        for (int index = 0; index < monstersOnDisk.Count; index++)
        {
            Card crtCard = monstersOnDisk[index];
            if (crtCard != null)
            {
                diskScript.SwitchAttackModeForIndex(index, highlight);
                if (highlight)
                {
                    if (crtCard.GetTurnPlayed() != turnCount && diskScript.GetPositionForIndex(index) == Enums.CardPosition.Atk)
                    {
                        diskScript.HighlightMonster(index);
                    }
                }
                else
                {
                    diskScript.UnhighlightMonster(index);
                }
                diskScript.ChangeTextForIndex(index, true);
            }
        }
    }
    
    public void StartMyTurn()
    {
        isMyTurn = true;

    }

    public void SetIsReadyForDuel(bool vIsReadyForDuel)
    {
        isReadyForDuel = vIsReadyForDuel;
    }

    public void DrawCard()
    {
        if (deckScript.CardsLeft() != 0)
        {
            //add animation
            
            handScript.AddCard(deckScript.DrawCard());
        }

        if (turn.getCurrentPhase() == Turn.Phase.Draw)
        {
            OnPhaseTrigger();
        }
    }

    public void SetMonsterOnDisk(int index, Card cardInfo, Enums.CardFace face)
    {
        int diskIndex = monstersOnDisk.IndexOf(null);
        cardInfo.SetTurnPlayed(turnCount);

        monstersOnDisk[diskIndex] = cardInfo;
        diskScript.SetMonster(diskIndex, face, cardInfo.GetCardNumber());
        canPlayMonster = false;
        RemoveCardFromHand(index);

        RehighlightHandCards();
    }

    public void SetSpellOnDisk(int index, Card cardInfo, Enums.CardFace face)
    {
        //activate effect if face is Up
        int diskIndex = spellsOnDisk.IndexOf(null);
        cardInfo.SetTurnPlayed(turnCount);

        spellsOnDisk[diskIndex] = cardInfo;
        diskScript.SetSpell(diskIndex, cardInfo.GetCardNumber(), (Enums.CardType) Enum.Parse(typeof(Enums.CardType), ((NonMonster)cardInfo).getType().ToString()), face);
        RemoveCardFromHand(index);

        RehighlightHandCards();
    }

    //to call this if needed
    public void DiscardRandomCard()
    {
        int randomPosition = UnityEngine.Random.Range(0, handScript.GetNoOfCards() - 1);
        RemoveCardFromHand(randomPosition);
    }

    public void RemoveCardFromHand(int index)
    {
        handScript.RemoveCard(index);
    }
    
    public Turn.Phase GetCurrentPhase()
    {
        return turn.getCurrentPhase();
    }

    private void WaitForTheOpponent()
    {
        //send value of "canActivateCardsDuringOpponentTurn" to the opponent
    }

    public void TriggerQuickActivation()
    {

    }
}

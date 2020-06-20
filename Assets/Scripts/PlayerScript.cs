using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

    public GameObject disk, deck, hand;

    private long lifePoints;
    private int turnCount;
    private bool isReadyForDuel, hasDrawnHand;
    private bool canPlayMonster;
    private bool canOpponentActivateCards;
    private bool isMyTurn = true; //only set for testing
    private bool hasDuelEnded;
    private bool askingForQuickActivation, askingForEffectActivation;
    private bool paused = false;

    private Deck deckScript;
    private HandScript handScript;
    private DiskScript diskScript;
    private List<Card> monstersOnDisk, spellsOnDisk;
    public HashSet<string> quickPlayCards;
    private Turn turn;

    private float cardHeight;

    void Awake()
    {
        lifePoints = Constants.STARTING_LIFE_POINTS;
        turnCount = 0;
        isReadyForDuel = false;
        hasDrawnHand = false;
        canPlayMonster = true;
        canOpponentActivateCards = false;
        hasDuelEnded = false;
        askingForQuickActivation = false;

        deckScript = deck.GetComponent<Deck>();
        handScript = hand.GetComponent<HandScript>();
        diskScript = disk.GetComponent<DiskScript>();
        monstersOnDisk = new List<Card> { null, null, null, null, null };
        spellsOnDisk = new List<Card> { null, null, null, null, null };
        quickPlayCards = new HashSet<string>();
        turn = new Turn();
    }
    
    void Start()
    {
        deckScript.LoadDeck(1); // YUGI
        ShuffleDeck();
        deck.SetActive(false);

        cardHeight = deck.transform.parent.localScale.y / deckScript.CardsLeft();
        GameManager.Get().ChangePhase(turn.getCurrentPhase().ToString());
    }

    public void InitDuel()
    {
        disk.GetComponent<Animation>()["Take 001"].speed = 2.0f;
        disk.GetComponent<Animation>().Play();
        
        StartCoroutine(ShowDeckCoroutine());
    }

    private IEnumerator ShowDeckCoroutine()
    {
        yield return new WaitForSeconds(2);

        deck.SetActive(true);
        isReadyForDuel = true;
        UIManager.Get().UpdateDeckSizeOnDisk(deckScript.CardsLeft());
        UIManager.Get().SetDeckSizeOnInfoPanel(deckScript.CardsLeft().ToString(), false); //TODO: remove this - will be set when we get some info at the beginning of the duel

        //temporarily, while not using headset
        Cursor.visible = true;
    }

    public void ShuffleDeck()
    {
        deckScript.ShuffleCards();
    }

    void Update()
    {
        if(paused || hasDuelEnded)
        {
            return;
        }

        if (askingForEffectActivation)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameManager.Get().StopFlipEffectChoicePhase(true);
                AskForEffectActivation(false);
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameManager.Get().StopFlipEffectChoicePhase(false);
                AskForEffectActivation(false);
            }
            return;
        }
        if(askingForQuickActivation)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                AskForQuickActivation(false);
                GameManager.Get().StartQuickActivation();
            }
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                AskForQuickActivation(false);
                GameManager.Get().StopQuickActivation();
                GameManager.Get().SendQuickActivationEndMessage();
            }
            return;
        }

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

        if (turn.getCurrentPhase() == Turn.Phase.Hold && hasDrawnHand && isMyTurn)
        //if (turn.getCurrentPhase() == Turn.Phase.Hold && hasDrawnHand)
        {
            //TODO: apply any needed effects or restrictions, then proceed to draw phase
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
                GameManager.Get().SendInformation(Constants.END_TURN, 0, new List<MessageParameter>());

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

        if(turn.getCurrentPhase() == Turn.Phase.Battle && GameManager.Get().IsAttacking())
        {
            GameManager.Get().CancelAttack();
        }

        turn.goToNextPhase();
        GameManager.Get().ChangePhase(turn.getCurrentPhase().ToString());

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
                UIManager.Get().SetInfoTextOnInfoPanel(Constants.DISCARD_INFO, false);
                SetDiscardingProperties(true);
            }
        }
        HighlightPlayerCards();
    }

    public void AskForQuickActivation(bool asking)
    {
        askingForQuickActivation = asking;
    }

    public void AskForEffectActivation(bool asking)
    {
        askingForEffectActivation = asking;
    }

    private void SetDiscardingProperties(bool start)
    {
        GameManager.Get().SetPlayerDiscarding(start);
        ProcessDiscardableHandCards(start);
    }

    public void HighlightPlayerCards()
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

    public void UnhighlightEverything()
    {
        ProcessSelectableMonstersOnDisk(Constants.DUMMY_INEXISTENT_ID, Constants.DUMMY_INEXISTENT_ID, 0, false);
        ProcessPosChangeableDiskMonsters(false);
        ProcessTributeAvailableMonsters(false);
        ProcessUsableHandCards(false);
        ProcessUsableDiskSpells(false);
    }

    public void PauseSwitch(bool shouldPause)
    {
        paused = shouldPause;
    }

    private void RefreshStuffWhenTurnStarts()
    {
        for(int index = 0; index < monstersOnDisk.Count; index++)
        {
            diskScript.RefreshVariablesForIndex(index);
        }
    }

    private void ProcessUsableHandCards(bool highlight)
    {
        for (int index = 0; index < handScript.GetNoOfCards(); index++)
        {
            Card crtCard = handScript.GetCardInfoForIndex(index);
            if (highlight)
            {
                if ((crtCard.IsMonster() && canPlayMonster))
                {
                    int needsTribute = Utils.NeedsTribute(((Monster)crtCard).GetRarity());

                    if (needsTribute > 0)
                    {
                        int noMonstersOnDisk = monstersOnDisk.FindAll(monster => monster != null).Count;
                        if (noMonstersOnDisk >= needsTribute)
                        {
                            handScript.SetCardHighlightable(index);
                        }
                    }
                    else
                    {
                        if(monstersOnDisk.IndexOf(null) != -1)
                        {
                            handScript.SetCardHighlightable(index);
                        }
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
                //TODO: if it's a trap that needs trigger, don't highlight
                if (highlight)
                {
                    if ((diskScript.GetTypeForIndex(index) == Enums.CardType.Spell || crtCard.GetTurnPlayed() != turnCount)
                            && !diskScript.IsSpellActivated(index))
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
                bool canAttackThisTurn = diskScript.SwitchAttackModeForIndex(index, highlight);
                if (canAttackThisTurn && highlight)
                {
                    if (diskScript.GetPositionForIndex(index) == Enums.CardPosition.Atk)
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

    public void ProcessTributeAvailableMonsters(bool highlight)
    {
        for (int index = 0; index < monstersOnDisk.Count; index++)
        {
            if (monstersOnDisk[index] != null)
            {
                if (highlight)
                {
                    diskScript.HighlightMonster(index);
                }
                else
                {
                    diskScript.UnhighlightMonster(index);
                }
                diskScript.ChangeTextForIndex(index, true);
            }
        }
    }

    public void ProcessQuickActivationCards(bool highlight)
    {
        foreach (string key in quickPlayCards)
        {
            int index = Int32.Parse(key.Substring(key.IndexOf('_') + 1));
            if(key.Contains("MONSTER"))
            {
                if (highlight)
                {
                    diskScript.HighlightMonster(index);
                }
                else
                {
                    diskScript.UnhighlightMonster(index);
                }
            }
            else
            {
                if (highlight)
                {
                    diskScript.HighlightSpell(index);
                }
                else
                {
                    diskScript.UnhighlightSpell(index);
                }
            }
            diskScript.ChangeTextForIndex(index, true);
        }
    }

    public void ProcessSelectableMonstersOnDisk(int attribute, int type, int superiorAtkLimit, bool highlight)
    {
        for (int index = 0; index < monstersOnDisk.Count; index++)
        {
            if (monstersOnDisk[index] != null)
            {
                bool followsConstraints = true;
                Monster monsterInfo = (Monster) monstersOnDisk[index];
                if((attribute != Constants.DUMMY_INEXISTENT_ID && monsterInfo.GetAttribute() != attribute)
                    || (type != Constants.DUMMY_INEXISTENT_ID && monsterInfo.GetMonsterType() != type)
                    || (superiorAtkLimit > 0 && monsterInfo.GetAttackPoints() > superiorAtkLimit))
                {
                    followsConstraints = false;
                }

                diskScript.SwitchSelectionModeForIndex(index, true, highlight);
                if (highlight && followsConstraints)
                {
                    diskScript.HighlightMonster(index);
                }
                else
                {
                    diskScript.ChangeTextForIndex(index, true);
                    diskScript.UnhighlightMonster(index);
                }
            }
        }
    }

    public void ProcessSelectableSpellsOnDisk(int type, bool highlight)
    {
        for (int index = 0; index < spellsOnDisk.Count; index++)
        {
            if (spellsOnDisk[index] != null)
            {
                NonMonster spellInfo = (NonMonster) spellsOnDisk[index];
                bool followsConstraints = spellInfo.GetSpellType() == type;

                diskScript.SwitchSelectionModeForIndex(index, false, highlight);
                if (highlight && followsConstraints)
                {
                    diskScript.HighlightSpell(index);
                }
                else
                {
                    diskScript.ChangeTextForIndex(index, false);
                    diskScript.UnhighlightSpell(index);
                }
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
        //TODO: add animation
        handScript.AddCard(deckScript.DrawCard());
        UIManager.Get().SetHandSizeOnInfoPanel(handScript.GetNoOfCards().ToString(), false);

        int cardsLeftInDeck = deckScript.CardsLeft();
        UIManager.Get().UpdateDeckSizeOnDisk(cardsLeftInDeck);
        UIManager.Get().SetDeckSizeOnInfoPanel(cardsLeftInDeck.ToString(), false);

        GameManager.Get().SendInformation(Constants.DRAW, 0, new List<MessageParameter>());

        if (deckScript.CardsLeft() != 0)
        {
            ComputeDeckHeight(-1);

            if (turn.getCurrentPhase() == Turn.Phase.Draw)
            {
                OnPhaseTrigger();
            }
        }
        else
        {
            deck.SetActive(false);
            hasDuelEnded = true;
            UIManager.Get().ShowDuelEnd(true);
            UnhighlightEverything();
        }
    }

    public int SetMonsterOnDisk(int handIndex, Monster cardInfo, Enums.CardFace face)
    {
        int diskIndex = monstersOnDisk.IndexOf(null);
        cardInfo.SetTurnPlayed(turnCount);

        monstersOnDisk[diskIndex] = cardInfo;
        diskScript.SetMonster(diskIndex, face, cardInfo);
        canPlayMonster = false;
        RemoveCardFromHand(handIndex);

        if(face == Enums.CardFace.Down && cardInfo.IsFlippable())
        {
            quickPlayCards.Add("MONSTER_" + diskIndex);
        }

        RehighlightHandCards();

        return diskIndex;
    }

    public int SetSpellOnDisk(int handIndex, NonMonster cardInfo, Enums.CardFace face)
    {
        int diskIndex = spellsOnDisk.IndexOf(null);
        cardInfo.SetTurnPlayed(turnCount);

        spellsOnDisk[diskIndex] = cardInfo;
        Enums.CardType cardType = (Enums.CardType) Enum.Parse(typeof(Enums.CardType), cardInfo.GetSpellType().ToString());
        diskScript.SetSpell(diskIndex, cardInfo, cardType, face);
        RemoveCardFromHand(handIndex);

        if (face == Enums.CardFace.Down && (cardInfo.IsQuickPlaySpell() || cardType == Enums.CardType.Trap))
        {
            quickPlayCards.Add("SPELL_" + diskIndex);
        }

        RehighlightHandCards();

        return diskIndex;
    }

    public bool FlipMonster(int index)
    {
        bool hasFlipEffect;
        if (hasFlipEffect = ((Monster)monstersOnDisk[index]).IsFlippable())
        {
            RemoveQuickPlayCard("MONSTER_", index);
        }

        return hasFlipEffect;
    }

    public void DestroyMonsters(List<int> diskIndices)
    {
        for(int index = 0; index < diskIndices.Count; index++)
        {
            RemoveQuickPlayCard("MONSTER_", index);
            diskScript.DestroyMonster(diskIndices[index]);
            monstersOnDisk[diskIndices[index]] = null;
        }
    }

    public void DestroySpells(List<int> diskIndices)
    {
        for(int index = 0; index < diskIndices.Count; index++)
        {
            RemoveQuickPlayCard("SPELL_", index);
            diskScript.DestroySpell(diskIndices[index]);
            spellsOnDisk[diskIndices[index]] = null;
        }
    }

    public bool CanQuickPlayCards()
    {
        return quickPlayCards.Count > 0;
    }

    public void RemoveQuickPlayCard(string prefix, int diskIndex)
    {
        quickPlayCards.RemoveWhere(card => card.Equals(String.Concat(prefix, diskIndex.ToString())));
        if(quickPlayCards.Count == 0 && GameManager.Get().IsQuickActivation())
        {
            //TODO: get out of quick activation phase if during it
            GameManager.Get().SetQuickActivation(false);
        }
    }
    
    //TODO: delete this if it won't be used
    public void DiscardRandomCard()
    {
        int randomPosition = UnityEngine.Random.Range(0, handScript.GetNoOfCards() - 1);
        RemoveCardFromHand(randomPosition);
    }

    public void RemoveCardFromHand(int index)
    {
        handScript.RemoveCard(index);
        UIManager.Get().SetHandSizeOnInfoPanel(handScript.GetNoOfCards().ToString(), false);
    }

    public Card GetCardInfoForIndex(int index, bool isMonster)
    {
        if(isMonster)
        {
            return monstersOnDisk[index];
        }
        else
        {
            return spellsOnDisk[index];
        }
    }
    
    public Turn.Phase GetCurrentPhase()
    {
        return turn.getCurrentPhase();
    }

    private void WaitForTheOpponent()
    {
        //TO DO: send value of "quickPlayCardsCounter > 0" to the opponent
    }

    public void ComputeDeckHeight(int operationCoefficient)
    {
        deck.transform.parent.localScale += new Vector3(0, operationCoefficient * cardHeight, 0);
    }

    public void ApplyRestrictionsForAttackingMonster(int index)
    {
        diskScript.ApplyRestrictionsForAttackingMonster(index);
    }

    public void ShowEnemySelection(List<int> indices, bool isMonster, bool selectedByEnemy)
    {
        foreach (int index in indices)
        {
            diskScript.SwitchEnemySelectionForIndex(index, isMonster, selectedByEnemy);
        }
    }

    public void DeselectDiskCards()
    {
        diskScript.DeselectAllDiskCards();
    }

    public long ModifyLifePoints(int points)
    {
        lifePoints += points;
        UIManager.Get().UpdateLPOnDisk(lifePoints);
        return lifePoints;
    }
}

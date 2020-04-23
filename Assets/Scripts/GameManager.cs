using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public GameObject player, field;
    public GameObject duelInfoCanvas;

    private static GameManager instance;
    private PlayerScript playerScript;
    private Graveyard playerGraveyard, enemyGraveyard;
    private FieldScript fieldScript;
    private InfoScreenScript infoScreenScript;

    private int enemyLifePoints = Constants.STARTING_LIFE_POINTS;
    private int enemyHand = Constants.INITIAL_HAND_SIZE;
    private Tribute tribute;
    private int attackingMonsterIndex = 100;
    private bool playerDiscarding = false, sacrificing = false, attacking = false, quickActivation = false;

    public static GameManager Get()
    {
        return instance;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            playerScript = player.GetComponent<PlayerScript>();
            fieldScript = field.GetComponent<FieldScript>();
            infoScreenScript = duelInfoCanvas.GetComponent<InfoScreenScript>();
        }
        else
        {
            Destroy(gameObject);
        }

        Config.Get().Load();
    }

    public Graveyard GetGraveyard(string key)
    {
        if (key.Equals("ENEMY"))
        {
            return enemyGraveyard;
        }
        return playerGraveyard;
    }

    public int GetFieldEffectValue(string monsterType)
    {
        return field.GetComponent<FieldScript>().GetEffectValueForType(monsterType);
    }

    public bool IsPlayerDiscarding()
    {
        return playerDiscarding;
    }

    public void SetPlayerDiscarding(bool value)
    {
        playerDiscarding = value;
    }

    public bool IsPlayerSacrificing()
    {
        return sacrificing;
    }

    private void SetPlayerSacrificing(bool value)
    {
        sacrificing = value;
    }

    public bool IsAttacking()
    {
        return attacking;
    }

    public void SetAttacking(bool value)
    {
        attacking = value;
    }

    public bool IsQuickActivation()
    {
        return quickActivation;
    }

    public void SetQuickActivation(bool value)
    {
        quickActivation = value;
    }

    public void DrawCard()
    {
        playerScript.DrawCard();
    }

    public void SummonMonster(int index, Card cardInfo, Enums.CardFace face)
    {
        int rarity = ((Monster)cardInfo).GetRarity();
        if (rarity < 5)
        {
            int diskIndex = playerScript.SetMonsterOnDisk(index, cardInfo, face, new List<int>());
            fieldScript.SetMonster(diskIndex, cardInfo, face);
        }
        else
        {
            PauseCurrentPhase();
            SetPlayerSacrificing(true);
            playerScript.ProcessTributeAvailableMonsters(true);
            tribute = new Tribute(index, cardInfo, face, Utils.NeedsTribute(rarity));
        }
    }

    public void AddTribute(bool isEnemy, int index)
    {
        fieldScript.AddTributeCircle(isEnemy, index);
        tribute.AddTribute(index);
        if (tribute.HasEnoughTributes())
        {
            PauseCurrentPhase();
            //TODO: tribute animation
            StartCoroutine(TributeSummoningCoroutine());
        }
    }

    private IEnumerator TributeSummoningCoroutine()
    {
        yield return new WaitForSeconds(2);

        List<int> tributeIndices = tribute.GetTributes();
        int handMonsterIndex = tribute.GetHandMonsterIndex();

        //destroy tributes and summon new monster
        DestroyOwnMonsters(tributeIndices);
        fieldScript.DestroyTributeCircles();

        int diskIndex = playerScript.SetMonsterOnDisk(tribute.GetHandMonsterIndex(), tribute.GetCardInfo(), tribute.GetFace(), tributeIndices);
        fieldScript.SetMonster(diskIndex, tribute.GetCardInfo(), tribute.GetFace());

        SetPlayerSacrificing(false);
        playerScript.ProcessTributeAvailableMonsters(false);
        playerScript.HighlightPlayerCards();
    }

    public void SwitchMonsterPosition(int index, Enums.CardFace oldFace, Enums.CardPosition oldPosition)
    {
        fieldScript.SwitchMonsterPosition(false, index, oldFace, oldPosition);
        if (oldFace == Enums.CardFace.Down)
        {
            playerScript.FlipMonster(index);
        }
    }

    private void FlipEnemyMonster(int index)
    {
        fieldScript.FlipMonster(index, true);
    }

    private void DestroyOwnMonsters(List<int> indices)
    {
        playerScript.DestroyMonsters(indices);
        fieldScript.DestroyFieldMonsters(false, indices);
    }

    public void UseSpell(int index, Card cardInfo, Enums.CardFace face)
    {
        int diskIndex = playerScript.SetSpellOnDisk(index, cardInfo, face);
        fieldScript.SetSpell(diskIndex, cardInfo, face);
        if(face == Enums.CardFace.Up)
        {
            //Send true if effect is continuous or send entire cardInfo
            StartCoroutine(ActivateSpellCoroutine(diskIndex, false));
        }
    }

    public void FlipSpell(int index, bool isEnemy)
    {
        playerScript.RemoveQuickPlayCard("SPELL_", index);
        fieldScript.FlipSpell(index, isEnemy);
    }

    public IEnumerator ActivateSpellCoroutine(int index, bool isContinuous)
    {
        yield return new WaitForSeconds(2);

        //TODO: apply spell effect

        if(!isContinuous)
        {
            playerScript.DestroySpells(new List<int>() { index });
            fieldScript.DestroyFieldSpells(false, new List<int>() { index });
        }
    }

    public string GetCardNumberForMonster(int index)
    {
        return playerScript.GetCardNumberForIndex(index);
    }

    public Turn.Phase GetTurnPhase()
    {
        return playerScript.GetCurrentPhase();
    }

    public void InitDuel()
    {
        playerScript.SetIsReadyForDuel(true);
        playerScript.InitDuel();
    }

    public void DiscardCard(int index)
    {
        playerScript.RemoveCardFromHand(index);
        SetInfoTextOnScreen("", false);

        // Send card to Graveyard
    }

    private void PauseCurrentPhase()
    {
        playerScript.UnhighlightEverything();
    }

    public void StartMyTurn()
    {
        playerScript.StartMyTurn();
    }

    public int GetAttackingMonsterIndex()
    {
        if (attacking)
        {
            return attackingMonsterIndex;
        }
        return 100;
    }

    public void AttackWithMonster(int index)
    {
        SetAttacking(true);
        attackingMonsterIndex = index;
        fieldScript.AddAttackSword(false, index);

        PauseCurrentPhase();
        if (fieldScript.GetNoAttackableMonsters() == 0)
        {
            //attack life points directly
            Monster attackingMonster = (Monster)playerScript.GetCardInfoForIndex(attackingMonsterIndex, true);
            DecreaseLifePoints(attackingMonster.GetAttackPoints(), true);

            AfterAttack();
        }
        else
        {
            fieldScript.ProcessAttackableMonsters(true);
        }
    }

    public void CancelAttack()
    {
        SetAttacking(false);
        fieldScript.DestroySword();
        fieldScript.ProcessAttackableMonsters(false);
        playerScript.HighlightPlayerCards();
    }

    private void AfterAttack()
    {
        SetAttacking(false);
        playerScript.ApplyRestrictionsForAttackingMonster(attackingMonsterIndex);
        fieldScript.ProcessAttackableMonsters(false);
        StartCoroutine(PostAttackOperationsCoroutine());
    }

    private void DecreaseLifePoints(int points, bool isEnemy)
    {
        bool hasDuelEnded = false;
        if (isEnemy)
        {
            enemyLifePoints -= points;
            if(enemyLifePoints < 0)
            {
                enemyLifePoints = 0;
                hasDuelEnded = true;
            }
            infoScreenScript.ChangePoints(enemyLifePoints.ToString(), true);
        } else
        {
            long newPoints = playerScript.DecreaseLifePoints(points);
            if(newPoints < 0)
            {
                newPoints = 0;
                hasDuelEnded = true;
            }
            infoScreenScript.ChangePoints(newPoints.ToString(), false);
        }
        if(hasDuelEnded)
        {
            EndDuel(!isEnemy);
        }
    }

    public void AttackTarget(int targetIndex, Enums.CardPosition targetPosition, Enums.CardFace targetFace)
    {
        string details = Constants.ATTACKING_TEXT + ";" + attackingMonsterIndex + ";" + targetIndex;
        List<MessageParameter> parameters = new List<MessageParameter>()
        {
            new MessageParameter(Constants.TARGET_INDEX_KEY, targetIndex.ToString())
        };
        SendInformation(Constants.ATTACKING_TEXT, attackingMonsterIndex, parameters);

        //TODO: wait for the response, update stuff if needed and then do this
        Monster attackingMonster = (Monster) playerScript.GetCardInfoForIndex(attackingMonsterIndex, true);
        Monster targetMonster = (Monster) fieldScript.GetEnemyCardInfo(targetIndex, true);

        if (targetPosition == Enums.CardPosition.Def && targetFace == Enums.CardFace.Down)
        {
            FlipEnemyMonster(targetIndex);
        }

        int enemyMonsterRelevantPoints = targetPosition == Enums.CardPosition.Atk ?
            targetMonster.GetAttackPoints() : targetMonster.GetDefensePoints();
        int diff = attackingMonster.GetAttackPoints() - enemyMonsterRelevantPoints;

        if(diff > 0)
        {
            fieldScript.DestroyFieldMonsters(true, new List<int>() { targetIndex });
            if (targetPosition == Enums.CardPosition.Atk)
            {
                DecreaseLifePoints(diff, true);
            }
        }

        if(diff < 0)
        {
            DecreaseLifePoints(-diff, false);
            if(targetPosition == Enums.CardPosition.Atk)
            {
                DestroyOwnMonsters(new List<int>() { attackingMonsterIndex });
            }
        }

        if (diff == 0 && targetPosition == Enums.CardPosition.Atk)
        {
            DestroyOwnMonsters(new List<int>() { attackingMonsterIndex });
            fieldScript.DestroyFieldMonsters(true, new List<int>() { targetIndex });
        }
        AfterAttack();
    }

    private IEnumerator PostAttackOperationsCoroutine()
    {
        yield return new WaitForSeconds(2);

        fieldScript.DestroySword();
        playerScript.HighlightPlayerCards();
    }

    private bool CanQuickPlayCards()
    {
        return playerScript.CanQuickPlayCards();
    }

    private void SendQuickActivationEndMessage()
    {
        List<MessageParameter> parameters = new List<MessageParameter>()
        {
            new MessageParameter(Constants.QA_PHASE_KEY, Constants.QA_END)
        };
        SendInformation(Constants.QUICK_ACTIVATION, 0, parameters);
    }

    private bool AskForQuickActivation()
    {
        if (!CanQuickPlayCards())
        {
            SendQuickActivationEndMessage();
            return false;
        }
        SetInfoTextOnScreen(Constants.QUICK_PLAY_INFO + Constants.ASK_QUICK_PLAY, false);
        playerScript.AskForQuickActivation(true);
        PauseCurrentPhase();
        return true;
    }

    public void StartQuickActivation()
    {
        quickActivation = true;
        SetInfoTextOnScreen("", false);
        playerScript.ProcessQuickActivationCards(true);
    }

    public void StopQuickActivation()
    {
        quickActivation = false;
        SetInfoTextOnScreen("", false);
        SendQuickActivationEndMessage();
        playerScript.ProcessQuickActivationCards(false);
        playerScript.HighlightPlayerCards();
    }

    public void SendInformation(string action, int cardIndex, List<MessageParameter> parameters)
    {
        if(quickActivation)
        {
            parameters.Add(new MessageParameter(Constants.QA_PHASE_KEY, action));
            action = Constants.QUICK_ACTIVATION;
        }

        Message message = new Message(action, cardIndex, parameters);
        string serializedMessage = Utils.SerializeMessage(message);

        //TODO: send the serialized message
    }

    public void ReceiveInformation(string serializedMessage)
    {
        Message message = Utils.DeserializeMessage(serializedMessage);

        string action = message.GetAction();
        switch(action)
        {
            case Constants.END_TURN:
                StartMyTurn();
                break;
            case Constants.CHANGE_PHASE:
                DecodeEnemyPhaseChange(message);
                break;
            case Constants.DRAW:
                DecodeCardDraw(message);
                break;
            case Constants.QUICK_ACTIVATION:
                //maybe the opponent can activate something
                DecodeQuickActivationInfo(message);
                break;
            case Constants.BATTLE:
                //calculate attack result and do further things if needed
                DecodeBattleInformation(message);
                break;
            default:
                //must be a main phase, process this further
                DecodeMainInformation(message);
                break;
        }
    }

    private void DecodeEnemyPhaseChange(Message message)
    {
        string newPhase;
        Dictionary<string, string> actionParams = message.ExtractParamDictionary();
        if (actionParams.TryGetValue(Constants.NEW_PHASE_KEY, out newPhase))
        {
            ChangePhaseOnScreen(newPhase, true);
        }
    }

    private void DecodeCardDraw(Message message)
    {
        Dictionary<string, string> actionParams = message.ExtractParamDictionary();
    }

    private void DecodeQuickActivationInfo(Message message)
    {
        //TODO: show the action

        //TODO: store somewhere the action that needs to be taken after the quick plays

        if (!AskForQuickActivation())
        {
            //TODO: apply the backlog actions
        }
    }

    private void DecodeBattleInformation(Message message)
    {
        //TODO: show the action

        //TODO: store somewhere the action that needs to be taken after the quick plays

        if (!AskForQuickActivation())
        {
            //TODO: apply the backlog actions
        }
    }

    private void DecodeMainInformation(Message message)
    {
        string action = message.GetAction();
        int cardIndex = message.GetCardIndex();
        Dictionary<string, string> actionParams = message.ExtractParamDictionary();
        string cardNumber, cardType;

        actionParams.TryGetValue(Constants.CARD_NO_KEY, out cardNumber);

        if (action == Constants.ATK_CHANGE_TEXT || action == Constants.DEF_CHANGE_TEXT)
        {
            string faceParam;
            actionParams.TryGetValue(Constants.FACE_KEY, out faceParam);

            Enums.CardFace face = (Enums.CardFace)Enum.Parse(typeof(Enums.CardFace), faceParam);
            Enums.CardPosition oldPosition = (action == Constants.ATK_CHANGE_TEXT) ? Enums.CardPosition.Atk : Enums.CardPosition.Def;
            fieldScript.SwitchEnemyMonsterPosition(cardIndex, cardNumber, face, oldPosition);

            AskForQuickActivation();
            return;
        }
        
        actionParams.TryGetValue(Constants.TYPE_KEY, out cardType);

        if(cardType == Constants.MONSTER)
        {
            Card cardInfo = (action == Constants.SETTING_TEXT) ? null : Config.Get().GetCardInfoByNumber(cardNumber, true);
            Enums.CardFace face = (action == Constants.SETTING_TEXT) ? Enums.CardFace.Down : Enums.CardFace.Up;

            if (action == Constants.SUMMONING_TEXT)
            {
                string tributeNumberParam;
                actionParams.TryGetValue(Constants.TRIBUTE_NO_KEY, out tributeNumberParam);

                int noTributes = Int32.Parse(tributeNumberParam);
                if (noTributes > 0)
                {
                    string tributes;
                    actionParams.TryGetValue(Constants.TRIBUTE_INDICES_KEY, out tributes);

                    List<int> tributeIndices = JsonUtility.FromJson<List<int>>(tributes);
                    foreach (int index in tributeIndices)
                    {
                        tributeIndices.Add(index);
                    }
                    fieldScript.DestroyFieldMonsters(true, tributeIndices);
                }
            }
            fieldScript.SetEnemyMonster(cardIndex, cardInfo, face);
            AskForQuickActivation();
        }
        else
        {
            Card cardInfo = (action == Constants.ACTIVATING_TEXT) ? Config.Get().GetCardInfoByNumber(cardNumber, false) : null;
            Enums.CardFace face = (action == Constants.ACTIVATING_TEXT) ? Enums.CardFace.Up : Enums.CardFace.Down;

            //set or activate card
            fieldScript.SetEnemySpell(cardIndex, cardInfo, face);

            if(action == Constants.ACTIVATING_TEXT)
            {
                AskForQuickActivation();
                //apply effect if not quick activating anything
                return;
            }
            AskForQuickActivation();
        }
    }
    
    public void ChangePhase(string newPhase)
    {
        List<MessageParameter> parameters = new List<MessageParameter>() {
            new MessageParameter(Constants.NEW_PHASE_KEY, newPhase)
        };
        ChangePhaseOnScreen(newPhase, false);
        SendInformation(Constants.CHANGE_PHASE, 0, parameters);
    }
    
    private void ChangePhaseOnScreen(string newPhase, bool isEnemy)
    {
        infoScreenScript.ChangePhase(newPhase, isEnemy);
    }

    public void SetHandSizeOnScreen(int newSize, bool isEnemy)
    {
        infoScreenScript.ChangeHandSize(newSize.ToString(), isEnemy);
    }

    public void SetDeckSizeOnScreen(int newSize, bool isEnemy)
    {
        infoScreenScript.ChangeDeckSize(newSize.ToString(), isEnemy);
    }

    //TODO: call this when the graveyard size increases/decreases for any player
    public void SetGraveyardSizeOnScreen(int newSize, bool isEnemy)
    {
        infoScreenScript.ChangeGraveyardSize(newSize.ToString(), isEnemy);
    }

    public void SetInfoTextOnScreen(string infoText, bool isEnemy)
    {
        infoScreenScript.SetInfoText(infoText, isEnemy);
    }

    public void EndDuel(bool isEnemyWinner)
    {
        infoScreenScript.ShowEndGameScreen(isEnemyWinner);
    }
}

using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TEO
using System.IO;

public class GameManager : MonoBehaviourPunCallbacks {

    public GameObject player, field;

    private static GameManager instance;
    private PlayerScript playerScript;
    private List<Card> playerGraveyard = new List<Card>();
    private List<Card> enemyGraveyard = new List<Card>();
    private FieldScript fieldScript;

    private int enemyLifePoints = Constants.STARTING_LIFE_POINTS;
    private int enemyHand = 0;
    private int enemyDeck = 50; //TODO: remove this initialization when we get this info at the beginning of the duel
    private Tribute tribute;
    private int attackingMonsterIndex = 100, flippableMonsterIndex = 100;
    private bool playerDiscarding = false, sacrificing = false, attacking = false, quickActivation = false;
    private bool cardInfoOn = true;

    private int cardsToBeSelected = 0;
    private string selectionSource, selectionOwner, selectionCardType;
    private List<int> selectedCards = new List<int>();

    private delegate void Actions();

    private Actions actionsToBeDone;
    private List<Message> actionBacklog;

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
            actionBacklog = new List<Message>();
            actionsToBeDone = null;
        }
        else
        {
            Destroy(gameObject);
        }

        Config.Get().Load();

        Card testMonster = new Monster("15025844", File.ReadAllBytes("Assets/Resources/Images/Card Images/MysticalElf.png"),
            "Mystical Elf", "blahblahblah", 0, 5, 19, 800, 2000, 4, false);
        Card testSpell = new NonMonster("83764718", File.ReadAllBytes("Assets/Resources/Images/Card Images/MonsterReborn.png"),
            "Monster Reborn", "x", 1, 1);
        Card testTrap = new NonMonster("04206964", File.ReadAllBytes("Assets/Resources/Images/Card Images/TrapHole.png"),
            "Trap Hole", "y", 1, 2);

        playerGraveyard.Add(testMonster);
        playerGraveyard.Add(testSpell);
        enemyGraveyard.Add(testTrap);
    }

    public MyPlayer PlayerPrefab;

    [HideInInspector]
    public MyPlayer LocalPlayer;

    private void Start()
    {
        MyPlayer.RefreshInstance(ref LocalPlayer, PlayerPrefab);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        MyPlayer.RefreshInstance(ref LocalPlayer, PlayerPrefab);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            cardInfoOn = !cardInfoOn;
        }
    }

    public List<Card> GetGraveyard(string key)
    {
        if (key.Equals(Constants.ENEMY))
        {
            return enemyGraveyard;
        }
        return playerGraveyard;
    }

    public void AddCardToPlayerGraveyard(Card card)
    {
        playerGraveyard.Add(card);
        UIManager.Get().UpdatePlayerGraveyardCount(playerGraveyard.Count);
    }

    public void AddCardToEnemyGraveyard(Card card)
    {
        enemyGraveyard.Add(card);
        UIManager.Get().UpdateEnemyGraveyardCount(playerGraveyard.Count);
    }

    public void PopCardToPlayerGraveyard(Card card)
    {
        playerGraveyard.Remove(card);
        UIManager.Get().UpdatePlayerGraveyardCount(playerGraveyard.Count);
    }

    public void PopCardToEnemyGraveyard(Card card)
    {
        enemyGraveyard.Remove(card);
        UIManager.Get().UpdateEnemyGraveyardCount(playerGraveyard.Count);
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

    public bool IsCardInfoOn()
    {
        return cardInfoOn;
    }

    public void DrawCard()
    {
        playerScript.DrawCard();
    }

    public void SummonMonster(int index, Monster cardInfo, Enums.CardFace face)
    {
        PauseCurrentPhase();
        int rarity = cardInfo.GetRarity();
        if (rarity < 5)
        {
            SetMonsterOnDisk(index, cardInfo, face, new List<int>());
        }
        else
        {
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

        SetPlayerSacrificing(false);
        SetMonsterOnDisk(tribute.GetHandMonsterIndex(), tribute.GetCardInfo(), tribute.GetFace(), tributeIndices);
    }

    private void SetMonsterOnDisk(int handIndex, Monster cardInfo, Enums.CardFace face, List<int> tributeIndices)
    {
        int diskIndex = playerScript.SetMonsterOnDisk(handIndex, cardInfo, face);
        string cardNumber = cardInfo.GetCardNumber();
        fieldScript.SetMonster(diskIndex, cardNumber, face);

        string cardNumberToSend = (face == Enums.CardFace.Up) ? cardInfo.GetCardNumber() : Constants.UNKNOWN;
        string action = (face == Enums.CardFace.Up) ? Constants.SUMMONING_TEXT : Constants.SETTING_TEXT;
        string tributeIndicesString = Utils.SerializeList(tributeIndices);

        List<MessageParameter> parameters = new List<MessageParameter>()
        {
            new MessageParameter(Constants.TYPE_KEY, Constants.MONSTER),
            new MessageParameter(Constants.CARD_NO_KEY, cardNumberToSend),
            new MessageParameter(Constants.TRIBUTE_NO_KEY, tributeIndices.Count.ToString()),
            new MessageParameter(Constants.TRIBUTE_INDICES_KEY, tributeIndicesString)
        };
        SendInformation(action, diskIndex, parameters);
        UIManager.Get().SetInfoTextOnInfoPanel(Constants.QUICK_PLAY_INFO, true);
        PauseCurrentPhase();
    }

    public void SwitchMonsterPosition(int index, Enums.CardFace oldFace, Enums.CardPosition oldPosition, Monster info)
    {
        fieldScript.SwitchMonsterPosition(false, index, oldFace, oldPosition);

        List<MessageParameter> parameters = new List<MessageParameter>()
        {
            new MessageParameter(Constants.TYPE_KEY, Constants.MONSTER),
            new MessageParameter(Constants.CARD_NO_KEY, info.GetCardNumber()),
            new MessageParameter(Constants.FACE_KEY, oldFace.ToString()),
            new MessageParameter(Constants.FLIPPABLE_KEY, info.IsFlippable().ToString())
        };

        string action = Constants.FLIPPING_TEXT;

        if (oldFace == Enums.CardFace.Up)
        {
            action = (oldPosition == Enums.CardPosition.Atk) ? Constants.DEF_CHANGE_TEXT : Constants.ATK_CHANGE_TEXT;
        }

        SendInformation(action, index, parameters);
        UIManager.Get().SetInfoTextOnInfoPanel(Constants.QUICK_PLAY_INFO, true);
        PauseCurrentPhase();

        if (oldFace == Enums.CardFace.Down && playerScript.FlipMonster(index))
        {
            Message message = new Message(Constants.FLIPPING_TEXT, index, parameters);
            message.SetEnemyAction(false);
            actionBacklog.Insert(0, message);
            actionsToBeDone = FlipMonsterAction + actionsToBeDone;
        }
    }

    private void FlipMonsterAction()
    {
        Message currentMessage = PopActionFromBacklog();
        actionsToBeDone -= FlipMonsterAction;

        int monsterIndex = currentMessage.GetCardIndex();
        string cardNumber;
        currentMessage.ExtractParamDictionary().TryGetValue(Constants.CARD_NO_KEY, out cardNumber);

        Monster cardInfo = (Monster) Config.Get().GetCardInfoByNumber(cardNumber, false);
        bool isEnemyAction = currentMessage.IsEnemyAction();
    }

    private void DestroyOwnMonsters(List<int> indices)
    {
        playerScript.DestroyMonsters(indices);
        fieldScript.DestroyFieldMonsters(false, indices);
    }

    public void UseSpell(int handIndex, NonMonster cardInfo, Enums.CardFace face)
    {
        int diskIndex = playerScript.SetSpellOnDisk(handIndex, cardInfo, face);
        fieldScript.SetSpell(diskIndex, cardInfo, face);
        if(face == Enums.CardFace.Up)
        {
            ActivateSpell(diskIndex, cardInfo, Constants.HAND);
        } else
        {
            List<MessageParameter> parameters = new List<MessageParameter>()
            {
                new MessageParameter(Constants.TYPE_KEY, Constants.SPELL),
                new MessageParameter(Constants.ORIGIN_KEY, Constants.HAND),
                new MessageParameter(Constants.CARD_NO_KEY, cardInfo.GetCardNumber())
            };

            SendInformation(Constants.SETTING_TEXT, diskIndex, parameters);
            UIManager.Get().SetInfoTextOnInfoPanel(Constants.QUICK_PLAY_INFO, true);
            PauseCurrentPhase();
        }
    }

    public void FlipSpell(int index, bool isEnemy)
    {
        playerScript.RemoveQuickPlayCard("SPELL_", index);
        fieldScript.FlipSpell(index, isEnemy);
    }

    public void ActivateSpell(int index, NonMonster cardInfo, string cardOrigin)
    {
        List<MessageParameter> parameters = new List<MessageParameter>()
        {
            new MessageParameter(Constants.TYPE_KEY, Constants.SPELL),
            new MessageParameter(Constants.ORIGIN_KEY, cardOrigin),
            new MessageParameter(Constants.CARD_NO_KEY, cardInfo.GetCardNumber())
        };

        SendInformation(Constants.ACTIVATING_TEXT, index, parameters);

        UIManager.Get().SetInfoTextOnInfoPanel(Constants.QUICK_PLAY_INFO, true);
        PauseCurrentPhase();

        Message message = new Message(Constants.ACTIVATING_TEXT, index, parameters);
        message.SetEnemyAction(false);
        actionBacklog.Insert(0, message);
        actionsToBeDone = ActivateSpellAction + actionsToBeDone;
    }

    private Message PopActionFromBacklog()
    {
        Message firstAction = actionBacklog[0];
        actionBacklog.RemoveAt(0);
        return firstAction;
    }

    private void ActivateSpellAction()
    {
        Message currentMessage = PopActionFromBacklog();
        actionsToBeDone -= ActivateSpellAction;

        int spellIndex = currentMessage.GetCardIndex();
        string cardNumber;
        currentMessage.ExtractParamDictionary().TryGetValue(Constants.CARD_NO_KEY, out cardNumber);

        NonMonster cardInfo = (NonMonster) Config.Get().GetCardInfoByNumber(cardNumber, false);
        bool isEnemyAction = currentMessage.IsEnemyAction();

        //TODO: apply spell effect

        if (!cardInfo.IsContinuous())
        {
            if(!isEnemyAction) {
                playerScript.DestroySpells(new List<int>() { spellIndex });
            }
            fieldScript.DestroyFieldSpells(isEnemyAction, new List<int>() { spellIndex });
        }
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
        UIManager.Get().SetInfoTextOnInfoPanel("", false);

        // Send card to Graveyard
    }

    private void PauseCurrentPhase()
    {
        playerScript.UnhighlightEverything();
        playerScript.PauseSwitch(true);
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
            SendInformation(Constants.ATTACKING_TEXT, index, new List<MessageParameter>());
            UIManager.Get().SetInfoTextOnInfoPanel(Constants.QUICK_PLAY_INFO, true);

            Message message = new Message(Constants.ATTACKING_TEXT, index, new List<MessageParameter>());
            message.SetEnemyAction(false);
            actionBacklog.Insert(0, message);
            actionsToBeDone = PostBattleAction + actionsToBeDone;
        }
        else
        {
            fieldScript.ProcessAttackableMonsters(true);
        }
    }

    private void PostBattleAction()
    {
        Message currentMessage = PopActionFromBacklog();
        actionsToBeDone -= PostBattleAction;
        Dictionary<string, string> parameters = currentMessage.ExtractParamDictionary();

        bool isEnemy = currentMessage.IsEnemyAction();
        int monsterIndex = currentMessage.GetCardIndex();
        string targetIndexParam;

        if (parameters.TryGetValue(Constants.TARGET_INDEX_KEY, out targetIndexParam))
        {
            actionBacklog.Add(currentMessage);
            actionsToBeDone = DamageCalculationAction + actionsToBeDone;

            string targetPositionParam, targetFaceParam;

            parameters.TryGetValue(Constants.TARGET_POS_KEY, out targetPositionParam);
            parameters.TryGetValue(Constants.TARGET_FACE_KEY, out targetFaceParam);

            Enums.CardPosition targetPosition = (Enums.CardPosition) Enum.Parse(typeof(Enums.CardPosition), targetPositionParam);
            Enums.CardFace targetFace = (Enums.CardFace) Enum.Parse(typeof(Enums.CardFace), targetFaceParam);
            int targetIndex = Int32.Parse(targetIndexParam);

            Monster targetMonster;

            if(isEnemy)
            {
                targetMonster = (Monster)playerScript.GetCardInfoForIndex(targetIndex, true);
            } else
            {
                targetMonster = (Monster)fieldScript.GetEnemyCardInfo(targetIndex, true);
            }

            if (targetPosition == Enums.CardPosition.Def && targetFace == Enums.CardFace.Down)
            {
                if(isEnemy)
                {
                    playerScript.FlipMonster(targetIndex);
                    fieldScript.FlipMonster(targetIndex, false);
                    List<MessageParameter> sendParameters = new List<MessageParameter>()
                    {
                        new MessageParameter(Constants.CARD_NO_KEY, targetMonster.GetCardNumber()),
                        new MessageParameter(Constants.TYPE_KEY, Constants.MONSTER)
                    };
                    
                    GameManager.Get().SendInformation(Constants.FLIPPING_TEXT, targetIndex, sendParameters);
                   
                    if (targetMonster.IsFlippable())
                    {
                        flippableMonsterIndex = targetIndex;
                        UIManager.Get().SetInfoTextOnInfoPanel(Constants.QUICK_PLAY_INFO + Constants.ASK_FLIP_EFFECT, false);
                        playerScript.AskForEffectActivation(true);
                        PauseCurrentPhase();
                        return;
                    }
                } else
                {
                    fieldScript.FlipMonster(targetIndex, true);
                    if (targetMonster.IsFlippable())
                    {
                        UIManager.Get().SetInfoTextOnInfoPanel(Constants.QUICK_PLAY_INFO, true);
                        PauseCurrentPhase();
                        return;
                    }
                }
            }
            ApplyActionBacklog();
        }
        else
        {
            //attack life points directly
            Monster attackingMonster;
            if(isEnemy)
            {
                attackingMonster = (Monster) fieldScript.GetEnemyCardInfo(monsterIndex, true);
            } else
            {
                attackingMonster = (Monster) playerScript.GetCardInfoForIndex(monsterIndex, true);
            }
            
            DecreaseLifePoints(attackingMonster.GetAttackPoints(), !isEnemy);

            if(!isEnemy)
            {
                AfterAttack();
            }
        }
    }

    private void ApplyActionBacklog()
    {
        if(actionsToBeDone != null)
        {
            actionsToBeDone();
            actionsToBeDone = null;
        }
    }

    private void DamageCalculationAction()
    {
        Message currentMessage = PopActionFromBacklog();
        actionsToBeDone -= DamageCalculationAction;
        Dictionary<string, string> parameters = currentMessage.ExtractParamDictionary();

        int monsterIndex = currentMessage.GetCardIndex();
        bool isEnemy = currentMessage.IsEnemyAction();
        string targetIndexParam, targetPositionParam, targetFaceParam;

        parameters.TryGetValue(Constants.TARGET_INDEX_KEY, out targetIndexParam);
        parameters.TryGetValue(Constants.TARGET_POS_KEY, out targetPositionParam);
        parameters.TryGetValue(Constants.TARGET_FACE_KEY, out targetFaceParam);

        Enums.CardPosition targetPosition = (Enums.CardPosition)Enum.Parse(typeof(Enums.CardPosition), targetPositionParam);
        Enums.CardFace targetFace = (Enums.CardFace)Enum.Parse(typeof(Enums.CardFace), targetFaceParam);
        int targetIndex = Int32.Parse(targetIndexParam);

        Monster attackingMonster, targetMonster;
        if(isEnemy)
        {
            attackingMonster = (Monster) fieldScript.GetEnemyCardInfo(monsterIndex, true);
            targetMonster = (Monster) playerScript.GetCardInfoForIndex(targetIndex, true);
        } else
        {
            attackingMonster = (Monster) playerScript.GetCardInfoForIndex(monsterIndex, true);
            targetMonster = (Monster) fieldScript.GetEnemyCardInfo(targetIndex, true);
        }

        int targetMonsterRelevantPoints = targetPosition == Enums.CardPosition.Atk ?
                targetMonster.GetAttackPoints() : targetMonster.GetDefensePoints();
        int diff = attackingMonster.GetAttackPoints() - targetMonsterRelevantPoints;

        if (diff > 0)
        {
            if(isEnemy)
            {
                DestroyOwnMonsters(new List<int>() { targetIndex });
            } else
            {
                fieldScript.DestroyFieldMonsters(true, new List<int>() { targetIndex });
            }
            if (targetPosition == Enums.CardPosition.Atk)
            {
                DecreaseLifePoints(diff, !isEnemy);
            }
        }

        if (diff < 0)
        {
            DecreaseLifePoints(-diff, isEnemy);
            if (targetPosition == Enums.CardPosition.Atk)
            {
                if(isEnemy)
                {
                    fieldScript.DestroyFieldMonsters(true, new List<int>() { monsterIndex });

                } else
                {
                    DestroyOwnMonsters(new List<int>() { monsterIndex });
                }
                
            }
        }

        if (diff == 0 && targetPosition == Enums.CardPosition.Atk)
        {
            DestroyOwnMonsters(new List<int>() { (isEnemy) ? targetIndex : monsterIndex });
            fieldScript.DestroyFieldMonsters(true, new List<int>() { (isEnemy) ? monsterIndex :  targetIndex });
        }

        if(!isEnemy)
        {
            AfterAttack();
        }
        else
        {
            StartCoroutine(PostAttackOperationsCoroutine(true));
        }
    }
 
    public void CancelAttack()
    {
        SetAttacking(false);
        fieldScript.DestroySword();
        fieldScript.ProcessAttackableMonsters(false);
        playerScript.HighlightPlayerCards();
        playerScript.PauseSwitch(false);
    }

    private void AfterAttack()
    {
        SetAttacking(false);
        playerScript.ApplyRestrictionsForAttackingMonster(attackingMonsterIndex);
        fieldScript.ProcessAttackableMonsters(false);
        StartCoroutine(PostAttackOperationsCoroutine(false));
    }

    private void DecreaseLifePoints(int points, bool isTargetEnemy)
    {
        bool hasDuelEnded = false;
        if (isTargetEnemy)
        {
            enemyLifePoints -= points;
            if(enemyLifePoints < 0)
            {
                enemyLifePoints = 0;
                hasDuelEnded = true;
            }
            UIManager.Get().UpdatePointsOnInfoPanel(enemyLifePoints.ToString(), true);
        } else
        {
            long newPoints = playerScript.ModifyLifePoints((-1) * points);
            if(newPoints < 0)
            {
                newPoints = 0;
                hasDuelEnded = true;
            }
            UIManager.Get().UpdatePointsOnInfoPanel(newPoints.ToString(), false);
        }
        if(hasDuelEnded)
        {
            UIManager.Get().ShowDuelEnd(!isTargetEnemy);
        }
    }

    public void AttackTarget(int targetIndex, Enums.CardPosition targetPosition, Enums.CardFace targetFace)
    {
        List<MessageParameter> parameters = new List<MessageParameter>()
        {
            new MessageParameter(Constants.TARGET_INDEX_KEY, targetIndex.ToString()),
            new MessageParameter(Constants.TARGET_POS_KEY, targetPosition.ToString()),
            new MessageParameter(Constants.TARGET_FACE_KEY, targetFace.ToString())
        };
        SendInformation(Constants.ATTACKING_TEXT, attackingMonsterIndex, parameters);
        UIManager.Get().SetInfoTextOnInfoPanel(Constants.QUICK_PLAY_INFO, true);

        PauseCurrentPhase();

        Message message = new Message(Constants.ATTACKING_TEXT, attackingMonsterIndex, parameters);
        message.SetEnemyAction(false);
        actionBacklog.Insert(0, message);
        actionsToBeDone = PostBattleAction + actionsToBeDone;
    }

    private IEnumerator PostAttackOperationsCoroutine(bool isEnemy)
    {
        yield return new WaitForSeconds(2);

        fieldScript.DestroySword();
        if(!isEnemy)
        {
            playerScript.HighlightPlayerCards();
            playerScript.PauseSwitch(false);
        }
    }

    public void StopFlipEffectChoicePhase(bool shouldActivate)
    {
        List<MessageParameter> parameters = new List<MessageParameter>()
        {
            new MessageParameter(Constants.PHASE_KEY, shouldActivate ? Constants.ACCEPT : Constants.DENY)
        };

        if (shouldActivate)
        {
            Message message = new Message(Constants.FLIP_EFFECT_ACTIVATION, flippableMonsterIndex, parameters);
            message.SetEnemyAction(false);
            actionBacklog.Add(message);
            actionsToBeDone = ActivateCardEffect + actionsToBeDone;
        }

        SendInformation(Constants.FLIP_EFFECT_ACTIVATION, flippableMonsterIndex, parameters);
        flippableMonsterIndex = 100;
    }

    public void TriggerQuickEffectActivation(int cardIndex, bool isMonster)
    {
        StopQuickActivation();
        playerScript.RemoveQuickPlayCard(isMonster ? "MONSTER_" : "SPELL_", cardIndex);

        List<MessageParameter> parameters = new List<MessageParameter>()
        {
            new MessageParameter(Constants.PHASE_KEY, Constants.ACTIVATING_TEXT),
            new MessageParameter(Constants.TYPE_KEY, isMonster ? Constants.MONSTER : Constants.SPELL)
        };

        SendInformation(Constants.QUICK_ACTIVATION, cardIndex, parameters);
        UIManager.Get().SetInfoTextOnInfoPanel(Constants.QUICK_PLAY_INFO, true);

        PauseCurrentPhase();

        Message message = new Message(Constants.QUICK_ACTIVATION, cardIndex, parameters);
        message.SetEnemyAction(false);
        actionBacklog.Insert(0, message);
        actionsToBeDone = ActivateCardEffect + actionsToBeDone;
    }

    private void ActivateCardEffect()
    {
        Message currentMessage = PopActionFromBacklog();
        actionsToBeDone -= ActivateCardEffect;
        Dictionary<string, string> parameters = currentMessage.ExtractParamDictionary();

        int cardIndex = currentMessage.GetCardIndex();
        bool isEnemy = currentMessage.IsEnemyAction(), isMonster = false;

        string action = currentMessage.GetAction();
        if(action == Constants.FLIP_EFFECT_ACTIVATION)
        {
            isMonster = true;
        }
        if(action == Constants.QUICK_ACTIVATION)
        {
            string cardType;
            parameters.TryGetValue(Constants.TYPE_KEY, out cardType);

            isMonster = cardType == Constants.MONSTER;
        }

        Card card;
        if(isEnemy)
        {
            card = playerScript.GetCardInfoForIndex(cardIndex, isMonster);
        } else
        {
            card = fieldScript.GetEnemyCardInfo(cardIndex, isMonster);
        }

        //TODO: activate the effect (will need some more information in the message)
    }

    private bool CanQuickPlayCards()
    {
        return playerScript.CanQuickPlayCards();
    }

    public void SendQuickActivationEndMessage()
    {
        List<MessageParameter> parameters = new List<MessageParameter>()
        {
            new MessageParameter(Constants.PHASE_KEY, Constants.DENY)
        };
        SendInformation(Constants.QUICK_ACTIVATION, 0, parameters);
    }

    private bool AskForQuickActivation()
    {
        if (!CanQuickPlayCards())
        {
            StopQuickActivation();
            SendQuickActivationEndMessage();
            return false;
        }
        UIManager.Get().SetInfoTextOnInfoPanel(Constants.QUICK_PLAY_INFO + Constants.ASK_QUICK_PLAY, false);
        playerScript.AskForQuickActivation(true);
        PauseCurrentPhase();
        return true;
    }

    public void StartQuickActivation()
    {
        quickActivation = true;
        UIManager.Get().SetInfoTextOnInfoPanel("", false);
        playerScript.ProcessQuickActivationCards(true);
    }

    public void StopQuickActivation()
    {
        quickActivation = false;
        UIManager.Get().SetInfoTextOnInfoPanel("", false);
        playerScript.ProcessQuickActivationCards(false);
 
        ApplyActionBacklog();

        playerScript.HighlightPlayerCards();
        playerScript.PauseSwitch(false);
    }

    public void SendInformation(string action, int cardIndex, List<MessageParameter> parameters)
    {
        if(quickActivation)
        {
            parameters.Add(new MessageParameter(Constants.PHASE_KEY, action));
            action = Constants.QUICK_ACTIVATION;
        }

        Message message = new Message(action, cardIndex, parameters);
        string serializedMessage = Utils.SerializeMessage(message);

        if (LocalPlayer && LocalPlayer.GetComponent<MyPlayer>())
        {
            Debug.Log("[TEO] Sent");
            LocalPlayer.GetComponent<MyPlayer>().SendRPCMessage(serializedMessage);
        }
    }

    public void ReceiveInformation(string serializedMessage)
    {
        Message message = Utils.DeserializeMessage(serializedMessage);
        Debug.Log("[TEO] Received");

        message.SetEnemyAction(true);
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
            case Constants.FLIP_EFFECT_ACTIVATION:
                DecodeQuickActivationInfo(message);
                break;
            case Constants.ATTACKING_TEXT:
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
        if (actionParams.TryGetValue(Constants.PHASE_KEY, out newPhase))
        {
            UIManager.Get().ChangePhaseOnInfoPanel(newPhase, true);
        }
    }

    private void DecodeCardDraw(Message message)
    {
        UIManager.Get().SetHandSizeOnInfoPanel((++enemyHand).ToString(), true);
        UIManager.Get().SetDeckSizeOnInfoPanel((--enemyDeck).ToString(), true);
    }

    private void DecodeQuickActivationInfo(Message message)
    {
        string action = message.GetAction();
        string activationPhase;
        message.ExtractParamDictionary().TryGetValue(Constants.PHASE_KEY, out activationPhase);
        if(activationPhase == Constants.DENY)
        {
            StopQuickActivation();
            return;
        }

        if(action == Constants.FLIP_EFFECT_ACTIVATION)
        {
            //TODO: show the flip effect somehow - maybe do the same as for spell effects (send each step from the player who activates it)
            //actionBacklog.Add(message);
            //actionsToBeDone = ActivateCardEffect + actionsToBeDone;
        } else
        {
            //TODO: show the action, put what's left in the backlog
        }

        if (!AskForQuickActivation())
        {
            ApplyActionBacklog();
        }
    }

    private void DecodeBattleInformation(Message message)
    {
        Dictionary<string, string> parameters = message.ExtractParamDictionary();
        string targetIndexString;

        fieldScript.AddAttackSword(true, message.GetCardIndex());

        if (parameters.TryGetValue(Constants.TARGET_INDEX_KEY, out targetIndexString))
        {
            //TODO: show somehow which monster is going to be attacked - highlight it (red or green)
            int targetIndex = Int32.Parse(targetIndexString);
            if(playerScript.GetCardFaceForIndex(targetIndex, true) == Enums.CardFace.Down)
            {
                SendInformation(Constants.REVEAL_CARD_KEY, targetIndex, new List<MessageParameter>()
                {
                    new MessageParameter(Constants.TYPE_KEY, Constants.MONSTER),
                    new MessageParameter(Constants.CARD_NO_KEY, playerScript.GetCardInfoForIndex(targetIndex, true).GetCardNumber())
                });
            }
            
        }

        message.SetEnemyAction(true);
        actionBacklog.Insert(0, message);
        actionsToBeDone = PostBattleAction + actionsToBeDone;

        if (!AskForQuickActivation())
        {
            ApplyActionBacklog();
        }
    }

    private void DecodeMainInformation(Message message)
    {
        string action = message.GetAction();
        int cardIndex = message.GetCardIndex();
        Dictionary<string, string> actionParams = message.ExtractParamDictionary();
        string cardNumber, cardType;

        actionParams.TryGetValue(Constants.CARD_NO_KEY, out cardNumber);
        actionParams.TryGetValue(Constants.TYPE_KEY, out cardType);

        if (action == Constants.REVEAL_CARD_KEY)
        {
            fieldScript.SetEnemyCardInfo(cardIndex, Config.Get().GetCardInfoByNumber(cardNumber, cardType == Constants.MONSTER));
            return;
        }

        if(action == Constants.DESELECT)
        {
            playerScript.DeselectDiskCards();
            fieldScript.DeselectAllFieldCards();
            //TODO: also deselect the deck/graveyard cards (if any) - maybe these should be stored
            //  and here we can check if there is any card selected

            selectionSource = null;
            selectionOwner = null;
            selectionCardType = null;
            selectedCards.Clear();
        }

        if (action == Constants.ATK_CHANGE_TEXT || action == Constants.DEF_CHANGE_TEXT)
        {
            string faceParam;
            actionParams.TryGetValue(Constants.FACE_KEY, out faceParam);

            Enums.CardFace face = (Enums.CardFace)Enum.Parse(typeof(Enums.CardFace), faceParam);
            Enums.CardPosition newPosition = (action == Constants.ATK_CHANGE_TEXT) ? Enums.CardPosition.Atk : Enums.CardPosition.Def;
            fieldScript.SwitchEnemyMonsterPosition(cardIndex, cardNumber, face, newPosition);

            AskForQuickActivation();
            return;
        }

        if (action == Constants.SELECTION_TEXT)
        {
            string selCount;
            actionParams.TryGetValue(Constants.SELECT_NO_KEY, out selCount);

            int noSelects = Int32.Parse(selCount);
            if (noSelects > 0)
            {
                string selIndices, selSource, selOwner;
                actionParams.TryGetValue(Constants.SELECT_INDICES_KEY, out selIndices);
                actionParams.TryGetValue(Constants.SELECT_SOURCE_KEY, out selSource);
                actionParams.TryGetValue(Constants.SELECT_OWNER_KEY, out selOwner);

                List<int> selectionIndices = Utils.DeserializeList(selIndices);
                HighlightSelectedCards(selectionIndices, selSource, selOwner, cardType);
            }
        }
        else
        {

            if (cardType == Constants.MONSTER)
            {
                Card cardInfo = (action == Constants.SETTING_TEXT) ? null : Config.Get().GetCardInfoByNumber(cardNumber, true);
                Enums.CardFace face = (action == Constants.SETTING_TEXT) ? Enums.CardFace.Down : Enums.CardFace.Up;

                if (action == Constants.SUMMONING_TEXT || action == Constants.SETTING_TEXT)
                {
                    string tributeNumberParam;
                    actionParams.TryGetValue(Constants.TRIBUTE_NO_KEY, out tributeNumberParam);

                    int noTributes = Int32.Parse(tributeNumberParam);
                    if (noTributes > 0)
                    {
                        string tributes;
                        actionParams.TryGetValue(Constants.TRIBUTE_INDICES_KEY, out tributes);

                        List<int> tributeIndices = Utils.DeserializeList(tributes);
                        fieldScript.DestroyFieldMonsters(true, tributeIndices);
                    }
                    fieldScript.SetEnemyMonster(cardIndex, cardInfo, face);
                }

                if (action == Constants.FLIPPING_TEXT && ((Monster)cardInfo).IsFlippable())
                {
                    fieldScript.SetEnemyMonster(cardIndex, cardInfo, face);
                    actionBacklog.Insert(0, message);
                    actionsToBeDone = FlipMonsterAction + actionsToBeDone;
                }
            }
            else
            {
                Card cardInfo = (action == Constants.ACTIVATING_TEXT) ? Config.Get().GetCardInfoByNumber(cardNumber, false) : null;
                Enums.CardFace face = (action == Constants.ACTIVATING_TEXT) ? Enums.CardFace.Up : Enums.CardFace.Down;

                fieldScript.SetEnemySpell(cardIndex, cardInfo, face);

                /*if(action == Constants.ACTIVATING_TEXT)
                {
                    actionBacklog.Insert(0, message);
                    actionsToBeDone = ActivateSpellAction + actionsToBeDone;
                }*/
            }
        }

        AskForQuickActivation();
    }
    
    public void ChangePhase(string newPhase)
    {
        List<MessageParameter> parameters = new List<MessageParameter>() {
            new MessageParameter(Constants.PHASE_KEY, newPhase)
        };
        UIManager.Get().ChangePhaseOnInfoPanel(newPhase, false);
        SendInformation(Constants.CHANGE_PHASE, 0, parameters);
    }

    public void ModifyPlayerLifePoints(int points)
    {
        playerScript.ModifyLifePoints(points);
    }

    public void ShuffleDeck()
    {
        playerScript.ShuffleDeck();
    }

    public void TriggerMonsterSelection(int noMonsters, int attribute, int type, string owner, string source, int superiorAtkLimit)
    {
        cardsToBeSelected = noMonsters;
        selectionSource = source;
        selectionOwner = owner;
        selectionCardType = Constants.MONSTER;

        PauseCurrentPhase();
        switch (source)
        {
            case Constants.FIELD:
                if(owner == Constants.PLAYER || owner == Constants.BOTH)
                {
                    playerScript.ProcessSelectableMonstersOnDisk(attribute, type, superiorAtkLimit, true);
                }
                if (owner == Constants.ENEMY || owner == Constants.BOTH)
                {
                    fieldScript.ProcessSelectableMonstersOnField(attribute, type, superiorAtkLimit, true);
                }
                break;
            case Constants.DECK:
                //TODO: implement selection from deck (only your own)
                break;
            case Constants.GRAVEYARD:
                //TODO: after graveyard is in place, implement selection from it - disable the other graveyard if the effect
                //  forces you to pick only from a certain player's graveyard
                break;
        }
    }

    public void TriggerSpellSelection(int noSpells, int type, string owner)
    {
        cardsToBeSelected = noSpells;
        selectionOwner = owner;
        selectionCardType = Constants.SPELL;

        PauseCurrentPhase();
        if (owner == Constants.PLAYER || owner == Constants.BOTH)
        {
            playerScript.ProcessSelectableSpellsOnDisk(type, true);
        }
        if (owner == Constants.ENEMY || owner == Constants.BOTH)
        {
            
        }
    }

    public void SelectMonster(int index)
    {
        selectedCards.Add(index);
        if(--cardsToBeSelected == 0)
        {
            Debug.Log("Selected " + selectedCards.Count + " monsters");
            playerScript.UnhighlightEverything();

            //send message to the enemy with the selection
            string selIndicesString = Utils.SerializeList(selectedCards);
            List<MessageParameter> parameters = new List<MessageParameter>() {
                new MessageParameter(Constants.SELECT_NO_KEY, selectedCards.Count.ToString()),
                new MessageParameter(Constants.SELECT_INDICES_KEY, selIndicesString),
                new MessageParameter(Constants.SELECT_SOURCE_KEY, selectionSource),
                new MessageParameter(Constants.SELECT_OWNER_KEY, selectionOwner),
                new MessageParameter(Constants.TYPE_KEY, Constants.MONSTER)
            };

            SendInformation(Constants.SELECTION_TEXT, 0, parameters);

            //TODO: proceed with the action backlog; have the effect stored there or somewhere else before the selection
        }
    }

    private void HighlightSelectedCards(List<int> indices, string source, string owner, string cardType)
    {
        owner = Utils.SwitchOwner(owner);
        switch (source)
        {
            case Constants.FIELD:
                if(owner == Constants.PLAYER || owner == Constants.BOTH)
                {
                    playerScript.ShowEnemySelection(indices, cardType == Constants.MONSTER, true);
                }
                if (owner == Constants.ENEMY || owner == Constants.BOTH)
                {
                    fieldScript.ShowEnemySelection(indices, cardType == Constants.MONSTER, true);
                }
                break;
            case Constants.DECK:
                //TODO: show the selected card somewhere (just if the effect says so - have a new param for this)
                break;
            case Constants.GRAVEYARD:
                //TODO: show the selection from the graveyard (disable switching from to the other player's graveyard maybe)
                break;
        }
    }

    private void TriggerSelectionUnhighlight()
    {
        SendInformation(Constants.DESELECT, 0, new List<MessageParameter>());
    }
}

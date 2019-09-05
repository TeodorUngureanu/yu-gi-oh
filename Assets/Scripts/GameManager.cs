using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public GameObject player, field;
    public GameObject enemyLP, myLP;

    private static GameManager instance;
    private PlayerScript playerScript;
    private Graveyard playerGraveyard, enemyGraveyard;
    private FieldScript fieldScript;

    private int enemyLifePoints = Constants.STARTING_LIFE_POINTS;
    private Tribute tribute;
    private int attackingMonsterIndex;
    private bool playerDiscarding = false, sacrificing = false, attacking = false;

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

    public void SetPlayerSacrificing(bool value)
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

    public void DrawCard()
    {
        playerScript.DrawCard();
    }

    public void SummonMonster(int index, Card cardInfo, Enums.CardFace face)
    {
        int rarity = ((Monster)cardInfo).getRarity();
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
            //tribute animation
            Invoke("CommenceTributeSummoning", 3.0f);
        }
    }

    private void CommenceTributeSummoning()
    {
        List<int> tributeIndices = tribute.GetTributes();
        int handMonsterIndex = tribute.GetHandMonsterIndex();

        //destroy tributes and summon new monster
        DestroyOwnMonsters(tributeIndices);
        fieldScript.DestroyTributeCircles();

        int diskIndex = playerScript.SetMonsterOnDisk(tribute.GetHandMonsterIndex(), tribute.GetCardInfo(), tribute.GetFace(), tributeIndices);
        fieldScript.SetMonster(diskIndex, tribute.GetCardInfo(), tribute.GetFace());

        playerScript.ProcessTributeAvailableMonsters(false);
        playerScript.HighlightPlayerCards();
    }

    public void SwitchMonsterPosition(int index, Enums.CardFace face, Enums.CardPosition position)
    {
        fieldScript.SwitchMonsterPosition(false, index, face, position);
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

    public void AttackWithMonster(int index)
    {
        SetAttacking(true);
        attackingMonsterIndex = index;
        fieldScript.AddAttackSword(false, index);

        PauseCurrentPhase();
        if (fieldScript.GetNoAttackableMonsters() == 0)
        {
            //attack life points directly
            Monster attackingMonster = (Monster)playerScript.GetCardInfoForIndex(index, true);
            enemyLifePoints -= attackingMonster.getAttackPoints();
            enemyLP.GetComponent<Text>().text = enemyLifePoints.ToString();

            Invoke("ApplyPostAttackOperations", 2.0f);
        }
        else
        {
            fieldScript.ProcessAttackableMonsters(true);
        }
    }

    public void AttackTarget(int targetIndex)
    {
        string details = Constants.ATTACKING_TEXT + ";" + attackingMonsterIndex + ";" + targetIndex;
        GameManager.Get().SendInformation(details);
    }

    public void ApplyPostAttackOperations()
    {
        fieldScript.DestroySword();
        playerScript.HighlightPlayerCards();
    }

    public void SendInformation(string details)
    {
        string message = "";
        message += GetTurnPhase().ToString();

        //add action
        message += ";" + details;
    }

    public void ReceiveInformation(string message)
    {
        string[] elements = message.Split(';');
        switch(elements[0])
        {
            case "End":
                StartMyTurn();
                break;
            case "Response":
                //maybe the opponent activated something
                DecodeResponse(elements);
                break;
            case "Battle":
                //calculate attack result and do further things if needed
                DecodeBattleInformation(elements);
                break;
            default:
                //must be a main phase, process this further
                DecodeMainInformation(elements);
                break;
        }
    }

    private void DecodeResponse(string[] messageElements)
    {

    }

    private void DecodeBattleInformation(string[] messageElements)
    {

    }

    private void DecodeMainInformation(string[] messageElements)
    {
        string action = messageElements[1];

        if(action == Constants.ATK_CHANGE_TEXT || action == Constants.DEF_CHANGE_TEXT)
        {
            int cardIndex = Int32.Parse(messageElements[2]);
            Enums.CardFace face = (Enums.CardFace)Enum.Parse(typeof(Enums.CardFace), messageElements[4]);
            Enums.CardPosition oldPosition = (action == Constants.ATK_CHANGE_TEXT) ? Enums.CardPosition.Atk : Enums.CardPosition.Def;
            fieldScript.SwitchEnemyMonsterPosition(cardIndex, messageElements[3], face, oldPosition);

            return;
        }

        string cardType = messageElements[2];

        if(cardType == Constants.MONSTER)
        {
            int cardIndex = Int32.Parse(messageElements[3]);
            Card cardInfo = (action == Constants.SUMMONING_TEXT) ? Config.Get().GetCardInfoByNumber(messageElements[4], true) : null;
            Enums.CardFace face = (action == Constants.SUMMONING_TEXT) ? Enums.CardFace.Up : Enums.CardFace.Down;

            int noTributes = Int32.Parse(messageElements[5]);
            List<int> tributeIndices = new List<int>();
            for(int i = 0; i < noTributes; i++)
            {
                int index = Int32.Parse(messageElements[i + 5]);
                tributeIndices.Add(index);
            }

            fieldScript.DestroyFieldMonsters(true, tributeIndices);

            fieldScript.SetEnemyMonster(cardIndex, cardInfo, face);
        }
        else
        {
            int cardIndex = Int32.Parse(messageElements[4]);
            Card cardInfo = (action == Constants.ACTIVATING_TEXT) ? Config.Get().GetCardInfoByNumber(messageElements[5], false) : null;
            Enums.CardFace face = (action == Constants.ACTIVATING_TEXT) ? Enums.CardFace.Up : Enums.CardFace.Down;

            //set or activate card
            fieldScript.SetEnemySpell(cardIndex, cardInfo, face);

            if(action == Constants.ACTIVATING_TEXT)
            {
                //also apply effect
            }
        }
    }
}

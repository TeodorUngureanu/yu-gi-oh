using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private static GameManager instance;
    public GameObject player, field;
    private Player playerScript;
    private Graveyard playerGraveyard, enemyGraveyard;

    private FieldScript fieldScript;

    private bool playerDiscarding = false;

    public static GameManager Get()
    {
        return instance;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            if(player != null)
            {
                playerScript = player.GetComponent<Player>();
            }
            fieldScript = field.GetComponent<FieldScript>();
        }
        else
        {
            Destroy(gameObject);
        }

        Config.Get().Load();
    }

    public Graveyard getGraveyard(string key)
    {
        if(key.Equals("ENEMY"))
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

    public void DrawCard()
    {
        playerScript.DrawCard();
    }

    public void SummonMonster(int index, Card cardInfo, Enums.CardFace face)
    {
        playerScript.SetMonsterOnDisk(index, cardInfo, face);
        fieldScript.SetMonster(index, cardInfo, face);
    }

    public void UseSpell(int index, Card cardInfo, Enums.CardFace face)
    {
        playerScript.SetSpellOnDisk(index, cardInfo, face);
        fieldScript.SetSpell(index, cardInfo, face);
    }

    public Turn.Phase GetTurnPhase()
    {
        return player.GetComponent<Player>().GetCurrentPhase();
    }

    public void InitDuel()
    {
        player.GetComponent<Player>().SetIsReadyForDuel(true);
        player.GetComponent<Player>().InitDuel();
    }

    public void DiscardCard(int index)
    {
        player.GetComponent<Player>().RemoveCardFromHand(index);

        // Send card to Graveyard
    }

    public void StartMyTurn()
    {
        player.GetComponent<Player>().StartMyTurn();
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
            case "Battle":
                //calculate attack result and do further things if needed
                break;
            default:
                //must be a main phase, process this further
                break;
        }
    }
}

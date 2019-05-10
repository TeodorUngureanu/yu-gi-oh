using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private static GameManager instance;
    public GameObject player, field;
    private Player enemy, playerScript; //is this needed?
    private Graveyard playerGraveyard, enemyGraveyard;

    private FieldScript fieldScript;

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

    public void DrawCard()
    {
        playerScript.DrawCard();
    }

    public void PlaceMonsterOnDisk(int index)
    {
        playerScript.SetMonsterOnDisk(index);
    }

    public void PlaceSpellOnDisk(int index)
    {
        playerScript.SetSpellOnDisk(index);
    }

    public void PlaceMonsterOnField(int index)
    {
        fieldScript.SetMonster(index);
    }

    public void PlaceSpellOnField(int index)
    {
        fieldScript.SetSpell(index);
    }

    public void SetFirst(bool value)
    {
        player.GetComponent<Player>().SetIsFirst(value);
    }

    public void InitDuel()
    {
        player.GetComponent<Player>().SetIsReadyForDuel(true);
        player.GetComponent<Player>().InitDuel();
    }
}

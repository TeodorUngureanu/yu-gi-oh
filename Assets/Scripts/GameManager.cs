using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private static GameManager instance;
    public GameObject player;
    private Player enemy; //is this needed?
    private Graveyard playerGraveyard, enemyGraveyard;
    private Field field;

    public static GameManager Get()
    {
        return instance;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
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
        return field.GetEffectValueForType(monsterType);
    }

    public void DrawCard()
    {
        player.GetComponent<Player>().DrawCard();
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

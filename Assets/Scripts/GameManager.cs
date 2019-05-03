using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject player;
    private Player enemy; //?
    private Graveyard playerGraveyard, enemyGraveyard;
    private Field field;

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

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager Instance;
    
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

    public static GameManager Get()
    {
        return Instance;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            Init();
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("GameManager Instance: " + (Instance == this));
        }
    }

    private void Init()
    {
        GameData.Load();
    }
}

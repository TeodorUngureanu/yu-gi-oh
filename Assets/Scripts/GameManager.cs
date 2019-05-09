using System.Collections;
using System.Collections.Generic;
using UnityEngine;

<<<<<<< HEAD
public class GameManager : MonoBehaviour {

    private static GameManager instance;
=======
public class GameManager : MonoBehaviour
{
    private static GameManager Instance;
    
>>>>>>> 924afc0db7f6b01457bed3fa31d052ad2f180cea
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

<<<<<<< HEAD
    public void DrawCard()
    {
        player.GetComponent<Player>().DrawCard();
    }

=======
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
>>>>>>> 924afc0db7f6b01457bed3fa31d052ad2f180cea
}

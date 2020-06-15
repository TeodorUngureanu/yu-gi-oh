using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMethodsManager : MonoBehaviour
{
    private static BaseMethodsManager instance;

    public static BaseMethodsManager Get()
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

    public void ModifyPlayerLifePoints(int points)
    {
        GameManager.Get().ModifyPlayerLifePoints(points);
    }

    public void ShuffleDeck()
    {
        GameManager.Get().ShuffleDeck();
    }

    public void TriggerMonsterSelection(int noMonsters, int attribute, int type, string owner, string source, int superiorAtkLimit)
    {
        GameManager.Get().TriggerMonsterSelection(noMonsters, attribute, type, owner, source, superiorAtkLimit);
    }

    public void TriggerSpellSelection(int noSpells, int type, string owner)
    {
        GameManager.Get().TriggerSpellSelection(noSpells, type, owner);
    }
}

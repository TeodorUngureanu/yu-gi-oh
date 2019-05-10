using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldScript : MonoBehaviour {

    public List<GameObject> monsterField, spellField;
    public List<GameObject> enemyMonsterField, enemySpellField;

    private Dictionary<string, int> effects;

    public void ClearField()
    {
        effects.Clear();
    }

    public void AddEffect(string monsterType, int value)
    {
        effects.Add(monsterType, value);
    }

    public int GetEffectValueForType(string monsterType)
    {
        int value = 0;
        effects.TryGetValue(monsterType, out value);
        return value;
    }

    public void SetMonster(int index)
    {
        Debug.Log("Setting monster on field on position " + index);
        monsterField[index].GetComponent<CardScript>().SetData(CardScript.Location.FIELD, index, true);
        monsterField[index].SetActive(true);
    }

    public void SetSpell(int index)
    {
        Debug.Log("Setting spell on field on position " + index);
        spellField[index].GetComponent<CardScript>().SetData(CardScript.Location.FIELD, index, false);
        spellField[index].SetActive(true);
    }
}

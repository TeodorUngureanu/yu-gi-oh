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

    public void SetMonster(int index, Card cardInfo, Enums.CardFace face)
    {
        Debug.Log("Setting monster on field on position " + index);
        //monsterField[index].GetComponent<CardScript>().SetData(CardScript.Location.FIELD, index, "Monster", cardName);
        Texture2D texture = Utils.LoadTexture(cardInfo.GetCardNumber(), Enums.CardType.Monster);
        //apply texture to front plane after creating it

        GameObject crtMonster = monsterField[index];
        Vector3 crtRotation = crtMonster.transform.localEulerAngles;

        if(face == Enums.CardFace.Up)
        {
            crtRotation += new Vector3(180, 0, 0);
        }
        else
        {
            crtRotation += new Vector3(0, -90, 0);
        }
        crtMonster.transform.localEulerAngles = crtRotation;
        crtMonster.SetActive(true);
    }

    public void SetSpell(int index, Card cardInfo, Enums.CardFace face)
    {
        Debug.Log("Setting spell on field on position " + index);
        //spellField[index].GetComponent<CardScript>().SetData(CardScript.Location.FIELD, index, "Spell/Trap", cardName);
        spellField[index].SetActive(true);
    }
}

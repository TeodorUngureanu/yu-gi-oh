using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldScript : MonoBehaviour {

    public GameObject tributeCircle, attackSword;
    public List<GameObject> monsterField, spellField;
    public List<GameObject> enemyMonsterField, enemySpellField;

    public List<int> attackableMonsters;

    private Dictionary<string, int> fieldEffects;
    private List<GameObject> tributeCircleInstances;
    private GameObject attackSwordInstance;

    private void Awake()
    {
        tributeCircleInstances = new List<GameObject>();
        attackableMonsters = new List<int>();
    }

    public void ClearField()
    {
        fieldEffects.Clear();
    }

    public void AddEffect(string monsterType, int value)
    {
        fieldEffects.Add(monsterType, value);
    }

    public int GetEffectValueForType(string monsterType)
    {
        int value = 0;
        fieldEffects.TryGetValue(monsterType, out value);
        return value;
    }

    public void SetMonster(int index, Card cardInfo, Enums.CardFace face)
    {
        Debug.Log("Setting monster on field on position " + index);

        GameObject crtMonster = monsterField[index];
        SetMonsterCardRotation(crtMonster, face);

        ApplyTexture(crtMonster, cardInfo.GetCardNumber(), Enums.CardType.Monster);
    }

    private void SetMonsterCardRotation(GameObject crtMonster, Enums.CardFace face)
    {
        Vector3 crtRotation = crtMonster.transform.localEulerAngles;

        if (face == Enums.CardFace.Up)
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

        GameObject crtSpell = spellField[index];
        SetSpellCardRotation(crtSpell, face);

        ApplyTexture(crtSpell, cardInfo.GetCardNumber(), (Enums.CardType)Enum.Parse(typeof(Enums.CardType), ((NonMonster)cardInfo).getType().ToString()));
    }

    private void SetSpellCardRotation(GameObject crtSpell, Enums.CardFace face)
    {
        Vector3 crtRotation = crtSpell.transform.localEulerAngles;

        if (face == Enums.CardFace.Up)
        {
            crtRotation += new Vector3(180, 0, 0);
        }

        crtSpell.transform.localEulerAngles = crtRotation;
        crtSpell.SetActive(true);
    }

    private void ApplyTexture(GameObject parent, string cardNumber, Enums.CardType cardType)
    {
        Texture2D texture = Utils.LoadTexture(cardNumber, cardType);
        if (texture != null)
        {
            GameObject frontImagePlane = parent.transform.Find("FrontPlane").gameObject;
            frontImagePlane.GetComponent<Renderer>().material.mainTexture = texture;
        }
    }

    public void AddTributeCircle(bool isEnemy, int index)
    {
        GameObject fieldCard;
        if(isEnemy)
        {
            fieldCard = enemyMonsterField[index];
        }
        else
        {
            fieldCard = monsterField[index];
        }

        GameObject newTributeCircle = Instantiate<GameObject>(
            tributeCircle,
            fieldCard.transform.position + new Vector3(0, 0.2f, 0),
            Quaternion.Euler(fieldCard.transform.localEulerAngles + new Vector3(90, 0, 90)),
            fieldCard.transform);
        
        newTributeCircle.transform.SetParent(fieldCard.transform);

        tributeCircleInstances.Add(newTributeCircle);
    }

    public void DestroyTributeCircles()
    {
        for(int index = 0; index < tributeCircleInstances.Count; index ++)
        {
            Destroy(tributeCircleInstances[index]);
        }
        tributeCircleInstances.Clear();
    } 

    public void DestroyFieldMonsters(bool isEnemy, List<int> indices)
    {
        for (int index = 0; index < indices.Count; index++)
        {
            if (isEnemy)
            {
                enemyMonsterField[index].SetActive(false);
                enemyMonsterField[index].transform.localEulerAngles = new Vector3(0, 90, 0);
                attackableMonsters.Remove(index);
            }
            else
            {
                monsterField[index].SetActive(false);
                monsterField[index].transform.localEulerAngles = new Vector3(0, 90, 0);
            }
        }
    }

    public void SetEnemyMonster(int index, Card cardInfo, Enums.CardFace face)
    {
        GameObject crtMonster = enemyMonsterField[index];
        crtMonster.GetComponent<EnemyFieldCardScript>().SetCardInfo(index, cardInfo);
        SetMonsterCardRotation(crtMonster, face);

        if (cardInfo != null)
        {
            ApplyTexture(crtMonster, cardInfo.GetCardNumber(), Enums.CardType.Monster);
        }
        attackableMonsters.Add(index);
    }

    public void SetEnemySpell(int index, Card cardInfo, Enums.CardFace face)
    {
        GameObject crtSpell = enemySpellField[index];
        crtSpell.GetComponent<EnemyFieldCardScript>().SetCardInfo(index, cardInfo);
        SetSpellCardRotation(crtSpell, face);

        if (cardInfo != null)
        {
            ApplyTexture(crtSpell, cardInfo.GetCardNumber(), Enums.CardType.Spell);
        }
    }

    public void SwitchMonsterPosition(bool isEnemy, int index, Enums.CardFace oldFace, Enums.CardPosition oldPosition)
    {
        Vector3 crtRotation;
        if (isEnemy)
        {
            crtRotation = enemyMonsterField[index].transform.localEulerAngles;
            if (oldFace == Enums.CardFace.Down)
            {
                crtRotation.x += 180;
            }
            int coeff = (oldPosition == Enums.CardPosition.Atk) ? -1 : 1;
            crtRotation.y += coeff * 90;
            enemyMonsterField[index].transform.localEulerAngles = crtRotation;
        }
        else
        {
            crtRotation = monsterField[index].transform.localEulerAngles;
            if (oldFace == Enums.CardFace.Down)
            {
                crtRotation.x += 180;
            }
            int coeff = (oldPosition == Enums.CardPosition.Atk) ? -1 : 1;
            crtRotation.y += coeff * 90;
            monsterField[index].transform.localEulerAngles = crtRotation;
        }
    }

    public void SwitchEnemyMonsterPosition(int index, string cardNumber, Enums.CardFace oldFace, Enums.CardPosition oldPos)
    {
        GameObject crtMonster = enemyMonsterField[index];
        if(crtMonster.GetComponent<EnemyFieldCardScript>().GetCardInfo() == null)
        {
            Card cardInfo = Config.Get().GetCardInfoByNumber(cardNumber, true);
            crtMonster.GetComponent<EnemyFieldCardScript>().SetCardInfo(index, cardInfo);
        }

        SwitchMonsterPosition(true, index, oldFace, oldPos);
    }

    public int GetNoAttackableMonsters()
    {
        return attackableMonsters.Count;
    }

    public void ProcessAttackableMonsters(bool value)
    {
        for(int i = 0; i < attackableMonsters.Count; i++)
        {
            enemyMonsterField[attackableMonsters[i]].GetComponent<EnemyFieldCardScript>().SetHighlightable(value);
        }
    }

    public void AddAttackSword(bool isEnemy, int index)
    {
        GameObject fieldCard;
        if (isEnemy)
        {
            fieldCard = enemyMonsterField[index];
        }
        else
        {
            fieldCard = monsterField[index];
        }

        attackSwordInstance = Instantiate<GameObject>(
            attackSword,
            fieldCard.transform.position + new Vector3(0, 0.2f, 0),
            Quaternion.Euler(fieldCard.transform.localEulerAngles + new Vector3(90, 0, 90)),
            fieldCard.transform);

        attackSwordInstance.transform.SetParent(fieldCard.transform);
    }

    public void DestroySword()
    {
        Destroy(attackSwordInstance);
        attackSwordInstance = null;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FieldScript : MonoBehaviour {

    public GameObject tributeCircle, attackSword;
    public List<GameObject> monsterField, spellField;
    public List<GameObject> enemyMonsterField, enemySpellField;

    private List<int> attackableMonsters;

    private Dictionary<string, int> fieldEffects;
    private List<GameObject> tributeCircleInstances;
    private GameObject attackSwordInstance;

    private void Awake()
    {
        tributeCircleInstances = new List<GameObject>();
        attackableMonsters = new List<int>();

        //TODO: delete this if not needed anymore - testing purpose only
        //MockDataForTesting();
    }

    private void MockDataForTesting()
    {
        Monster testCardInfo = new Monster("47060154", File.ReadAllBytes("Assets/Resources/Images/Card Images/MysticClown.png"),
            "Mystic Clown", "blahblahblah", 0, 9, 1, 1500, 1000, 4, false);

        Monster testCardInfo2 = new Monster("15025844", File.ReadAllBytes("Assets/Resources/Images/Card Images/MysticalElf.png"),
            "Mystical Elf", "blahblahblah", 0, 5, 19, 800, 2000, 4, false);
        testCardInfo2.SetIsFlippable(true);

        SetEnemyMonster(0, testCardInfo, Enums.CardFace.Up);
        SetEnemyMonster(1, testCardInfo2, Enums.CardFace.Down);

        NonMonster testCardInfo3 = new NonMonster("17092736", File.ReadAllBytes("Assets/Resources/Images/Card Images/AncientTelescope.png"),
            "Ancient Telescope", "sdasdadjhksa",  0, 1);

        SetEnemySpell(0, null, Enums.CardFace.Down);
        SetEnemySpell(1, testCardInfo3, Enums.CardFace.Down);
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

    public void SetMonster(int index, string cardNumber, Enums.CardFace face)
    {
        Debug.Log("Setting monster on field on position " + index);

        GameObject crtMonster = monsterField[index];
        crtMonster.GetComponent<PlayerFieldCardScript>().SetCardInformation(Config.Get().GetCardInfoByNumber(cardNumber, true));

        SetMonsterCardRotation(crtMonster, face);
        ApplyTexture(crtMonster, cardNumber, Enums.CardType.Monster);
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

    public void FlipMonster(int index, bool isEnemy)
    {
        GameObject crtMonster;

        if(isEnemy)
        {
            crtMonster = enemyMonsterField[index];
        }
        else
        {
            crtMonster = monsterField[index];
        }

        crtMonster.transform.localEulerAngles += new Vector3(180, 0, 0);
        crtMonster.GetComponent<EnemyFieldCardScript>().SetFace(Enums.CardFace.Up);
    }

    public void SetSpell(int index, NonMonster cardInfo, Enums.CardFace face)
    {
        Debug.Log("Setting spell on field on position " + index);

        GameObject crtSpell = spellField[index];
        crtSpell.GetComponent<PlayerFieldCardScript>().SetCardInformation(cardInfo);

        SetSpellCardRotation(crtSpell, face);
        ApplyTexture(crtSpell, cardInfo.GetCardNumber(), (Enums.CardType)Enum.Parse(typeof(Enums.CardType), cardInfo.GetSpellType().ToString()));
    }

    private void SetSpellCardRotation(GameObject crtSpell, Enums.CardFace face)
    {
        if (face == Enums.CardFace.Up)
        {
            crtSpell.transform.localEulerAngles += new Vector3(180, 0, 0);
        }
        
        crtSpell.SetActive(true);
    }

    public void FlipSpell(int index, bool isEnemy)
    {
        GameObject crtSpell;
        if (isEnemy)
        {
            crtSpell = enemySpellField[index];
        }
        else
        {
            crtSpell = spellField[index];
        }

        crtSpell.transform.localEulerAngles += new Vector3(180, 0, 0);
        if (isEnemy)
        {
            crtSpell.GetComponent<EnemyFieldCardScript>().SetFace(Enums.CardFace.Up);
        }
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
        
        //newTributeCircle.transform.SetParent(fieldCard.transform);

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

    public void DestroyFieldMonsters(bool isEnemy, List<int> fieldIndices)
    {
        for (int index = 0; index < fieldIndices.Count; index++)
        {
            if (isEnemy)
            {
                enemyMonsterField[fieldIndices[index]].SetActive(false);
                enemyMonsterField[fieldIndices[index]].transform.localEulerAngles = new Vector3(0, 0, 0);
                attackableMonsters.Remove(fieldIndices[index]);
            }
            else
            {
                monsterField[fieldIndices[index]].SetActive(false);
                monsterField[fieldIndices[index]].transform.localEulerAngles = new Vector3(0, 90, 0);
            }
        }
    }

    public void DestroyFieldSpells(bool isEnemy, List<int> fieldIndices)
    {
        for (int index = 0; index < fieldIndices.Count; index++)
        {
            if (isEnemy)
            {
                enemySpellField[fieldIndices[index]].SetActive(false);
                enemySpellField[fieldIndices[index]].transform.localEulerAngles = new Vector3(0, 0, 0);
            }
            else
            {
                spellField[fieldIndices[index]].SetActive(false);
                spellField[fieldIndices[index]].transform.localEulerAngles = new Vector3(0, 90, 0);
            }
        }
    }

    public void SetEnemyMonster(int index, Card cardInfo, Enums.CardFace face)
    {
        GameObject crtMonster = enemyMonsterField[index];
        crtMonster.GetComponent<EnemyFieldCardScript>().SetCardProperties(index, cardInfo, face);
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
        crtSpell.GetComponent<EnemyFieldCardScript>().SetCardProperties(index, cardInfo, face);
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
            Enums.CardFace newFace = oldFace == Enums.CardFace.Up ? Enums.CardFace.Down : Enums.CardFace.Up;
            crtMonster.GetComponent<EnemyFieldCardScript>().SetCardProperties(index, cardInfo, newFace);
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

        //attackSwordInstance.transform.SetParent(fieldCard.transform);
    }

    public void DestroySword()
    {
        if (attackSwordInstance != null)
        {
            Destroy(attackSwordInstance);
            attackSwordInstance = null;
        }
    }

    public Card GetEnemyCardInfo(int index, bool isMonster)
    {
        if(isMonster)
        {
            return enemyMonsterField[index].GetComponent<EnemyFieldCardScript>().GetCardInfo();
        }
        return enemySpellField[index].GetComponent<EnemyFieldCardScript>().GetCardInfo();
    }

    public void ProcessSelectableMonstersOnField(int attribute, int type, int superiorAtkLimit, bool highlight)
    {
        for (int index = 0; index < enemyMonsterField.Count; index++)
        {
            Monster monsterInfo = (Monster) GetEnemyCardInfo(index, true);

            if(monsterInfo != null
                && (attribute == Constants.DUMMY_INEXISTENT_ID || attribute == monsterInfo.GetAttribute())
                && (type == Constants.DUMMY_INEXISTENT_ID || type == monsterInfo.GetMonsterType())
                && (superiorAtkLimit == 0 || monsterInfo.GetAttackPoints() <= superiorAtkLimit)
                && highlight)
            {
                enemyMonsterField[index].GetComponent<EnemyFieldCardScript>().SetHighlightable(true);
            }

            if(!highlight)
            {
                enemyMonsterField[index].GetComponent<EnemyFieldCardScript>().SetHighlightable(false);
            }
        }
    }

    public void ShowEnemySelection(List<int> indices, bool isMonster, bool selectedByEnemy)
    {
        foreach (int index in indices)
        {
            if(isMonster)
            {
                enemyMonsterField[index].GetComponent<EnemyFieldCardScript>().SetEnemySelection(selectedByEnemy);
            } else
            {
                enemySpellField[index].GetComponent<EnemyFieldCardScript>().SetEnemySelection(selectedByEnemy);
            }
        }
    }

    public void DeselectAllFieldCards()
    {
        EnemyFieldCardScript script;
        for (int index = 0; index < enemyMonsterField.Count; index++)
        {
            script = enemyMonsterField[index].GetComponent<EnemyFieldCardScript>();
            script.SetEnemySelection(false);
            script.UnhighlightObject();
        }
        for (int index = 0; index < enemySpellField.Count; index++)
        {
            script = enemySpellField[index].GetComponent<EnemyFieldCardScript>();
            script.SetEnemySelection(false);
            script.UnhighlightObject();
        }
    }
}

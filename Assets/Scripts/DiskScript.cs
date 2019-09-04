using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskScript : MonoBehaviour {

    public List<GameObject> monstersOnDisk;
    public List<GameObject> spellsOnDisk;

    private bool IsMonsterHighlightable(int index)
    {
        return monstersOnDisk[index].GetComponent<DeskCardScript>().IsHighlightable();
    }

    public void HighlightMonster(int index)
    {
        monstersOnDisk[index].GetComponent<DeskCardScript>().SetHighlightable(true);
    }

    public void UnhighlightMonster(int index)
    {
        monstersOnDisk[index].GetComponent<DeskCardScript>().SetHighlightable(false);
    }

    public void HighlightSpell(int index)
    {
        spellsOnDisk[index].GetComponent<DeskCardScript>().SetHighlightable(true);
    }

    public void UnhighlightSpell(int index)
    {
        spellsOnDisk[index].GetComponent<DeskCardScript>().SetHighlightable(false);
    }

    public void SetMonster(int index, Enums.CardFace face, string cardNumber)
    {
        monstersOnDisk[index].GetComponent<DeskCardScript>().SetData(index, Enums.CardType.Monster, face, cardNumber);
        monstersOnDisk[index].SetActive(true);

        string action = (face == Enums.CardFace.Up) ? Constants.SUMMONING_TEXT : Constants.SETTING_TEXT;

        string details = action + ";" + Constants.MONSTER + ";" + Constants.HAND + ";" + cardNumber;
        GameManager.Get().SendInformation(details);
    }

    public void SetSpell(int index, string cardNumber, Enums.CardType spellType, Enums.CardFace face)
    {
        spellsOnDisk[index].GetComponent<DeskCardScript>().SetData(index, spellType, face, cardNumber);
        spellsOnDisk[index].SetActive(true);
        if(spellType == Enums.CardType.Spell)
        {
            HighlightSpell(index);
        }

        string action = (face == Enums.CardFace.Up) ? Constants.ACTIVATING_TEXT : Constants.SETTING_TEXT;

        string details = action + ";" + Constants.SPELL + ";" + Constants.HAND + ";" + cardNumber;
        GameManager.Get().SendInformation(details);
    }

    public Enums.CardPosition GetPositionForIndex(int index)
    {
        return index > 4 ? Enums.CardPosition.Def : monstersOnDisk[index].GetComponent<DeskCardScript>().GetPosition(); ;
    }

    public void SwitchAttackModeForIndex(int index, bool isAttackMode)
    {
        monstersOnDisk[index].GetComponent<DeskCardScript>().SetBattlingMonster(isAttackMode);
    }

    public void RefreshVariablesForIndex(int index)
    {
        monstersOnDisk[index].GetComponent<DeskCardScript>().RefreshTurnRestrictions();
    }

    public Enums.CardType GetTypeForIndex(int index)
    {
        return spellsOnDisk[index].GetComponent<DeskCardScript>().GetCardType();
    }

    public bool CanChangePositionForIndex(int index)
    {
        return monstersOnDisk[index].GetComponent<DeskCardScript>().CanChangePositionThisTurn();
    }

    public void ChangeTextForIndex(int index, bool monster)
    {
        if(monster)
        {
            monstersOnDisk[index].GetComponent<DeskCardScript>().ChangeText();
        }
        else
        {
            spellsOnDisk[index].GetComponent<DeskCardScript>().ChangeText();
        }
    }

    public int GetDiskMonstersCount()
    {
        return monstersOnDisk.Count;
    }

    public int GetDiskSpellsCount()
    {
        return spellsOnDisk.Count;
    }
}

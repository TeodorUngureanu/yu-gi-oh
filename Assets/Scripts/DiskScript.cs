using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskScript : MonoBehaviour {

    public List<GameObject> monstersOnDisk;
    public List<GameObject> spellsOnDisk;

    private bool IsMonsterHighlightable(int index)
    {
        return monstersOnDisk[index].GetComponent<DiskCardScript>().IsHighlightable();
    }

    public void HighlightMonster(int index)
    {
        monstersOnDisk[index].GetComponent<DiskCardScript>().SetHighlightable(true);
    }

    public void UnhighlightMonster(int index)
    {
        monstersOnDisk[index].GetComponent<DiskCardScript>().SetHighlightable(false);
    }

    public void HighlightSpell(int index)
    {
        spellsOnDisk[index].GetComponent<DiskCardScript>().SetHighlightable(true);
    }

    public void UnhighlightSpell(int index)
    {
        spellsOnDisk[index].GetComponent<DiskCardScript>().SetHighlightable(false);
    }

    public void SetMonster(int index, Enums.CardFace face, string cardNumber, List<int> tributes)
    {
        monstersOnDisk[index].GetComponent<DiskCardScript>().SetData(index, Enums.CardType.Monster, face, cardNumber);
        monstersOnDisk[index].SetActive(true);


        string cardNumberToSend = (face == Enums.CardFace.Up) ? cardNumber : Constants.UNKNOWN;
        string action = (face == Enums.CardFace.Up) ? Constants.SUMMONING_TEXT : Constants.SETTING_TEXT;

        string details = action + ";" + Constants.MONSTER + ";" + index + ";" + cardNumberToSend + ";" +  tributes.Count;
        for (int i = 0; i < tributes.Count; i++)
        {
            details += ";" + tributes[i];
        }
        GameManager.Get().SendInformation(details);
    }

    public void SetSpell(int index, string cardNumber, Enums.CardType spellType, Enums.CardFace face)
    {
        spellsOnDisk[index].GetComponent<DiskCardScript>().SetData(index, spellType, face, cardNumber);
        spellsOnDisk[index].SetActive(true);
        if(spellType == Enums.CardType.Spell)
        {
            HighlightSpell(index);
        }

        string action = (face == Enums.CardFace.Up) ? Constants.ACTIVATING_TEXT : Constants.SETTING_TEXT;

        string details = action + ";" + Constants.SPELL + ";" + Constants.HAND + ";" + index + ";" + cardNumber;
        GameManager.Get().SendInformation(details);
    }

    public Enums.CardPosition GetPositionForIndex(int index)
    {
        return index > 4 ? Enums.CardPosition.Def : monstersOnDisk[index].GetComponent<DiskCardScript>().GetPosition(); ;
    }

    public void SwitchAttackModeForIndex(int index, bool isAttackMode)
    {
        monstersOnDisk[index].GetComponent<DiskCardScript>().SetBattlingMonster(isAttackMode);
    }

    public void RefreshVariablesForIndex(int index)
    {
        monstersOnDisk[index].GetComponent<DiskCardScript>().RefreshTurnRestrictions();
    }

    public Enums.CardType GetTypeForIndex(int index)
    {
        return spellsOnDisk[index].GetComponent<DiskCardScript>().GetCardType();
    }

    public bool CanChangePositionForIndex(int index)
    {
        return monstersOnDisk[index].GetComponent<DiskCardScript>().CanChangePositionThisTurn();
    }

    public void ChangeTextForIndex(int index, bool monster)
    {
        if(monster)
        {
            monstersOnDisk[index].GetComponent<DiskCardScript>().ChangeText();
        }
        else
        {
            spellsOnDisk[index].GetComponent<DiskCardScript>().ChangeText();
        }
    }

    public void DestroyMonster(int index)
    {
        monstersOnDisk[index].SetActive(false);
        monstersOnDisk[index].GetComponent<DiskCardScript>().ResetData();
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

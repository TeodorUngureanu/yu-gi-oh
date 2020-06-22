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

    public void SetMonster(int index, Enums.CardFace face, Monster cardInfo)
    {
        monstersOnDisk[index].GetComponent<DiskCardScript>().SetData(index, Enums.CardType.Monster, face, cardInfo);
        monstersOnDisk[index].SetActive(true);
    }

    public void FlipMonster(int index)
    {
        monstersOnDisk[index].GetComponent<DiskCardScript>().Flip();

    public void SwitchMonsterPosition(int index)
    {
        monstersOnDisk[index].GetComponent<DiskCardScript>().SwitchPosition();
    }

    public void SetSpell(int index, NonMonster cardInfo, Enums.CardType spellType, Enums.CardFace face)
    {
        spellsOnDisk[index].GetComponent<DiskCardScript>().SetData(index, spellType, face, cardInfo);
        spellsOnDisk[index].SetActive(true);
        if(spellType == Enums.CardType.Spell)
        {
            HighlightSpell(index);
        }
    }

    public Enums.CardPosition GetPositionForIndex(int index)
    {
        return index > 4 ? Enums.CardPosition.Def : monstersOnDisk[index].GetComponent<DiskCardScript>().GetPosition(); ;
    }

    public Enums.CardFace GetFaceForIndex(int index)
    {
        return index > 4 ? Enums.CardFace.Up : monstersOnDisk[index].GetComponent<DiskCardScript>().GetFace();
    }

    public bool SwitchAttackModeForIndex(int index, bool isAttackMode)
    {
        DiskCardScript script = monstersOnDisk[index].GetComponent<DiskCardScript>();
        if(script.HasAttackedThisTurn())
        {
            return false;
        }

        script.SetBattlingMonster(isAttackMode);
        return true;
    }

    public void SwitchEnemySelectionForIndex(int index, bool isMonster, bool selectedByEnemy)
    {
        DiskCardScript script;
        if(isMonster)
        {
            script = monstersOnDisk[index].GetComponent<DiskCardScript>();
        } else
        {
            script = spellsOnDisk[index].GetComponent<DiskCardScript>();
        }
        
        script.SetEnemySelection(selectedByEnemy);
    }

    public void DeselectAllDiskCards()
    {
        for(int index = 0; index < monstersOnDisk.Count; index++)
        {
            SwitchEnemySelectionForIndex(index, true, false);
        }
        for (int index = 0; index < spellsOnDisk.Count; index++)
        {
            SwitchEnemySelectionForIndex(index, false, false);
        }
    }

    public void RefreshVariablesForIndex(int index)
    {
        monstersOnDisk[index].GetComponent<DiskCardScript>().RefreshTurnRestrictions();
    }

    public Enums.CardType GetTypeForIndex(int index)
    {
        return spellsOnDisk[index].GetComponent<DiskCardScript>().GetCardType();
    }

    public bool IsSpellActivated(int index)
    {
        return spellsOnDisk[index].GetComponent<DiskCardScript>().IsActivatedSpell();
    }

    public bool CanChangePositionForIndex(int index)
    {
        return monstersOnDisk[index].GetComponent<DiskCardScript>().CanChangePositionThisTurn();
    }

    public void ChangeTextForIndex(int index, bool isMonster)
    {
        if(isMonster)
        {
            monstersOnDisk[index].GetComponent<DiskCardScript>().ChangeText();
        }
        else
        {
            spellsOnDisk[index].GetComponent<DiskCardScript>().ChangeText();
        }
    }

    public void SwitchSelectionModeForIndex(int index, bool isMonster, bool shouldShow)
    {
        if (isMonster)
        {
            monstersOnDisk[index].GetComponent<DiskCardScript>().SwitchSelectionMode(shouldShow);
        }
        else
        {
            spellsOnDisk[index].GetComponent<DiskCardScript>().SwitchSelectionMode(shouldShow);
        }
    }

    public void DestroyMonster(int index)
    {
        monstersOnDisk[index].SetActive(false);
        monstersOnDisk[index].GetComponent<DiskCardScript>().ResetData();
    }

    public void DestroySpell(int index)
    {
        spellsOnDisk[index].SetActive(false);
        spellsOnDisk[index].GetComponent<DiskCardScript>().ResetData();
    }

    public int GetDiskMonstersCount()
    {
        return monstersOnDisk.Count;
    }

    public int GetDiskSpellsCount()
    {
        return spellsOnDisk.Count;
    }

    public void ApplyRestrictionsForAttackingMonster(int index)
    {
        monstersOnDisk[index].GetComponent<DiskCardScript>().ApplyPostAttackRestrictions();
    }
}

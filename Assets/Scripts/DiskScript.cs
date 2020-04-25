using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DiskScript : MonoBehaviour {

    public GameObject diskInformation;
    private DiskInfoScript diskInfoScript;

    public List<GameObject> monstersOnDisk;
    public List<GameObject> spellsOnDisk;

    private void Awake()
    {
        diskInfoScript = diskInformation.GetComponent<DiskInfoScript>();
    }

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

    public void SetMonster(int index, Enums.CardFace face, Card cardInfo, List<int> tributes)
    {
        monstersOnDisk[index].GetComponent<DiskCardScript>().SetData(index, Enums.CardType.Monster, face, cardInfo);
        monstersOnDisk[index].SetActive(true);

        string cardNumberToSend = (face == Enums.CardFace.Up) ? cardInfo.GetCardNumber() : Constants.UNKNOWN;
        string action = (face == Enums.CardFace.Up) ? Constants.SUMMONING_TEXT : Constants.SETTING_TEXT;
        string tributeIndices = string.Join(";", tributes.Select(i => i.ToString()).ToArray());
        
        List<MessageParameter> parameters = new List<MessageParameter>()
        {
            new MessageParameter(Constants.CARD_NO_KEY, cardNumberToSend),
            new MessageParameter(Constants.TRIBUTE_NO_KEY, tributes.Count.ToString()),
            new MessageParameter(Constants.TRIBUTE_INDICES_KEY, tributeIndices)
        };
        GameManager.Get().SendInformation(action, index, parameters);
    }

    public void SetSpell(int index, Card cardInfo, Enums.CardType spellType, Enums.CardFace face)
    {
        spellsOnDisk[index].GetComponent<DiskCardScript>().SetData(index, spellType, face, cardInfo);
        spellsOnDisk[index].SetActive(true);
        if(spellType == Enums.CardType.Spell)
        {
            HighlightSpell(index);
        }

        string action = (face == Enums.CardFace.Up) ? Constants.ACTIVATING_TEXT : Constants.SETTING_TEXT;
        List<MessageParameter> parameters = new List<MessageParameter>()
        {
            new MessageParameter(Constants.ORIGIN_KEY, Constants.HAND),
            new MessageParameter(Constants.CARD_NO_KEY, cardInfo.GetCardNumber())
        };

        GameManager.Get().SendInformation(action, index, parameters);
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

    public void UpdateLPOnDisk(long newLP)
    {
        diskInfoScript.ChangeLPText(newLP);
    }

    public void UpdateDeckSizeOnDisk(int newDeckSize)
    {
        diskInfoScript.ChangeDeckSizeText(newDeckSize);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskScript : MonoBehaviour {

    public List<GameObject> defMonstersOnDisk;
    public List<GameObject> atkMonstersOnDisk;
    public List<string> activePositions = new List<string> { "DEF", "DEF", "DEF", "DEF", "DEF" };
    public List<GameObject> spellsOnDisk;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private bool IsMonsterHighlighted(int index)
    {
        if (activePositions[index] == "DEF")
        {
            return defMonstersOnDisk[index].GetComponent<CardScript>().IsHighlight();
        }
        else
        {
            return atkMonstersOnDisk[index].GetComponent<CardScript>().IsHighlight();
        }
    }

    public void HighlightMonster(int index)
    {
        if (activePositions[index] == "DEF")
        {
            defMonstersOnDisk[index].GetComponent<CardScript>().SetHighlight(true);
        }
        else
        {
            atkMonstersOnDisk[index].GetComponent<CardScript>().SetHighlight(true);
        }
    }

    public void UnhighlightMonster(int index)
    {
        if (activePositions[index] == "DEF")
        {
            defMonstersOnDisk[index].GetComponent<CardScript>().SetHighlight(false);
        }
        else
        {
            atkMonstersOnDisk[index].GetComponent<CardScript>().SetHighlight(false);
        }
    }

    public void HighlightSpell(int index)
    {
        spellsOnDisk[index].GetComponent<CardScript>().SetHighlight(true);
    }

    public void UnhighlightSpell(int index)
    {
        spellsOnDisk[index].GetComponent<CardScript>().SetHighlight(false);
    }


    public void SetMonster(int index, string face, string cardName)
    {
        Debug.Log("Setting monster on position " + index);

        defMonstersOnDisk[index].GetComponent<CardScript>().SetData(CardScript.Location.DISK, index, "Monster", cardName);
        defMonstersOnDisk[index].GetComponent<CardScript>().SetFace(face);
        atkMonstersOnDisk[index].GetComponent<CardScript>().SetData(CardScript.Location.DISK, index, "Monster", cardName);
        atkMonstersOnDisk[index].GetComponent<CardScript>().SetFace(face);

        //Debug.Log("Set on disk already");

        ChangeMonsterPosition(index, face);
    }

    public void ChangeMonsterPosition(int index, string face)
    {
        bool highlight = IsMonsterHighlighted(index);
        if (highlight)
        {
            UnhighlightMonster(index);
            //Debug.Log("Unhighlighted monster");
        }

        ActivateCardPosition(index, face == "UP");

        activePositions[index] = (face == "UP") ? "ATK" : "DEF";
        if (highlight)
        {
            HighlightMonster(index);
        }
    }

    private void ActivateCardPosition(int index, bool isAttack)
    {
        atkMonstersOnDisk[index].SetActive(isAttack);
        defMonstersOnDisk[index].SetActive(!isAttack);
    }

    public void SetSpell(int index, string cardName, string spellType, string face)
    {
        Debug.Log("Setting spell on position " + index);
        spellsOnDisk[index].GetComponent<CardScript>().SetData(CardScript.Location.DISK, index, spellType, cardName);
        spellsOnDisk[index].GetComponent<CardScript>().SetFace(face);
        spellsOnDisk[index].SetActive(true);

        if(face == "DOWN" && spellType != "Trap")
        {
            spellsOnDisk[index].GetComponent<CardScript>().SetHighlight(true);
        }
    }

    public string GetPositionForIndex(int index)
    {
        return index > 4 ? "DEF" : activePositions[index];
    }

    public void SwitchAttackModeForIndex(int index)
    {
        atkMonstersOnDisk[index].GetComponent<CardScript>().SwitchAttackMode();
        atkMonstersOnDisk[index].GetComponent<CardScript>().ChangeText();
    }

    public void RefreshVariablesForIndex(int index)
    {
        if (activePositions[index] == "DEF")
        {
            defMonstersOnDisk[index].GetComponent<CardScript>().RefreshTurnRestrictions();
        }
        else
        {
            atkMonstersOnDisk[index].GetComponent<CardScript>().RefreshTurnRestrictions();
        }
    }

    public string GetTypeForIndex(int index)
    {
        return spellsOnDisk[index].GetComponent<CardScript>().GetCardType();
    }

    public bool HasPositionBeenChangedForIndex(int index)
    {
        if (activePositions[index] == "DEF")
        {
            return defMonstersOnDisk[index].GetComponent<CardScript>().HasPositionBeenChanged();
        }
        else
        {
            return atkMonstersOnDisk[index].GetComponent<CardScript>().HasPositionBeenChanged();
        }
    }
}

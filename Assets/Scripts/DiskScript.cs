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

    public void SetMonster(int index, string position, string cardName)
    {
        Debug.Log("Setting monster on position " + index);

        defMonstersOnDisk[index].GetComponent<CardScript>().SetData(CardScript.Location.DISK, index, true, cardName);
        atkMonstersOnDisk[index].GetComponent<CardScript>().SetData(CardScript.Location.DISK, index, true, cardName);

        Debug.Log("Set on disk already");

        ChangeMonsterPosition(index, position);
    }

    public void ChangeMonsterPosition(int index, string position)
    {
        UnhighlightMonster(index);

        Debug.Log("Unhighlighted monster");

        ActivateCardPosition(index, position == "ATK");

        activePositions[index] = position;
        HighlightMonster(index);
    }

    private void ActivateCardPosition(int index, bool isAttack)
    {
        atkMonstersOnDisk[index].SetActive(isAttack);
        defMonstersOnDisk[index].SetActive(!isAttack);
    }

    public void SetSpell(int index, string cardName)
    {
        Debug.Log("Setting spell on position " + index);
        spellsOnDisk[index].GetComponent<CardScript>().SetData(CardScript.Location.DISK, index, false, cardName);
        spellsOnDisk[index].SetActive(true);
    }
}

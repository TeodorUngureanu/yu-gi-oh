using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskScript : MonoBehaviour {

    public List<GameObject> monstersOnDisk;
    public List<GameObject> spellsOnDisk;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void HighlightMonster(int index)
    {
        monstersOnDisk[index].GetComponent<CardScript>().SetHighlight(true);
    }

    public void UnhighlightMonster(int index)
    {
        monstersOnDisk[index].GetComponent<CardScript>().SetHighlight(false);
    }

    public void HighlightSpell(int index)
    {
        spellsOnDisk[index].GetComponent<CardScript>().SetHighlight(true);
    }

    public void UnhighlightSpell(int index)
    {
        spellsOnDisk[index].GetComponent<CardScript>().SetHighlight(false);
    }

    public void SetMonster(int index)
    {
        Debug.Log("Setting monster on position " + index);
        monstersOnDisk[index].GetComponent<CardScript>().SetData(CardScript.Location.DISK, index, true);
        monstersOnDisk[index].SetActive(true);
    }

    public void SetSpell(int index)
    {
        Debug.Log("Setting spell on position " + index);
        spellsOnDisk[index].GetComponent<CardScript>().SetData(CardScript.Location.DISK, index, false);
        spellsOnDisk[index].SetActive(true);
    }
}

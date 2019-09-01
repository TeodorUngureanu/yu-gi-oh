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
            return defMonstersOnDisk[index].GetComponent<CardScript>().IsHighlightable();
        }
        else
        {
            return atkMonstersOnDisk[index].GetComponent<CardScript>().IsHighlightable();
        }
    }

    public void HighlightMonster(int index)
    {
        if (activePositions[index] == "DEF")
        {
            defMonstersOnDisk[index].GetComponent<CardScript>().SetHighlightable(true);
        }
        else
        {
            atkMonstersOnDisk[index].GetComponent<CardScript>().SetHighlightable(true);
        }
    }

    public void UnhighlightMonster(int index)
    {
        if (activePositions[index] == "DEF")
        {
            defMonstersOnDisk[index].GetComponent<CardScript>().SetHighlightable(false);
        }
        else
        {
            atkMonstersOnDisk[index].GetComponent<CardScript>().SetHighlightable(false);
        }
    }

    public void HighlightSpell(int index)
    {
        spellsOnDisk[index].GetComponent<CardScript>().SetHighlightable(true);
    }

    public void UnhighlightSpell(int index)
    {
        spellsOnDisk[index].GetComponent<CardScript>().SetHighlightable(false);
    }


    public void SetMonster(int index, Enums.CardFace face, string cardNumber)
    {
        Debug.Log("Setting monster on disk index " + index);

        defMonstersOnDisk[index].GetComponent<CardScript>().SetData(index, Enums.CardType.Monster, cardNumber);
        defMonstersOnDisk[index].GetComponent<CardScript>().SetFace(face);
        atkMonstersOnDisk[index].GetComponent<CardScript>().SetData(index, Enums.CardType.Monster, cardNumber);
        atkMonstersOnDisk[index].GetComponent<CardScript>().SetFace(face);

        ActivateCardPosition(index, face == Enums.CardFace.Up);
        activePositions[index] = (face == Enums.CardFace.Up) ? "ATK" : "DEF";
        

        //if (face == "DOWN")
        //{
        //    GameObject defCard = defMonstersOnDisk[index];
        //    Vector3 crtRotation = defCard.gameObject.transform.localEulerAngles;
        //    crtRotation.x = 0;
        //    defCard.gameObject.transform.localEulerAngles = crtRotation;
        //}
    }

    public void ChangeMonsterPosition(int index, string newPosition)
    {
        bool highlight = IsMonsterHighlighted(index);
        if (highlight)
        {
            UnhighlightMonster(index);
        }
        
        ActivateCardPosition(index, newPosition == "ATK");
        activePositions[index] = newPosition;
    }

    private void ActivateCardPosition(int index, bool isAttack)
    {
        atkMonstersOnDisk[index].SetActive(isAttack);
        defMonstersOnDisk[index].SetActive(!isAttack);
    }

    public void SetSpell(int index, string cardNumber, Enums.CardType spellType, Enums.CardFace face)
    {
        Debug.Log("Setting spell on position " + index);
        spellsOnDisk[index].GetComponent<CardScript>().SetData(index, spellType, cardNumber);
        spellsOnDisk[index].GetComponent<CardScript>().SetFace(face);
        spellsOnDisk[index].SetActive(true);

        if(face == Enums.CardFace.Down && spellType != Enums.CardType.Trap)
        {
            spellsOnDisk[index].GetComponent<CardScript>().SetHighlightable(true);
        }
    }

    public string GetPositionForIndex(int index)
    {
        return index > 4 ? "DEF" : activePositions[index];
    }

    public void SwitchAttackModeForIndex(int index)
    {
        atkMonstersOnDisk[index].GetComponent<CardScript>().SetBattlingMonster();
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

    public Enums.CardType GetTypeForIndex(int index)
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

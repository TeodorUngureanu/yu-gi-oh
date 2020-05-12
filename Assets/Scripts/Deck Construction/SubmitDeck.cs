using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubmitDeck : InteractibleElementScript
{
    public int DeckNumber;
    protected override void Awake()
    {
        base.Awake();
        objRenderer = GetComponent<Renderer>();
    }

    public override void InteractWithElement()
    {
        GameObject.Find("DeckConstructionManager").GetComponent<DeckConstructionManager>().SaveDeckToDB(DeckNumber);
    }

    void OnMouseEnter()
    {
        Debug.Log("OnMouseEnter");
        HighlightObject();

        GetComponent<Image>().color = Color.green;
        GetComponentInChildren<Text>().color = Color.white;
    }

    void OnMouseExit()
    {
        Debug.Log("OnMouseExit");
        UnhighlightObject();

        GetComponent<Image>().color = Color.white;
        GetComponentInChildren<Text>().color = Color.black;
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            InteractWithElement();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandCardScript : InteractibleAbstractCard
{
    public GameObject frontImagePlane;
    public Canvas canvas;

    private Enums.CardFace summoningFace;

    public void SetCardIndex(int vCardIndex)
    {
        cardIndex = vCardIndex;
    }

    public void SetData(int vCardIndex, Enums.CardType vCardType, string cardNumber)
    {
        cardIndex = vCardIndex;
        cardType = vCardType;

        objRenderer = GetComponent<Renderer>();

        if (canvas != null)
        {
            canvas.enabled = false;
        }

        SetDefaultFace();

        Texture2D texture = LoadTexture(cardNumber);
        if (texture != null)
        {
            frontImagePlane.GetComponent<Renderer>().material.mainTexture = texture;
        }
    }

    private void SetDefaultFace()
    {
        if (cardType == Enums.CardType.Monster)
        {
            summoningFace = Enums.CardFace.Up;
        }
        else
        {
            summoningFace = Enums.CardFace.Down;
        }
        ChangeText();
    }

    private void SwitchFace()
    {
        if(cardType == Enums.CardType.Trap)
        {
            return;
        }

        if (summoningFace == Enums.CardFace.Up)
        {
            summoningFace = Enums.CardFace.Down;
        }
        else
        {
            summoningFace = Enums.CardFace.Up;
        }
        ChangeText();
    }

    private void SetCanvasText(string newText)
    {
        canvas.GetComponentInChildren<Text>().text = newText;
    }

    public override void ChangeText()
    {
        if (GameManager.Get().IsPlayerDiscarding())
        {
            SetCanvasText(Constants.DISCARDING_TEXT);
            return;
        }
        if (IsMonster())
        {
            SetCanvasText( (summoningFace == Enums.CardFace.Up) ? Constants.SUMMONING_TEXT : Constants.SETTING_TEXT);
        }
        else
        {
            SetCanvasText( (cardType == Enums.CardType.Spell && summoningFace == Enums.CardFace.Up) ? Constants.ACTIVATING_TEXT : Constants.SETTING_TEXT);
        }
    }

    void OnMouseEnter()
    {
        if (highlightable)
        {
            HighlightObject();
            canvas.enabled = true;
        }
    }

    void OnMouseExit()
    {
        if (highlightable)
        {
            UnhighlightObject();
            SetDefaultFace();
            canvas.enabled = false;
        }
    }

    void OnMouseOver()
    {
        if (highlightable)
        {
            if (Input.GetMouseButtonDown(0))
            {
                InteractWithElement();
            }

            if (Input.GetMouseButtonDown(1) && !GameManager.Get().IsPlayerDiscarding())
            {
                SwitchFace();
            }
        }
    }

    public override void InteractWithElement()
    {
        if (GameManager.Get().IsPlayerDiscarding())
        {
            GameManager.Get().DiscardCard(cardIndex);
        }
        else
        {
            if (IsMonster())
            {
                GameManager.Get().PlaceMonsterOnDisk(cardIndex, summoningFace);
            }
            else
            {
                GameManager.Get().PlaceSpellOnDisk(cardIndex, summoningFace);
            }
        }
    }
}

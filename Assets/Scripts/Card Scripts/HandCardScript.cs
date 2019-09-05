using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandCardScript : InteractibleAbstractCard
{
    public GameObject frontImagePlane;
    public Canvas canvas;

    private Enums.CardFace summoningFace;
    private Card cardInfo;

    public void SetCardIndex(int vCardIndex)
    {
        cardIndex = vCardIndex;
    }

    public Card GetCardInfo()
    {
        return cardInfo;
    }

    public void SetData(int vCardIndex, Card vCardInfo)
    {
        cardIndex = vCardIndex;
        cardInfo = vCardInfo;

        cardType = cardInfo.IsMonster() ? Enums.CardType.Monster : (Enums.CardType) Enum.Parse(typeof(Enums.CardType), ((NonMonster)cardInfo).getType().ToString()); ;

        objRenderer = GetComponent<Renderer>();

        if (canvas != null)
        {
            canvas.enabled = false;
        }

        SetDefaultFace();

        Texture2D texture = Utils.LoadTexture(cardInfo.GetCardNumber(), cardType);
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
            if(Utils.NeedsTribute(((Monster)cardInfo).getRarity()) == 0)
            {
                SetCanvasText((summoningFace == Enums.CardFace.Up) ? Constants.SUMMONING_TEXT : Constants.SETTING_TEXT);
            }
            else
            {
                SetCanvasText((summoningFace == Enums.CardFace.Up) ? Constants.TRIBUTE_SUMMON_TEXT : Constants.TRIBUTE_SET_TEXT);
            }
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
                canvas.enabled = false;
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
                GameManager.Get().SummonMonster(cardIndex, cardInfo, summoningFace);
            }
            else
            {
                GameManager.Get().UseSpell(cardIndex, cardInfo, summoningFace);
            }
        }
    }
}

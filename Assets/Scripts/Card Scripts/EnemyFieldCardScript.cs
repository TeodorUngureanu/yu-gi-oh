using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFieldCardScript : InteractibleAbstractCard
{
    private Card cardInfo;

    public Card GetCardInfo()
    {
        return cardInfo;
    }

    public void SetCardInfo(int vCardIndex, Card vCardInfo)
    {
        cardIndex = vCardIndex;
        cardInfo = vCardInfo;
    }

    void OnMouseEnter()
    {
        if (highlightable)
        {
            HighlightObject();
        }
    }

    void OnMouseExit()
    {
        if (highlightable)
        {
            UnhighlightObject();
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
        }
    }

    public override void ChangeText()
    {
        throw new System.NotImplementedException();
    }

    public override void InteractWithElement()
    {
        GameManager.Get().AttackTarget(cardIndex);
    }
}

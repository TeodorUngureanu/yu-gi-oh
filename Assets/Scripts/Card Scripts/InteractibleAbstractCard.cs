using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractibleAbstractCard : InteractibleElementScript {

    protected int cardIndex;
    protected bool highlightable = false;
    protected Enums.CardType cardType;

    public Enums.CardType GetCardType()
    {
        return cardType;
    }

    public bool IsMonster()
    {
        return cardType == Enums.CardType.Monster;
    }

    public bool IsHighlightable()
    {
        return highlightable;
    }

    public virtual void SetHighlightable(bool vHighlightable)
    {
        highlightable = vHighlightable;
        if (!highlightable)
        {
            UnhighlightObject();
        }
    }

    public abstract void ChangeText();
}

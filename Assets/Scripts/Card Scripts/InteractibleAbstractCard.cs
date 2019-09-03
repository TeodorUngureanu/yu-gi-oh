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
    
    public virtual Texture2D LoadTexture(string cardNumber)
    {
        // Texture size does not matter - the size of the texture will be replaced by image size
        Texture2D texture = new Texture2D(2, 2);
        bool isLoaded = false;

        if (cardType == Enums.CardType.Monster)
        {
            foreach (KeyValuePair<int, Monster> monster in Config.Get()._Monster_Cards)
            {
                if (monster.Value.getCardNumber() == cardNumber)
                {
                    byte[] image = monster.Value.getImage();
                    isLoaded = texture.LoadImage(image);

                    break;
                }
            }
        }
        else
        {
            foreach (KeyValuePair<int, NonMonster> nonMonster in Config.Get()._Magic_Cards)
            {
                if (nonMonster.Value.getCardNumber() == cardNumber)
                {
                    byte[] image = nonMonster.Value.getImage();
                    isLoaded = texture.LoadImage(image);

                    break;
                }
            }
        }

        // Apply this texure as per requirement on image or material
        if (isLoaded)
        {
            return texture;
            
        }

        return null;
    }
}

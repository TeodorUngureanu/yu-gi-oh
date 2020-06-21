using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckConstructionCard : InteractibleAbstractCard
{
    public GameObject frontImagePlange;
    public GameObject previewImage;
    public Canvas canvas;
    public Text text;

    private int multiplier = 0;
    private int deckNumber;
    private string cardNumber;

    public override void ChangeText()
    {
        // throw new System.NotImplementedException();
    }

    public override void InteractWithElement()
    {
        multiplier = int.Parse(text.text);

        canvas.gameObject.SetActive(true);

        if (multiplier == 3)
        {
            Debug.Log("Warning message > 3");
        }
        else
        {
            multiplier++;
            text.text = "" + multiplier;
            DeckConstructionManager.Get().AddCardToDeck(cardNumber, deckNumber);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        objRenderer = GetComponent<Renderer>();
    }

    private void Start()
    {
        if (gameObject.name.Length > 11) {
            string[] monsterMagic_cardNumber = gameObject.name.Split('_');
            deckNumber = int.Parse(monsterMagic_cardNumber[0]);
            cardType = (Enums.CardType)int.Parse(monsterMagic_cardNumber[1]);
            cardNumber = monsterMagic_cardNumber[2];
        }
    }

    public void SetObjectRenderer()
    {
        objRenderer = GetComponent<Renderer>();
    }

    void OnMouseEnter()
    {
        HighlightObject();
    }

    void OnMouseExit()
    {
        UnhighlightObject();
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            InteractWithElement();
        }

        if (Input.GetMouseButtonDown(1))
        {
            DecreaseCard();
        }

        if (Input.GetMouseButtonDown(2))
        {
            PreviewImage();
        }
    }

    void DecreaseCard()
    {
        multiplier = int.Parse(text.text);

        if (multiplier > 0)
        {
            multiplier--;
            text.text = "" + multiplier;
            DeckConstructionManager.Get().RemoveCardFromDeck(cardNumber, deckNumber);

            if (multiplier == 0)
            {
                canvas.gameObject.SetActive(false);
            }
        }
    }

    void PreviewImage()
    {
        // Texture size does not matter - the size of the texture will be replaced by image size
        Texture2D texture = new Texture2D(2, 2);
        bool isLoaded = false;

        if (cardType == Enums.CardType.Monster)
        {
            foreach (KeyValuePair<int, Monster> monster in Config.Get()._Monster_Cards)
            {
                if (monster.Value.GetCardNumber() == cardNumber)
                {
                    byte[] image = monster.Value.GetImage();
                    isLoaded = texture.LoadImage(image);

                    break;
                }
            }
        }
        else
        {
            foreach (KeyValuePair<int, NonMonster> nonMonster in Config.Get()._Magic_Cards)
            {
                if (nonMonster.Value.GetCardNumber() == cardNumber)
                {
                    byte[] image = nonMonster.Value.GetImage();
                    isLoaded = texture.LoadImage(image);

                    break;
                }
            }
        }

        // Apply this texure as per requirement on image or material
        if (isLoaded)
        {
            previewImage.GetComponent<Renderer>().material.mainTexture = texture;
        }
    }
}

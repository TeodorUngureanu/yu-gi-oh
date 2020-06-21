using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraveyardCard : InteractibleAbstractCard
{
    public GameObject frontImagePlange;
    public GameObject previewImage;

    private int graveyardIndex;
    private string cardNumber;
    private Enums.CardPosition position;

    public override void ChangeText()
    {
        // throw new System.NotImplementedException();
    }

    public override void InteractWithElement()
    {
        base.HighlightObject();

        GameManager.Get().SetGraveyardSelectionIndex(graveyardIndex, position, GraveyardCanvas.Get().GetPlayerEnemyConstant());
    }

    public void SetGraveyardIndex(int index)
    {
        graveyardIndex = index;
    }

    protected override void Awake()
    {
        base.Awake();
        objRenderer = GetComponent<Renderer>();
    }

    private void Start()
    {
        string[] monsterMagic_cardNumber = gameObject.name.Split('_');

        cardNumber = monsterMagic_cardNumber[0];
        cardType = (Enums.CardType)int.Parse(monsterMagic_cardNumber[1]);
    }

    public void SetObjectRenderer()
    {
        objRenderer = GetComponent<Renderer>();
    }

    void OnMouseEnter()
    {
        // HighlightObject();
    }

    void OnMouseExit()
    {
        // UnhighlightObject();
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            InteractWithElement();
            position = Enums.CardPosition.Atk;
        }

        if (Input.GetMouseButtonDown(1))
        {
            InteractWithElement();
            position = Enums.CardPosition.Def;
        }

        if (Input.GetMouseButtonDown(2))
        {
            PreviewImage();
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

        if ( ! previewImage.activeSelf)
        {
            previewImage.SetActive(true);
        }

        // Apply this texure as per requirement on image or material
        if (isLoaded)
        {
            previewImage.GetComponent<Renderer>().material.mainTexture = texture;
        }
    }
}

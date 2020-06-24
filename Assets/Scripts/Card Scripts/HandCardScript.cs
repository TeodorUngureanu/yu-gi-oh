using System;
using UnityEngine;
using UnityEngine.UI;

public class HandCardScript : InteractibleAbstractCard
{
    public GameObject frontImagePlane;
    public Canvas canvas;

    private Enums.CardFace summoningFace;
    private Card cardInfo;
    private float originalY, maxYmovement = 0.15f;
    private bool isMovingUp = false, isMovingDown = false;

    protected override void Awake()
    {
        base.Awake();
        objRenderer = GetComponent<Renderer>();
        originalY = transform.position.y;
    }

    protected override void Update()
    {
        base.Update();
        if(isMovingUp)
        {
            float currentY = transform.position.y;
            if(currentY >= originalY + maxYmovement)
            {
                Vector3 defaultPos = transform.position;
                defaultPos.y = originalY + maxYmovement;
                transform.position = defaultPos;

                isMovingUp = false;
                return;
            }
            transform.Translate(Vector3.up * Time.deltaTime, Camera.main.transform);
        } else
        if (isMovingDown)
        {
            float currentY = transform.position.y;
            if (currentY <= originalY)
            {
                Vector3 defaultPos = transform.position;
                defaultPos.y = originalY;
                transform.position = defaultPos;

                isMovingDown = false;
                return;
            }
            transform.Translate(Vector3.down * Time.deltaTime, Camera.main.transform);
        }
    }

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
        cardType = cardInfo.IsMonster() ? Enums.CardType.Monster : (Enums.CardType) Enum.Parse(typeof(Enums.CardType), ((NonMonster)cardInfo).GetSpellType().ToString());

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
            if(Utils.NeedsTribute(((Monster)cardInfo).GetRarity()) == 0)
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
        isMovingUp = true;
        UIManager.Get().ShowInformation(cardInfo.GetCardNumber(), cardInfo.IsMonster() ? Enums.CardType.Monster : Enums.CardType.Spell);
        if (highlightable)
        {
            HighlightObject();
            canvas.enabled = true;
        }
    }

    void OnMouseExit()
    {
        isMovingDown = true;
        UIManager.Get().HideInformation();
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
                GameManager.Get().SummonMonster(cardIndex, (Monster) cardInfo, summoningFace);
            }
            else
            {
                GameManager.Get().UseSpell(cardIndex, (NonMonster) cardInfo, summoningFace);
            }
        }
    }
}

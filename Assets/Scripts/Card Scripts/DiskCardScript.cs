using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiskCardScript : InteractibleAbstractCard {
    
    public GameObject frontImagePlane;
    public Canvas canvas;

    private Enums.CardFace face = Enums.CardFace.Up;
    private Enums.CardPosition position = Enums.CardPosition.Atk;
    private bool hasChangedPositionThisTurn = true, hasAttackedThisTurn = false;

    private const int DEF_COEFF = 1, ATK_COEFF = -1;
    private Vector3 posTransitionVector = new Vector3(-0.00044f, 0.0002f, 0.00101f);
    private Vector3 rotTransitionVector = new Vector3(0, 270, 0);

    protected override void Awake()
    {
        base.Awake();
        objRenderer = GetComponent<Renderer>();
    }

    public void SetData(int vCardIndex, Enums.CardType vCardType, Enums.CardFace vFace, string cardNumber)
    {
        cardIndex = vCardIndex;
        cardType = vCardType;

        if(objRenderer == null)
        {
            objRenderer = GetComponent<Renderer>();
        }

        Texture2D texture = Utils.LoadTexture(cardNumber, cardType);
        if (texture != null)
        {
            frontImagePlane.GetComponent<Renderer>().material.mainTexture = texture;
        }

        SetFace(vFace);
        if (IsMonster() && face == Enums.CardFace.Down) {
            SetPosition(Enums.CardPosition.Def);
            RotateCard();
            TweakCardTransform(DEF_COEFF);
        }
        if(!IsMonster() && face == Enums.CardFace.Up)
        {
            RotateCard();
        }
    }
    
    public void ResetData()
    {
        face = Enums.CardFace.Up;
        position = Enums.CardPosition.Atk;
        hasChangedPositionThisTurn = true;
        hasAttackedThisTurn = false;
    }

    public void SetFace(Enums.CardFace newFace)
    {
        face = newFace;
        ChangeText();
    }

    public override void SetHighlightable(bool vHighlightable)
    {
        base.SetHighlightable(vHighlightable);
        if(!vHighlightable)
        {
            canvas.enabled = false;
        }
    }

    private void RotateCard()
    {
        Vector3 crtRotation = this.gameObject.transform.localEulerAngles;
        crtRotation.x += 180;
        this.gameObject.transform.localEulerAngles = crtRotation;
    }

    private void SetCanvasText(string newText)
    {
        canvas.GetComponentInChildren<Text>().text = newText;
    }

    public override void ChangeText()
    {
        if (IsMonster())
        {
            if (GameManager.Get().GetTurnPhase() == Turn.Phase.Battle)
            {
                SetCanvasText(Constants.ATTACKING_TEXT);
                return;
            }
            if(GameManager.Get().IsPlayerSacrificing())
            {
                SetCanvasText(Constants.SACRIFICE_TEXT);
                return;
            }
            if (face == Enums.CardFace.Down)
            {
                SetCanvasText(Constants.FLIPPING_TEXT);
            }
            else
            {
                SetCanvasText((position == Enums.CardPosition.Def) ? Constants.ATK_CHANGE_TEXT : Constants.DEF_CHANGE_TEXT);
            }
        }
        else if(face == Enums.CardFace.Down)
        {
            SetCanvasText(Constants.ACTIVATING_TEXT);
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
        }
    }

    public Enums.CardPosition GetPosition()
    {
        return position;
    }

    private void SetPosition(Enums.CardPosition newPosition)
    {
        position = newPosition;
        ChangeText();
    }

    private void TweakCardTransform(int coefficient)
    {
        Vector3 crtPosition = this.gameObject.transform.localPosition;
        this.gameObject.transform.localPosition = crtPosition + coefficient * posTransitionVector;

        Vector3 crtRotation = this.gameObject.transform.localEulerAngles;
        this.gameObject.transform.localEulerAngles = crtRotation + coefficient * rotTransitionVector;
    }

    private void SwitchPosition()
    {
        SetPosition((position == Enums.CardPosition.Atk) ? Enums.CardPosition.Def : Enums.CardPosition.Atk);
        int coefficient = (position == Enums.CardPosition.Def) ? DEF_COEFF : ATK_COEFF;

        if(IsMonster() && face == Enums.CardFace.Down)
        {
            SetFace(Enums.CardFace.Up);
            RotateCard();
        }

        TweakCardTransform(coefficient);

        UnhighlightObject();
        highlightable = false;
    }

    public override void InteractWithElement()
    {
        if(IsMonster())
        {
            Turn.Phase currentPhase = GameManager.Get().GetTurnPhase();

            if (currentPhase == Turn.Phase.Battle)
            {
                hasAttackedThisTurn = true;
                GameManager.Get().AttackWithMonster(cardIndex);

                //after attack, unhighlight it
                UnhighlightObject();
                highlightable = false;
            }

            if(GameManager.Get().IsPlayerSacrificing())
            {
                GameManager.Get().AddTribute(false, cardIndex);
                UnhighlightObject();
                highlightable = false;
                return;
            }

            if ((currentPhase == Turn.Phase.Main1 || currentPhase == Turn.Phase.Main2) && !hasChangedPositionThisTurn)
            {
                string action = (position == Enums.CardPosition.Atk) ? Constants.ATK_CHANGE_TEXT : Constants.DEF_CHANGE_TEXT;
                string cardNumber = GameManager.Get().GetCardNumberForMonster(cardIndex);

                string details = action + ";" + cardIndex + ";" + cardNumber + ";" + face;
                GameManager.Get().SendInformation(details);

                GameManager.Get().SwitchMonsterPosition(cardIndex, face, position);

                SwitchPosition();

                hasChangedPositionThisTurn = true;
            }
        }
    }

    public void RefreshTurnRestrictions()
    {
        hasChangedPositionThisTurn = false;
        hasAttackedThisTurn = false;
    }

    public bool CanChangePositionThisTurn()
    {
        return !hasChangedPositionThisTurn && !hasAttackedThisTurn;
    }
}

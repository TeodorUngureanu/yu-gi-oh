using UnityEngine;
using UnityEngine.UI;

public class DiskCardScript : InteractibleAbstractCard {
    
    public GameObject frontImagePlane;
    public Canvas canvas;

    private Card cardInfo;
    private Enums.CardFace face = Enums.CardFace.Up;
    private Enums.CardPosition position = Enums.CardPosition.Atk;
    private bool hasChangedPositionThisTurn = true, hasAttackedThisTurn = false;
    private bool isActivatedSpell = false;
    private bool selectionMode = false;

    private const int DEF_COEFF = 1, ATK_COEFF = -1;
    private Vector3 posTransitionVector = new Vector3(-0.00044f, 0.0002f, 0.00101f);
    private Vector3 rotTransitionVector = new Vector3(0, 270, 0);

    protected override void Awake()
    {
        base.Awake();
        objRenderer = GetComponent<Renderer>();
    }

    public void SetData(int vCardIndex, Enums.CardType vCardType, Enums.CardFace vFace, Card vCardInfo)
    {
        cardIndex = vCardIndex;
        cardType = vCardType;
        cardInfo = vCardInfo;

        if (objRenderer == null)
        {
            objRenderer = GetComponent<Renderer>();
        }

        Texture2D texture = Utils.LoadTexture(cardInfo.GetCardNumber(), cardType);
        if (texture != null)
        {
            frontImagePlane.GetComponent<Renderer>().material.mainTexture = texture;
        }

        SetFace(vFace);
        if (IsMonster() && face == Enums.CardFace.Down) {
            SetPosition(Enums.CardPosition.Def);
            RotateCard(false);
            TweakCardTransform(DEF_COEFF);
        }
        if (!IsMonster())
        {
            if(face == Enums.CardFace.Down) {
                RotateCard(false);
            }
            if(face == Enums.CardFace.Up || cardType == Enums.CardType.Trap)
            {
                UnhighlightObject();
                highlightable = false;
            }
        }
        hasChangedPositionThisTurn = true;
    }

    public void Flip()
    {
        RotateCard(true);
    }

    public void ResetData()
    {
        if (face == Enums.CardFace.Down)
        {
            RotateCard(false);
        }
        if(position == Enums.CardPosition.Def)
        {
            TweakCardTransform(ATK_COEFF);
        }

        face = Enums.CardFace.Up;
        position = Enums.CardPosition.Atk;
        hasChangedPositionThisTurn = true;
        hasAttackedThisTurn = false;
        isActivatedSpell = false;
        selectionMode = false;
    }

    public bool IsActivatedSpell()
    {
        return isActivatedSpell;
    }

    public Enums.CardFace GetFace()
    {
        return face;
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

    private void RotateCard(bool reverse)
    {
        Vector3 crtRotation = this.gameObject.transform.localEulerAngles;
        crtRotation.x += (reverse ? -1 : 1) * 180;
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
                if (GameManager.Get().GetAttackingMonsterIndex() == cardIndex)
                {
                    SetCanvasText(Constants.CANCELLING_TEXT);
                } else
                {
                    SetCanvasText(Constants.ATTACKING_TEXT);
                }
                    
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

    public void SwitchSelectionMode(bool isSelectionMode)
    {
        selectionMode = isSelectionMode;
        if (isSelectionMode)
        {
            SetCanvasText(Constants.SELECTION_TEXT);
        } else
        {
            canvas.enabled = false;
        }
    }

    void OnMouseEnter()
    {
        UIManager.Get().ShowInformation(cardInfo.GetCardNumber(), cardInfo.IsMonster() ? Enums.CardType.Monster : Enums.CardType.Spell);
        if (highlightable)
        {
            HighlightObject();
            canvas.enabled = true;
        }
    }

    void OnMouseExit()
    {
        UIManager.Get().HideInformation();
        if (highlightable)
        {
            UnhighlightObject();
            canvas.enabled = false;
        }
    }

    void OnMouseOver()
    {
        if (highlightable && Input.GetMouseButtonDown(0))
        {
            bool cardWasSelected = selectionMode;
            if (IsMonster())
            {
                ChangeText();
                if (GameManager.Get().GetAttackingMonsterIndex() == cardIndex)
                {
                    GameManager.Get().CancelAttack();
                    return;
                }
                
                InteractWithElement();

                if (!cardWasSelected)
                {

                    HighlightObject();
                    highlightable = true;
                }
            } else
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

    public void SwitchPosition()
    {
        SetPosition((position == Enums.CardPosition.Atk) ? Enums.CardPosition.Def : Enums.CardPosition.Atk);
        int coefficient = (position == Enums.CardPosition.Def) ? DEF_COEFF : ATK_COEFF;

        if(face == Enums.CardFace.Down)
        {
            SetFace(Enums.CardFace.Up);
            RotateCard(false);
        }

        TweakCardTransform(coefficient);

        UnhighlightObject();
        highlightable = false;
    }

    public override void InteractWithElement()
    {
        if(IsMonster())
        {
            if(selectionMode)
            {
                SwitchSelectionMode(false);
                UnhighlightObject();
                highlightable = false;
                GameManager.Get().SelectMonster(cardIndex);
                return;
            }

            Turn.Phase currentPhase = GameManager.Get().GetTurnPhase();

            if (currentPhase == Turn.Phase.Battle)
            {
                GameManager.Get().AttackWithMonster(cardIndex);
            }

            if(GameManager.Get().IsPlayerSacrificing())
            {
                GameManager.Get().AddTribute(false, cardIndex);
                UnhighlightObject();
                highlightable = false;
                return;
            }
            
            if(GameManager.Get().IsQuickActivation())
            {
                GameManager.Get().TriggerQuickEffectActivation(cardIndex, true);
                return;
            }

            if ((currentPhase == Turn.Phase.Main1 || currentPhase == Turn.Phase.Main2) && !hasChangedPositionThisTurn)
            {
                Enums.CardFace oldFace = face;
                Enums.CardPosition oldPos = position;

                SwitchPosition();
                hasChangedPositionThisTurn = true;
                GameManager.Get().SwitchMonsterPosition(cardIndex, oldFace, oldPos, (Monster) cardInfo);
            }
        }
        else
        {
            UnhighlightObject();
            highlightable = false;
            SetFace(Enums.CardFace.Up);
            RotateCard(false);
            GameManager.Get().FlipSpell(cardIndex, false);

            if (GameManager.Get().IsQuickActivation())
            {
                GameManager.Get().TriggerQuickEffectActivation(cardIndex, false);
                return;
            }

            GameManager.Get().ActivateSpell(cardIndex, (NonMonster)cardInfo, Constants.FIELD);
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

    public void ApplyPostAttackRestrictions()
    {
        hasAttackedThisTurn = true;
        SetBattlingMonster(false);

        //after attack, unhighlight it
        UnhighlightObject();
        highlightable = false;
    }

    public bool HasAttackedThisTurn()
    {
        return hasAttackedThisTurn;
    }
}

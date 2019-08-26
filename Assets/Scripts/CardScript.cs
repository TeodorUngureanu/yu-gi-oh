using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardScript : InteractibleAbstractCard {
    
    public int turnPlayed;
    public GameObject frontImagePlane;
    public Canvas frontCanvas, backCanvas;
    private Enums.CardFace face = Enums.CardFace.Up;
    public string position;
    private bool hasChangedPositionThisTurn = true;

    protected override void Awake()
    {
        base.Awake();
        objRenderer = GetComponent<Renderer>();
    }

    public void SetData(int vCardIndex, Enums.CardType vCardType, string cardName)
    {
        cardIndex = vCardIndex;
        cardType = vCardType;

        if(objRenderer == null)
        {
            objRenderer = GetComponent<Renderer>();
        }

        if (frontCanvas != null)
        {
            frontCanvas.enabled = false;
        }
        if (backCanvas != null)
        {
            backCanvas.enabled = false;
        }

        Texture2D texture = Resources.Load<Texture2D>("Images/Card Images/" + cardName);
        frontImagePlane.GetComponent<Renderer>().material.mainTexture = texture;
    }

    public void SetFace(Enums.CardFace newFace)
    {
        face = newFace;
        ChangeText();
    }

    public override void ChangeText()
    {
        if (IsMonster())
        {
            if (GameManager.Get().GetTurnPhase() == Turn.Phase.Battle)
            {
                frontCanvas.GetComponentInChildren<Text>().text = "Attack";
            }
            else
            {
                if (face == Enums.CardFace.Down)
                {
                    backCanvas.GetComponentInChildren<Text>().text = "Flip";
                }
                else
                {
                    frontCanvas.GetComponentInChildren<Text>().text = (position == "DEF") ? "Atk position" : "Def position";
                }
            }
        }
        else
        {
            if(face == Enums.CardFace.Down)
            {
                backCanvas.GetComponentInChildren<Text>().text = "Activate";
            }
        }
    }
    
    public int GetTurnPlayed()
    {
        return turnPlayed;
    }

    public void SetTurnPlayed(int vTurnPlayed)
    {
        turnPlayed = vTurnPlayed;
    }

    void OnMouseEnter()
    {
        if (highlightable)
        {
            HighlightObject();
            if(face == Enums.CardFace.Up)
            {
                frontCanvas.enabled = true;
            } else
            {
                backCanvas.enabled = true;
            }
        }
    }

    void OnMouseExit()
    {
        if (highlightable)
        {
            UnhighlightObject();
            if (face == Enums.CardFace.Up)
            {
                frontCanvas.enabled = false;
            }
            else
            {
                backCanvas.enabled = false;
            }
        }
    }

    void OnMouseOver()
    {
        if (highlightable)
        {
            if (Input.GetMouseButtonDown(0))
            {
                backCanvas.enabled = false;
                frontCanvas.enabled = false;
                InteractWithElement();
            }
        }
    }

    private void SwitchFace()
    {
        if(face == Enums.CardFace.Up)
        {
            SetFace(Enums.CardFace.Down);
        }
        else
        {
            if(cardType != Enums.CardType.Trap)
            {
                SetFace(Enums.CardFace.Up);
            }
        }
    }

    private void ChangePosition()
    {
        if (position == "DEF" && face == Enums.CardFace.Down)
        {
            backCanvas.enabled = false;
            frontCanvas.enabled = true;
            SetFace(Enums.CardFace.Up);
            
            Vector3 crtRotation = this.gameObject.transform.localEulerAngles;
            crtRotation.x = 180;
            this.gameObject.transform.localEulerAngles = crtRotation;
        }
    }

    public override void InteractWithElement()
    {
        if(IsMonster())
        {
            Turn.Phase currentPhase = GameManager.Get().GetTurnPhase();

            if (currentPhase == Turn.Phase.Battle)
            {
                //prepare attack, give tribute, select for spell/effect usage
                //SwitchAttackMode();

                //after attack, unhighlight it
                //unhighlightObject();
                //highlight = false;
            }

            if ((currentPhase == Turn.Phase.Main1 || currentPhase == Turn.Phase.Main2) && !hasChangedPositionThisTurn)
            {
                ChangePosition();
                string newPosition = (position == "ATK") ? "DEF" : "ATK";
                GameManager.Get().SwitchMonsterPosition(cardIndex, newPosition);
                hasChangedPositionThisTurn = true;
            }
        }
    }

    public void RefreshTurnRestrictions()
    {
        hasChangedPositionThisTurn = false;
    }

    public bool HasPositionBeenChanged()
    {
        return hasChangedPositionThisTurn;
    }
}

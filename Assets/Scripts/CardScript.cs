using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardScript : InteractibleElementScript {

    public enum Location { HAND, DISK, GRAVEYARD };

    private Location location;
    private int cardIndex;
    private bool highlight = false;
    private string cardType;
    public int turnPlayed;
    public GameObject frontImagePlane;
    public Canvas frontCanvas, backCanvas;
    private string face = "UP";
    private string position = "ATK";
    private bool hasChangedPositionThisTurn = true;

    protected override void Awake()
    {
        base.Awake();
        objRenderer = GetComponent<Renderer>();
    }

    public void SetData(Location vLocation, int vCardIndex, string vCardType, string cardName)
    {
        location = vLocation;
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

        if(location == Location.HAND)
        {
            if(cardType == "Spell")
            {
                face = "DOWN";
            }
            ChangeText();
        }

        Texture2D texture = Resources.Load<Texture2D>("Images/Card Images/" + cardName);
        frontImagePlane.GetComponent<Renderer>().material.mainTexture = texture;
    }

    public void SetFace(string newFace)
    {
        face = newFace;
        ChangeText();
    }

    public void ChangeText()
    {
        if(GameManager.Get().IsPlayerDiscarding())
        {
            frontCanvas.GetComponentInChildren<Text>().text = "Discard";
            return;
        }
        if(location == Location.HAND)
        {
            if(IsMonster())
            {
                frontCanvas.GetComponentInChildren<Text>().text = (face == "UP") ? "Summon" : "Set";
            }
            else
            {
                frontCanvas.GetComponentInChildren<Text>().text = (cardType == "Spell" && face == "UP") ? "Activate" : "Set";
            }
        }
        else
        {
            if (IsMonster())
            {
                if (GameManager.Get().GetTurnPhase() == Turn.Phase.Battle)
                {
                    frontCanvas.GetComponentInChildren<Text>().text = "Attack";
                }
                else
                {
                    if (face == "DOWN")
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
                if(face == "DOWN")
                {
                    backCanvas.GetComponentInChildren<Text>().text = "Activate";
                }
            }
        }
    }

    public Location GetLocation()
    {
        return location;
    }

    public int GetCardIndex()
    {
        return cardIndex;
    }

    public void SetCardIndex(int vCardIndex)
    {
        cardIndex = vCardIndex;
    }

    public string GetCardType()
    {
        return cardType;
    }

    public bool IsHighlight()
    {
        return highlight;
    }

    public void SetHighlight(bool vHighlight)
    {
        highlight = vHighlight;
        if(!highlight)
        {
            unhighlightObject();
        }
    }

    public bool IsMonster()
    {
        return Equals(cardType, "Monster");
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
        if (highlight)
        {
            highlightObject();
            if (location == Location.HAND)
            {
                frontCanvas.enabled = true;
            }
            if(location == Location.DISK)
            {
                if(face == "UP")
                {
                    frontCanvas.enabled = true;
                } else
                {
                    backCanvas.enabled = true;
                }
            }
        }
    }

    void OnMouseExit()
    {
        if (highlight)
        {
            unhighlightObject();
            if (location == Location.HAND)
            {
                frontCanvas.enabled = false;
                if(IsMonster())
                {
                    SetFace("UP");
                }
                else
                {
                    SetFace("DOWN");
                }
            }
            if (location == Location.DISK)
            {
                if (face == "UP")
                {
                    frontCanvas.enabled = false;
                }
                else
                {
                    backCanvas.enabled = false;
                }
            }
        }
    }

    void OnMouseOver()
    {
        if (highlight)
        {
            if (Input.GetMouseButtonDown(0))
            {
                interactWithElement();
            }

            if(Input.GetMouseButtonDown(1) && location == Location.HAND && !GameManager.Get().IsPlayerDiscarding())
            {
                SwitchFace();
            }
        }
    }

    private void SwitchFace()
    {
        if(face == "UP")
        {
            SetFace("DOWN");
        }
        else
        {
            if(cardType != "Trap")
            {
                SetFace("UP");
            }
        }
    }

    private void ChangePosition()
    {
        if(position == "DEF" && face == "DOWN")
        {
            backCanvas.enabled = false;
            frontCanvas.enabled = true;
        }
        position = (position == "ATK") ? "DEF" : "ATK";
    }

    public override void interactWithElement()
    {
        if(location.Equals(Location.HAND))
        {
            if (GameManager.Get().IsPlayerDiscarding())
            {
                GameManager.Get().DiscardCard(cardIndex);
            }
            else
            {
                //place it on the disk
                if (IsMonster())
                {
                    GameManager.Get().PlaceMonsterOnDisk(cardIndex, face);
                }
                else
                {
                    GameManager.Get().PlaceSpellOnDisk(cardIndex, face);
                }
            }
        }

        if(location.Equals(Location.DISK) && IsMonster())
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
                hasChangedPositionThisTurn = true;
                GameManager.Get().SwitchMonsterPosition(cardIndex, (face.Equals("DOWN")) ? "UP" : "DOWN");
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

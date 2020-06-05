using UnityEngine;

public class EnemyFieldCardScript : InteractibleAbstractCard
{
    private Card cardInfo;
    public Canvas canvas;

    private Enums.CardFace face;
    private Enums.CardPosition position;

    protected override void Awake()
    {
        base.Awake();
        objRenderer = GetComponent<Renderer>();
    }

    public Card GetCardInfo()
    {
        return cardInfo;
    }

    public Enums.CardPosition GetPosition()
    {
        return position;
    }

    public void SetFace(Enums.CardFace vFace)
    {
        face = vFace;
    }

    public void SetCardProperties(int vCardIndex, Card vCardInfo, Enums.CardFace vFace)
    {
        cardIndex = vCardIndex;
        cardInfo = vCardInfo;
        face = vFace;
        position = (face == Enums.CardFace.Up) ? Enums.CardPosition.Atk : Enums.CardPosition.Def;
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
        if (highlightable && Input.GetMouseButtonDown(0))
        {
            canvas.enabled = false;
            InteractWithElement();
        }
    }

    public override void ChangeText()
    {
        throw new System.NotImplementedException();
    }

    public override void InteractWithElement()
    {
        GameManager.Get().AttackTarget(cardIndex, position, face);
    }
}

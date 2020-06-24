using UnityEngine;
using VRTK.Highlighters;

public class EnemyFieldCardScript : InteractibleAbstractCard
{
    public Canvas canvas;

    private Card cardInfo;
    private Enums.CardFace face;
    private Enums.CardPosition position;
    private bool isBeingSummoned = false;
    private float originalY = 99999, maxYmovement = 0.7f;

    protected override void Awake()
    {
        base.Awake();
        objRenderer = GetComponent<Renderer>();
    }

    protected override void Update()
    {
        if (isBeingSummoned)
        {
            float currentY = transform.position.y;
            if (currentY >= originalY)
            {
                Vector3 defaultPos = transform.position;
                defaultPos.y = originalY;
                transform.position = defaultPos;
                isBeingSummoned = false;
            }
            transform.Translate(Vector3.up * Time.deltaTime, Camera.main.transform);
        }
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
        if (originalY == 99999)
        {
            originalY = transform.position.y;
        }

        cardIndex = vCardIndex;
        cardInfo = vCardInfo;
        face = vFace;
        position = (face == Enums.CardFace.Up) ? Enums.CardPosition.Atk : Enums.CardPosition.Def;

        transform.position -= new Vector3(0, maxYmovement, 0);
        isBeingSummoned = true;
    }

    public void SwitchPosition(Card vCardInfo, Enums.CardFace vFace, Enums.CardPosition vPosition)
    {
        cardInfo = vCardInfo;
        face = vFace;
        position = vPosition;
    }

    public void SetCardInfo(Card vCardInfo)
    {
        cardInfo = vCardInfo;
    }

    public void SetVRHighlightable(bool highlightable)
    {
        gameObject.GetComponentInChildren<VRTK_OutlineObjectCopyHighlighter>().active = highlightable;
    }

    public void OnPointerEnter()
    {
        string cardNumber = null;
        Enums.CardType cardType = Enums.CardType.Monster;

        if (cardInfo != null) {
            cardNumber = cardInfo.GetCardNumber();
            cardType = cardInfo.IsMonster() ? Enums.CardType.Monster : Enums.CardType.Spell;
        }

        UIManager.Get().ShowInformation(cardNumber, cardType);
        if (highlightable)
        {
            //HighlightObject();
            canvas.enabled = true;
        }
    }

    public void OnPointerExit()
    {
        UIManager.Get().HideInformation();
        if (highlightable)
        {
            //UnhighlightObject();
            canvas.enabled = false;
        }
    }

    void OnPointerOver()
    {
        if (highlightable && Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
    }

    public void HandleClick()
    {
        canvas.enabled = false;
        InteractWithElement();
    }

    public override void ChangeText()
    {
        throw new System.NotImplementedException();
    }

    public override void InteractWithElement()
    {
        highlightable = false;
        //UnhighlightObject();
        SetVRHighlightable(false);
        GameManager.Get().AttackTarget(cardIndex, position, face);
    }
}

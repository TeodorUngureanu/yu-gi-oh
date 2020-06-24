using UnityEngine;

public class PlayerFieldCardScript : MonoBehaviour
{
    private Card cardInfo;
    private bool isBeingSummoned = false;
    private float originalY = 99999, maxYmovement = 0.7f;

    private void Update()
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

    public void SetCardInformation(Card vCardInfo)
    {
        if(originalY == 99999)
        {
            originalY = transform.position.y;
        }

        cardInfo = vCardInfo;

        transform.position -= new Vector3(0, maxYmovement, 0);
        isBeingSummoned = true;
    }

    void OnMouseEnter()
    {
        UIManager.Get().ShowInformation(cardInfo.GetCardNumber(),
            cardInfo.IsMonster() ? Enums.CardType.Monster : Enums.CardType.Spell);
    }

    void OnMouseExit()
    {
        UIManager.Get().HideInformation();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFieldCardScript : MonoBehaviour
{
    private Card cardInfo;

    public void SetCardInformation(Card vCardInfo)
    {
        cardInfo = vCardInfo;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInfoScript : MonoBehaviour
{
    public GameObject infoCanvas;
    public GameObject unknownInfoCanvas;

    public void HideInfo()
    {
        gameObject.SetActive(false);
    }

    public void ShowInfo(string cardNumber, Enums.CardType cardType)
    {
        if(cardNumber == null)
        {
            infoCanvas.SetActive(false);
            unknownInfoCanvas.SetActive(true);
        } else
        {
            Texture2D texture = Utils.LoadTexture(cardNumber, cardType);
            if (texture != null)
            {
                infoCanvas.GetComponent<Renderer>().material.mainTexture = texture;
            }
            unknownInfoCanvas.SetActive(false);
            infoCanvas.SetActive(true);
        }

        gameObject.SetActive(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandScript : MonoBehaviour {

    public GameObject defaultCard;
    private List<GameObject> cardsInHand;

    private readonly float x = -0.44f, y = 2.46f, z = -5.0f;
    private readonly float xInterval = 0.88f;
    
    private Vector3 rotationVector = new Vector3(0, -90, -90);
    private Vector3 scalingVector = new Vector3(100, 0.2401343f, 69);

    void Awake()
    {
        cardsInHand = new List<GameObject>();
    }

    public void AddCard(int index, Enums.CardType cardType, string cardNumber)
    {
        float crtX = x + cardsInHand.Count * xInterval;
        GameObject crtCard = Instantiate<GameObject>(
            defaultCard,
            new Vector3(crtX, y, z) + gameObject.transform.position,
            Quaternion.Euler(rotationVector.x, rotationVector.y, rotationVector.z),
            gameObject.transform);

        crtCard.transform.localScale = scalingVector;
        crtCard.GetComponent<HandCardScript>().SetData(index, cardType, cardNumber);
        //string defaultFaceOnDisk = (cardType == "Monster") ? "UP" : "DOWN";
        //crtCard.GetComponent<CardScript>().SetFace(defaultFaceOnDisk);

        cardsInHand.Add(crtCard);
    }

    public void RecalculateIndex(int lowestIndex, bool isMonster)
    {
        Destroy(cardsInHand[lowestIndex]);
        cardsInHand.RemoveAt(lowestIndex);
        for(int index = 0; index < cardsInHand.Count; index ++)
        {
            GameObject crtCard = cardsInHand[index];
            if (isMonster && crtCard.GetComponent<HandCardScript>().IsMonster())
            {
                SetCardUnhighlightable(index);
            }
            if (index >= lowestIndex)
            {
                crtCard.GetComponent<HandCardScript>().SetCardIndex(index);
                crtCard.transform.position = new Vector3(crtCard.transform.position.x - xInterval, crtCard.transform.position.y, crtCard.transform.position.z);
            }
        }
    }

    public void SetCardHighlightable(int index)
    {
        cardsInHand[index].GetComponent<HandCardScript>().SetHighlightable(true);
    }

    public void SetCardUnhighlightable(int index)
    {
        cardsInHand[index].GetComponent<HandCardScript>().SetHighlightable(false);
    }

    public HandCardScript GetCardScriptForIndex(int index)
    {
        return cardsInHand[index].GetComponent<HandCardScript>();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandScript : MonoBehaviour {

    private GameObject defaultCard;
    public List<GameObject> cardsInHand;

    private float x = -0.44f;
    private float y = 2.46f, z = -5.0f;
    private float xInterval = 0.88f;
    
    private Vector3 rotationVector = new Vector3(0, -90, -90);
    private Vector3 scalingVector = new Vector3(100, 0.2401343f, 69);
	
    public void SetDefaultCard(GameObject card)
    {
        defaultCard = card;
    }

    public void AddCard(int index, bool isMonster, string cardName)
    {
        float crtX = x + cardsInHand.Count * xInterval;
        GameObject crtCard = Instantiate<GameObject>(
            defaultCard,
            new Vector3(crtX, y, z) + gameObject.transform.position,
            Quaternion.Euler(rotationVector.x, rotationVector.y, rotationVector.z),
            gameObject.transform);

        crtCard.transform.localScale = scalingVector;
        crtCard.GetComponent<CardScript>().SetData(CardScript.Location.HAND, index, isMonster, cardName);
        
        cardsInHand.Add(crtCard);
    }

    public void RecalculateIndex(int lowestIndex, bool isMonster)
    {
        Destroy(cardsInHand[lowestIndex]);
        cardsInHand.RemoveAt(lowestIndex);
        for(int index = 0; index < cardsInHand.Count; index ++)
        {
            GameObject crtCard = cardsInHand[index];
            if (isMonster && crtCard.GetComponent<CardScript>().IsMonster())
            {
                UnhighlightCard(index);
            }
            if (index >= lowestIndex)
            {
                crtCard.GetComponent<CardScript>().SetCardIndex(index);
                crtCard.transform.position = new Vector3(crtCard.transform.position.x - xInterval, crtCard.transform.position.y, crtCard.transform.position.z);
            }
        }
    }

    public void HighlightCard(int index)
    {
        cardsInHand[index].GetComponent<CardScript>().SetHighlight(true);
    }

    public void UnhighlightCard(int index)
    {
        cardsInHand[index].GetComponent<CardScript>().SetHighlight(false);
    }
}

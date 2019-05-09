using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandScript : MonoBehaviour {

    private GameObject defaultCard;
    public List<GameObject> cardsInHand;

    private float x = -0.44f;
    private float y = 2.46f, z = -5.0f;
    private float xInterval = 0.88f;
    
    private Vector3 rotationVector = new Vector3(0, -90, 90);
    private Vector3 scalingVector = new Vector3(100, 0.2401343f, 69);

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetDefaultCard(GameObject card)
    {
        defaultCard = card;
    }

    public void AddCard(int index)
    {
        float crtX = x + cardsInHand.Count * xInterval;
        GameObject crtCard = Instantiate<GameObject>(
            defaultCard,
            new Vector3(crtX, y, z) + gameObject.transform.position,
            Quaternion.Euler(rotationVector.x, rotationVector.y, rotationVector.z),
            gameObject.transform);

        crtCard.transform.localScale = scalingVector;
        crtCard.GetComponent<CardScript>().SetCardIndex(index);
        cardsInHand.Add(crtCard);
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

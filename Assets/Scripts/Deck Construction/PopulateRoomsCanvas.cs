using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulateRoomsCanvas : MonoBehaviour
{
    public GameObject card;

    // Start is called before the first frame update
    void Start()
    {
        for (int j = 0; j <= 6; j++)
        {
            for (int i = 0; i <= 14; i++)
            {
                Instantiate(card, new Vector3(card.transform.position.x, card.transform.position.y - (j * 0.358f), card.transform.position.z + (i * 0.25f)), card.transform.rotation, card.transform.parent);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
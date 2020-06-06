using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiskInfoScript : MonoBehaviour
{
    public Text lifePointsText, deckSizeText;

    public void ChangeLPText(long newLifePoints)
    {
        lifePointsText.text = newLifePoints.ToString();
    }

    public void ChangeDeckSizeText(int newDeckSize)
    {
        deckSizeText.text = newDeckSize.ToString();
    }
}

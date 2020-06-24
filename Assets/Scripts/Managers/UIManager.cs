using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject uiCardInformation;
    public GameObject diskInformation;
    public GameObject duelInfoCanvas;
    public Text PlayerName;
    public Text EnemyName;
    public Text PlayerGraveyardCount;
    public Text EnemyGraveyardCount;

    private static UIManager instance;
    private CardInfoScript cardInfoScript;
    private DiskInfoScript diskInfoScript;
    private InfoScreenScript infoScreenScript;

    public static UIManager Get()
    {
        return instance;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            cardInfoScript = uiCardInformation.GetComponent<CardInfoScript>();
            diskInfoScript = diskInformation.GetComponent<DiskInfoScript>();
            infoScreenScript = duelInfoCanvas.GetComponent<InfoScreenScript>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayerName.text = NetworkConnectionManager.Get().GetPlayerName();
        EnemyName.text = NetworkConnectionManager.Get().GetEnemyName();
    }

    public void HideInformation()
    {
        cardInfoScript.HideInfo();
    }

    public void ShowInformation(string cardNumber, Enums.CardType cardType)
    {
        if (GameManager.Get().IsCardInfoOn())
        {
            cardInfoScript.ShowInfo(cardNumber, cardType);
        }
    }

    public void UpdateLPOnDisk(long newLP)
    {
        diskInfoScript.ChangeLPText(newLP);
    }

    public void UpdateDeckSizeOnDisk(int newDeckSize)
    {
        diskInfoScript.ChangeDeckSizeText(newDeckSize);
    }

    public void UpdatePointsOnInfoPanel(string lifePoints, bool isEnemy)
    {
        infoScreenScript.ChangePoints(lifePoints, isEnemy);
    }

    public void ChangePhaseOnInfoPanel(string newPhase, bool isEnemy)
    {
        infoScreenScript.ChangePhase(newPhase, isEnemy);
    }

    public void SetHandSizeOnInfoPanel(string newSize, bool isEnemy)
    {
        infoScreenScript.ChangeHandSize(newSize, isEnemy);
    }

    public void SetDeckSizeOnInfoPanel(string newSize, bool isEnemy)
    {
        infoScreenScript.ChangeDeckSize(newSize, isEnemy);
    }

    //TODO: call this when the graveyard size increases/decreases for any player
    public void SetGraveyardSizeOnInfoPanel(string newSize, bool isEnemy)
    {
        infoScreenScript.ChangeGraveyardSize(newSize, isEnemy);
    }

    public void SetInfoTextOnInfoPanel(string infoText, bool isEnemy)
    {
        infoScreenScript.SetInfoText(infoText, isEnemy);
    }

    public void ShowDuelEnd(bool isEnemyWinner)
    {
        infoScreenScript.ShowEndGameScreen(isEnemyWinner);
    }

    public void UpdatePlayerGraveyardCount(int count)
    {
        PlayerGraveyardCount.text = "" + count;
}

    public void UpdateEnemyGraveyardCount(int count)
    {
        EnemyGraveyardCount.text = "" + count;
    }

}

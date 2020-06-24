using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraveyardButton : MonoBehaviour
{
    public GameObject GraveyardInformation;
    public Button PlayerTab;

    private void Awake()
    {
        GraveyardInformation.gameObject.SetActive(false);
    }

    private void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(TaskOnClick);
    }

    public void TaskOnClick()
    {
        List<Card> playerGraveyard = GameManager.Get().GetGraveyard(Constants.PLAYER);
        List<Card> enemyGraveyard = GameManager.Get().GetGraveyard(Constants.ENEMY);

        GraveyardInformation.gameObject.SetActive(true);
        PlayerTab.onClick.Invoke();

        GraveyardCanvas.Get().LoadBoardCards(playerGraveyard, Constants.PLAYER);
        GraveyardCanvas.Get().LoadBoardCards(enemyGraveyard, Constants.ENEMY);
    }
}

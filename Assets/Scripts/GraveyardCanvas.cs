using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraveyardCanvas : MonoBehaviour
{
    private static GraveyardCanvas instance;
    private const int DeckMaxWidth = 10;
    private string playerEnemyConstant;

    public GameObject InstantiateCardPlayer;
    public GameObject InstantiateCardEnemy;
    public GameObject PlayerCanvas;
    public GameObject EnemyCanvas;

    private List<Utils.InstantiatedGraveyardDeck> PlayerGraveyard = new List<Utils.InstantiatedGraveyardDeck>();
    private List<Utils.InstantiatedGraveyardDeck> EnemyGraveyard = new List<Utils.InstantiatedGraveyardDeck>();

    public static GraveyardCanvas Get()
    {
        return instance;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        PlayerCanvas.SetActive(false);
        EnemyCanvas.SetActive(false);
    }

    public void LoadBoardCards(List<Card> cards, string playerEnemyConstant)
    {
        GameObject crtCard;
        Texture2D texture;
        GameObject InstantiateCard = InstantiateCardPlayer;
        GameObject CanvasToSpawn = PlayerCanvas;
        Utils.InstantiatedGraveyardDeck currentInstantiatedGraveyardCard;
        List<Utils.InstantiatedGraveyardDeck> CurrentGraveyard = PlayerGraveyard;

        if (playerEnemyConstant == Constants.ENEMY)
        {
            InstantiateCard = InstantiateCardEnemy;
            CanvasToSpawn = EnemyCanvas;
            CurrentGraveyard = EnemyGraveyard;
        }

        int noRowsCards = cards.Count / DeckMaxWidth;

        for (int i = 0; i <= noRowsCards; i++)
        {
            for (int j = 0; j < DeckMaxWidth; j++)
            {
                if ((i * DeckMaxWidth + j) < cards.Count)
                {
                    crtCard = Instantiate<GameObject>(
                        InstantiateCard,
                        new Vector3(
                            InstantiateCard.transform.position.x + (j * 0.46f),
                            InstantiateCard.transform.position.y - (i * 0.57f),
                            InstantiateCard.transform.position.z
                        ),
                        Quaternion.Euler(180f, 90f, 90f),
                        CanvasToSpawn.transform
                    );

                    crtCard.transform.localScale = new Vector3(25.91897f, 25.91897f, 9.481894f);
                    crtCard.SetActive(true);

                    if (cards[i * DeckMaxWidth + j].IsMonster())
                    {
                        texture = Utils.LoadTexture(cards[i * DeckMaxWidth + j].GetCardNumber(), Enums.CardType.Monster);
                        crtCard.name = cards[i * DeckMaxWidth + j].GetCardNumber() + "_0";
                    }
                    else
                    {
                        texture = Utils.LoadTexture(cards[i * DeckMaxWidth + j].GetCardNumber(), Enums.CardType.Spell);
                        crtCard.name = cards[i * DeckMaxWidth + j].GetCardNumber() + "_1";
                    }

                    if (texture != null)
                    {
                        crtCard.GetComponentInChildren<GraveyardCard>().frontImagePlange.GetComponent<Renderer>().material.mainTexture = texture;
                    }

                    crtCard.GetComponentInChildren<GraveyardCard>().SetObjectRenderer();

                    currentInstantiatedGraveyardCard.InstantiatedCard = crtCard;

                    if (cards[i * DeckMaxWidth + j].IsMonster())
                    {
                        currentInstantiatedGraveyardCard.CardType = 1;
                    }
                    else
                    {
                        currentInstantiatedGraveyardCard.CardType = 2;
                    }

                    crtCard.GetComponentInChildren<GraveyardCard>().SetGraveyardIndex(CurrentGraveyard.Count);

                    CurrentGraveyard.Add(currentInstantiatedGraveyardCard);
                }
            }
        }
    }

    public List<Utils.InstantiatedGraveyardDeck> GetPlayerGraveyard ()
    {
        return PlayerGraveyard;
    }

    public List<Utils.InstantiatedGraveyardDeck> GetEnemyGraveyard()
    {
        return EnemyGraveyard;
    }

    public void SetPlayerEnemyConstant(string vPlayerEnemyConstant)
    {
        playerEnemyConstant = vPlayerEnemyConstant;
    }

    public string GetPlayerEnemyConstant()
    {
        return playerEnemyConstant;
    }
}

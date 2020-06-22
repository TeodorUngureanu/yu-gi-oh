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

        // PlayerCanvas.SetActive(false);
        // EnemyCanvas.SetActive(false);
    }

    public void LoadBoardCards(List<Card> cards, string playerEnemyConstant)
    {
        GameObject crtCard;
        Texture2D texture;
        Utils.InstantiatedGraveyardDeck currentInstantiatedGraveyardCard;

        if (playerEnemyConstant == Constants.ENEMY)
        {
            EnemyGraveyard.Clear();

            foreach (Transform child in EnemyCanvas.transform)
            {
                if (child.gameObject.name != "Card")
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
        }
        else
        {
            PlayerGraveyard.Clear();

            foreach (Transform child in PlayerCanvas.transform)
            {
                if (child.gameObject.name != "Card")
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
        }

        int noRowsCards = cards.Count / DeckMaxWidth;

        for (int i = 0; i <= noRowsCards; i++)
        {
            for (int j = 0; j < DeckMaxWidth; j++)
            {
                if ((i * DeckMaxWidth + j) < cards.Count)
                {
                    if (playerEnemyConstant == Constants.ENEMY)
                    {
                        crtCard = Instantiate<GameObject>(
                            InstantiateCardEnemy,
                            new Vector3(
                                InstantiateCardEnemy.transform.position.x + (j * 0.46f),
                                InstantiateCardEnemy.transform.position.y - (i * 0.57f),
                                InstantiateCardEnemy.transform.position.z
                            ),
                            Quaternion.Euler(180f, 90f, 90f),
                            EnemyCanvas.transform
                        );
                    }
                    else
                    {
                        crtCard = Instantiate<GameObject>(
                            InstantiateCardPlayer,
                            new Vector3(
                                InstantiateCardPlayer.transform.position.x + (j * 0.46f),
                                InstantiateCardPlayer.transform.position.y - (i * 0.57f),
                                InstantiateCardPlayer.transform.position.z
                            ),
                            Quaternion.Euler(180f, 90f, 90f),
                            PlayerCanvas.transform
                        );
                    }

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

                    if (playerEnemyConstant == Constants.ENEMY)
                    {
                        crtCard.GetComponentInChildren<GraveyardCard>().SetGraveyardIndex(EnemyGraveyard.Count);

                        EnemyGraveyard.Add(currentInstantiatedGraveyardCard);
                    }
                    else
                    {
                        crtCard.GetComponentInChildren<GraveyardCard>().SetGraveyardIndex(PlayerGraveyard.Count);

                        PlayerGraveyard.Add(currentInstantiatedGraveyardCard);
                    }
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

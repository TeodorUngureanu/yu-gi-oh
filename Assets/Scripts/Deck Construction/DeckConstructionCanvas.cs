using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckConstructionCanvas : MonoBehaviour
{
    private const int DeckMaxWidth = 15;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadBoardCards(GameObject card, List<Card> cards, int deckNumber, Dictionary<string, int> deck, int monsterCardsCount)
    {
        GameObject crtCard, _canvas;
        Texture2D texture;

        int noRowsCards = cards.Count / DeckMaxWidth;

        for (int i = 0; i <= noRowsCards; i++)
        {
            for (int j = 0; j < DeckMaxWidth; j++)
            {
                if ((i * DeckMaxWidth + j) < cards.Count)
                {
                    crtCard = Instantiate<GameObject>(
                        card,
                        new Vector3(card.transform.position.x + (j * 0.001f), card.transform.position.y - (i * 0.41f), card.transform.position.z + (j * 0.242f)),
                        Quaternion.Euler(180f, 0f, 90f),
                        gameObject.transform);

                    crtCard.transform.localScale = new Vector3(19.60209f, 31.73939f, 8.988595f);

                    crtCard.transform.position = new Vector3(crtCard.transform.position.x + 0.001f, crtCard.transform.position.y, crtCard.transform.position.z);

                    if ((i * DeckMaxWidth + j) < monsterCardsCount)
                    {
                        texture = Utils.LoadTexture(cards[i * DeckMaxWidth + j].GetCardNumber(), Enums.CardType.Monster);
                        crtCard.name = "" + deckNumber + "_0_" + cards[i * DeckMaxWidth + j].GetCardNumber();
                    }
                    else
                    {
                        texture = Utils.LoadTexture(cards[i * DeckMaxWidth + j].GetCardNumber(), Enums.CardType.Spell);
                        crtCard.name = "" + deckNumber + "_1_" + cards[i * DeckMaxWidth + j].GetCardNumber();
                    }

                    if (texture != null)
                    {
                        crtCard.GetComponentInChildren<DeckConstructionCard>().frontImagePlange.GetComponent<Renderer>().material.mainTexture = texture;
                    }

                    crtCard.GetComponentInChildren<DeckConstructionCard>().SetObjectRenderer();

                    _canvas = crtCard.transform.Find("Canvas").gameObject;
                    _canvas.SetActive(false);

                    if (deck.ContainsKey(cards[i * DeckMaxWidth + j].GetCardNumber()))
                    {
                        _canvas.SetActive(true);
                        _canvas.GetComponentInChildren<Text>().text = "" + deck[cards[i * DeckMaxWidth + j].GetCardNumber()];
                    }
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckConstructionManager : MonoBehaviour
{
    private static DeckConstructionManager instance;

    public List<GameObject> _rooms;
    public List<GameObject> _cards;
    public List<GameObject> _boards;
    public List<Text> numberOfSelectedCards;
    public List<Image> saveDecksButtons;

    Dictionary<int, Monster> _Monster_Cards;
    Dictionary<int, NonMonster> _Magic_Cards;

    Dictionary<string, int> deck_1;
    Dictionary<string, int> deck_2;
    Dictionary<string, int> deck_3;
    Dictionary<string, int> deck_4;
    Dictionary<string, int> deck_5;

    private int countDeck_1;
    private int countDeck_2;
    private int countDeck_3;
    private int countDeck_4;
    private int countDeck_5;

    List<int> userDeck;
    Dictionary<int, List<Dictionary<int, Constants.CardInfo>>> deckCards;

    public static DeckConstructionManager Get()
    {
        return instance;
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Config.Get().Load();
    }

    // Start is called before the first frame update
    void Start()
    {
        _Monster_Cards = Config.Get()._Monster_Cards;
        _Magic_Cards = Config.Get()._Magic_Cards;

        deck_1 = new Dictionary<string, int>();
        deck_2 = new Dictionary<string, int>();
        deck_3 = new Dictionary<string, int>();
        deck_4 = new Dictionary<string, int>();
        deck_5 = new Dictionary<string, int>();

        countDeck_1 = 0;
        countDeck_2 = 0;
        countDeck_3 = 0;
        countDeck_4 = 0;
        countDeck_5 = 0;

        LoadConfig();
        LoadAllCards();
        LoadScore();
    }

    void LoadConfig()
    {
        userDeck = Config.Get()._User_Deck;
        deckCards = Config.Get()._Deck_Cards;

        string roomName;
        int pos;

        for (int i = 0; i < _rooms.Count; i++)
        {
            roomName = _rooms[i].name;
            pos = _rooms[i].name.LastIndexOf("_") + 1;

            LoadRoomConfig(int.Parse(roomName.Substring(pos, roomName.Length - pos)));
        }
    }

    void LoadRoomConfig(int roomNumber)
    {
        if (userDeck.Contains(roomNumber))
        {
            string cardNumber;

            List<Dictionary<int, Constants.CardInfo>> currentDeckCards = deckCards[roomNumber];

            for (int i = 0; i < currentDeckCards.Count; i++)
            {
                foreach (KeyValuePair<int, Constants.CardInfo> kvp in currentDeckCards[i])
                {
                    if (kvp.Value.Card_Type == 1)
                    {
                        cardNumber = _Monster_Cards[kvp.Key].GetCardNumber();
                    }
                    else
                    {
                        cardNumber = _Magic_Cards[kvp.Key].GetCardNumber();
                    }

                    switch (roomNumber)
                    {
                        case 1:
                            {
                                if (deck_1.ContainsKey(cardNumber))
                                {
                                    deck_1[cardNumber]++;
                                }
                                else
                                {
                                    deck_1.Add(cardNumber, 1);
                                }

                                countDeck_1++;

                                break;
                            }
                        case 2:
                            {
                                if (deck_2.ContainsKey(cardNumber))
                                {
                                    deck_2[cardNumber]++;
                                }
                                else
                                {
                                    deck_2.Add(cardNumber, 1);
                                }

                                countDeck_2++;

                                break;
                            }
                        case 3:
                            {
                                if (deck_3.ContainsKey(cardNumber))
                                {
                                    deck_3[cardNumber]++;
                                }
                                else
                                {
                                    deck_3.Add(cardNumber, 1);
                                }

                                countDeck_3++;

                                break;
                            }
                        case 4:
                            {
                                if (deck_4.ContainsKey(cardNumber))
                                {
                                    deck_4[cardNumber]++;
                                }
                                else
                                {
                                    deck_4.Add(cardNumber, 1);
                                }

                                countDeck_4++;

                                break;
                            }
                        case 5:
                            {
                                if (deck_5.ContainsKey(cardNumber))
                                {
                                    deck_5[cardNumber]++;
                                }
                                else
                                {
                                    deck_5.Add(cardNumber, 1);
                                }

                                countDeck_5++;

                                break;
                            }
                    }
                }
            }
        }
    }

    List<Card> ConvertToCards<T>(Dictionary<int, T> _cardList) where T : Card
    {
        List<Card> cards = new List<Card>();

        foreach (KeyValuePair<int, T> kvp in _cardList)
        {
            cards.Add(kvp.Value);
        }

        return cards;
    }

    void LoadAllCards()
    {
        List<Card> monsterCards = new List<Card>();
        List<Card> magicCards = new List<Card>();
        List<Card> cards = new List<Card>();

        monsterCards = ConvertToCards(_Monster_Cards);
        magicCards = ConvertToCards(_Magic_Cards);

        cards.AddRange(monsterCards);
        cards.AddRange(magicCards);

        Dictionary<string, int> deck = new Dictionary<string, int>();

        int monsterCardsCount = monsterCards.Count;

        for (int i = 0; i < _boards.Count; i++)
        {
            switch (i)
            {
                case 0: {
                        deck = deck_1;

                        break;
                    }
                case 1:
                    {
                        deck = deck_2;

                        break;
                    }
                case 2:
                    {
                        deck = deck_3;

                        break;
                    }
                case 3:
                    {
                        deck = deck_4;

                        break;
                    }
                case 4:
                    {
                        deck = deck_5;

                        break;
                    }
            }

            _boards[i].GetComponent<DeckConstructionCanvas>().LoadBoardCards(_cards[i], cards, (i + 1), deck, monsterCardsCount);
        }
    }

    void LoadScore()
    {
        saveDecksButtons[0].gameObject.SetActive(false);
        saveDecksButtons[1].gameObject.SetActive(false);
        saveDecksButtons[2].gameObject.SetActive(false);
        saveDecksButtons[3].gameObject.SetActive(false);
        saveDecksButtons[4].gameObject.SetActive(false);

        if (countDeck_1 > 50)
        {
            numberOfSelectedCards[0].color = Color.red;
        }
        else if (countDeck_1 >= 40)
        {
            numberOfSelectedCards[0].color = Color.green;
            saveDecksButtons[0].gameObject.SetActive(true);
        }
        else
        {
            numberOfSelectedCards[0].color = Color.black;
        }

        if (countDeck_2 > 50)
        {
            numberOfSelectedCards[1].color = Color.red;
        }
        else if (countDeck_2 >= 40)
        {
            numberOfSelectedCards[1].color = Color.green;
            saveDecksButtons[1].gameObject.SetActive(true);
        }
        else
        {
            numberOfSelectedCards[1].color = Color.black;
        }

        if (countDeck_3 > 50)
        {
            numberOfSelectedCards[2].color = Color.red;
        }
        else if (countDeck_3 >= 40)
        {
            numberOfSelectedCards[2].color = Color.green;
            saveDecksButtons[2].gameObject.SetActive(true);
        }
        else
        {
            numberOfSelectedCards[2].color = Color.black;
        }

        if (countDeck_4 > 50)
        {
            numberOfSelectedCards[3].color = Color.red;
        }
        else if (countDeck_4 >= 40)
        {
            numberOfSelectedCards[3].color = Color.green;
            saveDecksButtons[3].gameObject.SetActive(true);
        }
        else
        {
            numberOfSelectedCards[3].color = Color.black;
        }

        if (countDeck_5 > 50)
        {
            numberOfSelectedCards[4].color = Color.red;
        }
        else if (countDeck_5 >= 40)
        {
            numberOfSelectedCards[4].color = Color.green;
            saveDecksButtons[4].gameObject.SetActive(true);
        }
        else
        {
            numberOfSelectedCards[4].color = Color.black;
        }

        numberOfSelectedCards[0].text = "" + countDeck_1;
        numberOfSelectedCards[1].text = "" + countDeck_2;
        numberOfSelectedCards[2].text = "" + countDeck_3;
        numberOfSelectedCards[3].text = "" + countDeck_4;
        numberOfSelectedCards[4].text = "" + countDeck_5;
    }

    public void AddCardToDeck(string cardNumber, int deckNumber)
    {
        switch (deckNumber)
        {
            case 1: {
                    if (deck_1.ContainsKey(cardNumber))
                    {
                        deck_1[cardNumber]++;
                    }
                    else
                    {
                        deck_1.Add(cardNumber, 1);
                    }

                    countDeck_1++;

                    break;
                }
            case 2:
                {
                    if (deck_2.ContainsKey(cardNumber))
                    {
                        deck_2[cardNumber]++;
                    }
                    else
                    {
                        deck_2.Add(cardNumber, 1);
                    }

                    countDeck_2++;

                    break;
                }
            case 3:
                {
                    if (deck_3.ContainsKey(cardNumber))
                    {
                        deck_3[cardNumber]++;
                    }
                    else
                    {
                        deck_3.Add(cardNumber, 1);
                    }

                    countDeck_3++;

                    break;
                }
            case 4:
                {
                    if (deck_4.ContainsKey(cardNumber))
                    {
                        deck_4[cardNumber]++;
                    }
                    else
                    {
                        deck_4.Add(cardNumber, 1);
                    }

                    countDeck_4++;

                    break;
                }
            case 5:
                {
                    if (deck_5.ContainsKey(cardNumber))
                    {
                        deck_5[cardNumber]++;
                    }
                    else
                    {
                        deck_5.Add(cardNumber, 1);
                    }

                    countDeck_5++;

                    break;
                }
        }

        LoadScore();
    }

    public void RemoveCardFromDeck(string cardNumber, int deckNumber)
    {
        switch (deckNumber)
        {
            case 1:
                {
                    if (deck_1[cardNumber] > 1)
                    {
                        deck_1[cardNumber]--;
                    }
                    else
                    {
                        deck_1.Remove(cardNumber);
                    }

                    countDeck_1--;

                    break;
                }
            case 2:
                {
                    if (deck_2[cardNumber] > 1)
                    {
                        deck_2[cardNumber]--;
                    }
                    else
                    {
                        deck_2.Remove(cardNumber);
                    }

                    countDeck_2--;

                    break;
                }
            case 3:
                {
                    if (deck_3[cardNumber] > 1)
                    {
                        deck_3[cardNumber]--;
                    }
                    else
                    {
                        deck_3.Remove(cardNumber);
                    }

                    countDeck_3--;

                    break;
                }
            case 4:
                {
                    if (deck_4[cardNumber] > 1)
                    {
                        deck_4[cardNumber]--;
                    }
                    else
                    {
                        deck_4.Remove(cardNumber);
                    }

                    countDeck_4--;

                    break;
                }
            case 5:
                {
                    if (deck_5[cardNumber] > 1)
                    {
                        deck_5[cardNumber]--;
                    }
                    else
                    {
                        deck_5.Remove(cardNumber);
                    }

                    countDeck_5--;

                    break;
                }
        }

        LoadScore();
    }

    public void SaveDeckToDB(int deckNumber)
    {
        switch (deckNumber)
        {
            case 1:
                {
                    Config.Get().SaveDeck(deckNumber, deck_1);
                    break;
                }
            case 2:
                {
                    Config.Get().SaveDeck(deckNumber, deck_2);
                    break;
                }
            case 3:
                {
                    Config.Get().SaveDeck(deckNumber, deck_3);
                    break;
                }
            case 4:
                {
                    Config.Get().SaveDeck(deckNumber, deck_4);
                    break;
                }
            case 5:
                {
                    Config.Get().SaveDeck(deckNumber, deck_5);
                    break;
                }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetDefaultDeck : MonoBehaviour
{
    public Image defaultDeckIcon;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        string tag = gameObject.tag;
        int defaultDeck = int.Parse(tag.Substring(tag.Length - 1));

        PlayerPrefs.SetInt("Default_Deck", defaultDeck);

        DeckConstructionManager.Get().SetDefaultDecksIconsInactive();

        defaultDeckIcon.gameObject.SetActive(true);
    }
}

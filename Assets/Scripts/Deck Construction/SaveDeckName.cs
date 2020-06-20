using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveDeckName : MonoBehaviour
{
    public InputField deckName;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("" + deckName.gameObject.tag))
        {
            deckName.text = PlayerPrefs.GetString("" + deckName.gameObject.tag);

            if (deckName.text == "" && deckName.gameObject.tag == "Deck_1")
            {
                PlayerPrefs.SetString("" + deckName.gameObject.tag, "Yugi");
                deckName.text = "Yugi";
            }
            else if (deckName.text == "" && deckName.gameObject.tag == "Deck_2")
            {
                PlayerPrefs.SetString("" + deckName.gameObject.tag, "Kaiba");
                deckName.text = "Kaiba";
            }
        }

        gameObject.GetComponent<Button>().onClick.AddListener(TaskOnClick);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void TaskOnClick()
    {
        if (deckName.text != "")
        {
            PlayerPrefs.SetString("" + deckName.gameObject.tag, deckName.text);
        }
    }
}

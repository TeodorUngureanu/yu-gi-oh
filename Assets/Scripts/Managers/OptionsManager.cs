using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionsManager : MonoBehaviour
{
    public InputField PlayerName;
    public Scrollbar SoundFX;
    public Scrollbar Music;
    public Scrollbar AmbientSound;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("PlayerName"))
        {
            PlayerName.text = PlayerPrefs.GetString("PlayerName");
        }

        if (PlayerPrefs.HasKey("SoundFX"))
        {
            SoundFX.value = PlayerPrefs.GetFloat("SoundFX");
        }

        if (PlayerPrefs.HasKey("Music"))
        {
            Music.value = PlayerPrefs.GetFloat("Music");
        }

        if (PlayerPrefs.HasKey("AmbientSound"))
        {
            AmbientSound.value = PlayerPrefs.GetFloat("AmbientSound");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SceneMultiplayer ()
    {
        SceneManager.LoadScene("network");
    }

    public void SavePlayerPrefs()
    {
        Debug.Log(PlayerName);
        PlayerPrefs.SetString("PlayerName", PlayerName.text);
        PlayerPrefs.SetFloat("SoundFX", SoundFX.value);
        PlayerPrefs.SetFloat("Music", Music.value);
        PlayerPrefs.SetFloat("AmbientSound", AmbientSound.value);
    }
}

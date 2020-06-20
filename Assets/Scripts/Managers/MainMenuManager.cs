using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public Button Multiplayer;
    public Button DeckConstruction;
    public Button Options;
    public Button Exit;

    private Image ImgMultiplayer;
    private Image ImgDeckConstruction;
    private Image ImgOptions;
    private Image ImgExit;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        ImgMultiplayer = Multiplayer.GetComponent<Image>();
        ImgDeckConstruction = DeckConstruction.GetComponent<Image>();
        ImgOptions = Options.GetComponent<Image>();
        ImgExit = Exit.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SceneMultiplayer ()
    {
        SceneManager.LoadScene("network");
    }

    public void SceneDeckConstruction()
    {
        SceneManager.LoadScene("Deck Construction");
    }

    public void SceneOptions()
    {
        SceneManager.LoadScene("options");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void OnButtonMultiplayerEnter()
    {
        Image currentImage = ImgMultiplayer;
        OnButtonEnter(currentImage);
    }

    public void OnButtonMultiplayerExit()
    {
        Image currentImage = ImgMultiplayer;
        OnButtonExit(currentImage);
    }

    public void OnButtonDeckConstructionEnter()
    {
        Image currentImage = ImgDeckConstruction;
        OnButtonEnter(currentImage);
    }

    public void OnButtonDeckConstructionExit()
    {
        Image currentImage = ImgDeckConstruction;
        OnButtonExit(currentImage);
    }

    public void OnButtonOptionsEnter()
    {
        Image currentImage = ImgOptions;
        OnButtonEnter(currentImage);
    }

    public void OnButtonOptionsExit()
    {
        Image currentImage = ImgOptions;
        OnButtonExit(currentImage);
    }

    public void OnButtonExitGameEnter()
    {
        Image currentImage = ImgExit;
        OnButtonEnter(currentImage);
    }

    public void OnButtonExitGameExit()
    {
        Image currentImage = ImgExit;
        OnButtonExit(currentImage);
    }

    public void OnButtonEnter(Image currentImage)
    {
        var tempColor = currentImage.color;
        tempColor.a = 1f;
        currentImage.color = tempColor;
    }

    public void OnButtonExit(Image currentImage)
    {
        var tempColor = currentImage.color;
        tempColor.a = 0f;
        currentImage.color = tempColor;
    }
}

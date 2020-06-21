using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GraveyardTab : MonoBehaviour
{
    public Button PlayerGraveyardTab;
    public Button EnemyGraveyardTab;
    public GameObject PlayerCanvas;
    public GameObject EnemyCanvas;
    public GameObject PreviewImage;
    public GameObject GraveyardInformation;
    
    private void Awake()
    {
        PlayerGraveyardTab.GetComponentInChildren<Text>().text = PlayerPrefs.GetString("PlayerName");
    }

    public void ActivatePlayerGraveyardTab()
    {
        PlayerCanvas.SetActive(true);
        EnemyCanvas.SetActive(false);

        ColorBlock cb = PlayerGraveyardTab.colors;
        cb.normalColor = new Color(0.1254902f, 0.2980392f, 0.3803922f, 1f);
        PlayerGraveyardTab.colors = cb;

        cb = EnemyGraveyardTab.colors;
        cb.normalColor = new Color(0.1254902f, 0.2980392f, 0.3803922f, 0f);
        EnemyGraveyardTab.colors = cb;

        PreviewImage.SetActive(false);
    }

    public void ActivateEnemyGraveyardTab()
    {
        PlayerCanvas.SetActive(false);
        EnemyCanvas.SetActive(true);

        ColorBlock cb = PlayerGraveyardTab.colors;
        cb.normalColor = new Color(0.1254902f, 0.2980392f, 0.3803922f, 0f);
        PlayerGraveyardTab.colors = cb;

        cb = EnemyGraveyardTab.colors;
        cb.normalColor = new Color(0.1254902f, 0.2980392f, 0.3803922f, 1f);
        EnemyGraveyardTab.colors = cb;

        PreviewImage.SetActive(false);
    }

    public void CancelGraveyard()
    {
        PlayerCanvas.SetActive(false);
        EnemyCanvas.SetActive(false);
        PreviewImage.SetActive(false);

        ColorBlock cb = PlayerGraveyardTab.colors;
        cb.normalColor = new Color(0.1254902f, 0.2980392f, 0.3803922f, 0f);
        PlayerGraveyardTab.colors = cb;

        cb = EnemyGraveyardTab.colors;
        cb.normalColor = new Color(0.1254902f, 0.2980392f, 0.3803922f, 0f);
        EnemyGraveyardTab.colors = cb;

        GraveyardInformation.SetActive(false);
    }

}

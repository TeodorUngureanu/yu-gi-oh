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

    float positionZ;

    private void Awake()
    {
        PlayerGraveyardTab.GetComponentInChildren<Text>().text = PlayerPrefs.GetString("PlayerName");
        positionZ = PlayerCanvas.transform.position.z;
    }

    public void ActivatePlayerGraveyardTab()
    {
        // PlayerCanvas.SetActive(true);
        // EnemyCanvas.SetActive(false);
        PlayerCanvas.transform.position = new Vector3(PlayerCanvas.transform.position.x, PlayerCanvas.transform.position.y, positionZ - 0.2f);
        EnemyCanvas.transform.position = new Vector3(EnemyCanvas.transform.position.x, EnemyCanvas.transform.position.y, positionZ + 0.2f);

        ColorBlock cb = PlayerGraveyardTab.colors;
        cb.normalColor = new Color(0.1254902f, 0.2980392f, 0.3803922f, 1f);
        PlayerGraveyardTab.colors = cb;

        cb = EnemyGraveyardTab.colors;
        cb.normalColor = new Color(0.1254902f, 0.2980392f, 0.3803922f, 0f);
        EnemyGraveyardTab.colors = cb;

        PreviewImage.SetActive(false);

        GraveyardCanvas.Get().SetPlayerEnemyConstant(Constants.PLAYER);
    }

    public void ActivateEnemyGraveyardTab()
    {
        // PlayerCanvas.SetActive(false);
        // EnemyCanvas.SetActive(true);
        PlayerCanvas.transform.position = new Vector3(PlayerCanvas.transform.position.x, PlayerCanvas.transform.position.y, positionZ + 0.2f);
        EnemyCanvas.transform.position = new Vector3(EnemyCanvas.transform.position.x, EnemyCanvas.transform.position.y, positionZ - 0.2f);

        ColorBlock cb = PlayerGraveyardTab.colors;
        cb.normalColor = new Color(0.1254902f, 0.2980392f, 0.3803922f, 0f);
        PlayerGraveyardTab.colors = cb;

        cb = EnemyGraveyardTab.colors;
        cb.normalColor = new Color(0.1254902f, 0.2980392f, 0.3803922f, 1f);
        EnemyGraveyardTab.colors = cb;

        PreviewImage.SetActive(false);

        GraveyardCanvas.Get().SetPlayerEnemyConstant(Constants.ENEMY);
    }

    public void CancelGraveyard()
    {
        // PlayerCanvas.SetActive(false);
        // EnemyCanvas.SetActive(false);
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

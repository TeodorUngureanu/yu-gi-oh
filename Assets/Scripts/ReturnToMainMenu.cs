using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ReturnToMainMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<Text>().color = new Color(15f / 255f, 217f / 255f, 203f / 255f, 1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Text>().color = new Color(1f, 1f, 1f, 1f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SceneManager.LoadScene("mainMenu");
    }
}

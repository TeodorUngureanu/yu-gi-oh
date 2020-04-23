using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class InfoScreenScript : MonoBehaviour
{
    public GameObject playerName, enemyName;
    public GameObject playerCrown, enemyCrown;
    public GameObject playerHand, enemyHand;
    public GameObject playerDeck, enemyDeck;
    public GameObject playerGraveyard, enemyGraveyard;
    public GameObject playerLP, enemyLP;
    public GameObject phasesParentObj;
    public List<GameObject> screenPhases;
    public GameObject infoTextArea, handInfoArea;
    public GameObject victoryScreen, defeatScreen;

    private Dictionary<string, GameObject> phaseMap;
    private Dictionary<string, Sprite> spriteMap;

    private string currentPhase;

    private void Awake()
    {
        //TODO: remove this after the multiplayer part is integrated and players can set their names
        SetDuelistName("Player", false);
        SetDuelistName("Enemy", true);

        spriteMap = new Dictionary<string, Sprite>();
        phaseMap = new Dictionary<string, GameObject>();

        DirectoryInfo levelDirectoryPath = new DirectoryInfo("Assets/Resources/Images/Phases/");

        foreach (FileInfo file in levelDirectoryPath.GetFiles("*.png", SearchOption.TopDirectoryOnly))
        {
            string fileName = file.Name.Remove(file.Name.IndexOf("."));
            spriteMap.Add(fileName, Resources.Load<Sprite>("Images/Phases/" + fileName));
        }
    }

    private void Start()
    {
        screenPhases.ForEach(phaseObject => {
            phaseMap.Add(phaseObject.name, phaseObject);
        });
    }

    public void SetDuelistName(string name, bool isEnemy)
    {
        if (isEnemy)
        {
            enemyName.GetComponent<Text>().text = name;
        } else
        {
            playerName.GetComponent<Text>().text = name;
        }
    }

    public void ChangeHandSize(string newHandSize, bool isEnemy)
    {
        if(isEnemy)
        {
            enemyHand.GetComponent<Text>().text = newHandSize;
        } else
        {
            playerHand.GetComponent<Text>().text = newHandSize;
        }
    }

    public void ChangeDeckSize(string newDeckSize, bool isEnemy)
    {
        if (isEnemy)
        {
            enemyDeck.GetComponent<Text>().text = newDeckSize;
        }
        else
        {
            playerDeck.GetComponent<Text>().text = newDeckSize;
        }
    }

    public void ChangeGraveyardSize(string newGraveyardSize, bool isEnemy)
    {
        if (isEnemy)
        {
            enemyGraveyard.GetComponent<Text>().text = newGraveyardSize;
        }
        else
        {
            playerGraveyard.GetComponent<Text>().text = newGraveyardSize;
        }
    }

    public void ChangePoints(string newPoints, bool isEnemy)
    {
        if(isEnemy)
        {
            enemyLP.GetComponent<Text>().text = newPoints;
        } else
        {
            playerLP.GetComponent<Text>().text = newPoints;
        }
    }

    public void ChangePhase(string newPhase, bool isEnemy)
    {
        GameObject crtPhaseObj, newPhaseObj;
        Sprite crtPhaseBaseSprite, newPhaseSprite;
        if(currentPhase != null && phaseMap.TryGetValue(currentPhase, out crtPhaseObj) && spriteMap.TryGetValue(currentPhase, out crtPhaseBaseSprite))
        {
            crtPhaseObj.GetComponent<Image>().sprite = crtPhaseBaseSprite;
        }

        string newPhaseSpriteName = (isEnemy ? "enemy" : "player") + newPhase;
        if(phaseMap.TryGetValue(newPhase, out newPhaseObj) && spriteMap.TryGetValue(newPhaseSpriteName, out newPhaseSprite))
        {
            newPhaseObj.GetComponent<Image>().sprite = newPhaseSprite;
        }
        currentPhase = newPhase;
    }


    public void SetInfoText(string info, bool isEnemy)
    {
        if (!info.Equals(""))
        {
            GameObject duelistName = isEnemy ? enemyName : playerName;
            string processedInfo = info.Replace(Constants.DUELIST_PLACEHOLDER, duelistName.GetComponent<Text>().text);
            infoTextArea.GetComponent<Text>().text = processedInfo;
        }
        else
        {
            infoTextArea.GetComponent<Text>().text = info;
        }
    }

    public void ShowEndGameScreen(bool isEnemyWinner)
    {
        phasesParentObj.SetActive(false);
        infoTextArea.SetActive(false);
        handInfoArea.SetActive(false);
        if (isEnemyWinner)
        {
            enemyCrown.SetActive(true);
            defeatScreen.SetActive(true);
        } else
        {
            playerCrown.SetActive(true);
            victoryScreen.SetActive(true);
        }
    }
}

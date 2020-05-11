using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ActionTest : MonoBehaviour
{
    private List<string> enemyPhases = new List<string>()
    {
        "Hold", "Draw", "Main1", "Battle", "Main2", "End" 
    };
    private int phaseIndex = 0;

    public void InitiateEnemyFlipTest()
    {
        Card testCardInfo2 = new Monster("15025844", File.ReadAllBytes("Assets/Resources/Images/Card Images/MysticalElf.png"),
            "Mystical Elf", "blahblahblah", 0, 5, 19, 800, 2000, 4, false);
        
        List<MessageParameter> paramList = new List<MessageParameter>()
        {
            new MessageParameter(Constants.CARD_NO_KEY, testCardInfo2.GetCardNumber()),
            new MessageParameter(Constants.TYPE_KEY, Constants.MONSTER)
        };
        Message testMessage = new Message(Constants.FLIPPING_TEXT, 1, paramList);

        testMessage.SetEnemyAction(true);
        testMessage.SetParameters(paramList);

        //this is to test if the newly added field is not serialized (should not be)
        string serializedMessage = Utils.SerializeMessage(testMessage);
        Debug.Log(serializedMessage);

        GameManager.Get().ReceiveInformation(Utils.SerializeMessage(testMessage));
    }

    public void InitiateQuickActivationTest()
    {
        List<MessageParameter> parameters = new List<MessageParameter>()
        {
            new MessageParameter(Constants.TYPE_KEY, Constants.MONSTER),
            new MessageParameter(Constants.CARD_NO_KEY, "54652250"),
            new MessageParameter(Constants.TRIBUTE_NO_KEY, "0")
        };
        Message testMessage = new Message(Constants.SUMMONING_TEXT, 2, parameters);
        GameManager.Get().ReceiveInformation(Utils.SerializeMessage(testMessage));
    }

    private void IncrementPhaseIndex()
    {
        if(++phaseIndex == enemyPhases.Count)
        {
            phaseIndex = 0;
        }
    }

    public void InitiateEnemyPhaseChangeTest()
    {
        List<MessageParameter> parameters = new List<MessageParameter>()
        {
            new MessageParameter(Constants.NEW_PHASE_KEY, enemyPhases[phaseIndex])
        };
        Message testMessage = new Message(Constants.CHANGE_PHASE, 0, parameters);
        GameManager.Get().ReceiveInformation(Utils.SerializeMessage(testMessage));

        IncrementPhaseIndex();
    }

    public void InitiateEnemyDrawTest()
    {
        Message testMessage = new Message(Constants.DRAW, 0, new List<MessageParameter>());
        GameManager.Get().ReceiveInformation(Utils.SerializeMessage(testMessage));
    }
}

using System;
using System.Collections.Generic;

[Serializable]
public class Message
{
    public string action;
    public int cardIndex;
    public List<MessageParameter> parameters;

    [NonSerialized]
    public bool isEnemyAction;

    public Message(string vAction, int vCardIndex, List<MessageParameter> vParameters)
    {
        action = vAction;
        cardIndex = vCardIndex;
        parameters = vParameters;
    }

    public string GetAction()
    {
        return action;
    }

    public void SetAction(string vAction)
    {
        action = vAction;
    }

    public int GetCardIndex()
    {
        return cardIndex;
    }

    public void SetCardIndex(int vCardIndex)
    {
        cardIndex = vCardIndex;
    }

    public bool IsEnemyAction()
    {
        return isEnemyAction;
    }

    public void SetEnemyAction(bool isEnemyAction)
    {
        this.isEnemyAction = isEnemyAction;
    }

    public Dictionary<string, string> ExtractParamDictionary()
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();

        foreach(MessageParameter param in parameters)
        {
            dictionary.Add(param.GetKey(), param.GetValue());
        }

        return dictionary;
    }

    public void SetParameters(List<MessageParameter> vParams)
    {
        parameters = vParams;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MessageParameter
{
    public string key;
    public string value;

    public MessageParameter(string newKey, string newValue)
    {
        key = newKey;
        value = newValue;
    }

    public string GetKey()
    {
        return key;
    }

    public string GetValue()
    {
        return value;
    }
}

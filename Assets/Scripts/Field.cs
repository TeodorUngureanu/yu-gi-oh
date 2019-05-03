using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field {

    private Dictionary<string, int> effects; 

    public void ClearField()
    {
        effects.Clear();
    }

    public void AddEffect(string monsterType, int value)
    {
        effects.Add(monsterType, value);
    }

    public int GetEffectValueForType(string monsterType)
    {
        int value = 0;
        effects.TryGetValue(monsterType, out value);
        return value;
    }
}

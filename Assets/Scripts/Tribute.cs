using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tribute
{
    private readonly int handMonsterIndex;
    private readonly Card cardInfo;
    private readonly Enums.CardFace face;

    private readonly int tributesNeeded;
    private List<int> tributeIndices;

    public Tribute(int vHandMonsterIndex, Card vCardInfo, Enums.CardFace vFace, int vTributesNeeded)
    {
        handMonsterIndex = vHandMonsterIndex;
        cardInfo = vCardInfo;
        face = vFace;

        tributesNeeded = vTributesNeeded;
        tributeIndices = new List<int>();
    }

    public int GetHandMonsterIndex()
    {
        return handMonsterIndex;
    }

    public Card GetCardInfo()
    {
        return cardInfo;
    }

    public Enums.CardFace GetFace()
    {
        return face;
    }

    public void AddTribute(int tributeIndex)
    {
        tributeIndices.Add(tributeIndex);
    }

    public bool HasEnoughTributes()
    {
        return tributeIndices.Count == tributesNeeded;
    }

    public List<int> GetTributes()
    {
        return tributeIndices;
    }
}

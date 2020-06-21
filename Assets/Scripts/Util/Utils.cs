using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Utils
{
    private static char SEPARATOR = ';';

    public static int NeedsTribute(int rarity)
    {
        if (rarity < 5)
        {
            return 0;
        }
        if (rarity >= 5 && rarity < 7)
        {
            return 1;
        }
        return 2;
    }

    public static string SwitchOwner(string owner)
    {
        if(owner == Constants.PLAYER)
        {
            return Constants.ENEMY;
        }
        if(owner == Constants.ENEMY)
        {
            return Constants.PLAYER;
        }
        return owner;
    }

    public static Texture2D LoadTexture(string cardNumber, Enums.CardType cardType)
    {
        // Texture size does not matter - the size of the texture will be replaced by image size
        Texture2D texture = new Texture2D(2, 2);
        bool isLoaded = false;

        if (cardType == Enums.CardType.Monster)
        {
            foreach (KeyValuePair<int, Monster> monster in Config.Get()._Monster_Cards)
            {
                if (monster.Value.GetCardNumber() == cardNumber)
                {
                    byte[] image = monster.Value.GetImage();
                    isLoaded = texture.LoadImage(image);

                    break;
                }
            }
        }
        else
        {
            foreach (KeyValuePair<int, NonMonster> nonMonster in Config.Get()._Magic_Cards)
            {
                if (nonMonster.Value.GetCardNumber() == cardNumber)
                {
                    byte[] image = nonMonster.Value.GetImage();
                    isLoaded = texture.LoadImage(image);

                    break;
                }
            }
        }

        // Apply this texure as per requirement on image or material
        if (isLoaded)
        {
            return texture;

        }

        return null;
    }

    public static string SerializeList(List<int> list)
    {
        return string.Join(SEPARATOR.ToString(), list.Select(i => i.ToString()).ToArray());
    }

    public static List<int> DeserializeList(string serializedList)
    {
        List<string> stringList = new List<string>(serializedList.Split(SEPARATOR));
        return stringList.Select(serializedElement => Int32.Parse(serializedElement)).ToList();
    }

    public static string SerializeMessage(Message message)
    {
        return JsonUtility.ToJson(message);
    }

    public static Message DeserializeMessage(string messageAsString)
    {
        return JsonUtility.FromJson<Message>(messageAsString);
    }
}

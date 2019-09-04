using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public const string DISCARDING_TEXT = "Discard";
    public const string SUMMONING_TEXT = "Summon";
    public const string SETTING_TEXT = "Set";
    public const string FLIPPING_TEXT = "Flip";
    public const string ACTIVATING_TEXT = "Activate";
    public const string ATK_CHANGE_TEXT = "Atk position";
    public const string DEF_CHANGE_TEXT = "Def position";
    public const string ATTACKING_TEXT = "Attack";
    public const string SACRIFICING_TEXT = "Tribute";

    public const int INITIAL_HAND_SIZE = 5;
    public const int MAX_HAND_SIZE = 6;
    public const int STARTING_LIFE_POINTS = 8000;

    public const string MONSTER = "Monster";
    public const string SPELL = "Spell";
    public const string HAND = "Hand";
    public const string DISK = "Disk";
    
    public struct CardInfo
    {
        public int Card_Type;
        public int Card_Order;
    }
}

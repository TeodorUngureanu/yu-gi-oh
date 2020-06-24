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
    public const string ATK_CHANGE_TEXT = "ATK Position";
    public const string DEF_CHANGE_TEXT = "DEF Position";
    public const string ATTACKING_TEXT = "Attack";
    public const string CANCELLING_TEXT = "Cancel";
    public const string TRIBUTE_SUMMON_TEXT = "Tribute Summon";
    public const string TRIBUTE_SET_TEXT = "Tribute Set";
    public const string SACRIFICE_TEXT = "Sacrifice";
    public const string SELECTION_TEXT = "Select";

    public const int INITIAL_HAND_SIZE = 5;
    public const int MAX_HAND_SIZE = 6;
    public const int STARTING_LIFE_POINTS = 2000;

    public const string PLAYER = "FromPlayer";
    public const string ENEMY = "FromEnemy";
    public const string BOTH = "FromBoth";
    public const string MONSTER = "Monster";
    public const string SPELL = "Spell";
    public const string HAND = "Hand";
    public const string FIELD = "Field";
    public const string DECK = "Deck";
    public const string GRAVEYARD = "Graveyard";
    public const string UNKNOWN = "UNKNOWN";

    //used only for communication between peers
    public const string DRAW = "Draw";
    public const string END_TURN = "End";
    public const string CHANGE_PHASE = "ChangePhase";
    public const string QUICK_ACTIVATION = "Quick Activation";
    public const string FLIP_EFFECT_ACTIVATION = "Flip Effect Activation";
    public const string DESELECT = "Deselect";
    public const string PHASE_KEY = "phase";
    public const string ACCEPT = "yeah";
    public const string DENY = "nope";
    public const string TARGET_INDEX_KEY = "targetIndex";
    public const string TARGET_POS_KEY = "targetPosition";
    public const string TARGET_FACE_KEY = "targetFace";
    public const string CARD_NO_KEY = "cardNumber";
    public const string FACE_KEY = "face";
    public const string FLIPPABLE_KEY = "isFlippabe";
    public const string TYPE_KEY = "cardType";
    public const string ORIGIN_KEY = "cardOrigin";
    public const string TRIBUTE_NO_KEY = "noTributes";
    public const string TRIBUTE_INDICES_KEY = "tributeIndices";
    public const string SELECT_NO_KEY = "noSelected";
    public const string SELECT_INDICES_KEY = "selectedIndices";
    public const string SELECT_SOURCE_KEY = "selectSource";
    public const string SELECT_OWNER_KEY = "selectOwner";
    public const string REVEAL_CARD_KEY = "Reveal";
    public const string GRAVEYARD_SUMMON = "graveyardSummon";
    public const string GRAVEYARD_INDEX_KEY = "graveyardIndex";

    //used only for showing information on screen
    public const string DUELIST_PLACEHOLDER = "{duelistName}";
    public const string DISCARD_INFO = "{duelistName} is discarding cards..";
    public const string QUICK_PLAY_INFO = "{duelistName} is thinking..";
    public const string ASK_QUICK_PLAY = " Wanna quick play a card? Space = Yes, Esc = No";
    public const string ASK_FLIP_EFFECT = " Wanna activate flip effect? Space = Yes, Esc = No";

    public const int MAGIC_TYPE = 1;
    public const int TRAP_TYPE = 2;

    public const int DUMMY_INEXISTENT_ID = 0;
    
    public struct CardInfo
    {
        public int Card_Type;
        public int Card_Order;
    }
}

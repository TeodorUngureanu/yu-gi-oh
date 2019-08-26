using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Deck : InteractibleElementScript {
    private List<Card> mainDeck;
    private bool isDrawPhase = false;

    protected override void Awake()
    {
        base.Awake();
        Debug.Log("Deck object awaken");
        mainDeck = new List<Card>();
        objRenderer = GetComponent<Renderer>();
    }

    public void LoadDeck(string deckKey)
    {
        //To implement

        //temporarily adding some cards - TO BE REMOVED
        temporarilyAddCards();
        
        //temporarily preparing file - TO BE REMOVED
        JsonSerializerSettings settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All, Formatting = Formatting.Indented };
        string json = JsonConvert.SerializeObject(mainDeck, settings);
        File.WriteAllText("pack.json", json);

        //keep this part
        String deckJson = File.ReadAllText("pack.json");
        mainDeck = JsonConvert.DeserializeObject<List<Card>>(deckJson, settings);

        Debug.Log("Deck loaded. No. cards: " + mainDeck.Count);
    }

    private void temporarilyAddCards()
    {
        Monster card = new Monster("Ancient Elf", "This elf is rumored to have lived for thousands of years. He leads an army of spirits " +
            "against his enemies.", "NO_EFFECT", Monster.Attribute.Light, Monster.MonsterType.Spellcaster, 1450, 1200, 4, false, false);
        NonMonster card2 = new NonMonster("Book of Secret Arts", "A Spellcaster-Type monster equipped with this card increases its ATK " +
            "and DEF by 300 points.", "NO_EFFECT", NonMonster.NonMonsterType.Spell);
        Monster card3 = new Monster("Ansatsu", "A silent and deadly warrior specializing in assassinations.", "NO_EFFECT", 
            Monster.Attribute.Earth, Monster.MonsterType.Warrior, 1700, 1200, 5, false, false);
        NonMonster card4 = new NonMonster("Card Destruction", "Both players discard as many cards as possible from their hands, then " +
            "each player draws the same number of cards they discarded.", "NO_EFFECT", NonMonster.NonMonsterType.Spell);
        Monster card5 = new Monster("Baron of the Fiend Sword", "An aristocrat who wields a sword possessed by a malicious spirit that " +
            "preys on the weak.", "NO_EFFECT", Monster.Attribute.Dark, Monster.MonsterType.Fiend, 1550, 800, 4, false, false);
        NonMonster card6 = new NonMonster("Castle Walls", "Increase a selected monster's DEF by 500 points during the turn this card " +
            "is activated.", "NO_EFFECT", NonMonster.NonMonsterType.Trap);
        Monster card7 = new Monster("Celtic Guardian", "An elf who learned to wield a sword, he baffles enemies with lightning-swift " +
            "attacks.", "NO_EFFECT", Monster.Attribute.Earth, Monster.MonsterType.Warrior, 1400, 1200, 4, false, false);
        NonMonster card8 = new NonMonster("Change of Heart", "Target 1 monster your opponent controls; take control of it until " +
            "the End Phase.", "NO_EFFECT", NonMonster.NonMonsterType.Spell);
        Monster card9 = new Monster("Claw Reacher", "Stretching arms and razor-sharp claws make this monster a formidable opponent.", 
            "NO_EFFECT", Monster.Attribute.Dark, Monster.MonsterType.Fiend, 1000, 800, 3, false, false);
        Monster card10 = new Monster("Curse of Dragon", "A wicked dragon that taps into dark forces to execute a powerful attack.",
            "NO_EFFECT", Monster.Attribute.Dark, Monster.MonsterType.Dragon, 2000, 1500, 5, false, false);
        NonMonster card11 = new NonMonster("Dark Hole", "Destroy all monsters on the field.", "NO_EFFECT", NonMonster.NonMonsterType.Spell);
        Monster card12 = new Monster("Dark Magician", "The ultimate wizard in terms of attack and defense.",
            "NO_EFFECT", Monster.Attribute.Dark, Monster.MonsterType.Spellcaster, 2000, 1500, 5, false, false);
        NonMonster card13 = new NonMonster("De Spell", "Target 1 face-up Spell, or 1 Set Spell/Trap, on the field; destroy that " +
            "target if it is a Spell. (If the target is Set, reveal it.)", "NO_EFFECT", NonMonster.NonMonsterType.Spell);
        NonMonster card14 = new NonMonster("Dian Keto the Cure Master", "Increase your Life Points by 1000 points.", "NO_EFFECT", 
            NonMonster.NonMonsterType.Spell);
        Monster card15 = new Monster("Doma the Angel of Silence", "This fairy rules over the end of existence.",
            "NO_EFFECT", Monster.Attribute.Dark, Monster.MonsterType.Fairy, 2000, 1500, 5, false, false);
        NonMonster card16 = new NonMonster("Dragon Capture Jar", "Change all face-up Dragon-Type monsters on the field to " +
            "Defense Position, also they cannot change their battle positions.", "NO_EFFECT", NonMonster.NonMonsterType.Trap);
        mainDeck.Add(card);
        mainDeck.Add(card2);
        mainDeck.Add(card3);
        mainDeck.Add(card4);
        mainDeck.Add(card5);
        mainDeck.Add(card6);
        mainDeck.Add(card7);
        mainDeck.Add(card8);
        mainDeck.Add(card9);
        mainDeck.Add(card10);
        mainDeck.Add(card11);
        mainDeck.Add(card12);
        mainDeck.Add(card13);
        mainDeck.Add(card14);
        mainDeck.Add(card15);
        mainDeck.Add(card16);
    }

    public void ShuffleCards()
    {
        int noCards = mainDeck.Count;
        for (int i = 0; i < noCards - 1; i++)
        {
            int randomPosition = UnityEngine.Random.Range(i, noCards);
            Card auxCard = mainDeck[i];
            mainDeck[i] = mainDeck[randomPosition];
            mainDeck[randomPosition] = auxCard;
        }
    }

    public int CardsLeft()
    {
        return mainDeck.Count;
    }

    public Card DrawCard()
    {
        Card firstCard = mainDeck[0];
        mainDeck.RemoveAt(0);
        Debug.Log("Cards remaining in deck: " + mainDeck.Count);
        return firstCard;
    }

    public void setIsDrawPhase(bool drawPhase)
    {
        isDrawPhase = drawPhase;
    }

    void OnMouseEnter()
    {
        if (isDrawPhase)
        {
            HighlightObject();
        }
    }

    void OnMouseExit()
    {
        if (isDrawPhase)
        {
            UnhighlightObject();
        }
    }

    void OnMouseDown()
    {
        if(isDrawPhase)
        {
            InteractWithElement();
        }
    }

    public override void InteractWithElement()
    {
        GameManager.Get().DrawCard();
        UnhighlightObject();
    }
}

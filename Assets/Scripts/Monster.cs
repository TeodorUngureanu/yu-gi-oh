using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Monster : Card {

    enum Attribute { Dark, Divine, Earth, Fire, Light, Water, Wind };
    //TODO: change this
    enum Type { Aqua, Beast, [Description("Beast-Warrior")] BW, Cyberse, Dinosaur,
                [Description("Divine-Beast")] DB, Dragon, Fairy, Fiend, Fish, Insect,
                Machine, Plant, Psychic, Pyro, Reptile, Rock, [Description("Sea Serpent")] SS,
                Spellcaster, Thunder, Warrior, [Description("Winged Beast")] WB, Wyrm, Zombie };

    private Attribute attribute;
    private Type type;
    private int attackPoints;
    private int defensePoints;
    private int rarity; //number of stars
    private bool hasEffect;
    private bool isForbidden;
    private bool isFusion;
}

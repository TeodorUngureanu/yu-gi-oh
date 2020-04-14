public class Monster : Card {

    private readonly int attribute;
    private readonly int type;
    private readonly int attackPoints;
    private readonly int defensePoints;
    private readonly int rarity; // Number of stars
    private readonly bool hasEffect;
    private readonly bool isFusion;
    private readonly bool isFlippable; //TODO: set this in constructor when it's available in DB

    public Monster(string vCardNumber, byte[] vImage, string vCardName, string vDescription, int vEffectKey, int vAttribute, int vType, int vAttackPoints, int vDefensePoints, int vRarity, bool vIsFusion)
        : base(vCardNumber, vImage, vCardName, vDescription, vEffectKey, true)
    {
        attribute = vAttribute;
        type = vType;
        attackPoints = vAttackPoints;
        defensePoints = vDefensePoints;
        rarity = vRarity;
        isFusion = vIsFusion;
    }

    public int GetAttribute()
    {
        return attribute;
    }

    public int GetMonsterType()
    {
        return type;
    }

    public int GetAttackPoints()
    {
        return attackPoints;
    }

    public int GetDefensePoints()
    {
        return defensePoints;
    }

    public int GetRarity()
    {
        return rarity;
    }

    public bool HasAnEffect()
    {
        return hasEffect;
    }

    public bool IsFusionCard()
    {
        return isFusion;
    }

    public bool IsFlippable()
    {
        return isFlippable;
    }
}

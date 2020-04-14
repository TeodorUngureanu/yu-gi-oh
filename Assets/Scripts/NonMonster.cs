public class NonMonster : Card {

    private readonly int spellType;
    private readonly bool isQuickPlaySpell;

    public NonMonster(string vCardNumber, byte[] vImage, string vCardName, string vDescription, int vEffectKey, int vType)
        : base(vCardNumber, vImage, vCardName, vDescription, vEffectKey, false)
    {
        spellType = vType;
    }

    public int GetSpellType()
    {
        return spellType;
    }

    public bool IsQuickPlaySpell()
    {
        return isQuickPlaySpell;
    }
}

namespace TheHeadHunter;

internal sealed class CombatProfile
{
    public CombatProfile(
        float slash = 0f,
        float blunt = 0f,
        float pierce = 0f,
        float chop = 0f,
        float fire = 0f,
        float frost = 0f,
        float lightning = 0f,
        float poison = 0f,
        float spirit = 0f,
        float blockPower = 0f,
        float durability = 0f,
        float attackStamina = 0f,
        float knockback = 0f,
        float drawStaminaDrain = 0f)
    {
        Damage = new HitData.DamageTypes
        {
            m_slash = slash,
            m_blunt = blunt,
            m_pierce = pierce,
            m_chop = chop,
            m_fire = fire,
            m_frost = frost,
            m_lightning = lightning,
            m_poison = poison,
            m_spirit = spirit,
        };
        BlockPower = blockPower;
        Durability = durability;
        AttackStamina = attackStamina;
        Knockback = knockback;
        DrawStaminaDrain = drawStaminaDrain;
    }

    public HitData.DamageTypes Damage { get; }
    public float BlockPower { get; }
    public float Durability { get; }
    public float AttackStamina { get; }
    public float Knockback { get; }
    public float DrawStaminaDrain { get; }

    public CombatProfile WithSecondary(
        float durability,
        float attackStamina,
        float knockback,
        float blockPower,
        float drawStaminaDrain = 0f)
    {
        return new CombatProfile(
            slash: Damage.m_slash,
            blunt: Damage.m_blunt,
            pierce: Damage.m_pierce,
            chop: Damage.m_chop,
            fire: Damage.m_fire,
            frost: Damage.m_frost,
            lightning: Damage.m_lightning,
            poison: Damage.m_poison,
            spirit: Damage.m_spirit,
            blockPower: blockPower,
            durability: durability,
            attackStamina: attackStamina,
            knockback: knockback,
            drawStaminaDrain: drawStaminaDrain);
    }
}

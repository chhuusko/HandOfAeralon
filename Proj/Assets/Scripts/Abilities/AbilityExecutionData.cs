using UnityEngine;

public readonly struct AbilityExecutionData
{
    public Character Caster { get; }
    public Character Target { get; }
    public CombatGridTile TargetTile { get; }
    public int Damage { get; }
    public int Heal { get; }

    public AbilityExecutionData(Character caster, Character target, CombatGridTile tile, int damage, int heal)
    {
        Caster = caster;
        Target = target;
        TargetTile = tile;
        Damage = damage;
        Heal = heal;
    }
}

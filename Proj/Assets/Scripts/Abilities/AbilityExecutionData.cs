using UnityEngine;

/// <summary>
/// Immutable data container describing the result of an ability execution.
/// Holds contextual information such as caster, target, affected tile
/// and the numerical outcome, damage or healing.
/// </summary>
public readonly struct AbilityExecutionData
{
    public Ability Ability { get; }
    public Character Caster { get; }
    public Character Target { get; }
    public CombatGridTile TargetTile { get; }

    /// <summary>
    /// Amount of damage dealt by this ability execution.
    /// Zero if no damage was dealt.
    /// </summary>
    public int Damage { get; }

    /// <summary>
    /// Amount of healing applied by this ability execution.
    /// Zero if no healing was applied.
    /// </summary>
    public int Heal { get; }

    public AbilityExecutionData(Ability ability, Character caster, Character target, CombatGridTile tile, int damage, int heal)
    {
        Ability = ability;
        Caster = caster;
        Target = target;
        TargetTile = tile;
        Damage = damage;
        Heal = heal;
    }
    /// <summary>
    /// Creates a new AbilityExecutionData instance and notifies all listeners
    /// that an ability has been executed.
    /// </summary>
    public static AbilityExecutionData Create(Ability ability, Character caster, Character target, CombatGridTile tile, int damage, int heal)
    {
        var data = new AbilityExecutionData(ability, caster, target, tile, damage, heal);
        CombatEventManager.InvokeOnAbilityDataCreated(data);
        return data;
    }
}

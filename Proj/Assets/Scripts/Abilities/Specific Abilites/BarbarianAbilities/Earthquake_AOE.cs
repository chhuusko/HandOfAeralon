using UnityEngine;

[CreateAssetMenu(fileName = "Earthquake_Ability", menuName = "Scriptable Objects/Abilities/Barbarian/Earthquake_Ability")]

public class Earthquake_AOE : AOEAbility
{
    [Header("- Ability Specific values -")]
    [SerializeField] private float _damageMultiplier = 0.9f;
    [SerializeField] private float _slowCharacterHitChance = 0.6f;

    [SerializeField] private int _charactersSlowedToGetMana = 2;
    [SerializeField] private int _manaGain = 1;
    protected override void ApplyEffectOnTile(CombatGridTile casterTile, CombatGridTile tileToEffect)
    {
        if (tileToEffect == null) return;

        Character affectedCharacter = tileToEffect.GetOccupantCharacter();
        if (affectedCharacter == null) return;
        Character castingCharacter = casterTile.GetOccupantCharacter();
        if (castingCharacter == null) return;

        int damage = CalculateDamage(castingCharacter, affectedCharacter);
        affectedCharacter.TakeDamage(damage);
        AbilityExecutionData.Create(this, castingCharacter, affectedCharacter, tileToEffect, damage, 0);


        int randomNum = Random.Range(1, 11);
        if ((randomNum / 10) > _slowCharacterHitChance)
        {

        }
    }

    private int CalculateDamage(Character castingCharacter, Character affectedCharacter)
    {
        int damage = (int)(castingCharacter.GetBaseDamage() * _damageMultiplier);
        damage = (int)castingCharacter.GetStatusEffectManager().ModifyOutgoingDamage(damage, this);
        damage = (int)affectedCharacter.GetStatusEffectManager().ModifyIncomingDamage(damage, this);
        return damage;
    }

    protected override void InitiateParticles(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        //
    }
}

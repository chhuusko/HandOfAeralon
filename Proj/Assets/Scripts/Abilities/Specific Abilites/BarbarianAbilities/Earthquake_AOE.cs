using UnityEngine;

[CreateAssetMenu(fileName = "Earthquake_Ability", menuName = "Scriptable Objects/Abilities/Barbarian/Earthquake_Ability")]

public class Earthquake_AOE : AOEAbility
{
    [Header("- Ability Specific values -")]
    [SerializeField] private float _damageMultiplier = 0.9f;
    [SerializeField] private float _slowCharacterHitChance = 0.6f;

    [SerializeField] private int _charactersSlowedToGetMana = 2;
    [SerializeField] private int _manaGain = 1;
    protected override void ApplyEffectOnTile(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        
    }

    private int CalculateDamage(Character castingCharacter, Character affectedCharacter)
    {
   

        //1.
        int damage = castingCharacter.GetBaseDamage();

        //2.
        damage = affectedCharacter.GetCurrentHealth() < (0.5 * affectedCharacter.GetMaxHealth()) ? (int)(damage * _damageMultiplier) : damage;

        damage = (int)castingCharacter.GetStatusEffectManager().ModifyOutgoingDamage(damage, this);
        damage = (int)affectedCharacter.GetStatusEffectManager().ModifyIncomingDamage(damage, this);

        return damage;

    }

    protected override void InitiateParticles(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        //
    }
}

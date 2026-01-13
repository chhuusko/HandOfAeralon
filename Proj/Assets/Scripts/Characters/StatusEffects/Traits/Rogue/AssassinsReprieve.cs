using UnityEngine;

public class AssassinsReprieve : Trait
{
    public override void OnAbilityDataCreated(AbilityExecutionData abilityData)
    {
        if (!abilityData.CharacterDied || abilityData.Caster != Character)
        {
            return;
        }

        var data = Data as FloatModifierData;

        if (!data)
        {
            return;
        }

        // Remove all debuffs on this character.
        Manager.ClearStatusEffects(StatusEffectType.Debuff);

        var modifier = data.ModifierPercent / 100f;
        Character.Heal(Mathf.RoundToInt(Character.GetBaseHealth() * modifier));
    }
}

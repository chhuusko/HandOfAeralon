using UnityEngine;

public class AssassinsReprieve : Trait
{
    public override void OnAbilityUsed(AbilityExecutionData abilityData)
    {
        if (!abilityData.CharacterDied)
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
        
        Character.Heal(Mathf.RoundToInt(Character.GetBaseHealth() / data.Modifier));
    }
}

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

        var statusEffects = Manager.GetAllStatusEffects();
        if (statusEffects.Count > 0)
        {
            Manager.RemoveStatusEffect(statusEffects[UnityEngine.Random.Range(0, statusEffects.Count)]);
        }
        
        Character.IncreaseCurrentHealthPoints(Mathf.RoundToInt(Character.GetCurrentHealth() / data.Modifier));
    }
}

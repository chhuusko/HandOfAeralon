using UnityEngine;

public class KillersFatigue : Trait
{
    public override void OnAbilityUsed(AbilityExecutionData abilityData)
    {
        if (!abilityData.CharacterDied)
        {
            return;
        }

        var data = Data as IntModifierData;

        if (!data)
        {
            return;
        }
        
        Manager.AddStatusEffect(new Slowed(data.Modifier));
    }
}

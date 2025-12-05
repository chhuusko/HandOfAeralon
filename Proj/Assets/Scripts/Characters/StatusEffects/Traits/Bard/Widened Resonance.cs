using UnityEngine;

public class WidenedResonance : Trait
{
    public override void ModifyAoE(ref int AoE)
    {
        var data = Data as IntModifierData;

        if (!data)
        {
            return;
        }
        
        AoE += data.Modifier;
    }
}

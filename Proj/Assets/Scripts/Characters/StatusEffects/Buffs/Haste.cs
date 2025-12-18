using UnityEngine;

public class Haste : StatusEffect
{
    public Haste(int duration) : base(duration)
    {
    }
    
    public override void OnApply()
    {
        var data = Data as SpeedChangeData;

        if (!data)
        {
            return;
        }
        
        Character.Data.SetBaseMovementPoints(Character.GetBaseMovementPoints() + data.MovementPoints);
        Character.SetBaseInitiative(Character.GetBaseInitiative() + data.Initiative);
        Character.IncreaseCurrentMovementPoints(2);
        Character.IncreaseCurrentInitiative(2);
    }

    public override void OnExpire()
    {
        var data = Data as SpeedChangeData;

        if (!data)
        {
            return;
        }
        
        Character.Data.SetBaseMovementPoints(Character.GetBaseMovementPoints() - data.MovementPoints);
        Character.SetBaseInitiative(Character.GetBaseInitiative() - data.Initiative);
        Character.DecreaseCurrentMovementPoints(2);
        Character.DecreaseCurrentInitiative(2);
    }
}

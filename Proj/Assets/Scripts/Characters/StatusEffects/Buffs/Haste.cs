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
        Character.IncreaseCurrentMovementPoints(data.MovementPoints);
        Character.IncreaseCurrentInitiative(data.Initiative);
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
        Character.DecreaseCurrentMovementPoints(data.MovementPoints);
        Character.DecreaseCurrentInitiative(data.Initiative);
    }
}

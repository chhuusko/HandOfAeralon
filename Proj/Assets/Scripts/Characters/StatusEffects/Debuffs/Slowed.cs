using UnityEngine;

public class Slowed : StatusEffect
{
    public Slowed(int duration) : base(duration)
    {
    }

    public override void OnApply()
    {
        var data = Data as SpeedChangeData;

        if (!data)
        {
            return;
        }
        
        Character.Data.DecreaseBaseMovementPoints(data.MovementPoints);
        Character.SetBaseInitiative(Character.GetBaseInitiative() - data.Initiative);
        Character.DecreaseCurrentMovementPoints(data.MovementPoints);
        Character.DecreaseCurrentInitiative(data.Initiative);
    }

    public override void OnExpire()
    {
        var data = Data as SpeedChangeData;

        if (!data)
        {
            return;
        }
        
        Character.Data.IncreaseBaseMovementPoints(data.MovementPoints);
        Character.SetBaseInitiative(Character.GetBaseInitiative() + data.Initiative);
        Character.IncreaseCurrentMovementPoints(data.MovementPoints);
        Character.IncreaseCurrentInitiative(data.Initiative);
    }
}

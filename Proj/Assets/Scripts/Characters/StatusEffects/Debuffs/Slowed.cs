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
        
        Character.IncreaseCurrentMovementPoints(data.MovementPoints);
        Character.IncreaseCurrentInitiative(data.Initiative);
    }
}

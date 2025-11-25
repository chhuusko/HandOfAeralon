using UnityEngine;

public class NewMonoBehaviourScript : StatusEffect
{
    private static StatusEffectData _data;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public NewMonoBehaviourScript(Character character, int duration) : base(character, duration)
    {
    }

    public override void SetData(StatusEffectData data)
    {
        _data = data;
    }

    public override void ModifyIncomingDamage(ref float damage)
    {
        damage /= 1.5f;
    }
}

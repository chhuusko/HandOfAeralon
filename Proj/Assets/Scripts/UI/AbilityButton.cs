using UnityEngine;

public class AbilityButton : MonoBehaviour
{
    private Ability _ability;

    public void SetAbility(Ability ability)
    {
        _ability = ability;
    }

    public void OnClick()
    {
        Selector._instance.PreviewAbilityRange(_ability);
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "Scriptable Objects/Ability")]
public abstract class Ability : ScriptableObject
{
    [Header("General")]
    [SerializeField] private string _abilityName;
    [SerializeField] private Sprite _icon;
    [SerializeField] private float _range;

    public abstract void RunAbility(CombatGridTile casterTile, CombatGridTile targetTile);

    protected abstract void ApplyEffectOnTile(CombatGridTile targetTile);

    public string GetAbilityName => _abilityName;
    public Sprite GetIcon => _icon;
    public float GetRange => _range;

    
}

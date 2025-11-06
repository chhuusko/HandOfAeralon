using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "Scriptable Objects/Ability")]
public abstract class Ability : ScriptableObject
{
    [Header("General")]
    [SerializeField] private string _abilityName;
    [SerializeField] private Sprite _icon;
    [SerializeField] private float _range;

    public abstract void RunAbility(CombatGridTile casterTile, CombatGridTile targetTile);

    public float GetRange => _range;

    
}

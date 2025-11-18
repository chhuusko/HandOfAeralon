using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Ability : ScriptableObject
{
    [Header("- General -")]
    [SerializeField] private string _abilityName;
    [SerializeField] private Sprite _icon;
    [SerializeField] private int _range;
    [SerializeField] private int _cooldown;

    [Header("- Targeting -")]
    [SerializeField] private RangeCalculation _rangeCalculation;
    [SerializeField] private AbilityTargetType _targetType;

    public enum AbilityTargetType
    {
        Any,
        CharacterOccupiedTile,
        Friendly,
        Enemy
    }

    public abstract void RunAbility(CombatGridTile casterTile, CombatGridTile targetTile);
    public abstract List<CombatGridTile> GetTilesToEffect(CombatGridTile tile);
    protected abstract void ApplyEffectOnTile(CombatGridTile targetTile);

    public string GetAbilityName() => _abilityName;
    public Sprite GetIcon() => _icon;
    public float GetRange() => _range;
    public int GetCooldown() => _cooldown;

    public void SetCooldown(int cooldown)
    {
        _cooldown = cooldown;
    }
    public AbilityTargetType GetAbilityTargetType() => _targetType;

    public RangeCalculation GetRangeCalculation => _rangeCalculation;

    public List<CombatGridTile> GetAvailableTargets(CombatGridTile casterTile)
    {
        return _rangeCalculation.CalculateTilesInRange(casterTile, _range);
    }


}

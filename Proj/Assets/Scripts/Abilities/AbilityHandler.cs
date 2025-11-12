using System.Collections.Generic;
using UnityEngine;

public class AbilityHandler : MonoBehaviour
{
    [SerializeField] private List<Ability> _abilities;

    List<CombatGridTile> _availableAbilityTargets = new List<CombatGridTile>();

    Character _characterCaster;
    CombatGridTile _casterTile;

    private void Start()
    {
        if (!TryGetComponent(out _characterCaster))
        {
            Debug.LogError("AbilityHandler is missing Character component!");
            return;
        }
        CombatGridTile _casterTile = _characterCaster.GetCurrentTileComponent();
    }
    public void UseAbility(Ability ability, CombatGridTile targetTile)
    {
        if (!CanCastAbility(targetTile))
        {
            Debug.Log("Tried casting ability on inaccaptable target.");
            return;
        }

        ability.RunAbility(_casterTile, targetTile);
    }
    public void ClearAbilityTargets()
    {
        _availableAbilityTargets.Clear();
    }

    private bool CanCastAbility(CombatGridTile targetTile)
    {
        return _availableAbilityTargets.Contains(targetTile);
    }

    private List<CombatGridTile> CheckAbilityTargets(Ability ability)
    {
        return null;
    }
}

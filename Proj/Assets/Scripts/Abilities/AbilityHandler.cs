using System.Collections.Generic;
using UnityEngine;

public class AbilityHandler : MonoBehaviour
{
    [SerializeField] private List<Ability> _abilities;

    List<CombatGridTile> _availableAbilityTargets = new List<CombatGridTile>();

    Character _characterCaster;
    CombatGridTile _casterTile;
    bool _bDebugAbilityHandler = false;

    private void Start()
    {
        if (!TryGetComponent(out _characterCaster))
        {
            Debug.LogError("AbilityHandler is missing Character component!");
            return;
        }
        CombatGridTile _casterTile = _characterCaster.GetCurrentTileComponent();
    }
    public bool UseAbility(Ability ability, CombatGridTile targetTile)
    {
        GetTilesInRange(ability);
        if (!CanCastAbility(ability, targetTile))
        {
            ClearAbilityTargets();
            if (_bDebugAbilityHandler)
            Debug.Log("Tried casting ability, but it failed");
            return false; ;
        }

        ability.RunAbility(_casterTile, targetTile);
        return true;
    }
    public void ClearAbilityTargets()
    {
        _availableAbilityTargets.Clear();
    }

    private bool CanCastAbility(Ability ability, CombatGridTile targetTile)
    {
        return IsValidTargetForAbility(ability, targetTile) && _availableAbilityTargets.Contains(targetTile);
    }

    private List<CombatGridTile> GetTilesInRange(Ability ability)
    {
        return ability.GetAvailableTiles(_casterTile);
    }

    private bool IsValidTargetForAbility(Ability ability, CombatGridTile tile)
    {
        var occupant = tile.GetOccupant();
        Character character = occupant.GetComponent<Character>();

        switch (ability.GetAbilityTargetType())
        {
            case Ability.AbilityTargetType.Any:
                 return true;
            case Ability.AbilityTargetType.CharacterOccupiedTile:
                 return occupant != null;
            case Ability.AbilityTargetType.Enemy:
                 return character != null && character.GetFaction() == Faction.Enemy;
            case Ability.AbilityTargetType.Friendly:
                return character != null && character.GetFaction() == Faction.Friendly;

            default: return false;
        }
     
    }
}

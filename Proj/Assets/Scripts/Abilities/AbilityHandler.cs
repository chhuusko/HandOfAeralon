using System.Collections.Generic;
using UnityEngine;

public class AbilityHandler : MonoBehaviour
{
    [SerializeField] private List<Ability> _abilities;

    private List<CombatGridTile> _tilesInRange = new List<CombatGridTile>();
    private List<CombatGridTile> _tilesEffected = new List<CombatGridTile>();
    private Character _characterCaster;
    private CombatGridTile _casterTile;
    [SerializeField] private Ability _pendingAbility;

    bool _bDebugAbilityHandler = false;

    private void Start()
    {
        if (!TryGetComponent(out _characterCaster))
        {
            Debug.LogError("AbilityHandler is missing Character component!");
            return;
        }
        _casterTile = _characterCaster.GetCurrentTileComponent();
    }
    public bool UseAbility(Ability ability, CombatGridTile targetTile)
    {
        GetAvailableTargets(ability);
        if (!CanCastAbility(ability, targetTile))
        {
            ClearAbilityTargetRange();
            if (_bDebugAbilityHandler)
                DebugLog.MGLog("Tried casting ability, but it failed");
            return false;
        }

        ability.RunAbility(_casterTile, targetTile);
        return true;
    }
    public Character GetCharacterCaster()
    {
        return _characterCaster;
    }
    public List<CombatGridTile> GetTilesInRange()
    {
        return _tilesInRange;
    }
    public void ClearAbilityTargetRange()
    {
        _tilesInRange.Clear();
    }

    public void SetPendingAbility(Ability ability)
    {
        _pendingAbility = ability;
    }
    public Ability GetPendingAbility()
    {
        return _pendingAbility;
    }

    public void CalculateAbilityRange()
    {
        ClearAbilityTargetRange();

        if(_pendingAbility == null)
        {
            Debug.LogError("No pending ability selected, but is still trying to calculate range");
            return;
        }
        _tilesInRange = GetAvailableTargets(_pendingAbility);
    }

    private bool CanCastAbility(Ability ability, CombatGridTile targetTile)
    {
        return IsValidTargetForAbility(ability, targetTile) && _tilesInRange.Contains(targetTile);
    }

    private List<CombatGridTile> GetAvailableTargets(Ability ability)
    {
        return ability.GetAvailableTargets(_casterTile);
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

    public void PreviewTargetTiles(CombatGridTile tile)
    {
       

    }
}

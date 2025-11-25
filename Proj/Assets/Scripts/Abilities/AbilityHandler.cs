using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

public class AbilityHandler : MonoBehaviour
{
    public static event System.Action OnAbilityCast;
    [SerializeField] private List<Ability> _abilities;

    private List<CombatGridTile> _tilesInRange = new();
    private List<CombatGridTile> _tilesEffected = new();
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
        OnAbilityCast?.Invoke();
        StartCoroutine(ability.PlayAbilityEffect(_casterTile, targetTile));
        ability.RunAbility(_casterTile, targetTile);
        _characterCaster.StartAbilityCooldown(ability);
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
        _casterTile = _characterCaster.GetCurrentTileComponent();

        if(_pendingAbility == null)
        {
            Debug.LogError("No pending ability selected, but is still trying to calculate range");
            return;
        }
        _tilesInRange = GetAvailableTargets(_pendingAbility);
    }

    private bool CanCastAbility(Ability ability, CombatGridTile targetTile)
    {
        return IsValidTargetTileForAbility(ability, targetTile) && _tilesInRange.Contains(targetTile);
    }

    private List<CombatGridTile> GetAvailableTargets(Ability ability)
    {
        return ability.GetAvailableTargets(_casterTile);
    }

    private bool IsValidTargetTileForAbility(Ability ability, CombatGridTile tile)
    {
        if (tile == null) return false;

        var occupant = tile.GetOccupant();
        Character character = occupant? occupant.GetComponent<Character>(): null;

        switch (ability.GetAbilityTargetType())
        {
            case Ability.ValidTargetOccupant.Any:
                return true;
            case Ability.ValidTargetOccupant.CharacterOccupiedTile:
                return occupant != null;
            case Ability.ValidTargetOccupant.Enemy:
                return character != null && character.GetFaction() != _characterCaster.GetFaction();
            case Ability.ValidTargetOccupant.Friendly:
                return character != null && character.GetFaction() == _characterCaster.GetFaction();
            default: return false;
        }
    }

    /// <summary>
    /// Updates the visual preview of which tiles will be affected by the pending ability
    /// based on the tile currently hovered by the player. 
    /// Removes highlight from old tiles, restores their original colors, 
    /// and highlights newly affected tiles in real time.
    /// </summary>
    /// <param name="tile">The tile currently hovered by the player.</param>
    public void PreviewTargetTiles(CombatGridTile tile)
    {
        List<CombatGridTile> newEffectedTiles = _pendingAbility.GetTilesToEffect(tile);

        // When no existing tiles are effected. (first frame)
        if (!_tilesEffected.Any())
        {
            foreach(CombatGridTile t in newEffectedTiles)
            {
                _tilesEffected.Add(t);
                t.SetTileColor(Color.red);
            }
            return;
        }

        // Reset old tiles that should not be effected.
        var previousEffectedTiles = new List<CombatGridTile>(_tilesEffected);
        foreach (CombatGridTile t in previousEffectedTiles)
        {
            if (newEffectedTiles.Contains(t))
            {
                continue;
            }

            if (_tilesInRange.Contains(t))
            {
                t.SetTileColor(Color.green);
                _tilesEffected.Remove(t);
                continue;
            }

            t.SetTileColor(Color.white);
            _tilesEffected.Remove(t);
        }

        // Add new tiles effected list and turn them red.
        foreach(CombatGridTile t in newEffectedTiles)
        {
            if (!_tilesEffected.Contains(t))
            {
                t.SetTileColor(Color.red);
                _tilesEffected.Add(t);
            }
        }
    }
}

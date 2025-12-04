using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityHandler : MonoBehaviour
{
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
        _characterCaster.CanUseAbility = false;
        CombatEventManager.InvokeOnAbilityCast();
        StartCoroutine(ability.StartAbilityEffects(_casterTile, targetTile));
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

    public void CalculateAbilityRange(CombatGridTile specificTile = null)
    {
        ClearAbilityTargetRange();
        
        if (specificTile != null)
        {
            _casterTile = specificTile;
        }
        else
        {
            _casterTile = _characterCaster.GetCurrentTileComponent();
        }

        if (_pendingAbility == null)
        {
            Debug.LogError("No pending ability selected, but is still trying to calculate range");
            return;
        }
        _pendingAbility.SetAbilityHandler(this);
        _tilesInRange = RemoveUntargetableTiles(GetAvailableTargets(_pendingAbility));
    }

    public bool CanCastAbility(Ability ability, CombatGridTile targetTile)
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
                return tile.IsWalkable();
            case Ability.ValidTargetOccupant.CharacterOccupiedTile:
                return occupant != null;
            case Ability.ValidTargetOccupant.Enemy:
                return character != null && character.GetFaction() != _characterCaster.GetFaction();
            case Ability.ValidTargetOccupant.Friendly:
                return character != null && character.GetFaction() == _characterCaster.GetFaction();
            default: return false;
        }
    }
    private List<CombatGridTile> RemoveUntargetableTiles(List<CombatGridTile> tiles)
    {
        List<CombatGridTile> filteredList = new();
        foreach(CombatGridTile tile in tiles){
            if (tile.IsWalkable())
            {
                filteredList.Add(tile);
            }
        }

        if ((_pendingAbility.GetAbilityTargetType() != Ability.ValidTargetOccupant.Any) && (_pendingAbility.GetAbilityTargetType() != Ability.ValidTargetOccupant.Friendly))
        {
            filteredList.Remove(_casterTile);
        }
        return filteredList;
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

        // Reset alla gamla effekter
        foreach (CombatGridTile t in _tilesEffected)
        {
            if (_tilesInRange.Contains(t))
                t.SetTileColor(Color.green);
            else
                t.SetTileColor(Color.white);
        }
        _tilesEffected.Clear();

        if(newEffectedTiles == null)
        {
            return;
        }
        // Applicera nya r�da
        foreach (CombatGridTile t in newEffectedTiles)
        {
            if (t == null) return; 
            t.SetTileColor(Color.red);
            _tilesEffected.Add(t);
        }
    }
}

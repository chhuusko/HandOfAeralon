using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityHandler : MonoBehaviour
{
    [SerializeField] private List<Ability> _abilities;

    private List<CombatGridTile> _tilesInRange = new();
    private List<CombatGridTile> _tilesEffected = new();
    private List<Character> _previewedCharacters = new();

    private Character _characterCaster;
    private CombatGridTile _casterTile;
    [SerializeField] private Ability _pendingAbility;

    bool _bDebugAbilityHandler = false;

    private void Start()
    {
        if (!TryGetComponent(out _characterCaster))
        {
            Debug.LogError("Object is missing Character component!");
            return;
        }
        _casterTile = _characterCaster.GetCurrentTileComponent();
        Selector._instance.OnCharacterDeselected += HandleCharacterDeselected;
    }
    /// <summary>
    /// Attempts to cast the given ability on the selected target tile.
    /// Validates the tile, triggers ability effects, consumes resources,
    /// and starts the ability cooldown if successful.
    /// </summary>
    public bool UseAbility(Ability ability, CombatGridTile targetTile)
    {
        ClearCharacterPreviews();

        // Set caster to get information that might alter ability, like extra AOE range.
        _pendingAbility.SetCharacterCaster(_characterCaster);

        _tilesInRange = RemoveUntargetableTiles(GetAvailableTargets(_pendingAbility));
        if (!CanCastAbility(ability, targetTile))
        {
            ClearAbilityTargetRange();
            if (_bDebugAbilityHandler)
                DebugLog.MGLog("Tried casting ability, but it failed");
            return false;
        }
        Selector._instance.InvokeCharacterActionStarted();

        _characterCaster.CanUseAbility = false;
        CombatEventManager.InvokeOnAbilityCast();
        StartCoroutine(ability.StartAbilityEffects(_casterTile, targetTile));
        _characterCaster.StartAbilityCooldown(ability);
        return true;
    }

    public void PreviewAbility(Ability ability, CombatGridTile targetTile)
    {
        _pendingAbility.SetCharacterCaster(_characterCaster);
        _tilesInRange = RemoveUntargetableTiles(GetAvailableTargets(_pendingAbility));
        if (!CanCastAbility(ability, targetTile))
        {
            return;
        }
        ability.PreviewAbilityEffects(_casterTile, targetTile);
    }
    public Character GetCharacterCaster()
    {
        return _characterCaster;
    }
    /// <summary>
    /// Returns the cached list of tiles currently in range for the pending ability.
    /// Note: Range must be calculated beforehand, otherwise the list may be empty.
    /// </summary>
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

    public List<Character> GetPreviewedCharacters() => _previewedCharacters;


    /// <summary>
    /// Calculates all tiles that the pending ability can target from the caster's position.
    /// Fetches tiles from the ability’s range calculation and filters out untargetable tiles.
    /// Saves the result into _tilesInRange.
    /// </summary>
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
        _pendingAbility.SetCharacterCaster(_characterCaster);
        _tilesInRange = RemoveUntargetableTiles(GetAvailableTargets(_pendingAbility));
    }

    /// <summary>
    /// Checks whether the ability can legally be cast on the given tile.
    /// Ensures the tile is valid for this ability AND is inside the computed range.
    /// </summary>
    public bool CanCastAbility(Ability ability, CombatGridTile targetTile)
    {
        return IsValidTargetTileForAbility(ability, targetTile) && _tilesInRange.Contains(targetTile);

    }

    /// <summary>
    /// Helper wrapper that asks the ability to compute which tiles are in range
    /// from the caster’s tile using its RangeCalculation.
    /// </summary>
    private List<CombatGridTile> GetAvailableTargets(Ability ability)
    {
        return ability.GetAvailableTargets(_casterTile);
    }

    /// <summary>
    /// Determines whether the tile is a valid target for the pending ability.
    /// Checks walkability, occupant type (enemy/friendly), and custom "targetable" rules.
    /// </summary>
    public bool IsValidTargetTileForAbility(Ability ability, CombatGridTile tile)
    {
        if (tile == null) return false;

        var occupant = tile.GetOccupant();
        Character character = occupant ? occupant.GetComponent<Character>() : null;

        var abilityType = ability.GetAbilityTargetType();

        if (abilityType != Ability.ValidTargetOccupant.Any && CharacterNotTargetable(character)) return false;

        switch (abilityType)
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
    /// Removes tiles that cannot be targeted (e.g., unwalkable tiles, caster tile when needed)
    /// from the list returned by the range calculation.
    /// </summary>
    private List<CombatGridTile> RemoveUntargetableTiles(List<CombatGridTile> tiles)
    {
        List<CombatGridTile> filteredList = new();
        foreach (CombatGridTile tile in tiles)
        {
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

        bool shouldClearPreview = true;
        if(newEffectedTiles != null)
        {
            shouldClearPreview = false;
            foreach(CombatGridTile t in newEffectedTiles)
            {
                if (!_tilesEffected.Contains(t)) shouldClearPreview = true;
            }
        }
        if(shouldClearPreview) ClearCharacterPreviews();

        // Reset all tiles
        foreach (CombatGridTile t in _tilesEffected)
        {
            if (_tilesInRange.Contains(t))
            {
                t.SetTileColor(Color.green);
            }
            else
            {
                t.SetTileColor(Color.white);
            }
        }
        _tilesEffected.Clear();
        if (newEffectedTiles == null){ return; }

        // Paint new tiles red and add them to tilesEffected.
        foreach (CombatGridTile t in newEffectedTiles)
        {
            if (t == null) return;
            t.SetTileColor(Color.red);
            _tilesEffected.Add(t);
        }
    }

    /// <summary>
    /// Returns false if the character cannot currently be targeted by abilities.
    /// Used as an extra layer of validation on top of regular targeting rules.
    /// For use cases such as when Stealth is activated on a character.
    /// </summary>
    private bool CharacterNotTargetable(Character targetCharacter)
    {
        if (targetCharacter == null) return false;

        if (targetCharacter.GetFaction() == _characterCaster.GetFaction()) return false;

        return !targetCharacter.IsTargetable;
    }

    public void AddPreviewedCharacter(Character character)
    {
        _previewedCharacters.Add(character);
    }

    private void ClearCharacterPreviews()
    {
        foreach (var c in _previewedCharacters)
        {
            c.HidePreviewVFX();
            c.StopPreviewingHealthChange();
        }

        _previewedCharacters.Clear();
    }

    private void HandleCharacterDeselected()
    {
        ClearCharacterPreviews();
    }

}

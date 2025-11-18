using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

public class AbilityHandler : MonoBehaviour
{
    [SerializeField] private List<Ability> _abilities;

    private List<CombatGridTile> _tilesInRange = new();
    private List<CombatGridTile> _tilesEffected = new();
    Dictionary<CombatGridTile, Color> tileColorMap = new();
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
        // Körs hela tiden och uppdateras alltså konstant. Eftersom den måste uppdateras medan man hovrar med musen.


        // Sätt 1.

        // Kolla alla tiles som kan träffas och lägg till dem i en lista.

        // Om listan på klassnivå _tileEffected är tom. Lägg till elementen i den listan och färga dem röda.

        // Gå igenom alla tiles i listan som är sparad på klass nivå: _tilesEffected.

        // Om den finns i den nya listan, gör ingenting.

        // Om den inte finns, kolla om den finns i den andra listan på klass nivå: _tilesInRange.

        // Om den finns i den andra listan, färga tilen grön, om den inte finns, färga tilen vit.

       // Gå sedan igenom den nya listan, om klass listan _tilesEffected inte innehåller ett element, färga den röd och lägg till den.

        List<CombatGridTile> newEffectedTiles = _pendingAbility.GetTilesToEffect(tile);

        if (!_tilesEffected.Any())
        {
            foreach(CombatGridTile t in newEffectedTiles)
            {
                _tilesEffected.Add(t);
                t.SetTileColor(Color.red);
            }
            return;
        }
        var copiedList = new List<CombatGridTile>(_tilesEffected);
        foreach (CombatGridTile t in copiedList)
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

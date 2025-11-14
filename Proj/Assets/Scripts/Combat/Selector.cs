using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

    public enum SelectorState
    {
        NonActive,
        PlacingCharacters,
        Idle,  
        CharacterSelected,
        ActionTypeSelected,
    }

public class Selector : MonoBehaviour
{
    public static Selector _instance {  get; private set; }
    
    [SerializeField] private CombatUI _combatUI;
    [SerializeField] private SelectorState _currentState = SelectorState.NonActive;
    [SerializeField] private CharacterActionType _pendingCharacterActionType = CharacterActionType.Null;
    [SerializeField] private Character _selectedCharacter;
    [SerializeField] private bool _bDebugSelector = false;
    public enum CharacterActionType
    {
        Null,
        Movevement,
        AbilityCasting
    } 

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
        }
    }

    void Update()
    {
        HandleTileClick();
        HandleTileHover();
        DebugCurrentState();
    }

    public Character GetSelectedCharacter()
    {
        return _selectedCharacter;
    }
    public void SetSelectedCharacter(Character selectedCharacter)
    {
        _selectedCharacter = selectedCharacter;
    }
    

    private void HandleTileClick()
    {
        // Execute different actions based on current state when clicking on tiles.
        if (Camera.main == null)
        {
            Debug.LogError("No MainCamera found! Tag your camera as 'MainCamera'."); return;
        }
        if (EventSystem.current == null)
        {
            Debug.LogError("No EventSystem in scene!"); return;
        }

        // Return early if mouse is over UI element or current state is NonActive.
        if (_currentState == SelectorState.NonActive) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (Input.GetMouseButtonDown(0))
        {
            CombatGridTile clickedTile = GetTileUnderMouse();
            if (_bDebugSelector && clickedTile != null)
            {
                DebugLog.MGLog("Clicked on tile " + clickedTile.gameObject);
            }
            switch (_currentState)
            {
                case SelectorState.NonActive: break;
                case SelectorState.PlacingCharacters: SetSelectedCharacterForPlacement(); break;
                case SelectorState.Idle: TrySelectCharacter(clickedTile); break;
                case SelectorState.CharacterSelected: DeselectCharacter(); break;
                case SelectorState.ActionTypeSelected: HandlePendingCharacterAction(clickedTile); break;
            }
        }
    }
    private void HandleTileHover()
    {
        // Return early if mouse is over UI element.
        if (EventSystem.current.IsPointerOverGameObject()) return;

        // Show info about character.
        CombatGridTile hoveredTile = GetTileUnderMouse();
        if (hoveredTile == null) return;

        if (hoveredTile.GetOccupant() != null && hoveredTile.GetOccupant().TryGetComponent<Character>(out var character)){
            // TODO: Call UIControll script to show character info on character position.
        }

        // TODO: Change state on tiles (with matching color) to indicate aoe abilities effected area.
        // if _currentState = SelectorState.ActionTypeSelected && hovoredTile = in range
    }

    public CombatGridTile GetTileUnderMouse()
    {
        // Cast ray cast from mouse to detect tile and return it if found.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int tileMask = LayerMask.GetMask("Tile");

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, tileMask))
        {
            // Check for tile script on gameobject.
            if (hit.collider.TryGetComponent(out CombatGridTile tile)) return tile;
        }
        return null;
    }
    public CombatGridTile GetTileClicked()
    {
        // Get clicked tile.
        if(!Input.GetMouseButtonDown(0)) return null;

        return GetTileUnderMouse();
    }
    public CombatGridTile GetUnoccupiedDeployTileClicked()
    {
        if (!Input.GetMouseButtonDown(0)) return null;

        CombatGridTile tile = GetTileUnderMouse();
        if (tile && tile.GetTileType() == TileType.Deploy && tile.GetOccupant() == null)
        {
            return tile;
        }
        else
        {
            return null;
        }
    }
    public void ResetSelectedCharacter()
    {
        _selectedCharacter = null;
    }

    private void SetSelectedCharacterForPlacement()
    {
        CombatGridTile tile = GetTileUnderMouse();
        if(tile && tile.GetOccupantCharacter() != null)
        {
            _selectedCharacter = tile.GetOccupantCharacter();
        }
    }

    private void TrySelectCharacter(CombatGridTile tile)
    {
        Character character = tile?.GetOccupantCharacter();
        if(character == null)
        {
            DeselectCharacter();
            return;
        }
        SelectCharacter(character);
    }

    public void UpdateTileColors(GameObject[] tiles)
    {

        foreach (GameObject tile in tiles)
        {
            if (tile.GetComponent<CombatGridTile>().IsMouseHovering())
            {
                tile.GetComponent<CombatGridTile>().SetTileColor(Color.yellow);
            }
            else if (tile.GetComponent<CombatGridTile>().GetOccupant())
            {
                tile.GetComponent<CombatGridTile>().SetTileColor(Color.green);
            }
            else
            {
                tile.GetComponent<CombatGridTile>().SetTileColor(Color.white);
            }
        }
    } 
    private void SelectCharacter(Character character)
    {
        bool bIsFriendly = character.GetFaction() == Faction.Friendly;
        bool bIsCharactersTurn = character == CombatManager._instance.GetNextTurnCharacter();

        if (bIsFriendly && bIsCharactersTurn)
        {
            ShowCharacterUIOptions(character);
            _currentState = SelectorState.CharacterSelected;
            _selectedCharacter = character;

            if (_bDebugSelector)
            {
                DebugLog.MGLog(character.GetCharacterClass().ToString() + " on tile index: " + character.GetCurrentTileIndex().ToString());
            }
        }
    }
    private void DeselectCharacter()
    {
        // if ui is active Deactivate UI
        HideCharacterOptions(_selectedCharacter);

        StopPreviewAbilityRange(_selectedCharacter);
        _selectedCharacter?.GetAbilityHandler().SetPendingAbility(null);
        _selectedCharacter = null;
        _pendingCharacterActionType = CharacterActionType.Null;
        

        if (CombatManager._instance.GetCombatTurn() == CombatTurn.PlayerTurn)
        {
            _currentState = SelectorState.Idle;
        }
        else
        {
            _currentState = SelectorState.NonActive;
        }

        if (_bDebugSelector) DebugLog.MGLog("Deselect Character");
    }

    private void ShowCharacterUIOptions(Character character)
    {
        // Activate UI and place it to show over characters head.
        _combatUI.LoadAbilities(character);
    }
    public void PreviewAbilityRange(Character character, Ability ability)
    {
        if (_selectedCharacter != null && _selectedCharacter.TryGetComponent<AbilityHandler>(out var abilityHandler))
        {
            SetColorOfTiles(abilityHandler.GetTilesInRange(), Color.green);
        }
    }
    public void StopPreviewAbilityRange(Character character)
    {
        if (_selectedCharacter != null && _selectedCharacter.TryGetComponent<AbilityHandler>(out var abilityHandler))
        {
            SetColorOfTiles(abilityHandler.GetTilesInRange(), Color.white);
        }
    }
    private void HideCharacterOptions(Character Character)
    {
        // Deactivate UI and reset tile color.
        if (_selectedCharacter != null && _selectedCharacter.TryGetComponent<AbilityHandler>(out var abilityHandler))
        {
            SetColorOfTiles(abilityHandler.GetTilesInRange(), Color.white);
            abilityHandler.ClearAbilityTargets();
        }
    }
    private void HandlePendingCharacterAction(CombatGridTile tile)
    {
        if (_selectedCharacter == null)
        {
            Debug.LogWarning("Tried to handle action with no selected character");
            return;
        }

        if (_pendingCharacterActionType == CharacterActionType.Movevement)
        {
            HandleMovement(tile);
            return;
        }
        if (_pendingCharacterActionType == CharacterActionType.AbilityCasting && _selectedCharacter.GetAbilityHandler().GetPendingAbility() != null)
        {
            HandleAbilityCast(tile);
            return;
        }

        if (_bDebugSelector)
        {
            DebugLog.MGLog("Pending character action: " + _currentState.ToString() + " failed");
        }
        DeselectCharacter();
    }
    private void HandleMovement(CombatGridTile tile)
    {
        _selectedCharacter.SetMoveTarget(tile);
        if (_bDebugSelector)
        {
            DebugLog.MGLog(_selectedCharacter.GetCharacterClass() + " on tile: " + _selectedCharacter.GetCurrentTileIndex().ToString() + " is set to move to: " + tile.GetComponentIndex().ToString());
        }
    }

    private void HandleAbilityCast(CombatGridTile tile)
    {
        bool success = _selectedCharacter.GetComponentInParent<AbilityHandler>().UseAbility(_selectedCharacter.GetAbilityHandler().GetPendingAbility(), tile);
        if (_bDebugSelector && success)
        {
            DebugLog.MGLog(_selectedCharacter.GetCharacterClass() + " used ability: " + _selectedCharacter.GetAbilityHandler().GetPendingAbility().GetAbilityName().ToString());
        }
        if (!success)
        {
            DeselectCharacter();
            _currentState = SelectorState.Idle;
        }
    }
    private void DebugCurrentState()
    {
        if(_bDebugSelector) DebugLog.MGLog("The current state is: " + GetCurrentState().ToString());
    }

    private void SetColorOfTiles(List<CombatGridTile> tiles, Color color)
    {
        foreach (CombatGridTile tile in tiles)
        {
            if (tile != null)
            {
                tile.SetTileColor(color);
            }
        }
    }
    public SelectorState GetCurrentState() { return _currentState; }
    public void SetCurrentState(SelectorState state) {  _currentState = state; }
}

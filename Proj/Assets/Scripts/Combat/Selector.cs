using System.Collections.Generic;
using Unity.VisualScripting;
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
    private void Start()
    {
        DebugPossibleStartErrors();
        CombatEventManager.OnCombatStateChange += HandleCombatStateUpdated;
        CombatEventManager.OnCombatTurnChange += HandleCombatTurnChanged;
    }

    void Update()
    {
        HandleTileClick();
        HandleTileHover();
    }

    private void HandleCombatStateUpdated(CombatState state)
    {
        switch (state)
        {
            case CombatState.IntroCinematic: _currentState = SelectorState.NonActive; break;
            case CombatState.PlaceCharacters: _currentState = SelectorState.PlacingCharacters;break;
            case CombatState.TakeTurn: break;
            case CombatState.EndCombat: _currentState = SelectorState.NonActive; break;
        }
    }
    private void HandleCombatTurnChanged(CombatTurn turn)
    {
        if (turn == CombatTurn.PlayerTurn)
        {
            _currentState = SelectorState.Idle;
        }
        else
        {
            _currentState = SelectorState.NonActive;
        }
    }

    public Character GetSelectedCharacter()
    {
        return _selectedCharacter;
    }
    public void SetSelectedCharacter(Character selectedCharacter)
    {
        _selectedCharacter = selectedCharacter;
    }

    /// <summary>
    /// Handles left-click interactions on tiles.  
    /// Behavior depends on the current selector state, such as selecting,
    /// deselecting, or executing pending actions.
    /// </summary>
    private void HandleTileClick()
    {
        // Execute different actions based on current state when clicking on tiles.

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
                case SelectorState.PlacingCharacters: SelectCharacter(clickedTile); break;
                case SelectorState.Idle: SelectCharacter(clickedTile); break;
                case SelectorState.CharacterSelected: DeselectCharacter(); break;
                case SelectorState.ActionTypeSelected: HandlePendingCharacterAction(clickedTile); break;
            }
        }
    }
    /// <summary>
    /// Handles tile hover logic.  
    /// Shows character information when hovering over a tile with an occupant  
    /// and will later be used for visualizing ability AoE or ranges.
    /// </summary>
    private void HandleTileHover()
    {
        // Return early if mouse is over UI element.
        if (EventSystem.current.IsPointerOverGameObject()) return;

        // Show info about character.
        CombatGridTile hoveredTile = GetTileUnderMouse();
        if (hoveredTile == null) return;

        GameObject characterObject = hoveredTile.GetOccupant();
        if (characterObject == null) return;
    

        if (characterObject.TryGetComponent<Character>(out var character)){
            // TODO: Call UIControll script to show character info on character position.
        }

        // TODO: Change state on tiles (with matching color) to indicate aoe abilities effected area.
        // if _currentState = SelectorState.ActionTypeSelected && hovoredTile = in range
        if (_currentState == SelectorState.ActionTypeSelected && characterObject.TryGetComponent<AbilityHandler>(out var abilityHandler))
        {
            if (abilityHandler.GetPendingAbility() == null) return;

            abilityHandler.PreviewTargetTiles(hoveredTile);
        }
       
    }

    /// <summary>
    /// Returns the tile currently under the mouse cursor using a raycast.  
    /// If no tile is detected, returns null.
    /// </summary>
    /// <returns>The tile under the mouse, or null if none was hit.</returns>
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

    /// <summary>
    /// Attempts to select a character based on the tile clicked.  
    /// Uses the current selector state to determine the appropriate selection behavior.
    /// </summary>
    /// <param name="tile">The tile that was clicked.</param>
    public void SelectCharacter(CombatGridTile tile)
    {
        // Selects the charater from the tile clicked. Checks state before to see which type of selection is appropriate.
        switch (_currentState)
        {
            case SelectorState.PlacingCharacters: SetSelectedCharacterPlacement(tile); break;
            case SelectorState.Idle: TrySelectCharacterIdle(tile); break;
        }
    }

    /// <summary>
    /// Selects a character directly from the UI.  
    /// Uses the current selector state to determine how the character should be selected.
    /// </summary>
    /// <param name="character">The character selected through UI.</param>
    public void SelectCharacterUI(Character character)
    {
        // Selects the charater from the UI buttons. Checks state before to see which type of selection is appropriate.
        if (character == null) return;

        switch (_currentState)
        {
            case SelectorState.PlacingCharacters: SetSelectedCharacterPlacementUI(character); break;
            case SelectorState.Idle: SelectCharacterIdle(character); break;
        }
    }
    private void SetSelectedCharacterPlacement(CombatGridTile tile)
    {
        if (_currentState != SelectorState.PlacingCharacters)
        {
            Debug.LogError("Wrong selecting method was called when selecting character. Method not matching state.");
            return;
        }

        if (tile && tile.GetOccupantCharacter() != null)
        {
            _selectedCharacter = tile.GetOccupantCharacter();
        }
    }

    private void SetSelectedCharacterPlacementUI(Character character)
    {
        if (_currentState != SelectorState.PlacingCharacters)
        {
            Debug.LogError("Wrong selecting method was called when selecting character. Method not matching state.");
            return;
        }

        if (_selectedCharacter != null && _currentState == SelectorState.Idle && character.GetFaction() != Faction.Friendly)
        {
            DeselectCharacter();
        }
        _selectedCharacter = character;
    }

    private void TrySelectCharacterIdle(CombatGridTile tile)
    {
        Character character = tile?.GetOccupantCharacter();
        if(character == null)
        {
            DeselectCharacter();
            return;
        }
        SelectCharacterIdle(character);
    }
    private void SelectCharacterIdle(Character character)
    {
        bool bIsFriendly = character.GetFaction() == Faction.Friendly;
        bool bIsCharactersTurn = character == CombatManager._instance.GetNextTurnCharacter();

        if (bIsFriendly && bIsCharactersTurn)
        {
            ShowCharacterUIWithOptions(character);
            _currentState = SelectorState.CharacterSelected;
            _selectedCharacter = character;

            if (_bDebugSelector)
            {
                DebugLog.MGLog(character.GetCharacterClass().ToString() + " on tile index: " + character.GetCurrentTileIndex().ToString());
            }

            return;
        }

        if (bIsFriendly)
        {
            ShowCharacterUI(character);
        }
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
    private void DeselectCharacter()
    {
        // if ui is active Deactivate UI
        HideCharacterOptions();

        StopPreviewAbilityRange();
        _selectedCharacter?.GetAbilityHandler()?.SetPendingAbility(null);
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

    /// <summary>
    /// Activates the character UI and displays all available actions,
    /// such as abilities and movement options, for the selected character.
    /// </summary>
    /// <param name="character">The character whose options should be shown.</param>
    private void ShowCharacterUIWithOptions(Character character)
    {
        // Activates character UI with options to cast abilities and walk.
        _combatUI.LoadAbilities(character.Data);
        _combatUI.UpdateSelectedPortrait(character.Data);
    }

    /// <summary>
    /// Activates the character UI without any action options.  
    /// Used when the character cannot perform actions at the moment.
    /// </summary>
    /// <param name="character">The character to display basic UI for.</param>
    private void ShowCharacterUI(Character character)
    {
        // Activates character UI without options since the character can't perform actions at the moment.

        _combatUI.UpdateSelectedPortrait(character.Data);
    }
    public void PreviewAbilityRange(Ability ability)
    {
        if (_selectedCharacter != null && _selectedCharacter.TryGetComponent<AbilityHandler>(out var abilityHandler))
        {
            abilityHandler.SetPendingAbility(ability);
            abilityHandler.CalculateAbilityRange();
            SetColorOfTiles(abilityHandler.GetTilesInRange(), Color.green);
        }
    }
    public void StopPreviewAbilityRange()
    {
        if (_selectedCharacter != null && _selectedCharacter.TryGetComponent<AbilityHandler>(out var abilityHandler))
        {
            SetColorOfTiles(abilityHandler.GetTilesInRange(), Color.white);
        }
    }

    /// <summary>
    /// Hides the character's action UI and resets any visual indicators,
    /// such as highlighted tiles or ability range previews.
    /// </summary>
    private void HideCharacterOptions()
    {
        // Deactivate UI and reset tile color.

        // Here could a method to deactivate UI for specific character be placed if we want to remove UI when deselecting.

        if (_selectedCharacter != null && _selectedCharacter.TryGetComponent<AbilityHandler>(out var abilityHandler))
        {
            SetColorOfTiles(abilityHandler.GetTilesInRange(), Color.white);
            abilityHandler.ClearAbilityTargetRange();
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
    private void DebugPossibleStartErrors()
    {
        if (Camera.main == null)
        {
            Debug.LogError("No MainCamera found! Tag your camera as 'MainCamera'."); return;
        }
        if (EventSystem.current == null)
        {
            Debug.LogError("No EventSystem in scene!"); return;
        }
    }
 
    public SelectorState GetCurrentState() { return _currentState; }
    public void SetCurrentState(SelectorState state) {  _currentState = state; }

    public void SetCharacterActionType(CharacterActionType actionType) { _pendingCharacterActionType = actionType;  }
}

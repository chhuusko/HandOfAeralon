using System;
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
    public static Selector _instance { get; private set; }

    [SerializeField] private CombatUI _combatUI;
    [SerializeField] private SelectorState _currentState = SelectorState.NonActive;
    [SerializeField] private CharacterActionType _pendingCharacterActionType = CharacterActionType.Null;
    [SerializeField] private Character _selectedCharacter;
    [SerializeField] private bool _bDebugSelector = true;
    private CharacterMovement _characterMovement;

    public event Action<Character> OnCharacterSelected;
    public event Action OnCharacterDeselected;
    public event Action OnCharacterActionStarted;
    public event Action OnCharacterActionStopped;



    public enum CharacterActionType
    {
        Null,
        Movement,
        AbilityCasting
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
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
        CombatEventManager.OnExitCombatStateTakeTurn += HandleCombatStateTakeTurn;
        CombatEventManager.OnEnterCombatStateTakeTurn += HandleEnterCombatStateTakeTurn;
    }

    void Update()
    {
        HandleTileClick();
        HandleTileHover();
    }

    private void HandleEnterCombatStateTakeTurn(Character character)
    {
        SelectCharacterFromUI(character);
    }

    private void HandleCombatStateTakeTurn()
    {
        DeselectCharacter();
    }

    private void HandleCombatStateUpdated(CombatState state)
    {
        switch (state)
        {
            case CombatState.IntroCinematic: _currentState = SelectorState.NonActive; break;
            case CombatState.PlaceCharacters: _currentState = SelectorState.PlacingCharacters; break;
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
            if (clickedTile ==null) return;

            if (_bDebugSelector && clickedTile != null)
            {
                DebugLog.MGLog("Clicked on tile " + clickedTile.gameObject);
            }
            switch (_currentState)
            {
                case SelectorState.NonActive: break;
                case SelectorState.PlacingCharacters: SelectCharacterFromTile(clickedTile); break;
                case SelectorState.Idle: SelectCharacterFromTile(clickedTile); break;
                case SelectorState.CharacterSelected: SelectCharacterFromTile(clickedTile); break;
                case SelectorState.ActionTypeSelected: HandlePendingCharacterAction(clickedTile); break;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            DeselectCharacter();
        }
    }
    /// <summary>
    /// Handles tile hover logic.  
    /// Shows character information when hovering over a tile with an occupant  
    /// and will later be used for visualizing ability AoE or ranges.
    /// </summary>
    private void HandleTileHover()
    {
        // JLW
        if (_characterMovement)
        {
            _characterMovement.PreviewPath(GetTileUnderMouse());
        }
        else
        {
            GridExplorer._instance.ClearPathDrawing();
        }

        // Return early if mouse is over UI element.
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // Show info about character.
        CombatGridTile hoveredTile = GetTileUnderMouse();
        if (hoveredTile == null)
        {
            return;
        }

        // Show hovered character info.
        GameObject occupant = hoveredTile.GetOccupant();
        if (occupant != null && occupant.TryGetComponent<Character>(out var character))
        {
            // TODO: Show character info in UI.
        }

        // Change color on tiles to indicate aoe abilities effected area.
        if (_currentState == SelectorState.ActionTypeSelected)
        {
            if (_selectedCharacter == null) return;
            AbilityHandler handler = _selectedCharacter.GetAbilityHandler();

            if (handler == null || handler.GetPendingAbility() == null) return;
            handler.PreviewTargetTiles(hoveredTile);
            handler.PreviewAbility(handler.GetPendingAbility(), hoveredTile);
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
        if (!Input.GetMouseButtonDown(0)) return null;

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
    public void SelectCharacterFromTile(CombatGridTile tile)
    {
        // Selects the charater from the tile clicked. Checks state before to see which type of selection is appropriate.

        if (_currentState == SelectorState.PlacingCharacters)
        {
            SelectPlacementCharacterFromTile(tile);
        }
        else
        {
            TrySelectCharacterFromTile(tile);
        }
    }

    /// <summary>
    /// Selects a character directly from the UI.  
    /// Uses the current selector state to determine how the character should be selected.
    /// </summary>
    /// <param name="character">The character selected through UI.</param>
    public void SelectCharacterFromUI(Character character)
    {
        // Selects the charater from the UI buttons. Checks state before to see which type of selection is appropriate.
        if (character == null) return;

        if (_currentState == SelectorState.PlacingCharacters)
        {
            SelectPlacementCharacterFromUI(character);
        }
        else
        {
            SelectCharacter(character);
        }
    }
    private void SelectPlacementCharacterFromTile(CombatGridTile tile)
    {
        if (_currentState != SelectorState.PlacingCharacters)
        {
            Debug.LogError("Wrong selecting method was called when selecting character. Method not matching state.");
            return;
        }
        Character character = tile?.GetOccupantCharacter();
        if (character?.GetFaction() == Faction.Friendly)
        {
            _selectedCharacter = tile.GetOccupantCharacter();
            ShowCharacterUI(character);
        }
    }

    private void SelectPlacementCharacterFromUI(Character character)
    {
        if (_currentState != SelectorState.PlacingCharacters)
        {
            Debug.LogError("Wrong selecting method was called when selecting character. Method not matching state.");
            return;
        }

        _selectedCharacter = character;
        ShowCharacterUI(character);
    }

    /// <summary>
    /// Attempts to select a character from the given tile.  
    /// If the tile is empty, the current selection is cleared.
    /// </summary>
    /// <param name="tile">The tile to check for a character.</param>
    private void TrySelectCharacterFromTile(CombatGridTile tile)
    {
        Character character = tile?.GetOccupantCharacter();
        if (character == null)
        {
            DeselectCharacter();
            return;
        }
        SelectCharacter(character);
    }

    /// <summary>
    /// Handles character selection logic.  
    /// Updates selector state, shows character UI and activates movement logic  
    /// if it is the selected character's turn.
    /// </summary>
    /// <param name="character">The character to select.</param>
    private void SelectCharacter(Character character)
    {
        DeselectCharacter();

        // Update selected character and show it's related UI.
        _selectedCharacter = character;
        ShowCharacterUI(character);
        _currentState = SelectorState.CharacterSelected;

        // Check to see if character is friendly before checking to activate movement.
        if (character.GetFaction() != Faction.Friendly) return;

        bool bIsCharactersTurn = character == CombatManager._instance.GetCombatTurnOrder().GetActiveCharacter();

        // If it's the characters turn, activate logic.
        if (bIsCharactersTurn)
        {
            _pendingCharacterActionType = CharacterActionType.Movement;
            _currentState = SelectorState.ActionTypeSelected;

            // JLW
            _characterMovement = character.GetComponent<CharacterMovement>();
            if (_characterMovement != null)
            {
                //DebugLog.JLWLog($"Selector.cs | Drawing move range for {character.name}");
                _characterMovement.DrawMoveRange();
            }

            if (_bDebugSelector)
            {
                DebugLog.MGLog(character.GetCharacterClass().ToString() + " on tile index: " + character.GetCurrentTileIndex().ToString());
            }
            return;
        }
    }

    /// <summary>
    /// Clears the currently selected character and resets all related state and visuals.  
    /// Hides character UI, stops ability previews, resets tile colors and updates selector state  
    /// based on the current combat state.
    /// </summary>
    private void DeselectCharacter()
    {
        OnCharacterDeselected?.Invoke();

        // if ui is active Deactivate UI
        HideCharacterOptions();

        StopPreviewAbilityRange();
        _selectedCharacter?.GetAbilityHandler()?.SetPendingAbility(null);
        _selectedCharacter = null;
        _characterMovement = null;
        _pendingCharacterActionType = CharacterActionType.Null;
        

        if (_currentState == SelectorState.PlacingCharacters)
        {
            // Stay in placement phase.
            _currentState = SelectorState.PlacingCharacters;
        }
        else if (CombatManager._instance.GetCombatTurnOrder().GetCurrentTurn() == CombatTurn.PlayerTurn)
        {
            // Back to idle if it's players turn.
            _currentState = SelectorState.Idle;
            ResetColorAllTiles();
        }
        else
        {
            _currentState = SelectorState.NonActive;
            ResetColorAllTiles();
        }

        if (_bDebugSelector) DebugLog.MGLog("Deselect Character");
    }

    /// <summary>
    /// Activates the character UI without any action options.  
    /// Used when the character cannot perform actions at the moment.
    /// </summary>
    /// <param name="character">The character to display basic UI for.</param>
    private void ShowCharacterUI(Character character)
    {
        // Activates character UI without options since the character can't perform actions at the moment.
        OnCharacterSelected?.Invoke(character);
    }
    public void PreviewAbilityRange(Ability ability)
    {
        if (_selectedCharacter != null && _selectedCharacter.TryGetComponent<AbilityHandler>(out var abilityHandler))
        {
            _characterMovement.ForgetMoveRange();
            ResetColorAllTiles();
            _pendingCharacterActionType = CharacterActionType.AbilityCasting;
            _currentState = SelectorState.ActionTypeSelected;
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

        if (_pendingCharacterActionType == CharacterActionType.Movement)
        {
            HandleMovement(tile); // JLW
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
        _characterMovement.ConfirmPath(tile);

        ResetColorAllTiles();
        // MG was here.
        Character character = tile.GetOccupantCharacter();
        if (character == null) return;
        SelectCharacter(character);
        // Hade varit nice om ConfirmPath kunde returna true eller false om den faktiskt lockar in en rutt och b�rjar g�.
    }

    private void HandleAbilityCast(CombatGridTile tile)
    {
        bool success = _selectedCharacter.GetComponentInParent<AbilityHandler>().UseAbility(_selectedCharacter.GetAbilityHandler().GetPendingAbility(), tile);

        if (success)
        {
            InvokeCharacterActionStarted();
        }

        _selectedCharacter?.GetAbilityHandler()?.SetPendingAbility(null);
        _pendingCharacterActionType = CharacterActionType.Null;
        ResetColorAllTiles();
        _currentState = CombatManager._instance.GetCombatTurnOrder().GetCurrentTurn() == CombatTurn.PlayerTurn? SelectorState.Idle: _currentState = SelectorState.NonActive;
    
        if (_bDebugSelector && success)
        {
            DebugLog.MGLog(_selectedCharacter.GetCharacterClass() + " used ability: " + _selectedCharacter.GetAbilityHandler().GetPendingAbility().GetAbilityName().ToString());
        }
        if (_bDebugSelector && !success)
        {
            DebugLog.MGLog(_selectedCharacter.GetCharacterClass() + " failed to use ability: " + _selectedCharacter.GetAbilityHandler().GetPendingAbility().GetAbilityName().ToString());
        }
    }

    public void SetColorOfTiles(List<CombatGridTile> tiles, Color color)
    {
        foreach (CombatGridTile tile in tiles)
        {
            if (tile != null)
            {
                tile.SetTileColor(color);
            }
        }
    }
    private void ResetColorAllTiles()
    {
        GameObject[] tileObjects = CombatGrid._instance.GetAllTiles();
        List<CombatGridTile> tiles = new();

        foreach (GameObject obj in tileObjects)
        {
            if (obj.TryGetComponent<CombatGridTile>(out var tile))
            {
                tiles.Add(tile);
            }
        }
        SetColorOfTiles(tiles, Color.white);
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

    public void InvokeCharacterActionStarted()
    {
        OnCharacterActionStarted?.Invoke();
    }
    public void InvokeCharacterActionStopped()
    {
        OnCharacterActionStopped?.Invoke();
    }

    public SelectorState GetCurrentState() { return _currentState; }
    public void SetCurrentState(SelectorState state) { _currentState = state; }

    public void SetCharacterActionType(CharacterActionType actionType) { _pendingCharacterActionType = actionType; }
}

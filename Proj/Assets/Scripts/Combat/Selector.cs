using UnityEngine;
using UnityEngine.EventSystems;

public class Selector : MonoBehaviour
{
    public static Selector _instance {  get; private set; }

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

    public enum SelectorState
    {
        NonActive,
        Idle,  
        CharacterSelected,
        ActionTypeSelected,
    }
    public enum CharacterActionType
    {
        Null,
        Movevement,
        AbilityCasting
    } 

    private SelectorState _currentState = SelectorState.NonActive;
    private CharacterActionType _pendingCharacterActionType = CharacterActionType.Null;
    private Character _selectedCharacter;
    private Ability _pendingAbility;
    private bool _bDebugSelector = false;
    

    void Start()
    {
        _bDebugSelector = true;
    }

    
    void Update()
    {
        HandleTileClick();
        HandleTileHover();
        DebugCurrentState();
    }

    private void HandleTileClick()
    {
        // Execute different actions based on current state when clicking on tiles.
        if (Camera.main == null)
        {
            Debug.LogError("No MainCamera found! Tag your camera as 'MainCamera'.");
            return;
        }
        if (EventSystem.current == null)
        {
            Debug.LogError("No EventSystem in scene!");
            return;
        }

        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (Input.GetMouseButtonDown(0))
        {
            CombatGridTile clickedTile = GetTileUnderMouse();
            if (_bDebugSelector && clickedTile != null)
            {
                Debug.Log("Clicked on tile " + clickedTile.gameObject);
            }
            switch (_currentState)
            {
                case SelectorState.NonActive: break;
                case SelectorState.Idle: TrySelectCharacter(clickedTile); break;
                case SelectorState.CharacterSelected: break;
                case SelectorState.ActionTypeSelected: HandlePendingCharacterAction(clickedTile); break;
            }
        }
    }
    private void HandleTileHover()
    {
        // Show info about character.
        CombatGridTile hoveredTile = GetTileUnderMouse();
        if (hoveredTile == null || hoveredTile.GetOccupant() == null) return;

        if (hoveredTile.GetOccupant().TryGetComponent<Character>(out var character)){
            // TODO: Call UIControll script to show character info on character position.
        }
    }

    private CombatGridTile GetTileUnderMouse()
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

    private void TrySelectCharacter(CombatGridTile tile)
    {
       if(!tile.GetOccupant().TryGetComponent<Character>(out var character))
        {
            DeselectCharacter();
            return;
        }

        bool bIsFriendly = character.GetFaction() == Faction.Friendly;
        bool bIsCharactersTurn = character == CombatManager._instance.GetNextTurnCharacter();

        if (bIsFriendly && bIsCharactersTurn)
        {
            ShowCharacterOptions(character);
            _currentState = SelectorState.CharacterSelected;
            _selectedCharacter = character;
        }
    }
    private void DeselectCharacter()
    {
        // if ui is active Deactivate UI

        _selectedCharacter = null;
        _pendingAbility = null;
        

        if (_bDebugSelector) Debug.Log("DeselectCharacter was run");
    }

    private void ShowCharacterOptions(Character Character)
    {
        // Activate UI and place it to show over characters head.
    }
    private void HandlePendingCharacterAction(CombatGridTile tile)
    {
        if(_pendingCharacterActionType == CharacterActionType.Movevement)
        {
            _selectedCharacter.SetMoveTarget(tile);
        }
        if(_pendingCharacterActionType == CharacterActionType.AbilityCasting && _pendingAbility != null)
        {
            _selectedCharacter.GetComponentInParent<AbilityHandler>().UseAbility(_pendingAbility, tile);
        }
    }
    private void DebugCurrentState()
    {
        if(_bDebugSelector) Debug.Log("The current state is: " + GetCurrentState().ToString());
    }
    public SelectorState GetCurrentState() { return _currentState; }
    public void SetCurrentState(SelectorState state) {  _currentState = state; }
    public void SetPendingAbility(Ability ability)
    {
        _pendingAbility = ability;
    }
}

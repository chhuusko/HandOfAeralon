using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;

public class Selector : MonoBehaviour
{
    public static Selector Instance {  get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public enum SelectorState
    {
        NonActive,
        Idle,  
        CharacterSelected,
        ActionTypeSelected,
    }

    private SelectorState _currentState = SelectorState.NonActive;
    private Character _selectedCharacter; 

    void Start()
    {
        
    }

    
    void Update()
    {
        HandleTileClick();
        HandleTileHover();
    }

    private void HandleTileClick()
    {   
        // Execute different actions based on current state when clicking on tiles.
        if (Input.GetMouseButtonDown(0))
        {
            CombatGridTile clickedTile = GetTileUnderMouse();
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
    }

    private CombatGridTile GetTileUnderMouse()
    {
        // Check mouse position, cast ray cast to detect tile and return it if found.
        
        return null;
    }

    private void TrySelectCharacter(CombatGridTile tile)
    {
        if(tile.GetOccupant().TryGetComponent<Character>(out var character)){
            // check if it's the characters turn and the character is friendly
            // If so, enable UI
            if(character.GetFaction() == Faction.Friendly /* && character.IsCharactersTurn*/)
            {

            }
            ShowCharacterOptions(character);
            _currentState = SelectorState.CharacterSelected;
            _selectedCharacter = character;
        }

    }

    private void ShowCharacterOptions(Character Character)
    {
        // Activate UI and place it to show over characters head.
    }
    private void HandlePendingCharacterAction(CombatGridTile tile)
    {
        // If Action type == move 
        //_selectedCharacter.SetMoveTarget(tile);

        // If Action type == Ability && _pendingAbility != null
        // _selectedCharacter.AbilityHandler.UseAbility(_pendingAbility, tile);
    }
    public SelectorState GetCurrentState() { return _currentState; }
    public void SetCurrentState(SelectorState state) {  _currentState = state; }
}

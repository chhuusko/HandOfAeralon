using Unity.VisualScripting;
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
        PendingCharacterAction
    }

    private SelectorState CurrentState = SelectorState.NonActive;

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
            switch (CurrentState)
            {
                case SelectorState.NonActive: break;
                case SelectorState.Idle: TrySelectCharacter(clickedTile); break;
                case SelectorState.CharacterSelected: break;
                case SelectorState.PendingCharacterAction: break;
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
            ShowCharacterOptions(character);
        }
        CurrentState = SelectorState.CharacterSelected;

    }

    private void ShowCharacterOptions(Character Character)
    {
        // Activate UI and place it to show over characters head.
    }
    public SelectorState GetCurrentState() { return CurrentState; }
    public void SetCurrentState(SelectorState state) {  CurrentState = state; }
}

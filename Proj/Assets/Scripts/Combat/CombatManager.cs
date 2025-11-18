using System;
using System.Collections.Generic;
using System.IO;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

[System.Serializable]
public enum CombatTurn
{
    PlayerTurn,
    EnemyTurn
};

[System.Serializable]
public enum PlayerTurnMode
{
    CharacterMode,
    CardMode
};

[System.Serializable]
public struct ClassAbilities
{
    public CharacterClass characterClass;
    public List<Ability> abilities;
}

public class CombatManager : MonoBehaviour
{
    public static CombatManager _instance;
    private Selector _selector;

    // NOTE (Calle): Could Pre load each CombatState here and make them public so 
    // each ICombatState derived class can access them
    // 
    // Ex:
    // public CombatStateLoadLevel _combatStateLoadLevel;
    // 
    // void Awake()
    // {
    //     _combatStateLoadLevel = new CombatStateLoadLevel();
    // }

    [SerializeField] private string _fileToLoadDEBUG;

    [SerializeField] private CombatCamera _combatCamera;
    [SerializeField] private float _cameraSpeed;

    [SerializeField] private ICombatState _currentCombatState;
    [SerializeField] private CombatState _currentCombatStateEnum;

    private CombatState _combatState;
    [SerializeField] private CombatTurn _currentTurn;
    [SerializeField] private PlayerTurnMode _currentPlayerTurnMode;
    [SerializeField] private GameObject _activeCharacter;

    [Header("Abilities")]
    [SerializeField] private List<ClassAbilities> _classAbilities;
    private Dictionary<CharacterClass, List<Ability>> _classAbilitiesDictionary;

    public UnityEvent TurnStart = new();
    
    private void Awake()
    {
        if (_instance == null)
        {
            Debug.Log("CombatManager Awake()");
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        _classAbilitiesDictionary = new Dictionary<CharacterClass, List<Ability>>();
        foreach (var pair in _classAbilities)
        {
            _classAbilitiesDictionary[pair.characterClass] = pair.abilities;
        }
    }

    private void OnEnable()
    {   
        CombatUI.Instance.OnStartCombatButtonPressed += StartTakingTurns;
        CombatUI.Instance.OnEndTurnButtonPressed += ChangeCurrentTurn;
    }

    private void OnDisable()
    {
        CombatUI.Instance.OnStartCombatButtonPressed -= StartTakingTurns;
        CombatUI.Instance.OnEndTurnButtonPressed -= ChangeCurrentTurn;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _combatState = CombatState.LoadCombatLevel;
        _selector = GetComponent<Selector>();

        ChangeCombatState(new CombatStateLoadLevel());
    }

    // Update is called once per frame
    void Update()
    {
        _currentCombatState?.Update();

        switch (_combatState)
        {
            case CombatState.LoadCombatLevel:
                {
                    //HandleLoadCombatLevel();
                }
                break;
            case CombatState.IntroCinematic:
                {
                   // HandleIntroCinematic();
                } break;
            case CombatState.PlaceCharacters:
                {
                    //HandlePlaceCharacters();
                } break;
            case CombatState.TakeTurn:
                {
                    HandleTakeTurn();
                } break;
            case CombatState.EndTurn:
                {
                    HandleEndTurn();
                } break;
            case CombatState.EndCombat:
                {
                    HandleEndCombat();
                } break;
        }
    }

    public void ChangeCombatState(ICombatState newCombatState)
    {
        _currentCombatState?.Exit();
        _currentCombatState = newCombatState;
        _currentCombatStateEnum = newCombatState._state;
        // NOTE (Calle): Broadcast the state change.
        CombatEventManager.CombatStateChanged(newCombatState._state);

        newCombatState?.Enter();
    }

    public CombatState GetCombatState()
    {
        return _combatState;
    }

    public CombatCamera GetCombatCamera() { return _combatCamera; }
    public Selector GetCombatSelector() { return _selector; }
    /// <summary>
    /// Gets all abilities available to the class.
    /// </summary>
    /// <param name="characterClass">The character class to get abilities for.</param>
    /// <returns>A list of the class' available abilities.</returns>
    public List<Ability> GetClassAbilities(CharacterClass characterClass)
    {
        return _classAbilitiesDictionary.TryGetValue(characterClass, out var abilities) ? abilities : new List<Ability>();
    }

    public GameObject GetNextTurnCharacter()
    {
        int highestInitiative = Int32.MinValue;
        GameObject nextCharacter = null;
        foreach (var g in CombatGrid._instance.GetAllCharacters())
        {
            int initiative = g.GetComponent<Character>().GetSpeed();
            if (initiative > highestInitiative)
            {
                highestInitiative = initiative;
                nextCharacter = g;
            }
        }

        return nextCharacter;
    }

    private void SetCurrentTurn(CombatTurn turn)
    {
        _currentTurn = turn;
        CombatEventManager.CombatTurnChanged(turn);
    }

    private void ChangeCurrentTurn()
    {
        if (_currentTurn == CombatTurn.PlayerTurn)
            _currentTurn = CombatTurn.EnemyTurn;
        else
            _currentTurn -= CombatTurn.PlayerTurn;
    }

    private void HandleTakeTurn()
    {
        // NOTE (Calle): Only wan't to set the _activeCharacter once each turn
        if(_activeCharacter == null)
        {
            // NOTE (Calle): Set current turn based on initiative and Faction
            _activeCharacter = GetNextTurnCharacter();
            //if(IsCharacterFriendly)
            if (_activeCharacter.GetComponent<Character>().GetFaction() == Faction.Friendly)
            {
                SetCurrentTurn(CombatTurn.PlayerTurn);
                CardHandManager._instance.ChangeMana(1);
            }
            else if (_activeCharacter.GetComponent<Character>().GetFaction() == Faction.Enemy)
                SetCurrentTurn(CombatTurn.EnemyTurn);
 
        }
         
        switch (_currentTurn)
        {
            case CombatTurn.PlayerTurn:
                HandlePlayerTurn();
                break;
            case CombatTurn.EnemyTurn:
                HandleEnemyTurn();
                break;
        }
    }

    private void HandlePlayerTurn()
    {
        // TODO (Calle): 
        //  Vid starten av varje hero karakt�rs turn sker dessa saker: 
        //  - Spelarens mana �kar med 1 -> I CardHandManager()
        //  - Hero karakt�rens ability cooldowns minskar med 1 -> WIP (MG/JOPPA)
        //  - Spelarens "cooldown" / timer f�r att dra ett till kort minskar med 1 -> WIP 

        // TODO: Call selector with character.
        
        //TurnStart.Invoke(); // Säger till AI att en ny tur börjat, Eventet broadcastas både här och i HandleEnemyTurn() för att AI ska kunna spela båda factions.
        switch(_currentPlayerTurnMode)
        {
            case PlayerTurnMode.CharacterMode:
                break;
            case PlayerTurnMode.CardMode:
                break;
        }
    }

    bool enemyDoingStuff = false;
    private void HandleEnemyTurn()
    {
        if(!enemyDoingStuff)
        {
            enemyDoingStuff = true;
            TurnStart.Invoke(); // Säger till AI att en ny tur börjat, Eventet broadcastas både här och i HandlePlayerTurn() för att AI ska kunna spela båda factions.
            _activeCharacter = null;
            //UpdateCombatState(CombatState.EndTurn);
        }
    }

    public void HandleEndTurn()
    {
        // TODO (Calle): If all characters are dead, either friendly or enemies or Quit Game?
        //               transition to EndCombat State.
        //               If not, transition to TakeTurn again

        // NOTE (Calle): Check End Combat conditions
        if(CombatGrid._instance.GetAllEnemyCharacters().Count == 0)
        {
            // TODO (Calle): All enemies killed, do something specific to that.
            UpdateCombatState(CombatState.EndCombat);
        }
        else if(CombatGrid._instance.GetAllFriendlyCharacters().Count == 0)
        {
            // TODO (Calle): All heroes killed, do something specific to that.
            UpdateCombatState(CombatState.EndCombat);
        }
        else
        {
            // TODO (Calle): Continue with next turn, do we need to do anything else specific?
            UpdateCombatState(CombatState.TakeTurn);
        }
    }

    private void HandleEndCombat()
    {

    }

    private void StartTakingTurns()
    {
        UpdateCombatState(CombatState.TakeTurn);
    }

    private void LoadCurrentPlayerParty()
    {
        
    }

    private void EvaluateInitiativeOrder()
    {

    }
    public CombatGridTile GetTileComponent(int x, int y)
    {
        GameObject tileObject = CombatGrid._instance.GetTileAtCoord(x, y);
        if (tileObject == null) return null;

        return tileObject.GetComponent<CombatGridTile>();
    }

    public CombatTurn GetCombatTurn()
    {
        return _currentTurn;
    }

    public void SetCombatTurn(CombatTurn turn) 
    { 
        _currentTurn = turn; 
    }

    public void UpdateCombatState(CombatState state)
    {
        if (_combatState != state)
        {
            _combatState = state;
            CombatEventManager.CombatStateChanged(state);
        }
    }
}

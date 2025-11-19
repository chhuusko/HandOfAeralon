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

    private CombatState _combatState;

    [Header("Combat State")]
    [SerializeReference] private CombatStateBase _currentCombatState;
    [SerializeField] private CombatState _currentCombatStateEnum;


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
    }

    private void OnDisable()
    {
    }

    void Start()
    {
        _combatState = CombatState.LoadCombatLevel;
        _selector = GetComponent<Selector>();

        ChangeCombatState(new CombatStateLoadLevel());
    }

    void Update()
    {
        _currentCombatState?.Update();
    }

    public void ChangeCombatState(CombatStateBase newCombatState)
    {
        _currentCombatState?.Exit();
        _currentCombatState = newCombatState;
        _currentCombatStateEnum = newCombatState._state;
        // NOTE (Calle): Broadcast the state change.
        CombatEventManager.InvokeCombatStateChanged(newCombatState._state);

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

    private void ChangeCurrentTurn()
    {
        if (_currentTurn == CombatTurn.PlayerTurn)
            _currentTurn = CombatTurn.EnemyTurn;
        else
            _currentTurn -= CombatTurn.PlayerTurn;
    }

    public GameObject GetActiveCharacter() { return _activeCharacter; }

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
}

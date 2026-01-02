using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    // NOTE (Calle): Could Pre load each CombatState here and let each combat state
    // access them through the CombatManager when changing combat state,
    // 
    // Ex:
    // public CombatStateLoadLevel _combatStateLoadLevel;
    // 
    // void Awake()
    // {
    //     _combatStateLoadLevel = new CombatStateLoadLevel();
    // }
    // 
    // In for example CombatStateLoadNextLevel's Update()
    //
    // if(CombatGrid._instance.IsCombatGridLoaded())
    // {
    //     CombatManager._instance.ChangeCombatState(CombatManager._instance.GetCombatState(CombatState.IntroCinematic));
    // }
    //
    // public CombatStateBase GetCombatState(CombatState combatState)
    // {
    //     switch(combatState)
    //     {
    //         case CombatState.IntroCinematic:
    //             return _combatStateIntroCinematic;
    //             break;
    //     }
    // }
    //
    //

    public static CombatManager _instance;
    private Selector _selector;

    [SerializeField] private CombatCamera _combatCamera;

    private GameObject _selectorOverHead;
    [SerializeField] private GameObject _selectorOverHeadPrefab;
    [SerializeField] private Vector3 _selectorOverHeadStartPos;

    private GameObject _selectorCube;
    [SerializeField] private GameObject _selectorCubePrefab;
    [SerializeField] private Character _currentSelectedCharacter;



    [Header("Combat State")]
    [SerializeField] private CombatState _debugCurrentState;
    [SerializeReference] private CombatStateBase _currentCombatState;
   

    private Dictionary<CharacterData, Character> _dataToCharacterDict;

    [Header("Combat Turn Order")]
    [SerializeField] private CombatTurnOrder _combatTurnOrder;

    [Header("Combat Turn Order")]
    private AI_Core _aiCore;

    [Header("Abilities")]
    [SerializeField] private List<ClassAbilities> _classAbilities;
    private Dictionary<CharacterClass, List<Ability>> _classAbilitiesDictionary;

    [Header("Enemy base stats")] 
    public int enemyMana = 6;
    public int enemyManaSpent = 2;

    public UnityEvent TurnStart = new();

    private void Awake()
    {
        if (_instance == null)
        {
            DebugLog.CJLog("CombatManager Awake()");
            _instance = this;
            //DontDestroyOnLoad(gameObject);
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
        _dataToCharacterDict = new Dictionary<CharacterData, Character>();
        _combatTurnOrder = new CombatTurnOrder();
    }

    private void OnEnable()
    {
        CombatEventManager.OnExitCombatStateEndCombat += HandleEndCombat;
    }

    private void OnDisable()
    {
        CombatEventManager.OnExitCombatStateEndCombat -= HandleEndCombat;
    }

    void Start()
    {
        _aiCore = AI_Core.BuildAICore(Faction.Enemy);

        _selector = GetComponent<Selector>();
        _selectorOverHead = Instantiate(_selectorOverHeadPrefab, Vector3.zero, Quaternion.identity);
        _selectorOverHead.SetActive(false);

        _selectorCube = Instantiate(_selectorCubePrefab, Vector3.zero, Quaternion.identity);
        Vector3 pos = _selectorCube.transform.position;
        pos.y = _selectorCube.transform.localScale.y / 2.0f;
        _selectorCube.transform.position = pos;
        _selectorCube.SetActive(false);

        ChangeCombatState(new CombatStateLoadLevel());        
    }

    void Update()
    {
        if (Time.timeScale <= 0.0f)
            return;
        if(_currentCombatState != null)
        {
            _currentCombatState?.Update();
            Character activeCharacter = _combatTurnOrder.GetActiveCharacter();
            if (activeCharacter)
            {
                InitiativeHoverSphere hoverSphere = _selectorOverHead.GetComponent<InitiativeHoverSphere>();
                _selectorOverHead?.SetActive(true);
                Vector3 selectorOverHeadPosition = activeCharacter.transform.position + (Vector3.up * 3.0f);
                hoverSphere.SetPosition(selectorOverHeadPosition);
                hoverSphere.SetHoverStartPosition(selectorOverHeadPosition);
                hoverSphere.SetHoverSpherePosition(selectorOverHeadPosition);
                hoverSphere.UpdatePosition();
            }
        }


        Character selectedCharacter = _selector.GetSelectedCharacter();
        if (selectedCharacter != null)
        {
            _currentSelectedCharacter = _selector.GetSelectedCharacter();
        }
        else
        {
            _currentSelectedCharacter = null;
        }


        if (_currentSelectedCharacter != null)
        {
            if (_selectorCube != null)
            {
                _selectorCube.SetActive(true);
                Vector3 pos = _currentSelectedCharacter.gameObject.transform.position;
                pos.y = _selectorCube.transform.localScale.y / 2.0f;
                _selectorCube.transform.position = pos;
            }
        }
        else
        {
            _selectorCube.SetActive(false);
        }
    }

    public void ChangeCombatState(CombatStateBase newCombatState)
    {
        _currentCombatState?.Exit();
        _currentCombatState = newCombatState;
        _debugCurrentState = newCombatState._state;
        // NOTE (Calle): Broadcast the state change.
        CombatEventManager.InvokeCombatStateChanged(newCombatState._state);

        newCombatState?.Enter();
    }

    private void HandleEndCombat(bool playerWon)
    {
        _currentCombatState = null;
        LevelManager.GetInstance().StartNextLevel();
    }

    public CombatState GetCombatState()
    {
        return _currentCombatState.GetState();
    }

    public CombatCamera GetCombatCamera() { return _combatCamera; }
    public GameObject GetSelectorOverHead() { return _selectorOverHead; }
    
    public Selector GetCombatSelector() { return _selector; }
    public AI_Core GetEnemyAI() { return _aiCore; }
    public CombatTurnOrder GetCombatTurnOrder() { return _combatTurnOrder; }
    public Dictionary<CharacterData, Character> GetCharacterDataDict()
    {
        return _dataToCharacterDict;
    }

    public void InitializeCharacterDataDict()
    {
        _dataToCharacterDict.Clear();

        List<CharacterData> characterDataList = GlobalGameManager.GetInstance().GetGameData().heroDataList;

        List<CombatGridTile> deployTiles = CombatGrid._instance.GetAllDeployTiles();

        CombatGridTile[] offGridTiles = CombatGrid._instance.GetOffGridTiles();

        // NOTE (Calle): only placing heroes on the first deploytiles in the list.
        int deployTileIndex = 0;
        foreach(CharacterData data in characterDataList)
        {
            Character playerHero = CombatGrid._instance.SpawnCharacter(data, 
                                                                       deployTiles[deployTileIndex].GetTilePosition(),
                                                                       Quaternion.Euler(0.0f, 90.0f, 0.0f));
            playerHero.Initialize(data);

            _dataToCharacterDict.Add(data, playerHero);

            //playerHero.gameObject.transform.position = new Vector3(-1, deployTileIndex++, 0);
            playerHero.gameObject.transform.position = offGridTiles[deployTileIndex++].GetTilePosition();
        }
    }

    public void SetSelectorOverHeadColor(Color color)
    {
        InitiativeHoverSphere hoverSphere = _selectorOverHead.GetComponent<InitiativeHoverSphere>();
        hoverSphere.SetHoverOverheadColor(color);
    }

    /// <summary>
    /// Gets all abilities available to the class.
    /// </summary>
    /// <param name="characterClass">The character class to get abilities for.</param>
    /// <returns>A list of the class' available abilities.</returns>
    public List<Ability> GetClassAbilities(CharacterClass characterClass)
    {
        return _classAbilitiesDictionary.TryGetValue(characterClass, out var abilities) ? abilities : new List<Ability>();
    }

    //public GameObject GetNextTurnCharacter()
    //{
    //    int highestInitiative = Int32.MinValue;
    //    GameObject nextCharacter = null;
    //    foreach (var g in CombatGrid._instance.GetAllCharacters())
    //    {
    //        int initiative = g.GetComponent<Character>().GetInitiative();
    //        if (initiative > highestInitiative)
    //        {
    //            highestInitiative = initiative;
    //            nextCharacter = g;
    //        }
    //    }
    //
    //    return nextCharacter;
    //}

    public CombatGridTile GetTileComponent(int x, int y)
    {
        GameObject tileObject = CombatGrid._instance.GetTileAtCoord(x, y);
        if (tileObject == null) return null;

        return tileObject.GetComponent<CombatGridTile>();
    }
}

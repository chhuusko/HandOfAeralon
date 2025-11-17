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
public enum CombatState
{
    IntroCinematic,
    LoadCombatLevel,
    PlaceCharacters,
    TakeTurn,
    EndTurn,
    EndCombat
};

[System.Serializable]
public enum CombatTurn
{
    PlayerTurn,
    EnemyTurn
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
    
    [SerializeField] private string _fileToLoadDEBUG;

    [SerializeField] private CombatCamera _combatCamera;
    [SerializeField] private float _cameraSpeed;

    [SerializeField] private CombatState _combatState;
    [SerializeField] private CombatTurn _currentTurn;
    [SerializeField] private GameObject _activeCharacter;

    [SerializeField] private bool _combatGridLoaded = false;

    private GameObject _friendlyCharacterRoot;
    private GameObject _enemyCharacterRoot;
    private GameObject _tileRoot;


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
        _friendlyCharacterRoot = new GameObject();
        _friendlyCharacterRoot.name = "-PLAYER PARTY-";
        _enemyCharacterRoot= new GameObject();
        _enemyCharacterRoot.name = "-ENEMY CHARACTERS-";
        _tileRoot = new GameObject();
        _tileRoot.name = "-GRID TILES-";
    }

    
    // Update is called once per frame
    void Update()
    {
        MoveCamera();

        switch (_combatState)
        {
            case CombatState.LoadCombatLevel:
                {
                    HandleLoadCombatLevel();

                    
                }
                break;
            case CombatState.IntroCinematic:
                {
                    HandleIntroCinematic();
                } break;
            case CombatState.PlaceCharacters:
                {
                    HandlePlaceCharacters();
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

    public CombatState GetCombatState()
    {
        return _combatState;
    }

    private void MoveCamera()
    {
        Vector3 cameraMovement = Vector3.zero;
        Vector3 cameraSpeedVector = new Vector3(_cameraSpeed, _cameraSpeed, _cameraSpeed);
        
        if (Input.GetKey(KeyCode.D))
            cameraMovement += Vector3.right;
        if (Input.GetKey(KeyCode.A))
            cameraMovement += Vector3.left;
        if (Input.GetKey(KeyCode.W))
            cameraMovement += Vector3.forward;
        if (Input.GetKey(KeyCode.S))
            cameraMovement += Vector3.back;

        cameraMovement = Vector3.Scale(cameraMovement, cameraSpeedVector);
        
        cameraMovement *= Time.deltaTime;

        if(cameraMovement != Vector3.zero)
            _combatCamera.transform.position = cameraMovement + _combatCamera.transform.position;
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

    private void HandleIntroCinematic()
    {   
        if (_combatCamera.IsIntroCinematicDone())
            UpdateCombatState(CombatState.PlaceCharacters);
        else
            _combatCamera.PlayIntroCinematic();
    }

    private void HandleLoadCombatLevel()
    {
        if(!_combatGridLoaded)
        {
            _combatGridLoaded = true;
           
            // TODO (Calle): Detta ska g�ra i LevelManagern
            LoadNextLevel();
            //LoadCurrentPlayerParty();
            UpdateCombatState(CombatState.IntroCinematic);

            Vector2Int tileIndex = new Vector2Int(5, 0);
            Vector3 position = new Vector3(1.0f + tileIndex.x * 2.0f, 0.0f, 1.0f + tileIndex.y * 2.0f);
            CombatGridCharacterData characterData = new CombatGridCharacterData(CharacterClass.Wizard,
                                                                               Faction.Friendly,
                                                                               10,
                                                                               1,
                                                                               tileIndex,
                                                                               position,
                                                                               Vector3.one,
                                                                               Quaternion.identity);

            //_combatGrid.AddCharacter(characterData).transform.SetParent(_friendlyCharacterRoot.transform);
            CombatGrid._instance.AddCharacter(characterData).transform.SetParent(_friendlyCharacterRoot.transform);
            tileIndex.x = 6;
            position.x += 2.0f;
            characterData.SetTileIndex(tileIndex);
            characterData.SetPosition(position);
            CombatGrid._instance.AddCharacter(characterData).transform.SetParent(_friendlyCharacterRoot.transform);

        }

    }

    private void HandlePlaceCharacters()
    {
        if(_selector)
        {
            // Return early if mouse is over UI element.
            if (EventSystem.current.IsPointerOverGameObject()) return;
            _selector.SetCurrentState(SelectorState.PlacingCharacters);

            _selector.UpdateTileColors(CombatGrid._instance.GetAllTiles());
            
            CombatGridTile unoccupiedDeployTile = _selector.GetUnoccupiedDeployTileClicked();
            Character selectedCharacter = _selector.GetSelectedCharacter();
            
            if(selectedCharacter)
            {
                if (unoccupiedDeployTile)
                {
                    Vector2Int tileIndex = unoccupiedDeployTile.GetTileIndex();
                    Vector3 tilePosition = unoccupiedDeployTile.GetTilePosition();

                    if (CombatGrid._instance.ContainsCharacter(selectedCharacter.gameObject))
                    {
                        selectedCharacter.gameObject.transform.position = tilePosition;
                        selectedCharacter.SetCurrentTileIndex(tileIndex);
                    }
                    else
                    {                     
                        CombatGridCharacterData characterData = new CombatGridCharacterData(CharacterClass.Wizard,
                                                                                            Faction.Friendly,
                                                                                            10,
                                                                                            1,
                                                                                            tileIndex,
                                                                                            tilePosition,
                                                                                            Vector3.one,
                                                                                            Quaternion.identity);

                        CombatGrid._instance.AddCharacter(characterData).transform.SetParent(_friendlyCharacterRoot.transform);
                    }
                }
                else
                {
                    DebugLog.CJLog("Show ERROR UI to place on a deploy tile.");
                }
            }
        }
        
        //_selector.ResetSelectedCharacter();
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

    private void LoadNextLevel()
    {
        string filePathToLoad = Application.streamingAssetsPath + "/JSON/BattleGrids/" + _fileToLoadDEBUG + ".json";

        if (!System.IO.File.Exists(filePathToLoad))
        {
            DebugLog.CJLog("Level File didn't exist or filepath was wrong!");
            return;
        }

        string jsonFileData = System.IO.File.ReadAllText(filePathToLoad);
        if(jsonFileData.Length == 0)
        {
            DebugLog.CJLog("json File Data was empty!");
            return;
        }

        CombatGridSerializedSaveData combatGridSaveData = JsonUtility.FromJson<CombatGridSerializedSaveData>(jsonFileData);

        CombatGrid._instance.SetCombatGridSize(combatGridSaveData._gridWidth, combatGridSaveData._gridHeight);
        CombatGrid._instance.SetTileSize(combatGridSaveData._tileSize);
        DebugLog.CJLog("CombatGrid tileSize: " + combatGridSaveData._tileSize);

        for (int i = 0; i < combatGridSaveData._tileData.Count; i++)
        {
            DebugLog.CJLog("tiled["+i+"]: " + "\tTileType : " + combatGridSaveData._tileData[i].GetTileType() + 
                      "\tTileIndex: " + combatGridSaveData._tileData[i].GetTilePosition() + "\n");

            CombatGrid._instance.AddTile(combatGridSaveData._tileData[i]).transform.SetParent(_tileRoot.transform);
            
        }

        GameObject NavMesh = GameObject.Find("NavMesh Surface");
        NavMesh.GetComponent<NavMeshSurface>().BuildNavMesh();

        for (int i = 0; i < combatGridSaveData._characterData.Count; i++)
        {
            CombatGrid._instance.AddCharacter(combatGridSaveData._characterData[i]).transform.SetParent(_enemyCharacterRoot.transform); ;
        }
        
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

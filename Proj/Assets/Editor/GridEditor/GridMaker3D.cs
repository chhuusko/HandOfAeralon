using Codice.CM.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static GridMaker3D;
using static UnityEditor.PlayerSettings;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class GridMaker3D : EditorWindow
{
    const int DRAWMODE_TILE = 0, DRAWMODE_CHARACTER = 1;
    
    [SerializeField] string _strRootObjectForTiles      = "-BATTLE GRID-";
    [SerializeField] string _strRootObjectForCharacters = "-CHARACTERS-";
    
    GridMaker3D _instance;
    [SerializeField] private CharacterPrefabLibrary _characterPrefabLibrary;
    [SerializeField] private TilePrefabLibrary _tilePrefabLibrary;

    LayerMask _tileLayerMask;

    string _fileNameToSaveJSON;
    string _fileNameToLoadJSON;

    int _drawMode = 0;
    int _battleGridWidth = 0;
    int _battleGridHeight= 0;
    int _prevBattleGridWidth = 0;
    int _prevBattleGridHeight = 0;
    int _defaultDeployZoneWidth = 0;

    Vector2 _windowEditorScrollPos;
    Vector3 _tileSizeInMeters;


    [System.Serializable]
    public class CharacterEntry
    {
        public Vector3 _position;
        public Vector3 _size;
        public Quaternion _rotation;
        public Vector2Int _tileIndex;
        public CharacterClass _characterClass;
        public GameObject _character;

        public CharacterEntry() { }
        public CharacterEntry(Vector3 goPos, Vector3 goSize, Quaternion rotation, GameObject prefab, GameObject parent, Vector2Int gridPos)
        {
            if (prefab != null)
            {
                // GameObject Specific
                this._character = Instantiate(prefab);
                this._character.transform.position = goPos;
                this._character.transform.localScale = goSize;
                this._character.transform.rotation = rotation;
                this._character.GetComponent<Character>().SetCurrentTileIndex(gridPos);

                if (parent != null)
                    this._character.transform.SetParent(parent.transform);

               

                // Save/Load Specific
                this._characterClass = prefab.GetComponent<Character>().GetCharacterClass();
                this._tileIndex = gridPos;
                this._position = goPos;
                this._size = goSize;
            }
        }
    };

    [System.Serializable]
    public class CharacterList : ScriptableObject
    {
        public List<CharacterEntry> _characterList = new List<CharacterEntry>();
    };

    CharacterList       _characterList;
    SerializedObject    _characterListSO;
    SerializedProperty  _characterListProperty;

    [System.Serializable]
    public class CharacterBrushPrefabHolder : ScriptableObject
    {
        public List<GameObject> _characterBrushPrefabs = new List<GameObject>();
    };

    CharacterBrushPrefabHolder _characterBrushPrefabHolder;
    SerializedObject           _characterBrushPrefabHolderSO;
    SerializedProperty         _characterBrushPrefabProperty;

    [System.Serializable]
    public class TileEntry
    {
        public Vector2Int _tileIndex;
        public Vector3 _position;
        public Vector3 _size;
        public TileType _tileType;
        public GameObject _tile;
        public GameObject _occupant;

        public TileEntry() { }
        public TileEntry(Vector3 goPos, Vector3 goSize, GameObject prefab, GameObject parent, Vector2Int gridPos)
        {
            if(prefab != null)
            {
                // GameObject specific
                this._tile = Instantiate(prefab);
                this._tile.transform.position = goPos;
                this._tile.transform.localScale = goSize;
                this._tile.GetComponent<CombatGridTile>().SetTileIndex(gridPos);
                if(parent != null)
                    this._tile.transform.SetParent(parent.transform);

                // Save/Load Data Specific
                this._tileType = prefab.GetComponent<CombatGridTile>().GetTileType();
                this._tileIndex = gridPos;
                this._position = goPos;
                this._size = goSize;
                this._occupant = null;
            }
        }
    };

    [System.Serializable]
    public class TileGrid : ScriptableObject
    {
        public List<TileEntry> _tileEntries = new List<TileEntry>();
    };

    TileGrid            _tileGridHolder;
    SerializedObject    _tileGridHolderSO;
    SerializedProperty  _tileGridProperty;

    [System.Serializable]
    public class TileBrushPrefabHolder : ScriptableObject
    {
        public List<GameObject> _tileBrushPrefabs = new List<GameObject>();
    }

    TileBrushPrefabHolder _tileBrushPrefabHolder;
    SerializedObject      _tileBrushPrefabHolderSO;
    SerializedProperty    _tileBrushPrefabProperty;

    [SerializeField] GameObject _defaultTile;
    [SerializeField] GameObject _defaultDeployTile;

    TileEntry _previewTile;
    CharacterEntry _previewCharacter;
    
    int _currentTileBrushIndex = 0;
    int _currentCharacterBrushIndex = 0;

    private bool _bIsHoldingF  = false, _bIsHoldingCtrl = false, _bInFocusMode = false;
    private bool _bFocusToggle = false;
    private bool _bDrawPreviewGrid = true;

    [MenuItem("Custom Tools/GridMaker 3D")]
    public static void ShowWindow()
    {
        GetWindow<GridMaker3D>("GridMaker 3D");
    }

    private void OnEnable()
    {
        AssemblyReloadEvents.beforeAssemblyReload += CleanupPreviewObjects;
        EditorApplication.quitting                += CleanupPreviewObjects;
        SceneView.duringSceneGui                  += OnSceneGUI;

        _instance = this;
        _previewTile = new TileEntry();
        _previewCharacter = new CharacterEntry();
        _tileLayerMask = LayerMask.GetMask("EditorTile");
  
        // create a temporary in-memory object
        _tileBrushPrefabHolder = ScriptableObject.CreateInstance<TileBrushPrefabHolder>();
        _tileBrushPrefabHolderSO = new SerializedObject(_tileBrushPrefabHolder);
        _tileBrushPrefabProperty = _tileBrushPrefabHolderSO.FindProperty("_tileBrushPrefabs");

        _characterBrushPrefabHolder = ScriptableObject.CreateInstance<CharacterBrushPrefabHolder>();
        _characterBrushPrefabHolderSO = new SerializedObject(_characterBrushPrefabHolder);
        _characterBrushPrefabProperty = _characterBrushPrefabHolderSO.FindProperty("_characterBrushPrefabs");

        _tileGridHolder = ScriptableObject.CreateInstance<TileGrid>();
        _tileGridHolderSO = new SerializedObject(_tileGridHolder);
        _tileGridProperty = _tileGridHolderSO.FindProperty("_tileEntries");

        _characterList = ScriptableObject.CreateInstance<CharacterList>();
        _characterListSO = new SerializedObject(_characterList);
        _characterListProperty = _characterListSO.FindProperty("_characterList");

        // Note (Calle): Preloading some default editor values, for the same reason as preloading the defaultTile.
        _tileSizeInMeters = new Vector3(2.0f, 0.01f, 2.0f);
        _battleGridHeight = 10;
        _battleGridWidth = 12;
        _defaultDeployZoneWidth = 3;

        // Note (Calle): Preloading the default tile so we don't have to manually assign it every time we open the editor
        string defaultTileFilePath = "Assets/Prefabs/Tiles/BattleGridTile_Walkable.prefab";
        _defaultTile = AssetDatabase.LoadAssetAtPath<GameObject>(defaultTileFilePath);

        string defaultDeployTilePath = "Assets/Prefabs/Tiles/BattleGridTile_Deploy.prefab"; ;
        _defaultDeployTile = AssetDatabase.LoadAssetAtPath<GameObject>(defaultDeployTilePath);

        // Note (Calle): Must be done after the SerializeObject Array has been Created. Preloading all tile brushes
        string folderPathTilePrefabs = "Assets/Prefabs/Tiles";
        string[] strTilePrefabGUIDS = AssetDatabase.FindAssets("t:Prefab", new[] { folderPathTilePrefabs });
        foreach (string guid in strTilePrefabGUIDS)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject tileObject = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            DebugLog.CJLog("TileName: " + tileObject.name);
            _tileBrushPrefabHolder._tileBrushPrefabs.Add(tileObject);
            _tileBrushPrefabHolderSO.Update();
        }

        string folderPathCharacterPrefabs = "Assets/Prefabs/Characters/Enemy";
        string[] strCharacterPrefabGUIS = AssetDatabase.FindAssets("t:prefab", new string[] { folderPathCharacterPrefabs });
        foreach(string guid in strCharacterPrefabGUIS)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath (guid);
            GameObject characterObject = AssetDatabase.LoadAssetAtPath<GameObject> (assetPath);
            DebugLog.CJLog("CharacterName" + characterObject.name);
            _characterBrushPrefabHolder._characterBrushPrefabs.Add(characterObject);
            _characterBrushPrefabHolderSO.Update();
        }

        _characterPrefabLibrary = AssetDatabase.LoadAssetAtPath<CharacterPrefabLibrary>("Assets/ScriptableObject/Characters/CharacterPrefabLibrary.asset");
        _tilePrefabLibrary      = AssetDatabase.LoadAssetAtPath<TilePrefabLibrary>("Assets/ScriptableObject/Tiles/TilePrefabLibrary.asset");

    }

    private void OnDisable()
    {
        AssemblyReloadEvents.beforeAssemblyReload -= CleanupPreviewObjects;
        EditorApplication.quitting -= CleanupPreviewObjects;

        if (_previewTile != null && _previewTile._tile != null)
            DestroyImmediate(_previewTile._tile);

        if(_previewCharacter != null && _previewCharacter._character != null)
            DestroyImmediate(_previewCharacter._character);

        GameObject parent = GameObject.Find(_strRootObjectForTiles);
        if (parent != null)
            DestroyImmediate(parent);

        parent = GameObject.Find(_strRootObjectForCharacters);
        if (parent != null)
            DestroyImmediate(parent);

        SceneView.duringSceneGui -= OnSceneGUI;
    }

    // NOTE (Calle): This one is for clering the ghost previewTiles that could occur
    //               prob cuz when switching between focus mode!
                    
    private void CleanupPreviewObjects()
    {
        // Clean preview character and tile directly
        if (_previewCharacter != null && _previewCharacter._character != null)
            DestroyImmediate(_previewCharacter._character);
        if (_previewTile != null && _previewTile._tile != null)
            DestroyImmediate(_previewTile._tile);

        // Also nuke all hidden preview objects globally
        foreach (var go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (go.name.Contains("PreviewCharacter") || go.name.Contains("PreviewTile"))
                DestroyImmediate(go);
        }

        // Optional: if you add a preview root
        var previewRoot = GameObject.Find("EditorPreviewsRoot");
        if (previewRoot)
            DestroyImmediate(previewRoot);
    }

    private void OnGUI()
    {
        _windowEditorScrollPos = EditorGUILayout.BeginScrollView(_windowEditorScrollPos);

        EditorGUILayout.LabelField("Character Prefab Library", EditorStyles.boldLabel);
        _characterPrefabLibrary = EditorGUILayout.ObjectField(
            "Character Prefab Library",
            _characterPrefabLibrary,
            typeof(CharacterPrefabLibrary),
            false
        ) as CharacterPrefabLibrary;

        EditorGUILayout.LabelField("Tile Prefab Library", EditorStyles.boldLabel);
        _tilePrefabLibrary = EditorGUILayout.ObjectField(
            "TilePrefab Library",
            _tilePrefabLibrary,
            typeof(TilePrefabLibrary),
            false
        ) as TilePrefabLibrary;

        _bFocusToggle           = EditorGUILayout.Toggle("Focus Toggle for Draw", _bFocusToggle);
        _bDrawPreviewGrid       = EditorGUILayout.Toggle("Draw Grid Lines", _bDrawPreviewGrid);
        _drawMode               = GUILayout.SelectionGrid(_drawMode, new[] { "Draw Tiles", "Draw Characters" }, 1);
        _battleGridWidth        = EditorGUILayout.IntSlider("BattleGrid Width", _battleGridWidth, 0, 30);
        _battleGridHeight       = EditorGUILayout.IntSlider("BattleGrid Height", _battleGridHeight, 0, 30);
        _defaultDeployZoneWidth = EditorGUILayout.IntSlider("BattleGrid Width", _defaultDeployZoneWidth, 0, 30);
        _defaultTile            = EditorGUILayout.ObjectField("Default Tile for Grid Generation", _defaultTile, typeof(GameObject), false) as GameObject;
        _defaultDeployTile      = EditorGUILayout.ObjectField("Default Deploy Tile for Deploy Zone Generation", _defaultDeployTile, typeof(GameObject), false) as GameObject;
        _tileSizeInMeters       = EditorGUILayout.Vector3Field("Size of a tile in meters", _tileSizeInMeters);
        _fileNameToSaveJSON     = EditorGUILayout.TextField("Save To: ", _fileNameToSaveJSON);
        _fileNameToLoadJSON     = EditorGUILayout.TextField("Load From: ", _fileNameToLoadJSON);


        if (_prevBattleGridHeight != _battleGridHeight || _prevBattleGridWidth != _battleGridWidth)
        {
            SetGridSize(_battleGridWidth, _battleGridHeight);
            _prevBattleGridWidth = _battleGridWidth;
            _prevBattleGridHeight = _battleGridHeight;

        }

        UpdatePrefabLists();
        UpdateTileGrid();
        UpdateCharacterList();

        EditorGUILayout.EndScrollView();
        
        HandleButtonLoadFromJSON();
        HandleButtonSaveToJSON();
        HandleButtonGenerateDefaultGrid();
        HandleButtonGenerateDefaultDeployZone();

    }

    private void HandleButtonLoadFromJSON()
    {
     
        if (GUILayout.Button("Load Grid from JSON", GUILayout.Height(50)))
        {
            if (_fileNameToLoadJSON == null || _fileNameToLoadJSON.Length == 0)
            {
                // Show OK / Cancel dialog
                EditorUtility.DisplayDialog(
                    "Confirm Load Battle Grid",
                    "Please enter a filename to load in the\n\"Load From:\" slot!\n\n" +
                    "Example: \n" +
                    "Load From: MyBattleGrid\n\n" +
                    "Note: DON'T enter file extension (.json)",
                    "OK"
                );
                return;
            }
            // Gather context info
            string message = $"You are about to load the current Battlegrid from file: " +
                             _fileNameToLoadJSON +
                             "\nDo you want to proceed?";

            // Show OK / Cancel dialog
            bool confirm = EditorUtility.DisplayDialog(
                "Confirm Load Battle Grid",
                message,
                "OK",
                "Cancel"
            );

            if (confirm)
            {
                LoadBattleGridFromJSON();
                DebugLog.CJLog($"Battle grid loaded!");
            }
            else
            {
                DebugLog.CJLog("Load cancelled.");
            }
        }
    }

    private void HandleButtonSaveToJSON()
    {
        if (GUILayout.Button("Save Grid to JSON", GUILayout.Height(50)))
        {
            // Gather context info
            string message = $"You are about to save the current battle grid:\n" +
                             $"Width: {_battleGridWidth}, Height: {_battleGridHeight}\n" +
                             $"Tiles in grid: {_tileGridProperty.arraySize}\n\n" +
                             $"At location: " + _fileNameToSaveJSON + ".json\n\n" +
                             "Do you want to proceed?";

            // Show OK / Cancel dialog
            bool confirm = EditorUtility.DisplayDialog(
                "Confirm Save Battle Grid",
                message,
                "OK",
                "Cancel"
            );

            if (confirm)
            {
                SaveBattleGridToJSON();
                DebugLog.CJLog($"Battle grid saved! {_tileGridProperty.arraySize} tiles exported.");
            }
            else
            {
                DebugLog.CJLog("Save cancelled.");
            }
        }
    }

    private void HandleButtonGenerateDefaultGrid()
    {
        if (GUILayout.Button("Generate Default Grid", GUILayout.Height(50)))
        {
            // Gather context info
            string message = $"No Default Prefab:\n";

            if (_defaultTile == null)
            {
                // Show Error Dialogue
                EditorUtility.DisplayDialog(
                    "Grid Generator Error",
                    message,
                    "OK"
                );
            }
            else
            {
                if (_battleGridWidth <= 0 || _battleGridHeight <= 0)
                {
                    message = "Battle Grid Width or Height must be creater than 0";
                    // Show Error Dialogue
                    EditorUtility.DisplayDialog(
                        "Grid Generator Error",
                        message,
                        "OK"
                    );
                }
                else
                {
                    var parent = GenerateParentRootObject(_strRootObjectForTiles);
                    // NOTE (Calle): Clear the grid before generating a new one.
                    _tileGridHolderSO.Update();
                    SerializedProperty tileEntries = _tileGridHolderSO.FindProperty("_tileEntries");
                    for (int i = 0; i < tileEntries.arraySize; i++)
                    {
                        // Get the tile entry property in the entry list property
                        SerializedProperty tileEntryProp = tileEntries.GetArrayElementAtIndex(i);
                        // Get the tile property in the tile entry property
                        SerializedProperty tileProp = tileEntryProp.FindPropertyRelative("_tile");
                        // Get the reference to actual tile GameObject
                        GameObject tileGO = tileProp.objectReferenceValue as GameObject;
                        if (tileGO != null)
                            DestroyImmediate(tileGO);
                    }
                    _tileGridHolderSO.ApplyModifiedProperties();

                    for (int y = 0; y < _battleGridHeight; y++)
                    {
                        for (int x = 0; x < _battleGridWidth; x++)
                        {
                            // World-space position of the tile's center
                            Vector3 pos = new Vector3(
                                x * _tileSizeInMeters.x + _tileSizeInMeters.x / 2f,
                                0f,
                                y * _tileSizeInMeters.z + _tileSizeInMeters.z / 2f
                            );

                            // Pass grid coordinates as Vector2
                            Vector2Int gridPos = new Vector2Int(x, y);

                            TileEntry newTileEntry = new TileEntry(pos, _tileSizeInMeters, _defaultTile, parent, gridPos);
                            Undo.RegisterCreatedObjectUndo(newTileEntry._tile, "Placed/Updated Tile");
                            // Add or replace in tile dictionary
                            AddOrReplaceTileEntry(x, y, newTileEntry);
                        }
                    }
                }
            }
        }
    }

    private void HandleButtonGenerateDefaultDeployZone()
    {

        if (GUILayout.Button("Generate Default Deploy Zone", GUILayout.Height(50)))
        {
            // Gather context info
            string message = $"No Default Prefab for DeployTile\n";

            if (_defaultTile == null)
            {
                // Show Error Dialogue
                EditorUtility.DisplayDialog(
                    "Deploy Zone Generator Error",
                    message,
                    "OK"
                );
            }
            else
            {
                if (_battleGridWidth <= 0 || _battleGridHeight <= 0)
                {
                    message = "Battle Grid Width or Height must be creater than 0";
                    // Show Error Dialogue
                    EditorUtility.DisplayDialog(
                        "Deploy Zone Generator Error",
                        message,
                        "OK"
                    );
                }
                else
                {
                    int deployZoneWidth = _defaultDeployZoneWidth;
                    if(_defaultDeployZoneWidth > _battleGridWidth)
                        deployZoneWidth = _defaultDeployZoneWidth - _battleGridWidth;

                    var parent = GenerateParentRootObject(_strRootObjectForTiles);
                    // NOTE (Calle): Clear Tiles in the Deploy Zone
                    _tileGridHolderSO.Update();
                    SerializedProperty tileEntries = _tileGridHolderSO.FindProperty("_tileEntries");
                    for(int y = 0; y < _battleGridHeight; y++)
                    {

                    }
                    for (int x = 0; x < deployZoneWidth; x++)
                    {
                        // Get the tile entry property in the entry list property
                        SerializedProperty tileEntryProp = tileEntries.GetArrayElementAtIndex(x);
                        // Get the tile property in the tile entry property
                        SerializedProperty tileProp = tileEntryProp.FindPropertyRelative("_tile");
                        // Get the reference to actual tile GameObject
                        GameObject tileGO = tileProp.objectReferenceValue as GameObject;
                        if (tileGO != null)
                            DestroyImmediate(tileGO);
                    }
                    //for (int i = 0; i < tileEntries.arraySize; i++)
                    //{
                    //    
                    //}
                    _tileGridHolderSO.ApplyModifiedProperties();

                    for (int y = 0; y < _battleGridHeight; y++)
                    {
                        for (int x = 0; x < deployZoneWidth; x++)
                        {
                            // World-space position of the tile's center
                            Vector3 pos = new Vector3(
                                x * _tileSizeInMeters.x + _tileSizeInMeters.x / 2f,
                                0f,
                                y * _tileSizeInMeters.z + _tileSizeInMeters.z / 2f
                            );

                            // Pass grid coordinates as Vector2
                            Vector2Int gridPos = new Vector2Int(x, y);

                            TileEntry newTileEntry = new TileEntry(pos, _tileSizeInMeters, _defaultDeployTile, parent, gridPos);
                            Undo.RegisterCreatedObjectUndo(newTileEntry._tile, "Placed/Updated Tile");
                            // Add or replace in tile dictionary
                            AddOrReplaceTileEntry(x, y, newTileEntry);
                        }
                    }
                }
            }
        }
    }

    private CharacterEntry InstantiateAndSetCharacterEntry(Vector3 goPos, Vector3 goSize, GameObject prefab, GameObject parent, Vector2Int gridPos)
    {
        if (prefab == null) return null;

        CharacterEntry newCharacterEntry= new CharacterEntry();
        
        // GameObject Specific
        newCharacterEntry._character = Instantiate(prefab);
        newCharacterEntry._character.transform.position = goPos;
        newCharacterEntry._character.transform.localScale = goSize;
        newCharacterEntry._character.transform.SetParent(parent.transform);
        newCharacterEntry._character.GetComponent<Character>().SetCurrentTileIndex(gridPos);

        // Save/Load Specific
        newCharacterEntry._characterClass = prefab.GetComponent<Character>().GetCharacterClass();
        newCharacterEntry._tileIndex = gridPos;
        newCharacterEntry._position = goPos;
        newCharacterEntry._size = goSize;

         Undo.RegisterCreatedObjectUndo(newCharacterEntry._character, "Placed/Updated Character");
        
        return newCharacterEntry;
    }



    private void OnSceneGUI(SceneView sceneView)
    {

        if (_bDrawPreviewGrid)
            DrawPreviewGrid();
        

        Event currentEvent = Event.current;

        UpdateFocusDrawMode(currentEvent);

        if (IsInFocusDrawMode())
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            HandleUtility.AddDefaultControl(controlID);
            _bFocusToggle = true;
            switch(_drawMode)
            {
                case DRAWMODE_TILE:
                    MovePreviewTile(currentEvent);
                    ScrollSelectTileBrush(currentEvent);
                    DrawTiles(currentEvent);
                    break;
                case DRAWMODE_CHARACTER:
                    MovePreviewCharacter(currentEvent);
                    ScrollSelectCharacterBrush(currentEvent);
                    DrawCharacters(currentEvent);
                    break;
            }
        }
        else
        {
            _bFocusToggle = false;
        }

        Repaint();
        SceneView.RepaintAll();
    }

    private void DrawPreviewGrid()
    {
        Handles.color = Color.yellow;

        Vector3 wireSize = new Vector3(_battleGridWidth*_tileSizeInMeters.x, 0.05f, _battleGridHeight*_tileSizeInMeters.z);
        Vector3 wirePos = new Vector3((_battleGridWidth * _tileSizeInMeters.x) / 2.0f , 0.0f, (_battleGridHeight * _tileSizeInMeters.z) / 2.0f);
        Handles.DrawWireCube(wirePos, wireSize);
        Handles.DrawWireDisc(Vector3.zero, Vector3.up, 0.3f, 2.0f);

        float lineRadius = 5.0f;
        Handles.color = Color.blue;
        Handles.DrawLine(Vector3.zero, Vector3.Scale(Vector3.forward, _tileSizeInMeters), lineRadius);
        Handles.color = Color.red;
        Handles.DrawLine(Vector3.zero, Vector3.Scale(Vector3.right, _tileSizeInMeters), lineRadius);
        Handles.color = Color.green;
        Handles.DrawLine(Vector3.zero, Vector3.Scale(Vector3.up, _tileSizeInMeters), lineRadius);


        for (int y = 0; y < _battleGridHeight; y++)
        {
            Vector3 p1 = new Vector3(0.0f, 0.0f, y * _tileSizeInMeters.z);
            Vector3 p2 = new Vector3(_battleGridWidth * _tileSizeInMeters.x, 0.0f, y * _tileSizeInMeters.z);
            Handles.DrawLine(p1, p2, 1.0f);
        }

        for (int x = 0; x < _battleGridWidth; x++)
        {
            // TODO (Calle) : 1. Place and save Character Friendly and Enemy 
            //                2. Load Characters from JSON into battlegrid

            Vector3 p1 = new Vector3(x * _tileSizeInMeters.x, 0.0f, 0.0f);
            Vector3 p2 = new Vector3(x * _tileSizeInMeters.x, 0.0f, _battleGridHeight * _tileSizeInMeters.z);
            Handles.DrawLine(p1, p2, 1.0f);
        }
    }
    private void UpdatePrefabLists()
    {
        UpdateTileBrushPrefabList();
        UpdateCharacterBrushPrefabList();
    }

    private void UpdateTileBrushPrefabList()
    {
        _tileBrushPrefabHolderSO.Update();

        EditorGUILayout.LabelField("Tile Prefabs to Spawn", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(_tileBrushPrefabProperty, includeChildren: true);
        EditorGUI.indentLevel--;

        _tileBrushPrefabHolderSO.ApplyModifiedProperties();

        EditorGUILayout.Space();
    }

    private void UpdateCharacterBrushPrefabList()
    {
        
        _characterBrushPrefabHolderSO.Update();

        EditorGUILayout.LabelField("Character Prefab Brushes", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(_characterBrushPrefabProperty, includeChildren: true);
        EditorGUI.indentLevel--;

        _characterBrushPrefabHolderSO.ApplyModifiedProperties();

        EditorGUILayout.Space();
    }

    private void UpdateTileGrid()
    {
        _tileGridHolderSO.Update();

        EditorGUILayout.LabelField("Tiles in Grid", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        EditorGUILayout.BeginVertical();
        EditorGUILayout.PropertyField(_tileGridProperty, includeChildren: true);

        EditorGUILayout.EndVertical();
        EditorGUI.indentLevel--;

        _tileGridHolderSO.ApplyModifiedProperties();
        EditorGUILayout.Space();
    }

    private void UpdateCharacterList()
    {
        _characterListSO.Update();
        EditorGUILayout.LabelField("Placed Characters", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        EditorGUILayout.BeginVertical();
        EditorGUILayout.PropertyField(_characterListProperty, includeChildren: true);
        EditorGUILayout.EndVertical();
        EditorGUI.indentLevel--;

        _characterListSO.ApplyModifiedProperties();
        EditorGUILayout.Space();
    }
    
    private void UpdatePreviewTile()
    {
        if (_tileBrushPrefabHolder == null || _tileBrushPrefabHolder._tileBrushPrefabs.Count == 0)
            return;

        GameObject prefab = _tileBrushPrefabHolder._tileBrushPrefabs[_currentTileBrushIndex];
        if (prefab == null)
            return;

        // Destroy previous preview
        if (_previewTile._tile != null)
            DestroyImmediate(_previewTile._tile);

        _previewTile._tile = Instantiate(prefab);
        _previewTile._tile.transform.localScale = _tileSizeInMeters;
        _previewTile._tile.name = "PreviewTile";
        _previewTile._tile.hideFlags = HideFlags.HideAndDontSave;

        // Make preview semi-transparent (safe for HDRP/URP/other shaders)
        foreach (var renderer in _previewTile._tile.GetComponentsInChildren<Renderer>())
        {
            foreach (var mat in renderer.sharedMaterials)
            {
                if (mat == null) continue;

                // Use material property block for transparency (safe)
                MaterialPropertyBlock mpb = new MaterialPropertyBlock();
                mpb.SetFloat("_SurfaceType", 1); // Transparent (HDRP)
                mpb.SetFloat("_BlendMode", 0);   // Alpha
                mpb.SetFloat("_Alpha", 0.5f);    // 50% opacity
                renderer.SetPropertyBlock(mpb);
            }
        }
    }

    private void UpdatePreviewCharacter()
    {
        if (_characterBrushPrefabHolder == null || _characterBrushPrefabHolder._characterBrushPrefabs.Count == 0)
            return;

        GameObject prefab = _characterBrushPrefabHolder._characterBrushPrefabs[_currentCharacterBrushIndex];
        if (prefab == null)
            return;

        // Destroy previous preview
        if (_previewCharacter._character != null)
            DestroyImmediate(_previewCharacter._character);

        _previewCharacter._character = Instantiate(prefab);
        _previewCharacter._character.transform.localScale = Vector3.one; // NOTE (Calle): Should maybe use prefabs scale?
        _previewCharacter._character.name = "PreviewCharacter";
        _previewCharacter._character.hideFlags = HideFlags.HideAndDontSave;

        // Make preview semi-transparent (safe for HDRP/URP/other shaders)
        foreach (var renderer in _previewCharacter._character.GetComponentsInChildren<Renderer>())
        {
            foreach (var mat in renderer.sharedMaterials)
            {
                if (mat == null) continue;

                // Use material property block for transparency (safe)
                MaterialPropertyBlock mpb = new MaterialPropertyBlock();
                mpb.SetFloat("_SurfaceType", 1); // Transparent (HDRP)
                mpb.SetFloat("_BlendMode", 0);   // Alpha
                mpb.SetFloat("_Alpha", 0.5f);    // 50% opacity
                renderer.SetPropertyBlock(mpb);
            }
        }
    }

    private void MovePreviewTile(Event currentEvent)
    {
        if (_previewTile._tile == null)
            return;
        
        Ray worldRay = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(worldRay, out float distance))
        {
            Vector3 hitPoint = worldRay.GetPoint(distance);

            // Snap in steps of the tile's own size
            float snappedX = Mathf.Floor(hitPoint.x / _tileSizeInMeters.x) * _tileSizeInMeters.x + _tileSizeInMeters.x / 2f;
            float snappedZ = Mathf.Floor(hitPoint.z / _tileSizeInMeters.z) * _tileSizeInMeters.z + _tileSizeInMeters.z / 2f;

            _previewTile._tile.transform.position = new Vector3(snappedX, 0f, snappedZ);
        }
    }


    private void MovePreviewCharacter(Event currentEvent)
    {
        if (_previewCharacter._character == null)
            return;

        Ray worldRay = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(worldRay, out float distance))
        {
            Vector3 hitPoint = worldRay.GetPoint(distance);

            // Snap in steps of the tile's own size
            float snappedX = Mathf.Floor(hitPoint.x / _tileSizeInMeters.x) * _tileSizeInMeters.x + _tileSizeInMeters.x / 2.0f;
            float snappedZ = Mathf.Floor(hitPoint.z / _tileSizeInMeters.z) * _tileSizeInMeters.z + _tileSizeInMeters.z / 2.0f;
            float snappedY = 0.0f;

            // TODO (Calle): Use this for X and Y aswell depending on the characters dimensions?
            Renderer renderer = _previewCharacter._character.GetComponent<Renderer>();
            if(renderer != null)
            {
                float objectHeight = renderer.bounds.size.y;
                //snappedY = objectHeight / 2.0f; // Used if the pivot of the character is in the centre.
            }
            
            _previewCharacter._character.transform.position = new Vector3(snappedX, snappedY, snappedZ);
        }
    }

    private void AddOrReplaceTileEntry(int gridX, int gridZ, TileEntry tile)
    {
        _tileGridHolderSO.Update();

        // Try to find existing tile at grid position
        for (int i = 0; i < _tileGridProperty.arraySize; i++)
        {
            SerializedProperty entryProp = _tileGridProperty.GetArrayElementAtIndex(i);
            Vector2 tileIndex = entryProp.FindPropertyRelative("_tileIndex").vector2IntValue;

            if ((int)tileIndex.x == gridX && (int)tileIndex.y == gridZ)
            {
                // Replace existing tile reference
                SerializedProperty oldTileProp = entryProp.FindPropertyRelative("_tile");
                GameObject oldTileGO = oldTileProp.objectReferenceValue as GameObject;

                if (oldTileGO != null)
                    Undo.DestroyObjectImmediate(oldTileGO);

                oldTileProp.objectReferenceValue = tile._tile;
                entryProp.FindPropertyRelative("_tileType").enumValueIndex = (int)tile._tileType;
                entryProp.FindPropertyRelative("_position").vector3Value = tile._position;
                entryProp.FindPropertyRelative("_size").vector3Value = tile._size;

                _tileGridHolderSO.ApplyModifiedProperties();
                return;
            }
        }

        // If not found, create a new entry
        int newIndex = _tileGridProperty.arraySize;
        _tileGridProperty.arraySize++;
        SerializedProperty newEntry = _tileGridProperty.GetArrayElementAtIndex(newIndex);

        newEntry.FindPropertyRelative("_size").vector3Value = tile._size;
        newEntry.FindPropertyRelative("_position").vector3Value = tile._position;
        newEntry.FindPropertyRelative("_tileIndex").vector2IntValue = new Vector2Int(gridX, gridZ);
        newEntry.FindPropertyRelative("_tile").objectReferenceValue = tile._tile;
        newEntry.FindPropertyRelative("_tileType").enumValueIndex = (int)tile._tileType;

        _tileGridHolderSO.ApplyModifiedProperties();
    }

    private void AddOrReplaceCharacterEntry(int gridX, int gridZ, CharacterEntry characterEntry)
    {
        _characterListSO.Update();

        // Try to find existing Character at grid position
        for (int i = 0; i < _characterListProperty.arraySize; i++)
        {
            SerializedProperty entryProp = _characterListProperty.GetArrayElementAtIndex(i);
            Vector2 tileIndex = entryProp.FindPropertyRelative("_tileIndex").vector2IntValue;

            if ((int)tileIndex.x == gridX && (int)tileIndex.y == gridZ)
            {
                // Replace existing Character reference
                SerializedProperty oldCharacterProp = entryProp.FindPropertyRelative("_character");
                GameObject oldCharacterGO = oldCharacterProp.objectReferenceValue as GameObject;

                if (oldCharacterGO != null)
                    Undo.DestroyObjectImmediate(oldCharacterGO);

                oldCharacterProp.objectReferenceValue = characterEntry._character;
                entryProp.FindPropertyRelative("_characterClass").enumValueIndex = (int)characterEntry._characterClass;
                entryProp.FindPropertyRelative("_position").vector3Value = characterEntry._position;
                entryProp.FindPropertyRelative("_size").vector3Value = characterEntry._size;

                _characterListSO.ApplyModifiedProperties();
                return;
            }
        }

        // If not found, create a new entry
        int newIndex = _characterListProperty.arraySize;
        _characterListProperty.arraySize++;
        SerializedProperty newEntry = _characterListProperty.GetArrayElementAtIndex(newIndex);

        newEntry.FindPropertyRelative("_size").vector3Value = characterEntry._size;
        newEntry.FindPropertyRelative("_position").vector3Value = characterEntry._position;
        newEntry.FindPropertyRelative("_tileIndex").vector2IntValue = new Vector2Int(gridX, gridZ);
        newEntry.FindPropertyRelative("_character").objectReferenceValue = characterEntry._character;
        newEntry.FindPropertyRelative("_characterClass").enumValueIndex = (int)characterEntry._characterClass;

        _characterListSO.ApplyModifiedProperties();
    }

    private void RemoveCharacterEntry(int gridX, int gridZ, CharacterEntry characterEntry)
    {
        _characterListSO.Update();
        for (int i = 0; i < _characterListProperty.arraySize; i++)
        {
            SerializedProperty entryProp = _characterListProperty.GetArrayElementAtIndex(i);
            Vector2 tileIndex = entryProp.FindPropertyRelative("_tileIndex").vector2IntValue;

            if ((int)tileIndex.x == gridX && (int)tileIndex.y == gridZ)
            {
                // Remove existing Character reference
                SerializedProperty oldCharacterProp = entryProp.FindPropertyRelative("_character");
                GameObject oldCharacterGO = oldCharacterProp.objectReferenceValue as GameObject;

                if (oldCharacterGO != null)
                    Undo.DestroyObjectImmediate(oldCharacterGO);

                entryProp.FindPropertyRelative("_characterClass").enumValueIndex = (int)CharacterClass.None; 
                entryProp.FindPropertyRelative("_position").vector3Value = Vector3.zero;
                entryProp.FindPropertyRelative("_size").vector3Value = Vector3.one;
                
                _characterListProperty.DeleteArrayElementAtIndex(i);
                _characterListSO.ApplyModifiedProperties();
                return;
            }
        }

    }
    private void SetGridSize(int width, int height)
    {
        SerializedProperty entriesProp = _tileGridHolderSO.FindProperty("_tileEntries");
        _tileGridHolderSO.Update();

        // Remove tiles outside new bounds
        List<int> removeIndices = new List<int>();
        for (int i = 0; i < entriesProp.arraySize; i++)
        {
            SerializedProperty entryProp = entriesProp.GetArrayElementAtIndex(i);
            SerializedProperty posProp = entryProp.FindPropertyRelative("_tileIndex");
            SerializedProperty tileProp = entryProp.FindPropertyRelative("_tile");

            Vector2 pos = posProp.vector2IntValue;

            if (pos.x >= width || pos.y >= height)
            {
                GameObject go = tileProp.objectReferenceValue as GameObject;
                if (go != null)
                    DestroyImmediate(go);

                removeIndices.Add(i);
            }
        }
        for (int i = removeIndices.Count - 1; i >= 0; i--)
        {
            entriesProp.DeleteArrayElementAtIndex(removeIndices[i]);
        }

        // Add empty tiles if expanding But don't instantiate visual GameObjects tiles
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bool exists = false;
                for (int i = 0; i < entriesProp.arraySize; i++)
                {
                    SerializedProperty entryProp = entriesProp.GetArrayElementAtIndex(i);
                    Vector2 pos = entryProp.FindPropertyRelative("_tileIndex").vector2IntValue;
                    if ((int)pos.x == x && (int)pos.y == y)
                    {
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                {
                    entriesProp.arraySize++;
                    SerializedProperty newEntry = entriesProp.GetArrayElementAtIndex(entriesProp.arraySize - 1);
                    newEntry.FindPropertyRelative("_tileIndex").vector2IntValue = new Vector2Int(x, y);
                    newEntry.FindPropertyRelative("_tile").objectReferenceValue = null;
                    newEntry.FindPropertyRelative("_tileType").enumValueIndex = (int)TileType.UnInitialized;
                }
            }
        }

        _tileGridHolderSO.ApplyModifiedProperties();
    }

    private void ScrollSelectTileBrush(Event currentEvent)
    {
        if (currentEvent.type == EventType.ScrollWheel)
        {
            if (_tileBrushPrefabHolder._tileBrushPrefabs.Count == 0) return;

            _currentTileBrushIndex -= (int)Mathf.Sign(currentEvent.delta.y); // scroll direction
            if (_currentTileBrushIndex < 0) _currentTileBrushIndex = _tileBrushPrefabHolder._tileBrushPrefabs.Count - 1;
            if (_currentTileBrushIndex >= _tileBrushPrefabHolder._tileBrushPrefabs.Count) _currentTileBrushIndex = 0;

            UpdatePreviewTile();
            currentEvent.Use(); // prevent scene camera scrolling
        }
    }

    private void ScrollSelectCharacterBrush(Event currentEvent)
    {
        if (currentEvent.type == EventType.ScrollWheel)
        {
            if (_characterBrushPrefabHolder._characterBrushPrefabs.Count == 0) return;

            _currentCharacterBrushIndex -= (int)Mathf.Sign(currentEvent.delta.y); // scroll direction
            if (_currentCharacterBrushIndex < 0) _currentCharacterBrushIndex = _characterBrushPrefabHolder._characterBrushPrefabs.Count - 1;
            if (_currentCharacterBrushIndex >= _characterBrushPrefabHolder._characterBrushPrefabs.Count) _currentCharacterBrushIndex = 0;

            UpdatePreviewCharacter();
            currentEvent.Use(); // prevent scene camera scrolling
        }
    }

    private bool IsInFocusDrawMode()
    {
        return _bInFocusMode;
    }
    private void UpdateFocusDrawMode(Event currentEvent)
    {

        if (currentEvent.type == EventType.KeyDown)
        {
            if (currentEvent.keyCode == KeyCode.F)
                _bIsHoldingF = true;
            if (currentEvent.keyCode == KeyCode.LeftControl)
                _bIsHoldingCtrl = true;

            if (_bIsHoldingCtrl && _bIsHoldingF)
            {
                if (!_bInFocusMode)
                    _bInFocusMode = true;
                else
                    _bInFocusMode = false;
            }
        }

        if (currentEvent.type == EventType.KeyUp)
        {
            if (currentEvent.keyCode == KeyCode.F)
                _bIsHoldingF = false;
            if (currentEvent.keyCode == KeyCode.LeftControl)
                _bIsHoldingCtrl = false;
        }
    }
    private void DrawTiles(Event currentEvent)
    {
        
        if ((currentEvent.type == EventType.MouseDrag || currentEvent.type == EventType.MouseDown) &&
            currentEvent.button == 0)
        {
            Ray worldRay = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

            PlaceTile(currentEvent, worldRay, groundPlane);
        }
    }

    private void DrawCharacters(Event currentEvent)
    {

        if ((currentEvent.type == EventType.MouseDrag || currentEvent.type == EventType.MouseDown) &&
            currentEvent.button == 0)
        {
            Ray worldRay = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

            PlaceCharacter(currentEvent, worldRay, groundPlane);
        }
    }

    private GameObject GenerateParentRootObject(string rootName)
    {
        var parentObject = GameObject.Find(rootName);

        if (parentObject == null)
        {
            parentObject = new GameObject(rootName);
            parentObject.transform.position = Vector3.zero;
            parentObject.transform.localScale = Vector3.one;
            parentObject.transform.rotation = Quaternion.identity;

            Undo.RegisterCreatedObjectUndo(parentObject, "Created Parent root object!");
        }

        return parentObject;
    }

    private bool IndexWithiGrid(int x, int y)
    {
        if ((x >= 0 && x < _battleGridWidth) &&
            (y >= 0 && y < _battleGridHeight))
            return true;

        return false;
    }

    private void PlaceTile(Event currentEvent, Ray worldRay, Plane groundPlane)
    {
        var parent = GenerateParentRootObject(_strRootObjectForTiles);

        if (!groundPlane.Raycast(worldRay, out float distance))
            return;

        Vector3 hitPoint = worldRay.GetPoint(distance);

        // Snap to grid based on tile size
        int gridX = Mathf.FloorToInt(hitPoint.x / _tileSizeInMeters.x);
        int gridZ = Mathf.FloorToInt(hitPoint.z / _tileSizeInMeters.z);

        if (!IndexWithiGrid(gridX, gridZ))
            return;

        Vector2Int gridPos = new Vector2Int(gridX, gridZ);

       TileEntry existingTileEntry = GetTileEntryAtPosition(new Vector3Int(gridX, 0, gridZ));
       GameObject newTilePrefab = _tileBrushPrefabHolder._tileBrushPrefabs[_currentTileBrushIndex];

        // If tile already exists and is same type, skip
       if (existingTileEntry._tile != null && PrefabUtility.GetCorrespondingObjectFromSource(existingTileEntry._tile) == newTilePrefab)
        {
            return;
        }

        // Destroy old tile if exists
        if (existingTileEntry._tile != null)
        {
            Undo.DestroyObjectImmediate(existingTileEntry._tile);
        }

        // Compute world-space position
        Vector3 worldPos = new Vector3(
            gridX * _tileSizeInMeters.x + _tileSizeInMeters.x / 2f,
            0f,
            gridZ * _tileSizeInMeters.z + _tileSizeInMeters.z / 2f
        );


        TileEntry newTileEntry = new TileEntry(worldPos, _tileSizeInMeters, newTilePrefab, parent, gridPos);
        Undo.RegisterCreatedObjectUndo(newTileEntry._tile, "Created/Placed Tile");

        // Add or replace in dictionary
        AddOrReplaceTileEntry(gridX, gridZ, newTileEntry);
    }

    private void PlaceCharacter(Event currentEvent, Ray worldRay, Plane groundPlane)
    {
        var parent = GenerateParentRootObject(_strRootObjectForCharacters);

        if (!groundPlane.Raycast(worldRay, out float distance))
            return;

        Vector3 hitPoint = worldRay.GetPoint(distance);

        // Snap to grid based on tile size
        int gridX = Mathf.FloorToInt(hitPoint.x / _tileSizeInMeters.x);
        int gridZ = Mathf.FloorToInt(hitPoint.z / _tileSizeInMeters.z);

        if (!IndexWithiGrid(gridX, gridZ))
            return;

        Vector2Int gridPos = new Vector2Int(gridX, gridZ);

        TileEntry existingTileEntry = GetTileEntryAtPosition(new Vector3Int(gridX, 0, gridZ));
        GameObject newCharacterPrefab = _characterBrushPrefabHolder._characterBrushPrefabs[_currentCharacterBrushIndex];

        // If tile already exists and is same type, skip
        if (existingTileEntry == null || existingTileEntry._tileType == TileType.UnInitialized)
        {
            bool confirm = EditorUtility.DisplayDialog(
                    "Place Character Error",
                    "You must place a character on a initialized tile!",
                    "OK"
                );
            return;
        }

        //if(existingTileEntry._occupant == null)
        //    existingTileEntry._occupant = newCharacterPrefab;
        //else
        //{
        //    Undo.DestroyObjectImmediate(existingTileEntry._occupant);
        //}

        // Compute world-space position
        Renderer ren = newCharacterPrefab.GetComponent<Renderer>();
        float snappedY = 0.0f;
        if (ren != null)
        {

            //snappedY = ren.bounds.size.y / 2.0f; // Used if the pivot is in the middle of an object
        }

        Vector3 worldPos = new Vector3(
            gridX * _tileSizeInMeters.x + _tileSizeInMeters.x / 2f,
            snappedY,
            gridZ * _tileSizeInMeters.z + _tileSizeInMeters.z / 2f
        );

        // Create a CharacterEntry and 
        //CharacterEntry characterEntry = InstantiateAndSetCharacterEntry(worldPos, Vector3.one, newCharacterPrefab, parent, gridPos);
        CharacterEntry newCharacterEntry = new CharacterEntry(worldPos, Vector3.one, Quaternion.identity, newCharacterPrefab, parent, gridPos);
        Undo.RegisterCreatedObjectUndo(newCharacterEntry._character, "Placed/Created Character");

        if(newCharacterEntry._character.CompareTag("DeleteCharacterBrush"))
        {
            RemoveCharacterEntry(gridX, gridZ,  newCharacterEntry);
            DestroyImmediate(newCharacterEntry._character);
        }
        else
        {
            AddOrReplaceCharacterEntry(gridX, gridZ, newCharacterEntry);
        }
            
    }


    private bool MouseRayHitGroundPlane(Event currentEvent, ref float outDistance)
    {
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        Ray worldRay = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);
        if (groundPlane.Raycast(worldRay, out float distance))
        {
            outDistance = distance;
            return true;
        }

        return false;
    }

    private GameObject GetTileAtPosition(Vector3Int position)
    {

        // TODO (Calle): Should make this a generic function that returns the ObjectType passed as a paramter at the specified position
        //SerializedProperty property = null;
        //if (objecType == tile)
        //        entryProperty = characterListProperty
        for (int i = 0; i < _tileGridProperty.arraySize; i++)
        {
            SerializedProperty entryProp = _tileGridProperty.GetArrayElementAtIndex(i);
            Vector2 pos = entryProp.FindPropertyRelative("_tileIndex").vector2IntValue;
            GameObject tile = entryProp.FindPropertyRelative("_tile").objectReferenceValue as GameObject;

            if ((int)pos.x == position.x && (int)pos.y == position.z && tile != null)
            {
                return tile; // return only valid tiles
            }
        }

        return null;
    }

    private TileEntry GetTileEntryAtPosition(Vector3Int position)
    {
        foreach(TileEntry tileEntry in _tileGridHolder._tileEntries)
        {
            if (tileEntry == null)
                continue;


            if (position.x == tileEntry._tileIndex.x && position.z == tileEntry._tileIndex.y)
                return tileEntry;
        }

        return null;
    }
 
    private void LoadBattleGridFromJSON()
    {
        string fileContent = Application.dataPath + "\\JSON BattleGrids\\" + _fileNameToLoadJSON + ".json";

        if(!System.IO.File.Exists(fileContent))
        {
            // Show Error Dialogue
            EditorUtility.DisplayDialog(
                "Loading Grid Error",
                "The file didn't exist!",
                "OK"
            );

            return;
        }

        string jsonFileData = System.IO.File.ReadAllText(fileContent);  

        if(jsonFileData.Length == 0)
        {
            // Show Error Dialogue
            EditorUtility.DisplayDialog(
                "Loading Grid Warning",
                "The file was empty!",
                "OK"
            );
            return;
        }

        // NOTE (Calle): LOADING TILES
        // NOTE (Calle): Must clear all scene objects and the List with TileEntries before loading the new Grid.
        _tileGridHolder._tileEntries.Clear();
        GameObject parentObject = GameObject.Find(_strRootObjectForTiles);
        if (parentObject != null)
            DestroyImmediate(parentObject);

        CombatGridSerializedSaveData combatGridSaveData = JsonUtility.FromJson<CombatGridSerializedSaveData>(jsonFileData);

        _battleGridWidth = combatGridSaveData._gridWidth;
        _battleGridHeight = combatGridSaveData._gridHeight;
        _tileSizeInMeters = combatGridSaveData._tileSize;

        var parent = GenerateParentRootObject(_strRootObjectForTiles);
        foreach(CombatGridTileData tileData in combatGridSaveData._tileData)
        {
            GameObject tilePrefab = _tilePrefabLibrary.GetPrefab(tileData.GetTileType());
            // TODO (Calle): Must find a way to fetch the correct prefab based on the tileDatas TileType.
            _tileGridHolder._tileEntries.Add(new TileEntry(tileData.GetTilePosition(),
                                                           tileData.GetTileSize(),
                                                           tilePrefab,
                                                           parent,
                                                           tileData.GetTileIndex()
                                                           ));
        }
        _tileGridHolderSO.Update();

        // NOTE (Calle): LOADING CHARACTERS
        _characterList._characterList.Clear();
        parentObject = GameObject.Find(_strRootObjectForCharacters);
        if(parentObject != null) 
            DestroyImmediate(parentObject);

        parent = GenerateParentRootObject(_strRootObjectForCharacters);
        foreach(CombatGridCharacterData characterData in combatGridSaveData._characterData)
        {
            GameObject characterPrefab = _characterPrefabLibrary.GetPrefab(characterData.GetCharacterClass());
            DebugLog.CJLog($"Loading character: {characterPrefab.name}");
            CharacterEntry characterEntry = new CharacterEntry(characterData.GetCharacterPosition(),
                                                               Vector3.one, // TODO (Calle): The Size is saved based on the renderer.bounds.size i think, so saving and loading multiple time will make characters bigger each time HAHA! XD
                                                               characterData.GetRotation(),
                                                               characterPrefab,
                                                               parent,
                                                               characterData.GetTileIndex());
            _characterList._characterList.Add(characterEntry);
        }
        _characterListSO.Update();

    }

    private void SaveBattleGridToJSON()
    {
        CombatGridSerializedSaveData combatGridSaveData = new CombatGridSerializedSaveData();
        
        combatGridSaveData._gridWidth = _battleGridWidth;
        combatGridSaveData._gridHeight = _battleGridHeight;
        combatGridSaveData._tileSize = _tileSizeInMeters;
        foreach (var entry in _tileGridHolder._tileEntries)
        {
            if(entry == null) continue;

            combatGridSaveData._tileData.Add(
                                 new CombatGridTileData(entry._tileType, 
                                                        entry._tileIndex,
                                                        entry._position, 
                                                        entry._size));
        }

        GameObject[] charactersInScene = GameObject.FindGameObjectsWithTag("Character");

        // TODO (Calle): Beh�vs Size h�r och vilken size ska returneras, g�ller tiles ocks�, render.bounds.size eller transform.localScale?
        foreach(GameObject character in  charactersInScene)
        {
            if (character.name.Equals("PreviewCharacter"))
                continue;

            combatGridSaveData._characterData.Add(
                              new CombatGridCharacterData(character.GetComponent<Character>().GetCharacterClass(),
                                                          character.GetComponent<Character>().GetFaction(),
                                                          character.GetComponent<Character>().GetHealthPoints(),
                                                          character.GetComponent<Character>().GetSpeed(),
                                                          character.GetComponent<Character>().GetCurrentTileIndex(),
                                                          character.transform.position,
                                                          character.GetComponent<Renderer>().bounds.size,
                                                          character.transform.rotation));
        }
        /*
        foreach(var characterEntry in characterList._characterList)
        {
            if (characterEntry == null)
                continue;

            combatGridSaveData._characterData.Add(
                                new CombatGridCharacterData(characterEntry._characterClass, 
                                                            characterEntry._tileIndex, 
                                                            characterEntry._position, 
                                                            characterEntry._size));        

        }
        */
        string strOutput = JsonUtility.ToJson(combatGridSaveData, true);   

        File.WriteAllText(Application.dataPath + "\\JSON BattleGrids\\" + _fileNameToSaveJSON + ".json", strOutput);


    }

    
}


using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;



public class GridMaker3D : EditorWindow
{
    GridMaker3D instance;

    LayerMask tileLayerMask;
    string fileNameJSON;
    const int DRAWMODE_TILE = 0, DRAWMODE_CHARACTER = 1;
    int _drawMode = 0;
    int battleGridWidth = 0;
    int battleGridHeight= 0;
    int prevBattleGridWidth = 0;
    int prevBattleGridHeight = 0;
    Vector2 tileGridScrollPos;
    Vector2 windowEditorScrollPos;
    Vector3 tileSizeInMeters;


    [System.Serializable]
    public class CharacterEntry
    {
        public Vector3 _position;
        public CharacterClass _characterClass;
        public GameObject _character;
    };

    [System.Serializable]
    public class CharacterList
    {
        public List<CharacterEntry> _characterList = new List<CharacterEntry>();
    };

    CharacterList       characterList;
    SerializedObject    characterListSO;
    SerializedProperty  charcterListProperty;

    [System.Serializable]
    public class CharacterBrushPrefabHolder : ScriptableObject
    {
        public GameObject[] _characterBrushPrefabs;
    };

    CharacterBrushPrefabHolder characterBrushPrefabHolder;
    SerializedObject           characterBrushPrefabHolderSO;
    SerializedProperty         characterBrushPrefabProperty;

    [System.Serializable]
    public class TileEntry
    {
        public Vector2 _tileIndex;
        public Vector3 _position;
        public Vector3 _size;
        public TileType _tileType;
        public GameObject _tile;
    };

    [System.Serializable]
    public class TileGrid : ScriptableObject
    {
        public List<TileEntry> _tileEntries = new List<TileEntry>();
    };

    TileGrid            tileGridHolder;
    SerializedObject    tileGridHolderSO;
    SerializedProperty  tileGridProperty;

    [System.Serializable]
    public class TileBrushPrefabHolder : ScriptableObject
    {
        public GameObject[] _tileBrushPrefabs;
    }

    TileBrushPrefabHolder tileBrushPrefabHolder;
    SerializedObject      tileBrushPrefabHolderSO;
    SerializedProperty    tileBrushPrefabProperty;

    [SerializeField] GameObject defaultTile;
    
    TileEntry previewTile;
    CharacterEntry previewCharacter;
    
    int currentTileBrushIndex = 0;
    int currentCharacterBrushIndex = 0;

    private bool isHoldingF  = false, isHoldingCtrl = false, inFocusMode = false;
    private bool focusToggle = false;
    
    [MenuItem("Custom Tools/GridMaker 3D")]
    public static void ShowWindow()
    {
        GetWindow<GridMaker3D>("GridMaker 3D");
    }

    private void OnEnable()
    {
        previewTile = new TileEntry();
        tileLayerMask = LayerMask.GetMask("EditorTile");

        instance = this;
        SceneView.duringSceneGui += OnSceneGUI;

        // create a temporary in-memory object
        tileBrushPrefabHolder = ScriptableObject.CreateInstance<TileBrushPrefabHolder>();
        tileBrushPrefabHolderSO = new SerializedObject(tileBrushPrefabHolder);
        tileBrushPrefabProperty = tileBrushPrefabHolderSO.FindProperty("_tileBrushPrefabs");

        characterBrushPrefabHolder = ScriptableObject.CreateInstance<CharacterBrushPrefabHolder>();
        characterBrushPrefabHolderSO = new SerializedObject(characterBrushPrefabHolder);
        characterBrushPrefabProperty = characterBrushPrefabHolderSO.FindProperty("_characterBrushPrefabs");

        tileGridHolder = ScriptableObject.CreateInstance<TileGrid>();
        tileGridHolderSO = new SerializedObject(tileGridHolder);
        tileGridProperty = tileGridHolderSO.FindProperty("_tileEntries");
    }

    private void OnDisable()
    {
        if (previewTile != null && previewTile._tile != null)
            DestroyImmediate(previewTile._tile);

        GameObject parent = GameObject.Find("-BATTLE GRID-");
        if (parent != null)
            DestroyImmediate(parent);

        SceneView.duringSceneGui -= OnSceneGUI;
    }

    
    private void OnGUI()
    {
        windowEditorScrollPos = EditorGUILayout.BeginScrollView(windowEditorScrollPos);

        focusToggle         = EditorGUILayout.Toggle("Focus Toggle for Draw", focusToggle);
        _drawMode           = GUILayout.SelectionGrid(_drawMode, new[] { "Draw Tiles", "Draw Characters" }, 1);
        battleGridWidth     = EditorGUILayout.IntSlider("BattleGrid Width", battleGridWidth, 0, 30);
        battleGridHeight    = EditorGUILayout.IntSlider("BattleGrid Height", battleGridHeight, 0, 30);
        defaultTile         = EditorGUILayout.ObjectField("Default Tile for Grid Generation", defaultTile, typeof(GameObject), false) as GameObject;
        tileSizeInMeters    = EditorGUILayout.Vector3Field("Size of a tile in meters", tileSizeInMeters);
        fileNameJSON        = EditorGUILayout.TextField("FileName: ", fileNameJSON);
        

        if (prevBattleGridHeight != battleGridHeight || prevBattleGridWidth != battleGridWidth)
        {
            SetGridSize(battleGridWidth, battleGridHeight);
            prevBattleGridWidth = battleGridWidth;
            prevBattleGridHeight = battleGridHeight;

        }

        UpdatePrefabLists();
        UpdateTileGrid();


        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Save Grid to JSON", GUILayout.Height(50)))
        {
            // Gather context info
            string message = $"You are about to save the current battle grid:\n" +
                             $"Width: {battleGridWidth}, Height: {battleGridHeight}\n" +
                             $"Tiles in grid: {tileGridProperty.arraySize}\n\n" +
                             $"At location: " + fileNameJSON + ".json\n\n" +
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
                Debug.Log($"Battle grid saved! {tileGridProperty.arraySize} tiles exported.");
            }
            else
            {
                Debug.Log("Save cancelled.");
            }
        }

        if (GUILayout.Button("Generate Default Grid", GUILayout.Height(50)))
        {
            // Gather context info
            string message = $"No Default Prefab:\n";


            if (defaultTile == null)
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
                if(battleGridWidth <= 0 || battleGridHeight <= 0)
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
                    var parent = GenerateTilemapParentRootObject("-BATTLE GRID-");
                    // NOTE (Calle): Clear the grid before generating a new one.
                    tileGridHolderSO.Update();
                    SerializedProperty tileEntries = tileGridHolderSO.FindProperty("_tileEntries");
                    for(int i = 0; i < tileEntries.arraySize; i++)
                    {
                        // Get the tile entry property in the entry list property
                        SerializedProperty tileEntryProp = tileEntries.GetArrayElementAtIndex(i);
                        // Get the tile property in the tile entry property
                        SerializedProperty tileProp = tileEntryProp.FindPropertyRelative("_tile");
                        // Get the reference to actual tile GameObject
                        GameObject tileGO = tileProp.objectReferenceValue as GameObject;
                        if(tileGO != null)
                            DestroyImmediate( tileGO );
                    }
                    tileGridHolderSO.ApplyModifiedProperties();

                    for (int y = 0; y < battleGridHeight; y++)
                    {
                        for (int x = 0; x < battleGridWidth; x++)
                        {
                            // World-space position of the tile's center
                            Vector3 pos = new Vector3(
                                x * tileSizeInMeters.x + tileSizeInMeters.x / 2f,
                                0f,
                                y * tileSizeInMeters.z + tileSizeInMeters.z / 2f
                            );

                            // Pass grid coordinates as Vector2
                            Vector2 gridPos = new Vector2(x, y);

                            // Instantiate tile entry
                            TileEntry newTile = InstantiateAndSetTileEntry(pos, tileSizeInMeters, defaultTile, parent, gridPos);

                            // Add or replace in tile dictionary
                            AddOrReplaceTile(x, y, newTile);
                        }
                    }
                }
            }   
        }


    }

    private TileEntry InstantiateAndSetTileEntry(Vector3 goPos, Vector3 goSize, GameObject prefab, GameObject parent, Vector2 gridPos)
    {
        if (prefab == null) return null;

        TileEntry newTile = new TileEntry();
        newTile._tileType = prefab.GetComponent<CombatGridTile>().GetTileType();
        newTile._tile = Instantiate(prefab);
        newTile._tile.transform.position = goPos;
        newTile._tile.transform.localScale = goSize;
        newTile._tile.transform.SetParent(parent.transform);
        newTile._tileIndex = gridPos;
        newTile._position = goPos;
        newTile._size = goSize;

        Undo.RegisterCreatedObjectUndo(newTile._tile, "Placed/Updated Tile");

        return newTile;
    }



    private void OnSceneGUI(SceneView sceneView)
    {

        DrawPreviewGrid();
        Event currentEvent = Event.current;

        UpdateFocusDrawMode(currentEvent);

        if (IsInFocusDrawMode())
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            HandleUtility.AddDefaultControl(controlID);
            focusToggle = true;
            if(_drawMode == DRAWMODE_TILE)
            {
                MovePreviewTile(currentEvent);
                ScrollSelectTileBrush(currentEvent);
                DrawTiles(currentEvent);
            }
            else if(_drawMode == DRAWMODE_CHARACTER)
            {
                MovePreviewCharacter(currentEvent);
                ScrollSelectCharacterBrush(currentEvent);
            }
        }
        else
        {
            focusToggle = false;
        }
        Repaint();
        SceneView.RepaintAll();
        

    }

    private void DrawPreviewGrid()
    {
        Handles.color = Color.yellow;

        Vector3 wireSize = new Vector3(battleGridWidth*tileSizeInMeters.x, 0.05f, battleGridHeight*tileSizeInMeters.z);
        Vector3 wirePos = new Vector3((battleGridWidth * tileSizeInMeters.x) / 2.0f , 0.0f, (battleGridHeight * tileSizeInMeters.z) / 2.0f);
        Handles.DrawWireCube(wirePos, wireSize);
        Handles.DrawWireDisc(Vector3.zero, Vector3.up, 0.3f, 2.0f);

        float lineRadius = 5.0f;
        Handles.color = Color.blue;
        Handles.DrawLine(Vector3.zero, Vector3.Scale(Vector3.forward, tileSizeInMeters), lineRadius);
        Handles.color = Color.red;
        Handles.DrawLine(Vector3.zero, Vector3.Scale(Vector3.right, tileSizeInMeters), lineRadius);
        Handles.color = Color.green;
        Handles.DrawLine(Vector3.zero, Vector3.Scale(Vector3.up, tileSizeInMeters), lineRadius);


        for (int y = 0; y < battleGridHeight; y++)
        {
            Vector3 p1 = new Vector3(0.0f, 0.0f, y * tileSizeInMeters.z);
            Vector3 p2 = new Vector3(battleGridWidth * tileSizeInMeters.x, 0.0f, y * tileSizeInMeters.z);
            Handles.DrawLine(p1, p2, 1.0f);
        }

        for (int x = 0; x < battleGridWidth; x++)
        {
            // TODO (Calle) : 1. Place and save Character Friendly and Enemy 
            //                2. Load Characters from JSON into battlegrid

            Vector3 p1 = new Vector3(x * tileSizeInMeters.x, 0.0f, 0.0f);
            Vector3 p2 = new Vector3(x * tileSizeInMeters.x, 0.0f, battleGridHeight * tileSizeInMeters.z);
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
        tileBrushPrefabHolderSO.Update();

        EditorGUILayout.LabelField("Tile Prefabs to Spawn", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(tileBrushPrefabProperty, includeChildren: true);
        EditorGUI.indentLevel--;

        tileBrushPrefabHolderSO.ApplyModifiedProperties();

        EditorGUILayout.Space();
    }

    private void UpdateCharacterBrushPrefabList()
    {
        
        characterBrushPrefabHolderSO.Update();

        EditorGUILayout.LabelField("Character Prefab Brushes", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(characterBrushPrefabProperty, includeChildren: true);
        EditorGUI.indentLevel--;

        characterBrushPrefabHolderSO.ApplyModifiedProperties();

        EditorGUILayout.Space();
    }

    private void UpdateTileGrid()
    {
        tileGridHolderSO.Update();

        EditorGUILayout.LabelField("Tiles in Grid", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        // Begin scroll view
        tileGridScrollPos = EditorGUILayout.BeginScrollView(tileGridScrollPos, GUILayout.Height(300)); // Set desired height

        EditorGUILayout.PropertyField(tileGridProperty, includeChildren: true);

        EditorGUILayout.EndScrollView();
        EditorGUI.indentLevel--;

        tileGridHolderSO.ApplyModifiedProperties();
        EditorGUILayout.Space();
    }
    
    private void UpdatePreviewTile()
    {
        if (tileBrushPrefabHolder == null || tileBrushPrefabHolder._tileBrushPrefabs.Length == 0)
            return;

        GameObject prefab = tileBrushPrefabHolder._tileBrushPrefabs[currentTileBrushIndex];
        if (prefab == null)
            return;

        // Destroy previous preview
        if (previewTile._tile != null)
            DestroyImmediate(previewTile._tile);

        previewTile._tile = Instantiate(prefab);
        previewTile._tile.transform.localScale = tileSizeInMeters;
        previewTile._tile.name = "PreviewTile";
        previewTile._tile.hideFlags = HideFlags.HideAndDontSave;

        // Make preview semi-transparent (safe for HDRP/URP/other shaders)
        foreach (var renderer in previewTile._tile.GetComponentsInChildren<Renderer>())
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
        if (characterBrushPrefabHolder == null || characterBrushPrefabHolder._characterBrushPrefabs.Length == 0)
            return;

        GameObject prefab = characterBrushPrefabHolder._characterBrushPrefabs[currentCharacterBrushIndex];
        if (prefab == null)
            return;

        // Destroy previous preview
        if (previewCharacter._character != null)
            DestroyImmediate(previewCharacter._character);

        previewCharacter._character = Instantiate(prefab);
        previewCharacter._character.transform.localScale = Vector3.one; // NOTE (Calle): Should maybe use prefabs scale?
        previewCharacter._character.name = "PreviewCharacter";
        previewCharacter._character.hideFlags = HideFlags.HideAndDontSave;

        // Make preview semi-transparent (safe for HDRP/URP/other shaders)
        foreach (var renderer in previewCharacter._character.GetComponentsInChildren<Renderer>())
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
        if (previewTile._tile == null)
            return;
        
        Ray worldRay = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(worldRay, out float distance))
        {
            Vector3 hitPoint = worldRay.GetPoint(distance);

            // Snap in steps of the tile's own size
            float snappedX = Mathf.Floor(hitPoint.x / tileSizeInMeters.x) * tileSizeInMeters.x + tileSizeInMeters.x / 2f;
            float snappedZ = Mathf.Floor(hitPoint.z / tileSizeInMeters.z) * tileSizeInMeters.z + tileSizeInMeters.z / 2f;

            previewTile._tile.transform.position = new Vector3(snappedX, 0f, snappedZ);
        }
    }


    private void MovePreviewCharacter(Event currentEvent)
    {
        if (previewCharacter._character == null)
            return;

        Ray worldRay = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(worldRay, out float distance))
        {
            Vector3 hitPoint = worldRay.GetPoint(distance);

            // Snap in steps of the tile's own size
            float snappedX = Mathf.Floor(hitPoint.x / tileSizeInMeters.x) * tileSizeInMeters.x + tileSizeInMeters.x / 2f;
            float snappedZ = Mathf.Floor(hitPoint.z / tileSizeInMeters.z) * tileSizeInMeters.z + tileSizeInMeters.z / 2f;

            previewCharacter._character.transform.position = new Vector3(snappedX, 0f, snappedZ);
        }
    }

    private void AddOrReplaceTile(int gridX, int gridZ, TileEntry tile)
    {
        tileGridHolderSO.Update();

        // Try to find existing tile at grid position
        for (int i = 0; i < tileGridProperty.arraySize; i++)
        {
            SerializedProperty entryProp = tileGridProperty.GetArrayElementAtIndex(i);
            Vector2 tileIndex = entryProp.FindPropertyRelative("_tileIndex").vector2Value;

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

                tileGridHolderSO.ApplyModifiedProperties();
                return;
            }
        }

        // If not found, create a new entry
        int newIndex = tileGridProperty.arraySize;
        tileGridProperty.arraySize++;
        SerializedProperty newEntry = tileGridProperty.GetArrayElementAtIndex(newIndex);

        newEntry.FindPropertyRelative("_size").vector3Value = tile._size;
        newEntry.FindPropertyRelative("_position").vector3Value = tile._position;
        newEntry.FindPropertyRelative("_tileIndex").vector2Value = new Vector2(gridX, gridZ);
        newEntry.FindPropertyRelative("_tile").objectReferenceValue = tile._tile;
        newEntry.FindPropertyRelative("_tileType").enumValueIndex = (int)tile._tileType;

        tileGridHolderSO.ApplyModifiedProperties();
    }

    private void SetGridSize(int width, int height)
    {
        SerializedProperty entriesProp = tileGridHolderSO.FindProperty("_tileEntries");
        tileGridHolderSO.Update();

        // Remove tiles outside new bounds
        List<int> removeIndices = new List<int>();
        for (int i = 0; i < entriesProp.arraySize; i++)
        {
            SerializedProperty entryProp = entriesProp.GetArrayElementAtIndex(i);
            SerializedProperty posProp = entryProp.FindPropertyRelative("_tileIndex");
            SerializedProperty tileProp = entryProp.FindPropertyRelative("_tile");

            Vector2 pos = posProp.vector2Value;

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

        // Add empty tiles if expanding
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bool exists = false;
                for (int i = 0; i < entriesProp.arraySize; i++)
                {
                    SerializedProperty entryProp = entriesProp.GetArrayElementAtIndex(i);
                    Vector2 pos = entryProp.FindPropertyRelative("_tileIndex").vector2Value;
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
                    newEntry.FindPropertyRelative("_tileIndex").vector2Value = new Vector2(x, y);
                    newEntry.FindPropertyRelative("_tile").objectReferenceValue = null;
                    newEntry.FindPropertyRelative("_tileType").enumValueIndex = (int)TileType.Walkable;
                }
            }
        }

        tileGridHolderSO.ApplyModifiedProperties();
    }

    private void ScrollSelectTileBrush(Event currentEvent)
    {
        if (currentEvent.type == EventType.ScrollWheel)
        {
            if (tileBrushPrefabHolder._tileBrushPrefabs.Length == 0) return;

            currentTileBrushIndex -= (int)Mathf.Sign(currentEvent.delta.y); // scroll direction
            if (currentTileBrushIndex < 0) currentTileBrushIndex = tileBrushPrefabHolder._tileBrushPrefabs.Length - 1;
            if (currentTileBrushIndex >= tileBrushPrefabHolder._tileBrushPrefabs.Length) currentTileBrushIndex = 0;

            UpdatePreviewTile();
            currentEvent.Use(); // prevent scene camera scrolling
        }
    }

    private void ScrollSelectCharacterBrush(Event currentEvent)
    {
        if (currentEvent.type == EventType.ScrollWheel)
        {
            if (characterBrushPrefabHolder._characterBrushPrefabs.Length == 0) return;

            currentCharacterBrushIndex -= (int)Mathf.Sign(currentEvent.delta.y); // scroll direction
            if (currentCharacterBrushIndex < 0) currentCharacterBrushIndex = characterBrushPrefabHolder._characterBrushPrefabs.Length - 1;
            if (currentCharacterBrushIndex >= characterBrushPrefabHolder._characterBrushPrefabs.Length) currentCharacterBrushIndex = 0;

            UpdatePreviewCharacter();
            currentEvent.Use(); // prevent scene camera scrolling
        }
    }

    private bool IsInFocusDrawMode()
    {
        return inFocusMode;
    }
    private void UpdateFocusDrawMode(Event currentEvent)
    {

        if (currentEvent.type == EventType.KeyDown)
        {
            if (currentEvent.keyCode == KeyCode.F)
                isHoldingF = true;
            if (currentEvent.keyCode == KeyCode.LeftControl)
                isHoldingCtrl = true;

            if (isHoldingCtrl && isHoldingF)
            {
                if (!inFocusMode)
                    inFocusMode = true;
                else
                    inFocusMode = false;
            }
        }

        if (currentEvent.type == EventType.KeyUp)
        {
            if (currentEvent.keyCode == KeyCode.F)
                isHoldingF = false;
            if (currentEvent.keyCode == KeyCode.LeftControl)
                isHoldingCtrl = false;
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

    private GameObject GenerateTilemapParentRootObject(string rootName)
    {
        var parentObject = GameObject.Find(rootName);

        if (parentObject == null)
        {
            parentObject = new GameObject(rootName);
            parentObject.transform.position = Vector3.zero;
            parentObject.transform.localScale = Vector3.one;
            parentObject.transform.rotation = Quaternion.identity;

            Undo.RegisterCreatedObjectUndo(parentObject, "Created Parent object for generated tile blocks");
        }

        return parentObject;
    }

    private bool TileWithinGrid(int x, int y)
    {
        if ((x >= 0 && x < battleGridWidth) &&
            (y >= 0 && y < battleGridHeight))
            return true;

        return false;
    }

    private void PlaceTile(Event currentEvent, Ray worldRay, Plane groundPlane)
    {
        var parent = GenerateTilemapParentRootObject("-BATTLE GRID-");

        if (!groundPlane.Raycast(worldRay, out float distance))
            return;

        Vector3 hitPoint = worldRay.GetPoint(distance);

        // Snap to grid based on tile size
        int gridX = Mathf.FloorToInt(hitPoint.x / tileSizeInMeters.x);
        int gridZ = Mathf.FloorToInt(hitPoint.z / tileSizeInMeters.z);

        if (!TileWithinGrid(gridX, gridZ))
            return;

        Vector2 gridPos = new Vector2(gridX, gridZ);

        GameObject existingTile = GetTileAtPosition(new Vector3Int(gridX, 0, gridZ));
        GameObject newTilePrefab = tileBrushPrefabHolder._tileBrushPrefabs[currentTileBrushIndex];

        // If tile already exists and is same type, skip
        if (existingTile != null && PrefabUtility.GetCorrespondingObjectFromSource(existingTile) == newTilePrefab)
        {
            return;
        }

        // Destroy old tile if exists
        if (existingTile != null)
        {
            Undo.DestroyObjectImmediate(existingTile);
        }

        // Compute world-space position
        Vector3 worldPos = new Vector3(
            gridX * tileSizeInMeters.x + tileSizeInMeters.x / 2f,
            0f,
            gridZ * tileSizeInMeters.z + tileSizeInMeters.z / 2f
        );

        // Instantiate new tile entry with grid coordinates
        TileEntry newTile = InstantiateAndSetTileEntry(worldPos, tileSizeInMeters, newTilePrefab, parent, gridPos);

        // Add or replace in dictionary
        AddOrReplaceTile(gridX, gridZ, newTile);
    }

    // TODO (Calle): Implement PlaceCharacter!

    

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
        for (int i = 0; i < tileGridProperty.arraySize; i++)
        {
            SerializedProperty entryProp = tileGridProperty.GetArrayElementAtIndex(i);
            Vector2 pos = entryProp.FindPropertyRelative("_tileIndex").vector2Value;
            GameObject tile = entryProp.FindPropertyRelative("_tile").objectReferenceValue as GameObject;

            if ((int)pos.x == position.x && (int)pos.y == position.z && tile != null)
            {
                return tile; // return only valid tiles
            }
        }

        return null;
    }

 
    private void SaveBattleGridToJSON()
    {
        CombatGridSerializedSaveData tileSaveData = new CombatGridSerializedSaveData();
        
        tileSaveData.gridWidth = battleGridWidth;
        tileSaveData.gridHeight = battleGridHeight;
        
        foreach (var entry in tileGridHolder._tileEntries)
        {
            if(entry == null) continue;

            tileSaveData.tileData.Add(new CombatGridTileData(entry._tileType, entry._tileIndex, entry._position, entry._size));
        }
        string strOutput = JsonUtility.ToJson(tileSaveData, true);   

        File.WriteAllText(Application.dataPath + "\\JSON BattleGrids\\" + fileNameJSON + ".json", strOutput);


    }
}


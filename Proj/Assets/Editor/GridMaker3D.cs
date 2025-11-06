using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;



public class GridMaker3D : EditorWindow
{
    GridMaker3D instance;

    LayerMask tileLayerMask;
    string fileNameJSON;
    int battleGridWidth = 0;
    int battleGridHeight= 0;
    int prevBattleGridWidth = 0;
    int prevBattleGridHeight = 0;
    Vector2 tileDictScrollPos;
    Vector2 windowEditorScrollPos;

    [System.Serializable]
    public class TileEntry
    {
        public Vector2 position;
        public TileType tileType;
        public GameObject tile;
    };


    [System.Serializable]
    public class TileGridSaveFormat
    {
        public List<TileEntry> tileEntries;
    };

    [System.Serializable]
    public class TileDictionary : ScriptableObject
    {
        public List<TileEntry> tileEntries = new List<TileEntry>();
    };

    TileDictionary      tileDictHolder;
    SerializedObject    tileDictHolderSO;
    SerializedProperty  tileDictProperty;

    [System.Serializable]
    public class TileBrushPrefabHolder : ScriptableObject
    {
        public GameObject[] tileBrushPrefabs;
    }

    TileBrushPrefabHolder tileBrushPrefabHolder;
    SerializedObject      tileBrushPrefabHolderSO;
    SerializedProperty    tileBrushPrefabProperty;

    GameObject tileToSpawn;
    TileEntry previewTile;
    int currentTileBrushIndex = 0;

    private bool isHoldingF = false, isHoldingCtrl = false, inFocusMode = false;
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
        tileBrushPrefabProperty = tileBrushPrefabHolderSO.FindProperty("tileBrushPrefabs");

        tileDictHolder = ScriptableObject.CreateInstance<TileDictionary>();
        tileDictHolderSO = new SerializedObject(tileDictHolder);
        tileDictProperty = tileDictHolderSO.FindProperty("tileEntries");
    }

    private void OnDisable()
    {
        if (previewTile != null && previewTile.tile != null)
            DestroyImmediate(previewTile.tile);

        GameObject parent = GameObject.Find("-BATTLE GRID-");
        if (parent != null)
            DestroyImmediate(parent);

        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnGUI()
    {
        windowEditorScrollPos = EditorGUILayout.BeginScrollView(windowEditorScrollPos);

        focusToggle = EditorGUILayout.Toggle("Focus Toggle for Draw", focusToggle);
        battleGridWidth = EditorGUILayout.IntSlider("BattleGrid Width", battleGridWidth, 0, 30);
        battleGridHeight = EditorGUILayout.IntSlider("BattleGrid Height", battleGridHeight, 0, 30);
        fileNameJSON = EditorGUILayout.TextField("FileName: ", fileNameJSON);

        if (prevBattleGridHeight != battleGridHeight || prevBattleGridWidth != battleGridWidth)
        {
            SetGridSize(battleGridWidth, battleGridHeight);
            prevBattleGridWidth = battleGridWidth;
            prevBattleGridHeight = battleGridHeight;

        }
        UpdatePrefabArray();
        UpdateTileDict();


        EditorGUILayout.EndScrollView();


        if (GUILayout.Button("Save Grid to JSON", GUILayout.Height(50)))
        {
            // Gather context info
            string message = $"You are about to save the current battle grid:\n" +
                             $"Width: {battleGridWidth}, Height: {battleGridHeight}\n" +
                             $"Tiles in grid: {tileDictProperty.arraySize}\n\n" +
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
                Debug.Log($"Battle grid saved! {tileDictProperty.arraySize} tiles exported.");
            }
            else
            {
                Debug.Log("Save cancelled.");
            }
        }

    }
    private void OnSceneGUI(SceneView sceneView)
    {
        Event currentEvent = Event.current;

        UpdateFocusDrawMode(currentEvent);

        if (IsInFocusDrawMode())
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            HandleUtility.AddDefaultControl(controlID);
            focusToggle = true;

            // Always move preview regardless of mouse button
            if (currentEvent.type == EventType.Repaint || currentEvent.type == EventType.Layout || currentEvent.type == EventType.MouseMove)
            {
                MovePreviewTile(currentEvent);
            }

            ScrollSelectBrush(currentEvent);
            DrawTiles(currentEvent);
        }
        else
        {
            focusToggle = false;
        }
        Repaint();
        SceneView.RepaintAll();
        

    }
    private void UpdatePrefabArray()
    {
        tileBrushPrefabHolderSO.Update();

        EditorGUILayout.LabelField("Prefabs to Spawn", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(tileBrushPrefabProperty, includeChildren: true);
        EditorGUI.indentLevel--;

        tileBrushPrefabHolderSO.ApplyModifiedProperties();

        EditorGUILayout.Space();
    }
    private void UpdateTileDict()
    {
        tileDictHolderSO.Update();

        EditorGUILayout.LabelField("Tiles in Grid", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        // Begin scroll view
        tileDictScrollPos = EditorGUILayout.BeginScrollView(tileDictScrollPos, GUILayout.Height(300)); // Set desired height

        EditorGUILayout.PropertyField(tileDictProperty, includeChildren: true);

        EditorGUILayout.EndScrollView();
        EditorGUI.indentLevel--;

        tileDictHolderSO.ApplyModifiedProperties();
        EditorGUILayout.Space();
    }
    private void UpdatePreviewTile()
    {
        if (tileBrushPrefabHolder == null || tileBrushPrefabHolder.tileBrushPrefabs.Length == 0)
            return;

        GameObject prefab = tileBrushPrefabHolder.tileBrushPrefabs[currentTileBrushIndex];
        if (prefab == null)
            return;

        // Destroy previous preview
        if (previewTile.tile != null)
            DestroyImmediate(previewTile.tile);

        previewTile.tile = Instantiate(prefab);
        previewTile.tile.name = "PreviewTile";
        previewTile.tile.hideFlags = HideFlags.HideAndDontSave;

        // Make preview semi-transparent (safe for HDRP/URP/other shaders)
        foreach (var renderer in previewTile.tile.GetComponentsInChildren<Renderer>())
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
        if (previewTile.tile == null)
            return;
        
        Ray worldRay = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(worldRay, out float distance))
        {
            Vector3 hitPoint = worldRay.GetPoint(distance);

            // Snap to grid
            previewTile.tile.transform.position = new Vector3(
                Mathf.Floor(hitPoint.x) + 0.5f,
                0f,
                Mathf.Floor(hitPoint.z) + 0.5f
            );
        }
    }
    private void AddOrReplaceTile(int x, int y, TileEntry tile)
    {
        tileDictHolderSO.Update();

        // Look for existing entry by looping over all tiles
        for (int i = 0; i < tileDictProperty.arraySize; i++)
        {
            SerializedProperty entryProp = tileDictProperty.GetArrayElementAtIndex(i);
            Vector2 pos = entryProp.FindPropertyRelative("position").vector2Value;

            if ((int)pos.x == x && (int)pos.y == y)
            {
                // Replace existing tile reference
                entryProp.FindPropertyRelative("tile").objectReferenceValue = tile.tile;
                TileType replacingTileType = tile.tile.GetComponent<CombatGridTile>().GetTileType();
                entryProp.FindPropertyRelative("tileType").enumValueIndex = (int)replacingTileType;
                tileDictHolderSO.ApplyModifiedProperties();
                return;
            }
        }

        // If not found, create new entry
        int index = tileDictProperty.arraySize;
        tileDictProperty.arraySize++;
        SerializedProperty newEntry = tileDictProperty.GetArrayElementAtIndex(index);
        newEntry.FindPropertyRelative("position").vector2Value = new Vector2(x, y);
        newEntry.FindPropertyRelative("tile").objectReferenceValue = tile.tile;
        TileType tileType = tile.tile.GetComponent<CombatGridTile>().GetTileType();
        newEntry.FindPropertyRelative("tileType").enumValueIndex = (int)tileType;

        tileDictHolderSO.ApplyModifiedProperties();
    }

    private void SetGridSize(int width, int height)
    {
        SerializedProperty entriesProp = tileDictHolderSO.FindProperty("tileEntries");
        tileDictHolderSO.Update();

        // Remove tiles outside new bounds
        List<int> removeIndices = new List<int>();
        for (int i = 0; i < entriesProp.arraySize; i++)
        {
            SerializedProperty entryProp = entriesProp.GetArrayElementAtIndex(i);
            SerializedProperty posProp = entryProp.FindPropertyRelative("position");
            SerializedProperty tileProp = entryProp.FindPropertyRelative("tile");

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
                    Vector2 pos = entryProp.FindPropertyRelative("position").vector2Value;
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
                    newEntry.FindPropertyRelative("position").vector2Value = new Vector2(x, y);
                    newEntry.FindPropertyRelative("tile").objectReferenceValue = null;
                }
            }
        }

        tileDictHolderSO.ApplyModifiedProperties();
    }



    private void RenderCurrentSelectedTile(Ray worldRay, Plane groundPlane, Event currentEvent)
    {

    }

    private void ScrollSelectBrush(Event currentEvent)
    {
        if (currentEvent.type == EventType.ScrollWheel)
        {
            if (tileBrushPrefabHolder.tileBrushPrefabs.Length == 0) return;

            currentTileBrushIndex -= (int)Mathf.Sign(currentEvent.delta.y); // scroll direction
            if (currentTileBrushIndex < 0) currentTileBrushIndex = tileBrushPrefabHolder.tileBrushPrefabs.Length - 1;
            if (currentTileBrushIndex >= tileBrushPrefabHolder.tileBrushPrefabs.Length) currentTileBrushIndex = 0;

            UpdatePreviewTile();
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
            ScrollSelectBrush(currentEvent);
            RenderCurrentSelectedTile(worldRay, groundPlane, currentEvent);
            PlaceTile(currentEvent, worldRay, groundPlane);
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

        float distance = 0.0f;
        if (!MouseRayHitGroundPlane(currentEvent, ref distance))
            return;

        Vector3 hitPoint = worldRay.GetPoint(distance);
        Vector3Int gridPos = Vector3Int.FloorToInt(hitPoint);
        if (!TileWithinGrid(gridPos.x, gridPos.z)) return;

        GameObject existingTile = GetTileAtPosition(gridPos);
        GameObject newTilePrefab = tileBrushPrefabHolder.tileBrushPrefabs[currentTileBrushIndex];

        // If there is a tile already
        if (existingTile != null)
        {
            // If it's the same type, just return
            if (PrefabUtility.GetCorrespondingObjectFromSource(existingTile) == newTilePrefab)
            {
                Debug.Log("Tile already of this type, skipping.");
                return;
            }

            // Otherwise, destroy old tile
            Undo.DestroyObjectImmediate(existingTile);
        }

        // Place the new tile
        Vector3 adjustmentPosition = new Vector3(0.5f, 0.0f, 0.5f);
        Vector3 finalPosition = gridPos + adjustmentPosition;
        finalPosition.y = 0.0f;

        TileEntry newTile = new TileEntry();
        newTile.tile = Instantiate(newTilePrefab);
        newTile.tile.transform.position = finalPosition;
        newTile.tile.transform.SetParent(parent.transform);
        newTile.tileType = newTile.tile.GetComponent<CombatGridTile>().GetTileType();
        Undo.RegisterCreatedObjectUndo(newTile.tile, "Placed/Updated Tile");

        AddOrReplaceTile(gridPos.x, gridPos.z, newTile);
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
        for (int i = 0; i < tileDictProperty.arraySize; i++)
        {
            SerializedProperty entryProp = tileDictProperty.GetArrayElementAtIndex(i);
            Vector2 pos = entryProp.FindPropertyRelative("position").vector2Value;
            GameObject tile = entryProp.FindPropertyRelative("tile").objectReferenceValue as GameObject;

            if ((int)pos.x == position.x && (int)pos.y == position.z && tile != null)
            {
                return tile; // return only valid tiles
            }
        }

        return null;
    }

 
    private void SaveBattleGridToJSON()
    {
        /*
        TileGridSaveFormat tileEntriesSave = new TileGridSaveFormat();
        tileEntriesSave.tileEntries = new List<TileEntry>(tileDictHolder.tileEntries);
        string strOutput = JsonUtility.ToJson(tileEntriesSave, true);
        */
        CombatGridTileSerializedSaveData tileSaveData = new CombatGridTileSerializedSaveData();

        foreach(var entry in tileDictHolder.tileEntries)
        {
            if(entry == null) continue;

            tileSaveData.tileData.Add(new CombatGridTileData(entry.tileType, entry.position));
        }
        string strOutput = JsonUtility.ToJson(tileSaveData, true);   

        File.WriteAllText(Application.dataPath + "\\JSON BattleGrids\\" + fileNameJSON + ".json", strOutput);


    }
}


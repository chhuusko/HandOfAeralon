using UnityEngine;
using UnityEditor;


public class CleanupEditorTiles
{
    [MenuItem("Custom Tools/Cleanup Ghost Grid Tiles")]
    static void CleanupGhostTiles()
    {
        string rootName = "-BATTLE GRID-";
        GameObject parent = GameObject.Find(rootName);

        if (parent != null)
        {
            Object.DestroyImmediate(parent); // destroys all child tiles safely
            Debug.Log("Destroyed leftover grid tiles.");
        }
        else
        {
            Debug.Log("No leftover grid tiles found.");
        }

        // Also clean preview tile if still exists
        var preview = GameObject.Find("PreviewTile");
        if (preview != null)
            Object.DestroyImmediate(preview);
    }
}
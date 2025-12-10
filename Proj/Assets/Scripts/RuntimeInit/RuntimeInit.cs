using UnityEngine;

public class RuntimeInit
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnAppStart()
    {
        CursorDatabase cursorDatabase = Resources.Load<CursorDatabase>("ScriptableObjects/CursorDatabase");
        
        if(cursorDatabase != null )
        {
            if (cursorDatabase.useUICursor)
            {
                Cursor.visible = false;
            }
            Texture2D cursor = cursorDatabase.defaultCursor;
            Cursor.SetCursor(cursorDatabase.defaultCursor, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            DebugLog.CJLog("CursorDatabase not found in Resources folder.");
        }
    }
}

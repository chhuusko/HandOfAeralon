using UnityEngine;

public class RuntimeInit
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnAppStart()
    {
        CursorDatabase cursorDatabase = Resources.Load<CursorDatabase>("CursorDatabase");
        
        if(cursorDatabase != null )
        {

            Texture2D cursor = Resources.Load<Texture2D>("Cursor/MyCursor");
            Vector2 hotspot = Vector2.zero;

            Cursor.SetCursor(cursor, hotspot, CursorMode.Auto);
        }
        
    }
}

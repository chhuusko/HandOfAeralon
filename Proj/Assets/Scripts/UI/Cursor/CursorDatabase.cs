using UnityEngine;

[CreateAssetMenu(fileName = "CursorDatabase", menuName = "Scriptable Objects/CursorDatabase")]
public class CursorDatabase : ScriptableObject
{
    public bool useUICursor;
    
    public Texture2D defaultCursor;
    public Texture2D dragCursor;
    public Texture2D hoverCursor;
}

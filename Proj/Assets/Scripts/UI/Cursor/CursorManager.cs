using System;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance;
    
    private CursorDatabase _cursorDatabase;
    
    private Texture2D defaultCursor;
    private Texture2D dragCursor;
    private Texture2D hoverCursor;
    
    private bool _isDragging;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        LoadCursorDatabase();

        CardHandManager.onHover += SetHoverCursor;
        CardHandManager.onDrag += SetDragCursor;
    }

    private void OnDisable()
    {
        CardHandManager.onHover -= SetHoverCursor;
        CardHandManager.onDrag -= SetDragCursor;
    }
    
    private void LoadCursorDatabase()
    {
        _cursorDatabase = Resources.Load<CursorDatabase>("ScriptableObjects/CursorDatabase");
        defaultCursor = _cursorDatabase.defaultCursor;
        dragCursor = _cursorDatabase.dragCursor;
        hoverCursor = _cursorDatabase.hoverCursor;
    }

    private void SetHoverCursor(bool isHovering)
    {
        if (!hoverCursor || !defaultCursor || _isDragging)
        {
            return;
        }
        
        Cursor.SetCursor((isHovering ? hoverCursor : defaultCursor), Vector2.zero, CursorMode.Auto);
    }
    
    private void SetDragCursor(bool isDragging)
    {
        if (!dragCursor || !defaultCursor)
        {
            return;
        }
        
        _isDragging = isDragging;
        
        Debug.Log("Drag Cursor");
        
        Cursor.SetCursor((isDragging ? dragCursor : defaultCursor), Vector2.zero, CursorMode.Auto);
    }
}

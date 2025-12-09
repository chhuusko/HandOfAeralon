using System;
using UnityEngine;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance;

    [SerializeField] private RectTransform _cursorImage;
    [SerializeField] private Vector2 _hotSpotOffset;
    
    private CursorDatabase _cursorDatabase;
    
    private Sprite defaultCursor;
    private Sprite dragCursor;
    private Sprite hoverCursor;
    
    private Texture2D defaultCursorTexture;
    private Texture2D dragCursorTexture;
    private Texture2D hoverCursorTexture;
    
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

        CardHandManager.onHover += UpdateHoverCursor;
        CardHandManager.onDrag += UpdateDragCursor;
    }

    private void LateUpdate()
    {
        if (!_cursorDatabase.useUICursor)
        {
            _cursorImage.gameObject.SetActive(false);
            return;
        }

        if (Cursor.visible || Cursor.lockState != CursorLockMode.None)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.None;
        }
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _cursorImage.parent as RectTransform, Input.mousePosition, null, out Vector2 pos);
        
        _cursorImage.anchoredPosition = pos + _hotSpotOffset;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && _cursorDatabase.useUICursor)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void OnDisable()
    {
        CardHandManager.onHover -= UpdateHoverCursor;
        CardHandManager.onDrag -= UpdateDragCursor;
    }
    
    private void LoadCursorDatabase()
    {
        _cursorDatabase = Resources.Load<CursorDatabase>("ScriptableObjects/CursorDatabase");
        
        defaultCursorTexture = _cursorDatabase.defaultCursor;
        dragCursorTexture = _cursorDatabase.dragCursor;
        hoverCursorTexture = _cursorDatabase.hoverCursor;
        
        defaultCursor = Sprite.Create(_cursorDatabase.defaultCursor, 
            new Rect(0, 0, _cursorDatabase.defaultCursor.width, _cursorDatabase.defaultCursor.height),
            new Vector2(0.5f, 0.5f));
        dragCursor = Sprite.Create(_cursorDatabase.dragCursor, 
            new Rect(0, 0, _cursorDatabase.dragCursor.width, _cursorDatabase.dragCursor.height),
            new Vector2(0.5f, 0.5f));
        hoverCursor = Sprite.Create(_cursorDatabase.hoverCursor, 
            new Rect(0, 0, _cursorDatabase.hoverCursor.width, _cursorDatabase.hoverCursor.height),
            new Vector2(0.5f, 0.5f));

        if (_cursorDatabase.useUICursor)
        {
            SetUICursor(defaultCursor);
        }
        else
        {
            SetCursor(defaultCursorTexture);
        }
    }

    private void UpdateHoverCursor(bool isHovering)
    {
        if (!hoverCursor || !defaultCursor || _isDragging)
        {
            return;
        }

        if (_cursorDatabase.useUICursor)
        {
            SetUICursor(isHovering ? hoverCursor : defaultCursor);
        }
        else
        {
            SetCursor(isHovering ? hoverCursorTexture : defaultCursorTexture);
        }
    }
    
    private void UpdateDragCursor(bool isDragging)
    {
        if (!dragCursor || !defaultCursor)
        {
            return;
        }
        
        _isDragging = isDragging;

        if (_cursorDatabase.useUICursor)
        {
            SetUICursor(isDragging ? dragCursor : defaultCursor);
        }
        else
        {
            SetCursor(isDragging ? dragCursorTexture : defaultCursorTexture);
        }
    }
    
    private void SetCursor(Texture2D cursor)
    {
        Cursor.SetCursor(cursor, Vector2.zero, CursorMode.ForceSoftware);
    }

    private void SetUICursor(Sprite cursor)
    {
        _cursorImage.GetComponent<Image>().sprite = cursor;
    }
}

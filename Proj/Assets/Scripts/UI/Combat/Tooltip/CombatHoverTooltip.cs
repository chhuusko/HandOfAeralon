using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CombatHoverTooltip : MonoBehaviour
{
    [SerializeField] private Canvas _tooltipCanvas;
    [SerializeField] private Camera _tooltipOverlayCamera;


    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _description;
    private bool _isHovering;
    private RectTransform _rectTransform;

    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        Hide();
    }

    // Update is called once per frame
    void Update()
    {
        if( _isHovering )
        {
            Vector2 mousePos = Input.mousePosition;

            // Convert the mouse position from screen space to local canvas space
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _tooltipCanvas.transform as RectTransform,
                mousePos,
                _tooltipOverlayCamera,   // Pass the UI camera to handle camera stacking
                out Vector2 localPoint);

            // Now set the position
            localPoint.x += 80f + _rectTransform.sizeDelta.x/2f;
            localPoint.y += -40f + _rectTransform.sizeDelta.y/2f;

            _rectTransform.anchoredPosition = localPoint;
            //_rectTransform.anchoredPosition = mousePos;
        }
    }


    public void UpdateText(string title, string description)
    {
        SetTitle(title);
        SetDescription(description);
    }


    public void SetIsHovering(bool isHovering) { _isHovering = isHovering; }
    public void SetTitle(string title) { _title.text = title; }
    public void SetDescription(string description) { _description.text = description; }

    public void Hide()
    {
        _isHovering = false;
        Vector3 pos = transform.position;
        pos.x = -9999;
        transform.position = pos;
    }
    public void Show(string title, string description)
    {
        _isHovering = true;
        UpdateText(title, description);
    }
}

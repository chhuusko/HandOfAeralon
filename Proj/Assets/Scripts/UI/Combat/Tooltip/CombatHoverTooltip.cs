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
        //DEBUGLogRayCastHits();
        if ( _isHovering )
        {
            Vector2 mousePos = Input.mousePosition;
            RectTransform canvasRect = _tooltipCanvas.transform as RectTransform;

            // Convert the mouse position from screen space to local canvas space
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                mousePos,
                _tooltipOverlayCamera,   // Pass the UI camera to handle camera stacking
                out Vector2 localPoint);

            // Now set the position
            localPoint.x += 80f + _rectTransform.sizeDelta.x/2f;
            localPoint.y += -40f + _rectTransform.sizeDelta.y/2f;

            ClampToScreenBounds(localPoint);

            _rectTransform.anchoredPosition = localPoint;
        }
    }

    private void DEBUGLogRayCastHits()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, 1000f);

            Debug.Log($"Raycast hit count: {hits.Length}");

            foreach (var hit in hits)
            {
                DebugLog.CJLogWarning("Hit: " + hit.collider.gameObject.name +
                          " (Layer: " + LayerMask.LayerToName(hit.collider.gameObject.layer) + ")");
            }
        }
    }

    private void ClampToScreenBounds(Vector2 localPoint)
    {

        RectTransform canvasRect = _tooltipCanvas.transform as RectTransform;
        Vector2 tooltipSize = _rectTransform.sizeDelta;
        Vector2 canvasSize = canvasRect.rect.size;

        float halfW = tooltipSize.x * 0.5f;
        float halfH = tooltipSize.y * 0.5f;

        float minX = -canvasSize.x * 0.5f + halfW;
        float maxX = canvasSize.x * 0.5f - halfW;
        float minY = -canvasSize.y * 0.5f + halfH;
        float maxY = canvasSize.y * 0.5f - halfH;

        localPoint.x = Mathf.Clamp(localPoint.x, minX, maxX);
        localPoint.y = Mathf.Clamp(localPoint.y, minY, maxY);
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

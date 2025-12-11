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
    [SerializeField] private float _offsetY;
    private bool _isHovering;
    private RectTransform _rectTransform;
    private Vector2 _buttonPosition;

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
            RectTransform canvasRect = _tooltipCanvas.transform as RectTransform;

            // Convert the BUTTON screen position → canvas local position
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                _buttonPosition,               // ✔ this is already in screen space
                _tooltipOverlayCamera,         // camera of the canvas
                out Vector2 localPoint);    

            // Optional offset so tooltip appears slightly above/right of the button
            //localPoint.x += 40f;
            //localPoint.y -= 40f;
            

            // Clamp tooltip inside canvas bounds
            localPoint = ClampToScreenBounds(localPoint, canvasRect.rect.size);

            // Apply position
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

    private Vector2 ClampToScreenBounds(Vector2 localPoint, Vector2 canvasSize)
    {
        Vector2 tooltipSize = _rectTransform.sizeDelta;

        float halfW = tooltipSize.x * 0.5f;
        float halfH = tooltipSize.y * 0.5f;

        float minX = -canvasSize.x * 0.5f + halfW;
        float maxX = canvasSize.x * 0.5f - halfW;
        float minY = -canvasSize.y * 0.5f + halfH;
        float maxY = canvasSize.y * 0.5f - halfH;

        localPoint.x = Mathf.Clamp(localPoint.x, minX, maxX);
        localPoint.y = Mathf.Clamp(localPoint.y, minY, maxY);

        return localPoint;
    }

    public void UpdateText(string title, string description)
    {
        SetTitle(title);
        SetDescription(description);
    }

    public void UpdateText(string title, string description, RectTransform targetRect)
    {
        SetTitle(title);
        SetDescription(description);
        _isHovering = true;

        SetTitle(title);
        SetDescription(description);
        _isHovering = true;


        _buttonPosition = targetRect.anchoredPosition;
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

    public void Show(string title, string description, RectTransform position)
    {
        _isHovering = true;
        UpdateText(title, description, position);
    }


}

using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CombatHoverTooltip : MonoBehaviour, IPointerExitHandler
{
    [SerializeField] private Canvas _tooltipCanvas;
    [SerializeField] private Camera _tooltipOverlayCamera;
    [SerializeField] private Camera _combatHUDOverlayCamera;
    

    [SerializeField] private Slider _slider;
    [SerializeField] private Image _sliderInnerArea;
    [SerializeField] private float _sliderSpeed;
    [SerializeField] private Color _defaultColor;
    [SerializeField] private Color _finishedColor;
    private bool _bSliderFinished = true;
    private bool _bTooltipLocked = false;


    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _description;
    [SerializeField] private float _offsetY;
    
    private bool _isHovering;
    private RectTransform _rectTransform;
    private RectTransform _targetRectTransform;
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
        
        if(!_bSliderFinished)
        {
            UpdateSlider();
        }

        if(IsLocked() && 
           (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
        {
            Hide();
            _bTooltipLocked = false;
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

    public void UpdateText(string title, string description, RectTransform targetRect)
    {
        if (_bTooltipLocked)
            return;

        SetTitle(title);
        SetDescription(description);
        _isHovering = true;

        SetTitle(title);
        SetDescription(description);
        _isHovering = true;

        _targetRectTransform = targetRect;
        
        Vector3[] corners = new Vector3[4];
        targetRect.GetWorldCorners(corners);
        Vector3 topCenter = (corners[1] + corners[2]) * 0.5f;

        // NOTE (Calle): So you have to pass the camera that the UI-element is rendered in. AbilityButton is rendered
        // on CombatHUDOverlayCamera for example.

        Camera camera = targetRect.GetComponentInParent<Canvas>().worldCamera;

        _buttonPosition = RectTransformUtility.WorldToScreenPoint(camera, topCenter);


        RectTransform canvasRect = _tooltipCanvas.transform as RectTransform;

        // Convert the BUTTON screen position to canvas local position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            _buttonPosition,               
            _tooltipOverlayCamera,         
            out Vector2 localPoint);

        
        localPoint.x += _rectTransform.rect.width / 2f + 10f;
        localPoint.y += _rectTransform.rect.height / 2f + 10f;


        localPoint = ClampToScreenBounds(localPoint, canvasRect.rect.size);

        _rectTransform.anchoredPosition = localPoint;

        StartSlider();
    }

    public void SetIsHovering(bool isHovering) { _isHovering = isHovering; }
    public void SetTitle(string title) { _title.text = title; }
    public void SetDescription(string description) { _description.text = description; }

    public void Hide()
    {
        _isHovering = false;
        StopSlider();
        Vector3 pos = transform.position;
        pos.x = -9999;
        transform.position = pos;
    }
    public void Show(string title, string description, RectTransform rectTransform)
    {
        _isHovering = true;
        UpdateText(title, description, rectTransform);
    }

    public void ShowAbility(string title, string description, RectTransform rectTransform)
    {
        _isHovering = true;
        UpdateText(title, description, rectTransform);
    }

    private void StartSlider() 
    {
        _slider.value = 0f;
        _sliderInnerArea.color = _defaultColor;
        _bSliderFinished = false; 
    }
    private void StopSlider() { _bSliderFinished = true; }
    private void UpdateSlider()
    {
        _slider.value += _sliderSpeed;
        if(_slider.value >= 1f)
        {
            _slider.value = 0f;
            _sliderInnerArea.color = _finishedColor;
            _bTooltipLocked = true;
            StopSlider();
        }
    }

    public bool IsLocked() { return _bTooltipLocked; }

    public void SetHoverLockSpeed(float newSpeed) 
    { 
        //NOTE (Calle): 0.1 is very fast so we should map the newSpeed to values between 0.0 and 0.1,
        // 0.0 = the hover lock will never lock
        // 0.1 = the hover lock will lock superquick

        float mappedSpeed = 0.1f * newSpeed;
        _sliderSpeed = mappedSpeed; 
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _bTooltipLocked = false;
        Hide();
    }
}

using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CombatHoverTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{


    [SerializeField] private GameObject _hoverTooltipPrefab;
    [SerializeField] private GameObject _subHoverTooltipObject;
    
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
    
    private bool _bIsHovering;
    private bool _bCloseRequest;
    private bool _bHoverLockON;
    private RectTransform _rectTransform;
    private RectTransform _targetRectTransform;
    private Vector2 _buttonPosition;

    public bool _hideOnStart = true;

    // LinkInfo
    private int _lastLinkIndex = -1;
    void Start()
    {
        Selector._instance.OnCharacterDeselected += Hide;
        _rectTransform = GetComponent<RectTransform>();
        if(_hideOnStart)
            Hide(); 
        _sliderSpeed = PlayerSettingsManager.GetInstance().GetHoverLockSpeed();
        SetSliderValue(_sliderSpeed);
        SetHoverLockSpeed(_sliderSpeed);

        if (_sliderSpeed <= 0f)
            _slider.gameObject.SetActive(false);
        else
            _slider.gameObject.SetActive(true);

    }

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    void OnDisable()
    {
        Selector._instance.OnCharacterDeselected -= Hide;
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
           (Input.GetMouseButtonDown(1)))
        {
            Hide();
            _bTooltipLocked = false;
        }

        int currentLinkIndex = TMP_TextUtilities.FindIntersectingLink(_description, Input.mousePosition, _tooltipOverlayCamera );

        // Hover enter
        if (currentLinkIndex != -1 && currentLinkIndex != _lastLinkIndex)
        {
            _lastLinkIndex = currentLinkIndex;
            TMP_LinkInfo linkInfo = _description.textInfo.linkInfo[currentLinkIndex];
            Debug.Log("Hover over link: " + linkInfo.GetLinkID());
            Burn burn = new Burn();
            StatusEffectData data = StatusEffectDataRegistry.GetDataForType(burn.GetType());

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                                                                    _tooltipCanvas.transform as RectTransform,
                                                                    Input.mousePosition,
                                                                    _tooltipOverlayCamera,
                                                                    out Vector2 localPoint);


            // TODO (Calle): The sub tooltip gets hidden directly and that's why its position is way off...
            // right now just setting a bool if it should hide on start

            // Also the tooltip will disappear directly as the text link is not hovered once the subtooltip spawns because it blocks it
            // and hence, gets destroyed.
            _subHoverTooltipObject = Instantiate(_hoverTooltipPrefab, localPoint, Quaternion.identity, _tooltipCanvas.transform);

            _subHoverTooltipObject.GetComponent<CombatHoverTooltip>().Show(data.Name, data.Description, localPoint);
            _subHoverTooltipObject.GetComponent<CombatHoverTooltip>()._hideOnStart = false;
            RectTransform tooltipRect = _subHoverTooltipObject.GetComponent<RectTransform>();
            
            tooltipRect.anchoredPosition = localPoint; // local X/Y
            tooltipRect.localPosition = new Vector3(tooltipRect.localPosition.x + 600f, tooltipRect.localPosition.y, 0f); // force Z = 0
        }

        // Hover exit
        if (_lastLinkIndex != -1 && currentLinkIndex == -1)
        {
            if (_subHoverTooltipObject != null)
                Destroy(_subHoverTooltipObject);
        }

        _lastLinkIndex = currentLinkIndex;
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
        if (_bTooltipLocked && targetRect.gameObject == _targetRectTransform.gameObject)
            return;

        SetTitle(title);
        SetDescription(description);

        _targetRectTransform = targetRect;
        
        Vector3[] corners = new Vector3[4];
        targetRect.GetWorldCorners(corners);
        Vector3 topCenter = (corners[1] + corners[2]) * 0.5f;

        // NOTE (Calle): So you have to pass the camera that the UI-element is rendered in. AbilityButton is rendered
        // on CombatHUDOverlayCamera for example. And the hover tooltip is rendered on TooltipOverlayCamera.
        Camera camera = targetRect.GetComponentInParent<Canvas>().worldCamera;

        _buttonPosition = RectTransformUtility.WorldToScreenPoint(camera, topCenter);

        RectTransform canvasRect = _tooltipCanvas.transform as RectTransform;

        // Convert the ability BUTTON screen position to canvas local position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            _buttonPosition,               
            _tooltipOverlayCamera,         
            out Vector2 localPoint);

        localPoint.x += _rectTransform.rect.width / 2f - 10f;

        // NOTE (Calle): This is a fkn MAGIC value that seemed to work for not making the hovertooltip flicker when mouse was hovering over
        // both the abilitybutton and the tooltip.
        localPoint.y += _rectTransform.rect.height / 2f + 5f;

        localPoint = ClampToScreenBounds(localPoint, canvasRect.rect.size);

        _rectTransform.anchoredPosition = localPoint;

        StartSlider();
    }

    private void SetHoverLock(bool bShouldHoverLock) { _bHoverLockON = bShouldHoverLock; }
    public void SetIsHovering(bool isHovering) { _bIsHovering = isHovering; }
    public void SetIsRequsetingClose(bool isRequestingClose) { _bCloseRequest = isRequestingClose; }

    public void SetTitle(string title) { _title.text = title; }

    public void SetDescription(string description) { _description.text = description; }

    public void Hide()
    {
        _bTooltipLocked = false;
        StopSlider();
        Vector3 pos = transform.position;
        pos.x = -2000;
        transform.position = pos;
    }

    public void Show(string title, string description, Vector2 position)
    {
        SetTitle(title);
        SetDescription(description);
        _rectTransform.anchoredPosition = position;
    }
    public void Show(string title, string description, RectTransform rectTransform)
    {
        UpdateText(title, description, rectTransform);
    }

    public void ShowAbility(string title, string description, RectTransform rectTransform)
    {
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
        SetSliderValue(_slider.value + _sliderSpeed);
        if(_slider.value >= 1f)
        {
            SetSliderValue(0f);
            _sliderInnerArea.color = _finishedColor;
            _bTooltipLocked = true;
            StopSlider();
        }
    }

    public bool IsLocked() { return _bTooltipLocked; }

    public void SetHoverLockSpeed(float newSpeed) 
    { 
        SetMappedSliderSpeed(newSpeed);
    }

    //NOTE (Calle): 0.1 is very fast so we should map the newSpeed to values between 0.0 and 0.1,
    // 0.0 = the hover lock will never lock
    // 0.1 = the hover lock will lock superquick
    private void SetMappedSliderSpeed(float speed)
    {
        _sliderSpeed = 0.1f * speed;

        // NOTE (Calle): The save setting value should not be mapped, 1 is 1 in saved settings
        PlayerSettingsManager.GetInstance().SetHoverLockSpeed(speed); 

        if (_sliderSpeed <= 0.0f)
        {
            SetHoverLock(false);
            _slider.gameObject.SetActive(false);
        }
        else
        {
            SetHoverLock(true);
            _slider.gameObject.SetActive(true);
        }
    }

    public bool GetIsHovering() { return _bIsHovering; }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetIsHovering(true);
    }

    bool IsRequestingClose() { return _bCloseRequest; }
    public void OnPointerExit(PointerEventData eventData)
    {
        SetIsHovering(false);
        Hide();
    }

    public void SetSliderValue(float value)
    {
        _slider.value = value;
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

    
}

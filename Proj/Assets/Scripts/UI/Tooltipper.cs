using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltipper : MonoBehaviour
{
    public static Tooltipper _instance;

    void Awake()
    {
        if (Tooltipper._instance != null && Tooltipper._instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Tooltipper._instance = this;
        }
    }

    [SerializeField] private RectTransform _panel;
    [SerializeField] private TMP_Text _tmpText;
    [SerializeField] private Vector2 _offset = new Vector2(15, -15);
    [SerializeField] private float hoverTime = 1f;
    private Vector3 _lastMousePos;
    private float _hoverTimer = 0f;
    private GameObject _currentObject = null;
    private Canvas _canvas;

    void Start()
    {
        _canvas = GetComponent<Canvas>();

        if (_canvas == null)
        {
            Debug.LogError("Tooltipper.cs | Canvas not found!");
        }

        HideTooltip();
    }

    void Update()
    {
        ScanForTooltip();
        UpdatePosition();
    }

    private void ScanForTooltip()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 999f))
        {
            if (hit.collider.gameObject != _currentObject)
            {
                _currentObject = hit.collider.gameObject;

                if (_currentObject.TryGetComponent(out TooltipComponent component))
                {
                    ShowTooltip();

                    string dynamicTooltip = GenerateTooltip();
                    _tmpText.text = dynamicTooltip + component.GetTooltip();

                    return;
                }

                HideTooltip();
            }
        }
    }

    private void UpdatePosition()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(_tmpText.rectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_panel);

        RectTransform tooltipRect = _panel;
        Vector2 tooltipSize = tooltipRect.rect.size;
        Vector2 pivot = tooltipRect.pivot;

        Vector2 pos = (Vector2)Input.mousePosition + _offset;

        // Clamp X
        if (pos.x + tooltipSize.x * (1 - pivot.x) > Screen.width)
            pos.x = Screen.width - tooltipSize.x * (1 - pivot.x);
        if (pos.x - tooltipSize.x * pivot.x < 0)
            pos.x = tooltipSize.x * pivot.x;

        // Clamp Y
        if (pos.y + tooltipSize.y * (1 - pivot.y) > Screen.height)
            pos.y = Screen.height - tooltipSize.y * (1 - pivot.y);
        if (pos.y - tooltipSize.y * pivot.y < 0)
            pos.y = tooltipSize.y * pivot.y;

        tooltipRect.position = pos;
    }

    /*
    void Update()
    {
        if (_canvas == null) return;

        Vector3 mousePos = Input.mousePosition;

        if ((mousePos - _lastMousePos).sqrMagnitude > 1f)
        {
            _hoverTimer = 0f;
            _lastMousePos = mousePos;
            HideTooltip();
            return;
        }

        _hoverTimer += Time.deltaTime;

        if (_hoverTimer >= hoverTime)
        {
            CheckForTooltip();
        }
    }

    private void CheckForTooltip()
    {
        _hoverTimer = 0f;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 999f))
        {
            if (hit.collider.gameObject != _currentObject)
            {
                _currentObject = hit.collider.gameObject;

                if (_currentObject.TryGetComponent(out TooltipComponent component))
                {
                    ShowTooltip();

                    string dynamicTooltip = GenerateTooltip();

                    _tmpText.text = dynamicTooltip + component.GetTooltip();

                    LayoutRebuilder.ForceRebuildLayoutImmediate(_tmpText.rectTransform);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(_panel);

                    RectTransform tooltipRect = _panel;
                    Vector2 tooltipSize = tooltipRect.rect.size;
                    Vector2 pivot = tooltipRect.pivot;

                    Vector2 pos = (Vector2)Input.mousePosition + _offset;

                    // Clamp X
                    if (pos.x + tooltipSize.x * (1 - pivot.x) > Screen.width)
                        pos.x = Screen.width - tooltipSize.x * (1 - pivot.x);
                    if (pos.x - tooltipSize.x * pivot.x < 0)
                        pos.x = tooltipSize.x * pivot.x;

                    // Clamp Y
                    if (pos.y + tooltipSize.y * (1 - pivot.y) > Screen.height)
                        pos.y = Screen.height - tooltipSize.y * (1 - pivot.y);
                    if (pos.y - tooltipSize.y * pivot.y < 0)
                        pos.y = tooltipSize.y * pivot.y;

                    tooltipRect.position = pos;

                    return;
                }

                HideTooltip();
            }
        }
    }
    */

    private void ShowTooltip()
    {
        _panel.gameObject.SetActive(true);
    }

    private void HideTooltip()
    {
        _currentObject = null;
        _tmpText.text = "";
        _panel.gameObject.SetActive(false);
    }

    private string GenerateTooltip()
    {
        string result = "";

        if (_currentObject.TryGetComponent<Character>(out Character c))
        {
            result += $"Faction: {c.GetFaction()}"
                    + $"\nClass: {c.GetCharacterClass()}" 
                    + $"\nHP: {c.GetCurrentHealth()}/{c.GetMaxHealth()}" 
                    + $"\n";
        }

        return result;
    }
}

using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public static Tutorial Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] private RectTransform[] _popups;

    private Canvas _canvas;
    private int _currentPopup = 0;

    void Start()
    {
        _canvas = GetComponent<Canvas>();

        if (_canvas == null)
        {
            Debug.LogError("Tutorial.cs | Canvas not found!");
        }

        HidePopups();
    }

    public void ShowPopup(int popup)
    {
        if (popup > 0 && popup < _popups.Length)
        {
            for (int i = 0; i < _popups.Length; i++)
            {
                _popups[i].gameObject.SetActive(false);
            }

            _popups[popup].gameObject.SetActive(true);
            _currentPopup = popup;
        }
        else
        {
            Debug.LogError($"ShowPopup({popup}) INDEX OUT OF BOUNDS for {name}.");
        }
    }

    public void HidePopups()
    {
        for (int i = 0; i < _popups.Length; i++)
        {
            _popups[i].gameObject.SetActive(false);
        }
    }

    public void NextPopup()
    {
        _currentPopup++;
        ShowPopup(_currentPopup);
    }

    public void PreviousPopup()
    {
        _currentPopup--;
        ShowPopup(_currentPopup);
    }

    public void Reset()
    {
        _currentPopup = 0;
        HidePopups();
    }
}

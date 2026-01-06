using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] private TutorialPopup[] _popups;
    [SerializeField] private Image _blocker;

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
        
        if (_currentPopup == 0)
        {
            StartCoroutine(FirstPopup());
        }
    }

    public void ShowPopup(int popup)
    {
        if (popup >= 0 && popup < _popups.Length)
        {
            for (int i = 0; i < _popups.Length; i++)
            {
                _popups[i].gameObject.SetActive(false);
            }

            _blocker.gameObject.SetActive(true);
            Tooltipper._instance.HideTooltip();
            _popups[popup].gameObject.SetActive(true);
            _currentPopup = popup;
        }
        else
        {
            Debug.LogError($"ShowPopup({popup}) INDEX OUT OF BOUNDS for {name}.");
            HidePopups();
        }
    }

    public void HidePopups()
    {
        for (int i = 0; i < _popups.Length; i++)
        {
            _popups[i].Reset();
            _popups[i].gameObject.SetActive(false);
        }

        _blocker.gameObject.SetActive(false);
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

    private IEnumerator FirstPopup()
    {
        float maxTime = 8f; // Lika lång tid som intro-cinematic tar
        float startTime = Time.time;

        while (Time.time - startTime < maxTime)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                break;

            yield return null;
        }

        ShowPopup(0);
    }

}

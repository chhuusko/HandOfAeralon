using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPopup : MonoBehaviour
{
    [SerializeField] private TMP_Text[] _pages;
    [SerializeField] private TMP_Text[] _titles;
    [SerializeField] private TMP_Text _counter;
    [SerializeField] private Button _previousButton, _nextButton, _closeButton;
    [SerializeField] private Image _closeButtonBG;
    private int _currentPage = 0;
    private bool _bSeenAll = false;

    void Start()
    {
        foreach (var page in _pages)
        {
            page.text = GameTextFormatter.LabeledDescription(page.text);
        }
    }

    public void ShowPage(int page)
    {
        if (page >= 0 && page < _pages.Length)
        {
            for (int i = 0; i < _pages.Length; i++)
            {
                _pages[i].alpha = 0f;
                _titles[i].alpha = 0f;
            }

            _pages[page].alpha = 1f;
            _titles[page].alpha = 1f;
            _currentPage = page;
            UpdateCounter();
        }
        else
        {
            Debug.LogError($"ShowPage({page}) INDEX OUT OF BOUNDS for {name}.");
        }
    }

    public void NextPage()
    {
        if (_currentPage + 1 < _pages.Length)
        {
            _currentPage++;
            ShowPage(_currentPage);
        }
        else
        {
            _currentPage = _pages.Length - 1;
            ShowPage(_currentPage);
            Debug.LogError($"NextPage({_currentPage + 1}) INDEX OUT OF BOUNDS for {name}.");
        }
    }

    public void PreviousPage()
    {
        if (_currentPage - 1 >= 0)
        {
            _currentPage--;
            ShowPage(_currentPage);
        }
        else
        {
            _currentPage = 0;
            ShowPage(_currentPage);
            Debug.LogError($"PreviousPage({_currentPage - 1}) INDEX OUT OF BOUNDS for {name}.");
        }
    }

    public void Reset()
    {
        _currentPage = 0;
        UpdateCounter();
        ShowPage(_currentPage);
    }

    private void UpdateCounter()
    {
        if (_pages.Length <= 1)
        {
            _bSeenAll = true;
            HideCounter();
            return;
        }

        _counter.text = $"{_currentPage + 1} / {_pages.Length}";

        Color grayedOut = new Color(0.5f, 0.5f, 0.5f, 0.5f);

        if (_currentPage == 0)
        {
            _previousButton.image.color = grayedOut;
            _previousButton.interactable = false;
        }
        else
        {
            _previousButton.image.color = Color.white;
            _previousButton.interactable = true;
        }

        if (_currentPage == _pages.Length - 1)
        {
            _nextButton.image.color = grayedOut;
            _nextButton.interactable = false;
            _bSeenAll = true;
        }
        else
        {
            _nextButton.image.color = Color.white;
            _nextButton.interactable = true;
        }

        _closeButton.interactable = _bSeenAll;
        
        if (_bSeenAll)
        {
            _closeButton.image.color = Color.white;
            _closeButtonBG.color = Color.white;
        }
        else
        {
            
            _closeButton.image.color = grayedOut;
            _closeButtonBG.color = grayedOut;
        }
    }

    private void HideCounter()
    {
        _previousButton.gameObject.SetActive(false);
        _nextButton.gameObject.SetActive(false);
        _counter.alpha = 0f;
    }
}

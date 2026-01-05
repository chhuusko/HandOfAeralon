using TMPro;
using UnityEngine;

public class TutorialPopup : MonoBehaviour
{
    [SerializeField] private TMP_Text[] _pages;
    private int _currentPage = 0;

    public void ShowPage(int page)
    {
        if (page > 0 && page < _pages.Length)
        {
            for (int i = 0; i < _pages.Length; i++)
            {
                _pages[i].alpha = 0f;
            }

            _pages[page].alpha = 1f;
            _currentPage = page;
        }
        else
        {
            Debug.LogError($"ShowPage({page}) INDEX OUT OF BOUNDS for {name}.");
        }
    }

    public void NextPage()
    {
        _currentPage++;
        ShowPage(_currentPage);
    }

    public void PreviousPage()
    {
        _currentPage--;
        ShowPage(_currentPage);
    }

    public void Reset()
    {
        _currentPage = 0;
        ShowPage(_currentPage);
    }
}

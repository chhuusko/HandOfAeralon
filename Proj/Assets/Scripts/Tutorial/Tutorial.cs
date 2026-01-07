using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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

    public static bool SkipTutorial = false;

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

        CombatUI.Instance.OnStartCombatButtonPressed += ShowMovementPointsPopup;
        CombatMenuManager.GetInstance().OnGoToShopButtonPressed += ShowShopPopup;

        HidePopups();
        
        if (_currentPopup == 0)
        {
            StartCoroutine(FirstPopup());
        }
    }

    public void ShowPopup(int popup)
    {
        if (SkipTutorial) return;

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

    public void PopupFinished(TutorialPopup popup)
    {
        HidePopups();

        if (_popups.Contains(popup))
        {
            int index = _popups.AsReadOnlyList().IndexOf(popup);

            switch(index)
            {
                case 0: NextPopup(); break; // Turn Order
                case 1: break; // Deploy Your Party
                case 2: break; // Movement Points
                case 3: break; // Abilities ...
                case 4: NextPopup(); break; // Combat Log
                case 5: NextPopup(); break; // Mana, Cards & Deck
                case 6: break; // Traits & Status
                case 7: break; // The Shop ...
            }
        }
    }

    private IEnumerator FirstPopup()
    {
        float maxTime = 8.5f; // Lika lång tid som intro-cinematic tar
        float startTime = Time.time;

        while (Time.time - startTime < maxTime)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                break;

            yield return null;
        }

        CombatLog combatLog = FindFirstObjectByType<CombatLog>();
        if (combatLog != null)
        {
            combatLog.OnCombatLogUpdate += ShowCombatLogPopup;
        }

        ShowPopup(0);
    }

    private void ShowMovementPointsPopup()
    {
        List<GameObject> characters = CombatGrid._instance.GetAllFriendlyCharacters();
        characters[0].GetComponent<CharacterMovement>().OnCharacterStoppedMoving += ShowAbilitiesPopup;

        CombatUI.Instance.OnStartCombatButtonPressed -= ShowMovementPointsPopup;
        _currentPopup = 2;
        ShowPopup(_currentPopup);
    }

    private void ShowAbilitiesPopup()
    {
        List<GameObject> characters = CombatGrid._instance.GetAllFriendlyCharacters();
        characters[0].GetComponent<CharacterMovement>().OnCharacterStoppedMoving -= ShowAbilitiesPopup;

        _currentPopup = 3;
        ShowPopup(_currentPopup);
    }

    private void ShowCombatLogPopup()
    {
        CombatLog combatLog = FindFirstObjectByType<CombatLog>();
        if (combatLog != null)
        {
            combatLog.OnCombatLogUpdate -= ShowCombatLogPopup;
        }

        _currentPopup = 4;
        ShowPopup(_currentPopup);
    }

    private void ShowShopPopup()
    {
        CombatMenuManager.GetInstance().OnGoToShopButtonPressed -= ShowShopPopup;
        _currentPopup = 7;
        ShowPopup(_currentPopup);
    }
}

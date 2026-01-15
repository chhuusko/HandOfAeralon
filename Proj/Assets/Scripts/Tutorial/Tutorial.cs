using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

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
    [SerializeField] private RawImage _rawImage;
    [SerializeField] private RenderTexture _videoRT;
    [SerializeField] private VideoPlayer _videoPlayer;

    private Canvas _canvas;
    private int _currentPopup = 0;
    private bool _traitsAndStatusReady = false;

    void Start()
    {
        _canvas = GetComponent<Canvas>();

        if (_canvas == null)
        {
            Debug.LogError("Tutorial.cs | Canvas not found!");
        }

        CombatUI.Instance.OnStartCombatButtonPressed += ShowMovementPointsPopup;
        CombatUI.Instance.OnStartCombatButtonPressed += ShowTurnOrderPopup;
        CombatMenuManager.GetInstance().OnGoToShopButtonPressed += ShowShopPopup;
        Selector._instance.OnSelectPlacementCharacterFromTile += ShowDeployPopup;
        Selector._instance.OnPreviewAbilityRange += ShowAbilitiesPopup;
        CardHandManager.onHover += ShowCardsPopup;

        HidePopups();
        
        if (_currentPopup == 0)
        {
            StartCoroutine(FirstPopup());
        }
    }

    public IEnumerator ShowPopupDelayed(int popup)
    {
        yield return new WaitForSeconds(1f);
        ShowPopup(popup);

        if (popup == 6) // Combat Log
        {
            _traitsAndStatusReady = true;
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

            CombatCamera[] camera = FindObjectsByType<CombatCamera>(FindObjectsSortMode.None);
            if (camera.Any() && camera[0] != null)
            {
                camera[0].FreezeCamera = true;
            }
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

        _rawImage.enabled = false;
        _blocker.gameObject.SetActive(false);

        CombatCamera[] camera = FindObjectsByType<CombatCamera>(FindObjectsSortMode.None);
        if (camera.Any() && camera[0] != null)
        {
            camera[0].FreezeCamera = false;
        }
    }

    public void NextPopup()
    {
        _currentPopup++;
        ShowPopup(_currentPopup);
    }

    public void NextPopupDelayed()
    {
        _currentPopup++;
        StartCoroutine(ShowPopupDelayed(_currentPopup));
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
                case 0: NextPopup(); break; // Introduction
                case 1: break; // Camera
                case 2: break; // Turn Order
                case 3: break; // Deploy Your Party
                case 4: break; // Movement Points
                case 5: break; // Abilities ...
                case 6: break; // Combat Log
                case 7: break; // Mana, Cards & Deck
                case 8: break; // Traits & Status
                case 9: break; // The Shop ...
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

        Selector._instance.DeselectCharacter();

        CombatLog combatLog = FindFirstObjectByType<CombatLog>();
        if (combatLog != null)
        {
            combatLog.OnCombatLogUpdate += ShowCombatLogPopup;
        }

        StartCoroutine(ShowPopupDelayed(0));
    }

    private void ShowTurnOrderPopup()
    {
        if (GlobalGameManager.GetInstance().GetTotalBattlesWon() > 0)
        {
            CombatEventManager.OnCharacterPlaced -= ShowTurnOrderPopup;
            _currentPopup = 2;
            StartCoroutine(ShowPopupDelayed(_currentPopup));
        }
    }

    private void ShowDeployPopup()
    {
        Selector._instance.OnSelectPlacementCharacterFromTile -= ShowDeployPopup;
        _currentPopup = 3;
        StartCoroutine(ShowPopupDelayed(_currentPopup));
    }

    private void ShowMovementPointsPopup()
    {
        CombatUI.Instance.OnStartCombatButtonPressed -= ShowMovementPointsPopup;
        _currentPopup = 4;
        StartCoroutine(ShowPopupDelayed(_currentPopup));
    }

    private void ShowAbilitiesPopup()
    {
        Selector._instance.OnPreviewAbilityRange -= ShowAbilitiesPopup;
        _currentPopup = 5;
        ShowPopup(_currentPopup);
    }

    public void ShowTraitsAndStatusPopup()
    {
        if (!_traitsAndStatusReady) return;

        _traitsAndStatusReady = false;
        _currentPopup = 8;
        StartCoroutine(ShowPopupDelayed(_currentPopup));
    }

    private void ShowCombatLogPopup()
    {
        CombatLog combatLog = FindFirstObjectByType<CombatLog>();
        if (combatLog != null)
        {
            combatLog.OnCombatLogUpdate -= ShowCombatLogPopup;
        }

        _currentPopup = 6;
        StartCoroutine(ShowPopupDelayed(_currentPopup));
    }

    private void ShowCardsPopup(bool b)
    {
        CardHandManager.onHover -= ShowCardsPopup;
        _currentPopup = 7;
        StartCoroutine(ShowPopupDelayed(_currentPopup));
    }

    private void ShowShopPopup()
    {
        CombatMenuManager.GetInstance().OnGoToShopButtonPressed -= ShowShopPopup;
        _currentPopup = 9;
        StartCoroutine(ShowPopupDelayed(_currentPopup));
    }

    public void PlayVideo(VideoClip clip)
    {
        if (clip == null) return;

        _videoPlayer.clip = clip;

        _videoPlayer.Stop();
        _rawImage.enabled = true;
        _videoPlayer.Prepare();
        _videoPlayer.prepareCompleted += PlayPreparedVideo;
    }

    private void PlayPreparedVideo(VideoPlayer vp)
    {
        vp.prepareCompleted -= PlayPreparedVideo;
        vp.Play();
    }
}

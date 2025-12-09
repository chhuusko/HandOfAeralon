using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class CombatMenuManager : MonoBehaviour
{
    private static CombatMenuManager _instance;

    // NOTE (Calle): The globalVolume for PP-effects to enable when this menu activates.
    [SerializeField] private Volume _globalVolume;
    
    // NOTE (Calle): This Menu Canvas
    [SerializeField] private Canvas _combatMenuCanvas;
    [SerializeField] private GameObject _inGameLayout;
    [SerializeField] private GameObject _victoryScreenLayout;

    // NOTE (Calle): Canvases to turn off Interactable on when this menu opens.
    [SerializeField] private CanvasGroup _combatHUDCanvasGroup;
    [SerializeField] private CanvasGroup _combatCardCanvasGroup;
    [SerializeField] private CanvasGroup _combatTooltipCanvasGroup;
    private Animator _endCombatMenuAnimator;

    public event Action OnGoToShopButtonPressed;

    private void Awake()
    {
        if( _instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }

    void Start()
    {
        _combatMenuCanvas.enabled = false;
        _endCombatMenuAnimator = _globalVolume.GetComponent<Animator>();
        if (_endCombatMenuAnimator == null)
            DebugLog.CJLogError("GlobalVolume has no Animator Comonent!");

        CombatEventManager.OnEnterCombatStateEndCombat += ShowEndCombatMenuScreen;
    }

    public static CombatMenuManager GetInstance() { return _instance; }
    private void OnDisable()
    {
        CombatEventManager.OnEnterCombatStateEndCombat -= ShowEndCombatMenuScreen;
    }

    private void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Escape))
        //{
        //    if (_endCombatMenuCanvas.enabled)
        //        HideEndCombatMenuScreen();
        //    else
        //        ShowEndCombatMenuScreen();
        //}
    }

    public void InvokeEndCombatButtonPressed()
    {
        HideEndCombatMenuScreen();
        OnGoToShopButtonPressed?.Invoke();
    }

    public void ShowEndCombatMenuScreen(bool playerWon)
    {
        ShowVictroyScreenLayout();
        HideInGameLayout();
        _combatMenuCanvas.enabled              = true;
        _combatHUDCanvasGroup.interactable     = false;
        _combatTooltipCanvasGroup.interactable = false;
        _combatCardCanvasGroup.interactable    = false;

        _endCombatMenuAnimator.Play("WeightFadeIn");

        Time.timeScale = 0f;

    }

    public void HideEndCombatMenuScreen()
    {
        _combatMenuCanvas.enabled           = false;
        _combatHUDCanvasGroup.interactable     = true;
        _combatTooltipCanvasGroup.interactable = true;
        _combatCardCanvasGroup.interactable    = true;

        _endCombatMenuAnimator.Play("WeightFadeOut");

        Time.timeScale = 1f;
    }

    public void ShowInGameLayout()
    {
        _inGameLayout.SetActive(true);
    }
    public void HideInGameLayout()
    {
        _inGameLayout.SetActive(false);
    }
    public void ShowVictroyScreenLayout()
    {
        _victoryScreenLayout.SetActive(true);
    }
    public void HideVictroyScreenLayout()
    {
        _victoryScreenLayout.SetActive(false);
    }

}

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
    [SerializeField] private GameObject _optionsLayout;
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

        CombatEventManager.OnEnterCombatStateEndCombat += OpenVictoryMenuScreen;
    }

    public static CombatMenuManager GetInstance() { return _instance; }
    private void OnDisable()
    {
        CombatEventManager.OnEnterCombatStateEndCombat -= OpenVictoryMenuScreen;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (_combatMenuCanvas.enabled)
            {
                if(_optionsLayout.activeSelf)
                {
                    CloseOptionsMenu();
                }
                else
                {
                    CloseInGameMenu();
                }
                    
            }
            else
            {
                OpenInGameMenu();
            }
        }

        //DEBUGLogRayCastHits();
    }

    public void InvokeEndCombatButtonPressed()
    {
        Debug.Log("GO TO SHOP");
        CloseVictoryMenuScreen();
        OnGoToShopButtonPressed?.Invoke();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenInGameMenu()
    {
        _combatMenuCanvas.enabled = true;
        
        ShowInGameLayout();
        TurnOFFCombatCanvases();
        _endCombatMenuAnimator.Play("WeightFadeIn");
    }

    public void CloseInGameMenu()
    {
        _combatMenuCanvas.enabled = false;
        TurnONCombatCanvases();
        _endCombatMenuAnimator.Play("WeightFadeOut");
    }

    public void OpenOptionsMenu()
    {
        ShowOptionsLayout();
    }

    public void CloseOptionsMenu()
    {
        ShowInGameLayout();
    }

    public void OpenVictoryMenuScreen(bool playerWon)
    {
        _combatMenuCanvas.enabled = true;

        ShowVictroyScreenLayout();
        TurnOFFCombatCanvases();

        _endCombatMenuAnimator.Play("WeightFadeIn");  
    }

    public void CloseVictoryMenuScreen()
    {
        _combatMenuCanvas.enabled = false;

        TurnONCombatCanvases();

        _endCombatMenuAnimator.Play("WeightFadeOut");  
    }

    private void ShowInGameLayout()
    {
        HideVictroyScreenLayout();
        HideOptionsLayout();
        _inGameLayout.SetActive(true);
    }
    private void ShowOptionsLayout()
    {
        HideVictroyScreenLayout();
        HideInGameLayout();
        _optionsLayout.SetActive(true);
    }
    private void ShowVictroyScreenLayout()
    {
        HideInGameLayout();
        HideOptionsLayout();
        _victoryScreenLayout.SetActive(true);
    }

    private void HideInGameLayout()
    {
        _inGameLayout.SetActive(false);
    }

    private void HideOptionsLayout()
    {
        _optionsLayout.SetActive(false);
    }

    private void HideVictroyScreenLayout()
    {
        _victoryScreenLayout.SetActive(false);
    }

    private void TurnOFFCombatCanvases()
    {
        _combatHUDCanvasGroup.interactable     = false;
        _combatTooltipCanvasGroup.interactable = false;
        _combatCardCanvasGroup.interactable    = false;
        Time.timeScale = 0f;
    }

    private void TurnONCombatCanvases()
    {
        _combatHUDCanvasGroup.interactable     = true;
        _combatTooltipCanvasGroup.interactable = true;
        _combatCardCanvasGroup.interactable    = true;
        Time.timeScale = 1f;
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

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class CombatMenuManager : MonoBehaviour
{
    private static CombatMenuManager _instance;

    [SerializeField] private Volume _globalVolume;
    [SerializeField] private Canvas _endCombatMenuCanvas;
    [SerializeField] private CanvasGroup _combatHUDCanvasGroup;
    [SerializeField] private CanvasGroup _combatCardCanvasGroup;
    [SerializeField] private CanvasGroup _combatTooltipCanvasGroup;
    private Animator _endCombatMenuAnimator;

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
        _endCombatMenuCanvas.enabled = false;
        _endCombatMenuAnimator = _globalVolume.GetComponent<Animator>();
        if (_endCombatMenuAnimator == null)
            DebugLog.CJLogError("GlobalVolume has no Animator Comonent!");
        

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (_endCombatMenuCanvas.enabled)
                HideEndCombatMenuScreen();
            else
                ShowEndCombatMenuScreen();
        }
    }

    public void ShowEndCombatMenuScreen()
    {
        _endCombatMenuCanvas.enabled           = true;
        _combatHUDCanvasGroup.interactable     = false;
        _combatTooltipCanvasGroup.interactable = false;
        _combatCardCanvasGroup.interactable    = false;

        _endCombatMenuAnimator.Play("WeightFadeIn");

        Time.timeScale = 0f;

    }
    public void HideEndCombatMenuScreen()
    {
        _endCombatMenuCanvas.enabled           = false;
        _combatHUDCanvasGroup.interactable     = true;
        _combatTooltipCanvasGroup.interactable = true;
        _combatCardCanvasGroup.interactable    = true;

        _endCombatMenuAnimator.Play("WeightFadeOut");

        Time.timeScale = 1f;
    }


}

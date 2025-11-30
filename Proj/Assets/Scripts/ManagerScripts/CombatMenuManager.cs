using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class CombatMenuManager : MonoBehaviour
{
    [SerializeField] private Volume _globalVolume;
    [SerializeField] private Canvas _endCombatMenuCanvas;
    [SerializeField] private CanvasGroup _combatCanvasGroup;
    [SerializeField] private Animator _endCombatMenuAnimator;


    void Start()
    {
        _endCombatMenuCanvas.enabled = false;    
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
        _endCombatMenuCanvas.enabled = true;
        _combatCanvasGroup.interactable = false;
        _endCombatMenuAnimator.Play("WeightFadeIn");
        Time.timeScale = 0f;
    }
    public void HideEndCombatMenuScreen()
    {
        _endCombatMenuCanvas.enabled = false;
        _combatCanvasGroup.interactable = true;
        _endCombatMenuAnimator.Play("WeightFadeOut");
        Time.timeScale = 1f;
    }


}

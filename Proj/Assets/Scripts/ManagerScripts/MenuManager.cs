using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class MenuManager : MonoBehaviour
{
    [SerializeField] private Volume _globalVolume;
    [SerializeField] private Canvas _endCombatMenuCanvas;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _endCombatMenuCanvas.enabled = false;    
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
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
        _globalVolume.weight = 1.0f;
        Time.timeScale = 0f;
    }
    public void HideEndCombatMenuScreen()
    {
        _endCombatMenuCanvas.enabled = false;
        _globalVolume.weight = 0.0f;
        Time.timeScale = 1f;
    }


}

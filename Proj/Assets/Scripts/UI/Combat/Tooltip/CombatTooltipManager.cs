using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class CombatTooltipManager : MonoBehaviour
{
    private static CombatTooltipManager _instance;

    [SerializeField] private CombatTooltipCharacterLayout _characterLayout;


    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }

    void Start()
    {
        _characterLayout.InitializeCharacterStats();
    }

    private void Update()
    {

    }

    public static CombatTooltipManager GetInstance() { return _instance; }

    public CombatTooltipCharacterLayout GetCharacterLayout() { return _characterLayout; }

    public void HideTooltipCanvas()
    {
        if(_characterLayout != null)
        {
            _characterLayout.HideCanvas();
        }
    }

    public void ShowTooltipCanvas()
    {
        if (_characterLayout != null)
        {
            _characterLayout.ShowCanvas();
        }
    }
}

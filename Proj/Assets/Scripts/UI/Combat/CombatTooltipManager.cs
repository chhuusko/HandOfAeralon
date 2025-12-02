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
        Selector s = Selector._instance;
    }  
}

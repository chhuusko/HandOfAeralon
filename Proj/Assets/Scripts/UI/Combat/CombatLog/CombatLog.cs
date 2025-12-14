using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatLog : MonoBehaviour
{
    [SerializeField] private GameObject _combatLogEntryPrefab;
    [SerializeField] private GameObject _combatLogPanel;
    [SerializeField] private GameObject _combatLogScrollbar;
    [SerializeField] private GameObject _combatLogButton;
    [SerializeField] private Transform _combatLogContent;
    [SerializeField] private ScrollRect _combatLogScrollRect;
    
    [Header("Prefabs")]
    [SerializeField] private CombatLogEntry _abilityEntryPrefab;
    [SerializeField] private CombatLogEntry _characterDeathEntryPrefab;
    [SerializeField] private CombatLogEntry _cardLogEntryPrefab;
    [SerializeField] private CombatLogEntry _combatBountyEntryPrefab;
    
    private bool _bCombatLogEnabled = true;
    
    private void OnEnable()
    {
        CombatEventManager.OnAbilityDataCreated += AddCombatLogEntry;
    }

    public void SetCombatLogActive()
    {
        Debug.Log("SetCombatLogActive");
        _bCombatLogEnabled = !_bCombatLogEnabled;
        _combatLogPanel.SetActive(_bCombatLogEnabled);
        _combatLogScrollbar.SetActive(_bCombatLogEnabled);
    }

    private void AddCombatLogEntry(AbilityExecutionData data)
    {
        AbilityLogData abilityLogData = new AbilityLogData
        {
            // ExecutionData = data
            Caster = data.Caster,
            Target = data.Target,
            Ability = data.Ability,
            Damage = data.Damage,
        };
        AddCombatLogEntry(abilityLogData);
    }

    public void AddCombatLogEntry(StatusEffect effect)
    {
        
    }

    public void AddCombatLogEntry(Card card)
    {
        
    }

    private void AddCombatLogEntry(CombatLogData data)
    {
        CombatLogEntry prefab = data switch
        {
            AbilityLogData => _abilityEntryPrefab,
            CharacterDeathLogData => _characterDeathEntryPrefab,
            CardLogData => _cardLogEntryPrefab,
            CombatBountyLogData => _combatBountyEntryPrefab,
            _ => null
        };

        if (!prefab)
        {
            DebugLog.JoppaLog("No prefab found");
            return;
        }
        
        var entry = Instantiate(prefab, _combatLogContent);
        entry.Initialize(data);
        
        StartCoroutine(ScrollToBottom());
    }
    
    private IEnumerator ScrollToBottom()
    {
        yield return null;
        
        // Set scroll to bottom.
        _combatLogScrollRect.verticalNormalizedPosition = 0;
    }
    
    private void OnDisable()
    {
        CombatEventManager.OnAbilityDataCreated -= AddCombatLogEntry;
    }
}

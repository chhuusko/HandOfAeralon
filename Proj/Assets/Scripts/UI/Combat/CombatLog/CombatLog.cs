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
    [SerializeField] private CombatLogEntry _cardUsedLogEntryPrefab;
    [SerializeField] private CombatLogEntry _cardTargetedLogEntryPrefab;
    [SerializeField] private CombatLogEntry _combatBountyEntryPrefab;
    
    private bool _bCombatLogEnabled = true;
    
    private void OnEnable()
    {
        CombatEventManager.OnAbilityDataCreated += AddCombatLogEntry;
        CardHandManager.onCardUse += AddCombatLogEntry;
        CardHandManager.onCardTargetCharacter += AddCombatLogEntry;
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
            Heal = data.Heal,
        };
        AddCombatLogEntry(abilityLogData);
    }

    private void AddCombatLogEntry(Card card)
    {
        CardUsedLogData cardUsedLogData = new CardUsedLogData()
        {
            Card = card
        };
        AddCombatLogEntry(cardUsedLogData);
    }

    private void AddCombatLogEntry(Character character, Card card)
    {
        
    }
    
    private void AddCombatLogEntry(StatusEffect effect)
    {
        
    }

    private void AddCombatLogEntry(CombatLogData data)
    {
        CombatLogEntry prefab = data switch
        {
            AbilityLogData => _abilityEntryPrefab,
            CharacterDeathLogData => _characterDeathEntryPrefab,
            CardUsedLogData => _cardUsedLogEntryPrefab,
            CardTargetedLogData => _cardTargetedLogEntryPrefab,
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
        CardHandManager.onCardUse -= AddCombatLogEntry;
    }
}

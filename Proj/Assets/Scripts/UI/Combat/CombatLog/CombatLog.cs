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
    [SerializeField] private Transform _combatLogViewPort;
    [SerializeField] private ScrollRect _combatLogScrollRect;
    
    private bool _bCombatLogEnabled;
    private List<AbilityExecutionData> _combatLogEntries = new();
    
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
        _combatLogEntries.Add(data);
        var go = Instantiate(_combatLogEntryPrefab, _combatLogViewPort);
        
        go.transform.Find("Icon").GetComponent<Image>().sprite = data.Ability.GetIcon();

        if (!data.Ability || !data.Target || !data.Caster)
        {
            return;
        }

        string text;
        
        // Check for type of ability.
        if (data.Ability.GetAbilityType() is Ability.Type.Elemental or Ability.Type.Physical)
        {
            text = $"{data.Caster.Data.ClassData.name} used {data.Ability.GetAbilityName()} and dealt " +
                   $"{data.Damage} damage to{(data.Target.GetFaction() == Faction.Friendly ? " " : " enemy")}" +
                   $" {data.Target.Data.ClassData.name}";
        }
        else
        {
            text = $"{data.Caster.Data.ClassData.name} used {data.Ability.GetAbilityName()}" +
                   $" on{(data.Target.GetFaction() == Faction.Friendly ? " " : " enemy")} " +
                   $"{data.Target.Data.ClassData.name}";
        }
        
        go.transform.Find("Text").GetComponent<TMP_Text>().text = text;
        
        StartCoroutine(ScrollToTop());
    }
    
    private IEnumerator ScrollToTop()
    {
        yield return null;
        
        // Set scroll to bottom.
        _combatLogScrollRect.verticalNormalizedPosition = 1;
    }
    
    private void OnDisable()
    {
        CombatEventManager.OnAbilityDataCreated -= AddCombatLogEntry;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CombatLog : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _combatLogPanel;
    [SerializeField] private GameObject _combatLogScrollbar;
    [SerializeField] private GameObject _combatLogButton;
    [SerializeField] private Transform _combatLogContent;
    [SerializeField] private ScrollRect _combatLogScrollRect;
    
    [Header("Prefabs")]
    [SerializeField] private GameObject _combatLogEntryPrefab;
    [SerializeField] private CombatLogEntry _abilityEntryPrefab;
    [SerializeField] private CombatLogEntry _characterDeathEntryPrefab;
    [SerializeField] private CombatLogEntry _cardUsedLogEntryPrefab;
    [SerializeField] private CombatLogEntry _cardTargetedLogEntryPrefab;
    [SerializeField] private CombatLogEntry _combatBountyEntryPrefab;
    [SerializeField] private CombatLogEntry _statusEffectEntryPrefab;
    
    private bool _bCombatLogEnabled = true;
    
    private void OnEnable()
    {
        CombatEventManager.OnAbilityDataCreated += AddCombatLogEntry;
        CombatEventManager.OnStatusEffectAppliedToCharacter += AddCombatLogEntry;
        CombatEventManager.OnCharacterDeath += AddCombatLogEntry;
        CardHandManager.onCardUse += AddCombatLogEntry;
        CardHandManager.onCardTargetCharacter += AddCombatLogEntry;
        LinkHandlerForTMPText.OnClickOnLink += SelectCharacter;
    }

    public void SetCombatLogActive()
    {
        _bCombatLogEnabled = !_bCombatLogEnabled;
        _combatLogPanel.SetActive(_bCombatLogEnabled);
        _combatLogScrollbar.SetActive(_bCombatLogEnabled);
    }

    private void AddCombatLogEntry(AbilityExecutionData data)
    {
        AbilityLogData abilityLogData = new AbilityLogData
        {
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
        // CardTargetedLogData cardTargetedLogData = new CardTargetedLogData()
        // {
        //     Card = card,
        //     Target = character
        // };
        // AddCombatLogEntry(cardTargetedLogData);
    }
    
    private void AddCombatLogEntry(Character character)
    {
        StartCoroutine(AddCharacterDeathNextFrame(character));
    }

    private IEnumerator AddCharacterDeathNextFrame(Character character)
    {
        yield return null;
        
        CharacterDeathLogData characterDeathLogData = new CharacterDeathLogData()
        {
            Character = character
        };
        AddCombatLogEntry(characterDeathLogData);
    }
    
    private void AddCombatLogEntry(Character caster, Character target, StatusEffect effect)
    {
        StartCoroutine(AddStatusEffectNextFrame(caster, target, effect));
    }

    private IEnumerator AddStatusEffectNextFrame(Character caster, Character target, StatusEffect effect)
    {
        // Wait one frame. Guarantees ability log is added before status effects.
        yield return null;
        
        StatusEffectLogData statusEffectLogData = new StatusEffectLogData()
        {
            StatusEffect = effect,
            Caster = caster,
            Target = target,
        };
        AddCombatLogEntry(statusEffectLogData);
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
            StatusEffectLogData => _statusEffectEntryPrefab,
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

    private void SelectCharacter(string keyword)
    {
        int id = int.Parse(keyword);
        foreach (var character in CombatGrid._instance.GetAllCharacterScripts())
        {
            if (character.CharacterID == id)
            {
                Selector._instance.SelectCharacterFromUI(character);
            }
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        CombatEventManager.InvokeOnIsHoveringUI(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CombatEventManager.InvokeOnIsHoveringUI(false);
    }
    
    private void OnDisable()
    {
        CombatEventManager.OnAbilityDataCreated -= AddCombatLogEntry;
        CombatEventManager.OnStatusEffectAppliedToCharacter -= AddCombatLogEntry;
        CombatEventManager.OnCharacterDeath -= AddCombatLogEntry;
        CardHandManager.onCardUse -= AddCombatLogEntry;
        CardHandManager.onCardTargetCharacter -= AddCombatLogEntry;
        LinkHandlerForTMPText.OnClickOnLink -= SelectCharacter;
    }
}

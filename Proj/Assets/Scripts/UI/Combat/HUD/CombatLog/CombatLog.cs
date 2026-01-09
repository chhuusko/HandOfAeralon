using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CombatLog : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public event Action OnCombatLogUpdate;

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
    [SerializeField] private CombatLogEntry _statusEffectAddedEntryPrefab;
    [SerializeField] private CombatLogEntry _statusEffectRemovedEntryPrefab;
    [SerializeField] private CombatLogEntry _statusEffectDamageEntryPrefab;
    
    private bool _bCombatLogEnabled = true;
    
    private void OnEnable()
    {
        CombatEventManager.OnAbilityDataCreated += AddAbilityEntry;
        CombatEventManager.OnStatusEffectAppliedToCharacter += AddStatusEffectAppliedEntry;
        CombatEventManager.OnStatusEffectExpiredOnCharacter += AddStatusEffectRemovedEntry;
        CombatEventManager.OnStatusEffectDamageDealt += AddStatusEffectDamageEntry;
        CombatEventManager.OnCharacterDeath += AddCharacterDeathEntry;
        CardHandManager.onCardUse += AddCardUsedEntry;
        CardHandManager.onCardTargetCharacter += AddCardTargetedEntry;
        LinkHandlerForTMPText.OnClickOnLink += SelectCharacter;
    }

    /// <summary>
    /// Cycles the combat log being active/inactive.
    /// </summary>
    public void SetCombatLogActive()
    {
        _bCombatLogEnabled = !_bCombatLogEnabled;
        _combatLogPanel.SetActive(_bCombatLogEnabled);
        _combatLogScrollbar.SetActive(_bCombatLogEnabled);
    }
    
    private void AddAbilityEntry(AbilityExecutionData data)
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

    private void AddCardUsedEntry(Card card)
    {
        CardUsedLogData cardUsedLogData = new CardUsedLogData()
        {
            Card = card
        };
        AddCombatLogEntry(cardUsedLogData);
    }

    private void AddCardTargetedEntry(Character character, Card card)
    {
        // CardTargetedLogData cardTargetedLogData = new CardTargetedLogData()
        // {
        //     Card = card,
        //     Target = character
        // };
        // AddCombatLogEntry(cardTargetedLogData);
    }
    
    private void AddCharacterDeathEntry(Character character)
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
    
    private void AddStatusEffectAppliedEntry(Character caster, Character target, StatusEffect effect)
    {
        StartCoroutine(AddStatusEffectNextFrame(caster, target, effect));
    }
    
    private IEnumerator AddStatusEffectNextFrame(Character caster, Character target, StatusEffect effect)
    {
        // Wait one frame. Guarantees ability log is added before status effects.
        yield return null;
        
        StatusEffectAddedLogData statusEffectAddedLogData = new StatusEffectAddedLogData()
        {
            StatusEffect = effect,
            Caster = caster,
            Target = target,
        };
        AddCombatLogEntry(statusEffectAddedLogData);
    }

    private void AddStatusEffectRemovedEntry(Character character, StatusEffect effect)
    {
        StartCoroutine(RemoveStatusEffectNextFrame(character, effect));
    }

    private IEnumerator RemoveStatusEffectNextFrame(Character character, StatusEffect effect)
    {
        yield return null;
        
        StatusEffectRemovedLogData statusEffectRemovedLogData = new StatusEffectRemovedLogData()
        {
            StatusEffect = effect,
            Character = character,
        };
        AddCombatLogEntry(statusEffectRemovedLogData);
    }

    private void AddStatusEffectDamageEntry(Character character, StatusEffect effect, int damage)
    {
        StatusEffectDamageLogData statusEffectDamageLogData = new StatusEffectDamageLogData()
        {
            Character = character,
            StatusEffect = effect,
            Damage = damage
        };
        AddCombatLogEntry(statusEffectDamageLogData);
    }

    private void AddCombatLogEntry(CombatLogData data)
    {
        // Get the prefab to instantiate.
        CombatLogEntry prefab = data switch
        {
            AbilityLogData => _abilityEntryPrefab,
            CharacterDeathLogData => _characterDeathEntryPrefab,
            CardUsedLogData => _cardUsedLogEntryPrefab,
            CardTargetedLogData => _cardTargetedLogEntryPrefab,
            CombatBountyLogData => _combatBountyEntryPrefab,
            StatusEffectAddedLogData => _statusEffectAddedEntryPrefab,
            StatusEffectRemovedLogData => _statusEffectRemovedEntryPrefab,
            StatusEffectDamageLogData => _statusEffectDamageEntryPrefab,
            _ => null
        };

        if (!prefab)
        {
            DebugLog.JoppaLog("No prefab found");
            return;
        }
        
        // Create the combat log entry.
        var entry = Instantiate(prefab, _combatLogContent);
        entry.Initialize(data);

        OnCombatLogUpdate?.Invoke();

        // Reset scroll.
        StartCoroutine(ScrollToBottom());
    }
    
    private IEnumerator ScrollToBottom()
    {
        yield return null;
        
        // Set scroll to bottom.
        _combatLogScrollRect.verticalNormalizedPosition = 0;
    }

    /// <summary>
    /// Selects the corresponding character based on its ID.
    /// </summary>
    /// <param name="linkID">The ID of the character to select.</param>
    private void SelectCharacter(string linkID)
    {
        int id = int.Parse(linkID);
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
        CombatEventManager.OnAbilityDataCreated -= AddAbilityEntry;
        CombatEventManager.OnStatusEffectAppliedToCharacter -= AddStatusEffectAppliedEntry;
        CombatEventManager.OnStatusEffectExpiredOnCharacter -= AddStatusEffectRemovedEntry;
        CombatEventManager.OnStatusEffectDamageDealt -= AddStatusEffectDamageEntry;
        CombatEventManager.OnCharacterDeath -= AddCharacterDeathEntry;
        CardHandManager.onCardUse -= AddCardUsedEntry;
        CardHandManager.onCardTargetCharacter -= AddCardTargetedEntry;
        LinkHandlerForTMPText.OnClickOnLink -= SelectCharacter;
    }
}

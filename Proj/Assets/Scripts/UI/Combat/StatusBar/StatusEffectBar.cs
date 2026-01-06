using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectBar : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject _prefabStatusEffectBarElement;
    [SerializeField] private List<StatusEffectBarElement> _statusEffectBarElements;

    private Character _ownerCharacter;

    void Start()
    {
        CombatEventManager.OnStatusEffectAppliedToCharacter += UpdateStatusEffectsBar;
        CombatEventManager.OnStatusEffectExpiredOnCharacter += RemoveStatusEffectBarElement;
    }

    private void OnDisable()
    {
        CombatEventManager.OnStatusEffectAppliedToCharacter -= UpdateStatusEffectsBar;
        CombatEventManager.OnStatusEffectExpiredOnCharacter -= RemoveStatusEffectBarElement;
    }

    private void UpdateStatusEffectsBar(Character caster, Character characterSubject, StatusEffect statusEffect)
    {
        if (characterSubject != _ownerCharacter)
            return;

        // NOTE (Calle): Don't add another status effect icon if it already exists.
        foreach (StatusEffectBarElement existingStatusEffect in _statusEffectBarElements)
        {
            if (existingStatusEffect.name.Equals(statusEffect.Data.name))
                return;
        }

        GameObject statusEffectBarElementObject = Instantiate(_prefabStatusEffectBarElement, transform);
        statusEffectBarElementObject.name = statusEffect.Data.name;

        StatusEffectBarElement statusEffectBarElement = statusEffectBarElementObject.GetComponent<StatusEffectBarElement>();
        
        statusEffectBarElement.SetSprite(statusEffect.Data.Icon);
        statusEffectBarElement.SetTitle(statusEffect.Data.name);
        statusEffectBarElement.SetDescription(GameTextFormatter.LabeledDescription(statusEffect.Data.Description));
        _statusEffectBarElements.Add(statusEffectBarElement);
     
     
    }

    private void RemoveStatusEffectBarElement(Character characterSubject, StatusEffect status)
    {
        if (characterSubject != _ownerCharacter)
            return;

        Debug.Log("Removing status effect bar element");
        
        foreach (StatusEffectBarElement statusEffect in _statusEffectBarElements)
        {
            if(statusEffect.name.Equals(status.Data.name))
            {
                _statusEffectBarElements.Remove(statusEffect);
                if(statusEffect.gameObject != null)
                    Destroy(statusEffect.gameObject);
                break;
            }
        }
    }

    public void Bind(Character character)   
    {
        _ownerCharacter = character;
    }

}

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatUI : MonoBehaviour
{
    public enum PanelType { Card, Ability }
    
    [SerializeField] private Image _abilityPanel;
    [SerializeField] private Button _abilityButtonPrefab;
    [SerializeField] private TextMeshProUGUI _mana;

    private void OnEnable()
    {
        CardHandManager.onManaChange += UpdateManaText;
    }

    private void OnDisable()
    {
        CardHandManager.onManaChange -= UpdateManaText;
    }
    
    private void UpdateManaText(int mana)
    {
        _mana.text = $"Mana\n{mana}/10";
    }

    public void ShowDeck()
    {
        CardHandManager._instance.OpenDeck();
    }

    public void ShowDiscardPile()
    {
        CardHandManager._instance.OpenDiscardPile();
    }

    public void SetCardsActive(bool active)
    {
        CardHandManager._instance.SetUIActive(active);
        _abilityPanel.color = active ? new Color(1, 1, 1, 0.5f) : new Color(1, 1, 1, 1);
    }

    public void LoadAbilities(Character character)
    {
        Debug.Log("Loading Abilities");

        if (character == null)
        {
            DebugLog.JoppaLog("No selected character");
            return;
        }
        
        DebugLog.JoppaLog($"Number of abilities: {character.GetAvailableAbilities().Count}");

        for (int i = 0; i < character.GetAvailableAbilities().Count; i++)
        {
            Button abilityButton = Instantiate(_abilityButtonPrefab, _abilityPanel.transform);
            
            var ability = character.GetAvailableAbilities()[i];
            abilityButton.GetComponentInChildren<TextMeshProUGUI>().text = ability.name;
            abilityButton.image.sprite = ability.GetIcon();
        }
    }
}

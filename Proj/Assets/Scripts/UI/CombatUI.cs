using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatUI : MonoBehaviour
{
    public enum PanelType { Card, Ability }
    
    [SerializeField] private Image _abilityPanel;
    [SerializeField] private GameObject _abilityButtonPrefab;
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

    private void LoadAbilities()
    {
        var selectedCharacter = Selector._instance.GetSelectedCharacter();

        if (selectedCharacter == null)
        {
            return;
        }
        
        foreach (var ability in selectedCharacter.GetAvailableAbilities())
        {
            GameObject abilityButton = Instantiate(_abilityButtonPrefab, _abilityPanel.transform);
        }
    }
}

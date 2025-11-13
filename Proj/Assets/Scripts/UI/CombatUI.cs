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

    // private void Start()
    // {
    //     _abilityPanel.SetActive(false);
    // }

    public void ShowDeck()
    {
        CardHandManager._instance.OpenDeck();
    }

    public void ShowDiscardPile()
    {
        CardHandManager._instance.OpenDiscardPile();
    }
    
    public void ShowCardPanel()
    {
        ShowPanel(PanelType.Card);
    }

    public void ShowAbilityPanel()
    {
        ShowPanel(PanelType.Ability);
        LoadAbilities();
    }

    private void ShowPanel(PanelType panelType)
    {
        CardHandManager._instance.SetUIActive(panelType == PanelType.Card);
        _abilityPanel.color = panelType == PanelType.Ability ? new Color(1, 1, 1, 1) :  new Color(1, 1, 1, 0.5f);
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

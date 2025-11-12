using UnityEngine;

public class CombatUI : MonoBehaviour
{
    public enum PanelType { Card, Ability }
    
    [SerializeField] private GameObject AbilityPanel;

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
        AbilityPanel.SetActive(panelType == PanelType.Ability);
    }

    private void LoadAbilities()
    {
        
    }
}

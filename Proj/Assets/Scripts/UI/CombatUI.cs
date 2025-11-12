using UnityEngine;

public class CombatUI : MonoBehaviour
{
    public enum PanelType { Card, Ability }
    
    [SerializeField] private GameObject CardPanel;
    [SerializeField] private GameObject AbilityPanel;

    public void ShowCardPanel()
    {
        ShowPanel(PanelType.Card);
    }

    public void ShowAbilityPanel()
    {
        ShowPanel(PanelType.Ability);
    }

    private void ShowPanel(PanelType panelType)
    {
        CardPanel.SetActive(panelType == PanelType.Card);
        AbilityPanel.SetActive(panelType == PanelType.Ability);
    }
}

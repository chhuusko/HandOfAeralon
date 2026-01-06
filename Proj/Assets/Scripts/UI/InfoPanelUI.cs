using TMPro;
using UnityEngine;

public class InfoPanelUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] TextMeshProUGUI title, description;
    public void SetUpUIElements(InfoPanel infoPanel)
    {
        transform.SetAsLastSibling();
        if (infoPanel.status == null)
        {
            
            title.text = GameTextFormatter.LabeledDescription(infoPanel.title);
            description.text = GameTextFormatter.LabeledDescription(infoPanel.description);
        }
        else
        {
            
            title.text = GameTextFormatter.LabeledDescription(infoPanel.status.Name);
            description.text = GameTextFormatter.LabeledDescription(infoPanel.status.Description);
        }
    }
}

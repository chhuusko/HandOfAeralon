using TMPro;
using UnityEngine;

public class InfoPanelUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] TextMeshProUGUI title, description;
    public void SetUpUIElements(InfoPanel infoPanel)
    {
        transform.SetAsLastSibling();
        title.text = infoPanel.title;
        description.text = GameTextFormatter.LabeledDescription(infoPanel.description);
    }
}

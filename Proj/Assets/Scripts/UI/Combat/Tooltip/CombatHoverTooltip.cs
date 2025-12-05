using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class CombatHoverTooltip : MonoBehaviour
{
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _description;
    private bool _isHovering;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void UpdateText(string title, string description)
    {
        SetTitle(title);
        SetDescription(description);
    }



    public void SetTitle(string title) { _title.text = title; }
    public void SetDescription(string description) { _description.text = description; }

}

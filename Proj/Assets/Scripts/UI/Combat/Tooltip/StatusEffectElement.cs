using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectElement : MonoBehaviour
{
    [SerializeField] private Image _statusEffectIcon;
    [SerializeField] private TMP_Text _statusEffectTitle;
    [SerializeField] private TMP_Text _statusEffectTurns;
    
    public void SetIcon(Sprite icon) { _statusEffectIcon.sprite = icon; }
    public void SetTitle(string title) { _statusEffectTitle.text = title; }
    public void SetTurns(int turns) 
    {
        _statusEffectTurns.text = "Turns: " + turns.ToString();  
    }
}

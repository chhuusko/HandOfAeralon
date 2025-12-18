using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatOptionsMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text _settingHoverLockSpeed;
    [SerializeField] private TMP_Text _settingMasterVolume;
    
    [SerializeField] private Slider _settingHoverLockSpeedSlider;
    [SerializeField] private CombatHoverTooltip _combatHoverTooltip;

    public void SetHoverLockSpeedText(float newLockSpeed) 
    { 
        _settingHoverLockSpeed.text = newLockSpeed.ToString("F2");  
    }
    public void SetMasterVolume(float newVolume)
    {
        _settingMasterVolume.text = newVolume.ToString("F2");
    }
}

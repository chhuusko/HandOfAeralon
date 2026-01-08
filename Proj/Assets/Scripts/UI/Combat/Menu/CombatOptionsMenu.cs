using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatOptionsMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text _settingHoverLockSpeed;
    [SerializeField] private TMP_Text _settingMasterVolume;
    
    [SerializeField] private Slider _settingHoverLockSpeedSlider;
    [SerializeField] private Slider _masterVolumeSlider;

    private void Start()
    {
        //SetHoverLockSliderValue(PlayerSettingsManager.GetInstance().GetHoverLockSpeed());
        //SetMasterVolumeSliderValue(PlayerSettingsManager.GetInstance().GetMasterVolume());
    }

    public void SetHoverLockSliderValue(float value)
    {
        _settingHoverLockSpeedSlider.value = value;
        SetHoverLockSpeedText(value);
        PlayerSettingsManager.GetInstance().SetHoverLockSpeed(value);
    }

    public void SetMasterVolumeSliderValue(float value)
    {
        _masterVolumeSlider.value = value;
        SetMasterVolumeText(value);
        PlayerSettingsManager.GetInstance().SetMasterVolume(value);
    }

    public void SetHoverLockSpeedText(float newLockSpeed) 
    { 
        _settingHoverLockSpeed.text = newLockSpeed.ToString("F2");  
    }

    public void SetMasterVolumeText(float newVolume)
    {
        _settingMasterVolume.text = newVolume.ToString("F2");
    }
}

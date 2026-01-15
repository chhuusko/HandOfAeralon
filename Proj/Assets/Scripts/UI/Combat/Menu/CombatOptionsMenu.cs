using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatOptionsMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text _settingHoverLockSpeed;
    [SerializeField] private TMP_Text _settingMasterVolume;
    [SerializeField] private TMP_Text _settingMusicVolume;
    [SerializeField] private TMP_Text _settingSFXVolume;

    [SerializeField] private Slider _settingHoverLockSpeedSlider;
    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _sfxVolumeSlider;

    private void Start()
    {
        if(PlayerSettingsManager.GetInstance())
        {
            SetHoverLockSliderValue(PlayerSettingsManager.GetInstance().GetHoverLockSpeed());
            SetMasterVolumeSliderValue(PlayerSettingsManager.GetInstance().GetMasterVolume());
            SetMusicVolumeSliderValue(PlayerSettingsManager.GetInstance().GetMusicVolume());
            SetVFXVolumeSliderValue(PlayerSettingsManager.GetInstance().GetSFXVolume());
        }
    }

    public void SetHoverLockSliderValue(float value)
    {
        SetSliderValueAndText(_settingHoverLockSpeedSlider, _settingHoverLockSpeed, value);
        PlayerSettingsManager.GetInstance().SetHoverLockSpeed(value);
    }

    public void SetMasterVolumeSliderValue(float value)
    {
        SetSliderValueAndText(_masterVolumeSlider, _settingMasterVolume, value);
        PlayerSettingsManager.GetInstance().SetMasterVolume(value);
        AudioManager.Instance?.SetMasterVolume(value);
    }
    public void SetMusicVolumeSliderValue(float value)
    {
        SetSliderValueAndText(_musicVolumeSlider, _settingMusicVolume, value);
        PlayerSettingsManager.GetInstance().SetMusicVolume(value);
        AudioManager.Instance?.SetMusicVolume(value);
    }
    public void SetVFXVolumeSliderValue(float value)
    {
        SetSliderValueAndText(_sfxVolumeSlider, _settingSFXVolume, value);
        PlayerSettingsManager.GetInstance().SetSFXVolume(value);
        AudioManager.Instance?.SetSFXVolume(value);
    }

    public void SetValueText(TMP_Text tmpText, float value)
    {
        tmpText.text = value.ToString("F2");
    }

    private void SetSliderValueAndText(Slider slider, TMP_Text tmpText, float value)
    {
        slider.value = value;
        SetValueText(tmpText, value);
    }

}

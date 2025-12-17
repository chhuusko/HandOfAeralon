using UnityEngine;
using UnityEngine.UI;

public class OptionsSettingMasterVolumeSlider : MonoBehaviour
{
    private Slider _masterVolumeSlider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        _masterVolumeSlider = GetComponent<Slider>();
    }

    void Start()
    {
        if(_masterVolumeSlider)
        {
            _masterVolumeSlider.onValueChanged.AddListener(OnValueChanged);
        }
    }

    private void OnDestroy()
    {
        _masterVolumeSlider.onValueChanged.RemoveListener(OnValueChanged);
    }

    private void OnValueChanged(float value)
    {
        //AudioManager.Instance?.SetMasterVolume(value);
    }
}

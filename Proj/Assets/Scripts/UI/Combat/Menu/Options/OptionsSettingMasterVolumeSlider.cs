using UnityEngine;
using UnityEngine.UI;

public class OptionsSettingMasterVolumeSlider : MonoBehaviour
{
    private Slider _slider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    void Start()
    {
        if(_slider)
        {
            _slider.onValueChanged.AddListener(OnValueChanged);
        }
    }

    private void OnDestroy()
    {
        _slider.onValueChanged.RemoveListener(OnValueChanged);
    }

    private void OnValueChanged(float value)
    {
        //AudioManager.Instance?.SetMasterVolume(value);
    }
}

using UnityEngine;
using TMPro;

public class CombatOptionsMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text _settingHoverLockSpeed;
    [SerializeField] private TMP_Text _settingMasterVolume;

    public void SetHoverLockSpeed(float newLockSpeed) 
    { 
        _settingHoverLockSpeed.text = newLockSpeed.ToString("F2");  
    }
    public void SetMasterVolume(float newVolume)
    {
        _settingMasterVolume.text = newVolume.ToString("F2");
    }
}

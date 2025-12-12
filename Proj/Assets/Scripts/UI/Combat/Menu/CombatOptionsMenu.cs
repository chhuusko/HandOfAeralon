using UnityEngine;
using TMPro;

public class CombatOptionsMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text _settingHoverLockSpeed;

    public void SetHoverLockSpeed(float newLockSpeed) 
    { 
        _settingHoverLockSpeed.text = newLockSpeed.ToString("F2");  
    }
}

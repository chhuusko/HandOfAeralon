using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    private Character _character;

    public void Bind(Character c)
    {
        _character = c;
        _slider.maxValue = c.GetMaxHealth();
        _slider.value = c.GetCurrentHealth();
        c.OnHealthChanged += HandleHealthChanged;
    }

    void OnDestroy()
    {
        if (_character != null)
        {
            _character.OnHealthChanged -= HandleHealthChanged;
        }
    }

    private void HandleHealthChanged(int currentHp, int maxHp)
    {
        _slider.maxValue = maxHp;
        _slider.value = currentHp;
    }
}

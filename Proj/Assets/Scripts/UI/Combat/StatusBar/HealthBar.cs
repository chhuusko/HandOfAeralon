using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Slider _easeSlider;
    [SerializeField] private float _easeSpeed = 0.001f;


    private Character _character;
    private Coroutine _easeRoutine;

    private void Update()
    {
        
    }

    public void Bind(Character c)
    {
        _character = c;
        _slider.maxValue = c.GetMaxHealth();
        _easeSlider.maxValue = c.GetMaxHealth();

        _slider.value = c.GetCurrentHealth();
        _easeSlider.value = c.GetCurrentHealth();
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
        _easeSlider.maxValue = maxHp;

        _slider.value = currentHp;

        if (_easeRoutine != null)
            StopCoroutine(_easeRoutine);

        _easeRoutine = StartCoroutine(EaseHealth());
    }

    private IEnumerator EaseHealth()
    {
        while (!Mathf.Approximately(_easeSlider.value, _slider.value))
        {
            _easeSlider.value = Mathf.Lerp(
                _easeSlider.value,
                _slider.value,
                Time.deltaTime * _easeSpeed
            );
            yield return null;
        }

        _easeSlider.value = _slider.value;
    }
}

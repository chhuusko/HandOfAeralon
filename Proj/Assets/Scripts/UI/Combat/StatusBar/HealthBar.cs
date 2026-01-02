using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider _easeHealthSlider;
    [SerializeField] private Slider _previewHealthSlider;
    [SerializeField] private Slider _mainHealthslider;
    [SerializeField] private float _easeSpeed = 1.5f;


    private Character _character;
    private Coroutine _easeRoutine;

    private void Update()
    {
        
    }

    public void Bind(Character c)
    {
        _character = c;
        _mainHealthslider.maxValue = c.GetMaxHealth();
        _easeHealthSlider.maxValue = c.GetMaxHealth();

        _mainHealthslider.value = c.GetCurrentHealth();
        _easeHealthSlider.value = c.GetCurrentHealth();
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
        _mainHealthslider.maxValue = maxHp;
        _easeHealthSlider.maxValue = maxHp;

        _mainHealthslider.value = currentHp;

        if (_easeRoutine != null)
            StopCoroutine(_easeRoutine);

        _easeRoutine = StartCoroutine(EaseHealth());
    }

    private IEnumerator EaseHealth()
    {
        while (!Mathf.Approximately(_easeHealthSlider.value, _mainHealthslider.value))
        {
            _easeHealthSlider.value = Mathf.Lerp(
                _easeHealthSlider.value,
                _mainHealthslider.value,
                Time.deltaTime * _easeSpeed
            );
            yield return null;
        }

        _easeHealthSlider.value = _mainHealthslider.value;
    }

    public void ShowPreviewDamage(int previewDamage)
    {
        if (_character == null) return;

        int currentHp = _character.GetCurrentHealth();
        int previewHp = Mathf.Max(0, currentHp - previewDamage);

        _previewHealthSlider.gameObject.SetActive(true);
        _previewHealthSlider.maxValue = _mainHealthslider.maxValue;
        _previewHealthSlider.value = previewHp;
    }

    public void HidePreview()
    {
        _previewHealthSlider.gameObject.SetActive(false);
    }
}

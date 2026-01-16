using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider _easeHealthSlider;
    [SerializeField] private Slider _previewHealthSlider;
    [SerializeField] private Slider _mainHealthslider;

    [SerializeField] private float _easeDuration = 0.5f;
    [SerializeField] private float _waitEaseDuration = 0.5f;

    [SerializeField] private Image _previewHealthFill;
    [SerializeField] private Color _startColor, _damageColor, _healColor;
    [SerializeField] private float _fadeDuration, _waitPreviewFadeDuration;

    Color _endColor;


    private Character _character;
    private Coroutine _easeRoutine;
    private Coroutine _previewColorRoutine;


    public void Bind(Character c)
    {
        _character = c;
        _mainHealthslider.maxValue = c.GetMaxHealth();
        _easeHealthSlider.maxValue = c.GetMaxHealth();
        _previewHealthSlider.maxValue = c.GetMaxHealth();

        _mainHealthslider.value = c.GetCurrentHealth();
        _easeHealthSlider.value = c.GetCurrentHealth();
        _previewHealthSlider.value = c.GetCurrentHealth();
        c.OnHealthChanged += HandleHealthChanged;
        _endColor = _damageColor;
    }

    private void OnDisable()
    {
        if (_character != null)
            _character.OnHealthChanged -= HandleHealthChanged;

        if (_easeRoutine != null)
        {
            StopCoroutine(_easeRoutine);
            _easeRoutine = null;
        }
    }

    void OnDestroy()
    {
        if (_character != null)
        {
            _character.OnHealthChanged -= HandleHealthChanged;
        }

        if (_easeRoutine != null)
        {
            StopCoroutine(_easeRoutine);
            _easeRoutine = null;
        }
    }

    private void HandleHealthChanged(int currentHp, int maxHp)
    {
        if (this == null || !isActiveAndEnabled)
            return;

        if (_mainHealthslider == null || _easeHealthSlider == null)
            return;

        _mainHealthslider.maxValue = maxHp;
        _easeHealthSlider.maxValue = maxHp;

        _mainHealthslider.value = currentHp;

        if (_easeRoutine != null)
        {
            StopCoroutine(_easeRoutine);
            _easeRoutine = null;
        }

        _easeRoutine = StartCoroutine(EaseHealth());
    }

    private IEnumerator EaseHealth()
    {
        float start = _easeHealthSlider.value;
        float target = _mainHealthslider.value;

        float elapsed = 0f;

        while(elapsed < _waitEaseDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _easeDuration;

            yield return null;
        }

        elapsed = 0;

        while (elapsed < _easeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _easeDuration;

            _easeHealthSlider.value = Mathf.Lerp(start, target, t);
            yield return null;
        }

        _easeHealthSlider.value = target;
    }

    public void ShowPreviewDamage(int previewHealthDifference)
    {
        if (_character == null) return;

        if (_previewColorRoutine != null) return;
        _previewColorRoutine = StartCoroutine(PreviewColorPulse());

        int current = _character.GetCurrentHealth();
        int previewHP = Mathf.Max(0, current + previewHealthDifference);

        _previewHealthSlider.gameObject.SetActive(true);
        _easeHealthSlider.gameObject.SetActive(false);

        if (previewHealthDifference < 0)
        {
            _endColor = _damageColor;
            _previewHealthSlider.value = current;   
            _mainHealthslider.value = previewHP;
        }
        else
        {
            _endColor = _healColor;
            _previewHealthSlider.value = previewHP;
        }
    }

    public void HidePreview()
    {
        if (_previewColorRoutine != null)
        {
            StopCoroutine(_previewColorRoutine);
            _previewColorRoutine = null;
        }

        _previewHealthFill.color = _startColor;

        _previewHealthSlider.gameObject.SetActive(false);
        _easeHealthSlider.gameObject.SetActive(true);
        _mainHealthslider.value = _character.GetCurrentHealth();
        _previewHealthSlider.value = _character.GetCurrentHealth();
    }

    private IEnumerator PreviewColorPulse()
    {
        
        while (true)
        {
            // Fade in
            yield return FadeColor(_startColor, _endColor, _fadeDuration);

            // Pause
            yield return new WaitForSeconds(_waitPreviewFadeDuration);

            // Fade out
            yield return FadeColor(_endColor, _startColor, _fadeDuration);

            // Pause
            yield return new WaitForSeconds(_waitPreviewFadeDuration);
        }
    }

    private IEnumerator FadeColor(Color from, Color to, float duration)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            _previewHealthFill.color = Color.Lerp(from, to, t);
            yield return null;
        }
    }
}

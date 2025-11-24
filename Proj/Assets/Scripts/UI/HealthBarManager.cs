using System.Collections.Generic;
using UnityEngine;

public class HealthBarManager : MonoBehaviour
{
    public static HealthBarManager _instance;

    [Header("World Space Setup")]
    [SerializeField] private Transform _healthBarContainer;
    [SerializeField] private GameObject _healthBarPrefab;
    [SerializeField] private Vector3 _offset = new Vector3(0, 2f, 0);

    private Dictionary<Character, HealthBar> _healthBars = new();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    public void Register(Character character)
    {
        if (_healthBars.ContainsKey(character)) return;

        GameObject barObj = Instantiate(_healthBarPrefab, _healthBarContainer);
        HealthBar bar = barObj.GetComponent<HealthBar>();
        bar.Bind(character);

        _healthBars.Add(character, bar);
    }

    public void Unregister(Character character)
    {
        if (!_healthBars.ContainsKey(character)) return;

        Destroy(_healthBars[character].gameObject);
        _healthBars.Remove(character);
    }

    void LateUpdate()
    {
        foreach (var pair in _healthBars)
        {
            Character character = pair.Key;
            RectTransform barRect = pair.Value.GetComponent<RectTransform>();

            if (character == null) continue;

            barRect.position = character.transform.position + _offset;
            barRect.forward = Camera.main.transform.forward;
        }
    }
}

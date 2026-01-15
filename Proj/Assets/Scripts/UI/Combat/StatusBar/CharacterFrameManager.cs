using System.Collections.Generic;
using UnityEngine;

public class CharacterFrameManager : MonoBehaviour
{
    public static CharacterFrameManager _instance;

    [Header("World Space Setup")]

    [SerializeField] private Vector3 _offset = new Vector3(0, 2.5f, 0);

    [SerializeField] private GameObject _CharacterFramePrefab;

    private Dictionary<Character, CharacterFrame> _characterFrames = new();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

    }

    public HealthBar GetHealthBarFrom(Character character)
    {
        CharacterFrame frame = _characterFrames[character];
        if (frame == null)
            return null;

        HealthBar healthBar = frame.GetHealthBar();

        if (healthBar == null)
            return null;

        return healthBar;
    }
        
    public void Register(Character character)
    {
        if(_characterFrames.ContainsKey(character)) return;
        
        GameObject frameObj = Instantiate(_CharacterFramePrefab, transform);
        CharacterFrame frame = frameObj.GetComponent<CharacterFrame>();
        frame.Bind(character);

        _characterFrames.Add(character, frame);
    }

    public void Unregister(Character character)
    {
        if (!_characterFrames.ContainsKey(character)) return;

        Destroy(_characterFrames[character].gameObject);
        _characterFrames.Remove(character);
    }

    void LateUpdate()
    {
        foreach (var pair in _characterFrames)
        {
            Character character = pair.Key;
            RectTransform barRect = pair.Value.GetComponent<RectTransform>();

            if (character == null) continue;

            barRect.position = character.transform.position + (_offset * 1f);
            float x = character.transform.position.x;
            float y = character.GetHealthBarPoint().y;
            float z = character.transform.position.z;

            Vector3 pos = new Vector3(x, y, z);

            barRect.position = pos + (_offset * 1f);

            barRect.forward = Camera.main.transform.forward;
        }
    }
}

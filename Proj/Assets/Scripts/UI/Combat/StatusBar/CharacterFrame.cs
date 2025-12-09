using UnityEngine;

public class CharacterFrame : MonoBehaviour
{

    [SerializeField] private GameObject _statusEffectBarObject;
    [SerializeField] private GameObject _healthBarObject;

    private HealthBar _healthBar;
    private StatusEffectBar _statusEffectBar;

    private Character _ownerCharacter;

    
    void Start()
    {
        
    }

    public void Bind(Character ownerCharacter)
    {
        _ownerCharacter  = ownerCharacter;
        _healthBar       = _healthBarObject.GetComponent<HealthBar>();
        _statusEffectBar = _statusEffectBarObject.GetComponent<StatusEffectBar>();

        if (_healthBar != null)
        {
            _healthBar.Bind(ownerCharacter);
        }
        else
            DebugLog.CJLogError("Healthbar is missing!");

        if (_statusEffectBar != null)
        {
            _statusEffectBar.Bind(ownerCharacter);
        }
        else
            DebugLog.CJLogError("StatusEffectsBar is missing!");
    }

    public HealthBar GetHealthBar() { return _healthBar; }
    public StatusEffectBar GetStatusEffectBar() { return _statusEffectBar; }

}

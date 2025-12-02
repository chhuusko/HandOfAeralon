using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class CombatTooltipManager : MonoBehaviour
{
    private static CombatTooltipManager _instance;

    private enum CharacterStatKey
    {
        CurrentHealth,
        CurrentInitiative,
        CurrentDamage,
        CurrentMovementPoints,

        BaseHealth,
        BaseInitiative,
        BaseDamage,
        BaseMovementPoints
    };
    
    [SerializeField] List<string> _characterStatValues = new List<string>();
    [SerializeField] private TMP_Text _characterStatFieldTMP;

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }

    void Start()
    {
        InitializeCharacterStats();
    }

    void Update()
    {
        
    }

    private void InitializeCharacterStats()
    {
        _characterStatValues.Insert((int)CharacterStatKey.CurrentHealth,          "10");
        _characterStatValues.Insert((int)CharacterStatKey.CurrentInitiative,      "10");
        _characterStatValues.Insert((int)CharacterStatKey.CurrentDamage,          "10");
        _characterStatValues.Insert((int)CharacterStatKey.CurrentMovementPoints,  "10\n");
                            
        _characterStatValues.Insert((int)CharacterStatKey.BaseHealth,        "10");
        _characterStatValues.Insert((int)CharacterStatKey.BaseInitiative,    "10");
        _characterStatValues.Insert((int)CharacterStatKey.BaseDamage,        "10");
        _characterStatValues.Insert((int)CharacterStatKey.BaseMovementPoints,"10");
        
        _characterStatFieldTMP.text = "";
        
        foreach (string value in  _characterStatValues)
        {
            _characterStatFieldTMP.text += value + "\n";
        }
    }

    public void RebuildCharacterStatTooltip(Character character)
    {
        _characterStatValues.Insert((int)CharacterStatKey.CurrentHealth,         $"{character.GetCurrentHealth()}");
        _characterStatValues.Insert((int)CharacterStatKey.CurrentInitiative,     $"{character.GetInitiative()}");
        _characterStatValues.Insert((int)CharacterStatKey.CurrentDamage,         $"{character.GetDamage()}");
        _characterStatValues.Insert((int)CharacterStatKey.CurrentMovementPoints, $"{character.GetMovementPoints()}" + "\n");

        _characterStatValues.Insert((int)CharacterStatKey.BaseHealth,            $"{character.GetMaxHealth()}");
        _characterStatValues.Insert((int)CharacterStatKey.BaseInitiative,        $"Joppa ge mig BaseInitiative");
        _characterStatValues.Insert((int)CharacterStatKey.BaseDamage,            $"{character.GetBaseDamage()}");
        _characterStatValues.Insert((int)CharacterStatKey.BaseMovementPoints,    $"{character.GetBaseMovementPoints()}" + "\n");
    }

    
}

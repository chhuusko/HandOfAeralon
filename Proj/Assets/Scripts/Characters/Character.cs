using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Faction { Friendly, Enemy }

[System.Serializable]
public class CharacterData
{
    [Header("Data")]
    [SerializeField] private ClassData _classData;
    [SerializeField] private CharacterClass _characterClass;
    [SerializeField] private Faction _faction;
    public ClassData ClassData => _classData;
    public CharacterClass CharacterClass => _characterClass;
    public Faction Faction => _faction;
    
    [Header("Base Stats")]
    [SerializeField] private int _baseHealthPoints;
    [SerializeField] private int _baseInitiative;
    [SerializeField] private int _baseDamage;
    [SerializeField] private int _baseMovementPoints;
    public int BaseHealthPoints => _baseHealthPoints;
    public int BaseInitiative => _baseInitiative;
    public int BaseDamage => _baseDamage;
    public int BaseMovementPoints => _baseMovementPoints;
    
    [Header("Current stats")]
    [SerializeField] private int _currentHealthPoints;
    [SerializeField] private List<Ability> _abilities;
    public int CurrentHealthPoints => _currentHealthPoints;
    public IReadOnlyList<Ability> Abilities => _abilities;
    public List<Ability> ActiveAbilities { get; set; }
    
    [Header("Status Effects")]
    private TraitManager _traitManager = new();
    public TraitManager TraitManager => _traitManager;

    public CharacterData(ClassData classData, Faction faction, bool generateTraits)
    {
        _classData = classData;
        _faction = faction;
        
        InitializeClassData();
        InitializeTraits();
        
        if (generateTraits)
        {
            GenerateTraits();
        }
    }
    
    /// <summary>
    /// Generates a new friendly character based on the class data.
    /// </summary>
    public void InitializeClassData()
    {
        if (ClassData == null)
        {
            return;
        }

        if (_faction == Faction.Friendly)
        {
            // Set values from class data.
            _currentHealthPoints = _baseHealthPoints = UnityEngine.Random.Range(ClassData.minHealthPoints, ClassData.maxHealthPoints + 1);
            _baseInitiative =  UnityEngine.Random.Range(ClassData.minInitiative, ClassData.maxInitiative + 1);
            _baseDamage = UnityEngine.Random.Range(ClassData.minDamage, ClassData.maxDamage + 1);
            _baseMovementPoints = UnityEngine.Random.Range(ClassData.minMovementPoints, ClassData.maxMovementPoints + 1);
        }
        
        _characterClass = ClassData.characterClass;
        _abilities = ClassData.abilities;
        ActiveAbilities = _abilities;
    }

    public void InitializeTraits()
    {
        if (_traitManager == null)
        {
            _traitManager = new TraitManager();
        }
    }
    
    private void GenerateTraits()
    {
        _traitManager.GenerateTraits(this);
    }

    public void SetClassData(ClassData classData) => _classData = classData;
    public void SetCharacterClass(CharacterClass characterClass) => _characterClass = characterClass;
    public void SetFaction(Faction faction) => _faction = faction;
    public void SetBaseHealthPoints(int health) => _baseHealthPoints = Mathf.Max(1, health);
    public void SetBaseInitiative(int initiative) => _baseInitiative = Mathf.Max(1, initiative);
    public void SetBaseDamage(int damage) => _baseDamage = Mathf.Max(1, damage);
    public void SetBaseMovementPoints(int movementPoints) => _baseMovementPoints = Mathf.Max(movementPoints, 1);
    public void SetCurrentHealthPoints(int health) => _currentHealthPoints = Mathf.Max(health, 0);
    public void Heal(int amount) => SetCurrentHealthPoints(Mathf.Min(CurrentHealthPoints + amount, _baseHealthPoints));
    public void SetAbilities(List<Ability> abilities) => _abilities = new List<Ability>(abilities);
    public void SetActiveAbilities(List<Ability> abilities) => ActiveAbilities = abilities;
}

[RequireComponent(typeof(Rigidbody))]
public class Character : MonoBehaviour
{
    [SerializeField] private Renderer _factionIndicator; // JLW

    public event Action<int> OnHealthChanged;
    public event Action<int, GameObject> OnTakeDamage;
    public event Action<int, GameObject> OnWasHealed;


    public const int MOVEMENT_POINTS = 5;
    public const float DEATH_COOLDOWN = 2.5f;
    
    [Header("Current stats")]
    [SerializeField] private int _currentInitiative;
    [SerializeField] private int _currentDamage;
    [SerializeField] private int _currentMovementPoints;
    
    [Header("Abilities")]
    private AbilityHandler _abilityHandler;
    private Dictionary<Ability, int> _currentCooldowns = new();

    [Header("State")] 
    public bool CanMove { get; set; } = true;
    public bool CanUseAbility { get; set; } = true;
    public bool IsTargetable { get; set; } = true;

    [Header("Status effects")]
    private StatusEffectManager _statusEffectManager;

    [Header("Misc")] 
    [SerializeField] private GameObject _bodyMesh;
    [SerializeField] private GameObject _weaponMesh;
    [SerializeField] private CharacterData _data;
    [SerializeField] private Vector2Int _currentTileIndex;
    public CharacterData Data => _data;

    private void OnEnable()
    {
        CombatEventManager.OnEnterCombatStateTakeTurn += UpdateAbilityCooldowns;
        CombatEventManager.OnEnterCombatStateTakeTurn += ResetCanAttack;
        
        PopupTextManager damagePopupTextManager = PopupTextManager.GetInstance();
        if(damagePopupTextManager != null)
        {
            damagePopupTextManager.BindEventOnTakeDamage(this);
            damagePopupTextManager.BindEventOnWasHealed(this);
        }

        CombatTooltipManager combatTooltipManager = CombatTooltipManager.GetInstance();
        if (combatTooltipManager != null)
        {
            combatTooltipManager.GetCharacterLayout().BindEventEventOnTakeDamage(this);
        }
    }

    private void Awake()
    {
        _statusEffectManager = GetComponent<StatusEffectManager>();
        
        // Enemies aren't created via character data, so traits have to be created at start.
        _statusEffectManager.SetTraitManager(_data?.TraitManager);
        _data?.InitializeTraits();

        if (!TryGetComponent(out _abilityHandler))
        {
            Debug.LogError("Character is missing AbilityHandler component!");
            return;
        }
    }

    private void Start()
    {
        UpdateFactionIndicator();
    }

    private void UpdateFactionIndicator()
    {
        if (_factionIndicator == null) return;

        Color c = (GetFaction() == Faction.Friendly) ? new Color(0f, 1f, 0.2f, 0.5f) : new Color(1f, 0.1f, 0.1f, 0.5f);

        var mat = _factionIndicator.material;
        mat.SetColor("_Color", c);
    }


    private void OnDisable()
    {
        CombatEventManager.OnEnterCombatStateTakeTurn -= UpdateAbilityCooldowns;
        CombatEventManager.OnEnterCombatStateTakeTurn -= ResetCanAttack;

        PopupTextManager damagePopupTextManager = PopupTextManager.GetInstance();
        if (damagePopupTextManager != null)
        {
            damagePopupTextManager.UnBindEventOnTakeDamage(this);
            damagePopupTextManager.UnBindEventOnWasHealed(this);
        }

        CombatTooltipManager combatTooltipManager = CombatTooltipManager.GetInstance();
        if (combatTooltipManager != null)
        {
            combatTooltipManager.GetCharacterLayout().UnBindEventEventOnTakeDamage(this);
        }
    }

    public void Update()
    {
        // NOTE (CJ & Carl): Testkod f�r animationer
        //Animator animator = GetComponent<Animator>();
        //if(animator)
        //{
        //    animator.GetBool("IsMoving");
        //    if (IsMoving())
        //    {
        //        animator.SetBool("IsMoving", true);
        //    }
        //    else
        //    {
        //        animator.SetBool("IsMoving", false);
        //    }
        //
        //}
    }
    
    // Data.
    public ClassData GetClassData() => _data.ClassData;
    public CharacterClass GetCharacterClass() => _data.CharacterClass;
    public Faction GetFaction() => _data.Faction;
    
    // Base stats.
    public int GetMaxHealth() => _data.BaseHealthPoints;
    public int GetBaseInitiative() => _data.BaseInitiative;
    public int GetBaseDamage() => _data.BaseDamage;
    public int GetBaseMovementPoints() => _data.BaseMovementPoints;
    
    // Current stats.
    public int GetCurrentHealth() => _data.CurrentHealthPoints;
    public int GetInitiative() => _currentInitiative;
    public int GetDamage() => _currentDamage;
    public int GetMovementPoints() => _currentMovementPoints;
    public Vector2Int GetCurrentTileIndex() => _currentTileIndex;
    public CombatGridTile GetCurrentTileComponent() =>
        CombatManager._instance.GetTileComponent(_currentTileIndex.x, _currentTileIndex.y);
    
    // Abilities.
    public AbilityHandler GetAbilityHandler() => _abilityHandler;
    public IReadOnlyList<Ability> GetAvailableAbilities() => _data.Abilities;

    // Status Effects.
    public StatusEffectManager GetStatusEffectManager() => _statusEffectManager;
    public TraitManager GetTraitManager() => _data.TraitManager;

    // Base stats.
    public void SetCharacterClass(CharacterClass characterClass) => _data.SetCharacterClass(characterClass);
    public void SetFaction(Faction faction) => _data.SetFaction(faction);
    public void SetBaseHealthPoints(int healthPoints) => _data.SetBaseHealthPoints(healthPoints);
    public void SetBaseInitiative(int initiative) => _data.SetBaseInitiative(initiative);
    public void SetBaseDamage(int damage) => _data.SetBaseDamage(damage);
    public void SetBaseMovementPoints(int movementPoints) => _data.SetBaseMovementPoints(movementPoints);
    
    // Misc.
    public GameObject GetBodyMesh() => _bodyMesh;
    public void SetBodyMesh(GameObject mesh) => _bodyMesh = mesh;
    public GameObject GetWeaponMesh() => _bodyMesh;
    public void SetWeaponMesh(GameObject mesh) => _bodyMesh = mesh;
    
    // Health.
    public void SetCurrentHealthPoints(int healthPoints)
    {
        _data.SetCurrentHealthPoints(healthPoints);
        OnHealthChanged?.Invoke(_data.CurrentHealthPoints);
        if (_data.CurrentHealthPoints <= 0)
        {
            StartCoroutine(RemoveCharacter());
        }
    }

    public void IncreaseCurrentHealthPoints(int amount = 1) => 
        SetCurrentHealthPoints(_data.CurrentHealthPoints + amount);
    
    public void DecreaseCurrentHealthPoints(int amount = 1) =>
        SetCurrentHealthPoints(_data.CurrentHealthPoints - amount);

    // Initiative.
    public void SetCurrentInitiative(int initiative) => 
        _currentInitiative = Mathf.Max(initiative, 0);
    
    public void IncreaseCurrentInitiative(int amount = 1) => 
        SetCurrentInitiative(_currentInitiative + amount);
    
    public void DecreaseCurrentInitiative(int amount = 1) =>
        SetCurrentInitiative(_currentInitiative - amount);
    
    // Damage.
    public void SetCurrentDamage(int damage) =>
        _currentDamage = Mathf.Max(damage, 0);
    
    public void IncreaseCurrentDamage(int amount = 1) =>
        SetCurrentDamage(_currentDamage + amount);
    
    public void DecreaseCurrentDamage(int amount = 1) =>
        SetCurrentDamage(_currentDamage - amount);
    
    // Movement points.
    public void SetCurrentMovementPoints(int movementPoints) =>
        _currentMovementPoints = Mathf.Max(movementPoints, 0);
    
    public void ResetCurrentMovementPoints() =>
        _currentMovementPoints = GetBaseMovementPoints();
    
    public void IncreaseCurrentMovementPoints(int amount = 1) =>
        SetCurrentMovementPoints(_currentMovementPoints + amount);
    
    public void DecreaseCurrentMovementPoints(int amount = 1) =>
        SetCurrentMovementPoints(_currentMovementPoints - amount);
    
    public void SetCurrentTileIndex(Vector2Int tileIndex) => _currentTileIndex = tileIndex;

    public void StartAbilityCooldown(Ability ability)
    {
        if (GetAvailableAbilities().Contains(ability) && !_currentCooldowns.ContainsKey(ability))
        {
            _currentCooldowns.Add(ability, ability.GetCooldown());
        }
    }

    private void ResetCanAttack(Character c)
    {
        CanUseAbility = true;
    }

    private void UpdateAbilityCooldowns(Character c)
    {
        // Only update cooldowns for this character.
        if (c != this)
        {
            return;
        }
        
        var finishedAbilities = new List<Ability>();
        
        foreach (var ability in _currentCooldowns.Keys.ToList())
        {
            _currentCooldowns[ability]--;
            if (_currentCooldowns[ability] <= 0)
            {
                finishedAbilities.Add(ability);
            }
        }

        foreach (var ability in finishedAbilities)
        {
            _currentCooldowns.Remove(ability);
        }
    }

    public bool IsAbilityCooldownActive(Ability ability)
    {
        return _currentCooldowns.ContainsKey(ability);
    }

    public int GetCurrentCooldown(Ability ability)
    {
        return _currentCooldowns.GetValueOrDefault(ability, 0);
    }

    /// <summary>
    /// Generates a new friendly character.
    /// </summary>
    /// <param name="data">The character data to generate from.</param>
    public void Initialize(CharacterData data)
    {
        _data = data;
        
        if (_data.ClassData == null)
        {
            return;
        }
            
        // Set values from class data.
        _currentInitiative = _data.BaseInitiative;
        _currentDamage = _data.BaseDamage;
        _currentMovementPoints = _data.BaseMovementPoints;
        
        SetMeshLayers(_bodyMesh);
        SetMeshLayers(_weaponMesh);
    }

    private void SetMeshLayers(GameObject mesh)
    {
        if (!mesh)
        {
            return;
        }

        uint layerMask;

        if (GetFaction() == Faction.Friendly) 
        {
            mesh.layer = LayerMask.NameToLayer("Friendly");
            layerMask = 1 << 2;
        }
        else
        {
            mesh.layer = LayerMask.NameToLayer("Enemy");
            layerMask = 1 << 7;
        }
        
        SkinnedMeshRenderer smr = mesh.GetComponent<SkinnedMeshRenderer>();
        if (!smr)
        {
            return;
        }

        smr.renderingLayerMask = layerMask;
    }

    public void AddHealthBar()
    {
        if (HealthBarManager._instance != null)
        {
            HealthBarManager._instance.Register(this);
        }
        else
        {
            Debug.LogError($"Character.cs | No health bar canvas (prefab by JLW) found in scene!");
        }
    }
    
    /// <summary>
    /// Takes damages.
    /// </summary>
    /// <param name="damage">The amount of damage to take.</param>
    /// <returns>Whether the character died.</returns>
    public bool TakeDamage(int damage)
    {
        _data.SetCurrentHealthPoints(_data.CurrentHealthPoints - damage);
        OnHealthChanged?.Invoke(_data.CurrentHealthPoints);
        OnTakeDamage?.Invoke(damage, gameObject);

        Debug.Log($"{name} took {damage} damage! Remaining health: {GetCurrentHealth()}");
        
        if (_data.CurrentHealthPoints <= 0)
        {
            StartCoroutine(RemoveCharacter());
            return true;
        }
        
        Animator animator = null;
        if (TryGetComponent<Animator>(out animator))
        {
            animator.SetTrigger("TakeDamage");
        }

        return false;
    }

    public bool TakeDamage(int damage, Character source)
    {
        if (source)
        {
            Debug.Log($"{name} took {damage} damage from {source.GetFaction()} {source.name}! Remaining health: {GetCurrentHealth()}");
        }
        
        return TakeDamage(damage);
    }
     
    private IEnumerator RemoveCharacter()
    {
        CombatEventManager.InvokeOnCharacterDeath(this);

        float deathCooldown = DEATH_COOLDOWN;
        
        Animator animator = null;
        if (TryGetComponent<Animator>(out animator))
        {
            animator.SetTrigger("Death");
            AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            deathCooldown = animatorStateInfo.length;
        }

        yield return new WaitForSeconds(deathCooldown);
        Destroy(gameObject);
    }

    public void Heal(int healAmount)
    {
        _data.Heal(healAmount);
        OnHealthChanged?.Invoke(_data.CurrentHealthPoints);
        OnWasHealed?.Invoke(healAmount, gameObject);
        Debug.Log($"Healing {healAmount} health. New health: {GetCurrentHealth()}");
    }
    
    public bool IsMoving()
    {
        CharacterMovement component = null;
        if (TryGetComponent<CharacterMovement>(out component))
        {
            return component.IsMoving();
        }
        Debug.Log($"Character.cs 245 | CharacterMovement component not found!");
        return false;
    }
    /// <summary>
    /// Rotates towards target over time.
    /// </summary>
    public void RotateTowards(Transform target, float duration)
    {
        // Calculate direction.
        Vector3 direction = (target.position - transform.position).normalized;

        // Y is zero to not rotate up or down.
        direction.y = 0f;

        Quaternion targetRot = Quaternion.LookRotation(direction);
        StartCoroutine(RotateCoroutine(targetRot, duration));
    }

    private IEnumerator RotateCoroutine(Quaternion targetRot, float duration)
    {
        Quaternion startRot = transform.rotation;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }
    }

    void OnDestroy()
    {
        if (HealthBarManager._instance != null)
        {
            HealthBarManager._instance.Unregister(this);
        }
    }
}

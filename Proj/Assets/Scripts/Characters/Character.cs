using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using UnityEngine;

public enum Faction { Friendly, Enemy }

[System.Serializable]
public class CharacterData
{
    public event Action OnDerivedStatsChanged;
    
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
    
    [Header("Derived Stats")]
    [SerializeField] private int _derivedHealthPoints;
    [SerializeField] private int _derivedDamage;
    public int DerivedHealthPoints => _derivedHealthPoints;
    public int DerivedDamage => _derivedDamage;
    
    [Header("Current stats")]
    // Normalized health (0-1).
    [SerializeField] private float _currentHealthPoints01;
    [SerializeField] private List<Ability> _abilities;
    [SerializeField] private List<Ability> _activeAbilities;
    public float CurrentHealthPoints01 => _currentHealthPoints01;
    public int CurrentHealthPoints => Mathf.RoundToInt(_derivedHealthPoints * _currentHealthPoints01);
    public IReadOnlyList<Ability> Abilities => _abilities;
    
    public List<Ability> ActiveAbilities => _activeAbilities;
    
    [Header("Status Effects")]
    private TraitManager _traitManager = new();
    public TraitManager TraitManager => _traitManager;

    private bool _healthInitialized;

    public CharacterData(ClassData classData, Faction faction, bool generateTraits)
    {
        _classData = classData;
        _faction = faction;
        
        if (generateTraits)
        {
            GenerateTraits();
        }
        
        InitializeClassData();
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
            _baseHealthPoints = UnityEngine.Random.Range(ClassData.minHealthPoints, ClassData.maxHealthPoints + 1);
            _baseInitiative =  UnityEngine.Random.Range(ClassData.minInitiative, ClassData.maxInitiative + 1);
            _baseDamage = UnityEngine.Random.Range(ClassData.minDamage, ClassData.maxDamage + 1);
            _baseMovementPoints = UnityEngine.Random.Range(ClassData.minMovementPoints, ClassData.maxMovementPoints + 1);
        }
        
        CalculateDerivedStats(1, 1, false);
        _currentHealthPoints01 = 1f;
        _healthInitialized = true;
        
        _characterClass = ClassData.characterClass;
        _abilities = ClassData.abilities;
        _activeAbilities = new List<Ability>(_abilities);
    }

    public void InitializeFromJSON(Faction faction, int baseHP, int baseDamage, int baseInitiative,
        int baseMovementPoints, int currentHP, List<Ability> abilities = null)
    {
        _faction = faction;

        _baseHealthPoints = baseHP;
        _baseDamage = baseDamage;
        _baseInitiative = baseInitiative;
        _baseMovementPoints = baseMovementPoints;

        _abilities = _classData.abilities;
        _activeAbilities = new List<Ability>(_abilities);

        // 1) Set derived to base (no preservation) so _derivedHealthPoints = baseDerived.
        SetDerivedHealthPoints(1f, false);
        SetDerivedDamage(_baseDamage);

        // 2) Compute saved fraction against base derived BEFORE any trait changes.
        float savedPercent = (_derivedHealthPoints > 0) ? (float)currentHP / _derivedHealthPoints : 0f;
        savedPercent = Mathf.Clamp01(savedPercent);

        // 3) Generate traits (this will change _derivedHealthPoints).
        GenerateTraits();

        // 4) Apply saved fraction — this preserves the saved percentage regardless of how traits changed max HP.
        _currentHealthPoints01 = savedPercent;
        _healthInitialized = true; // mark initialized so later derived changes preserve absolute HP if you want
    }

    public void InitializeTraits()
    {
        if (_traitManager == null)
        {
            _traitManager = new TraitManager();
        }

        _traitManager.CharacterData = this;
    }
    
    public void GenerateTraits()
    {
        InitializeTraits();
        
        _traitManager.GenerateTraits(this);
    }

    public void CalculateDerivedStats(float factor)
    {
        CalculateDerivedStats(factor, factor);
    }

    public void CalculateDerivedStats(float hpFactor, float damageFactor, bool preserveCurrentHP = true)
    {
        _traitManager.ModifyDerivedStats(ref hpFactor, ref damageFactor);
        
        SetDerivedHealthPoints(hpFactor, preserveCurrentHP);
        SetDerivedDamage(Mathf.RoundToInt(_baseDamage * damageFactor));
        
        OnDerivedStatsChanged?.Invoke();
    }

    public void InitializeCurrentHealthFromSave(int currentHealth)
    {
        _currentHealthPoints01 = Mathf.Clamp01((float)currentHealth / _derivedHealthPoints);
        _healthInitialized = true;
    }
    
    public void SetBaseHealthPoints(int health) => _baseHealthPoints = Mathf.Max(health, 1);
    public void SetBaseDamage(int damage) => _baseDamage = Mathf.Max(damage, 1);

    public void SetClassData(ClassData classData) => _classData = classData;
    public void SetCharacterClass(CharacterClass characterClass) => _characterClass = characterClass;
    public void SetFaction(Faction faction) => _faction = faction;
    public void SetBaseInitiative(int initiative) => _baseInitiative = Mathf.Max(1, initiative);
    
    public void SetDerivedHealthPoints(float hpFactor, bool preserveCurrentHP = true)
    {
        int oldDerived = _derivedHealthPoints;
        int newDerived = Mathf.Max(1, Mathf.RoundToInt(_baseHealthPoints * hpFactor));
    
        // If derived health hasn't changed, don't recalculate current health
        if (newDerived == oldDerived && _healthInitialized)
        {
            return;
        }
        
        int oldCurrentAbsolute = Mathf.RoundToInt(_currentHealthPoints01 * oldDerived);
        
        _derivedHealthPoints = newDerived;

        if (_healthInitialized)
        {
            if (preserveCurrentHP && oldDerived > 0)
            {
                _currentHealthPoints01 = Mathf.Clamp01((float)oldCurrentAbsolute / newDerived);
            }
            else
            {
                _currentHealthPoints01 = 1f;
            }
        }
        else
        {
            _currentHealthPoints01 = 1f;
            _healthInitialized = true;
        }
    }

    public void SetDerivedDamage(int damage) => _derivedDamage = Mathf.Max(1, damage);
    public void SetBaseMovementPoints(int movementPoints) => _baseMovementPoints = Mathf.Max(movementPoints, 1);
    public void SetCurrentHealthPoints(int health)
    {
        if (_derivedHealthPoints <= 0)
        {
            return;
        }
        
        _currentHealthPoints01 = Mathf.Clamp01((float)health / _derivedHealthPoints);
    }

    public void Heal(int amount)
    {
        float heal01 = (float)amount / _derivedHealthPoints;
        _currentHealthPoints01 = Mathf.Clamp01(_currentHealthPoints01 + heal01);
    }

    public void TakeDamage(int damage)
    {
        float dmg01 = (float)damage / _derivedHealthPoints;
        _currentHealthPoints01 = Mathf.Max(0f, _currentHealthPoints01 - dmg01);
    }

    public void SetAbilities(List<Ability> abilities) => _abilities = new List<Ability>(abilities);
    public void SetActiveAbilities(List<Ability> abilities)
    {
        _activeAbilities = abilities;
    }
}

[RequireComponent(typeof(Rigidbody))]
public class Character : MonoBehaviour
{
    [SerializeField] private Renderer _factionIndicator; // JLW

    public event Action<int, int> OnHealthChanged;
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
        CombatEventManager.OnEnterCombatStateEndCombat += ResetAbilities;
        
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
        CombatEventManager.OnEnterCombatStateEndCombat -= ResetAbilities;

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

        if (_data != null)
        {
            _data.OnDerivedStatsChanged -= DerivedStatsChanged; 
        }
    }
    
    /// <summary>
    /// Generates a new friendly character.
    /// </summary>
    /// <param name="data">The character data to generate from.</param>
    public void Initialize(CharacterData data)
    {
        _data = data;
        _data.OnDerivedStatsChanged += DerivedStatsChanged; 
        
        if (_data.ClassData == null)
        {
            return;
        }
            
        // Set values from class data.
        _currentInitiative = _data.BaseInitiative;
        _currentDamage = _data.DerivedDamage;
        _currentMovementPoints = _data.BaseMovementPoints;
        
        SetMeshLayers(_bodyMesh);
        SetMeshLayers(_weaponMesh);

        if(data.CharacterClass == CharacterClass.Bard)
        {
            data.SetActiveAbilities(data.Abilities.ToList());
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
    public int GetBaseHealth() => Data.BaseHealthPoints;
    public int GetBaseDamage() => Data.BaseDamage;
    public int GetBaseInitiative() => _data.BaseInitiative;
    public int GetBaseMovementPoints() => _data.BaseMovementPoints;
    
    // Derived stats.
    public int GetMaxHealth() => _data.DerivedHealthPoints;
    public int GetDerivedDamage() => _data.DerivedDamage;
    
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
    public void SetBaseInitiative(int initiative)
    {
        _data.SetBaseInitiative(initiative);
        CombatEventManager.InvokeOnCharacterInitiativeChanged();
    }
    public void SetBaseMovementPoints(int movementPoints) => _data.SetBaseMovementPoints(movementPoints);
    public void SetDerivedHealthPoints(int newMax, float hpFactor)
    {
        _data.SetDerivedHealthPoints(hpFactor);
        OnHealthChanged?.Invoke(_data.CurrentHealthPoints, _data.DerivedHealthPoints);
    }

    public void SetDerivedDamage(int damage) => _data.SetDerivedDamage(damage);
    
    // Misc.
    public GameObject GetBodyMesh() => _bodyMesh;
    public void SetBodyMesh(GameObject mesh) => _bodyMesh = mesh;
    public GameObject GetWeaponMesh() => _bodyMesh;
    public void SetWeaponMesh(GameObject mesh) => _bodyMesh = mesh;
    
    // Health.
    public void SetCurrentHealthPoints(int healthPoints)
    {
        _data.SetCurrentHealthPoints(healthPoints);
        OnHealthChanged?.Invoke(_data.CurrentHealthPoints, _data.DerivedHealthPoints);
    }

    public void IncreaseCurrentHealthPoints(int amount = 1) => 
        SetCurrentHealthPoints(_data.CurrentHealthPoints + amount);
    
    public void DecreaseCurrentHealthPoints(int amount = 1) =>
        SetCurrentHealthPoints(_data.CurrentHealthPoints - amount);

    // Initiative.
    public void SetCurrentInitiative(int initiative)
    {
        _currentInitiative = Mathf.Max(initiative, 0);
        CombatEventManager.InvokeOnCharacterInitiativeChanged();
    }
        
    
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

    private void DerivedStatsChanged()
    {
        OnHealthChanged?.Invoke(_data.CurrentHealthPoints, _data.DerivedHealthPoints);
    }

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

    private void ResetAbilities(bool playerWon)
    {
        _currentCooldowns = new Dictionary<Ability, int>();
        
        // Reset which abilities are active.
        _data.SetActiveAbilities(new List<Ability>(_data.Abilities));
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

    public void ChangeCooldown(Ability ability, int amount)
    {
        if (!_currentCooldowns.ContainsKey(ability))
        {
            return;
        }
        
        _currentCooldowns[ability] += amount;
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
            layerMask = (1 << 0) | (1 << 2);
        }
        else
        {
            mesh.layer = LayerMask.NameToLayer("Enemy");
            layerMask = (1 << 0) | (1 << 7);
        }
        
        SkinnedMeshRenderer smr = mesh.GetComponent<SkinnedMeshRenderer>();
        if (!smr)
        {
            return;
        }

        smr.renderingLayerMask = layerMask;
    }

    public void AddCharacterFrame()
    {
        if (CharacterFrameManager._instance != null)
        {
            CharacterFrameManager._instance.Register(this);
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
        float oldHealth = GetCurrentHealth();
        
        _data.TakeDamage(damage);
        OnHealthChanged?.Invoke(_data.CurrentHealthPoints, _data.DerivedHealthPoints);
        OnTakeDamage?.Invoke(damage, gameObject);
        
        float newHealth = GetCurrentHealth();

        Debug.Log($"{name} took {damage} damage! Remaining health: {GetCurrentHealth()}");
        
        if (_data.CurrentHealthPoints01 <= 0.001f)
        {
            PlayDamageSound(true, newHealth / oldHealth);
            StartCoroutine(RemoveCharacter());
            return true;
        }
        
        Animator animator = null;
        if (TryGetComponent<Animator>(out animator))
        {
            animator.SetTrigger("TakeDamage");
        }

        PlayDamageSound(false, newHealth / oldHealth);
        return false;
    }

    public void PreviewHealthChange(int health)
    {
        // TODO: Call an event that updates healthbar temporarily without updating current HP.
        DebugLog.MGLog("Health differens that healthbar should preview is: " + health);
    }

    public bool TakeDamage(int damage, Character source)
    {
        if (source)
        {
            Debug.Log($"{name} took {damage} damage from {source.GetFaction()} {source.name}! Remaining health: {GetCurrentHealth()}");
        }
        
        return TakeDamage(damage);
    }

    private void PlayDamageSound(bool died, float damageScale)
    {
        EventReference sound = default;
        switch (GetCharacterClass())
        {
            case CharacterClass.Barbarian:
                sound = died ? FMODEvents.Instance.BarbarianDeath : FMODEvents.Instance.BarbarianTakeDamage;
                break;
            case CharacterClass.Rogue:
                sound = died ? FMODEvents.Instance.RogueDeath : FMODEvents.Instance.RogueTakeDamage;
                break;
            case CharacterClass.Bard:
                sound = died ? FMODEvents.Instance.BardDeath : FMODEvents.Instance.BardTakeDamage;
                break;
            case CharacterClass.Sorceress:
                sound = died ? FMODEvents.Instance.SorceressDeath : FMODEvents.Instance.SorceressTakeDamage;
                break;
        }

        if (sound.IsNull)
        {
            return;
        }
        
        AudioManager.Instance.PlayParameterizedOneShot(sound, transform.position, FMODEvents.Instance.DamageParameter, damageScale);
    }
     
    private IEnumerator RemoveCharacter()
    {

        Selector._instance.DeselectCharacter();

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
        OnHealthChanged?.Invoke(_data.CurrentHealthPoints, _data.DerivedHealthPoints);
        OnWasHealed?.Invoke(healAmount, gameObject);
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.Healed, transform.position);
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
        if (CharacterFrameManager._instance != null)
        {
            CharacterFrameManager._instance.Unregister(this);
        }
    }
}

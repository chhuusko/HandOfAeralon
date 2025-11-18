using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

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
    [SerializeField] private List<Ability> _availableAbilities;
    public int CurrentHealthPoints => _currentHealthPoints;
    public IReadOnlyList<Ability> AvailableAbilities => _availableAbilities;

    public CharacterData(ClassData classData, Faction faction)
    {
        _classData = classData;
        _faction = faction;
        InitializeClassData();
    }
    
    /// <summary>
    /// Generates a new friendly character based on the class data.
    /// </summary>
    private void InitializeClassData()
    {
        if (ClassData == null)
        {
            return;
        }
            
        // Set values from class data.
        _currentHealthPoints = _baseHealthPoints = UnityEngine.Random.Range(ClassData.minHealthPoints, ClassData.maxHealthPoints + 1);
        _baseInitiative =  UnityEngine.Random.Range(ClassData.minSpeed, ClassData.maxSpeed + 1);
        _baseDamage = UnityEngine.Random.Range(ClassData.minDamage, ClassData.maxDamage + 1);
        _baseMovementPoints = UnityEngine.Random.Range(ClassData.minMovementPoints, ClassData.maxMovementPoints + 1);
        _characterClass = ClassData.characterClass;
        _availableAbilities = ClassData.abilities;
    }

    public void SetCharacterClass(CharacterClass characterClass) => _characterClass = characterClass;
    public void SetFaction(Faction faction) => _faction = faction;
    public void SetBaseHealthPoints(int health) => _baseHealthPoints = Mathf.Max(1, health);
    public void SetBaseInitiative(int initiative) => _baseInitiative = Mathf.Max(1, initiative);
    public void SetBaseDamage(int damage) => _baseDamage = Mathf.Max(1, damage);
    public void SetBaseMovementPoints(int movementPoints) => _baseMovementPoints = Mathf.Max(1, movementPoints);
    public void SetCurrentHealthPoints(int health) => _currentHealthPoints = health;
    public void Heal(int amount) => SetCurrentHealthPoints(Mathf.Min(CurrentHealthPoints + amount, _baseHealthPoints));
    public void SetCurrentAbilities(List<Ability> abilities) => _availableAbilities = new List<Ability>(abilities);
}

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(NavMeshAgent))]
public class Character : MonoBehaviour
{
    public const int MOVEMENT_POINTS = 5;
    
    // TODO: Traits.
    
    [Header("Current stats")]
    [SerializeField] private int _currentSpeed;
    [SerializeField] private int _currentDamage;
    [SerializeField] private int _currentMovementPoints;
    
    [Header("Abilities")]
    private AbilityHandler _abilityHandler;
    private Dictionary<Ability, int> _currentCooldowns = new();
    
    [Header("Misc")]
    [SerializeField] private CharacterData _data;
    [SerializeField] private Vector2Int _currentTileIndex;
    private NavMeshAgent _navMeshAgent;
    public CharacterData Data => _data;
    
    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }
    
    private void Start()
    {
        if (!TryGetComponent(out _abilityHandler))
        {
            Debug.LogError("Character is missing AbilityHandler component!");
            return;
        }
    }

    public void ResetCharacter() // Endast för testkörning (JLW)
    {
        _currentSpeed = _data.BaseInitiative;
        _currentDamage = _data.BaseDamage;
        _currentMovementPoints = _data.BaseMovementPoints;
    }

    public void Update()
    {
        // NOTE (CJ & Carl): Testkod f�r animationer
        Animator animator = GetComponent<Animator>();
        if(animator)
        {
            animator.GetBool("IsMoving");
            if (IsMoving())
            {
                animator.SetBool("IsMoving", true);
            }
            else
            {
                animator.SetBool("IsMoving", false);
            }

        }
    }
    
    // Data.
    public ClassData GetClassData() => _data.ClassData;
    public CharacterClass GetCharacterClass() => _data.CharacterClass;
    public Faction GetFaction() => _data.Faction;
    
    // Base stats.
    public int GetBaseHealthPoints() => _data.BaseHealthPoints;
    public int GetBaseSpeed() => _data.BaseInitiative;
    public int GetBaseDamage() => _data.BaseDamage;
    public int GetBaseMovementPoints() => _data.BaseMovementPoints;
    
    // Current stats.
    public int GetHealthPoints() => _data.CurrentHealthPoints;
    public int GetSpeed() => _currentSpeed;
    public int GetDamage() => _currentDamage;
    public int GetMovementPoints() => _currentMovementPoints;
    public Vector2Int GetCurrentTileIndex() => _currentTileIndex;
    public CombatGridTile GetCurrentTileComponent() =>
        CombatManager._instance.GetTileComponent(_currentTileIndex.x, _currentTileIndex.y);
    
    // Abilities.
    public AbilityHandler GetAbilityHandler() => _abilityHandler;
    public IReadOnlyList<Ability> GetAvailableAbilities() => _data.AvailableAbilities;

    // Base stats.
    public void SetCharacterClass(CharacterClass characterClass) => _data.SetCharacterClass(characterClass);
    public void SetFaction(Faction faction) => _data.SetFaction(faction);
    public void SetBaseHealthPoints(int healthPoints) => _data.SetBaseHealthPoints(healthPoints);
    public void SetBaseInitiative(int initiative) => _data.SetBaseInitiative(initiative);
    public void SetBaseDamage(int damage) => _data.SetBaseDamage(damage);
    public void SetBaseMovementPoints(int movementPoints) => _data.SetBaseMovementPoints(movementPoints);
    
    // Current stats.
    public void SetCurrentHealthPoints(int healthPoints) => _data.SetCurrentHealthPoints(healthPoints);
    public void SetCurrentSpeed(int speed) => _currentSpeed = speed;
    public void SetCurrentDamage(int damage) => _currentDamage = damage;
    public void SetCurrentMovementPoints(int movementPoints) => _currentMovementPoints = movementPoints;
    public void SetCurrentTileIndex(Vector2Int tileIndex) => _currentTileIndex = tileIndex;

    public void StartAbilityCooldown(Ability ability)
    {
        if (GetAvailableAbilities().Contains(ability) && !_currentCooldowns.ContainsKey(ability))
        {
            _currentCooldowns.Add(ability, ability.GetCooldown());
        }
    }

    public void UpdateAbilityCooldowns()
    {
        var finishedAbilities = new List<Ability>();
        
        foreach (var ability in _currentCooldowns.Keys)
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
        _currentSpeed = _data.BaseInitiative;
        _currentDamage = _data.BaseDamage;
        _currentMovementPoints = _data.BaseMovementPoints;
    }
    
    public void TakeDamage(int damage)
    {
        _data.SetCurrentHealthPoints(_data.CurrentHealthPoints - damage);
        if (_data.CurrentHealthPoints <= 0)
        {
            // TODO: Character dies.
        }
    }

    public void Heal(int healAmount)
    {
        _data.Heal(healAmount);
    }
    
    private bool IsMoving()
    {
        if (_navMeshAgent.pathPending)
        {
            return true; 
        }

        return _navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance
               || _navMeshAgent.velocity.sqrMagnitude > 0.03f;
    }

    /// <summary>
    /// Sets a new target move location.
    /// </summary>
    /// <param name="positions">The grid points to move to.</param>
    public void SetMovePath(Vector3[] positions)
    {
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, positions[^1], NavMesh.AllAreas, path);
        _navMeshAgent.SetPath(path);
    }
    
    /// <summary>
    /// Sets a new target move location.
    /// </summary>
    /// <param name="target">The position to move to.</param>
    public void SetMoveTarget(CombatGridTile target)
    {
        _navMeshAgent.SetDestination(target.GetTilePosition());
    }

    /// <summary>
    /// Sets a new target move location.
    /// </summary>
    /// <param name="target">The position to move to.</param>
    public void SetMoveTarget(Vector3 target)
    {
        _navMeshAgent.SetDestination(target);
    }
}

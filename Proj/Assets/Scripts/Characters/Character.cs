using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum Faction { Friendly, Enemy }

public class CharacterData
{
    public ClassData ClassData;
    public CharacterClass CharacterClass;
    public Faction Faction;
    
    public int BaseHealthPoints;
    public int BaseSpeed;
    public int BaseDamage;
    public int BaseMovementPoints;
    
    public List<Ability> AvailableAbilities;

    public CharacterData(ClassData classData, Faction faction)
    {
        ClassData = classData;
        Faction = faction;
        InitializeClassData();
        InitializeAbilities();
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
        BaseHealthPoints = UnityEngine.Random.Range(ClassData.minHealthPoints, ClassData.maxHealthPoints + 1);
        BaseSpeed =  UnityEngine.Random.Range(ClassData.minSpeed, ClassData.maxSpeed + 1);
        BaseDamage = UnityEngine.Random.Range(ClassData.minDamage, ClassData.maxDamage + 1);
        BaseMovementPoints = UnityEngine.Random.Range(ClassData.minMovementPoints, ClassData.maxMovementPoints + 1);
        CharacterClass = ClassData.characterClass;
    }

    /// <summary>
    /// Sets all the abilities available to the character.
    /// </summary>
    private void InitializeAbilities()
    {
        AvailableAbilities = ClassData.abilities;
    }
}

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(NavMeshAgent))]
public class Character : MonoBehaviour
{
    public const int MOVEMENT_POINTS = 5;
    
    // TODO: Traits.
    
    [Header("Current stats")]
    [SerializeField] private int _currentHealthPoints;
    [SerializeField] private int _currentSpeed;
    [SerializeField] private int _currentDamage;
    [SerializeField] private int _currentMovementPoints;
    
    [Header("Abilities")]
    private AbilityHandler _abilityHandler;
    private List<Ability> _availableAbilities;
    private Dictionary<Ability, int> _currentCooldowns = new();
    
    [Header("Misc")]
    private CharacterData _data;
    [SerializeField] private Vector2Int _currentTileIndex;
    private NavMeshAgent _navMeshAgent;
    
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
        _currentHealthPoints = _data.BaseHealthPoints;
        _currentSpeed = _data.BaseSpeed;
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
    
    public AbilityHandler GetAbilityHandler()
    {
        return _abilityHandler;
    }

    public ClassData GetClassData()
    {
        return _data.ClassData;
    }
    
    public CharacterClass GetCharacterClass()
    {
        return _data.CharacterClass;
    }

    public Faction GetFaction()
    {
        return _data.Faction;
    }

    public int GetBaseHealthPoints()
    {
        return _data.BaseHealthPoints;
    }

    public int GetBaseSpeed()
    {
        return _data.BaseSpeed;
    }

    public int GetBaseDamage()
    {
        return _data.BaseDamage;
    }

    public int GetBaseMovementPoints()
    {
        return _data.BaseMovementPoints; 
    }

    public int GetHealthPoints()
    {
        return _currentHealthPoints;
    }

    public int GetSpeed()
    {
        return _currentSpeed;
    }

    public int GetDamage()
    {
        return _currentDamage;
    }

    public int GetMovementPoints()
    {
        return _currentMovementPoints;
    }

    public Vector2Int GetCurrentTileIndex()
    {
        return _currentTileIndex;
    }

    public CombatGridTile GetCurrentTileComponent()
    {
        return CombatManager._instance.GetTileComponent(_currentTileIndex.x, _currentTileIndex.y);
    }

    public List<Ability> GetAvailableAbilities()
    {
        return _availableAbilities;
    }

    public void SetCharacterClass(CharacterClass characterClass)
    {
        _data.CharacterClass = characterClass;
    }

    public void SetFaction(Faction faction)
    {
        _data.Faction = faction;
    }

    public void SetBaseHealthPoints(int healthPoints)
    {
        _data.BaseHealthPoints = healthPoints;
    }

    public void SetBaseSpeed(int initiative)
    {
        _data.BaseSpeed = initiative;
    }

    public void SetBaseDamage(int damage)
    {
        _data.BaseDamage = damage;
    }
    public void SetBaseMovementPoints(int movementPoints)
    {
        _data.BaseMovementPoints = movementPoints;
    }
    public void SetCurrentHealthPoints(int healthPoints)
    {
        _currentHealthPoints = healthPoints;
    }

    public void SetCurrentSpeed(int speed)
    {
        _currentSpeed = speed;
    }

    public void SetCurrentDamage(int damage)
    {
        _currentDamage = damage;    
    }

    public void SetCurrentMovementPoints(int movementPoints)
    {
        _currentMovementPoints = movementPoints;
    }

    public void SetCurrentTileIndex(Vector2Int tileIndex)
    {
        _currentTileIndex = tileIndex;
    }

    public void StartAbilityCooldown(Ability ability)
    {
        if (_availableAbilities.Contains(ability) && !_currentCooldowns.ContainsKey(ability))
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
    /// Generates a new friendly character based on the character data.
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
        _currentHealthPoints = _data.BaseHealthPoints;
        _currentSpeed = _data.BaseSpeed;
        _currentDamage = _data.BaseDamage;
        _currentMovementPoints = _data.BaseMovementPoints;
        _availableAbilities = new List<Ability>(data.AvailableAbilities);
    }
    
    public void TakeDamage(int damage)
    {
        _currentHealthPoints -= damage;
        if (_currentHealthPoints <= 0)
        {
            // TODO: Character dies.
        }
    }

    public void Heal(int healAmount)
    {
        _currentHealthPoints = Mathf.Min(_currentHealthPoints + healAmount, _data.BaseHealthPoints);
    }

    public bool IsMoving()
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
    /// <param name="target">The grid to move to.</param>
    public void SetMoveTarget(CombatGridTile target)
    {
        SetMoveTarget(target.GetTilePosition());
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

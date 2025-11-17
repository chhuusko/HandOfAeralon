using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum Faction { Friendly, Enemy }

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(NavMeshAgent))]
public class Character : MonoBehaviour
{
    public const int MOVEMENT_POINTS = 5;
    
    [Header("Character")]
    [SerializeField] private ClassData _classData;
    [SerializeField] private CharacterClass _characterClass;
    [SerializeField] private Faction _faction;
    
    [Header("Base stats")]
    [SerializeField] private int _baseHealthPoints;
    [SerializeField] private int _baseSpeed;
    [SerializeField] private int _baseDamage;

    [SerializeField] private int _baseMovementPoints;
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
    [SerializeField] private Vector2Int _currentTileIndex;
    private NavMeshAgent _navMeshAgent;

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
        _currentHealthPoints = _baseHealthPoints;
        _currentSpeed = _baseSpeed;
        _currentDamage = _baseDamage;
        _currentMovementPoints = _baseMovementPoints;
    }

    public void Update()
    {
        // NOTE (CJ & Carl): Testkod f�r animationer
        Animator animator = GetComponent<Animator>();
        if(animator)
        {
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
        return _classData;
    }
    
    public CharacterClass GetCharacterClass()
    {
        return _characterClass;
    }

    public Faction GetFaction()
    {
        return _faction;
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
        _characterClass = characterClass;
    }

    public void SetFaction(Faction faction)
    {
        _faction = faction;
    }

    public void SetBaseHealthPoints(int healthPoints)
    {
        _baseHealthPoints = healthPoints;
    }

    public void SetBaseSpeed(int initiative)
    {
        _baseSpeed = initiative;
    }

    public void SetBaseDamage(int damage)
    {
        _baseDamage = damage;
    }

    public void SetMovementPoints(int movementPoints)
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
    
    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();

        // Only set class values for friendlies.
        if (_faction == Faction.Friendly)
        {
            InitializeClassData();
            InitializeAbilities();
        }
    }
    
    /// <summary>
    /// Generates a new friendly character based on the class data.
    /// </summary>
    private void InitializeClassData()
    {
        if (_classData == null)
        {
            return;
        }
            
        // Set values from class data.
        _currentHealthPoints = _baseHealthPoints = UnityEngine.Random.Range(_classData.minHealthPoints, _classData.maxHealthPoints + 1);
        _currentSpeed = _baseSpeed =  UnityEngine.Random.Range(_classData.minSpeed, _classData.maxSpeed + 1);
        _currentDamage = _baseDamage = UnityEngine.Random.Range(_classData.minDamage, _classData.maxDamage + 1);
        _currentMovementPoints = _baseMovementPoints = UnityEngine.Random.Range(_classData.minMovmementPoints, _classData.maxMovmentPoints + 1);
        _characterClass = _classData.characterClass;
    }

    /// <summary>
    /// Sets all the abilities available to the character.
    /// </summary>
    private void InitializeAbilities()
    {
        if (CombatManager._instance == null)
        {
            return;
        }
        _availableAbilities = new List<Ability>();
        _availableAbilities = CombatManager._instance.GetClassAbilities(_characterClass);
    }

    private IEnumerator WaitForCombatManager()
    {
        yield return new WaitUntil(() => CombatManager._instance != null);
        InitializeAbilities();
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
        _currentHealthPoints = Mathf.Min(_currentHealthPoints + healAmount, _baseHealthPoints);
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

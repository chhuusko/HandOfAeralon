using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum Faction { Friendly, Enemy }

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(NavMeshAgent))]
public class Character : MonoBehaviour
{
    [Header("Character")]
    [SerializeField] private ClassData _classData;
    [SerializeField] private CharacterClass _characterClass;
    [SerializeField] private Faction _faction;
    
    [Header("Base stats")]
    [SerializeField] private int _baseHealthPoints;
    [SerializeField] private int _baseSpeed;
    [SerializeField] private int _baseDamage;
    // TODO: Traits.
    
    [Header("Current stats")]
    [SerializeField] private int _currentHealthPoints;
    [SerializeField] private int _currentSpeed;
    [SerializeField] private int _currentDamage;
    
    [Header("Misc")]
    [SerializeField] private Vector2Int _currentTileIndex;
    private List<Ability> _availableAbilities;
    private NavMeshAgent _navMeshAgent;


    public void Update()
    {
        if(IsMoving())
        {
            GetComponent<Animator>().SetBool("IsMoving", true);
        }
        else
        {
            GetComponent<Animator>().SetBool("IsMoving", false);
        }
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
    
    public void SetCurrentTileIndex(Vector2Int tileIndex)
    {
        _currentTileIndex = tileIndex;
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
            return true; 

        return _navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance
               || _navMeshAgent.velocity.sqrMagnitude > 0.3f;
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

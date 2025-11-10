using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public enum Faction { Friendly, Enemy }

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(NavMeshAgent))]
public class Character : MonoBehaviour
{
    [Header("Base stats")]
    [SerializeField] private ClassData _classData;
    [SerializeField] private CharacterClass _characterClass;
    [SerializeField] private Faction _faction;
    [SerializeField] private int _baseHealthPoints;
    [SerializeField] private int _baseInitiative;
    // TODO: Traits.
    
    [Header("Current stats")]
    [SerializeField] private int _currentHealthPoints;
    [SerializeField] private int _currentInitiative;
    
    [SerializeField] private Vector2Int _currentTileIndex;
    
    private Vector3 _movePosition;
    private bool _bShouldMove;
    private List<Ability> _availableAbilities;
    
    private Rigidbody _rigidbody;
    private NavMeshAgent _navMeshAgent;

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

    public int GetInitiative()
    {
        return _currentInitiative;
    }

    public Vector2Int GetCurrentTileIndex()
    {
        return _currentTileIndex;
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

    public void SetBaseInitiative(int initiative)
    {
        _baseInitiative = initiative;
    }
    
    public void SetCurrentTileIndex(Vector2Int tileIndex)
    {
        _currentTileIndex = tileIndex;
    }
    
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        
        InitializeClassData();
        InitializeAbilities();
    }
    
    private void InitializeClassData()
    {
        // Set values from class data.
        _currentHealthPoints = _baseHealthPoints = UnityEngine.Random.Range(_classData.minHealthPoints, _classData.maxHealthPoints);
        _currentInitiative = _baseInitiative =  UnityEngine.Random.Range(_classData.minInitiative, _classData.maxInitiative);
        _characterClass = _classData.characterClass;
    }

    /// <summary>
    /// Sets all the abilities available to the character.
    /// </summary>
    private void InitializeAbilities()
    {
        _availableAbilities = CombatManager._instance.GetClassAbilities(_characterClass);
    }
    
    public void TakeDamage(int damage)
    {
        _currentHealthPoints -= damage;
    }

    public void Heal(int healAmount)
    {
        _currentHealthPoints += Mathf.Max(_currentHealthPoints + healAmount, _baseHealthPoints);
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

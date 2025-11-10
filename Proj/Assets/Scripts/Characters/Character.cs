using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum CharacterClass { Barbarian, Wizard, Rogue, Bard }

public enum Faction { Friendly, Enemy }

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(NavMeshAgent))]
public class Character : MonoBehaviour
{
    private const float MOVE_SPEED = 5f;
    
    [SerializeField] private CharacterClass _characterClass;
    [SerializeField] private Faction _faction;
    [SerializeField] private int _healthPoints;
    [SerializeField] private int _initiative;
    [SerializeField] private Vector2Int _currentTileIndex;
    // TODO: Traits.
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
        return _healthPoints;
    }

    public int GetInitiative()
    {
        return _initiative;
    }
    
    public void SetCurrentTileIndex(Vector2Int tileIndex)
    {
        _currentTileIndex = tileIndex;
    }
    
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        InitializeAbilities();
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
        _healthPoints -= damage;
    }

    public void Heal(int healAmount)
    {
        _healthPoints += healAmount;
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

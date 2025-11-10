using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum CharacterClass { Barbarian, Wizard, Rogue, Bard }

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(NavMeshAgent))]
public class Character : MonoBehaviour
{
    private const float MOVE_SPEED = 5f;
    
    [SerializeField] private CharacterClass _characterClass;
    [SerializeField] private int _healthPoints;
    [SerializeField] private int _initiative;
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

    public int GetHealthPoints()
    {
        return _healthPoints;
    }

    public int GetInitiative()
    {
        return _initiative;
    }
    
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void InitializeAbilities()
    {
        // _availableAbilities = CombatManager.;
    }
    
    public void TakeDamage(int damage)
    {
        _healthPoints -= damage;
    }

    public void Heal(int healAmount)
    {
        _healthPoints += healAmount;
    }

    public void SetMoveTarget(CombatGridTile target)
    {
        SetMoveTarget(target.GetTilePosition());
    }

    public void SetMoveTarget(Vector3 target)
    {
        _navMeshAgent.SetDestination(target);
    }
}

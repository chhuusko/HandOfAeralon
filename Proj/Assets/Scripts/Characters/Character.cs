using System;
using UnityEngine;
using UnityEngine.AI;

public enum CharacterClass { Barbarian, Wizard, Rogue, Bard }

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(NavMeshAgent))]
public class Character : MonoBehaviour
{
    private const float MOVE_SPEED = 5f;
    
    [SerializeField] private CharacterClass _characterClass;
    private int _healthPoints;
    private int _speed;
    // TODO: Traits.
    private Vector3 _movePosition;
    private bool _bShouldMove;
    
    private Rigidbody _rigidbody;
    private NavMeshAgent _navMeshAgent;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void FixedUpdate()
    {
        if (_bShouldMove)
        {
            Move();
        }
    }

    public CharacterClass GetCharacterClass()
    {
        return _characterClass;
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
        // Start moving.
        _movePosition = target;
        _bShouldMove = true;
        
        // Look toward goal.
        Vector3 lookDirection = _movePosition - transform.position;
        transform.rotation = Quaternion.LookRotation(lookDirection);
    }

    private void Move()
    {
        // Moves toward goal.
        Vector3 targetVelocity = transform.forward * MOVE_SPEED;
        Vector3 currentVelocity = _rigidbody.linearVelocity;
        Vector3 velocityChange = targetVelocity - currentVelocity;
            
        _rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
    }
}

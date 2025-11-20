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
    public const float DEATH_COOLDOWN = 1f;
    
    // TODO: Traits.
    
    [Header("Current stats")]
    [SerializeField] private int _currentInitiative;
    [SerializeField] private int _currentDamage;
    [SerializeField] private int _currentMovementPoints;
    
    [Header("Abilities")]
    private AbilityHandler _abilityHandler;
    private Dictionary<Ability, int> _currentCooldowns = new();
    
    [Header("Misc")]
    [SerializeField] private CharacterData _data;
    [SerializeField] private Vector2Int _currentTileIndex;
    private bool _bIsMoving;
    public CharacterData Data => _data;
    
    private void Start()
    {
        if (!TryGetComponent(out _abilityHandler))
        {
            Debug.LogError("Character is missing AbilityHandler component!");
            return;
        }
    }

    public void ResetCharacter() // Endast för testkörning (JLW), tills dess att turtagningen fungerar som tänkt
    {
        _currentInitiative = _data.BaseInitiative;
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
    public int GetInitiative() => _currentInitiative;
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
    public void SetCurrentInitiative(int initiative) => _currentInitiative = initiative;
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
        _currentInitiative = _data.BaseInitiative;
        _currentDamage = _data.BaseDamage;
        _currentMovementPoints = _data.BaseMovementPoints;
    }
    
    public void TakeDamage(int damage)
    {
        _data.SetCurrentHealthPoints(_data.CurrentHealthPoints - damage);
        
        Debug.Log($"Taking {damage} damage. New health: {GetHealthPoints()}");
        
        if (_data.CurrentHealthPoints <= 0)
        {
            StartCoroutine(RemoveCharacter());
        }
    }

    private IEnumerator RemoveCharacter()
    {
        CombatManager._instance.CharacterDied(this);
        // TODO: Play animation.
        yield return new WaitForSeconds(DEATH_COOLDOWN);
        Destroy(gameObject);
    }

    public void Heal(int healAmount)
    {
        _data.Heal(healAmount);
    }
    
    public bool IsMoving()
    {
        return _bIsMoving;

        /*
        if (_navMeshAgent.pathPending)
        {
            return true; 
        }

        return _navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance
               || _navMeshAgent.velocity.sqrMagnitude > 0.03f;
        */
    }

    public IEnumerator MoveAlongPath(List<GameObject> tiles)
    {
        if (tiles == null || tiles.Count == 0)
        {
            DebugLog.JLWLog($"Character.cs | MoveAlongPath called with an empty list!");
            yield break;
        }

        _bIsMoving = true;

        float moveSpeed = 4f; // Måste matcha animationerna

        foreach (var tile in tiles)
        {
            Vector3 targetPos = tile.transform.position;

            Vector3 direction = (targetPos - transform.position).normalized;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }

            DebugLog.JLWLog($"Character.cs | {this.name} moving towards {targetPos}");

            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPos,
                    moveSpeed * Time.deltaTime
                );

                yield return null;
            }

            transform.position = targetPos;

            //GetComponent<CombatGridTile>().SetOccupant(this.gameObject);
        }

        _bIsMoving = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class EnemyAI : MonoBehaviour
{
    public UnityEvent AIEndTurn;

    [SerializeField] private Faction controlledFaction = Faction.Enemy;
    private Character _currentCharacter = null;
    private GameObject _currentTile = null;
    private int _currentMoveRange = 0;
    private int _currentAttackRange = 0;
    private AbilityHandler _currentAbilityHandler = null;
    private IReadOnlyList<Ability> _currentAbilities;
    private List<CombatGridTile> _movePath = new();
    private Character _targetCharacter = null;
    private GameObject _closestOpponentTile = null;


    private void OnEnable()
    {
        CombatEventManager.OnEnterCombatStateTakeTurn += StartTurn;
    }

    private void OnDisable()
    {
        CombatEventManager.OnEnterCombatStateTakeTurn -= StartTurn;
    }

    // NOTE (Calle): Added this for test, and executing from CombatEventManager.OnEnterCombatStateTakeTurn
    private void StartTurn(Character character)
    {
        OnTurnStart();
    }

    private void OnTurnStart()
    {
        if (OnTurnStartProper())
        {
            _movePath = FindPath(_currentTile, _closestOpponentTile)
                .Select(obj => obj.GetComponent<CombatGridTile>())
                .Where(ch => ch != null)
                .ToList();

            if (_movePath == null || _movePath.Count == 0)
            {
                DebugLog.JLWLog($"EnemyAI.cs | _movePath NOT FOUND!");
            }

            if (_currentCharacter.CanMove)
            {
                _currentCharacter.GetComponent<CharacterMovement>().ForceCustomPath(_movePath);
            }
            else
            {
                TryAttack(_currentCharacter, _targetCharacter);
                EndTurn();
                return;
            }

            StartCoroutine(WaitForMovementCompletion());
        }
    }

    private bool OnTurnStartProper()
    {
        _currentCharacter = CombatManager._instance.GetCombatTurnOrder().GetActiveCharacter();
        if (_currentCharacter.GetFaction() == Faction.Friendly)
        {
            return false;
        }

        if (_currentCharacter == null || _currentCharacter.GetFaction() != controlledFaction)
        {
            return false;
        }

        //DebugLog.JLWLog($"EnemyAI.cs | {this.name}'s turn.");

        _currentMoveRange = _currentCharacter.GetMovementPoints();
        _currentAttackRange = 1;

        // Get abilities + handler
        _currentAbilityHandler = _currentCharacter.GetAbilityHandler();
        DebugLog.JLWLog($"EnemyAI.cs | AbilityHandler: {_currentAbilityHandler}");
        _currentAbilities = _currentCharacter.GetAvailableAbilities();
        if (_currentAbilities == null || _currentAbilities.Count == 0)
        {
            DebugLog.JLWLog($"EnemyAI.cs | No abilities found!");
        }
        else
        {
            foreach (var ability in _currentAbilities)
            {
                DebugLog.JLWLog($"EnemyAI.cs | Abilities found: {ability.name}");
            }
        }

        _currentTile = _currentCharacter.GetCurrentTileComponent().gameObject;
        if (_currentTile == null)
        {
            DebugLog.JLWLog($"EnemyAI.cs | _currentTile NOT FOUND!");
            return false;
        }

        _targetCharacter = GetClosestOpponentCharacter(_currentCharacter);
        if (_targetCharacter == null)
        {
            DebugLog.JLWLog($"EnemyAI.cs | _targetCharacter NOT FOUND IN SCENE!");
            return false;
        }

        _closestOpponentTile = _targetCharacter.GetCurrentTileComponent().gameObject;
        if (_closestOpponentTile == null)
        {
            DebugLog.JLWLog($"EnemyAI.cs | _closestOpponentTile NOT FOUND!");
            return false;
        }

        return true;
    }

    private IEnumerator WaitForMovementCompletion()
    {
        yield return new WaitWhile(() => _currentCharacter.IsMoving());

        _currentTile = _currentCharacter.GetCurrentTileComponent().gameObject;
        if (_currentTile != null)
        {
            TryAttack(_currentCharacter, _targetCharacter);
        }
        else
        {
            DebugLog.JLWLog($"EnemyAI.cs | currentTile NOT FOUND!");
        }

        EndTurn();
    }

    private void EndTurn()
    {
        ResetVariables();
        AIEndTurn.Invoke();
    }

    private void ResetVariables()
    {
        _currentCharacter = null;
        _currentTile = null;
        _currentMoveRange = 0;
        _currentAttackRange = 0;
        _currentAbilityHandler = null;
        _targetCharacter = null;
        _closestOpponentTile = null;
        _movePath = new();
    }

    private Character GetClosestOpponentCharacter(Character currentCharacter)
    {
        Character result = null;
        List<Character> opponentCharacters = new();

        if (controlledFaction == Faction.Enemy)
        {
            opponentCharacters = CombatGrid
                ._instance.GetAllFriendlyCharacters()
                .Select(obj => obj.GetComponent<Character>())
                .Where(ch => ch != null)
                .ToList();
        }
        else if (controlledFaction == Faction.Friendly)
        {
            opponentCharacters = CombatGrid
                ._instance.GetAllEnemyCharacters()
                .Select(obj => obj.GetComponent<Character>())
                .Where(ch => ch != null)
                .ToList();
        }

        float min = float.MaxValue;
        foreach (var opponentCharacter in opponentCharacters)
        {
            float distance = Vector3.Distance(currentCharacter.transform.position, opponentCharacter.transform.position);
            if (distance < min)
            {
                min = distance;
                result = opponentCharacter;
            }
        }

        return result;
    }

    private void TryAttack(Character attacker, Character target)
    {
        if (!attacker.CanAttack)
        {
            return;
        }

        Ability chosenAbility = _currentAbilities[Random.Range(0, _currentAbilities.Count)];
        CombatGridTile targetTile = target.GetCurrentTileComponent();

        _currentAbilityHandler.SetPendingAbility(chosenAbility);
        DebugLog.JLWLog($"EnemyAI.cs | {chosenAbility.name} set as pending ability.");
        _currentAbilityHandler.UseAbility(chosenAbility, targetTile);
        DebugLog.JLWLog($"EnemyAI.cs | {chosenAbility.name} cast on tile {targetTile.GetTileIndex()}");

        if (GridExplorer._instance.ChebyshevDistance(attacker.GetCurrentTileIndex(), target.GetCurrentTileIndex()) <= _currentAttackRange)
        {
            Vector3 direction = (target.transform.position - _currentCharacter.transform.position).normalized;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.0001f)
            {
                _currentCharacter.transform.rotation = Quaternion.LookRotation(direction);
            }

            target.TakeDamage(_currentCharacter.GetDamage());
            return;
        }
    }

    private List<GameObject> FindPath(GameObject currentTile, GameObject opponentTile)
    {
        List<GameObject> result = new();
        List<GameObject> path = GridExplorer._instance.FindPathAStar(currentTile, opponentTile);

        if (path == null || path.Count <= 1)
        {
            DebugLog.JLWLog($"EnemyAI.cs | No path found from {currentTile.GetComponent<CombatGridTile>().GetTileIndex()} to {opponentTile.GetComponent<CombatGridTile>().GetTileIndex()}");
            return new List<GameObject>();
        }

        for (int i = 1; i < path.Count && i <= _currentMoveRange; i++)
        {
            GameObject tile = path[i];
            int distToEnemy = GridExplorer._instance.ManhattanDistance(tile.GetComponent<CombatGridTile>().GetTileIndex(), opponentTile.GetComponent<CombatGridTile>().GetTileIndex());

            if (distToEnemy == _currentAttackRange)
            {
                result.Add(tile);
                return result;
            }

            if (distToEnemy > 0)
            {
                result.Add(tile);
            }
        }

        return result;
    }
}

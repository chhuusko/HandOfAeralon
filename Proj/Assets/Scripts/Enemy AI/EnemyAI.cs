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
    private int _currentMoveRange = 0;
    private int _currentAttackRange = 0;
    private List<GameObject> _movePath = new();
    private Character _targetCharacter = null;
    private GameObject _closestOpponentTile = null;

    // Only for testing
    [SerializeField] private bool _bDebug = false;
    [SerializeField] private bool _bAutoPlay = false;
    // End of only for testing

    void Start()
    {
        CombatManager._instance.TurnStart.AddListener(OnTurnStart);

        // Only for testing
        if (_bAutoPlay)
        {
            StartCoroutine(Autoplay());
        }
        // End of only for testing
    }

    private IEnumerator Autoplay() // Only for testing
    {
        while (_bAutoPlay)
        {
            yield return new WaitForSeconds(3f);

            if (controlledFaction == Faction.Enemy)
            {
                List<Character> allCharacters = CombatGrid._instance.GetAllCharacters()
                .Select(obj => obj.GetComponent<Character>())
                .Where(ch => ch != null)
                .ToList();

                foreach (var character in allCharacters)
                {
                    character.SetBaseHealthPoints(Random.Range(1, 4));
                    character.SetBaseInitiative(Random.Range(1, 5));
                    character.SetBaseDamage(Random.Range(1, 5));
                    character.ResetCharacter();
                    character.SetCurrentMovementPoints(2);
                }

                if (_bDebug) DebugLog.JLWLog($"EnemyAI.cs | All character stats randomized!");
            }

            OnTurnStart();
        }
    }

    private void OnTurnStart()
    {
        // Initialization & null checks
        //_currentCharacter = CombatManager._instance.GetNextTurnCharacter().GetComponent<Character>();
        _currentCharacter = CombatManager._instance.GetCombatTurnOrder().GetActiveCharacter();
        if (_currentCharacter == null || _currentCharacter.GetFaction() != controlledFaction)
        {
            if (_bDebug) DebugLog.JLWLog($"EnemyAI.cs | Not {this.name}'s turn...");
            return;
        }
        DebugLog.JLWLog($"EnemyAI.cs | {this.name}'s turn.");
        if (_bDebug) DebugLog.JLWLog($"EnemyAI.cs | _currentCharacter: {_currentCharacter.name}");
        _currentMoveRange = _currentCharacter.GetMovementPoints();
        _currentAttackRange = 1; // Bör vara -> occupantCharacter.GetAttackRange()
        if (_bDebug) DebugLog.JLWLog($"EnemyAI.cs | _currentMoveRange: {_currentMoveRange}, _currentAttackRange: {_currentAttackRange}");

        GameObject currentTile = _currentCharacter.GetCurrentTileComponent().gameObject;
        if (currentTile == null)
        {
            DebugLog.JLWLog($"EnemyAI.cs | currentTile NOT FOUND!");
            return;
        }

        _targetCharacter = GetClosestOpponentCharacter(_currentCharacter);
        if (_targetCharacter == null)
        {
            DebugLog.JLWLog($"EnemyAI.cs | _targetCharacter NOT FOUND IN SCENE!");
            return;
        }
        if (_bDebug) DebugLog.JLWLog($"EnemyAI.cs | _targetCharacter: {_targetCharacter.name}");

        _closestOpponentTile = _targetCharacter.GetCurrentTileComponent().gameObject;
        if (_closestOpponentTile == null)
        {
            DebugLog.JLWLog($"EnemyAI.cs | _closestOpponentTile NOT FOUND!");
            return;
        }

        // Move and attack
        _movePath = FindPath(currentTile, _closestOpponentTile);
        if (_movePath == null || _movePath.Count == 0)
        {
            DebugLog.JLWLog($"EnemyAI.cs | _movePath NOT FOUND!");
        }

        StartCoroutine(_currentCharacter.MoveAlongPath(_movePath));
        if (_bDebug && _movePath == null && _movePath.Count != 0) DebugLog.JLWLog($"EnemyAI.cs | Moving {_currentCharacter.name} to {_movePath[_movePath.Count - 1].GetComponent<CombatGridTile>().GetTileIndex()}");

        StartCoroutine(WaitForMovementCompletion());
    }

    private IEnumerator WaitForMovementCompletion()
    {
        yield return new WaitWhile(() => _currentCharacter.IsMoving());

        GameObject currentTile = _currentCharacter.GetCurrentTileComponent().gameObject;
        if (currentTile != null)
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
        _currentMoveRange = 0;
        _currentAttackRange = 0;
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
        if (GridExplorer._instance.ChebyshevDistance(attacker.GetCurrentTileIndex(), target.GetCurrentTileIndex()) <= _currentAttackRange)
        {
            Vector3 direction = (target.transform.position - _currentCharacter.transform.position).normalized;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.0001f)
            {
                _currentCharacter.transform.rotation = Quaternion.LookRotation(direction);
            }

            target.TakeDamage(_currentCharacter.GetDamage()); // Bör använda en ability istället
            if (_bDebug) DebugLog.JLWLog($"EnemyAI.cs | {_currentCharacter.name} strikes {target.name} for {_currentCharacter.GetDamage()} damage.");
            return;
        }

        if (_bDebug) DebugLog.JLWLog($"EnemyAI.cs | {target.name} is out of attack range!");
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

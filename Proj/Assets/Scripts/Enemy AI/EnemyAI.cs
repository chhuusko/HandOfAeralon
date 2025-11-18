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
    }

    private IEnumerator Autoplay() // Only for testing
    {
        while (_bAutoPlay)
        {
            yield return new WaitForSeconds(2f);

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
        _currentCharacter = CombatManager._instance.GetNextTurnCharacter().GetComponent<Character>();
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

        Character closestOpponentCharacter = GetClosestOpponentCharacter(_currentCharacter);
        if (closestOpponentCharacter == null)
        {
            DebugLog.JLWLog($"EnemyAI.cs | closestOpponentCharacter NOT FOUND IN SCENE!");
            return;
        }
        if (_bDebug) DebugLog.JLWLog($"EnemyAI.cs | closestOpponentCharacter: {closestOpponentCharacter.name}");

        GameObject closestOpponentTile = closestOpponentCharacter.GetCurrentTileComponent().gameObject;
        if (closestOpponentTile == null)
        {
            DebugLog.JLWLog($"EnemyAI.cs | closestOpponentTile NOT FOUND!");
            return;
        }

        // Move and attack
        GameObject targetTile = FindPath(currentTile, closestOpponentTile);
        if (targetTile == null)
        {
            DebugLog.JLWLog($"EnemyAI.cs | targetTile NOT FOUND!");
        }
        _currentCharacter.SetMoveTarget(targetTile.transform.position);
        if (_bDebug) DebugLog.JLWLog($"EnemyAI.cs | Moving {_currentCharacter.name} to {targetTile.GetComponent<CombatGridTile>().GetTileIndex()}");

        if (!TryAttack(targetTile, closestOpponentTile))
        {
            if (_bDebug) DebugLog.JLWLog($"EnemyAI.cs | {closestOpponentCharacter.name} outside attack range.");
            return;
        }

        // End turn
        _currentCharacter = null;
        _currentMoveRange = 0;
        _currentAttackRange = 0;
        AIEndTurn.Invoke();
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

    private bool TryAttack(GameObject fromTile, GameObject toTile)
    {
        Character target = toTile.GetComponent<CombatGridTile>().GetOccupantCharacter();

        if (GridExplorer._instance.ManhattanDistance(fromTile, toTile) <= _currentAttackRange)
        {
            target.TakeDamage(_currentCharacter.GetDamage()); // Bör använda en ability istället
            if (_bDebug) DebugLog.JLWLog($"EnemyAI.cs | {_currentCharacter.name} strikes {target.name} for {_currentCharacter.GetDamage()} damage.");
            return true;
        }

        return false;
    }

    private GameObject FindPath(GameObject currentTile, GameObject opponentTile)
    {
        GameObject result = currentTile;
        List<GameObject> path = GridExplorer._instance.FindPath(currentTile, opponentTile);

        if (path == null || path.Count <= 1)
        {
            DebugLog.JLWLog($"EnemyAI.cs | No path found from {currentTile.GetComponent<CombatGridTile>().GetTileIndex()} to {opponentTile.GetComponent<CombatGridTile>().GetTileIndex()}");
            return currentTile;
        }

        for (int i = 1; i < path.Count && i <= _currentMoveRange; i++)
        {
            GameObject tile = path[i];
            int distToEnemy = GridExplorer._instance.ManhattanDistance(tile, opponentTile);

            if (distToEnemy == _currentAttackRange)
            {
                return tile;
            }

            if (distToEnemy > 0)
            {
                result = tile;
            }
        }

        return result;
    }
}

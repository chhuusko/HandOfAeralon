using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Faction controlsFaction;
    [SerializeField] private Character _testCharacter;
    [SerializeField] private bool _bDebug = false;
    private InputSystem_Actions _inputActions;

    void Start()
    {
        CombatManager._instance.EnemyTurnStart.AddListener(OnEnemyTurnStart);

        _inputActions = new();
        _inputActions.Enable();
        _inputActions.Player.Jump.performed += OnJump;
    }

    void OnJump(InputAction.CallbackContext context) // Only for testing
    {
        if (!_bDebug) return;

        OnEnemyTurnStart();
    }

    private void OnEnemyTurnStart()
    {
        Character currentCharacter = CombatManager._instance.GetNextTurnCharacter().GetComponent<Character>();
        if (currentCharacter == null || currentCharacter.GetFaction() != Faction.Friendly)
        {
            if (_bDebug) Debug.Log($"EnemyAI.cs | Not my turn...");
            return;
        }
        GameObject currentTile = currentCharacter.GetCurrentTileComponent().gameObject;
        if (_bDebug) Debug.Log($"EnemyAI.cs | currentCharacter == {currentCharacter.name}");

        Character closestPlayerCharacter = GetClosestOpponentCharacter(currentCharacter);
        if (_bDebug && _testCharacter != null) closestPlayerCharacter = _testCharacter;
        if (closestPlayerCharacter == null)
        {
            Debug.LogError($"EnemyAI.cs | closestPlayerCharacter NOT FOUND IN SCENE!");
            return;
        }
        GameObject closestPlayerCharacterTile = closestPlayerCharacter.GetCurrentTileComponent().gameObject;
        if (_bDebug) Debug.Log($"EnemyAI.cs | closestPlayerCharacter == {closestPlayerCharacter.name}");

        if (TryAttack(currentTile, closestPlayerCharacterTile))
        {
            return;
        }
        if (_bDebug) Debug.Log($"EnemyAI.cs | {closestPlayerCharacter.name} outside attack range.");

        GameObject chosenTile = FindPath(currentTile, closestPlayerCharacterTile);
        if (chosenTile == null) return;
        currentCharacter.SetMoveTarget(chosenTile.transform.position);
        if (_bDebug) Debug.Log($"EnemyAI.cs | Moving {currentCharacter.name} to {chosenTile.GetComponent<CombatGridTile>().GetTileIndex()}");

        if (TryAttack(chosenTile, closestPlayerCharacterTile))
        {
            return;
        }
        if (_bDebug) Debug.Log($"EnemyAI.cs | {closestPlayerCharacter.name} outside attack range.");
    }

    private Character GetClosestOpponentCharacter(Character currentCharacter)
    {
        List<Character> opponentCharacters = new();

        if (controlsFaction == Faction.Enemy)
        {
            opponentCharacters = CombatManager
            ._instance.GetAllFriendlyCharacters()
            .Select(obj => obj.GetComponent<Character>())
            .Where(ch => ch != null)
            .ToList();
        }
        else if (controlsFaction == Faction.Friendly)
        {
            opponentCharacters = CombatManager
            ._instance.GetEnemyCharacters()
            .Select(obj => obj.GetComponent<Character>())
            .Where(ch => ch != null)
            .ToList();
        }

            float min = float.MaxValue;
        Character closestPlayerCharacter = null;
        foreach (var opponentCharacter in opponentCharacters)
        {
            float distance = Vector3.Distance(currentCharacter.transform.position, opponentCharacter.transform.position);
            if (distance < min)
            {
                min = distance;
                closestPlayerCharacter = opponentCharacter;
            }
        }

        return closestPlayerCharacter;
    }

    private bool TryAttack(GameObject fromTile, GameObject toTile)
    {
        Character myCharacter = fromTile.GetComponent<CombatGridTile>().GetOccupantCharacter();
        Character target = toTile.GetComponent<CombatGridTile>().GetOccupantCharacter();

        if (GridExplorer._instance.ManhattanDistance(
            fromTile,
            toTile)
            <= 2) // Bör vara -> currentCharacter.GetAttackRange()
        {
            target.TakeDamage(myCharacter.GetDamage()); // Bör vara -> currentCharacter.Attack(closestPlayerCharacter);
            if (_bDebug) Debug.Log($"EnemyAI.cs | {myCharacter.name} strikes {target.name} for {myCharacter.GetDamage()} damage.");
            return true;
        }

        return false;
    }

    private GameObject FindPath(GameObject currentTile, GameObject closestOpponentCharacterTile)
    {
        List<GameObject> pathToTarget = GridExplorer._instance.FindPath(currentTile, closestOpponentCharacterTile);
        int moveRange = 3; // Bör vara -> currentCharacter.GetMoveRange()

        if (pathToTarget == null || pathToTarget.Count <= 1)
        {
            Debug.LogError("EnemyAI.cs | No path found to target!");
            return currentTile;
        }

        int targetIndex = Mathf.Min(moveRange, pathToTarget.Count - 1);
        GameObject chosenTile = pathToTarget[targetIndex];

        for (int i = 1; i <= moveRange && i < pathToTarget.Count; i++)
        {
            if (GridExplorer._instance.ManhattanDistance(pathToTarget[i], closestOpponentCharacterTile) > 2) // Bör vara -> currentCharacter.GetAttackRange()
            {
                chosenTile = pathToTarget[i];
            }
        }

        return chosenTile;
    }
}

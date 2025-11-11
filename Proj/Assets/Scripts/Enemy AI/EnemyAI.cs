using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private bool _bDebug = false;
    private InputSystem_Actions _inputActions;

    void Start()
    {
        CombatManager._instance.EnemyTurnStart.AddListener(OnEnemyTurnStart);

        _inputActions = new();
        _inputActions.Enable();
        _inputActions.Player.Jump.performed += OnJump;
    }

    void OnJump(InputAction.CallbackContext context)
    {
        if (_bDebug) OnEnemyTurnStart();
    }

    private void OnEnemyTurnStart()
    {
        Character currentCharacter = CombatManager._instance.GetNextTurnCharacter().GetComponent<Character>();
        if (currentCharacter == null || currentCharacter.GetFaction() != Faction.Enemy)
        {
            if (_bDebug) Debug.Log($"EnemyAI.cs | Not my turn...");
            return;
        }

        List<Character> playerCharacters = CombatManager
            ._instance.GetAllFriendlyCharacters()
            .Select(obj => obj.GetComponent<Character>())
            .Where(ch => ch != null)
            .ToList();

        float min = float.MaxValue;
        Character closestPlayerCharacter = null;
        foreach (var playerCharacter in playerCharacters)
        {
            float distance = Vector3.Distance(currentCharacter.transform.position, playerCharacter.transform.position);
            if (distance < min)
            {
                min = distance;
                closestPlayerCharacter = playerCharacter;
            }
        }

        if (_bDebug) Debug.Log($"EnemyAI.cs | closestPlayerCharacter == {closestPlayerCharacter.name}");

        if (GridExplorer._instance.ManhattanDistance(
            currentCharacter.GetCurrentTileComponent().gameObject, 
            closestPlayerCharacter.GetCurrentTileComponent().gameObject) 
            <= 2) // Bör vara -> currentCharacter.GetAttackRange()
        {
            closestPlayerCharacter.TakeDamage(currentCharacter.GetDamage()); // Bör vara -> currentCharacter.Attack(closestPlayerCharacter);
            return;
        }

        if (_bDebug) Debug.Log($"EnemyAI.cs | {closestPlayerCharacter.name} outside attack range.");

        GameObject currentTile = currentCharacter.GetCurrentTileComponent().gameObject;
        List<GameObject> reachableTiles = GridExplorer._instance.GetTilesInRange(currentTile, 3, true); // Bör vara -> currentCharacter.GetMoveRange()

        min = float.MaxValue;
        GameObject closestTileToTarget = null;
        foreach (var tile in reachableTiles)
        {
            float distance = Vector3.Distance(tile.transform.position, closestPlayerCharacter.transform.position);
            if (distance < min)
            {
                min = distance;
                closestTileToTarget = tile;
            }
        }

        currentCharacter.SetMoveTarget(closestTileToTarget.transform.position);
        if (_bDebug) Debug.Log($"EnemyAI.cs | Moving {currentCharacter.name} to {currentCharacter.GetCurrentTileIndex()}");

        if (GridExplorer._instance.ManhattanDistance(
            currentCharacter.GetCurrentTileComponent().gameObject,
            closestPlayerCharacter.GetCurrentTileComponent().gameObject)
            <= 2) // Bör vara -> currentCharacter.GetAttackRange()
        {
            closestPlayerCharacter.TakeDamage(currentCharacter.GetDamage()); // Bör vara -> currentCharacter.Attack(closestPlayerCharacter);
            return;
        }

        if (_bDebug) Debug.Log($"EnemyAI.cs | {closestPlayerCharacter.name} outside attack range.");
    }
}

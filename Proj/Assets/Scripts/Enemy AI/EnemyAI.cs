using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyAI : MonoBehaviour
{
    private InputSystem_Actions _inputActions;
    [SerializeField] private bool _debug = false;

    void Start()
    {
        _inputActions = new();
        _inputActions.Enable();
        _inputActions.Player.Jump.performed += OnJump;
    }

    void OnJump(InputAction.CallbackContext context) // Byt ut mot "OnTurnStart"
    {
        Character currentCharacter = CombatManager._instance.GetNextTurnCharacter().GetComponent<Character>();
        if (currentCharacter == null || currentCharacter.GetFaction() != Faction.Enemy)
        {
            if (_debug) Debug.Log($"EnemyAI.cs | Not my turn...");
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

        if (_debug) Debug.Log($"EnemyAI.cs | closestPlayerCharacter == {closestPlayerCharacter.name}");

        /* Attempt attack
        if (GridExplorer._instance.ManhattanDistance(currentCharacter.GetCurrentTileComponent().gameObject, closestPlayerCharacter.GetCurrentTileComponent().gameObject) <= currentCharacter.GetAttackRange())
        {
            currentCharacter.Attack(closestPlayerCharacter);
            return;
        }

        if (_debug) Debug.Log($"EnemyAI.cs | {closestPlayerCharacter.name} out of attack range.");
        */

        GameObject currentTile = currentCharacter.GetCurrentTileComponent().gameObject;
        List<GameObject> reachableTiles = GridExplorer._instance.GetTilesInRange(currentTile, currentCharacter.GetMoveRange(), true);

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
        Debug.Log($"EnemyAI.cs | Moving {currentCharacter.name} to {currentCharacter.GetCurrentTileIndex()}");

        /* Attempt attack
        if (GridExplorer._instance.ManhattanDistance(currentCharacter.GetCurrentTileComponent().gameObject, closestPlayerCharacter.GetCurrentTileComponent().gameObject) <= currentCharacter.GetAttackRange())
        {
            currentCharacter.Attack(closestPlayerCharacter);
            return;
        }

        if (_debug) Debug.Log($"EnemyAI.cs | {closestPlayerCharacter.name} out of attack range.");
        */
    }
}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class EnemyAI : MonoBehaviour
{
    public UnityEvent AIEndTurn = new();

    [SerializeField] private Faction controlledFaction = Faction.Enemy;
    [SerializeField] private bool _bDebug = false;
    [SerializeField] private bool _bAutoPlay = false;
    private Character currentCharacter;
    private InputSystem_Actions _inputActions;

    void Start() // Only for testing
    {
        CombatManager._instance.TurnStart.AddListener(OnTurnStart);

        _inputActions = new();
        _inputActions.Enable();
        _inputActions.Player.Jump.performed += OnJump;

        if (_bAutoPlay)
        {
            StartCoroutine(Autoplay());
        }
    }

    void OnJump(InputAction.CallbackContext context) // Only for testing
    {
        if (!_bDebug) return;

        RunDebugTurn();
    }

    private IEnumerator Autoplay() // Only for testing
    {
        while (_bAutoPlay)
        {
            yield return new WaitForSeconds(2f);
            RunDebugTurn();
        }
    }

    void RunDebugTurn() // Only for testing
    {
        if (controlledFaction == Faction.Enemy)
        {
            List<Character> allCharacters = CombatGrid._instance.GetAllCharacters()
            .Select(obj => obj.GetComponent<Character>())
            .Where(ch => ch != null)
            .ToList();

            foreach (var character in allCharacters)
            {
                character.SetBaseHealthPoints(Random.Range(1, 4));
                character.SetBaseSpeed(Random.Range(0, 5));
                character.SetBaseDamage(Random.Range(0, 5));
                character.ResetCharacter();
                character.SetMovementPoints(2);
            }

            Debug.Log($"EnemyAI.cs | All character stats randomized!");
        }

        OnTurnStart();
    }

    private void OnTurnStart()
    {
        currentCharacter = CombatManager._instance.GetNextTurnCharacter().GetComponent<Character>();
        if (currentCharacter == null || currentCharacter.GetFaction() != controlledFaction)
        {
            if (_bDebug) Debug.Log($"EnemyAI.cs | Not my turn...");
            return;
        }
        GameObject currentTile = currentCharacter.GetCurrentTileComponent().gameObject;
        if (_bDebug) Debug.Log($"EnemyAI.cs | currentCharacter == {currentCharacter.name}");

        Character closestOpponentCharacter = GetClosestOpponentCharacter(currentCharacter);
        if (closestOpponentCharacter == null)
        {
            Debug.LogError($"EnemyAI.cs | closestPlayerCharacter NOT FOUND IN SCENE!");
            return;
        }
        GameObject closestOpponentCharacterTile = closestOpponentCharacter.GetCurrentTileComponent().gameObject;
        if (_bDebug) Debug.Log($"EnemyAI.cs | closestPlayerCharacter == {closestOpponentCharacter.name}");

        GameObject chosenTile = FindPath(currentTile, closestOpponentCharacterTile);
        if (chosenTile == null) return;
        currentCharacter.SetMoveTarget(chosenTile.transform.position);
        if (_bDebug) Debug.Log($"EnemyAI.cs | Moving {currentCharacter.name} to {chosenTile.GetComponent<CombatGridTile>().GetTileIndex()}");

        if (TryAttack(chosenTile, closestOpponentCharacterTile))
        {
            return;
        }
        if (_bDebug) Debug.Log($"EnemyAI.cs | {closestOpponentCharacter.name} outside attack range.");

        AIEndTurn.Invoke();
    }

    private Character GetClosestOpponentCharacter(Character currentCharacter)
    {
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
        Character closestOpponentCharacter = null;
        foreach (var opponentCharacter in opponentCharacters)
        {
            float distance = Vector3.Distance(currentCharacter.transform.position, opponentCharacter.transform.position);
            if (distance < min)
            {
                min = distance;
                closestOpponentCharacter = opponentCharacter;
            }
        }

        return closestOpponentCharacter;
    }

    private bool TryAttack(GameObject fromTile, GameObject toTile)
    {
        Character target = toTile.GetComponent<CombatGridTile>().GetOccupantCharacter();

        if (GridExplorer._instance.ManhattanDistance(fromTile, toTile) <= 1) // Bör vara -> currentCharacter.GetAttackRange()
        {
            target.TakeDamage(currentCharacter.GetDamage()); // Bör vara -> currentCharacter.Attack(closestPlayerCharacter);
            if (_bDebug) Debug.Log($"EnemyAI.cs | {currentCharacter.name} strikes {target.name} for {currentCharacter.GetDamage()} damage.");
            return true;
        }

        return false;
    }

    private GameObject FindPath(GameObject currentTile, GameObject opponentTile)
    {
        List<GameObject> path = GridExplorer._instance.FindPath(currentTile, opponentTile);

        int moveRange = currentTile.GetComponent<CombatGridTile>().GetOccupantCharacter().GetMovementPoints();
        int attackRange = 1; // Bör vara -> currentCharacter.GetAttackRange()

        CombatGridTile currentTileScript = currentTile.GetComponent<CombatGridTile>();

        if (path == null || path.Count <= 1)
        {
            Debug.Log("EnemyAI.cs | No path found!");
            return currentTile;
        }

        GameObject bestTile = currentTile;

        for (int i = 1; i < path.Count && i <= moveRange; i++)
        {
            GameObject tile = path[i];
            int distToEnemy = GridExplorer._instance.ManhattanDistance(tile, opponentTile);

            if (distToEnemy == attackRange)
            {
                return tile;
            }

            if (distToEnemy > 0)
            {
                bestTile = tile;
            }
        }

        return bestTile;
    }
}

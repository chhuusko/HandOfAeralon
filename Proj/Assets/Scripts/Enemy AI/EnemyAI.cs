using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyAI : MonoBehaviour
{
    // Måste ha en bild av game state (i.e vart står alla units)
    // Ge kommandon åt sina trupper
    // Attackera närmaste fiende
    // Retirera om låg hälsa
    // Flytta närmre om inget annat vettigt drag kan göras

    /* Nice-to-haves:
     * Olika targets värderas olika -> låg hälsa hög prio, healer hög prio, tank låg prio, inom lethal range superhög prio
     * rng för att slumpa drag
     * olika spelstilar
     * använder olika trupp-klasser på olika sätt
     */

    /* Hur den ska fungera:
     * Loopa igenom alla möjliga tiles att gå till,
     * för varje tile -> kolla om någon attack eller ability kan nå en spelare
     * för varje möjligt drag (move+ability) räkna ut ett värde för det draget
     */

    private InputSystem_Actions _inputActions;

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
            return;
        }

        List<Character> playerTroops = 

        List<DummyCharacter> playerTroops = GameObject
            .FindGameObjectsWithTag("Character")
            .Select(obj => obj.GetComponent<DummyCharacter>())
            .Where(dc => dc != null)
            .ToList();

        float min = float.MaxValue;
        DummyCharacter closestTroop = null;
        foreach (var troop in playerTroops)
        {
            float distance = Vector3.Distance(transform.position, troop.transform.position);
            if (distance < min)
            {
                min = distance;
                closestTroop = troop;
            }
        }

        Debug.Log($"Closest troop = {closestTroop.name}");

        if (GridExplorer._instance.ManhattanDistance(_dummyScript.GetTile(), closestTroop.GetTile()) <= _dummyScript.GetAttackRange())
        {
            _dummyScript.Attack(closestTroop.gameObject);
            return;
        }

        Debug.Log($"{closestTroop.name} out of attack range.");

        GameObject currentTile = _dummyScript.GetTile();
        List<GameObject> reachableTiles = GridExplorer._instance.GetTilesInRange(currentTile, _dummyScript.GetMoveRange(), true);

        min = 9999f;
        GameObject closestTile = null;
        foreach (var tile in reachableTiles)
        {
            float distance = Vector3.Distance(tile.transform.position, closestTroop.transform.position);
            if (distance < min)
            {
                min = distance;
                closestTile = tile;
            }
        }

        _dummyScript.MoveTo(closestTile);
        Debug.Log($"Moving {_dummyTroop.name}");

        if (GridExplorer._instance.ManhattanDistance(_dummyScript.GetTile(), closestTroop.GetTile()) <= _dummyScript.GetAttackRange())
        {
            _dummyScript.Attack(closestTroop.gameObject);
            return;
        }

        Debug.Log($"{closestTroop.name} out of attack range.");

        /*
        List<AIAction> scoredActions = new();
        foreach (var tile in reachableTiles)
        {
            ScoreAIActionOptions(tile);
        }

        randomTop5Index = Random.Range(scoredActions.Count - 5, scoredActions.Count);
        PerformAIAction(scoredActions(randomTop5Index));

        Debug.Log($"Random index: {randomIndex}");
        Debug.Log($"tiles[randomIndex]: {tiles[randomIndex]}");
        Debug.DrawLine(tiles[randomIndex].transform.position, tiles[randomIndex].transform.position + Vector3.up * 3f, Color.red, 5f);
        Debug.Log($"AI_Controller started at {tiles[randomIndex].transform.position}");
        foreach (var element in reachableTiles)
        {
            Debug.Log($"AI_Controller can reach {element.transform.position}");
        }
        */

    }
}

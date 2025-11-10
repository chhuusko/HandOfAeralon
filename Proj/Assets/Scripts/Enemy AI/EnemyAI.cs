using System.Collections.Generic;
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

    [SerializeField] private GameObject dummyTroop;
    private DummyCharacter dummyScript;
    private InputSystem_Actions inputActions;

    void Start()
    {
        dummyScript = dummyTroop.GetComponent<DummyCharacter>();
        if (dummyScript == null)
        {
            Debug.LogError("DummyCharacter script NOT FOUND!");
        }

        inputActions = new();
        inputActions.Enable();
        inputActions.Player.Jump.performed += OnJump;
    }

    void OnJump(InputAction.CallbackContext context)
    {
        if (dummyScript.GetOwner() != this) return;

        GameObject currentTile = dummyScript.GetTile();
        List<GameObject> reachableTiles = GridExplorer.Instance.GetTilesInRange(currentTile, dummyScript.GetMoveRange(), true);

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

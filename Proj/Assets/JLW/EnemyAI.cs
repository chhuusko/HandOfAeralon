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

    private GameObject _currentTroop;
    private InputSystem_Actions inputActions;

    void Start()
    {
        inputActions = new();
        inputActions.Enable();
        inputActions.Player.Jump.performed += OnJump;
    }

    void OnJump(InputAction.CallbackContext context)
    {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("EditorTile");
        int randomIndex = Random.Range(0, tiles.Length);
        Debug.Log($"Random index: {randomIndex}");

        List<GameObject> reachableTiles = GridExplorer.Instance.GetReachableTiles(tiles[randomIndex], 3);

        Debug.Log($"AI_Controller started at {tiles[randomIndex].transform.position}");
        foreach (var element in reachableTiles)
        {
            Debug.Log($"AI_Controller can reach {element.transform.position}");
        }
    }
}

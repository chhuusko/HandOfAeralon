using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AI_Controller : MonoBehaviour
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

    private int ManhattanDistance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs((a.x - b.x) + (a.y - b.y));
    }

    private List<Tile> FloodFill()
    {
        List<Tile> result = new();

        // 

        return result;
    }
}

using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.PlayerSettings;

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

    private class TestTile
    {
        public Vector2Int pos;
        public bool bIsWalkable;
        public bool bIsOccupied;

        public TestTile(Vector2Int pos, bool bIsWalkable, bool bIsOccupied)
        {
            this.pos = pos;
            this.bIsWalkable = bIsWalkable;
            this.bIsOccupied = bIsOccupied;
        }
    }

    private GameObject _currentTroop;
    private TestTile[,] testGrid;
    private Vector2Int startPos;
    private InputSystem_Actions inputActions;
    [SerializeField] private CombatManager combatManager;
    private GameObject[] combatGrid;

    void Start()
    {
        combatGrid = combatManager.GetGridTiles();
        testGrid = CreateTestGrid();
        startPos = new Vector2Int(0, 0);
        inputActions = new();
        inputActions.Enable();
        inputActions.Player.Jump.performed += OnJump;
    }

    void OnJump(InputAction.CallbackContext context)
    {
        List<Vector2Int> reachableTiles = GetReachableTiles(startPos, 5);

        Debug.Log($"AI_Controller started at {startPos}");
        foreach (var vector in reachableTiles)
        {
            Debug.Log($"AI_Controller can reach {vector}");
        }
    }

    private int ManhattanDistance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs((a.x - b.x) + (a.y - b.y));
    }

    private List<Vector2Int> GetReachableTiles(Vector2Int start, int range)
    {
        List<Vector2Int> result = new();
        
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(-1, 0),
            new Vector2Int(0, -1)
        };

        Queue<Vector2Int> queue = new();
        Dictionary<Vector2Int, int> cost = new();
        queue.Enqueue(start);
        cost[start] = 0;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            UnityEngine.Vector3 debug = new UnityEngine.Vector3(current.x, 0, current.y);
            Debug.DrawLine(debug, debug + UnityEngine.Vector3.up, Color.red, 3f);

            foreach (var dir in directions)
            {
                Vector2Int next = current + dir;
                int nextCost = cost[current] + 1;

                if (!IsWalkable(next)) continue;
                if (nextCost > range) continue;
                if (cost.ContainsKey(next)) continue;

                cost[next] = nextCost;
                queue.Enqueue(next);
                result.Add(next);
            }
        }

        return result;
    }

    private bool IsWalkable(Vector2Int pos)
    {
        if (pos.x < 0 || pos.y < 0 || pos.x > 19 || pos.y > 19)
        {
            return false;
        }

        return combatManager.GetTileAtCoord(pos.x, pos.y).GetComponent<CombatGridTile>().IsWalkable();
    }

    private TestTile[,] CreateTestGrid()
    {
        TestTile[,] testGrid = new TestTile[20, 20];

        for (int y = 0; y < 20; y++)
        {
            for (int x = 0; x < 20; x++)
            {
                testGrid[x, y] = new TestTile(new Vector2Int(x, y), true, false);
            }
        }

        return testGrid;
    }
}

using System.Collections.Generic;
using UnityEngine;

public class GridExplorer : MonoBehaviour
{
    public static GridExplorer Instance { get; private set; }

    void Awake()
    {
        if (GridExplorer.Instance != null && GridExplorer.Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            GridExplorer.Instance = this;
        }
    }

    [SerializeField] private bool _debug = false;
    private List<GameObject> _debugReachableTiles = new();
    private GameObject _debugStartTile;

    private CombatManager _combatManager;

    void Start()
    {
        _combatManager = FindFirstObjectByType<CombatManager>();
        if (_combatManager == null)
        {
            Debug.LogError("GridExplorer._combatManager not found in scene!");
        }
    }

    public int ManhattanDistance(GameObject a, GameObject b)
    {
        Vector2Int c = a.GetComponent<CombatGridTile>().GetTileIndex();
        Vector2Int d = b.GetComponent<CombatGridTile>().GetTileIndex();

        return Mathf.Abs(c.x - d.x) + Mathf.Abs(c.y - d.y);
    }

    /// <summary>
    /// Performs a breadth-first search (BFS) from the given origin tile to find all tiles within the specified range.
    /// </summary>
    /// <param name="origin">The starting tile GameObject used as the center of the search.</param>
    /// <param name="range">The maximum Manhattan distance (in tiles) to search from the origin.</param>
    /// <param name="checkWalkable">
    /// If true, only walkable tiles are included in the result. 
    /// If false, all tiles within range are returned regardless of walkability.
    /// </param>
    /// <returns>
    /// A list of <see cref="GameObject"/> tiles that are within the specified range of the origin.
    /// </returns>
    /// <remarks>
    /// This method uses a grid-based breadth-first search (BFS) algorithm to traverse the map in four cardinal directions. 
    /// It stops expanding when the specified range limit is reached or when encountering tiles marked as non-walkable 
    /// (if <paramref name="checkWalkable"/> is enabled). 
    /// 
    /// The method also updates internal debug fields (<c>_debugStartTile</c> and <c>_debugReachableTiles</c>) 
    /// used for visualization in the editor.
    /// </remarks>

    public List<GameObject> GetTilesInRange(GameObject origin, int range, bool checkWalkable)
    {
        List<GameObject> result = new();

        Vector2Int start = origin.GetComponent<CombatGridTile>().GetTileIndex();

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

            foreach (var dir in directions)
            {
                Vector2Int next = current + dir;
                int nextCost = cost[current] + 1;

                if (OutOfBounds(next))
                {
                    if (_debug)
                        Debug.Log("GridExplorer.GetTilesInRange() | continue: OutOfBounds(next)");
                    continue;
                }
                if (checkWalkable && !IsWalkable(next))
                {
                    if (_debug)
                        Debug.Log("GridExplorer.GetTilesInRange() | continue: !IsWalkable");
                    continue;
                }
                if (nextCost > range)
                {
                    if (_debug)
                        Debug.Log("GridExplorer.GetTilesInRange() | continue: nextCost > range");
                    continue;
                }
                if (cost.ContainsKey(next))
                {
                    if (_debug)
                        Debug.Log("GridExplorer.GetTilesInRange() | continue: cost.ContainsKey(next)");
                    continue;
                }

                cost[next] = nextCost;
                queue.Enqueue(next);
                result.Add(_combatManager.GetTileAtCoord(next.x, next.y));
            }
        }

        _debugStartTile = _combatManager.GetTileAtCoord(start.x, start.y);
        _debugReachableTiles = result;
        return result;
    }

    private bool OutOfBounds(Vector2Int pos)
    {
        if (pos.x < 0 || pos.y < 0 || pos.x >= _combatManager.GetGridWidth() || pos.y >= _combatManager.GetGridHeight())
        {
            return true;
        }

        return false;
    }

    private bool IsWalkable(Vector2Int pos)
    {
        return _combatManager.GetTileAtCoord(pos.x, pos.y).GetComponent<CombatGridTile>().IsWalkable();
    }

    private void OnDrawGizmos()
    {
        if (!_debug || _debugReachableTiles == null || _debugReachableTiles.Count == 0)
        {
            return;
        }

        Gizmos.color = new Color(0, 1, 0, 0.5f);
        foreach (var element in _debugReachableTiles)
        {
            Gizmos.DrawCube(element.transform.position, _combatManager.GetTileSize() * 0.9f);
        }

        Gizmos.color = new Color(1, 1, 1, 0.8f);
        Gizmos.DrawCube(_debugStartTile.transform.position, _combatManager.GetTileSize() * 0.9f);
    }
}

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

    public int ManhattanDistance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    public List<GameObject> GetWalkableTilesInRange(GameObject startTile, int range)
    {
        return BFS(startTile, range, true);
    }

    public List<GameObject> GetAllTilesInRange(GameObject startTile, int range)
    {
        return BFS(startTile, range, false);
    }

    private List<GameObject> BFS(GameObject origin, int range, bool checkWalkable)
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
                        Debug.Log("GridExplorer.BFS() | continue: OutOfBounds(next)");
                    continue;
                }
                if (checkWalkable && !IsWalkable(next))
                {
                    if (_debug)
                        Debug.Log("GridExplorer.BFS() | continue: !IsWalkable");
                    continue;
                }
                if (nextCost > range)
                {
                    if (_debug)
                        Debug.Log("GridExplorer.BFS() | continue: nextCost > range");
                    continue;
                }
                if (cost.ContainsKey(next))
                {
                    if (_debug)
                        Debug.Log("GridExplorer.BFS() | continue: cost.ContainsKey(next)");
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

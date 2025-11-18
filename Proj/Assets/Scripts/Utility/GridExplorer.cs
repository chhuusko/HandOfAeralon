using System.Collections.Generic;
using UnityEngine;

public class GridExplorer : MonoBehaviour
{
    public static GridExplorer _instance { get; private set; }

    void Awake()
    {
        if (GridExplorer._instance != null && GridExplorer._instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            GridExplorer._instance = this;
        }
    }

    [SerializeField] private LineRenderer _lineRendererPrefab;
    private LineRenderer _activeLineRenderer;

    [SerializeField] private bool _bDebug = false;
    private List<GameObject> _debugReachableTiles = new();
    private List<GameObject> _debugPath = new();
    private GameObject _debugStartTile;
    private GameObject _debugGoalTile;

    /// <summary>
    /// Calculates the ManhattanDistance between two tile objects (a.x - b.x + a.y - b.y).
    /// </summary>
    /// <param name="a">The GameObject of a GridTile.</param>
    /// <param name="b">The GameObject of a GridTile.</param>
    /// <returns>An int containing the ManhattanDistance value between object 'a' and object 'b'.</returns>
    public int ManhattanDistance(GameObject a, GameObject b)
    {
        Vector2Int c = a.GetComponent<CombatGridTile>().GetTileIndex();
        Vector2Int d = b.GetComponent<CombatGridTile>().GetTileIndex();

        return Mathf.Abs(c.x - d.x) + Mathf.Abs(c.y - d.y);
    }

    /// <summary>
    /// Performs a breadth-first search (BFS) from the given start tile to find a path from 'startTile' to 'goalTile'.
    /// </summary>
    /// <param name="startTile">The starting tile <see cref="GameObject"/> used as the origin of the search.</param>
    /// <param name="goalTile">The tile <see cref="GameObject"/> used as the goal of the search.</param>
    /// <returns>
    /// A list of <see cref="GameObject"/> tiles that represent the path taken from 'startTile' to 'goalTile'.
    /// </returns>
    /// <remarks>
    /// This method uses a grid-based breadth-first search (BFS) algorithm to traverse the map in four cardinal directions. 
    /// It steps back when it can't find any more neighbours to explore (either already visited or unwalkable) and finds a different path.
    /// It stops and returns the resulting path when it reaches the goal.
    /// The method also updates internal debug fields (_debugStartTile, _debugPath and _debugGoalTile) 
    /// used for visualization in the editor.
    /// </remarks>
    public List<GameObject> FindPath(GameObject startTile, GameObject goalTile)
    {
        List<GameObject> result = new();

        Vector2Int start = startTile.GetComponent<CombatGridTile>().GetTileIndex();
        Vector2Int goal = goalTile.GetComponent<CombatGridTile>().GetTileIndex();

        if (start == goal)
        {
            DebugLog.JLWLog($"GridExplorer.cs | start {start} == goal {goal}");
            return new List<GameObject>();
        }

        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(1, 1),
            new Vector2Int(-1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, -1),

            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(-1, 0),
            new Vector2Int(0, -1)
        };

        Queue<Vector2Int> queue = new();
        Dictionary<Vector2Int, Vector2Int> connection = new();
        HashSet<Vector2Int> visited = new();
        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            if (current == goal)
            {
                result = BuildPath(connection, start, goal);
                if (_bDebug) _debugStartTile = startTile;
                if (_bDebug) _debugPath = result;
                if (_bDebug) _debugGoalTile = goalTile;
                return result;
            }

            foreach (var dir in directions)
            {
                Vector2Int next = current + dir;

                if (OutOfBounds(next)) continue;
                if (!IsWalkable(next)) continue;

                if (IsDiagonal(dir))
                {
                    Vector2Int tile1 = new Vector2Int(current.x, next.y);
                    Vector2Int tile2 = new Vector2Int(next.x, current.y);

                    if (!IsWalkable(tile1) || !IsWalkable(tile2) || IsOccupied(tile1) || IsOccupied(tile2)) continue;
                }

                if (IsOccupied(next) && next != goal) continue;
                if (visited.Contains(next)) continue;

                queue.Enqueue(next);
                visited.Add(next);
                connection[next] = current;
            }
        }

        return new List<GameObject>(); // No path found
    }

    private List<GameObject> BuildPath(Dictionary<Vector2Int, Vector2Int> connection, Vector2Int start, Vector2Int goal)
    {
        List<GameObject> result = new();
        Vector2Int current = goal;

        while (connection.ContainsKey(current))
        {
            result.Insert(0, CombatGrid._instance.GetTileAtCoord(current.x, current.y));
            current = connection[current];
        }

        result.Insert(0, CombatGrid._instance.GetTileAtCoord(start.x, start.y));
        return result;
    }

    private bool IsDiagonal(Vector2Int dir)
    {
        return Mathf.Abs(dir.x) + Mathf.Abs(dir.y) == 2;
    }

    /// <summary>
    /// Performs a breadth-first search (BFS) from the given origin tile to find all tiles within the specified range.
    /// </summary>
    /// <param name="origin">The starting tile <see cref="GameObject"/> used as the center of the search.</param>
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
    /// The method also updates internal debug fields (_debugStartTile and _debugReachableTiles) 
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
                    DebugLog.JLWLog("GridExplorer.cs | continue: next OutOfBounds");
                    continue;
                }
                if (checkWalkable && !IsWalkable(next))
                {
                    DebugLog.JLWLog("GridExplorer.cs | continue: next !IsWalkable");
                    continue;
                }
                if (checkWalkable && IsOccupied(next))
                {
                    DebugLog.JLWLog("GridExplorer.cs | continue: next IsOccupied");
                    continue;
                }
                if (nextCost > range)
                {
                    DebugLog.JLWLog("GridExplorer.cs | continue: nextCost > range");
                    continue;
                }
                if (cost.ContainsKey(next))
                {
                    DebugLog.JLWLog("GridExplorer.cs | continue: cost.ContainsKey(next)");
                    continue;
                }

                cost[next] = nextCost;
                queue.Enqueue(next);
                result.Add(CombatGrid._instance.GetTileAtCoord(next.x, next.y));
            }
        }

        if (_bDebug) _debugStartTile = CombatGrid._instance.GetTileAtCoord(start.x, start.y);
        if (_bDebug) _debugReachableTiles = result;
        return result;
    }

    private bool OutOfBounds(Vector2Int pos)
    {
        if (pos.x < 0 || pos.y < 0 || pos.x >= CombatGrid._instance.GetGridWidth() || pos.y >= CombatGrid._instance.GetGridHeight())
        {
            return true;
        }

        return false;
    }

    private bool IsWalkable(Vector2Int pos)
    {
        return CombatGrid._instance.GetTileAtCoord(pos.x, pos.y).GetComponent<CombatGridTile>().IsWalkable();
    }

    private bool IsOccupied(Vector2Int pos)
    {
        return CombatGrid._instance.GetTileAtCoord(pos.x, pos.y).GetComponent<CombatGridTile>().GetOccupant() != null;
    }

    private void DrawPath(List<GameObject> path)
    {
        if (_activeLineRenderer != null)
        {
            Destroy(_activeLineRenderer.gameObject);
        }

        if (path == null || path.Count == 0)
        {
            return;
        }

        _activeLineRenderer = Instantiate(_lineRendererPrefab);
        _activeLineRenderer.positionCount = path.Count;

        for (int i = 0; i < path.Count; i++)
        {
            _activeLineRenderer.SetPosition(i, path[i].transform.position + Vector3.up * 0.1f);
        }
    }

    private void OnDrawGizmos()
    {
        if (!_bDebug || _debugStartTile == null)
        {
            return;
        }

        Gizmos.color = new Color(0, 1, 0, 0.5f);
        foreach (var element in _debugReachableTiles)
        {
            Gizmos.DrawCube(element.transform.position, CombatGrid._instance.GetTileSize() * 0.9f);
        }

        Gizmos.color = new Color(1, 0, 1, 0.5f);
        foreach (var element in _debugPath)
        {
            Gizmos.DrawCube(element.transform.position, CombatGrid._instance.GetTileSize() * 0.9f);
        }

        Gizmos.color = new Color(1, 1, 1, 0.8f);
        Gizmos.DrawCube(_debugStartTile.transform.position, CombatGrid._instance.GetTileSize() * 0.9f);

        Gizmos.color = new Color(1, 0, 0, 0.8f);
        Gizmos.DrawCube(_debugGoalTile.transform.position, CombatGrid._instance.GetTileSize() * 0.9f);
    }
}

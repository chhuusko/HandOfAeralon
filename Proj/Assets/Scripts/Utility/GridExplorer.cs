using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<T>
{
    private List<(T item, float priority)> elements = new();

    public int Count => elements.Count;

    public void Enqueue(T item, float priority)
    {
        elements.Add((item, priority));
    }

    public T Dequeue()
    {
        int bestIndex = 0;
        float bestPriority = elements[0].priority;

        for (int i = 1; i < elements.Count; i++)
        {
            if (elements[i].priority < bestPriority)
            {
                bestPriority = elements[i].priority;
                bestIndex = i;
            }
        }

        T bestItem = elements[bestIndex].item;
        elements.RemoveAt(bestIndex);
        return bestItem;
    }
}

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

    /*
    [SerializeField] private bool _bPaintTiles = false;
    private List<GameObject> _paintReachableTiles = new();
    private List<GameObject> _paintPath = new();
    private GameObject _paintStartTile;
    private GameObject _paintGoalTile;
    */

    /// <summary>
    /// Defines our melee attacking range. Includes diagonals, but only for attacks that reach 1 tile. Can't reach through diagonal obstacles.
    /// </summary>
    /// <param name="origin">The <see cref="GameObject"/> of the attacking character or tile.</param>
    /// <returns>A List of CombatGridTiles that represent all the reachable tiles for this characters current position.</returns>
    public List<CombatGridTile> GetTilesInMeleeRange(GameObject origin)
    {
        List<CombatGridTile> result = new();
        Vector2Int pos = new();

        if (origin.TryGetComponent<CombatGridTile>(out CombatGridTile tile))
        {
            pos = tile.GetTileIndex();
        }
        else if (origin.TryGetComponent<Character>(out Character character))
        {
            pos = character.GetCurrentTileIndex();
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

        foreach (var dir in directions)
        {
            Vector2Int next = pos + dir;

            if (OutOfBounds(next)) continue;
            if (!IsWalkable(next)) continue;

            if (IsDiagonal(dir))
            {
                Vector2Int t1 = new Vector2Int(pos.x, next.y);
                Vector2Int t2 = new Vector2Int(next.x, pos.y);

                if (!IsWalkable(t1) && !IsWalkable(t2)) continue;
            }

            result.Add(CombatGrid._instance.GetTileAtCoord(next.x, next.y).GetComponent<CombatGridTile>());
        }

        return result;
    }

    /// <summary>
    /// Calculates the ManhattanDistance between two Vector2Int (a.x - b.x + a.y - b.y).
    /// </summary>
    /// <param name="a">The Vector2Int representation of a grid tile.</param>
    /// <param name="b">The Vector2Int representation of a grid tile.</param>
    /// <returns>An int containing the ManhattanDistance value between object 'a' and object 'b'.</returns>
    public int ManhattanDistance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    /// <summary>
    /// Calculates the ChebyshevDistance between two Vector2Int.
    /// </summary>
    /// <param name="a">The Vector2Int representation of a grid tile.</param>
    /// <param name="b">The Vector2Int representation of a grid tile.</param>
    /// <returns>An int containing the ChebyshevDistance (diagonal distance) value between object 'a' and object 'b'.</returns>
    public int ChebyshevDistance(Vector2Int a, Vector2Int b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        return Mathf.Max(dx, dy);
    }

    /// <summary>
    /// Performs an A* search from the given start tile to find an optimal path from 
    /// <paramref name="startTile"/> to <paramref name="goalTile"/>.
    /// </summary>
    /// <param name="startTile">
    /// The starting tile <see cref="GameObject"/> used as the origin of the search.
    /// </param>
    /// <param name="goalTile">
    /// The destination tile <see cref="GameObject"/> the algorithm attempts to reach.
    /// </param>
    /// <returns>
    /// A list of <see cref="GameObject"/> tiles representing the shortest calculated path 
    /// from <paramref name="startTile"/> to <paramref name="goalTile"/>.
    /// Returns an empty list if no valid path could be found.
    /// </returns>
    /// <remarks>
    /// This method uses a grid-based A* pathfinding algorithm.  
    /// A* combines actual movement cost (G-cost) with a heuristic estimate (H-cost) to efficiently 
    /// determine the optimal route.
    ///
    /// The search expands both cardinal and diagonal neighbours.  
    /// Diagonal movement is slightly more expensive than straight movement, encouraging the algorithm 
    /// to prefer direct diagonal routes when available but still allowing natural cornering behavior.
    ///
    /// The method stops when the goal tile is dequeued from the open set, ensuring the returned path 
    /// is optimal according to the distance model used.
    ///
    /// The method also updates internal debug fields (_debugStartTile, _debugPath and _debugGoalTile) 
    /// to allow visualization of the final computed path inside the editor.
    /// </remarks>

    public List<GameObject> FindPathAStar(GameObject startTile, GameObject goalTile)
    {
        Vector2Int start = startTile.GetComponent<CombatGridTile>().GetTileIndex();
        Vector2Int goal = goalTile.GetComponent<CombatGridTile>().GetTileIndex();

        if (start == goal)
        {
            ClearPathDrawing();
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

        PriorityQueue<Vector2Int> open = new PriorityQueue<Vector2Int>();
        HashSet<Vector2Int> closed = new HashSet<Vector2Int>();

        Dictionary<Vector2Int, Vector2Int> cameFrom = new();
        Dictionary<Vector2Int, float> gCost = new();
        Dictionary<Vector2Int, float> fCost = new();

        gCost[start] = 0;
        fCost[start] = ManhattanDistance(start, goal);

        open.Enqueue(start, fCost[start]);

        while (open.Count > 0)
        {
            Vector2Int current = open.Dequeue();

            if (current == goal)
            {
                var result = BuildPath(cameFrom, start, goal);
                DrawPath(result);
                return result;
            }

            closed.Add(current);

            foreach (var dir in directions)
            {
                Vector2Int next = current + dir;

                if (OutOfBounds(next)) continue;
                if (!IsWalkable(next)) continue;

                if (IsDiagonal(dir))
                {
                    Vector2Int t1 = new Vector2Int(current.x, next.y);
                    Vector2Int t2 = new Vector2Int(next.x, current.y);

                    if (!IsWalkable(t1) || !IsWalkable(t2)) continue;
                }

                if (IsOccupied(next) && next != goal) continue;
                if (closed.Contains(next)) continue;

                float moveCost = (IsDiagonal(dir) ? 1.4f : 1f);
                float tentativeG = gCost[current] + moveCost;

                if (!gCost.ContainsKey(next) || tentativeG < gCost[next])
                {
                    cameFrom[next] = current;
                    gCost[next] = tentativeG;

                    float h = ManhattanDistance(next, goal);
                    fCost[next] = gCost[next] + h;

                    open.Enqueue(next, fCost[next]);
                }
            }
        }

        ClearPathDrawing();
        return new List<GameObject>();
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
    public List<GameObject> FindPathBFS(GameObject startTile, GameObject goalTile)
    {
        List<GameObject> result = new();

        Vector2Int start = startTile.GetComponent<CombatGridTile>().GetTileIndex();
        Vector2Int goal = goalTile.GetComponent<CombatGridTile>().GetTileIndex();

        if (start == goal)
        {
            DebugLog.JLWLog($"GridExplorer.cs | start {start} == goal {goal}");
            ClearPathDrawing();
            return new List<GameObject>();
        }

        Vector2Int[] directions = new Vector2Int[]
        {
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
                /*
                if (_bPaintTiles) _paintStartTile = startTile;
                if (_bPaintTiles) _paintPath = result;
                if (_bPaintTiles) _paintGoalTile = goalTile;
                */
                DrawPath(result);
                return result;
            }

            foreach (var dir in directions)
            {
                Vector2Int next = current + dir;

                if (OutOfBounds(next)) continue;
                if (!IsWalkable(next)) continue;
                if (IsOccupied(next) && next != goal) continue;
                if (visited.Contains(next)) continue;

                queue.Enqueue(next);
                visited.Add(next);
                connection[next] = current;
            }
        }

        ClearPathDrawing();
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

                if (OutOfBounds(next)) continue;
                if (checkWalkable && !IsWalkable(next)) continue;
                if (checkWalkable && IsOccupied(next)) continue;
                if (nextCost > range) continue;
                if (cost.ContainsKey(next)) continue;

                cost[next] = nextCost;
                queue.Enqueue(next);
                result.Add(CombatGrid._instance.GetTileAtCoord(next.x, next.y));
            }
        }

        /*
        if (_bPaintTiles) _paintStartTile = CombatGrid._instance.GetTileAtCoord(start.x, start.y);
        if (_bPaintTiles) _paintReachableTiles = result;
        */
        PaintReachableTiles(result, Color.green);
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

    public void ClearPathDrawing()
    {
        if (_activeLineRenderer != null)
        {
            Destroy(_activeLineRenderer.gameObject);
        }
    }

    private void PaintReachableTiles(List<GameObject> tileObjects, Color color)
    {
        //Debug.Log("PaintReachableTiles()");
        if (tileObjects != null && tileObjects.Count > 0)
        {
            //Debug.Log("GameObjects found!");
            foreach (var element in tileObjects)
            {
                if (element.TryGetComponent<CombatGridTile>(out CombatGridTile component))
                {
                    //Debug.Log("CombatGridTile component found!");
                    component.SetTileColor(color);
                }
            }
        }
    }

    /*
    private void OnDrawGizmos()
    {
        if (!_bPaintTiles)
        {
            return;
        }

        if (_paintReachableTiles != null && _paintReachableTiles.Count > 0)
        {
            Gizmos.color = new Color(0, 1, 0, 0.5f);
            foreach (var element in _paintReachableTiles)
            {
                Gizmos.DrawCube(element.transform.position, CombatGrid._instance.GetTileSize() * 0.9f);
            }
        }

        if (_paintPath != null && _paintPath.Count > 0)
        {
            Gizmos.color = new Color(1, 0, 1, 0.5f);
            foreach (var element in _paintPath)
            {
                Gizmos.DrawCube(element.transform.position, CombatGrid._instance.GetTileSize() * 0.9f);
            }
        }

        if (_paintStartTile != null)
        {
            Gizmos.color = new Color(1, 1, 1, 0.8f);
            Gizmos.DrawCube(_paintStartTile.transform.position, CombatGrid._instance.GetTileSize() * 0.9f);
        }

        if (_paintGoalTile != null)
        {
            Gizmos.color = new Color(1, 0, 0, 0.8f);
            Gizmos.DrawCube(_paintGoalTile.transform.position, CombatGrid._instance.GetTileSize() * 0.9f);
        }
    }
    */
}

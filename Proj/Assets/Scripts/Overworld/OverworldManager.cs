using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OverworldManager : MonoBehaviour
{
    public static OverworldManager _instance { get; private set; }

    [SerializeField] private NodeMapData _data;

    private List<OverworldNode> _currentNodePath = new();

    private OverworldNode _selectedNode;

    public OverworldNode GetSelectedNode() => _selectedNode;
    public void SetSelectedNode(OverworldNode node)
    {
        _selectedNode = node;
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        LoadPath();
        SetLastNodeNeighborsSelectable(true);
    }

    void Update()
    {
        HandleNodeClick();
        HandleNodeHover();
    }

    private void HandleNodeClick()
    {
        if (Input.GetMouseButtonDown(1)) DeselectNode();

        OverworldNode node = GetNodeUnderMouse();
        if (!Input.GetMouseButtonDown(0) || node == null) return;

        if (node.IsSelectable == false) return;

        SelectNode(node);
    }

    private void HandleNodeHover()
    {
        OverworldNode node = GetNodeUnderMouse();
        if (node == null) return;
    }

    private OverworldNode GetNodeUnderMouse()
    {
        // Cast ray cast from mouse to detect Node and return it if found.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int nodeMask = LayerMask.GetMask("Node");

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, nodeMask))
        {
            // Check for node script on gameobject.
            if (hit.collider.TryGetComponent(out OverworldNode node)) return node;
        }
        return null;
    }

    private void SelectNode(OverworldNode node)
    {
        SetSelectedNode(node);
    }

    private void DeselectNode()
    {
        SetSelectedNode(null);
    }

    private OverworldNode GetLastNode()
    {
        if (_currentNodePath.Count == 0) return null;

        return _currentNodePath[_currentNodePath.Count - 1];
    }

    private void LoadPath()
    {
        _currentNodePath.Clear();

        var allNodes = FindObjectsByType<OverworldNode>(FindObjectsSortMode.None);

        var nodeLookup = new Dictionary<string, OverworldNode>();
        foreach (var node in allNodes)
        {
            nodeLookup[node.GetNodeId()] = node;
        }

        foreach (var id in _data.GetNodePathIds())
        {
            if (nodeLookup.TryGetValue(id, out var node))
            {
                _currentNodePath.Add(node);
            }
        }

        if (_data.GetNodePathIds().Count == 0)
        {
            var startNode = allNodes.FirstOrDefault(n => n.IsStartNode);

            _currentNodePath.Add(startNode);
        }
    }

    public void SavePath()
    {
        _data.SetNodePath(_currentNodePath);
    }

    public void SetLastNodeNeighborsSelectable(bool isSelectable)
    {
        OverworldNode lastNode = GetLastNode();
        if (lastNode == null)
        {
            Debug.LogError("Did not find last node in path, something is wrong with path.");
            return;
        }
        List<OverworldNode> neighbors = lastNode.GetNeighbors();
        if (neighbors == null) return;

        foreach (OverworldNode node in neighbors)
        {
            node.SetSelectable(isSelectable);
        }
    }
    public void AddNodeToPath(OverworldNode node)
    {
        _currentNodePath.Add(node);
    }
    public void ExitOverworld()
    {
        SavePath();
    }
}

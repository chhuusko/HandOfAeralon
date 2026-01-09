using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NodeMapData", menuName = "Scriptable Objects/Overworld/NodeMapData")]

public class NodeMapData : ScriptableObject
{
    [SerializeField] private List<string> _nodePathIds = new();

    public List<string> GetNodePathIds() => _nodePathIds;

    public void SetNodePath(List<OverworldNode> nodes)
    {
        _nodePathIds.Clear();
        foreach (var node in nodes)
        {
            _nodePathIds.Add(node.GetNodeId());
        }
    }
}

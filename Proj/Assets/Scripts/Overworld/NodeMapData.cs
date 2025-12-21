
using System.Collections.Generic;
using UnityEngine;

public class NodeMapData : ScriptableObject
{
    private List<string> nodePathIds;

    public List<string> GetNodePathIds() => nodePathIds;

    public void SetNodePath(List<OverworldNode> nodes)
    {
        nodePathIds.Clear();
        foreach (var node in nodes)
        {
            nodePathIds.Add(node.GetNodeId());
        }
    }
}

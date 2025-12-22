using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OverworldNode : MonoBehaviour
{
    [SerializeField] private string _nodeId;

    [SerializeField] private string _sceneToLoad;

    [SerializeField] private List<OverworldNode> _neighbors;

    [SerializeField] private bool _isStartNode;
    public string GetNodeId() => _nodeId;

    private bool _isSelectable = false;

    public string GetSceneToLoad() => _sceneToLoad;

    public List<OverworldNode> GetNeighbors() => _neighbors;
    public bool IsSelectable => _isSelectable;
    public bool IsStartNode => _isStartNode;

    public void SetSelectable(bool isSelectable)
    {
        _isSelectable = isSelectable;
    }

    [ContextMenu("Generate GUID")]
    private void GenerateGuid()
    {
        _nodeId = System.Guid.NewGuid().ToString();
    }

    public IEnumerator StartLoadingNodeScene()
    {
        // TODO: Start loadning animation

        // Save path.
        OverworldManager._instance.AddNodeToPath(this);
        OverworldManager._instance.SavePath();

        // Disable selection for nodes 
        OverworldManager._instance.SetLastNodeNeighborsSelectable(false);

        yield return new WaitForSeconds(2);

        LoadScene();
    }

    protected virtual void LoadScene()
    {
        SceneManager.LoadScene(_sceneToLoad);
    }
}

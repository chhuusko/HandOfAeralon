using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static CanvasManager instance;
    [SerializeField] public GameObject MainCanvas;
    [SerializeField] public GameObject OverlayCanvas;
    [SerializeField] public GameObject CardInfoPanelCanvas;
    private void Awake()
    {
        instance = this;
    }
    public static CanvasManager Instance()
    {
        return instance;
    }

}

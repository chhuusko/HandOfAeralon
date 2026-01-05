using TMPro;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public static Tutorial Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] private RectTransform _turnOrder;

    private Canvas _canvas;

    void Start()
    {
        _canvas = GetComponent<Canvas>();

        if (_canvas == null)
        {
            Debug.LogError("Tutorial.cs | Canvas not found!");
        }
    }
}

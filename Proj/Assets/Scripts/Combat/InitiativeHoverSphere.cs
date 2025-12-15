using UnityEngine;

public class InitiativeHoverSphere : MonoBehaviour
{

    [SerializeField] private GameObject _hoverSphere;
    [SerializeField] private Vector3 _hoverStartPosition;
    [SerializeField] private float _selectorOverHeadBounceSpeed;
    [SerializeField] private float _selectorOverHeadBounceInterval;

    void Start()
    {
        
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }
    public void SetHoverStartPosition(Vector3 startPosition)
    {
        _hoverStartPosition = startPosition;
    }
    public void SetHoverSpherePosition(Vector3 position)
    {
        _hoverSphere.transform.position = position;
    }

    public void UpdatePosition()
    {

        float py = _hoverStartPosition.y + Mathf.Sin(Time.time * _selectorOverHeadBounceSpeed) * _selectorOverHeadBounceInterval;

        _hoverSphere.transform.position = new Vector3(_hoverStartPosition.x, py, _hoverStartPosition.z);
    }
    
    public void SetHoverOverheadColor(Color color)
    {
        MeshRenderer rend = _hoverSphere.GetComponent<MeshRenderer>();
        if (rend != null)
        {
            rend.material.SetColor("_BaseColor", color);
        }
    }
}

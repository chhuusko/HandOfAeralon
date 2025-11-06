using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardContainer : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    //Contains card.
    //Performs mainly ui of card
    [SerializeField] private Card _containedCard;
    private bool _isDragging;
    [SerializeField] private GameObject particleDrag, particleDrop;
    InputController _controller;
    GameObject _spawnedParticle;
    private void Awake()
    {
        _controller = new InputController();
        gameObject.GetComponent<Image>().sprite=_containedCard.icon; 
    }
    private void OnEnable()
    {
        _controller.Enable();
    }
    private void OnDisable()
    {
        _controller.Disable();
    }

    void Start()
    {
        
    }
    public void OnDrag(PointerEventData eventData)
    {
        
        
        Ray ray = Camera.main.ScreenPointToRay(_controller.UI.Point.ReadValue<Vector2>());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 worldPosition = hit.point;
            _spawnedParticle.transform.position = worldPosition;
            Debug.Log("Mouse hit world position: " + worldPosition);

        }
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _spawnedParticle = Instantiate(particleDrag);  
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Instantiate(particleDrop, _spawnedParticle.transform.position, Quaternion.identity);
        Destroy(_spawnedParticle);
    }
}

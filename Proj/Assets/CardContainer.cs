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
    [SerializeField] private GameObject _particleDrag, _particleDrop;
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
            _spawnedParticle.transform.position = hit.point;
        }
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _spawnedParticle = Instantiate(_particleDrag);  
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Destroy(Instantiate(_particleDrop, _spawnedParticle.transform.position, Quaternion.identity), 2f);
        Destroy(_spawnedParticle);
    }
}

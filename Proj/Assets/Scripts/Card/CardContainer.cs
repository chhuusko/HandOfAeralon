using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardContainer : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    //Contains card.
    //Performs mainly ui part of card
    [SerializeField] private Card _containedCard;
    [SerializeField] private GameObject _particleDrag, _particleDrop;
    private InputController _controller;
    private GameObject _spawnedParticle;
    Vector3 _startPosition, _hoverEndPosition;
    float _hoverDistance = 50f;
    private bool _isDragging;
    private void Awake()
    {
        _controller = new InputController();

        _startPosition = transform.position;
        _hoverEndPosition = transform.position + new Vector3(0, _hoverDistance, 0);
        
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
        // TODO GetGrid and do the Card thing
        Destroy(gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine(OnHover(true));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartCoroutine(OnHover(false));
    }
    IEnumerator OnHover(bool isEnter)
    {
        float duration = 0.1f; 
        float elapsed = 0f;

        if (isEnter)
        {   
            while (Vector3.Distance(transform.position, _hoverEndPosition) != 0)
            {
                transform.position = Vector3.Lerp(_startPosition, _hoverEndPosition, elapsed/duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            
            

        }
        else
        {
            while (Vector3.Distance(transform.position, _startPosition) != 0)
            {
                transform.position = Vector3.Lerp(_hoverEndPosition, _startPosition, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            
        }
    }
}

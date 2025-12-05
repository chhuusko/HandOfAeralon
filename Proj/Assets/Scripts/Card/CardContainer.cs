using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class CardContainer : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    //Contains card.
    //Performs mainly ui part of card
    [SerializeField] private Card _containedCard;
    [SerializeField] private GameObject _particleDrag, _particleDrop;
    private RectTransform _spriteTransform;
    private InputController _controller;
    private GameObject _spawnedParticle;
    private RectTransform _rect;
    Vector3 _startPosition, _hoverEndPosition;
    float _hoverDistance = 120f;
    private bool _isDragging;

    private float time;
    [SerializeField] private Vector3 angle;
    public float speed = 2f;
    private void Awake()
    {
        _controller = new InputController();
        _spriteTransform = GetComponent<RectTransform>();
    }
    private void Start()
    {
        _rect = GetComponent<RectTransform>();
        SetPos(_rect.position);
    }
    private void OnEnable()
    {
        _controller.Enable();
        _controller.Player.Cancel.performed += CancelUse;
    }

    private void OnDisable()
    {
        _controller.Disable();
        _controller.Player.Cancel.performed -= CancelUse;
    }

    private void CancelUse(InputAction.CallbackContext context)
    {
        CancelUse();
    }
    private void FixedUpdate()
    {
        //AnimationMabye
        
        time += Time.fixedDeltaTime;
        float x = Mathf.Sin(time * speed) * angle.x;
        float y = Mathf.Sin(time * speed) * angle.y;
        float z = Mathf.Sin(time * speed) * angle.z;
        
        _rect.localRotation = Quaternion.Euler(x, y, z);

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_isDragging)
        {
            Ray ray = Camera.main.ScreenPointToRay(_controller.UI.Point.ReadValue<Vector2>());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                _spawnedParticle.transform.position = hit.point;
            }
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!CanAfford()) return;
        if (!CanPlay()) return;
        _isDragging = true;
        _spawnedParticle = Instantiate(_particleDrag);  
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (_isDragging)
        {
            if (_containedCard.type == CardType.Target)
            {
                CombatGridTile grid;
                if (grid = Selector._instance.GetTileUnderMouse())
                {
                    if (!grid.GetOccupantCharacter())
                    {
                        CancelUse();
                        return;
                    }
                    else
                    {
                        CardHandManager.GetInstance().CharacterTarget(grid.GetOccupantCharacter());
                    }
                }
                else
                {
                    CancelUse();
                    return;
                }
            }
            
            Destroy(Instantiate(_particleDrop, _spawnedParticle.transform.position, Quaternion.identity), 2f);
            Destroy(_spawnedParticle);
            CardHandManager.GetInstance().ChangeMana(-_containedCard.Getcost());
            _containedCard.PlayCard();
            _containedCard.AfterCardPlay();
            CardHandManager.GetInstance().RemoveCardFromHand(this);   
        }
        
    }

    private void CancelUse()
    {
        Destroy(_spawnedParticle);
        _isDragging = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //StartCoroutine(OnHover(true));
        CardHandManager.GetInstance().ShowHighlightedCard(this, transform.position);
        setVisible(false);
    }

    private bool CanAfford()
    {
        return CardHandManager.GetInstance().GetMana() >= _containedCard.Getcost();
    }
    private bool CanPlay()
    {
        if (CombatManager._instance.GetCombatState() == CombatState.PlaceCharacters) return false;

        return (CombatManager._instance.GetCombatTurnOrder().GetCurrentTurn() == CombatTurn.PlayerTurn);
            
        
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        CardHandManager.GetInstance().HideHighlightedCard();
        setVisible(true);
        //StartCoroutine(OnHover(false));
    }
    
    public void AddCard(Card newCard)
    {
        _containedCard = newCard;
        GetComponent<CardUI>().SetUpUIElements(_containedCard);
    }
    public void UppdateCardUI()
    {
        GetComponent<CardUI>().SetUpUIElements(_containedCard);
    }
    public void SetPos(Vector3 newStarterPoint)
    {
        _startPosition = newStarterPoint;
        transform.position = _startPosition;
        _hoverEndPosition = _startPosition + new Vector3(0, _hoverDistance, 0);
        
    }
    public Card GetCard()
    {
        return _containedCard;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!CanAfford()) return;
    }
    public void setVisible(bool isVisible)
    {
        if (isVisible == true)
        {
            GetComponent<CanvasGroup>().alpha = 1;
        }
        else
        {
            GetComponent<CanvasGroup>().alpha = 0;
        }
        
    }
}

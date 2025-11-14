using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SellableCardUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Card _card;

    // Start is called once before the first execution of Update after the MonoBehaviour is created'
    private bool isHeldDown;
    private float sellTime = 3f;
    private float timeHeld = 0;
    private void Awake()
    {
        if (Shop.GetInstance() == null)
        {
            
        }

    }
    private void Update()
    {
        if (isHeldDown)
        {
            timeHeld += Time.deltaTime;
            Debug.Log(timeHeld);
            if (timeHeld > sellTime)
            {
                GlobalGameManager.GetInstance().GetGameData().cardList.Remove(_card);
                Destroy(gameObject);   
            }
        }
    }
    public void SetCard(Card card)
    {
        _card = card;
        GetComponent<Image>().sprite = card.icon;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        isHeldDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHeldDown = false;
        timeHeld = 0;
    }
}

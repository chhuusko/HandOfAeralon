using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CardViewUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected static CardViewUI _instance;

    [SerializeField] protected GameObject _cardUI;
    protected List<GameObject> _cardListUI;
    [SerializeField] protected Transform _cardContent;

    protected Vector3 basePosition;
    public static CardViewUI GetInstance() { return _instance; }
    private void Awake()
    {
        _cardListUI = new List<GameObject>();
        _instance = this;
        gameObject.SetActive(false);
        basePosition = _cardContent.position;
    }
    public virtual void UpdateCards(List<Card> newCardList)
    {
        gameObject.SetActive(true);

        if (_cardListUI.Count > 0 ) { ClearUI(); }
        if(newCardList ==  null ) { Debug.Log("Nothing");}
        _cardContent.transform.position = basePosition;
        for (int i = 0; i < newCardList.Count; i++)
        {
            _cardListUI.Add(Instantiate(_cardUI, _cardContent));
            _cardListUI[i].GetComponent<CardUI>().SetUpUIElements(newCardList[i]);
        }
    }
    protected void ClearUI()
    {
        foreach (GameObject card in _cardListUI)
        {
            Destroy(card);
        }
        _cardListUI.Clear();
    }

    public void ExitUI()
    {
        ClearUI();
        gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (SceneManager.GetActiveScene().name != "ShopScene")
        {
            CombatEventManager.InvokeOnIsHoveringUI(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (SceneManager.GetActiveScene().name != "ShopScene")
        {
            CombatEventManager.InvokeOnIsHoveringUI(false);
        }
        
    }
}

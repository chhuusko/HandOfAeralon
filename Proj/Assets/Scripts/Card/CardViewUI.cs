using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardViewUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private static CardViewUI _instance;

    [SerializeField] private GameObject _cardUI;
    private List<GameObject> _cardListUI;
    [SerializeField] private Transform _cardContent;

    public static CardViewUI GetInstance() { return _instance; }
    private void Awake()
    {
        _cardListUI = new List<GameObject>();
        _instance = this;
        gameObject.SetActive(false);
    }
    public void UpdateCards(List<Card> newCardList)
    {
        gameObject.SetActive(true);

        if (_cardListUI.Count > 0 ) { ClearUI(); }
        if(newCardList ==  null ) { Debug.Log("Nothing");}

        for (int i = 0; i < newCardList.Count; i++)
        {
            _cardListUI.Add(Instantiate(_cardUI, _cardContent));
            _cardListUI[i].GetComponent<Image>().sprite = newCardList[i].icon;
            int layer = i / 5;
            Vector3 position = transform.position + new Vector3(-600f + (i % 5 * 300f), (layer-1) * -400f, 0);
            _cardListUI[i].transform.position = position;
        }
    }
    private void ClearUI()
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

}

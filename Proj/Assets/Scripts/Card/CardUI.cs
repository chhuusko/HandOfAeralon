
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] TextMeshProUGUI _title, _description, _mana;
    [SerializeField] Image _base, _frame, _image;
    [SerializeField] List<InfoPanel> _infoPanels;
    [SerializeField] GameObject _infoPanelsInScene;
    [SerializeField] GameObject _pivotPoint;

    GameObject _infoPanelPrefab;
    private bool isHover;
    private void Awake()
    {
        _infoPanelPrefab = Resources.Load<GameObject>("UI/InfoPanel");
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (gameObject.GetComponent<CardContainer>()) return; 
        isHover = true;
        Debug.Log("spawnInfoPanel");
        _infoPanelsInScene = Instantiate(_pivotPoint, _pivotPoint.transform.position, Quaternion.identity, CanvasManager.Instance().CardInfoPanelCanvas.transform);
        foreach (InfoPanel info in _infoPanels)
        {
            GameObject newInfo = Instantiate(_infoPanelPrefab, _infoPanelsInScene.transform);
            newInfo.GetComponent<InfoPanelUI>().SetUpUIElements(info);
            StartCoroutine(FollowParent());
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHover = false;
        Destroy(_infoPanelsInScene);
    }
    public void SetShowInfoPanel(bool isShow)
    {
        if (isShow)
        {
            foreach (InfoPanel info in _infoPanels)
            {
                GameObject newInfo = Instantiate(_infoPanelPrefab, _pivotPoint.transform);
                newInfo.GetComponent<InfoPanelUI>().SetUpUIElements(info);
                StartCoroutine(FollowParent());
            }
        }
        else
        {
            Destroy(_infoPanelsInScene);
        }
    }
    IEnumerator FollowParent()
    {
        while (isHover)
        {
            _infoPanelsInScene.transform.position = _pivotPoint.transform.position;
            
            yield return new WaitForSeconds(0.01f);
        }
        
    }
    public void SetInfoPanel(List<InfoPanel> newInfoPanels)
    {
        _infoPanels = newInfoPanels;
    }
    public void SetUpUIElements(Card card)
    {
        _title.text = card.title;
        _description.text = card.description;
        _mana.text = "" + card.Getcost();
        _image.sprite = card.icon;
        _frame.color = card.GetRarityColor((int)card.rarity);
        _base.color = card.GetRarityColor((int)card.rarity);
        SetInfoPanel(card.info);
    }
    public void SetUpUIElements(Card card, bool showInfoPanels)
    {
        _title.text = card.title;
        _description.text = card.description;
        _mana.text = "" + card.Getcost();
        _image.sprite = card.icon;
        _frame.color = card.GetRarityColor((int)card.rarity);
        _base.color = card.GetRarityColor((int)card.rarity);
        SetInfoPanel(card.info);
        SetShowInfoPanel(showInfoPanels);
    }

}

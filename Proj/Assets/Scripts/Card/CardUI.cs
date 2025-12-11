
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
    [SerializeField] List<GameObject> _infoPanelInScene;
    [SerializeField] Transform _pivotPoint;

    GameObject _infoPanelPrefab;
    private bool isHover;
    private void Awake()
    {
        _infoPanelPrefab = Resources.Load<GameObject>("UI/InfoPanel");
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHover = true;
        foreach (InfoPanel info in _infoPanels)
        {
            _infoPanelInScene.Add(Instantiate(_infoPanelPrefab, _pivotPoint.position, Quaternion.identity, CanvasManager.Instance().OverlayCanvas.transform));
            _infoPanelInScene.Last<GameObject>().GetComponent<InfoPanelUI>().SetUpUIElements(info);
            StartCoroutine(FollowParent());
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHover = false;
        foreach (GameObject go in _infoPanelInScene)
        {
            Destroy(go);
        }
        _infoPanelInScene.Clear();
    }
    IEnumerator FollowParent()
    {
        while (isHover)
        {
            foreach(GameObject GO in _infoPanelInScene)
            {
                GO.transform.position = _pivotPoint.position;
            }
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
    
}

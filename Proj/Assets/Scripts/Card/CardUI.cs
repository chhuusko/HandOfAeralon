
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
    [SerializeField] Image _frame, _image;
    [SerializeField] List<InfoPanel> _infoPanels;
    [SerializeField] GameObject _infoPanelPrefab;
    [SerializeField] List<GameObject> _infoPanelInScene;
    public void OnPointerEnter(PointerEventData eventData)
    {
        foreach (InfoPanel info in _infoPanels)
        {
            Vector3 offset = transform.position + new Vector3(GetComponent<RectTransform>().rect.width, 0, 0) * 0.5f;
            _infoPanelInScene.Add(Instantiate(_infoPanelPrefab, offset, Quaternion.identity, CanvasManager.Instance().OverlayCanvas.transform));
            _infoPanelInScene.Last<GameObject>().GetComponent<InfoPanelUI>().SetUpUIElements(info);
        }
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
        foreach (GameObject go in _infoPanelInScene)
        {
            Destroy(go);
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
        _mana.text = "" + card.cost;
        _image.sprite = card.icon;
        SetInfoPanel(card.info);
    }
    
}

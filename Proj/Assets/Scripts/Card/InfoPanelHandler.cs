using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InfoPanelHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool isHover;
    [SerializeField] private GameObject _pivotPoint;
    private GameObject _infoPanelPrefab;
    private GameObject _infoPanelsInScene;
    List<InfoPanel> _infoPanels = new List<InfoPanel>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _infoPanelPrefab = Resources.Load<GameObject>("UI/InfoPanel");
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log(_infoPanels.Count);
        isHover = true;
        _infoPanelsInScene = Instantiate(_pivotPoint, _pivotPoint.transform.position, Quaternion.identity, CanvasManager.Instance().CardInfoPanelCanvas.transform);
        if (_infoPanels.Count <= 0) return;
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
        Debug.Log(newInfoPanels.Count);
        _infoPanels = newInfoPanels;
    }
}

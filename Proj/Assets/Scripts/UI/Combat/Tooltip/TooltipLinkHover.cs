using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class TooltipLinkHover : MonoBehaviour, IPointerMoveHandler
{
    [SerializeField] private TMP_Text _textLink;
    [SerializeField] private Camera _camera;
    private int lastLinkIndex = -1;

    public void OnPointerMove(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(_textLink, eventData.position, _camera);

        if(linkIndex != -1 && linkIndex != lastLinkIndex)
        {
            lastLinkIndex = linkIndex;

            TMP_LinkInfo linkInfo = _textLink.textInfo.linkInfo[linkIndex];
            string linkId = linkInfo.GetLinkID();
            string linkText = linkInfo.GetLinkText();
            
            ShowSubTooltip(linkId, linkText);
     
        }
        else if(linkIndex != -1)
        {
            lastLinkIndex = -1;
        }
    }

    private void ShowSubTooltip(string linkId, string linkText)
    {
        DebugLog.CJLog("linkID: " + linkId + "linkText: " + linkText);
    }

    private void GenerateTooltip()
    {

    }
}

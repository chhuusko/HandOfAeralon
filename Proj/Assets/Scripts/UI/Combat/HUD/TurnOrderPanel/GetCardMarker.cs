using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GetCardMarker : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _title;

   
    private void Start()
    {
        SetMaskable(true);
    }

    public void SetMaskable(bool maskable)
    {
        _image.maskable = maskable;
        _title.maskable = maskable;

        //NOTE (Calle): Must disable and enable the objects so that Unity
        // can update the MaskState.    (I think...)
        _image.enabled = false;
        _image.enabled = true;

        _title.enabled = false;
        _title.enabled = true;
    }

    public float GetRightSidePosition()
    {
        Vector3[] corners = new Vector3[4];
        GetComponent<RectTransform>().GetWorldCorners(corners);
        return corners[2].x;
    }

    public float GetLeftSidePosition()
    {
        Vector3[] corners = new Vector3[4];
        GetComponent<RectTransform>().GetWorldCorners(corners);
        return corners[0].x;
    }
}

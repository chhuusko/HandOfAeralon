using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnOrderRoundMarker : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _currentRound;

    private bool _isMasked = false;
    private void Update()
    {
        //if(Input.GetMouseButtonDown(0))
        //{
        //    if(_isMasked)
        //    {
        //        _isMasked = false;
        //        SetMaskable(_isMasked);
        //        
        //    }
        //    else
        //    {
        //        _isMasked = true;
        //        SetMaskable(_isMasked);
        //    }
        //}
    }
    private void Start()
    {
        CombatEventManager.OnRoundFinished += SetCurrentRound;
        SetMaskable(true);
    }

    private void OnDisable()
    {
        CombatEventManager.OnRoundFinished -= SetCurrentRound;
    }

    public void SetCurrentRound(int currentRound)
    {
        _currentRound.text = currentRound.ToString();
    }

    public void SetMaskable(bool maskable)
    {
        _image.maskable = maskable;
        _title.maskable = maskable;
        _currentRound.maskable = maskable;

        //NOTE (Calle): Must disable and enable the objects so that Unity
        // can update the MaskState.    (I think...)
        _image.enabled = false;
        _image.enabled = true;

        _title.enabled = false;
        _title.enabled = true;

        _currentRound.enabled = false;
        _currentRound.enabled = true;
    }

    public float GetRightSidePosition()
    {
        Vector3[] corners = new Vector3[4];
        _currentRound.GetComponent<RectTransform>().GetWorldCorners(corners);

        return corners[2].x;
    }
}

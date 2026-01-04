using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnOrderRoundMarker : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _currentRound;
    


    private void Start()
    {
        CombatEventManager.OnRoundFinished += SetCurrentRound;
        _image.maskable = false;
    }

    private void OnDisable()
    {
        CombatEventManager.OnRoundFinished -= SetCurrentRound;
    }

    public void SetCurrentRound(int currentRound)
    {
        _currentRound.text = currentRound.ToString();
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnOrderRoundMarker : MonoBehaviour
{
    [SerializeField] private TMP_Text _currentRound;

    private void Start()
    {
        CombatEventManager.OnRoundFinished += SetCurrentRound;
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

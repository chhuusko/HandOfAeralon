using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnOrder : MonoBehaviour
{
    [SerializeField] private GameObject _turnOrderPanel;
    [SerializeField] private GameObject _panelViewPort;
    [SerializeField] private ScrollRect _turnOrderScrollRect;
    [SerializeField] private GameObject _roundMarkerPrefab;
    private TurnOrderRoundMarker _turnOrderRoundMarker;

    private void OnEnable()
    {
        CombatEventManager.OnTurnOrderChanged += UpdateTurnOrder;
    }


    private void Update()
    {
        float markerRightSiderPos = _turnOrderRoundMarker.GetRightSidePosition();

        Vector3[] viewPortCorners = new Vector3[4];

        _panelViewPort.GetComponent<RectTransform>().GetWorldCorners(viewPortCorners);

        float panelRightSidePos = viewPortCorners[3].x;
        if (markerRightSiderPos > panelRightSidePos)
            _turnOrderRoundMarker.SetMaskable(true);
        else
            _turnOrderRoundMarker.SetMaskable(false);
    }
    private void UpdateTurnOrder(IReadOnlyList<Character> characters, int currentRound)
    {
        // Clear previous portraits.
        for (int i = 0; i < _turnOrderPanel.transform.childCount; i++)
        {
            Destroy(_turnOrderPanel.transform.GetChild(i).gameObject);
        }

        int turnOrderIndex = 0;
        CombatTurnOrder combatTurnOrder = CombatManager._instance.GetCombatTurnOrder();
        // Create portraits for current turn order.
        foreach (Character c in characters)
        {
            if (turnOrderIndex++ == combatTurnOrder.GetFullRoundMarkerPosition())
            {
                GameObject roundMarkerObject = Instantiate(_roundMarkerPrefab, _turnOrderPanel.transform);
                TurnOrderRoundMarker turnOrderRoundMarker = roundMarkerObject.GetComponent<TurnOrderRoundMarker>();
                turnOrderRoundMarker.SetCurrentRound(currentRound);
                _turnOrderRoundMarker = turnOrderRoundMarker;
            }

            PortraitButton pb = CombatUI.Instance.CreateCharacterPortrait(c, _turnOrderPanel.transform);
            CombatUI.Instance._characterPortraits.TryAdd(pb.Character.Data, pb);
        }
        
        StartCoroutine(ResetTurnOrder());
    }

    public IEnumerator ResetTurnOrder()
    {
        yield return null;
        
        // Set scroll to bottom.
        _turnOrderScrollRect.verticalNormalizedPosition = 0;
    }

    private void OnDisable()
    {
        CombatEventManager.OnTurnOrderChanged -= UpdateTurnOrder;
    }
}

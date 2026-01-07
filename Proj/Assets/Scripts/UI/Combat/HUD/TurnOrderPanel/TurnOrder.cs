using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TurnOrder : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _turnOrderPanel;
    [SerializeField] private GameObject _panelViewPort;
    [SerializeField] private ScrollRect _turnOrderScrollRect;
    [SerializeField] private GameObject _roundMarkerPrefab;
    [SerializeField] private GameObject _getCardMarkerPrefab;
    private TurnOrderRoundMarker _turnOrderRoundMarker;
    private GetCardMarker _getCardMarker;

    private void OnEnable()
    {
        CombatEventManager.OnTurnOrderChanged += UpdateTurnOrder;
    }

    private void Update()
    {
        Vector3[] viewPortCorners = new Vector3[4];
        _panelViewPort.GetComponent<RectTransform>().GetWorldCorners(viewPortCorners);
        float panelRightSidePos = viewPortCorners[3].x;

        if (_turnOrderRoundMarker)
        {
            float roundMarkerRightSiderPos = _turnOrderRoundMarker.GetRightSidePosition();
            if (roundMarkerRightSiderPos > panelRightSidePos)
                _turnOrderRoundMarker.SetMaskable(true);
            else
                _turnOrderRoundMarker.SetMaskable(false);

        }

        if (_getCardMarker)
        {
            float getCardMarkerRightSiderPos = _getCardMarker.GetRightSidePosition();
            if (getCardMarkerRightSiderPos > panelRightSidePos)
                _getCardMarker.SetMaskable(true);
            else
                _getCardMarker.SetMaskable(false);

        }
    }
    
    private void UpdateTurnOrder(IReadOnlyList<Character> characters, int currentRound)
    {
        // Clear previous portraits.
        for (int i = 0; i < _turnOrderPanel.transform.childCount; i++)
        {
            Destroy(_turnOrderPanel.transform.GetChild(i).gameObject);
        }

        int turnOrderIndex = 0;
        bool getCardMarkerDisplayed = false;

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

            if (c.GetFaction() == Faction.Friendly)
            {
                if (combatTurnOrder.IsNextRoundGetCard() && !getCardMarkerDisplayed)
                {
                    if (turnOrderIndex > combatTurnOrder.GetFullRoundMarkerPosition())
                    {
                        GameObject getCardMarkerObject = Instantiate(_getCardMarkerPrefab, _turnOrderPanel.transform);
                        _getCardMarker = getCardMarkerObject.GetComponent<GetCardMarker>();
                        getCardMarkerDisplayed = true;
                    }
                }
                else if(combatTurnOrder.IsGetCardRound()             && 
                        !getCardMarkerDisplayed                      && 
                        combatTurnOrder.IsFriendlyInPendingOrder()   && 
                        !combatTurnOrder.IsFriendlyInExecutedOrder() &&
                        combatTurnOrder.GetActiveCharacter().GetFaction() != Faction.Friendly)
                {
                    GameObject getCardMarkerObject = Instantiate(_getCardMarkerPrefab, _turnOrderPanel.transform);
                    _getCardMarker = getCardMarkerObject.GetComponent<GetCardMarker>();
                    getCardMarkerDisplayed = true;
                }
            }

            PortraitButton pb = CombatUI.Instance.CreateCharacterPortrait(c, _turnOrderPanel.transform);
            CombatUI.Instance._characterPortraits.TryAdd(pb.Character, pb);
        }
        
        StartCoroutine(ResetTurnOrder());
    }

    public IEnumerator ResetTurnOrder()
    {
        yield return null;
        
        // Set scroll to bottom.
        _turnOrderScrollRect.verticalNormalizedPosition = 0;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        CombatEventManager.InvokeOnIsHoveringUI(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CombatEventManager.InvokeOnIsHoveringUI(false);
    }

    private void OnDisable()
    {
        CombatEventManager.OnTurnOrderChanged -= UpdateTurnOrder;
    }
}

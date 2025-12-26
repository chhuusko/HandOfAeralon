using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MovementPointsUI : MonoBehaviour
{
    [SerializeField] private GameObject movementPointPrefab;
    
    [SerializeField] private Color _unspentMovementPointColor;
    [SerializeField] private Color _spentMovementPointColor;
    
    private Character _currentCharacter;

    private void OnEnable()
    {
        StartCoroutine(WaitForSelector());
    }
    
    private IEnumerator WaitForSelector()
    {
        while (!Selector._instance)
        {
            yield return null;
        }
        
        UpdateSelectedCharacter(Selector._instance.GetSelectedCharacter());
    }

    // Resets which character's movement UI should be updated.
    private void UpdateSelectedCharacter(Character character)
    {
        if (_currentCharacter)
        {
            _currentCharacter.OnMovementPointsChanged -= UpdateMovementPoints;
        }
        
        _currentCharacter = character;

        if (_currentCharacter)
        {
            _currentCharacter.OnMovementPointsChanged += UpdateMovementPoints;
            UpdateMovementPoints(_currentCharacter.GetMovementPoints(), _currentCharacter.Data.BaseMovementPoints);
        }
    }

    private void ClearMovementPoints()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
    
    private void UpdateMovementPoints(int currentMovementPoints, int baseMovementPoints)
    {
        if (!CombatUI.Instance || !CombatUI.Instance.bCombatStarted)
        {
            return;
        }
        
        ClearMovementPoints();

        for (int i = 0; i < baseMovementPoints; i++)
        {
            var movementPoint = Instantiate(movementPointPrefab, transform);
            movementPoint.GetComponent<Image>().color = 
                i < currentMovementPoints ? _unspentMovementPointColor : _spentMovementPointColor;
        }
    }

    private void OnDisable()
    {
        if (_currentCharacter)
        {
            _currentCharacter.OnMovementPointsChanged -= UpdateMovementPoints;
        }
    }
}

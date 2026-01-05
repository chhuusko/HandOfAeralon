using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovementPointsUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject movementPointPrefab;
    [SerializeField] private List<Image> _movementPoints = new();
    
    [Header("Colors")]
    [SerializeField] private Color _unspentMovementPointColor;
    [SerializeField] private Color _spentMovementPointColor;
    [SerializeField] private Color _previewMovementPointColor;
    
    private Character _currentCharacter;
    private CharacterMovement _currentMovement;
    
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
        if (_currentMovement)
        {
            _currentMovement.MovementCostPreview.RemoveListener(PreviewMovementPoints);
            _currentMovement.OnMovementPreviewStopped.RemoveListener(UpdateMovementPoints);
        }
        
        if (_currentCharacter)
        {
            _currentCharacter.OnMovementPointsChanged -= UpdateMovementPoints;
        }
        _currentCharacter = character;

        if (!_currentCharacter)
        {
            return;
        }
        
        _currentMovement = _currentCharacter.GetComponent<CharacterMovement>();
        _currentMovement.MovementCostPreview.AddListener(PreviewMovementPoints);
        _currentMovement.OnMovementPreviewStopped.AddListener(UpdateMovementPoints);
        _currentCharacter.OnMovementPointsChanged += UpdateMovementPoints;
        UpdateMovementPoints(_currentCharacter.GetMovementPoints(), _currentCharacter.Data.BaseMovementPoints);
    }

    /// <summary>
    /// Updates movement point UI to show the cost of moving along the currently selected path, if it is confirmed.
    /// </summary>
    /// <param name="cost">The cost of moving the previewed path.</param>
    private void PreviewMovementPoints(int cost)
    {
        UpdateMovementPoints(_currentCharacter.GetMovementPoints(), _currentCharacter.Data.BaseMovementPoints);
        
        int start = _currentCharacter.GetMovementPoints() - 1;
        int end = Mathf.Max(0, _currentCharacter.GetMovementPoints() - cost);

        for (int i = start; i >= end; i--)
        {
            _movementPoints[i].color = _previewMovementPointColor;
        }
    }

    // Resets all movement points.
    private void HideMovementPoints()
    {
        foreach (var point in _movementPoints)
        {
            point.gameObject.SetActive(false);   
        }
    }

    private void UpdateMovementPoints()
    {
        UpdateMovementPoints(_currentCharacter.GetMovementPoints(), _currentCharacter.Data.BaseMovementPoints);
    }
    
    /// <summary>
    /// Updates the movement points panel, showing max and current movementpoints by enabling objects from the pool.
    /// </summary>
    /// <param name="current">Current movement points.</param>
    /// <param name="max">Maximum movement points.</param>
    private void UpdateMovementPoints(int current, int max)
    {
        if (!CombatUI.Instance || !CombatUI.Instance.bCombatStarted)
        {
            return;
        }

        if (max > _movementPoints.Count)
        {
            ExpandMovementPointsList(max - _movementPoints.Count);
        }
        
        HideMovementPoints();

        // Set objects from the pool as active.
        for (int i = 0; i < max; i++)
        {
            var movementPoint = _movementPoints[i];
            movementPoint.gameObject.SetActive(true);
            movementPoint.color = 
                i < current ? _unspentMovementPointColor : _spentMovementPointColor;
        }
    }

    /// <summary>
    /// Adds more points to the pool if there are none left.
    /// </summary>
    /// <param name="amount">The amount of points to add.</param>
    private void ExpandMovementPointsList(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            var movementPoint = Instantiate(movementPointPrefab, transform).GetComponent<Image>();
            _movementPoints.Add(movementPoint);
        }
    }

    private void OnDisable()
    {
        if (_currentMovement)
        {
            _currentMovement.MovementCostPreview.RemoveListener(PreviewMovementPoints);
            _currentMovement.OnMovementPreviewStopped.RemoveListener(UpdateMovementPoints);
        }
        
        if (_currentCharacter)
        {
            _currentCharacter.OnMovementPointsChanged -= UpdateMovementPoints;
        }
    }
}

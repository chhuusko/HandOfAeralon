using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private List<CombatGridTile> _tilesInRange = new();
    private List<CombatGridTile> _pathPreview = new();
    private bool _bIsMoving = false;

    public void Reset()
    {
        _tilesInRange = new();
        _pathPreview = new();
    }

    public List<CombatGridTile> GetTilesInRange()
    {
        return _tilesInRange;
    }

    public List<CombatGridTile> GetPathPreview()
    {
        return _pathPreview;
    }

    public bool IsMoving()
    {
        return _bIsMoving;
    }

    public void ConfirmPreviewedPath()
    {
        if (_pathPreview == null || _pathPreview.Count == 0)
        {
            Debug.LogError($"CharacterMovement.cs | _pathPreview IS EMPTY!");
            return;
        }

        StartCoroutine(Move(_pathPreview));
    }

    public void DrawMoveRange()
    {
        Character character = null;
        GameObject currentTile = null;

        if (TryGetComponent<Character>(out character))
        {
            currentTile = character.GetCurrentTileComponent().gameObject;

            _tilesInRange = GridExplorer._instance.GetTilesInRange(currentTile, character.GetMovementPoints(), true)
            .Select(obj => obj.GetComponent<CombatGridTile>())
            .Where(ch => ch != null)
            .ToList();

            foreach (var tile in _tilesInRange)
            {
                tile.SetTileColor(Color.green);
            }

            return;
        }

        _tilesInRange = new();
    }

    public void PreviewPath(CombatGridTile goalTile)
    {
        Character character = null;
        GameObject currentTile = null;

        if (TryGetComponent<Character>(out character))
        {
            currentTile = character.GetCurrentTileComponent().gameObject;

            _pathPreview = GridExplorer._instance.FindPathAStar(currentTile, goalTile.gameObject)
            .Select(obj => obj.GetComponent<CombatGridTile>())
            .Where(ch => ch != null)
            .ToList();

            return;
        }

        _pathPreview = new();
    }

    public void ForceCustomPath(List<CombatGridTile> path)
    {
        if (path == null || path.Count == 0)
        {
            Debug.LogError($"CharacterMovement.cs | path IS EMPTY!");
            return;
        }

        StartCoroutine(Move(path));
    }

    private IEnumerator Move(List<CombatGridTile> path)
    {
        _bIsMoving = true;

        float moveSpeed = 4f; // Måste matcha animationerna

        foreach (var step in path)
        {
            Vector3 targetPos = step.transform.position;

            Vector3 direction = (targetPos - transform.position).normalized;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }

            DebugLog.JLWLog($"CharacterMovement.cs | {this.name} moving towards {targetPos}");

            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPos,
                    moveSpeed * Time.deltaTime
                );

                yield return null;
            }

            transform.position = targetPos;

            //GetComponent<CombatGridTile>().SetOccupant(this.gameObject);
        }

        _bIsMoving = false;
    }
}

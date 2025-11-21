using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterMovement : MonoBehaviour
{
    private Character _character;
    private List<CombatGridTile> _tilesInRange = new();
    private List<CombatGridTile> _pathPreview = new();
    private bool _bIsMoving = false;

    void Start()
    {
        _character = GetComponent<Character>();
        if (_character == null)
        {
            Debug.LogError($"CharacterMovement.cs | _character NOT FOUND!");
        }
    }

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

    public void ConfirmPath(CombatGridTile tile)
    {
        if (_pathPreview == null || _pathPreview.Count == 0)
        {
            Debug.LogError($"CharacterMovement.cs | _pathPreview IS EMPTY!");
            return;
        }

        if (_pathPreview[^1] == tile)
        {
            if (_character.GetMovementPoints() == 0)
            {
                Debug.LogError($"CharacterMovement.cs | {_character.name} is out of MP!");
                _tilesInRange = new();
                return;
            }

            _character.SetCurrentMovementPoints(Mathf.Max(_character.GetMovementPoints() - _pathPreview.Count, 0));

            StartCoroutine(Move(_pathPreview));
        }
    }

    public void DrawMoveRange()
    {
        GameObject currentTile = _character.GetCurrentTileComponent().gameObject;
        if (_character.GetMovementPoints() <= 0)
        {
            Debug.Log($"CharacterMovement.cs | {_character.name} is out of MP!");
            _tilesInRange = new();
            return;
        }

        Debug.Log($"CharacterMovement.cs | {_character.name} move range drawn.");
        _tilesInRange = GridExplorer._instance.GetTilesInRange(currentTile, _character.GetMovementPoints(), true)
        .Select(obj => obj.GetComponent<CombatGridTile>())
        .Where(ch => ch != null)
        .ToList();
    }

    public void PreviewPath(CombatGridTile tile)
    {
        if (tile == null || !_tilesInRange.Contains(tile))
        {
            GridExplorer._instance.Clear();
            return;
        }

        GameObject currentTile = null;
        currentTile = _character.GetCurrentTileComponent().gameObject;

        _pathPreview = GridExplorer._instance.FindPathAStar(currentTile, tile.gameObject)
        .Select(obj => obj.GetComponent<CombatGridTile>())
        .Where(ch => ch != null)
        .ToList();
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

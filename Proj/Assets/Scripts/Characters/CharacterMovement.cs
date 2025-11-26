using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private Character _character;
    private List<CombatGridTile> _tilesInRange = new();
    private List<CombatGridTile> _pathPreview = new();
    private bool _bIsMoving = false;
    private CombatGridTile _lastPreviewPathTile = null;

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
        _lastPreviewPathTile = null;
    }

    public bool IsMoving()
    {
        return _bIsMoving;
    }

    public void DrawMoveRange()
    {
        GameObject currentTile = _character.GetCurrentTileComponent().gameObject;
        if (_character.GetMovementPoints() <= 0 || !_character.CanMove)
        {
            DebugLog.JLWLog($"CharacterMovement.cs | {_character.name} can't move!");
            _tilesInRange = new();
            return;
        }

        DebugLog.JLWLog($"CharacterMovement.cs | {_character.name} move range drawn.");
        _tilesInRange = GridExplorer._instance.GetTilesInRange(currentTile, _character.GetMovementPoints(), true)
        .Select(obj => obj.GetComponent<CombatGridTile>())
        .Where(ch => ch != null)
        .ToList();
    }

    public void PreviewPath(CombatGridTile tile)
    {
        if (tile == null || !_tilesInRange.Contains(tile))
        {
            _lastPreviewPathTile = null;
            GridExplorer._instance.Clear();
            return;
        }

        if (_bIsMoving || tile == _lastPreviewPathTile || _character.GetMovementPoints() <= 0)
        {
            DebugLog.JLWLog($"CharacterMovement::PreviewPath() skipped");
            return;
        }

        _lastPreviewPathTile = tile;

        GameObject currentTile = null;
        currentTile = _character.GetCurrentTileComponent().gameObject;

        DebugLog.JLWLog($"CharacterMovement::PreviewPath() called A*");
        _pathPreview = GridExplorer._instance.FindPathAStar(currentTile, tile.gameObject)
        .Select(obj => obj.GetComponent<CombatGridTile>())
        .Where(ch => ch != null)
        .ToList();
    }

    public void ConfirmPath(CombatGridTile tile)
    {
        if (_pathPreview == null || _pathPreview.Count == 0)
        {
            DebugLog.JLWLog($"CharacterMovement.cs | _pathPreview IS EMPTY!");
            return;
        }

        if (_pathPreview[^1] == tile)
        {
            if (_character.GetMovementPoints() <= 0)
            {
                DebugLog.JLWLog($"CharacterMovement.cs | {_character.name} is out of MP!");
                _tilesInRange = new();
                return;
            }

            _character.DecreaseCurrentMovementPoints(_pathPreview.Count - 1);

            StartCoroutine(Move(_pathPreview));
        }
    }

    public void ForceCustomPath(List<CombatGridTile> path)
    {
        if (path == null || path.Count == 0)
        {
            DebugLog.JLWLog($"CharacterMovement.cs | path IS EMPTY!");
            return;
        }

        StartCoroutine(Move(path));
    }

    private IEnumerator Move(List<CombatGridTile> path)
    {
        _bIsMoving = true;
        GridExplorer._instance.Clear();
        float moveSpeed = 4f; // Måste matcha animationerna

        Animator animator = null;
        if (_character.TryGetComponent<Animator>(out animator))
        {
            //Debug.LogError($"{_character.name} går!");
            animator.SetBool("IsMoving", true);
        }

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
        }

        _bIsMoving = false;

        if (animator != null)
        {
            //Debug.LogError($"{_character.name} stannade!");
            animator.SetBool("IsMoving", false);
        }

        DrawMoveRange();
    }
}

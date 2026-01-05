// Joel Larsson Wendt || jola6902

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class CharacterMovement : MonoBehaviour
{
    public UnityEvent<int> MovementCostPreview;

    private Character _character;
    private List<CombatGridTile> _tilesInRange = new();
    private List<CombatGridTile> _pathPreview = new();
    private bool _bIsMoving = false;
    private CombatGridTile _lastPreviewPathTile = null;
    private Animator _animator;

    void Start()
    {
        _character = GetComponent<Character>();
        if (_character == null)
        {
            Debug.LogError($"CharacterMovement.cs | _character NOT FOUND!");
        }

        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError($"CharacterMovement.cs | _animator NOT FOUND!");
        }
    }

    void Update()
    {
        _animator.SetBool("IsMoving", _bIsMoving);
    }

    public bool IsMoving()
    {
        return _bIsMoving;
    }

    public void DrawMoveRange()
    {
        if (IsDead()) return;

        GameObject currentTile = _character.GetCurrentTileComponent().gameObject;
        if (_character.GetMovementPoints() <= 0 || !_character.CanMove || _character.IsStunned)
        {
            //DebugLog.JLWLog($"CharacterMovement.cs | {_character.name} can't move!");
            _tilesInRange = new();
            return;
        }

        //DebugLog.JLWLog($"CharacterMovement.cs | {_character.name} move range drawn.");
        _tilesInRange = GridExplorer._instance.GetReachableTilesWithMovement(currentTile, _character.GetMovementPoints())
        .Select(obj => obj.GetComponent<CombatGridTile>())
        .Where(ch => ch != null)
        .ToList();
        
        if (_character.GetFaction() == Faction.Friendly)
        {
            Selector._instance.SetColorOfTiles(_tilesInRange, Color.green);
        }
    }

    public void ForgetMoveRange()
    {
        _tilesInRange = new();
    }

    public void PreviewPath(CombatGridTile tile)
    {
        if (IsDead() || !_character.CanMove || _character.IsStunned) return;

        if (_bIsMoving || tile == _character.GetCurrentTileComponent() || tile == null || !_tilesInRange.Contains(tile) || CombatManager._instance.GetCombatTurnOrder().GetActiveCharacter().GetFaction() != Faction.Friendly)
        {
            _lastPreviewPathTile = null;
            GridExplorer._instance.ClearPathDrawing();
            return;
        }

        if (tile == _lastPreviewPathTile || _character.GetMovementPoints() <= 0)
        {
            //DebugLog.JLWLog($"CharacterMovement::PreviewPath() skipped");
            return;
        }

        _lastPreviewPathTile = tile;

        GameObject currentTile = null;
        currentTile = _character.GetCurrentTileComponent().gameObject;

        //DebugLog.JLWLog($"CharacterMovement::PreviewPath() called A*");
        _pathPreview = GridExplorer._instance.FindPathAStar(currentTile, tile.gameObject, true, _tilesInRange)
        .Select(obj => obj.GetComponent<CombatGridTile>())
        .Where(ch => ch != null)
        .ToList();

        int cost = CalculateMovementCost(_pathPreview);
        MovementCostPreview.Invoke(cost);
    }

    public bool ConfirmPath(CombatGridTile tile)
    {
        if (IsDead() || !_character.CanMove || _character.IsStunned) return false;

        if (_bIsMoving || tile == _character.GetCurrentTileComponent() || _pathPreview == null || _pathPreview.Count == 0)
        {
            //DebugLog.JLWLog($"CharacterMovement.cs | _pathPreview IS EMPTY!");
            return false;
        }

        if (_pathPreview[^1] == tile)
        {
            if (_character.GetMovementPoints() <= 0)
            {
                //DebugLog.JLWLog($"CharacterMovement.cs | {_character.name} is out of MP!");
                _tilesInRange = new();
                return false;
            }

            _character.DecreaseCurrentMovementPoints(CalculateMovementCost(_pathPreview));

            StartCoroutine(Move(_pathPreview));
        }

        return true;
    }

    public void ForceCustomPath(List<CombatGridTile> path)
    {
        if (IsDead() || !_character.CanMove || _character.IsStunned) return;

        if (path == null || path.Count == 0)
        {
            DebugLog.JLWLog($"CharacterMovement.cs | path IS EMPTY!");
            return;
        }

        StartCoroutine(Move(path));
    }

    private IEnumerator Move(List<CombatGridTile> path)
    {
        if (IsDead() || !_character.CanMove || _character.IsStunned) yield break;

        _bIsMoving = true;
        Debug.LogWarning($"{_character.name} _isMoving = true");
        CombatEventManager.InvokeOnCharacterMove(_character, _bIsMoving);
        Selector._instance.InvokeCharacterActionStarted();
        GridExplorer._instance.ClearPathDrawing();
        float moveSpeed = 4f; // M�ste matcha animationerna

        foreach (var step in path)
        {
            Vector3 targetPos = step.transform.position;

            Vector3 direction = (targetPos - transform.position).normalized;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }

            //DebugLog.JLWLog($"CharacterMovement.cs | {this.name} moving towards {targetPos}");

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
        Debug.LogWarning($"{_character.name} _isMoving = false");
        CombatEventManager.InvokeOnCharacterMove(_character, _bIsMoving);
        Selector._instance.InvokeCharacterActionStopped();

        DrawMoveRange();
    }

    private int CalculateMovementCost(List<CombatGridTile> path)
    {
        if (IsDead() || !_character.CanMove || _character.IsStunned) return 0;

        int result = 0;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2Int a = path[i - 1].GetTileIndex();
            Vector2Int b = path[i].GetTileIndex();

            bool diagonal = Mathf.Abs(a.x - b.x) == 1 && Mathf.Abs(a.y - b.y) == 1;

            result += diagonal ? 2 : 1;
        }

        return result;
    }

    private bool IsDead()
    {
        bool bIsDead = _character == null || _character.GetCurrentHealth() <= 0;

        if (bIsDead)
        {
            _tilesInRange = new();
            _pathPreview = new();
            _bIsMoving = false;
            _lastPreviewPathTile = null;
        }

        return bIsDead;
    }
}

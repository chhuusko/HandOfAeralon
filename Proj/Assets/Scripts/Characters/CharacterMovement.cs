using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private bool _bIsMoving = false;

    public bool IsMoving()
    {
        return _bIsMoving;
    }

    public void MoveAlongPath(List<GameObject> path)
    {
        if (path == null || path.Count == 0)
        {
            Debug.LogError($"CharacterMovement.cs 18 | MoveAlongPath called with an empty list!");
            return;
        }

        StartCoroutine(Move(path));
    }

    private IEnumerator Move(List<GameObject> path)
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

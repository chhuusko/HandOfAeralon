using UnityEngine;

public class DummyCharacter : MonoBehaviour
{
    private GameObject _owner;
    private int _moveRange = 3;
    private int _attackRange = 1;

    void Start()
    {
        _owner = GameObject.FindGameObjectWithTag("EnemyAI");
        if (_owner == null)
        {
            Debug.LogError("DummyCharacter.owner NOT FOUND IN SCENE!");
        }
    }

    void Update()
    {
        
    }

    public void MoveTo(GameObject target)
    {
        transform.position = target.transform.position;
    }

    public void Attack(GameObject target)
    {
        if (GridExplorer.Instance.ManhattanDistance(this.gameObject, target) <= _attackRange)
        {
            Debug.Log($"{this.name} attacked {target.name}!");
        }
    }

    public GameObject GetOwner()
    {
        return _owner;
    }

    public int GetMoveRange()
    {
        return _moveRange;
    }

    public int GetAttackRange()
    {
        return _attackRange;
    }

    public GameObject GetTile()
    {
        return null;
    }
}

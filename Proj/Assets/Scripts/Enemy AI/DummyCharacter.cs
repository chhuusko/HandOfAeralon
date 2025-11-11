using UnityEngine;

public class DummyCharacter : MonoBehaviour
{
    [SerializeField] private GameObject _owner;
    private int _moveRange = 3;
    private int _attackRange = 5;
    private CombatManager _combatManager;

    void Start()
    {
        _combatManager = FindFirstObjectByType<CombatManager>();
        if (_combatManager == null)
        {
            Debug.LogError("DummyCharacter._combatManager NOT FOUND IN SCENE!");
        }
    }

    public void MoveTo(GameObject target)
    {
        transform.position = target.transform.position;
    }

    public void Attack(GameObject target)
    {
        Debug.Log($"{this.name} attacked {target.name}!");
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
        Vector3 tileSize = _combatManager.GetTileSize();

        int x = Mathf.FloorToInt(transform.position.x / tileSize.x);
        int y = Mathf.FloorToInt(transform.position.z / tileSize.z);

        return _combatManager.GetTileAtCoord(x, y);
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CombatGridTile : MonoBehaviour
{
    [SerializeField] private CombatGridTileData _tileData;
    [SerializeField] private GameObject _occupant;

    public CombatGridTile(CombatGridTileData tileData)
    {
        _tileData = new CombatGridTileData(tileData.GetTileType(), 
                                      tileData.GetTileIndex(),
                                      tileData.GetTilePosition(),
                                      tileData.GetTileSize());
    }
    public GameObject GetOccupant()     { return _occupant; }
    public TileType   GetTileType()     { return _tileData.GetTileType(); } 
    public Vector2Int GetTileIndex()    { return _tileData.GetTileIndex(); }
    public Vector3    GetTilePosition() { return _tileData.GetTilePosition(); }
    public Vector3    GetTileSize()     { return _tileData.GetTileSize(); } 
    public bool       IsWalkable()      { return _tileData.IsWalkable(); }

    public void SetWalkable(bool bWalkable)        { _tileData.SetWalkable(bWalkable); }
    public void SetTileIndex(Vector2Int tileIndex) { _tileData.SetTileIndex(tileIndex); }
    public void SetTileType(TileType tileType)     { _tileData.SetTileType(tileType); }
    public void SetOccupant(GameObject occupant) { _occupant = occupant; }

    public Character GetOccupantCharacter()
    {
        if (_occupant == null) return null;

        _occupant.TryGetComponent(out Character character);
        return character;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Character"))
        {
            _occupant = gameObject;
            _occupant.GetComponent<Character>().SetCurrentTileIndex(GetTileIndex());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Character"))
        {
            _occupant = null;
        }
    }
}

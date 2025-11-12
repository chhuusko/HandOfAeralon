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
    public bool IsMouseHovering()
    {
        // Get a ray from the camera through the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Raycast against this tile’s collider
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, LayerMask.GetMask("Tile")))
        {
            // Check if the hit object is this tile
            return hitInfo.collider.gameObject == gameObject;
        }

        return false;
    }

    public void SetTileColor(Color color)
    {
        MeshRenderer meshRend = GetComponent<MeshRenderer>();
        if(meshRend != null)
        {
            meshRend.material.SetColor("_TileColor", color);
        }
    }

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
            _occupant = other.gameObject;
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

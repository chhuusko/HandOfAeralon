using System.Collections.Generic;
using UnityEngine;

public class CombatGridTile : MonoBehaviour
{
    [SerializeField] private CombatGridTileData data;
    public CombatGridTile(CombatGridTileData tileData)
    {
        data = new CombatGridTileData(tileData.GetTileType(), 
                                      tileData.GetTileIndex(),
                                      tileData.GetTilePosition(),
                                      tileData.GetTileSize());
    }
    public TileType GetTileType() {  return data.GetTileType(); } 
    public Vector2 GetTileIndex() { return data.GetTileIndex(); }
    public Vector3 GetTilePosition() { return data.GetTilePosition(); }
    public Vector3 GetTileSize() { return data.GetTileSize(); } 
    public bool IsWalkable() { return data.IsWalkable(); }

    public void SetWalkable(bool bWalkable) { data.SetWalkable(bWalkable); }
}

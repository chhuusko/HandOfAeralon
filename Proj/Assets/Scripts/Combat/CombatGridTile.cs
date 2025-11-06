using System.Collections.Generic;
using UnityEngine;



public class CombatGridTile : MonoBehaviour
{
    [SerializeField] private CombatGridTileData data;
    public CombatGridTile(CombatGridTileData tileData)
    {
        data = new CombatGridTileData(tileData.GetTileType(), 
                                      tileData.GetTilePosition());
    }
    public TileType GetTileType() {  return data.GetTileType(); } 
    public Vector2 GetTilePosition() { return data.GetTilePosition(); }
}

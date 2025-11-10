using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CombatGridSerializedSaveData
{
    public int _gridWidth;
    public int _gridHeight;
    public Vector3 _tileSize;
    public List<CombatGridTileData> _tileData = new List<CombatGridTileData>();

    public List<CombatGridCharacterData> _characterData = new List<CombatGridCharacterData>();

}

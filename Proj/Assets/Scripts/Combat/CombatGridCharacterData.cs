using UnityEditor.Build.Content;
using UnityEngine;

[System.Serializable]
public class CombatGridCharacterData
{
    [SerializeField] private CharacterClass _characterClass;
    [SerializeField] private Faction _faction;
    [SerializeField] private int _healthPoints;
    [SerializeField] private Vector2Int _tileIndex;
    [SerializeField] private Vector3 _position;
    [SerializeField] private Vector3 _size;
    
    public CombatGridCharacterData(CharacterClass characterClass, 
                                   Faction faction,        
                                   int healthPoints,
                                   Vector2Int tileIndex, 
                                   Vector3 position,
                                   Vector3 size)
    {
        _characterClass = characterClass;
        _faction = faction;
        _healthPoints = healthPoints;
        _tileIndex = tileIndex;
        _position = position;
        _size = size;
    }
    public CharacterClass GetCharacterClass() { return _characterClass; }
    public Faction GetFaction() {  return _faction; }
    public int GetHealthPoints() { return _healthPoints; }
    public Vector2Int GetTileIndex() { return _tileIndex; }
    public Vector3 GetCharacterPosition() { return _position; }
    public Vector3 GetCharacterSize() { return _size; }
    
    public void SetCharacterClass(CharacterClass characterClass) { _characterClass = characterClass; }
    public void SetFaction(Faction faction) { _faction = faction; }
    public void SetHealthPoints(int healtPoints) { _healthPoints = healtPoints; }
    public void SetTileIndex(Vector2Int tileIndex) { _tileIndex = tileIndex; }
}

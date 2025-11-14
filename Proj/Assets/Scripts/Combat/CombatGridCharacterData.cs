using UnityEditor.Build.Content;
using UnityEngine;

[System.Serializable]
public class CombatGridCharacterData
{
    [SerializeField] private CharacterClass _characterClass;
    [SerializeField] private Faction _faction;
    [SerializeField] private int _healthPoints;
    [SerializeField] private int _speed;
    [SerializeField] private Vector2Int _tileIndex;
    [SerializeField] private Vector3 _position;
    [SerializeField] private Vector3 _size;
    [SerializeField] private Quaternion _rotation;

    public CombatGridCharacterData(CharacterClass characterClass, 
                                   Faction faction,        
                                   int healthPoints,
                                   int speed,
                                   Vector2Int tileIndex, 
                                   Vector3 position,
                                   Vector3 size,
                                   Quaternion rotation)
    {
        _characterClass = characterClass;
        _faction = faction;
        _healthPoints = healthPoints;
        _speed = speed;
        _tileIndex = tileIndex;
        _position = position;
        _size = size;
        _rotation = rotation;
    }
    public CharacterClass GetCharacterClass() { return _characterClass; }
    public Faction GetFaction() {  return _faction; }
    public int GetHealthPoints() { return _healthPoints; }
    public int GetInitiative() {  return _speed; }
    public int GetSpeed() { return _speed; }
    public Vector2Int GetTileIndex() { return _tileIndex; }
    public Vector3 GetCharacterPosition() { return _position; }
    public Vector3 GetCharacterSize() { return _size; }
    public Quaternion GetRotation() { return _rotation; }   
    
    public void SetPosition(Vector3 position) { _position = position; }
    public void SetCharacterClass(CharacterClass characterClass) { _characterClass = characterClass; }
    public void SetFaction(Faction faction) { _faction = faction; }
    public void SetHealthPoints(int healthPoints) { _healthPoints = healthPoints; }
    public void SetInitiative(int initiative) {  _speed = initiative; }
    public void SetSpeed(int initiative) { _speed = initiative; }
    public void SetTileIndex(Vector2Int tileIndex) { _tileIndex = tileIndex; }
}

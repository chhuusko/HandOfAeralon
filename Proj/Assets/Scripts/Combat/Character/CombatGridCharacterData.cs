using UnityEngine;

[System.Serializable]
public class CombatGridCharacterData
{
    [Header("Character")]
    [SerializeField] private CharacterClass _characterClass;
    [SerializeField] private Faction _faction;

    [SerializeField] private Vector3 _position;
    [SerializeField] private Vector3 _size;
    [SerializeField] private Quaternion _rotation;
    
    [Header("Base stats")]
    [SerializeField] private int _baseHealthPoints;
    [SerializeField] private int _baseSpeed;
    [SerializeField] private int _baseDamage;

    [SerializeField] private int _baseMovementPoints;
  

    [Header("Current stats")]
    [SerializeField] private int _currentHealthPoints;
    [SerializeField] private int _currentSpeed;
    [SerializeField] private int _currentDamage;
    [SerializeField] private int _currentMovementPoints;

    [Header("Misc")]
    [SerializeField] private Vector2Int _currentTileIndex;

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
        _currentHealthPoints = healthPoints;
        _currentSpeed = speed;
        _currentTileIndex = tileIndex;
        _position = position;
        _size = size;
        _rotation = rotation;
    }
    public CombatGridCharacterData(GameObject characterGameObject)
    {
        if (characterGameObject == null)
            return;

        Character characterScript = characterGameObject.GetComponent<Character>();

        if(characterScript == null) 
            return;

        _position               = characterGameObject.transform.position;
        _size                   = characterGameObject.transform.localScale;
        _rotation               = characterGameObject.transform.rotation;

        _characterClass        = characterScript.GetCharacterClass();
        _faction               = characterScript.GetFaction();

        _baseHealthPoints      = characterScript.GetBaseHealthPoints();
        _baseSpeed             = characterScript.GetBaseSpeed();
        _baseDamage            = characterScript.GetBaseDamage();
        _baseMovementPoints    = characterScript.GetBaseMovementPoints();

        _currentHealthPoints   = characterScript.GetHealthPoints();
        _currentSpeed          = characterScript.GetInitiative();
        _currentDamage         = characterScript.GetDamage();
        _currentMovementPoints = characterScript.GetMovementPoints();

        _currentTileIndex      = characterScript.GetCurrentTileIndex();

    }


    public CharacterClass GetCharacterClass() { return _characterClass; }
    public Faction GetFaction() {  return _faction; }
    public Vector3 GetCharacterPosition() { return _position; }
    public Vector3 GetCharacterSize() { return _size; }
    public Quaternion GetRotation() { return _rotation; }
    public int GetHealthPoints() { return _currentHealthPoints; }
    public int GetInitiative() {  return _currentSpeed; }
    public int GetSpeed() { return _currentSpeed; }
    public int GetDamage() { return _currentDamage; }   
    public int GetMovementPoints() { return _currentMovementPoints; }
    public int GetBaseHealthPoints() { return _baseHealthPoints; }
    public int GetBaseInitiative() { return _baseSpeed; }
    public int GetBaseSpeed() { return _baseSpeed; }
    public int GetBaseDamage() { return _baseDamage; }
    public int GetBaseMovementPoints() { return _baseMovementPoints; }
    
    public Vector2Int GetCurrentTileIndex() { return _currentTileIndex; }

    
    public void SetPosition(Vector3 position) { _position = position; }
    public void SetCharacterClass(CharacterClass characterClass) { _characterClass = characterClass; }
    public void SetFaction(Faction faction) { _faction = faction; }
    public void SetHealthPoints(int healthPoints) { _currentHealthPoints = healthPoints; }
    public void SetInitiative(int initiative) {  _currentSpeed= initiative; }
    public void SetSpeed(int initiative) { _currentSpeed = initiative; }
    public void SetDamage(int damage) {  _currentDamage = damage; }
    public void SetMovementPoints(int movementPoints) {  _currentMovementPoints = movementPoints; }
    public void SetBaseHealthPoints(int healthPoints) { _baseHealthPoints = healthPoints; }
    public void SetBaseInitiative(int initiative) { _baseSpeed = initiative; }
    public void SetBaseSpeed(int initiative) { _baseSpeed = initiative; }
    public void SetBaseDamage(int damage) { _baseDamage = damage; }
    public void SetBaseMovementPoints(int movementPoints) { _baseMovementPoints = movementPoints; }
    public void SetCurrentTileIndex(Vector2Int tileIndex) { _currentTileIndex = tileIndex; }
}

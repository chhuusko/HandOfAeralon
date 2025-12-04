using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.EventSystems.EventTrigger;

public class EnemyAI : MonoBehaviour
{
    private struct AIAction
    {
        public CombatGridTile movement;
        public Ability ability;
        public CombatGridTile target;
    }

    public UnityEvent AIEndTurn;

    [SerializeField] private ClassData _barbData, _rogueData, _sorcData, _bardData;
    [SerializeField] private Faction _controlledFaction = Faction.Enemy;

    private Character _currentCharacter = null;
    private GameObject _currentTile = null;
    private CharacterClass _currentClass = CharacterClass.None;
    private AbilityHandler _currentAbilityHandler = null;
    private List<Ability> _currentAbilities = new();
    private int _currentMoveRange = 0;

    private Character _targetCharacter = null;
    private GameObject _targetTile = null;
    private List<CombatGridTile> _movePath = new();

    private Dictionary<AIAction, int> _scoredActions = new();
    private AIAction _chosenAction = new();

    private void OnEnable()
    {
        CombatEventManager.OnEnterCombatStateTakeTurn += OnTurnStart;
    }

    private void OnDisable()
    {
        CombatEventManager.OnEnterCombatStateTakeTurn -= OnTurnStart;
    }

    private void OnTurnStart(Character character)
    {
        if (TurnStartedProperly())
        {
            //Run();

            RunOld();
        }
    }

    private bool TurnStartedProperly() // Caching and null checks
    {
        _currentCharacter = CombatManager._instance.GetCombatTurnOrder().GetActiveCharacter();
        if (_currentCharacter == null || _currentCharacter.GetFaction() != _controlledFaction)
        {
            return false;
        }

        _currentTile = _currentCharacter.GetCurrentTileComponent().gameObject;
        if (_currentTile == null)
        {
            DebugLog.JLWLog($"EnemyAI.cs | _currentTile NOT FOUND!");
            return false;
        }

        _currentClass = _currentCharacter.GetCharacterClass();
        if (_currentClass == CharacterClass.None)
        {
            DebugLog.JLWLog($"EnemyAI.cs | _currentClass NOT FOUND!");
            return false;
        }

        _currentAbilityHandler = _currentCharacter.GetAbilityHandler();
        if (_currentAbilityHandler == null)
        {
            DebugLog.JLWLog($"EnemyAI.cs | _currentAbilityHandler NOT FOUND!");
            return false;
        }

        switch (_currentClass)
        {
            case CharacterClass.Barbarian: _currentAbilities = _barbData.abilities; break;
            case CharacterClass.Bard: _currentAbilities = _bardData.abilities; break;
            case CharacterClass.Rogue: _currentAbilities = _rogueData.abilities; break;
            case CharacterClass.Sorceress: _currentAbilities = _sorcData.abilities; break;
        }
        if (_currentAbilities == null || !_currentAbilities.Any())
        {
            DebugLog.JLWLog($"EnemyAI.cs | _currentAbilities NOT FOUND!");
            return false;
        }

        _currentMoveRange = _currentCharacter.GetMovementPoints();

        _targetCharacter = FindClosestOpponentCharacter(_currentCharacter);
        if (_targetCharacter == null)
        {
            DebugLog.JLWLog($"EnemyAI.cs | _targetCharacter NOT FOUND!");
            return false;
        }

        _targetTile = _targetCharacter.GetCurrentTileComponent().gameObject;
        if (_targetTile == null)
        {
            DebugLog.JLWLog($"EnemyAI.cs | _closestOpponentTile NOT FOUND!");
            return false;
        }

        return true;
    }

    private void Run()
    {
        List<CombatGridTile> moveRange = new();
        if (_currentCharacter.CanMove && _currentMoveRange > 0)
        {
            moveRange = GridExplorer._instance.GetTilesInRange(_currentTile, _currentMoveRange, true)
                .Select(obj => obj.GetComponent<CombatGridTile>())
                .Where(ch => ch != null)
                .ToList();
        }
        else
        {
            moveRange.Add(_currentTile.GetComponent<CombatGridTile>());
        }

        foreach (var pos in moveRange)
        {
            int score = 0;
            AIAction moveOnly = new AIAction { movement = pos };

            Character closestOpponent = FindClosestOpponentCharacter(_currentCharacter);
            int distance = GridExplorer._instance.ManhattanDistance(pos.GetTileIndex(), closestOpponent.GetCurrentTileIndex());
            switch (_currentClass)
            {
                case CharacterClass.Barbarian: score -= distance; break;
                case CharacterClass.Bard: score += distance; break;
                case CharacterClass.Rogue: score -= distance; break;
                case CharacterClass.Sorceress: score += distance; break;
            }

            _scoredActions[moveOnly] = score;

            foreach (var ability in _currentAbilities)
            {
                _currentAbilityHandler.SetPendingAbility(ability);
                _currentAbilityHandler.CalculateAbilityRange(pos);
                List<CombatGridTile> abilityRange = _currentAbilityHandler.GetTilesInRange();

                foreach (var tile in abilityRange)
                {
                    AIAction moveAndUseAbility = new AIAction { movement = pos, ability = ability, target = tile };
                    int newScore = score;

                    if (tile.GetOccupantCharacter() != null && tile.GetOccupantCharacter().GetFaction() != _controlledFaction)
                    {
                        newScore += 10;
                        _scoredActions[moveAndUseAbility] = newScore;
                    }

                    // Get ability area of effect
                    // Check if any damage or healing is done and add score
                }
            }
        }

        foreach (var entry in _scoredActions)
        {
            string ability = entry.Key.ability != null ? entry.Key.ability.name : "None";
            string target = entry.Key.target != null ? entry.Key.target.GetTileIndex().ToString() : "None";

            DebugLog.JLWLogWarning($"EnemyAI.cs | Move to: {entry.Key.movement.GetTileIndex()}, Ability: {ability}, Target: {target}, Score: {entry.Value}.");
        }

        string chosenAbility = _chosenAction.ability != null ? _chosenAction.ability.name : "None";
        string chosenTarget = _chosenAction.target != null ? _chosenAction.target.GetTileIndex().ToString() : "None";
        _chosenAction = _scoredActions.OrderByDescending(x => x.Value).First().Key;
        DebugLog.JLWLogWarning($"EnemyAI.cs | CHOSEN ACTION = Move to: {_chosenAction.movement.GetTileIndex()}, Ability: {chosenAbility}, Target: {chosenTarget}, Score: {_scoredActions[_chosenAction]}.");

        if (_currentCharacter.CanMove)
        {
            _movePath = FindPath(_currentTile, _chosenAction.movement.gameObject)
                .Select(obj => obj.GetComponent<CombatGridTile>())
                .Where(ch => ch != null)
                .ToList();

            _currentCharacter.GetComponent<CharacterMovement>().ForceCustomPath(_movePath);
            StartCoroutine(WaitForMovement());
        }

        // Use chosen ability
    }

    private void RunOld()
    {
        _movePath = FindPath(_currentTile, _targetTile)
                .Select(obj => obj.GetComponent<CombatGridTile>())
                .Where(ch => ch != null)
                .ToList();

        if (_currentCharacter.CanMove)
        {
            _currentCharacter.GetComponent<CharacterMovement>().ForceCustomPath(_movePath);
            StartCoroutine(WaitForMovement());
        }
        else
        {
            TryAttack(_currentCharacter, _targetCharacter);
            EndTurn();
        }
    }

    private Character FindClosestOpponentCharacter(Character currentCharacter)
    {
        Character result = null;
        List<Character> opponentCharacters = new();

        if (_controlledFaction == Faction.Enemy)
        {
            opponentCharacters = CombatGrid
                ._instance.GetAllFriendlyCharacters()
                .Select(obj => obj.GetComponent<Character>())
                .Where(ch => ch != null)
                .ToList();
        }
        else if (_controlledFaction == Faction.Friendly)
        {
            opponentCharacters = CombatGrid
                ._instance.GetAllEnemyCharacters()
                .Select(obj => obj.GetComponent<Character>())
                .Where(ch => ch != null)
                .ToList();
        }

        float min = float.MaxValue;
        foreach (var opponentCharacter in opponentCharacters)
        {
            float distance = Vector3.Distance(currentCharacter.transform.position, opponentCharacter.transform.position);
            if (distance < min)
            {
                min = distance;
                result = opponentCharacter;
            }
        }

        return result;
    }

    private List<GameObject> FindPath(GameObject currentTile, GameObject opponentTile)
    {
        List<GameObject> result = new();
        List<GameObject> path = GridExplorer._instance.FindPathAStar(currentTile, opponentTile);

        if (path == null || path.Count <= 1)
        {
            DebugLog.JLWLog($"EnemyAI.cs | No path found from {currentTile.GetComponent<CombatGridTile>().GetTileIndex()} to {opponentTile.GetComponent<CombatGridTile>().GetTileIndex()}");
            return new List<GameObject>();
        }

        for (int i = 1; i < path.Count && i <= _currentMoveRange; i++)
        {
            GameObject tile = path[i];
            int distToEnemy = GridExplorer._instance.ManhattanDistance(tile.GetComponent<CombatGridTile>().GetTileIndex(), opponentTile.GetComponent<CombatGridTile>().GetTileIndex());

            if (distToEnemy == 1)
            {
                result.Add(tile);
                return result;
            }

            if (distToEnemy > 0)
            {
                result.Add(tile);
            }
        }

        if (result == null || result.Count == 0)
        {
            DebugLog.JLWLog($"EnemyAI.cs | Path to target NOT FOUND!");
        }

        return result;
    }

    private IEnumerator WaitForMovement()
    {
        yield return new WaitWhile(() => _currentCharacter.IsMoving());

        TryAttack(_currentCharacter, _targetCharacter);

        EndTurn();
    }

    private void TryAttack(Character attacker, Character target)
    {
        if (!attacker.CanUseAbility)
        {
            return;
        }

        if (GridExplorer._instance.ChebyshevDistance(attacker.GetCurrentTileIndex(), target.GetCurrentTileIndex()) <= 1)
        {
            Vector3 direction = (target.transform.position - _currentCharacter.transform.position).normalized;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.0001f)
            {
                _currentCharacter.transform.rotation = Quaternion.LookRotation(direction);
            }

            target.TakeDamage(_currentCharacter.GetDamage());
        }
    }

    private void EndTurn()
    {
        //Debug.LogWarning("EnemyAI.cs | Turn ended!");

        _currentCharacter = null;
        _currentTile = null;
        _currentClass = CharacterClass.None;
        _currentAbilityHandler = null;
        _currentAbilities = new();
        _currentMoveRange = 0;
        _targetCharacter = null;
        _targetTile = null;
        _movePath = new();
        _scoredActions = new();
        _chosenAction = new();

        AIEndTurn.Invoke();
    }
}

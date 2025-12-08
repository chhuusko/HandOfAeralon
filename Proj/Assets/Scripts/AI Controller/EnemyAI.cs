using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

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
            Run();
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
        // Check move range
        List<CombatGridTile> moveRange = new();
        List<CombatGridTile> canReach = new();
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

        // Filter reachable tiles
        foreach (var tile in moveRange)
        {
            List<GameObject> pathSample = GridExplorer._instance.FindPathAStar(_currentTile.gameObject, tile.gameObject, false);

            if (pathSample != null && pathSample.Count > 0)
            {
                canReach.Add(tile);
            }
        }

        // Check all movement + ability combinations and score them
        foreach (var pos in canReach)
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

            if (_currentAbilities == null || !_currentAbilities.Any())
            {
                Debug.LogError($"{_currentCharacter.name} has no abilities!");
                continue;
            }

            foreach (var ability in _currentAbilities)
            {
                if (ability == null) continue;

                _currentAbilityHandler.SetPendingAbility(ability);
                _currentAbilityHandler.CalculateAbilityRange(pos);
                List<CombatGridTile> abilityRange = _currentAbilityHandler.GetTilesInRange();

                foreach (var tile in abilityRange)
                {
                    AIAction moveAndUseAbility = new AIAction { movement = pos, ability = ability, target = tile };
                    int newScore = score;

                    Character occupant = tile.GetOccupantCharacter();

                    if (occupant != null && occupant.GetFaction() != _controlledFaction)
                    {
                        newScore += 10;
                        _scoredActions[moveAndUseAbility] = newScore;
                    }

                    if (occupant != null && _currentCharacter.GetCharacterClass() == CharacterClass.Bard && occupant.GetFaction() == _controlledFaction && occupant != _currentCharacter)
                    {
                        newScore += 10;

                        if (ability.name == "SongOfRenewal_Ability" && occupant.GetCurrentHealth() != occupant.GetMaxHealth())
                        {
                            newScore += 999;
                        }

                        _scoredActions[moveAndUseAbility] = newScore;
                    }

                    // Get ability area of effect
                    // Check if any damage or healing is done and add score
                }
            }
        }

        /*
        foreach (var entry in _scoredActions)
        {
            string ability = entry.Key.ability != null ? entry.Key.ability.name : "None";
            string target = entry.Key.target != null ? entry.Key.target.GetTileIndex().ToString() : "None";

            DebugLog.JLWLogWarning($"EnemyAI.cs | Move to: {entry.Key.movement.GetTileIndex()}, Ability: {ability}, Target: {target}, Score: {entry.Value}.");
        }
        */

        _chosenAction = _scoredActions.OrderByDescending(x => x.Value).First().Key;

        string chosenAbility = _chosenAction.ability != null ? _chosenAction.ability.name : "None";
        string chosenTarget = _chosenAction.target != null ? _chosenAction.target.GetTileIndex().ToString() : "None";
        DebugLog.JLWLog($"AI | Move {_currentCharacter.name} to: {_chosenAction.movement.GetTileIndex()}, Ability: {chosenAbility}, Target: {chosenTarget}, ActionScore: {_scoredActions[_chosenAction]}.");

        if (_currentCharacter.CanMove)
        {
            _movePath = GridExplorer._instance.FindPathAStar(_currentTile, _chosenAction.movement.gameObject, false)
                .Select(obj => obj.GetComponent<CombatGridTile>())
                .Where(ch => ch != null)
                .ToList();

            _currentCharacter.GetComponent<CharacterMovement>().ForceCustomPath(_movePath);
        }

        StartCoroutine(WaitForMovement());
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

    private IEnumerator WaitForMovement()
    {
        CharacterMovement movementComponent = null;
        if (_currentCharacter.TryGetComponent<CharacterMovement>(out movementComponent))
        {
            yield return new WaitWhile(() => movementComponent.IsMoving());
            UseAbility(_chosenAction.ability, _chosenAction.target);
        }
        
        EndTurn();
    }

    private void UseAbility(Ability ability, CombatGridTile target)
    {
        if (!_currentCharacter.CanUseAbility || ability == null)
        {
            return;
        }

        _currentAbilityHandler.SetPendingAbility(ability);
        _currentAbilityHandler.CalculateAbilityRange();
        _currentAbilityHandler.UseAbility(ability, target);
    }

    private void EndTurn()
    {
        //DebugLog.JLWLog($"EnemyAI.cs | {_currentCharacter.name}'s turn ended!");

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

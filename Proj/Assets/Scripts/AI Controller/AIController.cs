using FMOD;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class AIController : MonoBehaviour
{
    private const int TOP_N_ACTIONS = 3;
    private const float TURN_START_WAIT_TIME = 1f;
    private const float TURN_END_WAIT_TIME = 1f;

    private class AIAction
    {
        public CombatGridTile movement;
        public Ability ability;
        public CombatGridTile target;
    }

    public UnityEvent AIEndTurn;

    [SerializeField] private Faction _controlledFaction = Faction.Enemy;
    private Character _character = null;
    private List<Character> _allies = new();
    private List<Character> _enemies = new();

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

    private bool TurnStartedProperly()
    {
        _character = CombatManager._instance.GetCombatTurnOrder().GetActiveCharacter();
        if (_character == null || _character.GetFaction() != _controlledFaction)
        {
            return false;
        }

        if (_controlledFaction == Faction.Enemy)
        {
            _allies = CombatGrid
                ._instance.GetAllEnemyCharacters()
                .Select(obj => obj.GetComponent<Character>())
                .Where(ch => ch != null)
                .ToList();

            _enemies = CombatGrid
                ._instance.GetAllFriendlyCharacters()
                .Select(obj => obj.GetComponent<Character>())
                .Where(ch => ch != null)
                .ToList();
        }
        else if (_controlledFaction == Faction.Friendly)
        {
            _allies = CombatGrid
                ._instance.GetAllFriendlyCharacters()
                .Select(obj => obj.GetComponent<Character>())
                .Where(ch => ch != null)
                .ToList();

            _enemies = CombatGrid
                ._instance.GetAllEnemyCharacters()
                .Select(obj => obj.GetComponent<Character>())
                .Where(ch => ch != null)
                .ToList();
        }

        return true;
    } // Caching and null checks

    private void Run()
    {
        StartCoroutine(AIBehaviour());
    }

    private IEnumerator AIBehaviour()
    {
        yield return new WaitForSeconds(TURN_START_WAIT_TIME);

        List<CombatGridTile> moveRange = FindMoveRange();
        Dictionary<AIAction, float> scoredActions = EvaluatePossibleActions(moveRange);
        if (scoredActions == null || !scoredActions.Any())
        {
            UnityEngine.Debug.LogError($"AIController.cs | AIBehaviour INTERRUPTED!");
            yield return new WaitForSeconds(TURN_END_WAIT_TIME);
            EndTurn();
            yield break;
        }
        AIAction chosenAction = SelectAction(scoredActions);
        //PrintAIAction(chosenAction);

        CharacterMovement movementComponent = null;
        if (!IsDead() && chosenAction.movement != _character.GetCurrentTileComponent() && _character.CanMove && 
            _character.GetMovementPoints() > 0 && _character.TryGetComponent<CharacterMovement>(out movementComponent))
        {
            List<CombatGridTile> movePath = 
                GridExplorer._instance.FindPathAStar(_character.GetCurrentTileComponent().gameObject, chosenAction.movement.gameObject, false, moveRange)
                .Select(obj => obj.GetComponent<CombatGridTile>()).Where(cgt => cgt != null).ToList();

            movementComponent.ForceCustomPath(movePath);
            yield return new WaitWhile(() => movementComponent.IsMoving());
        }

        if (!IsDead() && _character.CanUseAbility && chosenAction.ability != null && chosenAction.target != null)
        {
            //UnityEngine.Debug.LogError($"AIController.cs | {_character.name} tries to cast {chosenAction.ability.name}!");
            PerformAbilityCast(chosenAction);
        }

        yield return new WaitForSeconds(TURN_END_WAIT_TIME);
        EndTurn();
    }

    private List<CombatGridTile> FindMoveRange()
    {
        List<CombatGridTile> result = 
            GridExplorer._instance.GetReachableTilesWithMovement(_character.GetCurrentTileComponent().gameObject, _character.GetMovementPoints())
            .Select(obj => obj.GetComponent<CombatGridTile>()).Where(cgt => cgt != null).ToList();

        result.Add(_character.GetCurrentTileComponent());

        return result;
    }

    private Dictionary<AIAction, float> EvaluatePossibleActions(List<CombatGridTile> tiles)
    {
        Dictionary<AIAction, float> result = new();

        List<Ability> abilities = GetAbilities();
        AbilityHandler abilityHandler = _character.GetAbilityHandler();
        if (abilityHandler == null)
        {
            UnityEngine.Debug.LogError($"AIController.cs | AbilityHandler NOT FOUND!");
            return new Dictionary<AIAction, float>();
        }

        foreach (var tile in tiles) // Go through all possible movements and score them
        {
            AIAction move = new AIAction { movement = tile };
            float moveScore = 0f;

            if (_enemies.Any()) // Evaluate distance to enemies
            {
                Character closestEnemy = FindClosestCharacter(_enemies);
                int enemyDistance = 0;
                if (closestEnemy != null)
                {
                    enemyDistance = GridExplorer._instance.ManhattanDistance(tile.GetTileIndex(), closestEnemy.GetCurrentTileIndex());
                }

                switch (_character.GetCharacterClass())
                {
                    case CharacterClass.Barbarian:  moveScore -= enemyDistance; break;
                    case CharacterClass.Bard:       moveScore += enemyDistance; break;
                    case CharacterClass.Rogue:      moveScore -= enemyDistance; break;
                    case CharacterClass.Sorceress:  moveScore += enemyDistance; break;
                }

                if (_character.GetCurrentHealth() < _character.GetMaxHealth() / 5 && _character.GetCharacterClass() != CharacterClass.Barbarian && _allies.Count > 1)
                {
                    moveScore += enemyDistance * 2f;
                }
            }

            if (_allies.Count > 1) // Evaluate distance to allies
            {
                Character closestAlly = FindClosestCharacter(_allies);
                int allyDistance = 0;
                if (closestAlly != null)
                {
                    allyDistance = GridExplorer._instance.ManhattanDistance(tile.GetTileIndex(), closestAlly.GetCurrentTileIndex());
                }

                switch (_character.GetCharacterClass())
                {
                    case CharacterClass.Barbarian:  break;
                    case CharacterClass.Bard:       moveScore -= allyDistance; break;
                    case CharacterClass.Rogue:      break;
                    case CharacterClass.Sorceress:  moveScore -= allyDistance; break;
                }

                if (_character.GetCurrentHealth() < _character.GetMaxHealth() / 5 && _character.GetCharacterClass() != CharacterClass.Barbarian)
                {
                    moveScore -= allyDistance * 2f;
                }
            }

            if (tile != _character.GetCurrentTileComponent()) // Check for hazards
            {
                List<CombatGridTile> path = 
                    GridExplorer._instance.FindPathAStar(_character.GetCurrentTileComponent().gameObject, tile.gameObject, false, tiles)
                    .Select(obj => obj.GetComponent<CombatGridTile>()).Where(ch => ch != null).ToList();

                foreach (var step in path)
                {
                    if (step.GetTileType() == TileType.Lava || step.GetTileType() == TileType.Poison)
                    {
                        switch(_character.GetCharacterClass())
                        {
                            case CharacterClass.Barbarian:  moveScore -= 1f; break;
                            case CharacterClass.Bard:       moveScore -= 3f; break;
                            case CharacterClass.Rogue:      moveScore -= 5f; break;
                            case CharacterClass.Sorceress:  moveScore -= 3f; break;
                        }
                    }
                }
            }

            result[move] = moveScore; // Save movement as it's own possible action, before checking abilities

            foreach (var ability in abilities)
            {
                abilityHandler.SetPendingAbility(ability);
                abilityHandler.CalculateAbilityRange(tile);
                List<CombatGridTile> targets = abilityHandler.GetTilesInRange();

                foreach (var target in targets) // Go through all possible ability casts and score them
                {
                    AIAction act = new AIAction { movement = tile, ability = ability, target = target };
                    float actScore = moveScore;
                    actScore += ScoreAbilityUsage(ability, target);
                    result[act] = actScore;
                }
            }
        }

        return result;
    }

    private List<Ability> GetAbilities()
    {
        List<Ability> result = new();

        foreach (var ability in _character.GetAvailableAbilities())
        {
            result.Add(ability);
        }

        return result;
    }

    private Character FindClosestCharacter(List<Character> characters)
    {
        Character result = null;

        if (characters == null || !characters.Any())
        {
            return null;
        }

        float min = float.MaxValue;
        foreach (var character in characters)
        {
            float distance = Vector3.Distance(_character.transform.position, character.transform.position);
            if (distance < min && character != _character)
            {
                min = distance;
                result = character;
            }
        }

        return result;
    }

    private float ScoreAbilityUsage(Ability ability, CombatGridTile target)
    {
        float result = 0f;

        Character occupant = target.GetOccupantCharacter();
        if (occupant == null)
        {
            return result;
        }

        int occupantHP = occupant.GetCurrentHealth();
        int occupantMAXHP = occupant.GetMaxHealth();
        float occupantPERCENTHP = occupantHP / occupantMAXHP;

        bool bIsEnemy = occupant.GetFaction() != _controlledFaction;

        switch (ability.name)
        {
            // Barbarian
            case "Skullsplitter_Ability":
                {
                    if (bIsEnemy && occupantPERCENTHP < 0.5f)
                    {
                        result += 5;

                        if (occupantPERCENTHP < 0.2f)
                        {
                            result += 5;
                        }
                    }
                    break;
                }
            case "Earthquake_Ability":
                {
                    if (bIsEnemy)
                    {
                        result += 2;

                        if (occupantPERCENTHP < 0.2f)
                        {
                            result += 5;
                        }
                    }
                    break;
                }
            case "RuptureOfTheWilds_Ability":
                {
                    if (bIsEnemy)
                    {
                        result += 2;

                        if (occupantPERCENTHP < 0.2f)
                        {
                            result += 5;
                        }
                    }
                    break;
                }
            case "RoarOfTheAncients_Ability":
                {
                    if (bIsEnemy)
                    {
                        result += 2;
                    }
                    break;
                }

            // Bard
            case "InspiringAnthem_Ability":
                {
                    if (!bIsEnemy && occupantPERCENTHP >= 0.75f)
                    {
                        result += 5;
                    }
                    break;
                }
            case "SongOfRenewal_Ability":
                {
                    if (!bIsEnemy && occupantPERCENTHP < 0.75f)
                    {
                        result += 10;
                    }
                    break;
                }
            case "DissonantChord_Ability":
                {
                    if (bIsEnemy)
                    {
                        result += 2;
                    }
                    break;
                }
            case "LuteSmash_Ability":
                {
                    if (bIsEnemy)
                    {
                        if (occupantPERCENTHP < 0.1f)
                        {
                            result += 10f;
                        }
                        
                        if (_allies.Count == 1)
                        {
                            result += 10f;
                        }
                    }
                    break;
                }

            // Rogue
            case "SandfangStrike_Ability":
                {
                    if (bIsEnemy)
                    {
                        result += 5 / occupantPERCENTHP;

                        if (occupant.GetCharacterClass() == CharacterClass.Sorceress || occupant.GetCharacterClass() == CharacterClass.Bard)
                        {
                            result += 5;
                        }
                    }
                    break;
                }
            case "Desert's Grasp_Ability":
                {
                    if (bIsEnemy)
                    {
                        result += 5 / occupantPERCENTHP;

                        if (occupant.GetCharacterClass() == CharacterClass.Sorceress || occupant.GetCharacterClass() == CharacterClass.Bard)
                        {
                            result += 5;
                        }
                    }
                    break;
                }
            case "ThrowingKnives_Ability":
                {
                    if (bIsEnemy)
                    {
                        result += 5 / occupantPERCENTHP;

                        if (occupant.GetCharacterClass() == CharacterClass.Sorceress || occupant.GetCharacterClass() == CharacterClass.Bard)
                        {
                            result += 5;
                        }
                    }
                    break;
                }
            case "VeilOfDust_Ability":
                {
                    result += 2;
                    break;
                }

            // Sorceress
            case "ArcaneBolt_Ability":
                {
                    if (bIsEnemy)
                    {
                        result += 5;

                        if (occupantPERCENTHP < 0.2f)
                        {
                            result += 5;
                        }
                    }
                    break;
                }
            case "FlameSurge_Ability":
                {
                    if (bIsEnemy)
                    {
                        result += 5;

                        if (occupantPERCENTHP < 0.2f)
                        {
                            result += 5;
                        }
                    }
                    break;
                }
            case "LightningStorm_Ability":
                {
                    if (bIsEnemy)
                    {
                        result += 5;

                        if (occupantPERCENTHP < 0.2f)
                        {
                            result += 5;
                        }
                    }
                    break;
                }
            case "Emberwake_Ability":
                {
                    result += 2;
                    break;
                }
        }

        return result;
    }

    private AIAction SelectAction(Dictionary<AIAction, float> dictionary)
    {
        AIAction result = null;

        var topActions = dictionary
            .OrderByDescending(key => key.Value)
            .Take(TOP_N_ACTIONS)
            .ToList();

        if (topActions.Count == 0)
        {
            UnityEngine.Debug.LogError($"AIController.cs | SelectAction NO ACTIONS FOUND!");
            result = new AIAction { movement = _character.GetCurrentTileComponent() };
        }
        else
        {
            result = topActions[Random.Range(0, topActions.Count)].Key;
        }

        return result;
    }

    private void PerformAbilityCast(AIAction action)
    {
        AbilityHandler abilityHandler = _character.GetAbilityHandler();
        if (abilityHandler != null)
        {
            abilityHandler.SetPendingAbility(action.ability);
            abilityHandler.CalculateAbilityRange();
            abilityHandler.UseAbility(action.ability, action.target);
        }
    }

    private void EndTurn()
    {
        //UnityEngine.Debug.LogError($"AIController.cs | {_character.name} turn ended!");
        _character = null;
        _allies = new();
        _enemies = new();
        AIEndTurn.Invoke();
    }

    private bool IsDead()
    {
        return _character == null || _character.GetCurrentHealth() <= 0;
    }

    private void PrintAIAction(AIAction action)
    {
        UnityEngine.Debug.LogError($"AIController.cs | Move {_character.name} to {action.movement.GetTileIndex()}," +
            $" use ability: {(action.ability != null ? action.ability.name : "None")}" +
            $" at position {(action.target != null ? action.target.GetTileIndex() : "None")}");
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class EnemyAI : MonoBehaviour
{
    private const int TOP_N_ACTIONS = 3;
    private const float TURN_START_WAIT_TIME = 1f;
    private const float TURN_END_WAIT_TIME = 2.5f;

    private class AIAction
    {
        public CombatGridTile movement;
        public Ability ability;
        public CombatGridTile target;
    }

    public UnityEvent AIEndTurn;

    [SerializeField] private Faction _controlledFaction = Faction.Enemy;
    [SerializeField] private DirectedAOEPattern _linePattern, _flamePattern, _housePattern, _trisquarePattern;
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
        //Debug.LogError($"EnemyAI.cs | moveRange: {moveRange.Count}");
        Dictionary<AIAction, float> scoredActions = EvaluatePossibleActions(moveRange);
        //Debug.LogWarning($"EnemyAI.cs | {_character.name} evaluated {scoredActions.Count} actions.");
        if (scoredActions == null || !scoredActions.Any())
        {
            Debug.LogError($"EnemyAI.cs | AIBehaviour INTERRUPTED!");
            yield return new WaitForSeconds(TURN_END_WAIT_TIME);
            EndTurn();
            yield break;
        }
        AIAction chosenAction = SelectAction(scoredActions);
        /*
        Debug.LogWarning($"EnemyAI.cs | Move {_character.name} to {chosenAction.movement.GetTileIndex()}," +
            $" use ability: {(chosenAction.ability != null ? chosenAction.ability.name : "NONE")}," +
            $" at position: {(chosenAction.target != null ? chosenAction.target.GetTileIndex() : "NONE")}," +
            $" score: {scoredActions[chosenAction]}!");
        */

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
            //Debug.LogError($"EnemyAI.cs | {_character.name} tries to cast {chosenAction.ability.name}!");
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
        for (int i = 0; i < abilities.Count; i++)
        {
            if (_character.IsAbilityCooldownActive(abilities[i]))
            {
                abilities.RemoveAt(i);
            }
        }
        //Debug.LogError($"EnemyAI.cs | Found {abilities.Count} abilities ready to use!");

        AbilityHandler abilityHandler = _character.GetAbilityHandler();
        if (abilityHandler == null)
        {
            Debug.LogError($"EnemyAI.cs | AbilityHandler NOT FOUND!");
            return new Dictionary<AIAction, float>();
        }

        foreach (var tile in tiles) // Go through all possible movements and score them
        {
            AIAction move = new AIAction { movement = tile };
            float moveScore = 0f;
            int stepsUsed = GridExplorer._instance.ManhattanDistance(_character.GetCurrentTileIndex(), tile.GetTileIndex());
            moveScore += Mathf.Min(stepsUsed, 3) * 5;

            if (_enemies.Any()) // Evaluate distance to enemies
            {
                Character closestEnemy = FindClosestCharacter(_enemies);
                int enemyDistance = 0;
                int movedEnemyDistance = 0;
                if (closestEnemy != null)
                {
                    int currentEnemyDistance = GridExplorer._instance.ManhattanDistance(_character.GetCurrentTileIndex(), closestEnemy.GetCurrentTileIndex());
                    movedEnemyDistance = GridExplorer._instance.ManhattanDistance(tile.GetTileIndex(), closestEnemy.GetCurrentTileIndex());
                    enemyDistance = movedEnemyDistance - currentEnemyDistance;
                }

                switch (_character.GetCharacterClass())
                {
                    case CharacterClass.Barbarian:  moveScore -= enemyDistance * 15; break;
                    case CharacterClass.Bard:       moveScore += enemyDistance * 5; break;
                    case CharacterClass.Rogue:      moveScore -= enemyDistance * 10; break;
                    case CharacterClass.Sorceress:  moveScore += enemyDistance * 5; break;
                }

                if (_character.GetCurrentHealth() < _character.GetMaxHealth() / 5 && _character.GetCharacterClass() != CharacterClass.Barbarian && _allies.Count > 1)
                {
                    moveScore += enemyDistance * 2;
                }
            }

            if (_allies.Count > 1) // Evaluate distance to allies
            {
                Character closestAlly = FindClosestCharacter(_allies);
                int allyDistance = 0;
                if (closestAlly != null)
                {
                    int currentAllyDistance = GridExplorer._instance.ManhattanDistance(_character.GetCurrentTileIndex(), closestAlly.GetCurrentTileIndex());
                    int movedAllyDistance = GridExplorer._instance.ManhattanDistance(tile.GetTileIndex(), closestAlly.GetCurrentTileIndex());
                    allyDistance = movedAllyDistance - currentAllyDistance;
                }

                switch (_character.GetCharacterClass())
                {
                    case CharacterClass.Barbarian:  break;
                    case CharacterClass.Bard:       moveScore -= allyDistance * 10; break;
                    case CharacterClass.Rogue:      break;
                    case CharacterClass.Sorceress:  moveScore -= allyDistance * 10; break;
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
                        switch (_character.GetCharacterClass())
                        {
                            case CharacterClass.Barbarian:  moveScore -= 50f; break;
                            case CharacterClass.Bard:       moveScore -= 50f; break;
                            case CharacterClass.Rogue:      moveScore -= 75f; break;
                            case CharacterClass.Sorceress:  moveScore -= 50f; break;
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
                    if (!abilityHandler.IsValidTargetTileForAbility(ability, target)) continue;
                    AIAction act = new AIAction { movement = tile, ability = ability, target = target };
                    float actScore = moveScore;
                    actScore += ScoreAbilityUsage(tile, ability, target);
                    result[act] = actScore;
                }
            }

            bool canHitInstead = result.Any(kvp =>
                kvp.Key.movement == tile &&
                kvp.Key.ability != null &&
                kvp.Value > moveScore + 10f);

            if (canHitInstead)
            {
                result[move] -= 50f;
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

    private float ScoreAbilityUsage(CombatGridTile tile, Ability ability, CombatGridTile target)
    {
        float result = 0f;

        StatusEffectManager mySEM = _character.GetStatusEffectManager();
        if (mySEM == null)
        {
            Debug.LogError($"EnemyAI.cs | {_character.name} StatusEffectManager NOT FOUND!");
            return result;
        }

        switch (ability.name)
        {
            // Barbarian
            case "Skullsplitter_Ability":
                {
                    Character occupant = target.GetOccupantCharacter();
                    if (occupant != null && occupant.GetCurrentHealth() > 0)
                    {
                        StatusEffectManager occupantSEM = occupant.GetStatusEffectManager();
                        bool isEnemy = occupant.GetFaction() != _controlledFaction;
                        float occupantPERCENTHP = occupant.GetCurrentHealth() / occupant.GetMaxHealth();

                        if (isEnemy)
                        {
                            result += 30f;

                            if (occupantPERCENTHP < 0.5f)
                            {
                                result += 100f;
                            }
                            
                            if (occupantSEM != null && occupantSEM.ContainsStatusEffect<Stealth>())
                            {
                                result = 0f;
                            }
                        }
                    }
                    break;
                }
            case "Earthquake_Ability":
                {
                    int hitCount = 0;
                    DirectedAOEPattern.Direction direction = GetDirection(tile, target);
                    _housePattern.SetDirection(direction);
                    List<CombatGridTile> aoe = _housePattern.CalculateTilesToEffect(target);
                    foreach (var hit in aoe)
                    {
                        Character occupant = hit.GetOccupantCharacter();
                        if (occupant != null && occupant.GetCurrentHealth() > 0)
                        {
                            StatusEffectManager occupantSEM = occupant.GetStatusEffectManager();
                            bool isEnemy = occupant.GetFaction() != _controlledFaction;
                            float occupantPERCENTHP = occupant.GetCurrentHealth() / occupant.GetMaxHealth();

                            if (!isEnemy) result -= 75f;

                            if (isEnemy)
                            {
                                hitCount++;
                                result += 30f;

                                if (occupantSEM != null && !occupantSEM.ContainsStatusEffect<Slowed>())
                                {
                                    result += 10f;
                                }

                                if (occupantPERCENTHP < 0.2f)
                                {
                                    result += 100f;
                                }
                            }
                        }
                    }
                    if (hitCount == 0) result -= 100;
                    break;
                }
            case "RuptureOfTheWilds_Ability":
                {
                    int hitCount = 0;
                    DirectedAOEPattern.Direction direction = GetDirection(tile, target);
                    _trisquarePattern.SetDirection(direction);
                    List<CombatGridTile> aoe = _trisquarePattern.CalculateTilesToEffect(target);
                    foreach (var hit in aoe)
                    {
                        Character occupant = hit.GetOccupantCharacter();
                        if (occupant != null && occupant.GetCurrentHealth() > 0)
                        {
                            StatusEffectManager occupantSEM = occupant.GetStatusEffectManager();
                            bool isEnemy = occupant.GetFaction() != _controlledFaction;
                            float occupantPERCENTHP = occupant.GetCurrentHealth() / occupant.GetMaxHealth();

                            if (!isEnemy) result -= 75f;

                            if (isEnemy)
                            {
                                hitCount++;
                                result += 30f;

                                if (occupantSEM != null && occupantSEM.ContainsStatusEffect<Slowed>())
                                {
                                    result += 20f;
                                }

                                if (occupantPERCENTHP < 0.2f)
                                {
                                    result += 100f;
                                }
                            }
                        }
                    }
                    if (hitCount == 0) result -= 100;
                    break;
                }
            case "RoarOfTheAncients_Ability":
                {
                    int hitCount = 0;
                    List<CombatGridTile> aoe = DiamondPattern(target, 2);
                    foreach (var hit in aoe)
                    {
                        Character occupant = hit.GetOccupantCharacter();
                        if (occupant != null && occupant.GetCurrentHealth() > 0)
                        {
                            StatusEffectManager occupantSEM = occupant.GetStatusEffectManager();
                            bool isEnemy = occupant.GetFaction() != _controlledFaction;
                            float occupantPERCENTHP = occupant.GetCurrentHealth() / occupant.GetMaxHealth();

                            if (isEnemy)
                            {
                                hitCount++;

                                if (occupantSEM != null && !occupantSEM.ContainsStatusEffect<Slowed>())
                                {
                                    result += 40f;
                                }
                            }
                        }
                    }
                    if (hitCount == 0) result -= 100;
                    break;
                }

            // Bard
            case "InspiringAnthem_Ability":
                {
                    int hitCount = 0;
                    List<CombatGridTile> aoe = DiamondPattern(target, 2);
                    foreach (var hit in aoe)
                    {
                        Character occupant = hit.GetOccupantCharacter();
                        if (occupant != null && occupant.GetCurrentHealth() > 0)
                        {
                            StatusEffectManager occupantSEM = occupant.GetStatusEffectManager();
                            bool isAlly = occupant.GetFaction() == _controlledFaction;

                            if (isAlly)
                            {
                                hitCount++;

                                if (occupantSEM != null && !occupantSEM.ContainsStatusEffect<Haste>())
                                {
                                    result += 50f;
                                }
                            }
                        }
                    }
                    if (hitCount == 0) result -= 100;
                    break;
                }
            case "SongOfRenewal_Ability":
                {
                    int hitCount = 0;
                    List<CombatGridTile> aoe = DiamondPattern(target, 3);
                    foreach (var hit in aoe)
                    {
                        Character occupant = hit.GetOccupantCharacter();
                        if (occupant != null && occupant.GetCurrentHealth() > 0)
                        {
                            bool isAlly = occupant.GetFaction() == _controlledFaction;
                            float occupantPERCENTHP = occupant.GetCurrentHealth() / occupant.GetMaxHealth();

                            if (isAlly && occupantPERCENTHP < 1f)
                            {
                                hitCount++;
                                result += 40;

                                if (occupantPERCENTHP < 0.75f)
                                {
                                    result += 10;

                                    if (occupantPERCENTHP < 0.5f)
                                    {
                                        result += 10;
                                    }
                                }
                            }
                        }
                    }
                    if (hitCount == 0) result -= 100;
                    break;
                }
            case "DissonantChord_Ability":
                {
                    int hitCount = 0;
                    List<CombatGridTile> aoe = DiamondPattern(target, 2);
                    foreach (var hit in aoe)
                    {
                        Character occupant = hit.GetOccupantCharacter();
                        if (occupant != null && occupant.GetCurrentHealth() > 0)
                        {
                            StatusEffectManager occupantSEM = occupant.GetStatusEffectManager();
                            bool isEnemy = occupant.GetFaction() == _controlledFaction;

                            if (isEnemy)
                            {
                                hitCount++;
                                if (occupantSEM.ContainsStatusEffect<Haste>()) result += 25f;
                                if (occupantSEM.ContainsStatusEffect<Empowered>()) result += 25f;
                                if (occupantSEM.ContainsStatusEffect<Emberwake>()) result += 25f;
                                if (occupantSEM.ContainsStatusEffect<Enraged>()) result += 25f;
                                if (occupantSEM.ContainsStatusEffect<ConduitOfPower>()) result += 25f;
                                if (occupantSEM.ContainsStatusEffect<Fortified>()) result += 25f;
                                if (occupantSEM.ContainsStatusEffect<Sanctified>()) result += 25f;
                                if (occupantSEM.ContainsStatusEffect<Stealth>()) result += 25f;
                            }
                        }
                    }
                    if (hitCount == 0) result -= 100;
                    break;
                }
            case "LuteSmash_Ability":
                {
                    Character occupant = target.GetOccupantCharacter();
                    if (occupant != null && occupant.GetCurrentHealth() > 0)
                    {
                        StatusEffectManager occupantSEM = occupant.GetStatusEffectManager();
                        bool isEnemy = occupant.GetFaction() != _controlledFaction;
                        float occupantPERCENTHP = occupant.GetCurrentHealth() / occupant.GetMaxHealth();

                        if (isEnemy)
                        {
                            result += 10f;

                            if (occupantPERCENTHP < 0.2f)
                            {
                                result += 20f;
                            }

                            if (_allies.Count == 1)
                            {
                                result += 100f;
                            }

                            if (occupantSEM != null && occupantSEM.ContainsStatusEffect<Stealth>())
                            {
                                result = 0f;
                            }
                        }
                    }
                    break;
                }

            // Rogue
            case "SandfangStrike_Ability":
                {
                    Character occupant = target.GetOccupantCharacter();
                    if (occupant != null && occupant.GetCurrentHealth() > 0)
                    {
                        StatusEffectManager occupantSEM = occupant.GetStatusEffectManager();
                        bool isEnemy = occupant.GetFaction() != _controlledFaction;
                        float occupantPERCENTHP = occupant.GetCurrentHealth() / occupant.GetMaxHealth();

                        if (isEnemy)
                        {
                            result += 40f;

                            if (occupantSEM != null && occupantSEM.ContainsStatusEffect<Poison>())
                            {
                                result += 10f;
                            }

                            if (occupantPERCENTHP < 0.5f)
                            {
                                result += 10f;

                                if (occupantPERCENTHP < 0.2f)
                                {
                                    result += 100f;
                                }
                            }

                            if (occupant.GetCharacterClass() == CharacterClass.Bard || occupant.GetCharacterClass() == CharacterClass.Sorceress)
                            {
                                result += 20;
                            }

                            if (occupantSEM != null && occupantSEM.ContainsStatusEffect<Stealth>())
                            {
                                result = 0f;
                            }
                        }
                    }
                    break;
                }
            case "Desert's Grasp_Ability":
                {
                    result += 50f;
                    int hitCount = 0;
                    Vector2Int[] directions = new Vector2Int[]
                        {
                        new Vector2Int(1, 1),
                        new Vector2Int(-1, 1),
                        new Vector2Int(1, -1),
                        new Vector2Int(-1, -1),

                        new Vector2Int(1, 0),
                        new Vector2Int(0, 1),
                        new Vector2Int(-1, 0),
                        new Vector2Int(0, -1)
                        };

                    foreach (var dir in directions)
                    {
                        GameObject obj = CombatGrid._instance.GetTileAtCoord(target.GetTileIndex().x + dir.x, target.GetTileIndex().y + dir.y);
                        CombatGridTile hit = null;
                        Character occupant = null;
                        if (obj != null && obj.TryGetComponent<CombatGridTile>(out hit))
                        {
                            occupant = hit.GetOccupantCharacter();
                        }
                        if (occupant != null && occupant.GetCurrentHealth() > 0)
                        {
                            StatusEffectManager occupantSEM = occupant.GetStatusEffectManager();
                            bool isEnemy = occupant.GetFaction() != _controlledFaction;
                            float occupantPERCENTHP = occupant.GetCurrentHealth() / occupant.GetMaxHealth();

                            if (!isEnemy) result -= 75f;

                            if (isEnemy)
                            {
                                hitCount++;
                                result += 30f;

                                if (occupantSEM != null && occupantSEM.ContainsStatusEffect<Poison>())
                                {
                                    result += 10f;
                                }

                                if (occupantSEM != null && occupantSEM.ContainsStatusEffect<Stealth>())
                                {
                                    result += 20f;
                                }

                                if (occupantPERCENTHP < 0.5f)
                                {
                                    result += 10f;

                                    if (occupantPERCENTHP < 0.2f)
                                    {
                                        result += 100f;
                                    }
                                }

                                if (occupant.GetCharacterClass() == CharacterClass.Bard || occupant.GetCharacterClass() == CharacterClass.Sorceress)
                                {
                                    result += 20;
                                }
                            }
                        }
                    }
                    if (hitCount >= 2)
                    {
                        result += 125f;
                    }

                    if (hitCount >= 3)
                    {
                        result += 300f;
                    }
                    if (hitCount < 2) result -= 200;
                    break;
                }
            case "ThrowingKnives_Ability":
                {
                    int hitCount = 0;
                    DirectedAOEPattern.Direction direction = GetDirection(tile, target);
                    _linePattern.SetDirection(direction);
                    List<CombatGridTile> aoe = _linePattern.CalculateTilesToEffect(target);
                    foreach (var hit in aoe)
                    {
                        Character occupant = hit.GetOccupantCharacter();
                        if (occupant != null && occupant.GetCurrentHealth() > 0)
                        {
                            StatusEffectManager occupantSEM = occupant.GetStatusEffectManager();
                            bool isEnemy = occupant.GetFaction() != _controlledFaction;
                            float occupantPERCENTHP = occupant.GetCurrentHealth() / occupant.GetMaxHealth();

                            if (!isEnemy) result -= 100f;

                            if (isEnemy)
                            {
                                hitCount++;
                                result += 30f;

                                if (occupantSEM != null && occupantSEM.ContainsStatusEffect<Poison>())
                                {
                                    result += 10f;
                                }

                                if (occupantPERCENTHP < 0.5f)
                                {
                                    result += 10f;

                                    if (occupantPERCENTHP < 0.2f)
                                    {
                                        result += 100f;
                                    }
                                }

                                if (occupant.GetCharacterClass() == CharacterClass.Bard || occupant.GetCharacterClass() == CharacterClass.Sorceress)
                                {
                                    result += 20;
                                }
                            }
                        }
                    }
                    if (hitCount == 0) result -= 100;
                    break;
                }
            case "VeilOfDust_Ability":
                {
                    if (!mySEM.ContainsStatusEffect<Stealth>())
                    {
                        if (mySEM.ContainsStatusEffect<Poison>()) result += 10f;
                        if (mySEM.ContainsStatusEffect<Burn>()) result += 10f;
                        if (mySEM.ContainsStatusEffect<Aftershock>()) result += 10f;
                        if (mySEM.ContainsStatusEffect<Vulnerable>()) result += 10f;
                        if (mySEM.ContainsStatusEffect<Weakened>()) result += 10f;
                        if (mySEM.ContainsStatusEffect<Slowed>()) result += 10f;
                    }
                    else
                    {
                        result -= 5;
                    }
                    break;
                }

            // Sorceress
            case "ArcaneBolt_Ability":
                {
                    Character occupant = target.GetOccupantCharacter();
                    if (occupant != null && occupant.GetCurrentHealth() > 0)
                    {
                        StatusEffectManager occupantSEM = occupant.GetStatusEffectManager();
                        bool isEnemy = occupant.GetFaction() != _controlledFaction;
                        float occupantPERCENTHP = occupant.GetCurrentHealth() / occupant.GetMaxHealth();

                        if (isEnemy)
                        {
                            result += 40f;

                            if (occupantPERCENTHP < 0.2f)
                            {
                                result += 100f;
                            }

                            if (occupantSEM != null && occupantSEM.ContainsStatusEffect<Stealth>())
                            {
                                result = 0f;
                            }
                        }
                    }
                    break;
                }
            case "FlameSurge_Ability":
                {
                    int hitCount = 0;
                    DirectedAOEPattern.Direction direction = GetDirection(tile, target);
                    _flamePattern.SetDirection(direction);
                    List<CombatGridTile> aoe = _flamePattern.CalculateTilesToEffect(target);
                    foreach (var hit in aoe)
                    {
                        Character occupant = hit.GetOccupantCharacter();
                        if (occupant != null && occupant.GetCurrentHealth() > 0)
                        {
                            StatusEffectManager occupantSEM = occupant.GetStatusEffectManager();
                            bool isEnemy = occupant.GetFaction() != _controlledFaction;
                            float occupantPERCENTHP = occupant.GetCurrentHealth() / occupant.GetMaxHealth();

                            if (!isEnemy)
                            {
                                result -= 75f;

                                if (mySEM.ContainsStatusEffect<Emberwake>())
                                {
                                    result -= 10f;
                                }
                            }

                            if (isEnemy)
                            {
                                hitCount++;
                                result += 30f;

                                if (mySEM.ContainsStatusEffect<Emberwake>())
                                {
                                    result += 10f;
                                }

                                if (occupantPERCENTHP < 0.2f)
                                {
                                    result += 100f;
                                }

                                if (occupantSEM != null && occupantSEM.ContainsStatusEffect<Stealth>())
                                {
                                    result += Random.Range(0f, 20f);
                                }
                            }
                        }
                    }
                    if (hitCount == 0) result -= 100;
                    break;
                }
            case "LightningStorm_Ability":
                {
                    int hitCount = 0;
                    List<CombatGridTile> aoe = DiamondPattern(target, 3);
                    foreach (var hit in aoe)
                    {
                        Character occupant = hit.GetOccupantCharacter();
                        if (occupant != null && occupant.GetCurrentHealth() > 0)
                        {
                            StatusEffectManager occupantSEM = occupant.GetStatusEffectManager();
                            bool isEnemy = occupant.GetFaction() != _controlledFaction;
                            float occupantPERCENTHP = occupant.GetCurrentHealth() / occupant.GetMaxHealth();

                            if (!isEnemy)
                            {
                                result -= 75f;

                                if (mySEM.ContainsStatusEffect<Emberwake>())
                                {
                                    result -= 10f;
                                }
                            }

                            if (isEnemy)
                            {
                                hitCount++;
                                result += 30f;

                                if (mySEM.ContainsStatusEffect<Emberwake>())
                                {
                                    result += 10f;
                                }

                                if (occupantPERCENTHP < 0.2f)
                                {
                                    result += 100f;
                                }

                                if (occupantSEM != null && occupantSEM.ContainsStatusEffect<Stealth>())
                                {
                                    result += Random.Range(0f, 20f);
                                }
                            }
                        }
                    }
                    if (hitCount == 0) result -= 100;
                    break;
                }
            case "Emberwake_Ability":
                {
                    if (!mySEM.ContainsStatusEffect<Emberwake>())
                    {
                        result += 30f;
                    }
                    else
                    {
                        result -= 5;
                    }
                    break;
                }
        }

        return result;
    }

    private DirectedAOEPattern.Direction GetDirection(CombatGridTile from, CombatGridTile to)
    {
        Vector2Int delta = to.GetTileIndex() - from.GetTileIndex();
        if (delta == Vector2Int.up) return DirectedAOEPattern.Direction.Up;
        if (delta == Vector2Int.down) return DirectedAOEPattern.Direction.Down;
        if (delta == Vector2Int.left) return DirectedAOEPattern.Direction.Left;
        if (delta == Vector2Int.right) return DirectedAOEPattern.Direction.Right;
        return DirectedAOEPattern.Direction.None;
    }

    private List<CombatGridTile> DiamondPattern(CombatGridTile targetTile, int radius)
    {
        List<CombatGridTile> result = new();
        Vector2 centerIndex = targetTile.GetTileIndex();

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                if (Mathf.Abs(x) + Mathf.Abs(y) <= radius)
                {
                    int checkX = (int)centerIndex.x + x;
                    int checkY = (int)centerIndex.y + y;

                    if (checkX < 0 || checkX >= CombatGrid._instance.GetGridWidth()) continue;
                    if (checkY < 0 || checkY >= CombatGrid._instance.GetGridHeight()) continue;

                    var tile = CombatManager._instance.GetTileComponent(checkX, checkY);
                    if (tile != null)
                        result.Add(tile);
                }
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
            Debug.LogError($"AIController.cs | SelectAction NO ACTIONS FOUND!");
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
        //Debug.LogError($"AIController.cs | {_character.name} turn ended!");
        _character = null;
        _allies = new();
        _enemies = new();
        AIEndTurn.Invoke();
    }

    private bool IsDead()
    {
        return _character == null || _character.GetCurrentHealth() <= 0;
    }
}

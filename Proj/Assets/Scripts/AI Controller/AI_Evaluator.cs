// Joel Larsson Wendt | jola6902

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AI_Evaluator
{
    private static readonly HashSet<CharacterClass> _meleeClassSet = new()
    {
        CharacterClass.Barbarian,
        CharacterClass.Rogue
    };

    private const int TOP_N_ACTIONS = 3;
    private const float HAZARD_AVOIDANCE = 25f;
    private const int RANGED_CHARACTER_PREFERRED_RANGE = 6;
    private const float USE_ABILITY_BONUS = 30f;
    private const float DISPEL_BUFFS_INCLINATION = 50f;
    private const float PREFER_POISONED_TARGETS = 30f;
    private const float PREFER_UNARMORED_TARGETS = 30f;
    private const float PROACTIVE_STEALTH_CHANCE = 0.5f;
    private const float FRIENDLY_FIRE_AVOIDANCE = 60f;

    private LinePattern _linePattern = ScriptableObject.Instantiate(Resources.Load<LinePattern>("AI/LinePattern"));
    private FlamePattern _flamePattern = ScriptableObject.Instantiate(Resources.Load<FlamePattern>("AI/FlamePattern"));
    private HousePattern _housePattern = ScriptableObject.Instantiate(Resources.Load<HousePattern>("AI/HousePattern"));
    private TriSquarePattern _trisquarePattern = ScriptableObject.Instantiate(Resources.Load<TriSquarePattern>("AI/TriSquarePattern"));

    public AI_Action Evaluate(AI_Context context, List<AI_Action> actions)
    {
        if (!actions.Any())
        {
            Debug.LogError($"AI_Evaluator.cs | NO ACTIONS FOUND for {context.Self.name}! Defaulting to standing still.");
            return new AI_Action { Movement = context.Self.GetCurrentTileComponent() };
        }

        foreach (var action in actions)
        {
            float score = 0f;
            score += EvaluateMovement(context, action);
            score += EvaluateAbilityUsage(context, action);
            action.Score = score;
        }

        PenalizeMovementOnlyActions(actions);

        return SelectAction(context, actions);
    }

    private float EvaluateMovement(AI_Context context, AI_Action action)
    {
        float result = 0f;

        if (HAZARD_AVOIDANCE > 0f && context.Self.GetCurrentTileComponent() != action.Movement) // Check for hazards
        {
            List<CombatGridTile> path =
            GridExplorer._instance.FindPathAStar(context.Self.GetCurrentTileComponent().gameObject, action.Movement.gameObject, false, context.ReachableTiles)
            .Select(obj => obj.GetComponent<CombatGridTile>())
            .Where(ch => ch != null)
            .ToList();

            foreach (var step in path)
            {
                if (step.GetTileType() == TileType.Lava || step.GetTileType() == TileType.Poison)
                {
                    result -= HAZARD_AVOIDANCE / context.PercentHP;
                }
            }
        }

        if (!_meleeClassSet.Contains(context.Self.GetCharacterClass()) && context.Allies.Count > 1) // Ranged classes keep close to allies
        {
            Character closestAlly = FindClosestCharacter(context, context.Allies);
            int deltaDistance = 0;
            if (closestAlly != null)
            {
                deltaDistance = GetDeltaDistance(context.Self, closestAlly, action.Movement);
                result -= deltaDistance * 5f / context.PercentHP;
            }
        }

        if (context.Enemies.Any()) // Encourage advancing toward the enemy
        {
            Character closestEnemy = FindClosestCharacter(context, context.Enemies);
            int deltaDistance = 0;
            if (closestEnemy != null)
            {
                deltaDistance = GetDeltaDistance(context.Self, closestEnemy, action.Movement);
                float modifier = 5f;

                if (!_meleeClassSet.Contains(context.Self.GetCharacterClass()))
                {
                    if (GridExplorer._instance.ManhattanDistance(context.Self.GetCurrentTileIndex(), closestEnemy.GetCurrentTileIndex()) > context.Self.GetMovementPoints() + RANGED_CHARACTER_PREFERRED_RANGE)
                    {
                        modifier = -5f;
                    }
                }

                result -= deltaDistance * modifier;
            }
        }

        return result;
    }

    private Character FindClosestCharacter(AI_Context context, List<Character> characters)
    {
        Character result = null;

        if (characters == null || !characters.Any())
        {
            Debug.LogError($"AI_Evaluator.cs | CAN'T CALCULATE closest character for {context.Self.name}!");
            return null;
        }

        int min = int.MaxValue;
        foreach (var character in characters)
        {
            int distance = GridExplorer._instance.ManhattanDistance(context.Self.GetCurrentTileIndex(), character.GetCurrentTileIndex());
            if (distance < min && character != context.Self)
            {
                min = distance;
                result = character;
            }
        }

        return result;
    }

    private int GetDeltaDistance(Character a, Character b, CombatGridTile movement)
    {
        int currentDistance = GridExplorer._instance.ManhattanDistance(a.GetCurrentTileIndex(), b.GetCurrentTileIndex());
        int postMoveDistance = GridExplorer._instance.ManhattanDistance(movement.GetTileIndex(), b.GetCurrentTileIndex());
        return postMoveDistance - currentDistance;
    }

    private float EvaluateAbilityUsage(AI_Context context, AI_Action action)
    {
        float result = 0f;

        if (action.Ability == null)
        {
            return 0f;
        }

        switch (action.Ability.name)
        {
            // Barbarian
            case "Skullsplitter_Ability":       result += Skullsplitter_Ability(context, action); break;
            case "Earthquake_Ability":          result += Earthquake_Ability(context, action); break;
            case "RuptureOfTheWilds_Ability":   result += RuptureOfTheWilds_Ability(context, action); break;
            case "RoarOfTheAncients_Ability":   result += RoarOfTheAncients_Ability(context, action); break;

            // Bard
            case "InspiringAnthem_Ability":     result += InspiringAnthem_Ability(context, action); break;
            case "SongOfRenewal_Ability":       result += SongOfRenewal_Ability(context, action); break;
            case "DissonantChord_Ability":      result += DissonantChord_Ability(context, action); break;
            case "LuteSmash_Ability":           result += LuteSmash_Ability(context, action); break;

            // Rogue
            case "SandfangStrike_Ability":      result += SandfangStrike_Ability(context, action); break;
            case "Desert's Grasp_Ability":      result += DesertsGrasp_Ability(context, action); break;
            case "ThrowingKnives_Ability":      result += ThrowingKnives_Ability(context, action); break;
            case "VeilOfDust_Ability":          result += VeilOfDust_Ability(context, action); break;

            // Sorceress
            case "ArcaneBolt_Ability":          result += ArcaneBolt_Ability(context, action); break;
            case "FlameSurge_Ability":          result += FlameSurge_Ability(context, action); break;
            case "LightningStorm_Ability":      result += LightningStorm_Ability(context, action); break;
            case "Emberwake_Ability":           result += Emberwake_Ability(context, action); break;
        }

        return result;
    }

    private class OccupantData
    {
        public Character Character { get; private set; }
        public bool IsEnemy { get; private set; }
        public float PercentHP { get; private set; }
        public StatusEffectManager StatusEffectManager { get; private set; }

        public bool Exists => Character != null && PercentHP > 0f;

        public OccupantData(Character self, Character occupant)
        {
            Character = occupant;
            if (Character != null && self != null)
            {
                IsEnemy = Character.GetFaction() != self.GetFaction();
                PercentHP = Character.GetMaxHealth() == 0 ? 1f : Character.GetCurrentHealth() / Character.GetMaxHealth();
                StatusEffectManager = Character.GetStatusEffectManager();
            }
            else
            {
                IsEnemy = false;
                PercentHP = 1f;
                StatusEffectManager = null;
            }
        }

        public bool HasStatusEffect<T>() where T : StatusEffect
        {
            if (StatusEffectManager == null)
            {
                return false;
            }

            return StatusEffectManager.ContainsStatusEffect<T>();
        }
    }


    // Barbarian
    private float Skullsplitter_Ability(AI_Context context, AI_Action action)
    {
        float result = 0f;

        OccupantData occupant = AnalyzeOccupant(context, action.Target);
        if (occupant.Exists && occupant.IsEnemy)
        {
            result += USE_ABILITY_BONUS;
            result += 30f;

            if (occupant.PercentHP < 0.5f)
            {
                result += 100f;
            }

            if (occupant.HasStatusEffect<Stealth>())
            {
                result = float.NegativeInfinity;
            }
        }

        return result;
    }

    private float Earthquake_Ability(AI_Context context, AI_Action action)
    {
        float result = 0f;

        int hitCount = 0;
        DirectedAOEPattern.Direction direction = GetDirection(action.Movement, action.Target);
        _housePattern.SetDirection(direction);
        List<CombatGridTile> areaOfEffect = _housePattern.CalculateTilesToEffect(action.Target);

        foreach (var tile in areaOfEffect)
        {
            OccupantData occupant = AnalyzeOccupant(context, tile);
            if (occupant.Exists && occupant.IsEnemy)
            {
                hitCount++;
                result += 30f;

                if (occupant.PercentHP < 0.2f)
                {
                    result += 100f;
                }

                if (!occupant.HasStatusEffect<Slowed>())
                {
                    result += 10f;
                }
            }

            if (occupant.Exists && !occupant.IsEnemy)
            {
                hitCount--;
                result -= FRIENDLY_FIRE_AVOIDANCE;
            }
        }

        if (hitCount < 2)
        {
            result -= 30f;
        }

        if (hitCount > 0)
        {
            result += USE_ABILITY_BONUS;
        }

        if (hitCount == 0)
        {
            Debug.LogError("HIT COUNT == 0 FOR AOE SKILL");
            result = float.NegativeInfinity;
        }

        return result;
    }

    private float RuptureOfTheWilds_Ability(AI_Context context, AI_Action action)
    {
        float result = 0f;

        int hitCount = 0;
        DirectedAOEPattern.Direction direction = GetDirection(action.Movement, action.Target);
        _trisquarePattern.SetDirection(direction);
        List<CombatGridTile> areaOfEffect = _trisquarePattern.CalculateTilesToEffect(action.Target);

        foreach (var tile in areaOfEffect)
        {
            OccupantData occupant = AnalyzeOccupant(context, tile);
            if (occupant.Exists && occupant.IsEnemy)
            {
                hitCount++;
                result += 30f;

                if (occupant.PercentHP < 0.2f)
                {
                    result += 100f;
                }

                if (occupant.HasStatusEffect<Slowed>())
                {
                    result += 30f;
                }
            }

            if (occupant.Exists && !occupant.IsEnemy)
            {
                hitCount--;
                result -= FRIENDLY_FIRE_AVOIDANCE;
            }
        }

        if (hitCount > 0)
        {
            result += USE_ABILITY_BONUS;
        }

        if (hitCount == 0)
        {
            Debug.LogError("HIT COUNT == 0 FOR AOE SKILL");
            result = float.NegativeInfinity;
        }

        return result;
    }

    private float RoarOfTheAncients_Ability(AI_Context context, AI_Action action)
    {
        float result = 0f;

        int hitCount = 0;
        List<CombatGridTile> areaOfEffect = DiamondPattern(action.Target, 3);

        foreach (var tile in areaOfEffect)
        {
            OccupantData occupant = AnalyzeOccupant(context, tile);
            if (occupant.Exists && occupant.IsEnemy)
            {
                if (!occupant.HasStatusEffect<Slowed>())
                {
                    hitCount++;
                    result += 50f;
                }
            }
        }

        if (hitCount < 2)
        {
            result -= 50f;
        }

        if (hitCount > 0)
        {
            result += USE_ABILITY_BONUS;
        }

        if (hitCount == 0)
        {
            result = float.NegativeInfinity;
        }

        return result;
    }

    // Bard
    private float InspiringAnthem_Ability(AI_Context context, AI_Action action)
    {
        float result = 0f;

        int hitCount = 0;
        List<CombatGridTile> areaOfEffect = DiamondPattern(action.Target, 2);

        foreach (var tile in areaOfEffect)
        {
            OccupantData occupant = AnalyzeOccupant(context, tile);
            if (occupant.Exists && !occupant.IsEnemy)
            {
                if (!occupant.HasStatusEffect<Haste>())
                {
                    hitCount++;
                    result += 50f;
                }
            }
        }

        if (hitCount < 2)
        {
            result -= 50f;
        }

        if (hitCount > 0)
        {
            result += USE_ABILITY_BONUS;
        }

        if (hitCount == 0)
        {
            result = float.NegativeInfinity;
        }

        return result;
    }

    private float SongOfRenewal_Ability(AI_Context context, AI_Action action)
    {
        float result = 0f;

        int hitCount = 0;
        List<CombatGridTile> areaOfEffect = DiamondPattern(action.Target, 3);

        foreach (var tile in areaOfEffect)
        {
            OccupantData occupant = AnalyzeOccupant(context, tile);
            if (occupant.Exists && !occupant.IsEnemy)
            {
                if (occupant.PercentHP < 1f)
                {
                    hitCount++;
                    result += 50f / occupant.PercentHP;

                    if (tile == action.Target)
                    {
                        hitCount++;
                        result += 50f / occupant.PercentHP;
                    }
                }
            }
        }

        if (hitCount < 2)
        {
            result -= 50f;
        }

        if (hitCount > 0)
        {
            result += USE_ABILITY_BONUS;
        }

        if (hitCount == 0)
        {
            result = float.NegativeInfinity;
        }

        return result;
    }

    private float DissonantChord_Ability(AI_Context context, AI_Action action)
    {
        float result = 0f;

        int hitCount = 0;
        List<CombatGridTile> areaOfEffect = DiamondPattern(action.Target, 2);

        foreach (var tile in areaOfEffect)
        {
            OccupantData occupant = AnalyzeOccupant(context, tile);
            if (occupant.Exists && occupant.IsEnemy)
            {
                if (occupant.HasStatusEffect<Haste>())
                {
                    result += DISPEL_BUFFS_INCLINATION;
                    hitCount++;
                }

                if (occupant.HasStatusEffect<Empowered>())
                {
                    result += DISPEL_BUFFS_INCLINATION;
                    hitCount++;
                }

                if (occupant.HasStatusEffect<Emberwake>())
                {
                    result += DISPEL_BUFFS_INCLINATION;
                    hitCount++;
                }

                if (occupant.HasStatusEffect<Enraged>())
                {
                    result += DISPEL_BUFFS_INCLINATION;
                    hitCount++;
                }

                if (occupant.HasStatusEffect<ConduitOfPower>())
                {
                    result += DISPEL_BUFFS_INCLINATION;
                    hitCount++;
                }

                if (occupant.HasStatusEffect<Fortified>())
                {
                    result += DISPEL_BUFFS_INCLINATION;
                    hitCount++;
                }

                if (occupant.HasStatusEffect<Sanctified>())
                {
                    result += DISPEL_BUFFS_INCLINATION;
                    hitCount++;
                }

                if (occupant.HasStatusEffect<Stealth>())
                {
                    result += DISPEL_BUFFS_INCLINATION;
                    hitCount++;
                }
            }
        }

        if (hitCount < 2)
        {
            result -= DISPEL_BUFFS_INCLINATION;
        }

        if (hitCount > 0)
        {
            result += USE_ABILITY_BONUS;
        }

        if (hitCount == 0)
        {
            result = float.NegativeInfinity;
        }

        return result;
    }

    private float LuteSmash_Ability(AI_Context context, AI_Action action) // TO-DO: Byt ut mot nya abilityn.
    {
        // Deprecated
        return 0f;
    }

    // Rogue
    private float SandfangStrike_Ability(AI_Context context, AI_Action action)
    {
        float result = 0f;

        OccupantData occupant = AnalyzeOccupant(context, action.Target);
        if (occupant.Exists && occupant.IsEnemy)
        {
            result += USE_ABILITY_BONUS;
            result += 30f;

            if (occupant.PercentHP < 0.2f)
            {
                result += 100f;
            }

            if (occupant.HasStatusEffect<Poison>())
            {
                result += PREFER_POISONED_TARGETS;
            }

            if (!_meleeClassSet.Contains(occupant.Character.GetCharacterClass()))
            {
                result += PREFER_UNARMORED_TARGETS;
            }

            if (occupant.HasStatusEffect<Stealth>())
            {
                result = float.NegativeInfinity;
            }
        }

        return result;
    }

    private float DesertsGrasp_Ability(AI_Context context, AI_Action action)
    {
        float result = 0f;

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
            GameObject obj = CombatGrid._instance.GetTileAtCoord(action.Target.GetTileIndex().x + dir.x, action.Target.GetTileIndex().y + dir.y);
            CombatGridTile tile = obj.GetComponent<CombatGridTile>();
            if (tile != null)
            {
                OccupantData occupant = AnalyzeOccupant(context, tile);
                if (occupant.Exists && occupant.IsEnemy)
                {
                    hitCount++;
                    result += 30f;

                    if (occupant.PercentHP < 0.2f)
                    {
                        result += 100f;
                    }

                    if (occupant.HasStatusEffect<Poison>())
                    {
                        result += PREFER_POISONED_TARGETS;
                    }

                    if (!_meleeClassSet.Contains(occupant.Character.GetCharacterClass()))
                    {
                        result += PREFER_UNARMORED_TARGETS;
                    }
                }

                if (occupant.Exists && !occupant.IsEnemy)
                {
                    hitCount--;
                    result -= FRIENDLY_FIRE_AVOIDANCE;
                }
            }
        }

        if (hitCount < 2)
        {
            result -= 30f;
        }

        if (hitCount > 0)
        {
            result += USE_ABILITY_BONUS;
        }

        if (hitCount == 0)
        {
            result = float.NegativeInfinity;
        }

        return result;
    }

    private float ThrowingKnives_Ability(AI_Context context, AI_Action action)
    {
        float result = 0f;

        int hitCount = 0;
        DirectedAOEPattern.Direction direction = GetDirection(action.Movement, action.Target);
        _linePattern.SetDirection(direction);
        List<CombatGridTile> areaOfEffect = _linePattern.CalculateTilesToEffect(action.Target);

        foreach (var tile in areaOfEffect)
        {
            OccupantData occupant = AnalyzeOccupant(context, tile);
            if (occupant.Exists && occupant.IsEnemy)
            {
                hitCount++;
                result += 30f;

                if (occupant.PercentHP < 0.2f)
                {
                    result += 100f;
                }

                if (occupant.HasStatusEffect<Poison>())
                {
                    result += PREFER_POISONED_TARGETS;
                }

                if (!_meleeClassSet.Contains(occupant.Character.GetCharacterClass()))
                {
                    result += PREFER_UNARMORED_TARGETS;
                }
            }

            if (occupant.Exists && !occupant.IsEnemy)
            {
                hitCount--;
                result -= FRIENDLY_FIRE_AVOIDANCE;
            }
        }

        if (hitCount < 2)
        {
            result -= 30f;
        }

        if (hitCount > 0)
        {
            result += USE_ABILITY_BONUS;
        }

        if (hitCount == 0)
        {
            result = float.NegativeInfinity;
        }

        return result;
    }

    private float VeilOfDust_Ability(AI_Context context, AI_Action action)
    {
        float result = 0f;

        int hitCount = 0;
        if (!context.StatusEffectManager.ContainsStatusEffect<Stealth>())
        {
            if (Random.Range(0f, 1f) > PROACTIVE_STEALTH_CHANCE)
            {
                hitCount++;
                result += 30f;
            }

            if (context.StatusEffectManager.ContainsStatusEffect<Poison>())
            {
                result += 30f;
                hitCount++;
            }

            if (context.StatusEffectManager.ContainsStatusEffect<Burn>())
            {
                result += 30f;
                hitCount++;
            }

            if (context.StatusEffectManager.ContainsStatusEffect<Aftershock>())
            {
                result += 30f;
                hitCount++;
            }

            if (context.StatusEffectManager.ContainsStatusEffect<Weakened>())
            {
                result += 30f;
                hitCount++;
            }

            if (context.StatusEffectManager.ContainsStatusEffect<Slowed>())
            {
                result += 30f;
                hitCount++;
            }
        }

        if (hitCount < 2)
        {
            result -= 30f;
        }

        if (hitCount > 0)
        {
            result += USE_ABILITY_BONUS;
        }

        if (hitCount == 0)
        {
            result = float.NegativeInfinity;
        }

        return result;
    }

    // Sorceress
    private float ArcaneBolt_Ability(AI_Context context, AI_Action action)
    {
        float result = 0f;

        OccupantData occupant = AnalyzeOccupant(context, action.Target);
        if (occupant.Exists && occupant.IsEnemy)
        {
            result += USE_ABILITY_BONUS;
            result += 30f;

            if (occupant.PercentHP < 0.2f)
            {
                result += 100f;
            }

            if (occupant.HasStatusEffect<Stealth>())
            {
                result = float.NegativeInfinity;
            }
        }

        return result;
    }

    private float FlameSurge_Ability(AI_Context context, AI_Action action)
    {
        float result = 0f;

        int hitCount = 0;
        DirectedAOEPattern.Direction direction = GetDirection(action.Movement, action.Target);
        _flamePattern.SetDirection(direction);
        List<CombatGridTile> areaOfEffect = _flamePattern.CalculateTilesToEffect(action.Target);

        foreach (var tile in areaOfEffect)
        {
            OccupantData occupant = AnalyzeOccupant(context, tile);
            if (occupant.Exists && occupant.IsEnemy)
            {
                hitCount++;
                result += 30f;

                if (occupant.PercentHP < 0.2f)
                {
                    result += 100f;
                }
            }

            if (occupant.Exists && !occupant.IsEnemy)
            {
                hitCount--;
                result -= FRIENDLY_FIRE_AVOIDANCE;
            }
        }

        if (hitCount < 2)
        {
            result -= 30f;
        }

        if (hitCount > 0)
        {
            result += USE_ABILITY_BONUS;
        }

        if (hitCount == 0)
        {
            result = float.NegativeInfinity;
        }

        return result;
    }

    private float LightningStorm_Ability(AI_Context context, AI_Action action)
    {
        float result = 0f;

        int hitCount = 0;
        List<CombatGridTile> areaOfEffect = DiamondPattern(action.Target, 2);

        foreach (var tile in areaOfEffect)
        {
            OccupantData occupant = AnalyzeOccupant(context, tile);
            if (occupant.Exists && occupant.IsEnemy)
            {
                hitCount++;
                result += 30f;

                if (occupant.PercentHP < 0.2f)
                {
                    result += 100f;
                }
            }

            if (occupant.Exists && !occupant.IsEnemy)
            {
                hitCount--;
                result -= FRIENDLY_FIRE_AVOIDANCE;
            }
        }

        if (hitCount < 2)
        {
            result -= 30f;
        }

        if (hitCount > 0)
        {
            result += USE_ABILITY_BONUS;
        }

        if (hitCount == 0)
        {
            result = float.NegativeInfinity;
        }

        return result;
    }

    private float Emberwake_Ability(AI_Context context, AI_Action action)
    {
        float result = 0f;

        if (!context.StatusEffectManager.ContainsStatusEffect<Emberwake>())
        {
            result += USE_ABILITY_BONUS;

            foreach (var cooldownCheck in context.Self.GetAvailableAbilities())
            {
                if (cooldownCheck is LightningStorm_Ability && !context.Self.IsAbilityCooldownActive(cooldownCheck))
                {
                    result += 30f;
                }

                if (cooldownCheck is FlameSurge_Ability && !context.Self.IsAbilityCooldownActive(cooldownCheck))
                {
                    result += 30f;
                }
            }
        }
        else
        {
            result = float.NegativeInfinity;
        }

        return result;
    }

    private OccupantData AnalyzeOccupant(AI_Context context, CombatGridTile target)
    {
        return new OccupantData(context.Self, target?.GetOccupantCharacter());
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

    private void PenalizeMovementOnlyActions(List<AI_Action> actions)
    {
        float required = 10f;
        float penalty = 50f;

        var actionsByTile = actions.GroupBy(a => a.Movement);

        foreach (var group in actionsByTile)
        {
            var movementOnly = group
                .Where(a => a.Ability == null && !float.IsNegativeInfinity(a.Score))
                .ToList();

            if (!movementOnly.Any())
                continue;

            float bestAbilityScore = group
                .Where(a => a.Ability != null)
                .Select(a => a.Score)
                .DefaultIfEmpty(float.NegativeInfinity)
                .Max();

            foreach (var action in movementOnly)
            {
                if (bestAbilityScore > action.Score + required)
                {
                    action.Score -= penalty;
                }
            }
        }
    }

    private AI_Action SelectAction(AI_Context context, List<AI_Action> actions)
    {
        var validActions = actions
            .Where(a => !float.IsNegativeInfinity(a.Score) && !float.IsNaN(a.Score))
            .ToList();

        Debug.Log($"AI_Evaluator.cs | Found {validActions.Count} worthwhile actions out of {actions.Count} total for {context.Self.name}.");
        
        foreach (var action in validActions)
        {
            AI_Core.PrintAction(action);
        }

        var topActions = validActions
            .OrderByDescending(a => a.Score)
            .Take(TOP_N_ACTIONS)
            .ToList();

        if (topActions.Count == 0)
        {
            Debug.LogError($"AI_Evaluator.cs | NO VALID ACTIONS FOUND for {context.Self.name}! Defaulting to standing still.");
            return new AI_Action { Movement = context.Self.GetCurrentTileComponent() };
        }

        return topActions[Random.Range(0, topActions.Count)];
    }
}

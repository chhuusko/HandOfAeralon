// Joel Larsson Wendt | jola6902

using System.Collections.Generic;

public class AI_Searcher
{
    private static readonly HashSet<string> _selfCastSet = new()
    {
        "RoarOfTheAncients_Ability",
        "Desert's Grasp_Ability",
        "VeilOfDust_Ability",
        "Emberwake_Ability"
    };

    public List<AI_Action> GetPossibleActions(AI_Context context)
    {
        List<AI_Action> result = new();

        foreach (var tile in context.ReachableTiles)
        {
            result.Add(new AI_Action { Movement = tile });

            foreach (var ability in context.AvailableAbilities)
            {
                if (_selfCastSet.Contains(ability.name))
                {
                    result.Add(new AI_Action
                    {
                        Movement = tile,
                        Ability = ability,
                        Target = tile
                    });
                    continue;
                }

                List<CombatGridTile> targets =
                    FindPossibleTargets(tile, ability, context.AbilityHandler);

                foreach (var target in targets)
                {
                    if (!context.AbilityHandler
                        .IsValidTargetTileForAbility(ability, target))
                        continue;

                    result.Add(new AI_Action
                    {
                        Movement = tile,
                        Ability = ability,
                        Target = target
                    });
                }
            }
        }

        return result;
    }

    private List<CombatGridTile> FindPossibleTargets(
        CombatGridTile tile,
        Ability ability,
        AbilityHandler abilityHandler)
    {
        abilityHandler.SetPendingAbility(ability);
        abilityHandler.CalculateAbilityRange(tile);
        return abilityHandler.GetTilesInRange();
    }
}

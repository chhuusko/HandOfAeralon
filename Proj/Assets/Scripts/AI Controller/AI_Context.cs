// Joel Larsson Wendt | jola6902

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AI_Context
{
    public Character Self { get; private set; }
    public float PercentHP { get; private set; }
    public Faction Faction { get; private set; }
    public List<Character> Enemies { get; private set; }
    public List<Character> Allies { get; private set; }
    public List<CombatGridTile> ReachableTiles { get; private set; }
    public AbilityHandler AbilityHandler { get; private set; }
    public List<Ability> AvailableAbilities { get; private set; }
    public StatusEffectManager StatusEffectManager { get; private set; }

    public AI_Context(Character character)
    {
        Self = character;
        PercentHP = character.GetMaxHealth() == 0 ? 1f : character.GetCurrentHealth() / character.GetMaxHealth();
        Faction = character.GetFaction();
        Enemies = GetEnemies();
        Allies = GetAllies();
        ReachableTiles = GetReachableTiles(character);
        AbilityHandler = character.GetAbilityHandler();
        AvailableAbilities = GetAvailableAbilities(character);
        StatusEffectManager = character.GetStatusEffectManager();
    }

    private List<Character> GetEnemies()
    {
        List<GameObject> result = Faction == Faction.Enemy
            ? CombatGrid._instance.GetAllFriendlyCharacters()
            : CombatGrid._instance.GetAllEnemyCharacters();

        return result
            .Select(go => go.GetComponent<Character>())
            .Where(c => c != null && c != Self)
            .ToList();
    }

    private List<Character> GetAllies()
    {
        List<GameObject> result = Faction == Faction.Enemy
            ? CombatGrid._instance.GetAllEnemyCharacters()
            : CombatGrid._instance.GetAllFriendlyCharacters();

        return result
            .Select(go => go.GetComponent<Character>())
            .Where(c => c != null && c != Self)
            .ToList();
    }

    private List<CombatGridTile> GetReachableTiles(Character character)
    {
        var result = GridExplorer._instance
            .GetReachableTilesWithMovement(Self.GetCurrentTileComponent().gameObject, Self.GetMovementPoints())
            .Select(obj => obj.GetComponent<CombatGridTile>())
            .Where(cgt => cgt != null)
            .ToList();

        result.Add(Self.GetCurrentTileComponent());

        return result;
    }

    private List<Ability> GetAvailableAbilities(Character character)
    {
        List<Ability> result = new();

        foreach (var ability in character.GetAvailableAbilities())
        {
            if (!character.IsAbilityCooldownActive(ability))
            {
                result.Add(ability);
            }
        }

        //Debug.Log($"AI_Context.cs | Found {result.Count} abilities ready to use!");

        return result;
    }
}

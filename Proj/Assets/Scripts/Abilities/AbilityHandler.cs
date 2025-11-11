using System.Collections.Generic;
using UnityEngine;

public class AbilityHandler : MonoBehaviour
{
    [SerializeField] private List<Ability> _abilities;
    [SerializeField] private CombatGrid _combatGrid;

    List<CombatGridTile> _availableAbilityTargets;

    Character characterCaster;

    private void Start()
    {
        if(TryGetComponent<Character>(out var character)){
            characterCaster = character;
        }
    }
    public void UseAbility(Ability ability, CombatGridTile targetTile)
    {
        if (!CanCastAbility(targetTile))
        {
            Debug.Log("Tried casting ability on inaccaptable target.");
            return;
        }

        //ability.RunAbility(characterCaster.currentTile, targetTile);
    }

    private bool CanCastAbility(CombatGridTile targetTile)
    {
        return _availableAbilityTargets.Contains(targetTile);
    }

    private void CheckAbilityTargets(Ability ability)
    {
       // _availableAbilityTargets = ability.targetingPattern.GetVaildTiles(characterCaster.currentTile)
    }
}

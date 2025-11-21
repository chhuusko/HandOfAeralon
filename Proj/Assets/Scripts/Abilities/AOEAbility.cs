using System.Collections.Generic;
using UnityEngine;

    public abstract class AOEAbility : Ability
    {
        [Header("- Type Specific values - ")]
        [SerializeField] protected AOEPattern _pattern;
        public override void RunAbility(CombatGridTile casterTile, CombatGridTile targetTile)
        {
            // Calculate all tiles around with in radius and apply effect to all of them.
            List<CombatGridTile> tilesToEffect = _pattern.CalculateTilesToEffect(targetTile);

            foreach (CombatGridTile tile in tilesToEffect)
            {
                if (tile != null)
                {
                    ApplyEffectOnTile(casterTile, tile);
                }
            }
        }
        public override List<CombatGridTile> GetTilesToEffect(CombatGridTile tile)
        {
            return _pattern.CalculateTilesToEffect(tile);
        }
    }


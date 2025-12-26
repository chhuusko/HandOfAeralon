using UnityEngine;

[CreateAssetMenu(fileName = "Coordinated Shift", menuName = "Item/Card Data/Coordinated Shift", order = 1)]
public class CoordinatedShift : Card
{
    public override void PlayCardOnTarget(Character character)
    {
        Character activeCharacter = CombatManager._instance.GetCombatTurnOrder().GetActiveCharacter();
        CombatGridTile targetTile = character.GetCurrentTileComponent();

        if (activeCharacter && activeCharacter.GetFaction() == Faction.Friendly)
        {
            if (targetTile)
            {
                Vector2Int tileIndex = targetTile.GetTileIndex();
                Vector3 tilePosition = targetTile.GetTilePosition();

                if (CombatGrid._instance.ContainsCharacter(activeCharacter.gameObject))
                {

                    character.gameObject.transform.position = activeCharacter.GetCurrentTileComponent().GetTilePosition();
                    character.SetCurrentTileIndex(activeCharacter.GetCurrentTileComponent().GetTileIndex());

                    activeCharacter.gameObject.transform.position = tilePosition;
                    activeCharacter.SetCurrentTileIndex(tileIndex);

                    

                }

            }
            else
            {
                //DebugLog.CJLog("Show ERROR UI to place on a deploy tile.");
            }
        }
        character.GetCurrentTileComponent().SetOccupant(activeCharacter.gameObject);
        activeCharacter.GetCurrentTileComponent().SetOccupant(character.gameObject);
        Debug.LogWarning(activeCharacter.GetCurrentTileIndex());
    }
        
}

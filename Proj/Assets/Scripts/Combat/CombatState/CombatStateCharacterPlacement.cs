using UnityEngine;
using UnityEngine.EventSystems;

public class CombatStateCharacterPlacement : CombatStateBase
{
    public override CombatState _state => CombatState.PlaceCharacters;
    private Selector _selector;

    public CombatStateCharacterPlacement(Selector selector)
    {
        _selector = selector;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        if (_selector)
        {
            // Return early if mouse is over UI element.
            if (EventSystem.current.IsPointerOverGameObject()) return;
            _selector.SetCurrentState(SelectorState.PlacingCharacters);

            _selector.UpdateTileColors(CombatGrid._instance.GetAllTiles());

            CombatGridTile unoccupiedDeployTile = _selector.GetUnoccupiedDeployTileClicked();
            Character selectedCharacter = _selector.GetSelectedCharacter();

            if (selectedCharacter)
            {
                if (unoccupiedDeployTile)
                {
                    Vector2Int tileIndex = unoccupiedDeployTile.GetTileIndex();
                    Vector3 tilePosition = unoccupiedDeployTile.GetTilePosition();

                    if (CombatGrid._instance.ContainsCharacter(selectedCharacter.gameObject))
                    {
                        selectedCharacter.gameObject.transform.position = tilePosition;
                        selectedCharacter.SetCurrentTileIndex(tileIndex);
                    }
                    else
                    {
                        CombatGridCharacterData characterData = new CombatGridCharacterData(CharacterClass.Wizard,
                                                                                            Faction.Friendly,
                                                                                            10,
                                                                                            1,
                                                                                            tileIndex,
                                                                                            tilePosition,
                                                                                            Vector3.one,
                                                                                            Quaternion.identity);
                        CombatGrid._instance.SpawnCharacter(characterData);
                    }
                }
                else
                {
                    DebugLog.CJLog("Show ERROR UI to place on a deploy tile.");
                }
            }
        }
    }
}

using UnityEngine;


[System.Serializable]
public class CombatStateLoadLevel : CombatStateBase
{
    public override CombatState _state => CombatState.LoadCombatLevel;

    public override void Enter()
    {
        base.Enter();
        CombatGrid._instance.LoadNextLevel();

        // NOTE (Calle): Spawning two test characters
        Vector2Int tileIndex = new Vector2Int(5, 0);
        Vector3 position = new Vector3(1.0f + tileIndex.x * 2.0f, 0.0f, 1.0f + tileIndex.y * 2.0f);
        CombatGridCharacterData characterData = new CombatGridCharacterData(CharacterClass.Wizard,
                                                                           Faction.Friendly,
                                                                           10,
                                                                           1,
                                                                           tileIndex,
                                                                           position,
                                                                           Vector3.one,
                                                                           Quaternion.identity);

        CombatGrid._instance.SpawnCharacter(characterData);
        tileIndex.x = 6;
        position.x += 2.0f;
        characterData.SetCurrentTileIndex(tileIndex);
        characterData.SetPosition(position);
        CombatGrid._instance.SpawnCharacter(characterData);
        
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        if(CombatGrid._instance.IsCombatGridLoaded())
        {
            CombatManager._instance.ChangeCombatState(new CombatStateIntroCinematic(CombatManager._instance.GetCombatCamera()));
        }
    }
}

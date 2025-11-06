using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CombatGrid
{
    List<CombatGridTile> combatGrid = new List<CombatGridTile>();
}

[System.Serializable]
public enum CombatState
{
    IntroCinematic,
    LoadCombatLevel,
    PlaceCharacters,
    MakeTurn,
    EndTurn,
    EndCombat

};

[System.Serializable]
public enum CombatTurn
{
    PlayerTurn,
    EnemyTurn
};

public class CombatManager : MonoBehaviour
{
    [SerializeField] private CombatState combatState;
    [SerializeField] private CombatTurn currentTurn;

    [SerializeField] private bool combatGridLoaded = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        combatState = CombatState.IntroCinematic;
    }

    // Update is called once per frame
    void Update()
    {
        switch(combatState)
        {
            case CombatState.IntroCinematic:
                {
                    HandleIntroCinematic();
                } break;
            case CombatState.LoadCombatLevel:
                {
                    HandleLoadCombatLevel();
                } break;
            case CombatState.PlaceCharacters:
                {
                    HandlePlaceCharacters();
                } break;
            case CombatState.MakeTurn:
                {
                    HandleMakeTurn();
                } break;
            case CombatState.EndTurn:
                {
                    HandleEndTurn();
                } break;
            case CombatState.EndCombat:
                {
                    HandleEndCombat();
                } break;
        }
    }


    private void HandleMakeTurn()
    {
        switch(currentTurn)
        {
            case CombatTurn.PlayerTurn:
                HandlePlayerTurn();
                break;
            case CombatTurn.EnemyTurn:
                HandleEnemyTurn();
                break;
        }
    }

    private void HandleIntroCinematic()
    {

    }

    private void HandleLoadCombatLevel()
    {
        if(!combatGridLoaded)
        {
            combatGridLoaded = true;
            LoadNextLevel();
        }
    }

    private void HandlePlaceCharacters()
    {

    }
    private void HandleEndTurn()
    {

    }

    private void HandlePlayerTurn()
    {

    }

    private void HandleEnemyTurn()
    {

    }

    private void HandleEndCombat()
    {

    }

    private void LoadNextLevel()
    {
        
    }

    private void EvaluateInitiativeOrder()
    {

    }
}

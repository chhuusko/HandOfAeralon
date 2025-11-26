using UnityEngine;

[System.Serializable]
public enum CombatState
{
    IntroCinematic,
    LoadCombatLevel,
    PlaceCharacters,
    TakeTurn,
    EndTurn,
    EndCombat
};

public interface ICombatState
{
    CombatState _state {  get; }
    void Enter();
    void Update();
    void Exit();

}

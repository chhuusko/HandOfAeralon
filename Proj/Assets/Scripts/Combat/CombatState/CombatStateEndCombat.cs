using UnityEngine;

public class CombatStateEndCombat : CombatStateBase
{
    public override CombatState _state => CombatState.EndCombat;
    [SerializeField] private bool _playerWon;

    public CombatStateEndCombat(bool playerWon)
    {
        _playerWon = playerWon;
    }

    public override void Enter()
    {
        base.Enter();
        CombatEventManager.InvokeEnterCombatStateEndCombat(_playerWon);
        if (!_playerWon)
            LevelManager.GetInstance().RestartGame();
            
        CombatMenuManager.GetInstance().OnGoToShopButtonPressed += OnGoToShop;
    }

    public override void Exit()
    {
        base.Exit();
        
    }

    public override void Update()
    {
        
    }

    private void OnGoToShop()
    {
        CombatEventManager.InvokeExitCombatStateEndCombat(_playerWon);
    }
}

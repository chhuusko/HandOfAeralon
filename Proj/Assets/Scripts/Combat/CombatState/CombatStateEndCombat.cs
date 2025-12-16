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
        CombatMenuManager.GetInstance().OnGoToShopButtonPressed += OnGoToShop;
        CombatMenuManager.GetInstance().OnGoToMainMenuPressed   += OnGoToMainMenu;
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

    private void OnGoToMainMenu()
    {
        CombatEventManager.InvokeExitCombatStateEndCombat(false);
    }
}

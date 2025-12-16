using UnityEngine;

[CreateAssetMenu(fileName = "Reapers Ledger", menuName = "Item/Card Data/Reapers Ledger", order = 1)]
public class ReapersLedger : Card
{
    public override void PlayCardOnTarget(Character character)
    {
        if (character != null)
        {
            character.TakeDamage(10 + (GlobalGameManager.GetInstance().GetGameData().reapersLedgerKills*5));
            if (character.GetCurrentHealth() <= 0)
            {
                GlobalGameManager.GetInstance().ReapersLedgerKillChange(1);
            }
        }
    }
}

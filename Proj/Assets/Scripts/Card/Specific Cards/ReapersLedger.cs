using UnityEngine;

[CreateAssetMenu(fileName = "Reapers Ledger", menuName = "Item/Card Data/Reapers Ledger", order = 1)]
public class ReapersLedger : Card
{
    public override void PlayCard()
    {
        Character targetCharacter = Selector._instance.GetTileUnderMouse().GetOccupantCharacter();
        if (targetCharacter != null)
        {
            targetCharacter.TakeDamage(15 + (GlobalGameManager.GetInstance().GetGameData().reapersLedgerKills*5));
            if (targetCharacter.GetCurrentHealth() <= 0)
            {
                GlobalGameManager.GetInstance().ReapersLedgerKillChange(1);
            }
        }
    }
}

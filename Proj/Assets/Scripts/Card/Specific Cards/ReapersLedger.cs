using UnityEngine;

[CreateAssetMenu(fileName = "Reapers Ledger", menuName = "Item/Card Data/Reapers Ledger", order = 1)]
public class ReapersLedger : Card
{
    public override void PlayCardOnTarget(Character character)
    {
        if (character != null)
        {
            
            character.TakeDamage(GetDamage(character));
            if (character.GetCurrentHealth() <= 0)
            {
                GlobalGameManager.GetInstance().ReapersLedgerKillChange(1);
            }
        }
    }
    public override int GetDamage(Character character)
    {
        damage = 25 + (GlobalGameManager.GetInstance().GetGameData().reapersLedgerKills * 25);
        return Mathf.RoundToInt(character.GetStatusEffectManager().ModifyIncomingDamage(damage, null));

    }
}
